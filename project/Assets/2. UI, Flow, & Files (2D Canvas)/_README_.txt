2. UI, Flow, & Files (2D Canvas)

Create a working application using the wireframes provided.

• Create an object with the following data:
    o Title, Reporter Email, Submission Date
    o Priority (true, false)
    o Type: Internal, External
    o State: New, Review, Complete
    
• Reports Screen: List all reports from a specific folder
• Open Report Screen: Display the data of a specific report, allow the change of state and priority
• New Report Screen: Create a new report and save it in a specific folder

• Serialize the data in JSON and create an easy to replace alternative with XML


Notes: 
Found a plugin that will allow the user to pick from a popup calender.
https://assetstore.unity.com/packages/tools/gui/datepicker-for-unityui-68264

This may be too long to implement, for now let's just make some rules so that the use can easily use the text, eg: only only use the numbers, if there is a number larger than the required, change placement.

Most of the code has been blocked out. 
Most of the ui has been blocked out and about ready for multiple resolutions. 

Need to finish the 'opened-report' block-out for code and ui hooks. 

to finish:
- At runtime, add a new report.
- Provide feedback if there are no reports yet.
- Validate the list.
- Open a report when one is pressed.
- Get a basic state flow for a main menu, list reports, open, and new report. 

/*
/// ============//NOTES//========== ///

- filter the reports by date
- filter the reports by priority
- filter the reports by state

- hold off on the filtering. Right now make sure that all the reports can be updated. 
- priority, review, and complete should all update the report.

/*