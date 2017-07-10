
'****************************************************************************************
'				The IEX Test User Library				'
'											'
'Warning: Do not change the signatures of UserInit() and UserFailStep()			'
'This will make your scripts invalid.							'
'Use cases:										'
'	1. Add custom logic to the default Initialize() and FailStep().			'
'	2. Add subroutines necessary for the execution of the test (or group of tests).	'
'****************************************************************************************

'======================================================================================================================
' Use the following variables to control header-footer default settings:
' TestOptions_UserBitmapPath	= Define a custom bitmap path
' TestOptions_UserBitmapSet	= Define a custom bitmap set
' TestOptions_TakeSnapshot	= Set to 'False' to prevent IEX Gateway from taking a snapshot when FailStep is called
' TestOptions_SetLogHeader	= Set to 'False' to prevent the IEX Gateway from setting the log header in Initialize
'======================================================================================================================


' This method is the last call from the footer's Initialize() method, and can override all the
' default settings done in Initialize().
' Warning: DO NOT CONNECT TO THE IEX GATEWAY IN THIS SUBROUTINE,AND DO NOT ATTEMPT TO OVERRIDE 'STB'
' AND 'No'. Do only the following (if needed):
' 1. Perform non-IEX initialization steps (EA initialization, Telent connection, etc...).
' 2. Configure Gateway properties: BitmapSet, BitmapSetTable, LogHeader, etc...
' 3. Configure test execution settings: BMP_Set, BMP_Path, etc...
' 4. Insert initiailzation logic (e.g., If No=2 Then IEX.LogHeader = "{TEST_NAME}_2"...).
Sub UserInit(ByVal STB,ByVal No)

End Sub


' This method is called from the footer's FailStep() method. Its return value determines whether to
' fail the test.
Function UserFailStep(Reason)

	IEX.FailStep "Step Failed. Reason: " & Reason
	UserFailStep = True

End Function



Function UserNoTest(Reason)

	IEX.LogComment "No Test- Reason: " & Reason, false, false, false, "", "Black"
	IEX.GetSnapshot "noTest Reason: " & Reason
	UserNoTest = True

End Function


Sub UserPassStep(Comment)

	IEX.PassStep Comment

End Sub


Sub UserStartStep(StepInfo)

	IEX.StartStep "Step started " & StepInfo, false, false, false, 0, "Black"

End Sub


Sub TearDown

End Sub