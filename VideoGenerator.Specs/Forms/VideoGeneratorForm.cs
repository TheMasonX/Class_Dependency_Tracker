using OpenQA.Selenium.Interactions;

using SpecFlow.Actions.WindowsAppDriver;

namespace VideoGenerator.Specs.Drivers;

public class VideoGeneratorForm (AppDriver appDriver) : VideoGeneratorElements(appDriver)
{
    public void ClickFilesTab () => FilesTab.Click();
    public void ClickImageEditorTab () => ImageEditorTab.Click();
    public void ClickFilesGrid () => FilesGrid.Click();
    public void ClickFileNameFilterToggle () => FileNameFilterToggle.Click();
    public void TypeFileNameFilterText (string text)
    {
        FileNameFilterText.Click();
        new Actions(Driver).SendKeys(text).Perform();
    }
}
