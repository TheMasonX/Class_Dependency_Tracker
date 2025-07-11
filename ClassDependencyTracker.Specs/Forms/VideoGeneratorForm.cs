using OpenQA.Selenium.Interactions;

using SpecFlow.Actions.WindowsAppDriver;

namespace ClassDependencyTracker.Specs.Drivers;

public class ClassDependencyTrackerForm (AppDriver appDriver) : ClassDependencyTrackerElements(appDriver)
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
