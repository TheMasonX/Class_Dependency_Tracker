using OpenQA.Selenium.Appium;
using OpenQA.Selenium.Appium.Windows;
using OpenQA.Selenium.Interactions;

using SpecFlow.Actions.WindowsAppDriver;

namespace VideoGenerator.Specs.Drivers;

public class VideoGeneratorElements (AppDriver appDriver)
{
    private readonly AppDriver _appDriver = appDriver;
    protected WindowsDriver<WindowsElement> Driver => _appDriver.Current;

    public WindowsElement FilesTab => _appDriver.Current.FindElementByAccessibilityId("filesTab");
    public WindowsElement ImageEditorTab => _appDriver.Current.FindElementByAccessibilityId("imageEditorTab");
    public WindowsElement FilesGrid => _appDriver.Current.FindElementByAccessibilityId("fileGrid");
    public WindowsElement FilesGrid_DataGrid => _appDriver.Current.FindElementByAccessibilityId("filesGrid_DataGrid");

    public AppiumWebElement FilesGrid_DataGrid2 => FilesGrid.FindElementByAccessibilityId("filesGrid_DataGrid");
    public AppiumWebElement FileNameFilterToggle => FilesGrid.FindElementByAccessibilityId("fileNameFilterEnabled");
    public AppiumWebElement FileNameFilterText => FilesGrid.FindElementByAccessibilityId("fileNameFilterText");

    public void MoveMouse (int x, int y) => new Actions(_appDriver.Current).MoveByOffset(x, y).Perform();
    public void Click () => new Actions(_appDriver.Current).Click().Perform();
}