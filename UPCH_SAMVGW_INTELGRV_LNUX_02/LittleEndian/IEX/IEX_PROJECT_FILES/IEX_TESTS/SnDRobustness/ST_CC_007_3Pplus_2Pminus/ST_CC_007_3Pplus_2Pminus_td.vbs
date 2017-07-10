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
'****h* UPC robustness script/ST_ScriptTemplate_TestOnLoop
'* TITLE
'*  ST_ScriptTemplate_TestOnLoop
TEST_NAME ="ST_CC_007_3Pplus_2Pminus"
TEST_CODE = "ST_CC_007"
sScriptVersionNumber  = "v3.00"

'* COMPATIBILITY
'* dll IEX.ElementaryActions.E2E.UPC_v3.01
'* EdIni   v3.00
'* UserLib v3.00

'* LOCALIZATION
'*  NDS_FR

'* PROJECT SUPPORTED
'* Gateway / Affiliates / ISTB/ IPClient

'* TOOL
'* S&D IEX

'* SYNOPSIS
'*  The goal of this test is perform 3P+ 2P-. Video must be present for each zapping

'* HISTORY
'* v2.0
'* Script adapted for the S&D Infrastructure
'* v2.1
'* Script adapted to authorize the report of the result in a file if the HPQC server is not accessible
'* Call the UserLib_GetUserLibVersion function to know the UserLib version used.
'* not compatible with the DLL version oldest that the version 2_06
'* not compatible with the UserLib version oldest that the version 2_03
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

'EdINI parameters specific for this test

'Set Res =EA.AC_GetStringConfigFile( 0,0,0,"CHECK CONFIG FILE", "ini_sInitialServiceKey", ini_sInitialServiceKey ): If  Not Res.CommandSucceeded  Then Exit Sub
'Call EA.AC_WriteLineTrace("Initial Service Key = " & ini_sInitialServiceKey)

Call EA.AC_WriteLineTrace("************************************************************")

'counters initialization

iErrorIter = 0
iIterOk = 0
iTotalIterOk = 0
iActionNber = 0
sTargetStatus = "No updated"
sDetailedResult = "No updated"
sHealthCheckResult = "No updated"
iModulo=1

'* ACTION
'* first report to HPQC, call UserLib_FirstReportToQC

If (ini_bReportToHPQC = "YES") Then
    Call UserLib_FirstReportToQC(ini_sSoftwareBranch, ini_sSoftwareVersion, ini_sHeadEnd, ini_sPhase, ini_sOnSite, ini_sSTBType, ini_iTarget, ini_iExpectedDurationInMin, ini_sSite, ini_sBinaryType)
    Call EA.AC_Traces("INFO", "UserLib_FirstReportToQC : direct reporting of the result")
End If

PassStep("STEP 2 -- Getting test environnement")

'---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
'------------------------------------------------------                                                     STEP 3 -- Execution                                                                  -----------------------------------------------------------------------------------
'---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

StartStep("STEP 3 -- Execution")

'-------------------Starting scripts----------------------

If ini_sMemoryDump = "YES" Then
    Call EA.RG_DumpMemory( iErrorIter, iIterOk, 0)
End If

'* ACTION
'* Press the GUIDE RCU key to allows to the STB team to synchronize the IEX timestamps and the CPE timestamps
Call EA.RG_SynchroIEXtracesWithCPEtraces(iErrorIter,0,0)

' ******  TO BE COMPLETED  : Here, write the actions to execute the pre-conditions (with the ROBODOC comments) ******

Do Until (EA.HL_ExitDo) 'loop to manage the errors

        iIterOk = 0
        iErrorIter =  iErrorIter +1

            '    Call EA.AC_InsertDigits(iErrorIter,iIterOk, iActionNber, ini_sInitialServiceKey,1,5)

        Do Until (EA.HL_ExitDo) 'loop to manage the test on loop

                        iIterOk = iIterOk+1
             '                                   CheckVideoPPlus = 0
             '                                   CheckVideoPMoins = 0

                If ini_sMemoryDump = "YES" Then
                        Call EA.RG_DumpMemory( iErrorIter, iIterOk, 0)
                End If

        For i= 0 To 2
                    iActionNber=iActionNber+1
                    Call EA.AC_MultipleKeyPressed(iErrorIter, iIterOk, iActionNber,"CH_+",1,17)
                    Call EA.AC_JpegSnapshot( iErrorIter,iIterOk,iActionNber, "CHECK_CHANNEL_CHANGE_P+")

                                         '* CHECK
                     '* check that the video is ok after a zapping (3 trials)
                        Call EA.CK_CheckVideoWithInfoResult( iErrorIter,iIterOk, 2, "ZAPPING_CHANNEL" &ini_sInitialServiceKey, True, checkvideo)
                                        If checkvideo=False Then
                                                iIterOk = iIterOk - 1
                                                      Call EA.AC_Traces("RESULT","******* ErrorNumber " & iErrorIter & " numberOfConsecutiveIterationOk " & iIterOk & " totalIterationOk " & iTotalIterOk & " check video failed")
                                                     Exit Do
                                        End If

                                iTotalIterOk = iTotalIterOk + 1
                                Call EA.AC_Traces("RESULT","******* Number of cumulated OK  " & iTotalIterOk & " iterations and " & iErrorIter - 1 & " errors.")
        Next


                For i= 0 To 1
                    iActionNber=iActionNber+1
                    Call EA.AC_MultipleKeyPressed(iErrorIter, iIterOk, iActionNber,"CH_-",1,17)
                    Call EA.AC_JpegSnapshot( iErrorIter,iIterOk,iActionNber, "CHECK_CHANNEL_CHANGE_P-")

                                        '* CHECK
                     '* check that the video is ok after a zapping (3 trials)
                                          Call EA.CK_CheckVideoWithInfoResult( iErrorIter,iIterOk, 2, "ZAPPING_CHANNEL" &ini_sInitialServiceKey, True, checkvideo)
                                        If checkvideo=False Then
                                                iIterOk = iIterOk - 1
                                                      Call EA.AC_Traces("RESULT","******* ErrorNumber " & iErrorIter & " numberOfConsecutiveIterationOk " & iIterOk & " totalIterationOk " & iTotalIterOk & " check video failed")
                                                     Exit Do
                                        End If

                                iTotalIterOk = iTotalIterOk + 1
                                Call EA.AC_Traces("RESULT","******* Number of cumulated OK  " & iTotalIterOk & " iterations and " & iErrorIter - 1 & " errors.")
                Next


        Loop

        If sTargetStatus = "OK" Then Exit Do

         ' code to manage the end of the test when the ini_iMaxErrors is reached
        If iErrorIter = ini_iMaxErrors Then
               If sMemoryDump = "YES" Then
                       Call EA.RG_DumpMemory(  iErrorIter, iIterOk, 0)
                End If
                sDetailedResult = "MAX Errors (" & ini_iMaxErrors & ") REACHED"
                Call EA.AC_Traces( "RESULT","*******" & sDetailedResult)
                sTargetStatus = "KO"
                Exit Do
        End If

Loop

        ' code to manage the end of the step "execution" when this step has been stopped because the target chosen has been reached
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

'---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
'------------------------------------------------------                                                     END                                                                  -----------------------------------------------------------------------------------
'---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
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


'**********************************************************************
Sub Initialize()

   'BMP_Path = "C:\Program Files\IEX" & tempTestLoc & "ST_CC_007_3Pplus_2Pminus" & "_" & BMPSet & "_" & IP & "_" & ServerNo
   'BMP_Set = BMPSET 'Already set in Test_Main
   TEST_NAME = "ST_CC_007_3Pplus_2Pminus"
   IEX.BitmapSet = BMP_Set
   IEX.BitmapSetTable = BMP_Path  & "\ST_CC_007_3Pplus_2Pminus.xml"

   IEX.LogHeader "Test: ST_CC_007_3Pplus_2Pminus."

   'EA Users -  Uncomment this line:
   'Call InitEA(IEX)
   UserInit IEX.RemoteHost,IEX.IEXServerNumber

End Sub


Sub startStep(StepInfo)

    Dim Status
    If forwardStepWhenNoPassStep Then Call implicitPassStep("Step " & CStr(testStep) & " passed.") 
    forwardStepWhenNoPassStep = True 	
    Status = "N/A"
    TDOutput.Print "Step started: " & StepInfo
    UserStartStep StepInfo
    call ReportStep(StepInfo, Status, false, "", "")

End Sub


Function FailStep(Reason)

    Dim Status
    AbortTest = UserFailStep(Reason)
    FailStep = AbortTest
    forwardStepWhenNoPassStep = Not FailStep
    Status = "Failed"
    TDOutput.Print "Step Failed. Reason: " & Reason
    Set Res = IEX.GetSnapShot(CStr(Reason))
    
    If Len(Res.ResponseString) > 4 Then
	    tempString = split(left(Res.ResponseString, len(Res.ResponseString) - 4), "\")
    	    If Not tempString = Empty Then Call AddURL("http://" & IP & "/IEX_" & ServerNo & "/snapshot/" & tempString(UBound(tempString)) & "/index.htm")   		
    End If	

    Call ReportStep(Reason, Status, Not FailStep, "failTest", "")

End Function


Sub implicitPassStep(Comment)

    forwardStepWhenNoPassStep = False
    TDOutput.Print "Step Passed: " & Comment
    call ReportStep(Comment, "Passed", true, "", "")   

End Sub


Sub passStep(Comment)

    implicitPassStep Comment    
    UserPassStep Comment

End Sub


Function noTest(Reason)

    TDOutput.Print "No Test- Reason: " & Reason
    call ReportStep(Reason, "Not Completed", True, "failTest", "")
    noTest = userNoTest(Reason)
	
End Function


Function GetParam(param)

   GetParam = InputBox(CStr(param))

End Function


Sub CleanUp(Debug, CurrentTest, CurrentRun)

    If not Debug then
       TDOutput.Print "Started clean-up."
       IEX.LogComment "Started clean-up."
       IEX.Disconnect
       IEX.Dispose
       On Error Resume Next
       EA.Dispose
       Set EA = Nothing
       On Error Goto 0
       Set IEX = Nothing
    End If

    call zipAttachments(strScriptFolder & "\")
    call AddAttachment(BMP_Path & "\" & "IEXRunLogs.zip", CurrentRun)
    call delAttachments(strScriptFolder)
    call delAttachments(BMP_Path)

End Sub


Function ReportStep(Actual, Status, forwardStep, testStat, runTimeStat)

    Steps.Item(testStep).Field("ST_ACTUAL") = Steps.Item(testStep).Field("ST_ACTUAL") & " " & Actual
    Steps.Item(testStep).Status = Status
    Steps.Post()
    If forwardStep then testStep = testStep + 1	
    If testStat = "failTest" Then testStatus = "Failed"
    If runTimeStat = "Warning" then runTimeErrors = runTimeErrors & " " & "Run-time error " & CStr(Err.Number) & ";"
    If AbortTest Then testStep = Steps.Count

End Function


Sub verifyStage(Debug, CurrentRun, Message)

   If forwardStepWhenNoPassStep Then Call passStep("Step " & CStr(testStep) & " passed.") 
   If Err.Description <> "" Then
     Dim errString
     errString = "Error [" & Err.Number & "] during step " & CStr(testStep) & " : " & Err.Description
     call ReportStep(errString, "Failed", true, "", "Warning")
     Err.Clear
   Else
     call ReportStep(Message, "Passed", true, "", "")
   End If

End Sub


sub getAttachments (ScriptDir, CurrentTest)

   dim TestF
   dim TestList
   dim Item
   dim relevantTest
   dim AttF
   dim AttList
   dim bmpName
   Dim subFso

   Set subFso = CreateObject("Scripting.FileSystemObject")
   Call createFolderStruct(ScriptDir)

   Set TestF = TDConnection.TestFactory
   Set TestList = TestF.NewList("")
   For Each Item In TestList
   	If Item.ID = Cint(CurrentTest.TestId) Then
                Set relevantTest = Item
                Set AttF = relevantTest.Attachments
                Set AttList = AttF.NewList("")
                For index = 1 To AttList.Count
                    AttList(index).Load True, "C:"
                    bmpName = DeriveAttachmentName(AttList(index).Name)
                    subFso.CopyFile AttList(index).FileName, ScriptDir & "\" & bmpName, True
                Next
        End If
   Next

end sub

Function DeriveAttachmentName(TDAttachmentName)
   
   Dim FirstCounter
   Dim SecondCounter
   Dim AttName
   FirstCounter = 0	
   bmpNameHolder = Split(TDAttachmentName, "_")
   If UBound(bmpNameHolder) = 0 Then 
   	DeriveAttachmentName = bmpNameHolder(0)
   	Exit Function
   End If
   While bmpNameHolder(FirstCounter) <> BMP_Set And FirstCounter <= (UBound(bmpNameHolder) - 1) 
   	FirstCounter = FirstCounter + 1
   Wend		    	 
   For SecondCounter = FirstCounter to (UBound(bmpNameHolder) - 1)
   	AttName = AttName & bmpNameHolder(SecondCounter) & "_"
   Next
   DeriveAttachmentName = AttName & bmpNameHolder(UBound(bmpNameHolder))	 

End Function


Sub zipAttachments(ScriptDir)

   dim zp
   Set zp = CreateObject("QuickZip.RecursiveZip")
   Call zp.Zip(ScriptDir, "", BMP_Path & "\IEXRunLogs.zip") 
   set zp = nothing
   tdoutput.Print "Test run logs were zipped and uploaded to the server successfully."

End Sub


Sub createFolderStruct(ScriptDir)

   Dim tempDirName
   If Right(ScriptDir, 1) <> "\" Then 
     tempDirName = ScriptDir & "\"
   Else
     tempDirName = ScriptDir
   End If
   Dim locFso
   Set locFso = CreateObject("Scripting.FileSystemObject")
   TDOutput.Print "checking for existence of folder " & ScriptDir
   If Not locFso.FolderExists(ScriptDir) Then
      TDOutput.Print "Folder " & ScriptDir & " does not exist"
      Dim looptest
      Dim foldindex
      Dim tempDirStr
      Dim folderdesc
      Dim Index
      Index = 1
      looptest = True
      Do While looptest
         Index = InStr(Index, tempDirName, "\")
         If Index = 0 Then
            looptest = False
         Else
            tempDirStr = Left(tempDirName, Index - 1)
            If Not locFso.FolderExists(tempDirStr) Then
               Set folderdesc = locFso.CreateFolder(tempDirStr)
               Set folderdesc = Nothing
            Else
               TDOutput.Print "Folder " & tempDirStr & " already exists "
            End If
            Index = Index + 1
         End If
      Loop
    End If
    Set locFso = Nothing
    TDOutput.Print "Folder " & ScriptDir & " created."

End Sub


sub AddAttachment(Attach, CurrentRun)

    Dim atf
    Set atf = CurrentRun.Attachments
    Dim att
    Set att = atf.AddItem(Null)
    att.FileName = Attach
    att.Type = 1
    att.Post()

end sub


sub delAttachments(scriptDir)

    Dim objfso
    Dim tempDirStr

    Set objfso = CreateObject("Scripting.FileSystemObject")
    TempDirStr = Left(ScriptDir, Len(ScriptDir))
    objfso.DeleteFolder (tempDirStr)
    Set objfso = Nothing

end sub


Sub AddURL(URL)

    Dim atf
    Dim att

    Set atf = oCurrentRun.Attachments
    Set att = atf.AddItem(Null)
    att.FileName = URL
    att.Type = 2
    att.Post()

End Sub


Sub Test_Main(byval Debug, ByVal CurrentTestSet, ByVal CurrentTest, ByVal CurrentRun)

    Dim Com
    Dim RecSet
    Dim SQL
    Dim BMPSET
    Dim tempTestLoc
    Dim tempScriptLoc

    On Error Resume Next

    Set oCurrentRun = CurrentRun

    TDOutput.Clear
    Set Steps = CurrentRun.StepFactory.NewList("")
    testStep = 1


    SQL = "SELECT * FROM CYCLE WHERE CY_CYCLE_ID = " & CurrentTestSet.ID
    Set Com = TDConnection.Command
    Com.CommandText = SQL
    Set RecSet = Com.Execute

    IP = RecSet.FieldValue("CY_USER_01")
    ServerNo = RecSet.FieldValue("CY_USER_02")
    BMPSET = RecSet.FieldValue("CY_USER_03")
    'prxyPath = RecSet.FieldValue("CY_USER_04")
    tempTestLoc = "TD"
    tempScriptLoc = "TD_LOG" 
	
    If IP = "" Then
       IP = "127.0.0.1"
    End If

    If prxyPath = "" Then
        prxyPath = "\\" & IP & "\ElementaryActionsProxy\ElementaryActions.dll"
    End If		

    If ServerNo = "" Then
       ServerNo = "2"
    End If
       
    If BMPSET = "" Then   
       BMPSET = "Generic"	     
    End If
	
    TDOutput.Print "Chosen IP is: " & CStr(IP)
    TDOutput.Print "Server no. is " & CStr(ServerNo)
    TDOutput.Print "BMP Set is: " & CStr(BMPSET)

    Set IEX = CreateObject("IEXGateWay_NET.IEX")
    'Create the local folder for the log files:
    strScriptFolder = "C:\Program Files\IEX\" & tempScriptLoc & "_" & "ST_CC_007_3Pplus_2Pminus" & BMPSet & "_" & IP & "_" & ServerNo
    Call createFolderStruct(strScriptFolder)
    strLogFileNameXML = strScriptFolder  & "\TD_" & "ST_CC_007_3Pplus_2Pminus" & ".xml"
    strLogFileNameXSL = strScriptFolder  & "\TD_" & "ST_CC_007_3Pplus_2Pminus" & ".xsl"
    IEX.LogFileName = strLogFileNameXML

    IEX.RemoteHost = IP
    IEX.IEXServerNumber = ServerNo

    TDOutput.Print "Connecting to IEX."

    If Not Debug Then
     IEX.Connect
     If IEX.IsConnected Then
	call ReportStep("IEX is Connected.", "Passed", false, "", "")
	TDOutput.Print "IEX is connected."
	TDOutput.Print "Test " & CurrentTest.Test.Name & " - Test begins."
   	
        BMP_Path = "C:\Program Files\IEX\" & tempTestLoc & "_" & "ST_CC_007_3Pplus_2Pminus" & BMPSet & "_" & IP & "_" & ServerNo
	BMP_Set = BMPSET
        
        Call getAttachments(BMP_Path, CurrentTest)
        TDOutput.Print "Attachments are downloaded."      
        Call verifyStage(Debug, CurrentRun, "Setup completed successfully.")        	
        IEX.VisibleGateway = True 'Causes the IEXGateway to display when running from VBScript.
	Call Main 
	Call CleanUp (Debug, CurrentTest, CurrentRun)
	Call verifyStage(Debug, CurrentRun, "Clean-up completed successfully.")     

        If testStatus = "Failed" Then
          CurrentTest.Status = "Failed"
          CurrentRun.Status = "Failed"
        End If
     Else
	TDOutput.Print "Test (" & CurrentTest.Test.Name & ") aborted because the gateway could not connect to the IEX Server."
     	Call ReportStep("Could not connect to IEX Server", "Failed", false, "failTest", "")
        CurrentRun.Status = "Failed"
        CurrentTest.Status = "Failed"
	Call CleanUp (Debug, CurrentTest, CurrentRun)        
     End If

     CurrentRun.Field("RN_USER_02") = runTimeErrors 	
     Set Steps = nothing
     Set oCurrentRun = nothing
    End If

End Sub


Sub InitEA(gw)
   On Error Resume Next
   Dim prxy
   Set prxy = CreateObject("DotNetProxy.Proxy")
   If prxy is nothing Then
	Call ReportStep("Failed to create EA object.", "Failed", true, "failTest", "")
	Exit sub
   End If
   prxy.LoadFile(prxyPath)
   Set EA = prxy.GetClass("ElementaryActions.Methods")
   If EA is nothing Then
	Call ReportStep("Failed to create EA object.", "Failed", true, "failTest", "")
	Exit Sub	
   End If
   EA.Init(gw.GetDotNetGateway)
   On Error Goto 0
End Sub


Dim EA
Dim AbortTest
Dim objfso
Dim strLogFileNameXML
Dim strLogFileNameXSL
Dim IEXServerNumber
Dim RemoteHost
Dim strScriptFolder
Dim success
Dim reason
Dim StepInfo
Dim Comment
Dim Steps
Dim testStatus
Dim runTimeErrors
Dim testStep
Dim IEX
Dim BMP_Set
Dim BMP_Path
Dim snapshotURL
Dim Res
Dim URLValueHolder
Dim counter
Dim IP
Dim ServerNo
Dim prxyPath
Dim oCurrentRun
Dim tempString
Dim forwardStepWhenNoPassStep
dim TEST_NAME

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


