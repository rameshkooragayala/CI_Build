'-----------------------------------------------------------------------+ 
'	FOOTER CHANGE:						    	'		
'								    	'
'	This sub-routine replaces the old "Sub Main()".		    	'		
'	"Sub Main()" is now used as the entry point to the test.    	'
'	The actual test logic should go into this sub-routine.	    	'
'	"Initialize()" and "TearDown()" are called from "Sub Main()".	'
'	"Sub Main()" appears directly below.				'
'-----------------------------------------------------------------------+
Sub TestMain()
'<!-- Begin of the Robodoc comments -->
'Sub Main()
	'Initialize
	
        '****h* S&D robustness script/ST_CC_002bis_Channel_change_with_RB_withPplus
        '* TITLE
        '*  ST_CC_002bis_Channel_change_with_RB_withPplus
        TEST_NAME ="ST_CC_002bis_Channel_change_with_RB_withPplus"
        TEST_CODE = "ST_CC_002bis"
        sScriptVersionNumber  = "v3.00"

        '* COMPATIBILITY
        '* dll IEX.ElementaryActions.E2E.UPC_v3.01
        '* EdIni   v3.00
        '* UserLib v3.00


        '* LOCALIZATION
        '*  NDS_FR

        '* PROJECT SUPPORTED
        '* Gateway / Affiliates / IPClient

        '* TOOL
        '*  S&D IEX

        '* SYNOPSIS
        '*  The goal of this test is to zap between two services defined in the configuration file

        '* HISTORY
        '* v2.0 :
        '* modification of the test Zapping between two services in 2 tests (ST_CC_001 and ST_CC_002)
        '* v2.1
        '* Script adapted to authorize the report of the result in a file if the HPQC server is not accessible
        '* Call the UserLib_GetUserLibVersion function to know the UserLib version used.
        '* not compatible with the DLL version oldest that the version 2_06
        '* not compatible with the UserLib version oldest that the version 2_03
        '* Since the begin of the PHASE2, the script ST_CC_002 has been modified, now a zapping P+ replace the zappings P+ then P-
        '* v3.00
        '* remove the call AC_GenerateMemoryReport because now this call is directly managed from the UPC DLL since the DLL version v3.01

'---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
'------------------------------------------------------                                                     STEP 1 -- Initialisation start                                                                  -----------------------------------------------------------------------------------
'---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

StartStep("STEP 1 -- Initialisation start")

'* ACTION
'* Call EA.AC_InitializeScript to initialize the test

Set Res = EA.AC_InitializeScript(TEST_CODE,TEST_NAME, sScriptVersionNumber, ini_sProject, ini_sSTBType, ini_sSoftwareBranch, ini_sSoftwareVersion,ini_sPhase,ini_sSite,ini_sOnSite,ini_sHeadEnd,ini_sBinaryType,ini_iTimerWallMovement, ini_iMaxTempInMainMenu, ini_bReportToHPQC, ini_iTarget, ini_iMaxErrors, ini_iExpectedDurationInMin, ini_sMemoryDump): If  Not Res.CommandSucceeded  Then FailStep("STEP 1 -- Initialisation") : Exit Sub

PassStep("STEP 1 -- Initialisation end")


'---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
'------------------------------------------------------                                                     STEP 2 -- Getting test environnement                                                                  -----------------------------------------------------------------------------------
'---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------


StartStep("STEP 2 -- Getting test environnement")

'* ACTION
'* Call the UserLib_GetUserLibVersion function to print the UserLib version in the Header of the script file.

UserLibVersion = UserLib_GetUserLibVersion()
Call EA.AC_WriteLineTrace("UserLib version = " & UserLibVersion)

Set Res =EA.AC_GetStringConfigFile( 0,0,0,"CHECK CONFIG FILE", "ini_sServiceSynchroKey",  ini_sServiceSynchroKey ): If  Not Res.CommandSucceeded  Then FailStep("STEP 2 -- Getting test environnement") : Exit Sub
Call EA.AC_WriteLineTrace("Service to re-synchronize the test =  " & ini_sServiceSynchroKey)

Set Res =EA.AC_GetStringConfigFile( 0,0,0,"CHECK CONFIG FILE", "ini_iWaitBetweenZapping", ini_iWaitBetweenZapping ):  If  Not Res.CommandSucceeded  Then FailStep("STEP 2 -- Getting test environnement") : Exit Sub
Call EA.AC_WriteLineTrace("The time to wait between the zapping =  " & ini_iWaitBetweenZapping)

iErrorIter = 0
iIterOk = 0
iTotalIterOk = 0
iActionNber = 0
sTargetStatus = "No updated"
sDetailedResult = "No updated"
sHealthCheckResult = "No updated"
iModulo=1
iModuloPplus=0

'* ACTION
'* first report to HPQC, call UserLib_FirstReportToQC

If (ini_bReportToHPQC = "YES") Then
    Call UserLib_FirstReportToQC(ini_sSoftwareBranch, ini_sSoftwareVersion, ini_sHeadEnd, ini_sPhase, ini_sOnSite, ini_sSTBType, ini_iTarget, ini_iExpectedDurationInMin, ini_sSite, ini_sBinaryType)
    Call EA.AC_Traces("INFO", "UserLib_FirstReportToQC : direct reporting of the result")
End If

Call EA.AC_WriteLineTrace("************************************************************")

PassStep("STEP 2 -- Getting test environnement")

'---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
'------------------------------------------------------                                                     STEP 3 -- Execution                                                                  -----------------------------------------------------------------------------------
'---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

StartStep("STEP 3 -- Execution")

If ini_sMemoryDump = "YES" Then
    Call EA.RG_DumpMemory( iErrorIter, iIterOk, 0)
End If


        '* ACTION
        '* Press the BLUE RCU key to allows to the STB team to synchronize the IEX timestamps and the CPE timestamps
        Call EA.RG_SynchroIEXtracesWithCPEtraces(iErrorIter,0,0)

Do Until (EA.HL_ExitDo)
          iIterOk = 0
         iErrorIter =  iErrorIter+1



                '* ACTION
                '* Select a service to re-synchronize, wait 7 seconds
                Call EA.AC_InsertDigits(iErrorIter, 0 , 0, ini_sServiceSynchroKey, 1,7)

                '* ACTION
                '* Press the MENU RCU key and wait 5 seconds (normally "CHANNELS" item focused)
                Call EA.AC_MultipleKeyPressed( iErrorIter,iIterOk, iActionNber, "MENU",1,ini_iMaxTempInMainMenu)

                 '* CHECK
                '* check that the MAIN MENU is displayed
                Call EA.CK_CheckOrSearchItemInMainMenu(iErrorIter,iIterOk,iActionNber,"RESYNCHRO", "DIC_MAIN_MENU_ON_NOW", False, False, CheckMenu)


                '* ACTION
                '* Repeat on loop the following actions : update the description of the actions executed

        Do Until (EA.HL_ExitDo)
                iIterOk = iIterOk + 1
                RealTimeToWait = 0

                        If ini_sMemoryDump = "YES" Then
                                Call EA.RG_DumpMemory( iErrorIter, iIterOk, 0)
                        End If

                        RealTimeToWait = ini_iWaitBetweenZapping - 3
                        '* ACTION
                        '* Select a service, wait 7 seconds
                        iActionNber = iActionNber + 1
                        Call EA.AC_MultipleKeyPressed(iErrorIter, iIterOk, iActionNber,"CH_+",1,RealTimeToWait)




                        iModuloPplus=iModuloPplus+1
                        If  iModuloPplus = 10 then
                                '*CHECK
                                '* [EA.AC_JpegSnapshot] Take snapshot to control that the channel has changed
                                iActionNber = iActionNber + 1
                                Call EA.AC_JpegSnapshot(iErrorIter, iIterOk, iActionNber, "Control_Channel_Changed")
                                iModuloPplus =0
                        End If



                        iTotalIterOk = iTotalIterOk + 1
                        Call EA.AC_Traces("RESULT","******* Number of cumulated OK  " & iTotalIterOk & " iterations and " & iErrorIter -1 & " errors.")



                Loop

                If sTargetStatus =  "OK" Then Exit Do

                If iErrorIter = ini_iMaxErrors Then
                        If ini_sMemoryDump = "YES" Then
                                Call EA.RG_DumpMemory( iErrorIter, iIterOk, 3)
                        End If
                        sDetailedResult = "the max number of errors (" & ini_iMaxErrors & ") has been reached before the target expected (" & ini_iTarget & ") after " & iTotalIterOk & " good iterations and " & iErrorIter - 1 & " errors."
                        Call EA.AC_Traces("RESULT","******* KO, " & sDetailedResult)
                        sTargetStatus =  "KO"
                        Exit Do
                End If

        Loop

         ' code to manage the end of the step "execution" when this step has been stopped because the ini_iExpectedDurationInMin has been reached
         Call EA.HL_EndOfTimeReporting( iTotalIterOK, iErrorIter, sDetailedResult, sTargetStatus, iModulo)

        ' code to manage the end of the step "execution"
        If sTargetStatus = "OK" Then
                        PassStep("STEP 3 -- Execution")
                Else
                        FailStep("STEP 3 -- Execution")
        End If


'---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
'------------------------------------------------------                                                     STEP 4 -- Verfication                                                                  -----------------------------------------------------------------------------------
'---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

StartStep("STEP 4 -- Verification")

'* ACTION
'* Call EA.HL_HealthCheck to check the behavior of the CPE at the end of the test

Call EA.HL_HealthCheck(iErrorIter, iIterOk, iActionNber, bReturnHealthCheck )

        If bReturnHealthCheck = True Then

                        sHealthCheckResult = "OK"
                        PassStep("STEP 4 -- Verification")
            Else
                        sHealthCheckResult = "KO"
                        FailStep ("STEP 4 -- Verification")

        End If

'---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
'------------------------------------------------------                                                     STEP 5 -- Report to QC                                                                  -----------------------------------------------------------------------------------
'---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

StartStep("STEP 5 -- Report to QC")

  '* ACTION
  '* final report to HPQC : call UserLib_FinalReportToQC or EA.AC_GenerateReportInFile

  If (ini_bReportToHPQC = "YES") Then
        Call UserLib_FinalReportToQC(iTotalIterOk, sDetailedResult, sTargetStatus, sHealthCheckResult)
        Call EA.AC_Traces("INFO", "loop " & iErrorIter & " step " & iIterOk & " action " & iActionNber & " UserLib_FinalReportToQC : direct reporting of the result")
  Else
        Call EA.AC_GenerateReportInFile(iTotalIterOk, sDetailedResult, sTargetStatus, sHealthCheckResult)
        Call EA.AC_Traces("INFO", "loop " & iErrorIter & " step " & iIterOk & " action " & iActionNber & " AC_GenerateReportInFile : indirect reporting of the result in a file")
  End If

PassStep("STEP 5 -- Report to QC")
End Sub

'	<FooterAttributes Version=1.0.0/>
Sub Main

Dim errorDesc
	errorDesc = ""

	Initialize
	
	On Error Resume Next
	
	TestMain
	errorDesc = Err.Description
	
	On Error Goto 0
	
	TearDown
	
	If errorDesc <> "" Then MsgBox "Test was aborted due to an error." & VbCrlf & VbCrlf & "Error description: " & errorDesc

End Sub


'Initialize variables for use by the Test. This Sub MUST be called before any IEX statements in the TEST.
Dim TEST_NAME, BMP_Set,BMP_Path, EA, args
Sub Initialize()
   Set args = WScript.Arguments	
   TEST_NAME = "ST_CC_002bis_Channel_change_with_RB_withPplus"
   BMP_Path = "O:\TestScripts\ST_CC_002bis_Channel_change_with_RB_withPplus"
   BMP_Set = "Generic"
   IEX.BitmapSet = BMP_Set
   IEX.BitmapSetTable = BMP_Path  & "\ST_CC_002bis_Channel_change_with_RB_withPplus.xml"

   IEX.LogHeader "Test: ST_CC_002bis_Channel_change_with_RB_withPplus."

   UserInit IEX.RemoteHost,IEX.IEXServerNumber

End Sub

'Called when the user inserts a 'StartStep' command
Sub StartStep(StepInfo)

   UserStartStep StepInfo   

End Sub


'Called when the IEX system fails (as opposed to unit under test failure)
Function FailStep(Reason)

   FailStep = UserFailStep(Reason)
   IEX.GetSnapshot cstr(Reason)

End Function


'Called when the user inserts a 'PassStep' command
Sub PassStep(Comment)

   UserPassStep Comment

End Sub


'This function should be called when the unit under test fails. 
'It is generated by default whenever a verification fails.
Function noTest(Reason)

   noTest = UserNoTest(Reason)

End Function


Function GetParam(paramName)

   Dim varIndex
   Dim valIndex

   Set args = WScript.Arguments	

   For counter = 0 to args.Count - 1
      varIndex = InStr(LCase(args(counter)), LCase(paramName))
      If varIndex > 0 Then 
         ' Index where assignment starts should be index where variable name starts, plus length of variable name plus one for "=" 
         valIndex = varIndex + Len(paramName) + 1
	 GetParam = Mid(args(counter), valIndex)
	 Exit Function
      End If
   Next   
   	
   GetParam = InputBox(CStr(paramName))

End Function

'==========================================
'=    TC2 Batch Execution Support Code    =
'= -------------------------------------- =
'= The code below is used to allow correct=
'= execution of VBS files when they are   =
'= run from a batch file.                 =
'= -------------------------------------- = 
'= !          Edit with care.           ! =
'==========================================


Sub BatchInit
    Dim IP,No

    IP = GetParam("IP")
    No = GetParam("No")

    Set IEX = CreateObject("IEXGateWay_NET.IEX")

    If Len(IP) > 0 And Len(No) > 0 Then
	IEX.IEXServerNumber = CInt(No)
	IEX.RemoteHost = IP
    Else
	IEX.RemoteHost = "127.0.0.1"
    IEX.IEXServerNumber = 1
    End If
	
    IEX.LogFileName = "{LOG_FILE}"
    
    IEX.Connect
    
    Dim attempts : attempts = 4
    While Not IEX.IsConnected And attempts > 0
       IEX.LogComment "Could not connect - waiting two seconds and retrying."
       IEX.wait 2
       IEX.Connect
       attempts = attempts - 1
    Wend

    If IEX.IsConnected Then 
       IEX.VisibleGateway = True 'Causes the IEXGateway to display when running from VBScript.
       Main
       If forwardStep then call PassStep("Step passed")
       IEX.Disconnect
    Else
       IEX.LogComment "Test Aborted because the gateway could not connect to the IEX Server.",,,,,"red"
    End If
    IEX.Dispose
End Sub


'Provides the subscript mechanism:
Function SubScript(ScriptName)
    Set sc = CreateObject("ScriptControl")
    sc.Language = "VBScript"
    sc.Reset
    sc.AddObject "IEX", IEX, true
    Set f = CreateObject("Scripting.FileSystemObject")
    Set t = f.OpenTextFile(ScriptName)
    sc.AddCode t.ReadAll
    t.Close

    'If your script starting point is not "Main()" please change the 
    'following line to reflect the starting point. Any parameters 
    'must be passed. eg sc.Run("MyMain",param1,param2)
    sc.Run("Main") 
End Function

'=======================================
'= Critical Script Code                =
'= DO NOT EDIT!                        =
'=======================================
Dim IEX : doInit = true
On Error Resume Next
doInit = IEX Is Nothing
On Error Goto 0
If doInit Then Call BatchInit Else Call Main


'****************************************************************************************
'								The IEX Test User Library				'																						'
'Warning: Do not change the signatures of UserInit() and UserFailStep()					'
'This will make your scripts invalid.													'
'Use cases:																				'
'	1. Add custom logic to the default Initialize() and FailStep().						'
'	2. Add subroutines necessary for the execution of the test (or group of tests).		'
'****************************************************************************************

' This method is the last call from the footer's Initialize() method, and can override all the
' default settings done in Initialize().
' Warning: DO NOT CONNECT TO THE IEX GATEWAY IN THIS SUBROUTINE,AND DO NOT ATTEMPT TO OVERRIDE 'STB'
' AND 'No'. Do only the following (if needed):
' 1. Perform non-IEX initialization steps (EA initialization, Telent connection, etc...).
' 2. Configure Gateway properties: BitmapSet, BitmapSetTable, LogHeader, etc...
' 3. Configure test execution settings: BMP_Set, BMP_Path, etc...
' 4. Insert initiailzation logic (e.g., If No=2 Then IEX.LogHeader = "10004_1 video yes_2"...).

	'<!-- Begin of the Robodoc comments -->
'****h* UPC robustness script/UserLib

'* Version
'* v3.1

'* SYNOPSIS
'* UserLib Library
	
'* COMPATIBILITY
'* IEX.ElementaryActions.E2E.UPC_V2.0

'* HISTORY
'*  v1.2 :
'* Old UserLibrary
'*  v1.7 :
'* Set Trace Level to 00
'* Adding functions to report to QC 
'*  v1.8 :
'* Adding filed sSoftwareBranch to ReportToQC
'*  v2.0 :
'* UserLib adapted for the S&D infrastructure
'*  v2.1 :
'* In the FinalReportToQC function : report the informations in the HPQC field "CommentText" instead of the HPQC field "Comments".
'* The goal is to have the same FinalReportToQC function for the robustness tests and for the Requete Monitoring tests. 
'*  v2.2 :
'* In the FinalReportToQC function : revert back because for the Requete Monitoring tests, some new parameters must be added in the FinalReportToQC. 
'* For this reason, a specific FinalReportToQC will created for these tests.
'*  v2.3 :
'* Add the function UserLib_GetUserLibVersion
'* Rename FirstReportToQC by UserLib_FirstReportToQC
'* Rename FinalReportToQC by UserLib_FinalReportToQC
'*  v2.4 : ZC :
'* Add functions to capture logs via IEXDebug
'* Concatenation of IEX logs slices of 10 MB to full one.
'* Report test report link in field "Logs Link"  
'* Report STB IP Address to FieldValue8
'* Add check logs analyse & report function
'* Add call to EAs AC_CompressLogsFile(), AC_CleanIEXLogs()
'* v2.5 : DC :
'* Add UserLib_FinalReportToQCForRM function to replace FinalReportToQCForRM
'* v2.6 : ZC :
'* Replace hard coded settings file for checklogs tool by variable. 
'* Add GetCoreFile EA call.
'* v3.0 :
'* Merge of the UserLib used for the robustness tests and for the RM tests
'* First version compatible with the new UPC_DLL delivery v3.0
'* ZC : Add a flag to check if launching CheckLogs is allowed.
'* v3.1 :
'* ZC : Add fields Group ID and Group Run ID to HPQC final reporting. 
'* ZC : [TAUM-410], Add explicit traces in results file when com port is in use.


'<!-- End of the Robodoc comments -->
'******
Dim TestName

Sub UserInit(ByVal STB,ByVal No)

UserInitEA(IEX)

	'* ACTION
	'* START LOGS CAPTURE
	Set Res = IEX.Debug.BeginLogging( EA.SerialLogsManagement.sIEXLogFileRecords , "", True, DebugDevice_Serial,True) 
	
	'* CHECK
	'* Check if last command succeeded
	If Res.CommandSucceeded() Then
			Call EA.AC_Traces("INFO", "Starting Logs capture.")
	else
			Call EA.AC_Traces("WARNING", "Can't start logs capture!!! " & Res.FailureReason)	
			If noTest("Fail to record logs top path : " & EA.SerialLogsManagement.sIEXLogFileRecords & " And the response was: " & Res.FailureReason) Then Exit Sub
	End If

End Sub


' This method is called from the footer's FailStep() method. Its return value determines whether to

' fail the test.
Function UserFailStep(Reason)
	IEX.LogComment "Step failed. Reason: "+ Reason,True,,,"12","red" 
      	'Terminate test execution:
	UserFailStep = True
End Function



Function UserNoTest(Reason)

	IEX.LogComment "No Test- Reason: " & Reason
	IEX.GetSnapshot "noTest Reason: " + Reason
	UserNoTest = True

End Function


Sub UserPassStep(Comment)

    IEX.PassStep Comment
    IEX.LogComment "Step passed. Reason: "+ Comment,True,,,"12","green"

End Sub


Sub UserStartStep(StepInfo)
	IEX.StartStep StepInfo, True, False, False, 16, "purple"

End Sub


Sub TearDown

    '* ACTION
    '* Print state of different threads of driver in logs file
    if EA.sOnSite = "ONSITE" then
        Call EA.HL_LaunchSysrqTrigger()
        Call EA.AC_Traces("INFO", "Last loop, Last step, Last action. Print state of different threads of driver in logs file.")
    End If
    '* ACTION
	'* STOP LOGS CAPTURE
	Set Res = IEX.Debug.EndLogging(DebugDevice_Serial) : If Res.CommandExecutionFailed Then If noTest("IEX server reported that the command failed. The command was: ""IEX.Debug.EndLogging(DebugDevice_Serial)"". And the response was: " & Res.FailureReason) Then Exit Sub
	'* ACTION
	'* CONCAT LOGS FILE
	Call EA.AC_ConcatIEXLogs()
	'* ACTION
    '* If the generation of the memory stat is requested, the EA AC_GenerateMemoryReport is called by analysing logs file just generated above in EA AC_ConcatIEXLogs.
    If EA.sMemoryDump = "YES" Then
       Call EA.AC_Traces("INFO", "Last loop, Last step, Last action, Generation of the memory report started")
       Call EA.AC_GenerateMemoryReport()
    End If
    '* CHECK
	'* Check if CheckLogs launch is allowed
	If EA.sLaunchChecklog = "YES" Then
		'* ACTION
		'* Launch CheckLogs
		Call EA.HL_CheckLogs(EA.sCheckLogsSettingsFilePath, sCountersResults, sCQResults, sGeneralTracesResults)
		'* ACTION
		'* Report CheckLogs result to HPQC
		Call UserLib_CheckLogsReportToQC(sCountersResults, sCQResults, sGeneralTracesResults)
	End If
    '* ACTION
    '* Get core file
	Call EA.HL_GetCorefile()
    '* ACTION
	'* Compress IEX Logs file
	Call EA.AC_CompressLogsFile()
	'* ACTION
	'* Clean IEX logs file slices and full IEX Logs after compressing it
	Call EA.AC_CleanIEXLogs()
	'* ACTION
	'* SEND TEST RESULTS TO SERVER 
	Call EA.AC_SendTestResultToServer()

End Sub

Sub UserInitEA(gw)
   On Error Resume Next
   Set prxy = CreateObject("DotNetProxy.Proxy")
   If prxy Is Nothing Then Exit Sub
   prxy.LoadFile("E:\IEX\UPC_EA_DLL\IEX.ElementaryActions.E2E.UPC.dll")
   Set EA = prxy.GetClass("IEX.ElementaryActions.E2E.UPC.Manager")
   EA.TracerSettings.SetTraceLevels.Level_00_Off
   If EA Is Nothing Then Exit Sub
   EA.TestName=TEST_NAME
   EA.Init gw.GetDotNetGateway
   EA.TracerSettings.SetTraceLevels.Level_00_Off
   On Error GoTo 0
End Sub

'======================================================================================================================
	'<!-- Begin of the Robodoc comments -->
'****h* UPC robustness script/UserLib_GetUserLibVersion
'* TITLE
'*  UserLib_GetUserLibVersion

'* LOCALIZATION
'*  NDS_FR

'* PROJECT SUPPORTED
'* All

'* SYNOPSIS
'*  The goal of this function is to indicate the userLib version used

'* FUNCTION_PARAMETERS
'* input : none
'* output : string containing the userLib version
Function UserLib_GetUserLibVersion() 

sUserLibVersion = "UserLib_UPCv3.1"
UserLib_GetUserLibVersion = sUserLibVersion

'<!-- End of the Robodoc comments -->
'******
End Function

'======================================================================================================================
	'<!-- Begin of the Robodoc comments -->
'****h* UPC robustness script/UserLib_FirstReportToQC
'* TITLE
'*  UserLib_GetUserLibVersion

'* LOCALIZATION
'*  NDS_FR

'* PROJECT SUPPORTED
'* All

'* SYNOPSIS
'*  The goal of this function is start the direct reporting from IEX to HPQC at the begin of the script execution

'* FUNCTION_PARAMETERS
'* input : sSoftwareBranch, sSoftwareVersion, sHeadEnd, sPhase, sOnSite, sSTBType, iTarget, iExpectedDurationInMin, sSite, sBinaryType
'* output : none

Sub UserLib_FirstReportToQC(sSoftwareBranch, sSoftwareVersion, sHeadEnd, sPhase, sOnSite, sSTBType, iTarget, iExpectedDurationInMin, sSite, sBinaryType)

ReportToQC "FieldName1", "Software Branch"
ReportToQC "FieldName2", "Head-End"
ReportToQC "FieldName3", "Binary Type"
ReportToQC "FieldName4", "Phase"
ReportToQC "FieldName5", "On Site"
ReportToQC "FieldName6", "TargetStatus"
ReportToQC "FieldName7", "HealthCheck"
ReportToQC "FieldName8", "STBipAddress"
ReportToQC "FieldName9", "STB_GroupID"
ReportToQC "FieldName10", "STB_GroupRunID"
ReportToQC "FieldValue1" , sSoftwareBranch
ReportToQC "FieldValue2" , sHeadEnd
ReportToQC "FieldValue3" , sBinaryType
ReportToQC "FieldValue4", sPhase
ReportToQC "FieldValue5" , sOnSite
ReportToQC "FieldValue8" , EA.sSTBipAddress
ReportToQC "Logs Link" , EA.oPath.sTestReportLink
ReportToQC "STB Type", sSTBType
ReportToQC "Site", sSite
ReportToQC "Software Version" , sSoftwareVersion
ReportToQC "Iterations" , iTarget
ReportToQC "Exp Dur" , iExpectedDurationInMin
ReportToQC "Validated" , "N"

'<!-- End of the Robodoc comments -->
'******
End Sub


'======================================================================================================================
	'<!-- Begin of the Robodoc comments -->
'****h* UPC robustness script/UserLib_FinalReportToQC
'* TITLE
'*  UserLib_GetUserLibVersion

'* LOCALIZATION
'*  NDS_FR

'* PROJECT SUPPORTED
'* All

'* SYNOPSIS
'*  The goal of this function is start the direct reporting from IEX to HPQC at the end of the script execution

'* FUNCTION_PARAMETERS
'* input : iTotalIterOk, sDetailedResult, sTargetStatus, sHealthCheckResult
'* output : none

Sub UserLib_FinalReportToQC(iTotalIterOk, sDetailedResult, sTargetStatus, sHealthCheckResult)

ReportToQC "Successful Iterations" ,iTotalIterOk
ReportToQC "Comments" , sDetailedResult
ReportToQC "FieldValue6" , sTargetStatus
ReportToQC "FieldValue7" , sHealthCheckResult
ReportToQC "FieldValue9" , EA.sSTBGroupID
ReportToQC "FieldValue10" , EA.sSTBRunID
'<!-- End of the Robodoc comments -->
'******
End Sub

'======================================================================================================================
	'<!-- Begin of the Robodoc comments -->
'****h* UPC robustness script/FinalReportToQCForRM
'* TITLE
'*  UserLib_GetUserLibVersion

'* LOCALIZATION
'*  NDS_FR

'* PROJECT SUPPORTED
'* All

'* SYNOPSIS
'*  The goal of this function is start the direct reporting from IEX for RM (Request monitoring) to HPQC at the end of the script execution

'* FUNCTION_PARAMETERS
'* input : NbreReboot, sCaptureContent, sTargetStatus, sResult, ini_sComparequest, sReqByTyp, sRequestStatus)
'* output : none

Sub FinalReportToQCForRM(NbreReboot, sCaptureContent, sTargetStatus, sResult, ini_sComparequest, sReqByTyp, sRequestStatus)

ReportToQC "Successful Iterations" ,NbreReboot
ReportToQC "CommentText" , sCaptureContent
ReportToQC "FieldValue8" , sTargetStatus
ReportToQC "Comments" , sResult
ReportToQC "TextField1" , ini_sComparequest
ReportToQC "Highlights" , sReqByTyp
ReportToQC "FieldValue7" , sRequestStatus
ReportToQC "Validated" , "Y"

'<!-- End of the Robodoc comments -->
'******
End Sub

'======================================================================================================================
    '<!-- Begin of the Robodoc comments -->
'****h* UPC robustness script/UserLib_FinalReportToQCForRM
'* TITLE
'*  UserLib_GetUserLibVersion

'* LOCALIZATION
'*  NDS_FR

'* PROJECT SUPPORTED
'* All

'* SYNOPSIS
'*  The goal of this function is start the direct reporting from IEX for RM (Request monitoring) to HPQC at the end of the script execution

'* FUNCTION_PARAMETERS
'* input : NbreReboot, sCaptureContent, sTargetStatus, sResult, ini_sComparequest, sReqByTyp, sRequestStatus)
'* output : none

Sub UserLib_FinalReportToQCForRM(NbreReboot, sCaptureContent, sTargetStatus, sResult, ini_sComparequest, sReqByTyp, sRequestStatus)

ReportToQC "Successful Iterations" ,NbreReboot
ReportToQC "CommentText" , sCaptureContent
ReportToQC "FieldValue8" , sTargetStatus
ReportToQC "Comments" , sResult
ReportToQC "TextField1" , ini_sComparequest
ReportToQC "Highlights" , sReqByTyp
ReportToQC "FieldValue7" , sRequestStatus
ReportToQC "Validated" , "Y"

'<!-- End of the Robodoc comments -->
'******
End Sub


'======================================================================================================================
'****h* UPC robustness script/UserLib_CheckLogsReportToQC
'* TITLE
'*  UserLib_CheckLogsReportToQC

'* LOCALIZATION
'*  CISCO_FR

'* PROJECT SUPPORTED
'* All

'* SYNOPSIS
'*  The goal of this function is start the direct reporting from IEX to HPQC at the end of the Check Logs execution

'* FUNCTION_PARAMETERS
'* input : sCountersResults, sCQResults, sGeneralTracesResults
'* output : none

Sub UserLib_CheckLogsReportToQC(sCountersResults, sCQResults, sGeneralTracesResults)

ReportToQC "TextField1" ,sCountersResults
ReportToQC "Highlights" , sCQResults
ReportToQC "CommentText" , sGeneralTracesResults

'******
End Sub

'======================================================================================================================

'Initializes a log file containing only text
'Usage: declare the folliwing before: Dim filesys, filetxt
Sub InitLogFile(filesys, filetxt)
    Set filesys = CreateObject("Scripting.FileSystemObject")
    IEXNUM = IEX.IEXServerNumber
    logfilename = "IEX_" & IEXNUM & "_" & TEST_NAME & ".log"
    Set filetxt = filesys.OpenTextFile("E:\IEX\UPC_EA\simplelogs\" & logfilename, 2, True)
End Sub
'To be called before Teardown
Sub CloseLogFile(filetxt)
	filetxt.close
End Sub
'Writes logtext in the filetext file
'To be used after LogComment if needed
Sub WriteSimpleLog(filetxt, logtext)
	filetxt.WriteLine(Date & " : "  & Time & " : " & logtext)
End Sub


