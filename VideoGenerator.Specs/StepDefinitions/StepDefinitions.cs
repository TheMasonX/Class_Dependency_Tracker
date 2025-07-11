using TechTalk.SpecFlow;

using VideoGenerator.Specs.Drivers;

namespace VideoGenerator.Specs.StepDefinitions;

[Binding]
public sealed class StepDefinitions (VideoGeneratorForm form)
{
    private readonly VideoGeneratorForm _form = form;

    [Given(@"App is loaded")]
    public static void GivenAppIsLoaded ()
    {
        Task.Delay(5000).Wait();
    }

    #region Specific Controls

    [When(@"Click on FilesTab")]
    public void WhenClickOnFilesTab ()
    {
        _form.ClickFilesTab();
    }

    [When(@"Click on ImageEditorTab")]
    public void WhenClickOnImageEditorTab ()
    {
        _form.ClickImageEditorTab();
    }

    [When(@"View Selected Image for (.*) Seconds")]
    public void WhenViewSelectedImageForSeconds (int delaySeconds)
    {
        WhenClickOnImageEditorTab();
        WhenWaitSeconds(delaySeconds);
    }


    [When(@"Click on FilesGrid")]
    public void WhenClickOnFilesGrid ()
    {
        _form.ClickFilesGrid();
    }

    [When(@"Click on FileNameFilterToggle")]
    public void WhenClickOnFileNameFilterToggle ()
    {
        _form.ClickFileNameFilterToggle();
    }

    [When(@"Type FileNameFilterText ""([^""]*)""")]
    public void WhenTypeFileNameFilterText (string text)
    {
        _form.TypeFileNameFilterText(text);
    }

    #endregion Specific Controls


    #region General Controls

    [When(@"Move Mouse (.*), (.*) pixels")]
    public void WhenMoveMousePixels (int x, int y)
    {
        _form.MoveMouse(x, y);
    }

    [When(@"Move Mouse (.*), (.*) pixels and Click")]
    public void WhenMoveMousePixelsAndClick (int x, int y)
    {
        WhenMoveMousePixels(x, y);
        WhenClick();
    }

    [When(@"Click")]
    public void WhenClick ()
    {
        _form.Click();
    }

    [When(@"Wait (\d*) Seconds")]
    public static void WhenWaitSeconds (int delaySeconds)
    {
        Task.Delay(delaySeconds * 1000).Wait();
    }

    [When(@"Wait (.*) to (.*) Seconds")]
    public static void WhenWaitToSeconds (int minDelaySeconds, int maxDelaySeconds)
    {
        WhenWaitSeconds(Random.Shared.Next(minDelaySeconds, maxDelaySeconds)); //TODO: Is this range inclusive?
    }

    #endregion General Controls
}
