Feature: SpecFlowTest
![SpecFlowTest](https://github.com/TheMasonX/VideoGenerator/)
Simple calculator for adding **two** numbers

Link to a feature: [SpecFlowTest](VideoGenerator.Specs/Features/SpecFlowTest.feature)
***Further read***: **[Learn more about how to generate Living Documentation](https://docs.specflow.org/projects/specflow-livingdoc/en/latest/LivingDocGenerator/Generating-Documentation.html)**


Scenario: Click Around
	Given App is loaded
	When Wait 5 Seconds
	When Click on FilesGrid
	When Move Mouse -30, 25 pixels and Click
	When Move Mouse 30, -50 pixels and Click
	When Move Mouse 40, -50 pixels and Click
	When Wait 1 to 3 Seconds
	When Move Mouse -15, 20 pixels and Click
	When Wait 1 to 2 Seconds
	When Move Mouse -80, 95 pixels and Click
	When Wait 5 Seconds
	When Move Mouse 60, 43 pixels and Click
	When Wait 1 Seconds
	When Click
	When Click
	When Wait 2 to 3 Seconds
	When Wait 1 Seconds
	When Move Mouse -20, -10 pixels and Click
	When Wait 1 Seconds
	When Move Mouse -30, -50 pixels and Click
	When Wait 1 Seconds

Scenario: Open A File
	Given App is loaded
	When Click on FilesTab
	When Wait 10 Seconds
	When Click on FilesGrid
	When Wait 1 Seconds
	When Click
	When View Selected Image for 4 Seconds
	When Click on FilesTab

Scenario: Filter Files
	Given App is loaded
	When Click on FilesTab
	When Wait 10 Seconds
	When Click on FilesGrid
	When Click on FileNameFilterToggle
	When Type FileNameFilterText "Text"
	When Wait 1 Seconds
	When Click
	When View Selected Image for 4 Seconds
	When Click on FilesTab

Scenario: Open Multiple Files
	Given App is loaded

	When Click on FilesTab
	When Wait 10 Seconds
	When Click on FilesGrid
	When Wait 1 Seconds
	When Click
	When View Selected Image for 3 Seconds

	When Click on FilesTab
	When Click on FilesGrid
	When Move Mouse -50, 20 pixels and Click
	When View Selected Image for 5 Seconds
	
	When Click on FilesTab
	When Click on FilesGrid
	When Move Mouse -100, -30 pixels and Click
	When View Selected Image for 2 Seconds
	
	When Click on FilesTab
	When Click on FilesGrid
	When Move Mouse -120, -60 pixels and Click
	When Wait 1 Seconds
	When View Selected Image for 4 Seconds

	When Click on FilesTab
	When Click on FilesGrid
	When Move Mouse -200, 70 pixels
	When Click
	When View Selected Image for 3 Seconds