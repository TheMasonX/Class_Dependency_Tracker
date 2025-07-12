/*
 * MathConverter and accompanying samples are copyright (c) 2011 by Ivan Krivyakov
 * ivan [at] ikriv.com
 * They are distributed under the Apache License http://www.apache.org/licenses/LICENSE-2.0.html
 */
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace ClassDependencyTracker.Utils.Converters;

/// <summary>
/// Value converter that performs arithmetic calculations over its argument(s)
/// </summary>
/// <remarks>
/// MathConverter can act as a value converter, or as a multivalue converter (WPF only).
/// It is also a markup extension (WPF only) which allows to avoid declaring resources,
/// ConverterParameter must contain an arithmetic expression over converter arguments. Operations supported are +, -, * and /
/// Single argument of a value converter may referred as x, a, or {0}
/// Arguments of multi value converter may be referred as x,y,z,t (first-fourth argument), or a,b,c,d, or {0}, {1}, {2}, {3}, {4}, ...
/// The converter supports arithmetic expressions of arbitrary complexity, including nested subexpressions
/// </remarks>
public partial class MathConverter :
#if !SILVERLIGHT
    MarkupExtension,
    IMultiValueConverter,
#endif
    IValueConverter
{
    Dictionary<string, IExpression> _storedExpressions = [];

    public object Convert (object value, Type targetType, object parameter, CultureInfo culture)
    {
        return Convert([value], targetType, parameter, culture);
    }

    public object ConvertBack (object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }

    public object Convert (object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        try
        {
            decimal result = Parse(parameter.ToString()!).Evaluate(values);

            if (targetType == typeof(decimal))
            {
                return result;
            }

            if (targetType == typeof(string))
            {
                return result.ToString();
            }

            if (targetType == typeof(int))
            {
                return (int)result;
            }

            if (targetType == typeof(double))
            {
                return (double)result;
            }

            if (targetType == typeof(long))
            {
                return (long)result;
            }

            throw new ArgumentException($"Unsupported target type {targetType.FullName}");
        }
        catch (Exception ex)
        {
            ProcessException(ex);
        }

        return DependencyProperty.UnsetValue;
    }

    public object[] ConvertBack (object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }

#if !SILVERLIGHT
    public override object ProvideValue (IServiceProvider serviceProvider)
    {
        return this;
    }
#endif

    protected virtual void ProcessException (Exception ex)
    {
        Trace.WriteLine(ex.Message);
    }

    private IExpression Parse (string s)
    {
        if (!_storedExpressions.TryGetValue(s, out IExpression? result))
        {
            result = new Parser().Parse(s);
            _storedExpressions[s] = result;
        }

        return result;
    }

    interface IExpression
    {
        decimal Evaluate(object[] args);
    }

    class Constant : IExpression
    {
        private readonly decimal _value;

        public Constant (string text)
        {
            if (!decimal.TryParse(text, out _value))
            {
                throw new ArgumentException($"'{text}' is not a valid number");
            }
        }

        public decimal Evaluate (object[] args) => _value;
    }

    class Variable : IExpression
    {
        private readonly int _index;

        public Variable (string text)
        {
            if (!int.TryParse(text, out _index) || _index < 0)
            {
                throw new ArgumentException($"'{text}' is not a valid parameter index");
            }
        }

        public Variable (int n)
        {
            _index = n;
        }

        public decimal Evaluate (object[] args)
        {
            if (_index >= args.Length)
            {
                throw new ArgumentException($"MathConverter: parameter index {_index} is out of range. {args.Length} parameter(s) supplied");
            }

            return System.Convert.ToDecimal(args[_index]);
        }
    }

    class BinaryOperation (char operation, IExpression left, IExpression right) : IExpression
    {
        private readonly Func<decimal, decimal, decimal> _operation = operation switch
        {
            '+' => (a, b) => (a + b),
            '-' => (a, b) => (a - b),
            '*' => (a, b) => (a * b),
            '/' => (a, b) => (a / b),
            _ => throw new ArgumentException($"Invalid operation: {operation}"),
        };
        private readonly IExpression _left = left;
        private readonly IExpression _right = right;

        public decimal Evaluate (object[] args)
        {
            return _operation(_left.Evaluate(args), _right.Evaluate(args));
        }
    }

    class Negate (IExpression param) : IExpression
    {
        private readonly IExpression _param = param;

        public decimal Evaluate (object[] args) => -_param.Evaluate(args);
    }

    partial class Parser
    {
        private string text = "";
        private int pos;

        [GeneratedRegex(@"(\d+\.?\d*|\d*\.?\d+)")]
        private static partial Regex decimalRegex();

        /// <summary>
        /// Parse the expression text into an <see cref="IExpression"/>
        /// </summary>
        /// <param name="text">Text containing a mathematical expression representable by an <see cref="IExpression"/></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException">Throws if the expression fails to parse</exception>
        public IExpression Parse (string text)
        {
            try
            {
                pos = 0;
                this.text = text;
                var result = ParseExpression();
                RequireEndOfText();
                return result;
            }
            catch (Exception ex)
            {
                string msg = $"MathConverter: error parsing expression '{text}'. {ex.Message} at position {pos}";
                throw new ArgumentException(msg, ex);
            }
        }

        private IExpression ParseExpression ()
        {
            IExpression left = ParseTerm();

            while (true)
            {
                if (pos >= text.Length)
                {
                    return left;
                }

                var c = text[pos];

                if (c == '+' || c == '-')
                {
                    ++pos;
                    IExpression right = ParseTerm();
                    left = new BinaryOperation(c, left, right);
                }
                else
                {
                    return left;
                }
            }
        }

        private IExpression ParseTerm ()
        {
            IExpression left = ParseFactor();

            while (true)
            {
                if (pos >= text.Length)
                {
                    return left;
                }

                var c = text[pos];

                if (c == '*' || c == '/')
                {
                    ++pos;
                    IExpression right = ParseFactor();
                    left = new BinaryOperation(c, left, right);
                }
                else
                {
                    return left;
                }
            }
        }

        private IExpression ParseFactor ()
        {
            SkipWhiteSpace();
            if (pos >= text.Length)
            {
                throw new ArgumentException("Unexpected end of text");
            }

            var c = text[pos];

            switch (c)
            {
                case '+':
                    ++pos;
                    return ParseFactor();
                case '-':
                    ++pos;
                    return new Negate(ParseFactor());
                case 'x':
                case 'a':
                    return CreateVariable(0);
                case 'y':
                case 'b':
                    return CreateVariable(1);
                case 'z':
                case 'c':
                    return CreateVariable(2);
                case 't':
                case 'd':
                    return CreateVariable(3);
                case '(':
                    {
                        ++pos;
                        var expression = ParseExpression();
                        SkipWhiteSpace();
                        Require(')');
                        SkipWhiteSpace();
                        return expression;
                    }

                case '{':
                    {
                        ++pos;
                        var end = text.IndexOf('}', pos);
                        if (end < 0) { --pos; throw new ArgumentException("Unmatched '{'"); }
                        if (end == pos) { throw new ArgumentException("Missing parameter index after '{'"); }
                        var result = new Variable(text[pos..end].Trim());
                        pos = end + 1;
                        SkipWhiteSpace();
                        return result;
                    }
            }

            var match = decimalRegex().Match(text[pos..]);
            if (match.Success)
            {
                pos += match.Length;
                SkipWhiteSpace();
                return new Constant(match.Value);
            }
            else
            {
                throw new ArgumentException($"Unexpected character '{c}'");
            }
        }

        private IExpression CreateVariable (int n)
        {
            ++pos;
            SkipWhiteSpace();
            return new Variable(n);
        }

        private void SkipWhiteSpace ()
        {
            while (pos < text.Length && char.IsWhiteSpace((text[pos])))
            {
                ++pos;
            }
        }

        private void Require (char c)
        {
            if (pos >= text.Length || text[pos] != c)
            {
                throw new ArgumentException("Expected '" + c + "'");
            }

            ++pos;
        }

        private void RequireEndOfText ()
        {
            if (pos != text.Length)
            {
                throw new ArgumentException("Unexpected character '" + text[pos] + "'");
            }
        }
    }
}