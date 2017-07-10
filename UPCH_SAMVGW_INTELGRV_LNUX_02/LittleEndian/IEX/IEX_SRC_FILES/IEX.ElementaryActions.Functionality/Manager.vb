Imports System.Runtime.InteropServices
Imports System.Reflection
Imports FailuresHandler
Imports System.Net.NetworkInformation
Imports OpenQA.Selenium.Support.UI
Imports OpenQA.Selenium.Firefox
Imports OpenQA.Selenium

<ClassInterface(ClassInterfaceType.AutoDual)> _
Public Class Manager
    Inherits IEX.ElementaryActions.Manager
    Public UI As IEX.ElementaryActions.EPG.SF.UI
    Public CurrentChannel As String
    'This Variable States That The Project Is COGECO Like (GW Without EPG) If False Acts Like IPC (GW With EPG)
    Private _logFile As String
    Private _project As Project

#Region "Local Variables"
    Dim _LogFilePath As String
    Dim _PVR As PVR
    Dim _PCAT As PCAT
    Dim _MEM As MEM
    Dim _STBSettings As STBSettings
    Dim _VOD As VOD
#End Region

#Region "Properties"

    Public Property PVR() As PVR
        Get
            Return _PVR
        End Get
        Set(ByVal value As PVR)
            _PVR = value
        End Set
    End Property

    Public Property PCAT() As PCAT
        Get
            Return _PCAT
        End Get
        Set(ByVal value As PCAT)
            _PCAT = value
        End Set
    End Property

    Public Property MEM() As MEM
        Get
            Return _MEM
        End Get
        Set(ByVal value As MEM)
            _MEM = value
        End Set
    End Property

    Public Property STBSettings() As STBSettings
        Get
            Return _STBSettings
        End Get
        Set(ByVal value As STBSettings)
            _STBSettings = value
        End Set
    End Property

    Public Property VOD() As VOD
        Get
            Return _VOD
        End Get
        Set(ByVal value As VOD)
            _VOD = value
        End Set
    End Property

    Public ReadOnly Property LogFilePath() As String
        Get
            Return _LogFilePath
        End Get
    End Property

    Public Property LogFile() As String
        Get
            Return _logFile
        End Get
        Set(ByVal value As String)
            _logFile = value
        End Set
    End Property

    Public Property Project() As Project
        Get
            Return _project
        End Get
        Set(ByVal value As Project)
            _project = value
        End Set
    End Property

#End Region

#Region "Constructors"

    Public Sub New()

    End Sub

    Overridable Function Init(ByRef _iex As IEXGateway._IEX, ByVal pName As String, Optional ByRef errorDescription As String = "") As Boolean
        Dim res As IEXGateway._IEXResult

        Try
            Try
                MyBase.Initialize(_iex, "Functionality." + pName)
                MyBase.Initialize(_iex, "FunctionalityCS")
            Catch ex As Exception
                _iex.LogComment("Manager Init : ERROR Failed To Initialize EA Object Exception Occured ! : " + ex.Message.ToString, False, False, False, CByte(10), "red")
                errorDescription = "Manager Init : ERROR Failed To Initialize EA Object Exception Occured ! : " + ex.Message.ToString
                Return False
            End Try

            If pName.ToLower.Contains("cogeco.prd") Then
                pName = "COGECO"
            End If

            Try
                Dim assemblyFile As String = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)
                Dim SF_Object As Assembly = Assembly.LoadFrom(assemblyFile + "\IEX.ElementaryActions.EPG.SF." + pName.ToUpper + ".dll")
                Dim UI_Type As Type = SF_Object.GetType("IEX.ElementaryActions.EPG.SF." + pName.ToUpper + ".UI")
                UI = Activator.CreateInstance(UI_Type, _iex)
            Catch ex As Exception
                _iex.LogComment("Manager Init : Failed To Create UI Object Instance", False, False, False, CByte(10), "red")
                errorDescription = "Manager Init : Failed To Create UI Object Instance"
                Return False
            End Try

            IEX.ElementaryActions.Objects.FailureHandling.ExitCodes = GetType(ExitCodes)
            FailuresHandler.ExceptionUtils.ExIEX = _iex

            If UI.Utils.GetEnvironmentIni = False Then
                UI.Utils.LogCommentFail("Manager Init : Failed To Read Environment.ini")
                errorDescription = "Manager Init : Failed To Read Environment.ini"
                Return False
            End If

            Me.Project = New Project(UI.Utils.GetValueFromEnvironment("Project"))
            UI.Project = Me.Project.Name

            Me._LogFilePath = UI.Utils.GetValueFromEnvironment("LogDirectory")

            res = ChangeLogFileName(Me._LogFilePath)
            If Not res.CommandSucceeded Then
                UI.Utils.LogCommentFail("Manager Init : Failed To Change Log File Name !!!")
                errorDescription = "Manager Init : Failed To Change Log File Name !!!"
                Return False
            End If

            UI.Utils.StartHideFailures("Initializing EA..")
            ' write dll version
            UI.Utils.LogCommentImportant(System.Reflection.Assembly.GetExecutingAssembly().FullName)
            UI.Utils.LogCommentImportant("RUNNING ON PROJECT : " + pName)

            System.Threading.Thread.CurrentThread.CurrentCulture = New Globalization.CultureInfo("en-GB", False)

            Try
                PVR = New PVR(_iex, Me)
            Catch ex As Exception
                _iex.ForceHideFailure()
                UI.Utils.LogCommentFail("Manager Init : Failed To Create PVR Object Instance")
                errorDescription = "Manager Init : Failed To Create PVR Object Instance"
                Return False
            End Try

            Try
                PCAT = New PCAT(_iex, Me)
            Catch ex As Exception
                _iex.ForceHideFailure()
                UI.Utils.LogCommentFail("Manager Init : Failed To Create PCAT Object Instance")
                errorDescription = "Manager Init : Failed To Create PCAT Object Instance"
                Return False
            End Try

            Try
                STBSettings = New STBSettings(_iex, Me)
            Catch ex As Exception
                _iex.ForceHideFailure()
                UI.Utils.LogCommentFail("Manager Init : Failed To Create STBSettings Object Instance")
                errorDescription = "Manager Init : Failed To Create STBSettings Object Instance"
                Return False
            End Try

            Try
                MEM = New MEM(_iex, Me)
            Catch ex As Exception
                _iex.ForceHideFailure()
                UI.Utils.LogCommentFail("Manager Init : Failed To Create MEM Object Instance")
                errorDescription = "Manager Init : Failed To Create MEM Object Instance"
                Return False
            End Try

            Try
                VOD = New VOD(_iex, Me)
            Catch ex As Exception
                _iex.ForceHideFailure()
                UI.Utils.LogCommentFail("Manager Init : Failed To Create VOD Object Instance")
                errorDescription = "Manager Init : Failed To Create VOD Object Instance"
                Return False
            End Try

            Try
                If Not UI.Utils.GetTextsFromDictionary() Then
                    _iex.ForceHideFailure()
                    UI.Utils.LogCommentWarning("Manager Init : WARNNING Failed To Get Dictionary")
                End If
            Catch ex As Exception
                UI.Utils.LogCommentWarning("Manager Init : WARNNING Failed To Get Dictionary")
            End Try


            If Not UI.Utils.GetMilestonesFromIni() Then
                _iex.ForceHideFailure()
                UI.Utils.LogCommentFail("Manager Init : ERROR Failed To Get Milestones")
                errorDescription = "Manager Init : ERROR Failed To Get Milestones"
                Return False
            End If

            If Me.Project.CanSwitchRF Then
                Try
                    UI.Utils.SwitchRF()
                Catch ex As Exception
                    UI.Utils.LogCommentWarning("WORAROUND : Unable To Do RF Switch But Will Continue")
                End Try
            ElseIf Me.Project.IsMoblie Then
                UI.Utils.InitAgent()
            End If

        Catch ex As Exception
            _iex.ForceHideFailure()
            UI.Utils.LogCommentFail("Manager Init : ERROR Exception Occured ! : " + ex.Message.ToString)
            errorDescription = "Manager Init : ERROR Exception Occured ! : " + ex.Message.ToString
            Return False
        End Try

        _iex.ForceHideFailure()
        Return True

    End Function

#End Region

#Region "API"
    ''' <summary>
    '''    Dummy function to test the EA
    ''' </summary>
    ''' <returns>IEXGateway._IEXResult</returns>
    ''' <remarks></remarks>
    <ComVisible(False)> _
    Public Function DUMMY(ByVal keyname As String, ByVal toSet As Boolean) As IEXGateway._IEXResult
        Return Invoke("Dummy", keyname, toSet, Me)
    End Function

    ''' <summary>
    '''   Tunning To A Locked Channel
    ''' </summary>
    ''' <param name="ChannelNumber">Channel Number To Tune To</param>
    ''' <param name="CheckForVideo">Optional Parameter Default = True,If True Then Checks For Video Else Not Checking For Video</param>
    ''' <returns>IEXGateway._IEXResult</returns>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>300 - NavigationFailure</para> 
    ''' <para>301 - DictionaryFailure</para> 
    ''' <para>304 - IRVerificationFailure</para> 
    ''' <para>322 - VerificationFailure</para> 
    ''' <para>328 - INIFailure</para> 
    ''' <para>332 - NoValidParameters</para> 
    ''' <para>334 - VideoNotPresent</para> 
    ''' <para>349 - ReturnToLiveFailure</para> 
    ''' <para>351 - SurfingFailure</para> 
    ''' <para>355 - TuneToChannelFailure</para> 
    ''' </remarks>
    Public Function TuneToLockedChannel(ByVal ChannelNumber As String, Optional ByVal CheckForVideo As Boolean = True) As IEXGateway._IEXResult
        Return Invoke("TuneToLockedChannel", ChannelNumber, CheckForVideo, Me)
    End Function

    ''' <summary>
    '''    Surfing To A Channel In Guide
    ''' </summary>
    ''' <param name="ChannelNumber">Channel Number To Tune To</param>
    ''' <returns>IEXGateway._IEXResult</returns>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>300 - NavigationFailure</para> 
    ''' <para>304 - IRVerificationFailure</para> 
    ''' <para>322 - VerificationFailure</para> 
    ''' <para>332 - NoValidParameters</para> 
    ''' </remarks>
    Public Function SurfToChannelInGuide(ByVal ChannelNumber As String) As IEXGateway._IEXResult
        Return Invoke("SurfToChannelInGuide", ChannelNumber, Me)
    End Function

    ''' <summary>
    '''   Setting Audio Track On Action Bar
    ''' </summary>
    ''' <param name="Language">Requested Language</param>
    ''' <param name="Type">Requested Type - STEREO Or DOLBY DIGITAL</param>
    ''' <param name="VerifyAudio">Optional Parameter Default = False,If True Verifies Audio</param>
    ''' <returns>IEXGateway._IEXResult</returns>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>301 - DictionaryFailure</para> 
    ''' <para>332 - NoValidParameters</para> 
    ''' <para>336 - AudioNotPresent</para> 
    ''' </remarks>
    Public Function ChangeAudioTrack(ByVal Language As EnumLanguage, ByVal Type As EnumAudioType, Optional ByVal VerifyAudio As Boolean = False) As IEXGateway._IEXResult
        Return Invoke("ChangeAudioTrack", Language, Type, VerifyAudio, Me)
    End Function

    ''' <summary>
    '''   Sets Subtitles Language From Action Bar
    ''' </summary>
    ''' <param name="Language">Language To Set To By Enum Language</param>
    ''' <returns>IEXGateway._IEXResult</returns>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>300 - NavigationFailure</para> 
    ''' <para>301 - DictionaryFailure</para> 
    ''' <para>302 - EmptyEpgInfoFailure</para> 
    ''' <para>319 - SetSubtitlesLanguageFailure</para> 
    ''' <para>332 - NoValidParameters</para> 
    ''' </remarks>
    Public Function SubtitlesLanguageChange(ByVal Language As EnumLanguage, Optional ByVal _Type As EnumSubtitleType = EnumSubtitleType.NORMAL) As IEXGateway._IEXResult
        Return Invoke("SubtitlesLanguageChange", Language, Me, _Type)
    End Function

    ''' <summary>
    '''   Enters The PIN
    ''' </summary>
    ''' <param name="PIN">PIN Requested To Enter</param>
    ''' <param name="NextState">The Next State After Entering PIN</param>
    ''' <returns>IEXGateway._IEXResult</returns>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>300 - NavigationFailure</para> 
    ''' <para>301 - DictionaryFailure</para> 
    ''' <para>302 - EmptyEpgInfoFailure</para> 
    ''' <para>304 - IRVerificationFailure</para> 
    ''' <para>322 - VerificationFailure</para> 
    ''' <para>328 - INIFailure</para> 
    ''' <para>332 - NoValidParameters</para> 
    ''' </remarks>
    Public Function EnterPIN(ByVal PIN As String, ByVal NextState As String) As IEXGateway._IEXResult
        Return Invoke("EnterPIN", PIN, NextState, Me)
    End Function

    ''' <summary>
    '''   Enters The PIN
    ''' </summary>
    ''' <param name="NextState">The Next State After Entering PIN</param>
    ''' <returns>IEXGateway._IEXResult</returns>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>300 - NavigationFailure</para> 
    ''' <para>301 - DictionaryFailure</para> 
    ''' <para>302 - EmptyEpgInfoFailure</para> 
    ''' <para>304 - IRVerificationFailure</para> 
    ''' <para>322 - VerificationFailure</para> 
    ''' <para>328 - INIFailure</para> 
    ''' <para>332 - NoValidParameters</para> 
    ''' </remarks>
    Public Function EnterDeafultPIN(ByVal NextState As String) As IEXGateway._IEXResult
        Return Invoke("EnterPIN", "", NextState, Me)
    End Function

    ''' <summary>
    '''    Do STB Health Check By Checking Tune To Channel,Checking For Video,Navigate And Get Event Name From Guide And Lunching The Action Bar
    ''' </summary>
    ''' <param name="ChannelNumber">Channel Number To Tune To</param>
    ''' <returns>IEXGateway._IEXResult</returns>
    ''' <remarks></remarks>
    Public Function HealthCheck(ByVal ChannelNumber As String) As IEXGateway._IEXResult
        Return Invoke("HealthCheck", ChannelNumber, Me)
    End Function

    ''' <summary>
    '''    Sync The Stream By Given Time
    ''' </summary>
    ''' <param name="StreamStartTime">The Stream Start Time</param>
    ''' <param name="StreamEndTime">The Stream End Time</param>
    ''' <param name="MinutesFromStreamEndToReboot">Minutes Before Stream End Time To Perform Power Cycle</param>
    ''' <param name="TestDurationInMin">Optional Parameter Default = -1 : On Default TestStartTime Is Checked Else The Test Duration In Minutes Is Checked</param>
    ''' <param name="TestStartTime">Optional Parameter Default = "" : The Test Requested Start Time</param>
    ''' <returns>IEXGateway._IEXResult</returns>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>301 - DictionaryFailure</para> 
    ''' <para>309 - GetEpgTimeFailure</para> 
    ''' <para>324 - MountFailure</para> 
    ''' <para>327 - SyncFailure</para> 
    ''' <para>328 - INIFailure</para> 
    ''' <para>332 - NoValidParameters</para> 
    ''' <para>349 - ReturnToLiveFailure</para> 
    ''' </remarks>
    Public Function SyncStream(ByVal StreamStartTime As String, ByVal StreamEndTime As String, ByVal MinutesFromStreamEndToReboot As Integer, Optional ByVal TestDurationInMin As Integer = -1, Optional ByVal TestStartTime As String = "") As IEXGateway._IEXResult
        Return Invoke("SyncStream", StreamStartTime, StreamEndTime, MinutesFromStreamEndToReboot, TestDurationInMin, TestStartTime, Me)
    End Function

    ''' <summary>
    '''   Entering Or Existing Standby
    ''' </summary>
    ''' <param name="IsOn">If True Exiting Standby Else Entering</param>
    ''' <returns>IEXGateway._IEXResult</returns>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>328 - INIFailure</para> 
    ''' <para>358 - StandByFailure</para> 
    ''' </remarks>
    Public Function StandBy(ByVal IsOn As Boolean) As IEXGateway._IEXResult
        Return Invoke("StandBy", IsOn, Me)
    End Function

    ''' <summary>
    '''   Power STB Off And On With Mount
    ''' </summary>
    ''' <param name="SecBeforePowerOn">Seconds Before Powering ON The STB</param>
    ''' <param name="GetOutOfStandBy">Optional Parameter Default = True : If True Gets Out Of Standby</param>
    ''' <param name="FormatSTB">Optional Parameter Default = True : If True Mount The STB With FORMAT Else With NOCLEAN</param>
    ''' <returns>IEXGateway._IEXResult</returns>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>301 - DictionaryFailure</para> 
    ''' <para>324 - MountFailure</para> 
    ''' <para>328 - INIFailure</para> 
    ''' <para>330 - TelnetFailure</para> 
    ''' <para>332 - NoValidParameters</para> 
    ''' <para>358 - StandByFailure</para> 
    ''' </remarks>
    Public Function PowerCycle(Optional ByVal SecBeforePowerOn As Integer = 0, Optional ByVal GetOutOfStandBy As Boolean = True, Optional ByVal FormatSTB As Boolean = True) As IEXGateway._IEXResult
        Return Invoke("PowerCycle", SecBeforePowerOn, GetOutOfStandBy, FormatSTB, Me)
    End Function

    ''' <summary>
    '''    Checks If Video Is Present Or Not Present
    ''' </summary>
    ''' <param name="IsPresent">If True Video Present Else No Video Present</param>
    ''' <param name="CheckFullArea">If True Checks As Much As Possible Of The Screen Else Checks Top Left Corner</param>
    ''' <param name="Timeout">Timeout To Check For Video Presence</param>
    ''' <returns>IEXGateway._IEXResult</returns>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>333 - VideoPresent</para> 
    ''' <para>334 - VideoNotPresent</para> 
    ''' </remarks>
    Public Function CheckForVideo(ByVal IsPresent As Boolean, ByVal CheckFullArea As Boolean, ByVal Timeout As Integer, Optional ByVal isPIP As Boolean = False) As IEXGateway._IEXResult
        Return Invoke("CheckForVideo", "", IsPresent, CheckFullArea, Timeout, isPIP, Me)
    End Function


    ''' <summary>
    '''   Checks If Video Is Present Or Not Present By Coordinates
    ''' </summary>
    ''' <param name="Coordinates">The CheckForVideo Coordinates</param>
    ''' <param name="IsPresent">If True Video Present Else No Video Present</param>
    ''' <param name="Timeout">Timeout To Check For Video Presence</param>
    ''' <returns></returns>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>333 - VideoPresent</para> 
    ''' <para>334 - VideoNotPresent</para> 
    ''' </remarks>
    Public Function CheckForVideo(ByVal Coordinates As String, ByVal IsPresent As Boolean, ByVal Timeout As Integer, Optional ByVal isPIP As Boolean = False) As IEXGateway._IEXResult
        Return Invoke("CheckForVideo", Coordinates, IsPresent, True, Timeout, isPIP, Me)
    End Function


    ''' <summary>
    '''   Checks If Audio Is Present Or Not Present
    ''' </summary>
    ''' <param name="IsPresent">If True Audio Present Else No Audio Present</param>
    ''' <param name="Timeout">Timeout To Check For Audio Presence</param>
    ''' <returns>IEXGateway._IEXResult</returns>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>335 - AudioPresent</para> 
    ''' <para>336 - AudioNotPresent</para> 
    ''' </remarks>
    Public Function CheckForAudio(ByVal IsPresent As Boolean, ByVal Timeout As Integer) As IEXGateway._IEXResult
        Return Invoke("CheckForAudio", IsPresent, Timeout, Me)
    End Function

    ''' <summary>
    '''   Surfs Channel Up Down On Live,Guide,ChannelBar And Channel Lineup And Tune
    ''' </summary>
    ''' <param name="SurfIn">Can Be : Live, Guide Or ChannelBar</param>
    ''' <param name="ChannelNumber">Optional Parameter Default = "" : Channel Number</param>
    ''' <param name="IsNext">Optional Parameter Default = True : If True Surfs To Next Channel Else To Previous</param>
    ''' <param name="NumberOfPresses">Optional Parameter Default = 1 : Number Of Channels To Surf Up OR Down</param>
    ''' <param name="IsPredicted">Optional Parameter Default = Ignore : If The Next Or Previous Channel Is Predicted</param>
    ''' <param name="DoTune">Optional Parameter Default = False : If True Tune To Surfed Channel</param>
    ''' <returns>IEXGateway._IEXResult</returns>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>301 - DictionaryFailure</para> 
    ''' <para>349 - ReturnToLiveFailure</para> 
    ''' <para>334 - VideoNotPresent</para> 
    ''' <para>328 - INIFailure</para> 
    ''' <para>351 - SurfingFailure</para> 
    ''' <para>355 - TuneToChannelFailure</para> 
    ''' <para>328 - INIFailure</para> 
    ''' <para>355 - TuneToChannelFailure</para> 
    ''' <para>322 - VerificationFailure</para> 
    ''' <para>347 - SelectEventFailure</para> 
    ''' </remarks>
    Public Function ChannelSurf(ByVal SurfIn As EnumSurfIn, Optional ByVal ChannelNumber As String = "", Optional ByVal IsNext As Boolean = True, Optional ByVal NumberOfPresses As Integer = -1, Optional ByVal IsPredicted As EnumPredicted = EnumPredicted.Default, Optional ByVal DoTune As Boolean = False, Optional ByVal IsDCA As Boolean = True, Optional ByVal GuideTimeline As String = "", Optional ByVal IsPastEvent As Boolean = False) As IEXGateway._IEXResult
        Return Invoke("ChannelSurf", SurfIn, ChannelNumber, IsNext, NumberOfPresses, IsPredicted, DoTune, IsDCA, GuideTimeline, IsPastEvent, Me)
    End Function


    ''' <summary>
    '''   Surfs Channel Up Down On Live,Guide,ChannelBar And Channel Lineup And Tune
    ''' </summary>
    ''' <param name="SurfIn">Can Be : Live, Guide, ChannelBar Or Channel Lineup</param>
    ''' <param name="Offset">If Smaller Than 0 Surfs Down Else Surfs Up</param>
    ''' <param name="IsPredicted">Optional Parameter Default = Ignore : If The Next Or Previous Channel Is Predicted</param>
    ''' <returns>IEXGateway._IEXResult</returns>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>301 - DictionaryFailure</para> 
    ''' <para>349 - ReturnToLiveFailure</para> 
    ''' <para>334 - VideoNotPresent</para> 
    ''' <para>328 - INIFailure</para> 
    ''' <para>351 - SurfingFailure</para> 
    ''' <para>355 - TuneToChannelFailure</para> 
    ''' <para>328 - INIFailure</para> 
    ''' <para>355 - TuneToChannelFailure</para> 
    ''' <para>322 - VerificationFailure</para> 
    ''' <para>347 - SelectEventFailure</para> 
    ''' </remarks>
    Public Function TuneToChannel(ByVal SurfIn As EnumSurfIn, ByVal Offset As Integer, Optional ByVal IsPredicted As EnumPredicted = EnumPredicted.Default, Optional ByVal IsPastEvent As Boolean = False) As IEXGateway._IEXResult
        Dim IsNext As Boolean
        If Offset < 0 Then
            IsNext = False
        Else
            IsNext = True
        End If

        Return Invoke("ChannelSurf", SurfIn, "", IsNext, Math.Abs(Offset), IsPredicted, True, False,"",IsPastEvent, Me)
    End Function

    ''' <summary>
    '''    Surfs Channel Up Down On Live,Guide,ChannelBar And Channel Lineup Without Tune 
    ''' </summary>
    ''' <param name="SurfIn">Importent !! Live Is not a Valid param Can Be : Guide, ChannelBar Or Channel Lineup</param>
    ''' <param name="Offset">If Smaller Than 0 Surfs Down Else Surfs Up</param>
    ''' <returns>IEXGateway._IEXResult</returns>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>301 - DictionaryFailure</para> 
    ''' <para>349 - ReturnToLiveFailure</para> 
    ''' <para>334 - VideoNotPresent</para> 
    ''' <para>328 - INIFailure</para> 
    ''' <para>351 - SurfingFailure</para> 
    ''' <para>355 - TuneToChannelFailure</para> 
    ''' <para>328 - INIFailure</para> 
    ''' <para>355 - TuneToChannelFailure</para> 
    ''' <para>322 - VerificationFailure</para> 
    ''' <para>347 - SelectEventFailure</para> 
    ''' </remarks>
    Public Function SurfToChannel(ByVal SurfIn As EnumSurfIn, ByVal Offset As Integer, Optional ByVal IsPastEvent As Boolean = False) As IEXGateway._IEXResult
        Dim IsNext As Boolean
        If Offset < 0 Then
            IsNext = False
        Else
            IsNext = True
        End If

        Return Invoke("ChannelSurf", SurfIn, "", IsNext, Math.Abs(Offset), EnumPredicted.Ignore, False, False,IsPastEvent, Me)
    End Function

    ''' <summary>
    '''   Tune To Channel
    ''' </summary>
    ''' <param name="ChannelNumber">The Channel Number</param>
    ''' <param name="ReturnToLive">If True Returnes To Live Before Tunning Else Not (For Tunning From Locked Channel)</param>
    ''' <returns>IEXGateway._IEXResult</returns>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>301 - DictionaryFailure</para> 
    ''' <para>322 - VerificationFailure</para> 
    ''' <para>328 - INIFailure</para> 
    ''' <para>334 - VideoNotPresent</para> 
    ''' <para>349 - ReturnToLiveFailure</para> 
    ''' <para>351 - SurfingFailure</para> 
    ''' <para>355 - TuneToChannelFailure</para> 
    ''' </remarks>
    Public Function TuneToChannel(ByVal ChannelNumber As String, Optional ByVal ReturnToLive As Boolean = True) As IEXGateway._IEXResult
        Return Invoke("TuneToChannel", ChannelNumber, ReturnToLive, Me)
    End Function

    ''' <summary>
    '''   Tune To Radio Channel
    ''' </summary>
    ''' <param name="ChannelNumber">The Channel Number</param>
    ''' <returns>IEXGateway._IEXResult</returns>
    ''' <remarks>
    ''' <para>301 - DictionaryFailure</para> 
    ''' <para>322 - VerificationFailure</para> 
    ''' <para>328 - INIFailure</para> 
    ''' <para>333 - VideoPresent</para> 
    ''' <para>334 - VideoNotPresent</para> 
    ''' <para>335 - AudioPresent</para> 
    ''' <para>336 - AudioNotPresent</para> 
    ''' <para>349 - ReturnToLiveFailure</para> 
    ''' <para>351 - SurfingFailure</para> 
    ''' <para>355 - TuneToChannelFailure</para> 
    ''' </remarks>
    Public Function TuneToRadioChannel(ByVal ChannelNumber As String) As IEXGateway._IEXResult
        Return Invoke("TuneToRadioChannel", ChannelNumber, Me)
    End Function

    ''' <summary>
    '''   Surfs Channel Up Down Or DCA On Live With Subtitles
    ''' </summary>
    ''' <param name="ChannelNumber">Optional Parameter Default = "" : Channel Number</param>
    ''' <param name="IsNext">Optional Parameter Default = True : If True Surfs To Next Channel Else To Previous</param>
    ''' <param name="NumberOfPresses">Optional Parameter Default = 1 : Number Of Channels To Surf Up OR Down</param>
    ''' <param name="IsPredicted">Optional Parameter Default = True : If The Next Or Previous Channel Is Predicted</param>
    ''' <returns>IEXGateway._IEXResult</returns>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>301 - DictionaryFailure</para> 
    ''' <para>322 - VerificationFailure</para> 
    ''' <para>328 - INIFailure</para> 
    ''' <para>332 - NoValidParameters</para> 
    ''' <para>334 - VideoNotPresent</para> 
    ''' <para>349 - ReturnToLiveFailure</para> 
    ''' <para>351 - SurfingFailure</para> 
    ''' <para>355 - TuneToChannelFailure</para> 
    ''' </remarks>
    Public Function ChannelSurfWithSubtitles(Optional ByVal ChannelNumber As String = "", Optional ByVal IsNext As Boolean = True, Optional ByVal NumberOfPresses As Integer = 1, Optional ByVal IsPredicted As EnumPredicted = EnumPredicted.Ignore) As IEXGateway._IEXResult
        Return Invoke("ChannelSurfWithSubtitles", ChannelNumber, IsNext, NumberOfPresses, IsPredicted, Me)
    End Function

    ''' <summary>
    '''   Surfs Channel Up Or Down And Then To Next Event In ChannelBar
    ''' </summary>
    ''' <param name="IsNext">If True Surfs To Next Channel Else To Previous</param>
    ''' <param name="NumberOfPressesUpDown">Optional Parameter Default = 1 : Number Of Surfs</param>
    ''' <returns>IEXGateway._IEXResult</returns>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>328 - INIFailure</para> 
    ''' <para>347 - SelectEventFailure</para> 
    ''' <para>351 - SurfingFailure</para> 
    ''' </remarks>
    Public Function ChannelBarSurfFuture(ByVal IsNext As Boolean, Optional ByVal NumberOfPressesUpDown As Integer = 1) As IEXGateway._IEXResult
        Return Invoke("ChannelBarSurfFuture", IsNext, NumberOfPressesUpDown, Me)
    End Function

    ''' <summary>
    '''   Browses In Guide Left Right Or Up Down Directions
    ''' </summary>
    ''' <param name="MoveRight">If True Moves Right Else Moves Left</param>
    ''' <param name="NumOfRightLeftPresses">Optional Parameter Default = 1 : Number Of Moves Right Or Left</param>
    ''' <param name="MoveChannelUp">Optional Parameter Default = True : If True Moves Up Else Moves Down</param>
    ''' <param name="NumOfUpDownPresses">Optional Parameter Default = 0 : Number Of Moves Up OR Down</param>
    ''' <returns>IEXGateway._IEXResult</returns>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>300 - NavigationFailure</para> 
    ''' <para>351 - SurfingFailure</para> 
    ''' </remarks>
    Public Function BrowseGuideFuture(ByVal MoveRight As Boolean, Optional ByVal NumOfRightLeftPresses As Integer = 1, Optional ByVal MoveChannelUp As Boolean = True, Optional ByVal NumOfUpDownPresses As Integer = 0) As IEXGateway._IEXResult
        Return Invoke("BrowseGuideFuture", MoveRight, NumOfRightLeftPresses, MoveChannelUp, NumOfUpDownPresses, Me)
    End Function

    ''' <summary>
    '''   Raise The Action Bar
    ''' </summary>
    ''' <param name="FromPlayback">Optional Parameter Default = False, If True Lunching Action Bar From Playback State</param>
    ''' <returns>IEXGateway._IEXResult</returns>
    ''' <remarks></remarks>
    Public Function LaunchActionBar(Optional ByVal FromPlayback As Boolean = False) As IEXGateway._IEXResult
        Return Invoke("LaunchActionBar", FromPlayback, Me)
    End Function

    ''' <summary>
    '''   Add Channel To Favorites From Action Bar
    ''' </summary>
    ''' <returns>IEXGateway._IEXResult</returns>
    ''' <remarks></remarks>
    Public Function AddToFavouritesFromBanner() As IEXGateway._IEXResult
        Return Invoke("AddToFavouritesFromBanner", Me)
    End Function

    ''' <summary>
    '''    Remove Channel To Favorites From Action Bar
    ''' </summary>
    ''' <returns>IEXGateway._IEXResult</returns>
    ''' <remarks></remarks>
    Public Function RemoveFavouritesFromBanner() As IEXGateway._IEXResult
        Return Invoke("RemoveFavouritesFromBanner", Me)
    End Function

    ''' <summary>
    '''   Lock Channel From Action Bar
    ''' </summary>
    ''' <returns>IEXGateway._IEXResult</returns>
    ''' <remarks></remarks>
    Public Function LockChannelFromBanner(Optional ByVal EnterPIN As Boolean = True) As IEXGateway._IEXResult
        Return Invoke("LockChannelFromBanner", EnterPIN, Me)
    End Function

    ''' <summary>
    '''   UnLock Channel From Action Bar
    ''' </summary>
    ''' <returns>IEXGateway._IEXResult</returns>
    ''' <remarks></remarks>
    Public Function UnLockChannelFromBanner() As IEXGateway._IEXResult
        Return Invoke("UnLockChannelFromBanner", Me)
    End Function

    ''' <summary>
    '''   UnLock Event From Channel Bar
    ''' </summary>
    ''' <returns>IEXGateway._IEXResult</returns>
    ''' <remarks></remarks>
    Public Function UnLockEvent() As IEXGateway._IEXResult
        Return Invoke("UnLockEvent", Me)
    End Function

    ''' <summary>
    '''   Return To Live Viewing By Pressing MENU And Then SELECT
    ''' </summary>
    ''' <param name="CheckForVideo">Optional Parameter Default = False. If True Checks For Video After Returnning To Live Viewing</param>
    ''' <returns>IEXGateway._IEXResult</returns>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>301 - DictionaryFailure</para> 
    ''' <para>334 - VideoNotPresent</para> 
    ''' <para>349 - ReturnToLiveFailure</para> 
    ''' </remarks>
    Public Function ReturnToLiveViewing(Optional ByVal CheckForVideo As Boolean = False) As IEXGateway._IEXResult
        Return Invoke("ReturnToLiveViewing", CheckForVideo, Me)
    End Function

    ''' <summary>
    '''   Return To Playback Viewing
    ''' </summary>
    ''' <param name="CheckForVideo">Optional Parameter Default = False. If True Checks For Video After Returnning To Playback Viewing</param>
    ''' <returns>IEXGateway._IEXResult</returns>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>334 - VideoNotPresent</para> 
    ''' </remarks>
    Public Function ReturnToPlaybackViewing(Optional ByVal CheckForVideo As Boolean = False) As IEXGateway._IEXResult
        Return Invoke("ReturnToPlaybackViewing", CheckForVideo, Me)
    End Function

    ''' <summary>
    '''   Verify Event Duration Is Greater Or Smaller From Given Duration
    ''' </summary>
    ''' <param name="EventKeyName">The Key Of The Event</param>
    ''' <param name="DurationInSec">Duration In Seconds To Check</param>
    ''' <param name="IsDurationLarger">If True Larger Else Smaller</param>
    ''' <returns>IEXGateway._IEXResult</returns>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>305 - PCATFailure</para> 
    ''' <para>307 - GetStreamInfoFailure</para> 
    ''' <para>322 - VerificationFailure</para> 
    ''' <para>328 - INIFailure</para> 
    ''' <para>330 - TelnetFailure</para> 
    ''' <para>331 - CopyFileFailure</para> 
    ''' <para>332 - NoValidParameters</para> 
    ''' <para>338 - EventNotExistsFailure</para> 
    ''' </remarks>
    Public Function VerifyEventDuration(ByVal EventKeyName As String, ByVal DurationInSec As Integer, ByVal IsDurationLarger As Boolean) As IEXGateway._IEXResult
        Return Invoke("VerifyEventDuration", EventKeyName, DurationInSec, IsDurationLarger, Me)
    End Function

	''' <summary>
    '''   Performs dynamic navigation applicable only for GET LIVE Environment
    ''' </summary>
    Public Function VODDataBase(ByVal CatalogueName As String) As IEXGateway._IEXResult
        Return Invoke("VODDataBase", CatalogueName, Me)
    End Function
    '''<summary>
    ''' Performs Dynamic Layered Asset Play
    ''' Applicable only for GET Live Environment
    ''' </summary>
    Public Function LayerFocusandPlay() As IEXGateway._IEXResult
        Return Invoke("LayerFocusandPlay", Me)
    End Function
    '''<summary>
    ''' Applicable only for GET Live Environment
    ''' Performs Transactional VOD Asset
    ''' </summary>
    Public Function TransactionalVOD(ByVal transaction As Integer) As IEXGateway._IEXResult
        Return Invoke("TransactionalVOD", transaction, Me)
    End Function
    '''   Search Event In VOD
    ''' </summary>
    ''' <param name="EventName">The Event Name To Find</param>
    ''' <param name="Navigate">If True Navigates To VOD</param>
    ''' <returns>IEXGateway._IEXResult</returns>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>300 - NavigationFailure</para> 
    ''' <para>302 - EmptyEpgInfoFailure</para> 
    ''' <para>304 - IRVerificationFailure</para> 
    ''' <para>322 - VerificationFailure</para> 
    ''' </remarks>
    Public Function SearchVodEvent(ByVal EventName As String, Optional ByVal Navigate As Boolean = True) As IEXGateway._IEXResult
        Return Invoke("SearchVodEvent", EventName, Navigate, Me)
    End Function

    ''' <summary>
    ''' Search Content in My Devices
    ''' </summary>
    ''' <param name="Content">The Content To Find</param>
    ''' <param name="Navigate">If True Navigates To MY DEVICES</param>
    ''' <returns>IEXGateway._IEXResult</returns>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>300 - NavigationFailure</para> 
    ''' <para>302 - EmptyEpgInfoFailure</para> 
    ''' <para>304 - IRVerificationFailure</para> 
    ''' <para>322 - VerificationFailure</para> 
    ''' </remarks>
    Public Function SearchMCContent(ByVal Content As MediaContent, Optional ByVal Navigate As Boolean = True, Optional ByVal fromDeviceNavigator As Boolean = True) As IEXGateway._IEXResult
        Return Invoke("SearchMCContent", Content, Navigate, fromDeviceNavigator, Me)
    End Function


    ''' <summary>
    ''' Playback a content from My Devices
    ''' </summary>
    ''' <param name="Content">The Content To play</param>
    ''' <param name="playMode">Optional Parameter Default="", Playmode can be PLAY,SLIDESHOW</param>
    ''' <param name="playbackSetting">Optional Parameter Default="", Slideshow Setting</param>
    ''' <returns>IEXGateway._IEXResult</returns>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>300 - NavigationFailure</para> 
    ''' <para>302 - EmptyEpgInfoFailure</para> 
    ''' <para>304 - IRVerificationFailure</para> 
    ''' <para>322 - VerificationFailure</para> 
    ''' </remarks>
    Public Function PlaybackMCContent(ByVal Content As MediaContent, Optional ByVal playMode As EnumMCPlayMode = EnumMCPlayMode.PLAY, Optional ByVal playbackSetting As String = "") As IEXGateway._IEXResult
        Return Invoke("PlaybackMCContent", Content, playMode, playbackSetting, Me)
    End Function

    ''' <summary>
    ''' In case of executing from Iexecuter the this function is taking care that a new folder
    ''' under the name of the test with time stamp will create for the 
    ''' log in the path alocated for the logs in the XML file
    ''' By this every execution of the same test will create a new log file
    ''' and the log will be located in the local machine and not in the tests folder (shared drive in most of the cases)
    ''' </summary>
    ''' <param name="LogFilePath">The Path Of The IEX Log</param>
    ''' <returns>IEXGateway._IEXResult</returns>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>329 - IEXSystemError</para> 
    ''' <para>332 - NoValidParameters</para> 
    ''' </remarks>
    <ComVisible(False)> _
    Public Function ChangeLogFileName(ByVal LogFilePath As String) As IEXGateway._IEXResult
        Return Invoke("ChangeLogFileName", LogFilePath, Me)
    End Function

    ''' <summary>
    '''   Get Current Event Left Time In Seconds To End Time Of The Event
    ''' </summary>
    ''' <param name="TimeLeftInSec">ByRef The Returned Left Time</param>
    ''' <param name="IsPayPerView">True For PPV Event, False Otherwise</param>
    ''' <returns>IEXGateway._IEXResult</returns>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>337 - ParseEventTimeFailure</para> 
    ''' <para>350 - ParsingFailure</para> 
    ''' </remarks>
    Public Function GetCurrentEventLeftTime(ByRef TimeLeftInSec As Integer, Optional ByVal IsPayPerView As Boolean = False) As IEXGateway._IEXResult
        Dim res As IEXGateway.IEXResult

        res = Invoke("GetCurrentEventLeftTime", IsPayPerView, Me)
        Try
            TimeLeftInSec = DirectCast(ReturnValues(0), Integer)
        Catch ex As Exception
            TimeLeftInSec = -1
        End Try

        Return res
    End Function

    ''' <summary>
    '''   Get Current Event Time From Start In Seconds
    ''' </summary>
    ''' <param name="TimeOverInSec">ByRef The Returned Passed Time</param>
    ''' <returns>IEXGateway._IEXResult</returns>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>337 - ParseEventTimeFailure</para> 
    ''' <para>350 - ParsingFailure</para> 
    ''' </remarks>
    Public Function GetCurrentEventTimeFromStart(ByRef TimeOverInSec As Integer, Optional ByVal PayPerView As Boolean = False) As IEXGateway._IEXResult
        Dim res As IEXGateway.IEXResult

        res = Invoke("GetCurrentEventTimeFromStart", PayPerView, Me)
        Try
            TimeOverInSec = DirectCast(ReturnValues(0), Integer)
        Catch ex As Exception
            TimeOverInSec = -1
        End Try

        Return res
    End Function

    ''' <summary>
    '''   Waits Until Event Start
    ''' </summary>
    ''' <param name="EventKeyName">Key Of The Event</param>
    ''' <param name="StartGuardTime">Optional Parameter. Default=""</param>
    ''' <returns>IEXGateway._IEXResult</returns>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>301 - DictionaryFailure</para> 
    ''' <para>309 - GetEpgTimeFailure</para> 	
    ''' <para>332 - NoValidParameters</para> 
    ''' <para>334 - VideoNotPresent</para> 
    ''' <para>338 - EventNotExistsFailure</para> 
    ''' <para>349 - ReturnToLiveFailure</para> 
    ''' <para>350 - ParsingFailure</para> 
    ''' </remarks>
    Public Function WaitUntilEventStarts(ByVal EventKeyName As String, Optional ByVal StartGuardTime As String = "") As IEXGateway._IEXResult
        Return Invoke("WaitUntilEventStarts", EventKeyName, StartGuardTime, Me)
    End Function

    ''' <summary>
    '''  Waits Until Event Ends
    ''' </summary>
    ''' <param name="EventKeyName">Key Of The Event</param>
    ''' <param name="EndGuardTime">Optional Parameter. Default=""</param>
    ''' <returns>IEXGateway._IEXResult</returns>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>301 - DictionaryFailure</para> 
    ''' <para>309 - GetEpgTimeFailure</para> 	
    ''' <para>332 - NoValidParameters</para> 
    ''' <para>334 - VideoNotPresent</para> 
    ''' <para>338 - EventNotExistsFailure</para> 
    ''' <para>349 - ReturnToLiveFailure</para> 
    ''' <para>350 - ParsingFailure</para> 
    ''' </remarks>
    Public Function WaitUntilEventEnds(ByVal EventKeyName As String, Optional ByVal EndGuardTime As String = "") As IEXGateway._IEXResult
        Return Invoke("WaitUntilEventEnds", EventKeyName, EndGuardTime, Me)
    End Function

    ''' <summary>
    '''  Waits Until Reminder Supposed To Be Shown ( Start Time Of The Event Minus 60 Sec )
    ''' </summary>
    ''' <param name="EventKeyName">Key Of The Event</param>
    ''' <returns>IEXGateway._IEXResult</returns>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>301 - DictionaryFailure</para> 
    ''' <para>309 - GetEpgTimeFailure</para> 	
    ''' <para>332 - NoValidParameters</para> 
    ''' <para>334 - VideoNotPresent</para> 
    ''' <para>338 - EventNotExistsFailure</para> 
    ''' <para>349 - ReturnToLiveFailure</para> 
    ''' <para>350 - ParsingFailure</para> 
    ''' <para>354 - ReminderFailure</para> 
    ''' </remarks>
    Public Function WaitUntilReminder(ByVal EventKeyName As String) As IEXGateway._IEXResult
        Return Invoke("WaitUntilReminder", EventKeyName, Me)
    End Function

    ''' <summary>
    '''   Mount The STB Through Serial
    ''' </summary>
    ''' <param name="IsGw">Optional Parameter Default = True : If True Mount Gw Else Mount The Client</param>
    ''' <param name="IsFormat">Optional Parameter Default = True : If True Mount Stb With Format</param>
    ''' <param name="DoReboot">Optional Parameter Default = True : If True Reboot The Stb Else Does Not</param>
    ''' <param name="Retries">Optional Parameter Default = 3 : Number Of Retries In Case Mount Failed</param>
    ''' <returns>IEXGateway._IEXResult</returns>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>324 - MountFailure</para> 	
    ''' <para>329 - IEXSystemError</para> 
    ''' </remarks>
       Public Function MountSerialStb(Optional ByVal IsGw As Boolean = True, Optional ByVal IsFormat As Boolean = True, Optional ByVal DoReboot As Boolean = True, Optional ByVal Retries As Integer = 3, Optional ByVal IsLastDelivery As Boolean = False, Optional ByVal WakeUp As Boolean = False, Optional ByVal IsReturnToLive As Boolean = True) As IEXGateway._IEXResult
        Return Invoke("MountSerialStb", IsGw, IsFormat, DoReboot, Retries, IsLastDelivery,WakeUp, IsReturnToLive, Me)
    End Function

    ''' <summary>
    '''   Mount The STB Through Telnet
    ''' </summary>
    ''' <param name="IsGw">Optional Parameter Default = True : If True Mount Gw Else Mount The Client</param>
    ''' <param name="IsFormat">Optional Parameter Default = True : If True Mount Stb With Format</param>
    ''' <param name="DoReboot">Optional Parameter Default = True : If True Reboot The Stb Else Does Not</param>
    ''' <param name="Retries">Optional Parameter Default = 3 : Number Of Retries In Case Mount Failed</param>
    ''' <returns>IEXGateway._IEXResult</returns>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>324 - MountFailure</para> 	
    ''' <para>329 - IEXSystemError</para> 
    ''' </remarks>
    Public Function MountTelnetStb(Optional ByVal IsGw As Boolean = True, Optional ByVal IsFormat As Boolean = True, Optional ByVal DoReboot As Boolean = True, Optional ByVal IsFactoryReset As Boolean = False, Optional ByVal Retries As Integer = 3, Optional ByVal IsLastDelivery As Boolean = False, Optional ByVal WakeUp As Boolean = False, Optional ByVal IsReturnToLive As Boolean = True) As IEXGateway._IEXResult
        Return Invoke("MountTelnetStb", IsGw, IsFormat, DoReboot, IsFactoryReset, Retries, IsLastDelivery, WakeUp, IsReturnToLive, Me)
    End Function

    ''' <summary>
    '''   Mount The Client
    ''' </summary>
    ''' <param name="MountAs">Optional Parameter Default = EnumMountAs.FORMAT : If True Mount The Gateway With Format</param>
    ''' <param name="Retries">Optional Parameter Default = 3 : Number Of Retries In Case Mount Failed</param>
    ''' <returns>IEXGateway._IEXResult</returns>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>326 - MountClientFailure</para> 
    ''' </remarks>
    Public Function MountClient(Optional ByVal MountAs As EnumMountAs = EnumMountAs.FORMAT, Optional ByVal Retries As Integer = 3, Optional ByVal IsLastDelivery As Boolean = False, Optional ByVal WakeUp As Boolean = False, Optional ByVal IsReturnToLive As Boolean = True) As IEXGateway._IEXResult
        Return Invoke("MountClient", MountAs, Retries, IsLastDelivery, WakeUp, IsReturnToLive, Me)
    End Function

    ''' <summary>
    '''   Mount The Gateway
    ''' </summary>
    ''' <param name="MountAs">Optional Parameter Default = EnumMountAs.FORMAT : If True Mount The Gateway With Format</param>
    ''' <param name="Retries">Optional Parameter Default = 3 : Number Of Retries In Case Mount Failed</param>
    ''' <returns>IEXGateway._IEXResult</returns>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>325 - MountGwFailure</para> 
    ''' </remarks>
    Public Function MountGw(Optional ByVal MountAs As EnumMountAs = EnumMountAs.FORMAT, Optional ByVal Retries As Integer = 3, Optional ByVal IsLastDelivery As Boolean = False, Optional ByVal WakeUp As Boolean = False, Optional ByVal IsReturnToLive As Boolean = True) As IEXGateway._IEXResult
        Return Invoke("MountGw", MountAs, Retries, IsLastDelivery, WakeUp, IsReturnToLive, Me)
    End Function


    ''' <summary>
    '''   Verifies the OTA downaload on the box
    ''' </summary>
    ''' <param name="VersionID"> Binary Vesion on the BOX </param>
    ''' <returns>IEXGateway._IEXResult</returns>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>301 - DictionaryFailure</para> 
    ''' <para>302 - EmptyEpgInfoFailure</para> 
    ''' <para>304 - IRVerificationFailure</para> 
    ''' <para>314 - SetSettingsFailure</para>
    ''' <para>370 - FactoryResetFailure</para> 
    ''' </remarks>
    Public Function OtaDownload(ByVal VersionID As String, ByVal UsageID As String, ByVal NITTable As String, Optional ByVal IsLastDelivery As Boolean = False, Optional ByVal RFFeed As String = "NL",Optional ByVal IsLive As Boolean = False) As IEXGateway._IEXResult
        Return Invoke("OtaDownload", VersionID, UsageID, NITTable, IsLastDelivery, RFFeed,IsLive,Me)
    End Function

    '''' <summary>
    '''' <param> Doing Check for Updates and download new version</param>
    '''' </summary>
    '''' <param name="dwnload option"> Forced/Automatic</param>
    '''' <returns>IEXGateway._IEXResult</returns>
    '''' <remarks>
    '''' Possible Error Codes:
    '''' <para>301 - DictionaryFailure</para> 
    '''' <para>302 - EmptyEpgInfoFailure</para> 
    ''''<para>304 - IRVerificationFailure</para> 
    '''' <para>314 - SetSettingsFailure</para>
    '''' <para>370 - FactoryResetFailure</para> 

    ''' </remarks>
    Public Function OtaDownloadOption(ByVal dwnloadOption As EnumOTADownloadOption, Optional ByVal IsDownload As Boolean = True) As IEXGateway._IEXResult
        Return Invoke("OtaDownloadOption", dwnloadOption, IsDownload, Me)
    End Function
    ''' <summary>
    '''   Check and verify the Epg version after and before the OTA download
    ''' </summary>
    ''' <param name="OldSoftVersion"> EPG version before OTA download on the BOX </param>
    ''' <returns>IEXGateway._IEXResult</returns>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>301 - DictionaryFailure</para> 
    ''' <para>302 - EmptyEpgInfoFailure</para> 
    ''' <para>304 - IRVerificationFailure</para> 
    ''' <para>314 - SetSettingsFailure</para>
    ''' <para>370 - FactoryResetFailure</para> 
    ''' </remarks>
    Public Function GetAndVerifySoftVersion(ByRef SoftVersion As String, ByRef UsageID As String, Optional ByVal IsVerify As Boolean = False, Optional ByVal OldSoftVersion As String = "") As IEXGateway._IEXResult
        Dim res As IEXGateway.IEXResult
        res = Invoke("GetAndVerifySoftVersion", IsVerify, OldSoftVersion, Me)
        Try

            SoftVersion = DirectCast(ReturnValues(0), String)
            UsageID = DirectCast(ReturnValues(1), String)

        Catch ex As Exception

        End Try
        Return res
    End Function
	
    '''' <summary>
    '''' <param> Verifies the AMS tags</param>
    '''' </summary>
    '''' <param name="AMSEvent"> AMS Tags</param>
    '''' <param name="service"> Service ID of the service</param>
    '''' <param name="IsRBPlayback"> Whether RB playback or Recording Playback</param>
    '''' <param name="Speed"> What speed do we need to verify</param>
    '''' <returns>IEXGateway._IEXResult</returns>
    '''' <remarks>
    '''' Possible Error Codes:
    '''' <para>301 - DictionaryFailure</para> 
    '''' <para>302 - EmptyEpgInfoFailure</para> 
    ''''<para>304 - IRVerificationFailure</para> 
    '''' <para>314 - SetSettingsFailure</para>
    '''' <para>370 - FactoryResetFailure</para> 

    ''' </remarks>
	
    Public Function VerifyAMSTags(ByVal AMSEvent As EnumAMSEvent, Optional ByVal service As Service = Nothing, Optional ByVal IsRBPlayback As String = "", Optional ByVal Speed As Double = 0, Optional ByVal commonVariable As String = "") As IEXGateway._IEXResult
        Return Invoke("VerifyAMSTags", AMSEvent, service, IsRBPlayback, Speed, commonVariable, Me)
    End Function
	
    ''' <summary>
    ''' Calculate the current RB depth in mins
    ''' </summary>
    ''' <param name="timeStampMarginLine">RB depth milestone to be parsed</param>
    ''' <param name="rbDepthInMin">The Returned RBdepth in mins </param>
    ''' <returns>IEXGateway._IEXResult</returns>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>350 - ParsingFailure</para> 
    ''' </remarks>
    Public Function GetRBDepthInSec(ByVal timeStampMarginLine As String, ByRef rbDepthInMin As Double) As IEXGateway._IEXResult
        Dim res As IEXGateway.IEXResult

        res = Invoke("GetRBDepthInSec", timeStampMarginLine, Me)
        Try
            rbDepthInMin = DirectCast(ReturnValues(0), Double)
        Catch ex As Exception

        End Try

        Return res
    End Function
    ''' <summary>
    ''' Stanby wakeup the STB with specified duration
    ''' </summary>
    ''' <param name="waitInStby">Time to wait in stby in secs</param>
    ''' <returns>IEXGateway._IEXResult</returns>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>328 - INIFailure</para> 
    ''' <para>358 - StandByFailure</para>
    ''' </remarks>
    Public Function FlushRB(Optional ByVal waitInStby As Double = 10) As IEXGateway._IEXResult
        Dim res As IEXGateway.IEXResult
        res = Invoke("FlushRB", waitInStby, Me)
        Return res
    End Function


#Region "REMINDERS"

    ''' <summary>
    '''  Adding Reminder From Action Bar
    ''' </summary>
    ''' <param name="EventKeyName">Key Of The Event</param>
    ''' <param name="MinTimeBeforeEvStart">Optional Parameter Default = 2 : Minimum Time Left For The Event To Start ( EXAMPLE : For Guard Time )</param>
    ''' <param name="VerifyBookingInPCAT">Optional Parameter Default = True : If True Verifies Event Booked In PCAT</param>
    ''' <returns>IEXGateway._IEXResult</returns>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>305 - PCATFailure</para> 
    ''' <para>311 - SetEventReminderFailure</para> 
    ''' <para>328 - INIFailure</para> 		  
    ''' <para>332 - NoValidParameters</para> 	
    ''' <para>334 - VideoNotPresent</para> 
    ''' <para>337 - ParseEventTimeFailure</para> 
    ''' <para>347 - SelectEventFailure</para> 
    ''' <para>349 - ReturnToLiveFailure</para> 
    ''' <para>350 - ParsingFailure</para> 
    ''' </remarks>
    Public Function BookReminderFromBanner(ByVal EventKeyName As String, Optional ByVal MinTimeBeforeEvStart As Integer = 2, Optional ByVal VerifyBookingInPCAT As Boolean = True) As IEXGateway._IEXResult
        Return Invoke("BookReminderFromBanner", EventKeyName, MinTimeBeforeEvStart, VerifyBookingInPCAT, Me)
    End Function

    ''' <summary>
    '''   Adding Reminder From Guide
    ''' </summary>
    ''' <param name="EventKeyName">Key Of The Event</param>
    ''' <param name="ChannelNumber">Channel Of The Event To Be Added As Reminder</param>
    ''' <param name="NumberOfPresses">Optional Parameter Default = 1 : Number Of Right Presses From Current Event</param>
    ''' <param name="MinTimeBeforeEvStart">Optional Parameter Default = 2 : Minimum Time Left For The Event To Start ( EXAMPLE : For Guard Time )</param>
    ''' <param name="VerifyBookingInPCAT">Optional Parameter Default = True : If True Verifies Event Booked In PCAT</param>
    ''' <param name="ReturnToLive">Optional Parameter Default = True : If True Returns To Live Viewing</param>
    ''' <returns>IEXGateway._IEXResult</returns>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>305 - PCATFailure</para> 
    ''' <para>306 - GetEventInfoFailure</para> 
    ''' <para>323 - VerifyStateFailure</para>    
    ''' <para>328 - INIFailure</para> 
    ''' <para>332 - NoValidParameters</para> 	
    ''' <para>334 - VideoNotPresent</para> 
    ''' <para>339 - RecordEventFailure</para> 
    ''' <para>344 - ConflictFailure</para> 
    ''' <para>347 - SelectEventFailure</para> 
    ''' <para>349 - ReturnToLiveFailure</para> 
    ''' <para>350 - ParsingFailure</para> 		
    ''' <para>355 - TuneToChannelFailure</para> 
    ''' </remarks>
    Public Function BookReminderFromGuide(ByVal EventKeyName As String, ByVal ChannelNumber As String, Optional ByVal NumberOfPresses As Integer = 1, _
                                          Optional ByVal MinTimeBeforeEvStart As Integer = 2, Optional ByVal VerifyBookingInPCAT As Boolean = True, _
                                          Optional ByVal ReturnToLive As Boolean = True) As IEXGateway._IEXResult

        Return Invoke("BookReminderFromGuide", EventKeyName, ChannelNumber, NumberOfPresses, MinTimeBeforeEvStart, VerifyBookingInPCAT, ReturnToLive, Me)

    End Function

    ''' <summary>
    '''   Handles Reminder
    ''' </summary>
    ''' <param name="EventKeyName">The Key Of The Event</param>
    ''' <param name="Action">Can Be : Accept,Reject,Ignore Or Wait</param>
    ''' <returns>IEXGateway._IEXResult</returns>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>302 - EmptyEpgInfoFailure</para> 
    ''' <para>304 - IRVerificationFailure</para> 
    ''' <para>308 - GetChannelFailure</para> 
    ''' <para>322 - VerificationFailure</para> 
    ''' <para>332 - NoValidParameters</para> 	
    ''' <para>338 - EventNotExistsFailure</para> 
    ''' </remarks>
    Public Function HandleReminder(ByVal EventKeyName As String, ByVal Action As EnumReminderActions) As IEXGateway._IEXResult
        Return Invoke("HandleReminder", EventKeyName, Action, Me)
    End Function

    ''' <summary>
    '''    Cancelling Reminder From Guide
    ''' </summary>
    ''' <param name="EventKeyName">Key Of The Event</param>
    ''' <param name="VerifyCanceledInPCAT">Optional Parameter Default = True : If True Verifies Event Canceled In PCAT</param>
    ''' <param name="ReturnToLive">Optional Parameter Default = True : If True Returns To Live Viewing</param>
    ''' <returns>IEXGateway._IEXResult</returns>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>300 - NavigationFailure</para> 
    ''' <para>301 - DictionaryFailure</para> 
    ''' <para>302 - EmptyEpgInfoFailure</para> 
    ''' <para>304 - IRVerificationFailure</para> 
    ''' <para>305 - PCATFailure</para> 
    ''' <para>306 - GetEventInfoFailure</para>  
    ''' <para>322 - VerificationFailure</para> 
    ''' <para>328 - INIFailure</para> 
    ''' <para>330 - TelnetFailure</para> 
    ''' <para>331 - CopyFileFailure</para> 
    ''' <para>332 - NoValidParameters</para> 	
    ''' <para>334 - VideoNotPresent</para> 
    ''' <para>338 - EventNotExistsFailure</para>  
    ''' <para>346 - FindEventFailure</para> 
    ''' <para>347 - SelectEventFailure</para> 
    ''' <para>349 - ReturnToLiveFailure</para> 
    ''' <para>355 - TuneToChannelFailure</para> 
    ''' <para>368 - CancelReminderFailure</para>
    ''' </remarks>
    Public Function CancelReminderFromGuide(ByVal EventKeyName As String, Optional ByVal VerifyCanceledInPCAT As Boolean = True, Optional ByVal ReturnToLive As Boolean = True) As IEXGateway._IEXResult
        Return Invoke("CancelReminderFromGuide", EventKeyName, VerifyCanceledInPCAT, ReturnToLive, Me)
    End Function

#End Region

    ''' <summary>
    '''   Copies TelnetLog.txt From BuildWinPath To Log Folder
    ''' </summary>
    ''' <param name="FileName">Optional Parameter Default = "Diag_TelnetLog.txt" : Filename Of The Log File</param>
    ''' <returns>IEXGateway._IEXResult</returns>
    ''' <remarks></remarks>
    Public Function CopyLogTelnet(Optional ByVal FileName As String = "Diag_TelnetLog.txt") As IEXGateway._IEXResult
        Return Invoke("CopyLogTelnet", FileName, Me)
    End Function

    ''' <summary>
    '''   Copies TelnetLog.txt From BuildWinPath To Log Folder
    ''' </summary>
    ''' <param name="FileName">Optional Parameter Default = "Diag_TelnetLog.txt" : Filename Of The Log File</param>
    ''' <returns>IEXGateway._IEXResult</returns>
    ''' <remarks></remarks>
    Public Function CopyLogSerial(Optional ByVal FileName As String = "Diag_TelnetLog.txt") As IEXGateway._IEXResult
        Return Invoke("CopyLogSerial", FileName, Me)
    End Function
    ''' <summary>
    ''' Copies Diag Binary Logs from STB to Host
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>

    Public Function CopyDiagBinaryLog(ByVal FileName As String) As IEXGateway._IEXResult
        Return Invoke("CopyDiagBinaryLog", FileName, Me)
    End Function


    ''' <summary>
    '''   Delete TelnetLog.txt From BuildWinPath
    ''' </summary>
    ''' <returns>IEXGateway._IEXResult</returns>
    ''' <remarks></remarks>
    Public Function DeleteTelnetLog() As IEXGateway._IEXResult
        Return Invoke("DeleteTelnetLog", Me)
    End Function

    ''' <summary>
    '''  Checkes If STB Is Stuck Or Crashed 
    ''' </summary>
    ''' <returns>IEXGateway._IEXResult</returns>
    ''' <remarks></remarks>
    Public Function StillAlive() As IEXGateway._IEXResult
        Return Invoke("StillAlive", Me)
    End Function
 ''' <summary>
    '''  DaySkipin Guide 
    ''' </summary>
    ''' <returns>IEXGateway._IEXResult</returns>
    ''' <remarks></remarks>
    Public Function DaySkipInGuide(ByVal guideView As EnumGuideViews, ByVal isForward As Boolean, ByVal numberOfPresses As Integer, ByVal isGridInCurrentDate As Boolean, ByVal isDisplayIconVerify As Boolean) As IEXGateway._IEXResult
        Return Invoke("DaySkipInGuide", guideView, isForward, numberOfPresses, isGridInCurrentDate, isDisplayIconVerify, Me)
    End Function

    ''' <summary>
    '''   Get Value From Channels.ini 
    ''' </summary>
    ''' <param name="Key">Key Of Value : HD,SD...</param>
    ''' <returns>String With The Channel</returns>
    ''' <remarks></remarks>
    Public Function GetValue(ByVal Key As String) As String
        Try
            Dim _iexNumber As String = _iex.IEXServerNumber
            Dim iniFile As AMS.Profile.Ini
            iniFile = New AMS.Profile.Ini("C:\Program Files\IEX\Tests\TestsINI\IEX" + _iexNumber + "\Channels.ini")
            Dim value As String = iniFile.GetValue("CHANNELS", Key).ToString

            If value = "" Then
                UI.Utils.LogCommentFail("Failed To Get Value For Key : " + Key)
            Else
                UI.Utils.LogCommentInfo("Retrieved Value From ini File: " + Key + " = " + value)
            End If

            Return value
        Catch ex As Exception
            UI.Utils.LogCommentFail("Key : " + Key.ToString + " Doesn't Exists On Channels.ini")
            Return ""
        End Try
    End Function

    ''' <summary>
    '''   Get Value From The Test INI file
    ''' </summary>
    ''' <param name="Key">Key Of Value</param>
    ''' <returns>String With The Value</returns>
    ''' <remarks></remarks>
    Public Function GetTestParams(ByVal Key As String) As String
        Try
            Dim _iexNumber As String = _iex.IEXServerNumber
            Dim iniFile As AMS.Profile.Ini
            iniFile = New AMS.Profile.Ini("C:\Program Files\IEX\Tests\TestsINI\IEX" + _iexNumber + "\Test.ini")
            Dim value As String = iniFile.GetValue("TEST PARAMS", Key).ToString

            If Key = "SERVICE_TYPE" AndAlso value <> "" Then
                value = GetValue(value)
            End If

            If value = "" Then
                UI.Utils.LogCommentFail("Failed To Get Value For Key : " + Key)
            Else
                UI.Utils.LogCommentInfo("Retrieved Value From ini File: " + Key + " = " + value)
            End If

            Return value
        Catch ex As Exception
            UI.Utils.LogCommentFail("GetValue : Failed To Get Value From Test.ini With Exception : " + ex.Message)
            Return ""
        End Try
    End Function

    ''' <summary>
    '''   Get Value From The INI file
    ''' </summary>
    ''' <param name="INIFile">INI File Name</param>
    ''' <param name="Section">Section Of Value</param>
    ''' <param name="Key">Key Of Value</param>
    ''' <returns>String With The Value</returns>
    ''' <remarks></remarks>
    Public Function GetValueFromINI(ByVal INIFile As EnumINIFile, ByVal Section As String, ByVal Key As String) As String
        Try
            Dim _iexNumber As String = _iex.IEXServerNumber
            Dim _iniFile As AMS.Profile.Ini
            _iniFile = New AMS.Profile.Ini("C:\Program Files\IEX\Tests\TestsINI\IEX" + _iexNumber + "\" + INIFile.ToString + ".ini")

            If INIFile = EnumINIFile.Environment Then
                Section += _iexNumber.ToString
            End If

            Dim value As String = _iniFile.GetValue(Section, Key).ToString

            If value = "" Then
                UI.Utils.LogCommentFail("Failed To Get Value For Key : " + Key)
            Else
                UI.Utils.LogCommentInfo("Retrieved Value From ini File: " + Key + " = " + value)
            End If

            Return value
        Catch ex As Exception
            UI.Utils.LogCommentFail("GetValue : Failed To Get Value From ini File With Exception : " + ex.Message)
            Return ""
        End Try
    End Function

#Region "Content XML APIs"

    ''' <summary>
    ''' Get service from Stream configuration XML file based on the passed criterion
    ''' </summary>
    ''' <param name="posCrit">Positive criterion passed as string which will be matched with the required service</param>
    ''' <param name="negCrit">Negative criterion passed as string which will be matched with the required service</param>
    ''' <returns>The first matching service object based on the criteria specified,or nothing if the criteria does not match any service</returns>
    ''' <remarks>
    ''' Criteria are to be passed in the following format - "key1=value1;key2=value2" Eg- "eventDuration=5;parentalRating=high"
    ''' Multiple criteria can also be specified in the following format - "key=value1,value2" Eg- "LCN=101,102"
    ''' Also the exceptions thrown will be caught in the script. Developer should ensure handling of no match scenario since nothing will be returned
    ''' </remarks>
    Public Overloads Function GetServiceFromContentXML(ByVal posCrit As String, Optional ByVal negCrit As String = "") As Service

        UI.Utils.StartHideFailures("Entering method - " + MethodBase.GetCurrentMethod.Name())
        Dim service As Service = Nothing

        'Call Get Content EA passing Service as the required type
        Dim serviceList As List(Of Service) = GetContent(Of Service)(posCrit, negCrit)

        If Not IsNothing(serviceList) Then
            If serviceList.Count <> 0 Then
                service = serviceList.Item(0)
            End If
        End If
        _iex.ForceHideFailure()

        'Return the service
        Return service

    End Function
    ''' Return List of Services from Content.xml
    ''' </summary>
    ''' <param name="posCrit"></param>Positive criterion passed as string which will be matched with the required service</param>
    ''' <param name="negCrit">Negative criterion passed as string which will be matched with the required service</param>
    ''' <returns></returns>Return complete list of services fetched from content.xml
    ''' <remarks></remarks>
    Public Function GetServiceListFromContentXML(ByVal posCrit As String, Optional ByVal negCrit As String = "") As List(Of Service)
        UI.Utils.StartHideFailures("Entering method - " + MethodBase.GetCurrentMethod.Name())

        'Call Get Content EA passing Service as the required type
        Dim serviceList As List(Of Service) = GetContent(Of Service)(posCrit, negCrit)

        Return serviceList
    End Function

    ''' <summary>
    ''' Get service from Stream configuration XML file based on the passed criterion
    ''' </summary>
    ''' <param name="posCrit">Positive criterion passed as service object which will be matched with the required service</param>
    ''' <param name="negCrit">Negative criterion passed as service object which will be matched with the required service</param>
    ''' <returns>The first matching service object based on the criteria specified,or nothing if the criteria does not match any service</returns>
    ''' <remarks>
    ''' Criteria are to be passed as an object created with attributes specified - service.eventDuration = 10
    ''' Also the exceptions thrown will be caught in the script. Developer should ensure handling of no match scenario since nothing will be returned
    ''' </remarks>
    Public Overloads Function GetServiceFromContentXML(ByVal posCrit As Service, Optional ByVal negCrit As Service = Nothing) As Service

        UI.Utils.StartHideFailures("Entering method - " + MethodBase.GetCurrentMethod.Name())
        Dim service As Service = Nothing

        'Call Get Content EA passing Service as the required type
        Dim serviceList As List(Of Service) = GetContent(Of Service)(posCrit, negCrit)

        If Not IsNothing(serviceList) Then
            If serviceList.Count <> 0 Then
                service = serviceList.Item(0)
            End If
        End If
        _iex.ForceHideFailure()

        'Return the service
        Return service

    End Function

    ''' <summary>
    ''' Get VOD asset from Stream configuration XML file based on the passed criterion
    ''' </summary>
    ''' <param name="posCrit">Positive criterion passed as string which will be matched with the required VOD asset</param>
    ''' <param name="negCrit">Negative criterion passed as string which will be matched with the required VOD asset</param>
    ''' <returns>The first matching service object based on the criteria specified,or nothing if the criteria does not match any VOD asset</returns>
    ''' <remarks>
    ''' Criteria are to be passed in the following format - "key1=value1;key2=value2" Eg- "Trailer=True;parentalRating=high"
    ''' Multiple criteria can also be specified in the following format - "key=value1,value2" Eg- "Type=TVOD"
    ''' Also the exceptions thrown will be caught in the script. Developer should ensure handling of no match scenario since nothing will be returned
    ''' </remarks>
    Public Overloads Function GetVODAssetFromContentXML(ByVal posCrit As String, Optional ByVal negCrit As String = "") As VODAsset

        UI.Utils.StartHideFailures("Entering method - " + MethodBase.GetCurrentMethod.Name())
        Dim vodAsset As VODAsset = Nothing

        'Call Get Content EA passing VODAsset as the required type
        Dim vodAssetList As List(Of VODAsset) = GetContent(Of VODAsset)(posCrit, negCrit)

        If Not IsNothing(vodAssetList) Then
            If vodAssetList.Count <> 0 Then
                vodAsset = vodAssetList.Item(0)
            End If
        End If
        _iex.ForceHideFailure()

        'Replace special characters (TBC with other characters) 
        vodAsset.Synopsis = vodAsset.Synopsis.Replace("\r\n", vbCrLf)
        vodAsset.Synopsis = vodAsset.Synopsis.Replace("\r", vbCr)
        vodAsset.Synopsis = vodAsset.Synopsis.Replace("\n", vbLf)

        'Return the vod asset
        Return vodAsset

    End Function

    ''' <summary>
    ''' Get VOD asset from Stream configuration XML file based on the passed criterion
    ''' </summary>
    ''' <param name="posCrit">Positive criterion passed as VOD asset object which will be matched with the required VOD asset</param>
    ''' <param name="negCrit">Negative criterion passed as VOD asset object which will be matched with the required VOD asset</param>
    ''' <returns>The first matching VOD asset object based on the criteria specified,or nothing if the criteria does not match any VOD asset</returns>
    ''' <remarks>
    ''' Criteria are to be passed as an object created with attributes specified - vodAsset.Trailer = True
    ''' Also the exceptions thrown will be caught in the script. Developer should ensure handling of no match scenario since nothing will be returned
    ''' </remarks>
    Public Overloads Function GetVODAssetFromContentXML(ByVal posCrit As VODAsset, Optional ByVal negCrit As VODAsset = Nothing) As VODAsset

        UI.Utils.StartHideFailures("Entering method - " + MethodBase.GetCurrentMethod.Name())
        Dim vodAsset As VODAsset = Nothing

        'Call Get Content EA passing Service as the required type
        Dim vodAssetList As List(Of VODAsset) = GetContent(Of VODAsset)(posCrit, negCrit)

        If Not IsNothing(vodAssetList) Then
            If vodAssetList.Count <> 0 Then
                vodAsset = vodAssetList.Item(0)
            End If
        End If
        _iex.ForceHideFailure()

        'Replace special characters (TBC with other characters) 
        vodAsset.Synopsis = vodAsset.Synopsis.Replace("\r\n", vbCrLf)
        vodAsset.Synopsis = vodAsset.Synopsis.Replace("\r", vbCr)
        vodAsset.Synopsis = vodAsset.Synopsis.Replace("\n", vbLf)

        'Return the vod asset
        Return vodAsset

    End Function


    ''' <summary>
    ''' Get Media Centre content from Stream configuration XML file based on the passed criterion
    ''' </summary>
    ''' <param name="posCrit">Positive criterion passed as MCContent object which will be matched with the required MCContent</param>
    ''' <param name="negCrit">Negative criterion passed as MCContent object which will be matched with the required MCContent</param>
    ''' <returns>The first matching MCContent object based on the criteria specified,or nothing if the criteria does not match any MCContent</returns>
    ''' <remarks>
    ''' Criteria are to be passed as an object created with attributes specified - content.Type = Pictures
    ''' Also the exceptions thrown will be caught in the script. Developer should ensure handling of no match scenario since nothing will be returned
    ''' </remarks>
    Public Overloads Function GetMediaCentreContentFromContentXML(ByVal posCrit As String, Optional ByVal negCrit As String = Nothing) As MediaContent

        UI.Utils.StartHideFailures("Entering method - " + MethodBase.GetCurrentMethod.Name())
        Dim content As MediaContent = Nothing

        'Call Get Content EA passing Service as the required type
        Dim contentList As List(Of MediaContent) = GetContent(Of MediaContent)(posCrit, negCrit)

        If Not IsNothing(contentList) Then
            If contentList.Count <> 0 Then
                content = contentList.Item(0)
            End If
        End If
        _iex.ForceHideFailure()

        'Return the service
        Return content

    End Function

    ''' <summary>
    '''   Get Content from Stream configuration XML file based on the passed criterion 
    ''' </summary>
    ''' <typeparam name="T"> The type of the content object to be searched</typeparam>
    ''' <param name="posCrit">Positive criterion as string for matching of required content</param>
    ''' <param name="negCrit">Negative criterion as string for matching of required content</param>
    ''' <returns>The matching Content object</returns>
    ''' <remarks>
    ''' Criteria are to be passed in the following format - "key1=value1;key2=value2" Eg- "eventDuration=5;parentalRating=high"
    ''' Also the exceptions thrown will be caught in the script. Developer should ensure handling of no match scenario since nothing will be returned
    ''' </remarks>
    Public Overloads Function GetContent(Of T)(ByVal posCrit As String, Optional ByVal negCrit As String = "") As List(Of T)

        UI.Utils.StartHideFailures("Entering method - " + MethodBase.GetCurrentMethod.Name())

        'Set XML file Path
        Dim xmlFilePath As String = GetContentXMLFilePath()
        'Return content object
        Dim matchedContent As List(Of T) = Nothing

        'Check if both criteria are empty
        If String.IsNullOrEmpty(posCrit) And String.IsNullOrEmpty(negCrit) Then
            ExceptionUtils.ThrowEx(New EAException(FailuresHandler.ExitCodes.NoValidParameters, "Atleast one of the criteria must be specified for matching!"))
        Else
            'Build a positive and negative criterion hashtable from the criterion string passed
            Dim posCritTbl = CreateHashTable(posCrit)
            Dim negCritTbl = CreateHashTable(negCrit)

            'Fetch the matched content element based on the HashTables passed
            matchedContent = FetchObjFromXMLFile(Of T)(xmlFilePath, posCritTbl, negCritTbl)
            If IsNothing(matchedContent) Then
                UI.Utils.LogCommentFail("Obtained Content list is empty!")
            ElseIf matchedContent.Count = 0 Then
                UI.Utils.LogCommentFail("Failed to get content matching the passed criterion!")
                matchedContent = Nothing
            End If
        End If

        _iex.ForceHideFailure()
        'Return the table
        Return matchedContent

    End Function

    ''' <summary>
    '''   Get Content from Stream configuration XML file based on the passed criterion 
    ''' </summary>
    ''' <typeparam name="T"> The type of the content object to be searched</typeparam>
    ''' <param name="posCrit">Positive criterion as string for matching of required content</param>
    ''' <param name="negCrit">Negative criterion as string for matching of required content</param>
    ''' <returns>The matching Content object list</returns>
    ''' <remarks>
    ''' Criteria are to be passed as an object created with attributes specified - content.eventDuration = 10
    ''' Also the exceptions thrown will be caught in the script. Developer should ensure handling of no match scenario since nothing will be returned
    ''' </remarks>
    Public Overloads Function GetContent(Of T)(ByVal posCrit As T, Optional ByVal negCrit As T = Nothing) As List(Of T)

        UI.Utils.StartHideFailures("Entering method - " + MethodBase.GetCurrentMethod.Name())
        'Set XML file Path
        Dim xmlFilePath As String = GetContentXMLFilePath()
        'Return content object
        Dim matchedContent As List(Of T) = Nothing

        'Check if both criteria are empty
        If IsNothing(posCrit) And IsNothing(negCrit) Then
            ExceptionUtils.ThrowEx(New EAException(FailuresHandler.ExitCodes.NoValidParameters, "Atleast one of the criteria must be specified for matching!"))
        Else
            'Build a positive and negative criterion hashtable from the criterion string passed
            Dim posCritTbl = CreateHashTable(posCrit)
            Dim negCritTbl = CreateHashTable(negCrit)
            'Fetch the matched content element based on the HashTables passed
            matchedContent = FetchObjFromXMLFile(Of T)(xmlFilePath, posCritTbl, negCritTbl)
            If IsNothing(matchedContent) Then
                UI.Utils.LogCommentFail("Obtained Content list is empty!")
            ElseIf matchedContent.Count = 0 Then
                UI.Utils.LogCommentFail("Failed to get content matching the passed criterion!")
                matchedContent = Nothing
            End If
        End If

        _iex.ForceHideFailure()
        'Return the table
        Return matchedContent

    End Function

    ''' <summary>
    ''' Get attributes from Content XML
    ''' </summary>
    ''' <param name="tag">The Tag of XML in which attribute is defined</param>
    ''' <param name="attributeKey">The attribute key as String</param>
    ''' <returns>The attribute value</returns>
    ''' <remarks></remarks>
    Public Function GetAttributeFromContentXML(ByVal tag As String, ByVal attributeKey As String) As String
        Dim attributeValue As String = ""
        Try
            Dim xmlFilePath As String = GetContentXMLFilePath()
            Dim xmlFile As XDocument = XDocument.Load(xmlFilePath)
            attributeValue = (From ele In xmlFile.Descendants(tag)
                                           Select ele.Attribute(attributeKey).Value).FirstOrDefault
        Catch ex As Exception
            ExceptionUtils.ThrowEx(New EAException(FailuresHandler.ExitCodes.NoValidParameters, "Invalid string passed for parsing!"))
        End Try
        Return attributeValue
    End Function

    ''' <summary>
    ''' Gets the Content XML File Path
    ''' </summary>
    ''' <returns>The Content XML File Path as String</returns>
    Private Function GetContentXMLFilePath()
        Dim _iexNumber As String = _iex.IEXServerNumber
        Dim xmlFileName As String = GetContentXmlFileName()
        Dim xmlFilePath As String = "C:\Program Files\IEX\Tests\TestsINI\IEX" + _iexNumber + "\" + xmlFileName
        Return xmlFilePath
    End Function

    ''' <summary>
    ''' Get Content XML File name
    ''' </summary>
    ''' <returns>String</returns>
    ''' <remarks></remarks>
    Private Function GetContentXmlFileName() As String

        Dim xmlFileName As String = "Content.xml"

        UI.Utils.StartHideFailures("Getting Content.xml File name")
        'Try to get the XML File name
        Try
            xmlFileName = "Content-" + UI.Utils.GetValueFromEnvironment("StreamLocation") + ".xml"
        Catch
            UI.Utils.LogCommentWarning("Stream Location not defined in environment! Taking default value instead.")
            Return xmlFileName
        Finally
            _iex.ForceHideFailure()
        End Try

        Return xmlFileName

    End Function

    ''' <summary>
    ''' Create a hashtable from the delimited string passed in the following format
    ''' Eg - "key1=value1;key2=value2"
    ''' </summary>
    ''' <param name="dlmtedStr">The string as in the above format</param>
    ''' <returns>Hashtable</returns>
    Private Overloads Function CreateHashTable(ByVal dlmtedStr As String) As Hashtable

        UI.Utils.StartHideFailures("Entering method - " + MethodBase.GetCurrentMethod.Name())
        'Define the table to be returned
        Dim hashtable As New Hashtable
        UI.Utils.LogCommentInfo("Creating hash table from the passed string - " + dlmtedStr)

        'Check whether the string passed is empty
        If Not String.IsNullOrEmpty(dlmtedStr) Then
            Dim spltStrArr As String() = dlmtedStr.Split(";")
            UI.Utils.LogCommentInfo("The string array created after splitting at delimiter ';' is " + spltStrArr.ToString)

            'Fetch the key and value for each pair and add it to the table
            For Each strPair As String In spltStrArr
                Dim spltPair As String() = strPair.Split("=")
                UI.Utils.LogCommentInfo("The string array created after splitting at delimiter '=' is " + spltPair.ToString)
                If (spltPair.Length = 2) Then
                    Try
                        hashtable.Add(spltPair(0), spltPair(1))
                    Catch ex As Exception
                        UI.Utils.LogCommentFail("Failed to add entry into the hashtable with the exception message - " + ex.Message)
                        hashtable = Nothing
                        Exit For
                    End Try
                Else
                    ExceptionUtils.ThrowEx(New EAException(FailuresHandler.ExitCodes.NoValidParameters, "Invalid string passed for parsing!"))
                    hashtable = Nothing
                    Exit For
                End If
            Next
        End If

        _iex.ForceHideFailure()
        'Return the table
        Return hashtable

    End Function

    ''' <summary>
    ''' Create a hashtable from the object passed. The hashtable will be such that the key will be the field name and value will be the field value
    ''' </summary>
    ''' <param name="obj">The object which is to be converted to hashtable</param>
    ''' <returns>Hashtable with key as the field name and value as the field value</returns>
    Private Overloads Function CreateHashTable(ByVal obj As Object) As Hashtable

        UI.Utils.StartHideFailures("Entering method - " + MethodBase.GetCurrentMethod.Name())
        'Define the table to be returned
        Dim hashtable As New Hashtable

        'Check if the object passed is null
        If Not IsNothing(obj) Then
            Try
                UI.Utils.LogCommentInfo("Creating Hashtable from the passsed object.")
                'Get the fields of the object
                Dim objFields = obj.GetType().GetFields()

                'Iterate through all fields to add them to the hashtable
                For Each objField As System.Reflection.FieldInfo In objFields
                    If Not IsNothing(objField.GetValue(obj)) Then
                        hashtable.Add(objField.Name, objField.GetValue(obj))
                    End If
                Next
            Catch ex As Exception
                UI.Utils.LogCommentFail("Creating Hashtable from object failed with the exception - " + ex.Message)
                hashtable = Nothing
            End Try
        Else
            'Object is null
            UI.Utils.LogCommentInfo("Null object is passed.")
            hashtable = Nothing
        End If

        _iex.ForceHideFailure()
        'Return the table
        Return hashtable

    End Function

    ''' <summary>
    ''' Fetch a required object from XML file which matches the specified criterion
    ''' </summary>
    ''' <typeparam name="T">The class to which the object will be casted to</typeparam>
    ''' <param name="xmlFilePath">The path of the XML file</param>
    ''' <param name="posCritTbl">Positive criterion for match built as a Hashtable</param>
    ''' <param name="negCritTbl">Negative criterion for match built as a Hashtable</param>
    ''' <returns>Returns the objects which matches the Criteria</returns>
    Public Function FetchObjFromXMLFile(Of T)(ByVal xmlFilePath As String, ByVal posCritTbl As Hashtable, ByVal negCritTbl As Hashtable) As List(Of T)

        UI.Utils.StartHideFailures("Entering method - " + MethodBase.GetCurrentMethod.Name())
        'The object to be fetched from XML file
        Dim matchedObjList As New List(Of T)
        Try
            UI.Utils.LogCommentInfo("Xmlfile path given - " + xmlFilePath)
            'Load XML Document
            Dim xmlFile = XDocument.Load(xmlFilePath)
            Dim xmlObjMatchList = From obj In xmlFile.Descendants(GetType(T).Name) Select obj

            'Check whether both criteria are empty
            If IsNothing(posCritTbl) And IsNothing(negCritTbl) Then
                ExceptionUtils.ThrowEx(New EAException(FailuresHandler.ExitCodes.NoValidParameters, "Atleast one of the criteria must be passed as hashtable!"))
            Else
                'Run through the positive criterion to get the required elements
                If Not IsNothing(posCritTbl) Then
                    UI.Utils.LogCommentInfo("Running through the positive criteria to get the required elements...")
                    For Each posCritPair As DictionaryEntry In posCritTbl
                        Dim tempPair = posCritPair
                        'Check if multiple values are specified
                        Dim tempPairValueArray = tempPair.Value.ToString.Split(",")
                        For Each tempPairValue In tempPairValueArray
                            Dim value = tempPairValue
                            xmlObjMatchList = From obj In xmlObjMatchList Where (CStr(obj.Element(tempPair.Key.ToString)) = value) Select obj
                        Next
                    Next
                End If

                'Run through the negative criterion to get the required elements
                If Not IsNothing(negCritTbl) Then
                    UI.Utils.LogCommentInfo("Running through the negative criteria to get the required elements...")
                    For Each negCritPair As DictionaryEntry In negCritTbl
                        Dim tempPair = negCritPair
                        'Check if multiple values are specified
                        Dim tempPairValueArray = tempPair.Value.ToString.Split(",")
                        For Each tempPairValue In tempPairValueArray
                            Dim value = tempPairValue
                            xmlObjMatchList = From obj In xmlObjMatchList Where Not (CStr(obj.Element(tempPair.Key.ToString)) = value) Select obj
                        Next
                    Next
                End If

                'Deserialize the element into the required object
                If xmlObjMatchList.Count <> 0 Then
                    UI.Utils.LogCommentInfo("Deserializing the xml element into an object.")
                    Dim xmlSerializer = New System.Xml.Serialization.XmlSerializer(GetType(T))
                    For Each matchedXmlObj As XElement In xmlObjMatchList
                        Dim matchedObj As T
                        Using memoryStream As New IO.MemoryStream(Text.Encoding.ASCII.GetBytes(matchedXmlObj.ToString))
                            matchedObj = xmlSerializer.Deserialize(memoryStream)
                        End Using
                        matchedObjList.Add(matchedObj)
                    Next
                Else
                    UI.Utils.LogCommentInfo("No object match found for the given criteria.")
                End If
            End If
        Catch ex As Exception
            UI.Utils.LogCommentFail("FetchObjFromXMLFile failed with the following exception message - " + ex.Message)
        End Try

        _iex.ForceHideFailure()
        'Return the matched object
        Return matchedObjList

    End Function

#End Region

    ''' <summary>
    '''   Gets Event Info By EnumEventInfo From Events Collection
    ''' </summary>
    ''' <param name="EventKeyName">The Key Of The Event</param>
    ''' <param name="Info">Enum Event Info : The Requested Info</param>
    ''' <returns>String</returns>
    ''' <remarks>
    ''' EnumEventInfo Fields :
    ''' <para>EventName</para> 	
    ''' <para>EventStartTime</para> 
    ''' <para>EventEndTime</para> 
    ''' <para>EventChannel</para> 
    ''' <para>RightPresses</para> 
    ''' <para>EventStatus</para> 
    ''' <para>EventDuration</para> 
    ''' <para>EventDate</para> 
    ''' <para>EventConvertedDate</para> 
    ''' <para>EventPlayedDuration</para> 
    ''' <para>EventSource</para> 
    ''' <para>EventRecurrence</para> 
    ''' </remarks>
    Public Function GetEventInfo(ByVal EventKeyName As String, ByVal Info As EnumEventInfo) As String
        Dim EventName As String = ""

        If EventKeyName = "" Then
            ExceptionUtils.ThrowEx(New EAException(FailuresHandler.ExitCodes.NoValidParameters, "Param EventKeyName Is Empty Please Check"))
        Else
            Try
                EventName = UI.Events(EventKeyName).Name
            Catch ex As Exception
                ExceptionUtils.ThrowEx(New EAException(FailuresHandler.ExitCodes.EventNotExistsFailure, "Event With Key Name - " + EventKeyName + " Does Not Exists On Collection"))
            End Try
        End If

        Select Case Info
            Case EnumEventInfo.EventName
                Return UI.Events(EventKeyName).Name.ToString
            Case EnumEventInfo.EventStartTime
                Return UI.Events(EventKeyName).StartTime.ToString
            Case EnumEventInfo.EventEndTime
                Return UI.Events(EventKeyName).EndTime.ToString
            Case EnumEventInfo.EventChannel
                Return UI.Events(EventKeyName).Channel.ToString
            Case EnumEventInfo.EventDuration
                Return UI.Events(EventKeyName).Duration.ToString
            Case EnumEventInfo.EventOriginalDuration
                Return UI.Events(EventKeyName).OriginalDuration
            Case EnumEventInfo.EventDate
                Return UI.Events(EventKeyName).EventDate.ToString
            Case EnumEventInfo.EventConvertedDate
                Return UI.Events(EventKeyName).ConvertedDate.ToString
            Case EnumEventInfo.EventPlayedDuration
                Return UI.Events(EventKeyName).PlayedDuration.ToString
            Case EnumEventInfo.EventSource
                Return UI.Events(EventKeyName).Source.ToString
            Case EnumEventInfo.EventRecurrence
                Return UI.Events(EventKeyName).Occurrences.ToString
            Case Else
                ExceptionUtils.ThrowEx(New EAException(FailuresHandler.ExitCodes.NoValidParameters, "Param Info Is Not Valid Please Check"))
        End Select

        Return ""
    End Function

    ''' <summary>
    ''' Fetch channel logo and parse it to get channel logo name
    ''' </summary>
    ''' <returns>The parsed channel logo name</returns>
    ''' <remarks>
    ''' The channel logo log will be in the format /mnt/nds/../../{logoname}.{extn}
    ''' We will fetch the logoname and return it to be used in the script
    ''' </remarks>
    Public Function GetChannelLogo() As String

        UI.Utils.LogCommentInfo("Entering - " + MethodBase.GetCurrentMethod.Name())
        Dim channelLogoStr As String = ""
        Dim parsedChannelLogo As String = ""
        Dim pathDelimiter As Char = "/"
        Dim extnDelimiter As Char = "."

        'Fetch channel logo from EPG Info
        Try
            UI.Utils.GetEpgInfo("channel_logo", channelLogoStr)
        Catch ex As EAException
            UI.Utils.LogCommentInfo("Channel logo is not present")
        End Try

        'Split the channel logo path
        Dim parsedChannelLogoArr As String() = channelLogoStr.Split(pathDelimiter)
        'Take the last element which has the channel logo with extension
        Dim parsedChannelLogoWithExt As String = parsedChannelLogoArr(UBound(parsedChannelLogoArr))
        'Remove the extension
        Dim parsedChannelLogoWithExtArr As String() = parsedChannelLogoWithExt.Split(extnDelimiter)
        If (parsedChannelLogoWithExtArr.Count <> 2) Then
            UI.Utils.LogCommentFail("Channel logo not present!")
        Else
            parsedChannelLogo = parsedChannelLogoWithExtArr(0)
            UI.Utils.LogCommentInfo("The Channel Logo : " + parsedChannelLogo)
        End If

        UI.Utils.LogCommentInfo("Exiting - " + MethodBase.GetCurrentMethod.Name())
        Return parsedChannelLogo

    End Function

#Region "PPV"

    ''' <summary>
    '''   Renting Current PPV Event From Guide
    ''' </summary>
    ''' <param name="ChannelNumber">Channel Of The PPV Event To Be Rented</param>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>302 - EmptyEpgInfoFailure</para> 
    ''' <para>304 - IRVerificationFailure</para>
    ''' <para>328 - INIFailure</para> 
    ''' <para>355 - TuneToChannelFailure</para>
    ''' <para>347 - SelectEventFailure</para>
    ''' <para>369 - RentPPVEventFailure</para>
    ''' </remarks>
    Public Function RentCurrentPPVEventFromGuide(ByVal ChannelNumber As String) As IEXGateway._IEXResult
        Return Invoke("RentCurrentPPVEventFromGuide", ChannelNumber, Me)
    End Function


    ''' <summary>
    '''   Renting Future PPV Event From Guide
    ''' </summary>
    ''' <param name="ChannelNumber">Channel Of The Event To Be Rented</param>
    ''' <param name="NumberOfPresses">Optional Parameter Default = 1 : Number Of Right Presses From Current Event</param>
    ''' <returns>IEXGateway._IEXResult</returns>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>300 - NavigationFailure</para>
    ''' <para>302 - EmptyEpgInfoFailure</para>
    ''' <para>304 - IRVerificationFailure</para>
    ''' <para>328 - INIFailure</para> 
    ''' <para>347 - SelectEventFailure</para>
    ''' <para>355 - TuneToChannelFailure</para> 
    ''' <para>369 - RentPPVEventFailure</para>
    ''' </remarks>
    Public Function RentFuturePPVEventFromGuide(ByVal ChannelNumber As String, Optional ByVal NumberOfPresses As Integer = 1) As IEXGateway._IEXResult
        Return Invoke("RentFuturePPVEventFromGuide", ChannelNumber, NumberOfPresses, Me)
    End Function


    ''' <summary>
    '''   Renting Current PPV Event From Banner
    ''' </summary>
    ''' <returns>IEXGateway._IEXResult</returns>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>304 - IRVerificationFailure</para>
    ''' <para>328 - INIFailure</para>
    ''' <para>347 - SelectEventFailure</para>
    ''' <para>369 - RentPPVEventFailure</para>
    ''' </remarks>
    Public Function RentCurrentPPVEventFromBanner() As IEXGateway._IEXResult
        Return Invoke("RentCurrentPPVEventFromBanner", Me)
    End Function

    'TODO update doc
    ''' <summary>
    '''   Renting Future PPV Event From Banner
    ''' </summary>
    ''' <returns>IEXGateway._IEXResult</returns>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>304 - IRVerificationFailure</para>
    ''' <para>328 - INIFailure</para>
    ''' <para>347 - SelectEventFailure</para>
    ''' <para>369 - RentPPVEventFailure</para>  
    ''' </remarks>
    Public Function RentFuturePPVEventFromBanner() As IEXGateway._IEXResult
        Return Invoke("RentFuturePPVEventFromBanner", Me)
    End Function

    ''' <summary>
    '''   Modifications in Action Bar
    ''' </summary>
    ''' <returns>IEXGateway._IEXResult</returns>
    ''' <remarks></remarks>
    Public Function NavigateAndHighlight(ByVal NavigationPath As String, Optional ByVal dictionary As Dictionary(Of [EnumEpgKeys], String) = Nothing) As IEXGateway._IEXResult
        Return Invoke("NavigateAndHighlight", NavigationPath, dictionary, Me)
    End Function

    ''' <summary>
    '''   Get audio type (from Service/VODAsset object)
    ''' </summary>
    ''' <returns>IEXGateway._IEXResult</returns>
    ''' <remarks></remarks>
    Public Function GetAudioType(ByVal stream As Object, ByVal audioLanguageIndex As Integer, ByRef audioType As EnumAudioType) As IEXGateway._IEXResult
        Dim res As IEXGateway.IEXResult

        res = Invoke("GetAudioType", stream, audioLanguageIndex, Me)
        Try
            audioType = DirectCast(ReturnValues(0), EnumAudioType)
        Catch ex As Exception
        End Try

        Return res
    End Function
    ''' <summary>
    '''   Get Subtitle type (from Service/VODAsset object)
    ''' </summary>
    ''' <returns>IEXGateway._IEXResult</returns>
    ''' <remarks></remarks>
    Public Function GetSubtitleType(ByVal stream As Object, ByVal subtitleLanguageIndex As Integer, ByRef subtitleType As EnumSubtitleType) As IEXGateway._IEXResult
        Dim res As IEXGateway.IEXResult

        res = Invoke("GetSubtitleType", stream, subtitleLanguageIndex, Me)
        Try
            subtitleType = DirectCast(ReturnValues(0), EnumSubtitleType)
        Catch ex As Exception
        End Try

        Return res
    End Function

    ''' <summary>
    '''   Get audio language (from Service/VODAsset object)
    ''' </summary>
    ''' <returns>IEXGateway._IEXResult</returns>
    ''' <remarks></remarks>
    Public Function GetAudioLanguage(ByVal stream As Object, ByVal audioLanguageIndex As Integer, ByRef audioLanguage As EnumLanguage) As IEXGateway._IEXResult
        Dim res As IEXGateway.IEXResult

        res = Invoke("GetAudioLanguage", stream, audioLanguageIndex, Me)
        Try
            audioLanguage = DirectCast(ReturnValues(0), EnumLanguage)
        Catch ex As Exception
        End Try

        Return res
    End Function

    ''' <summary>
    '''   Get subtitle language (from Service/VODAsset object)
    ''' </summary>
    ''' <returns>IEXGateway._IEXResult</returns>
    ''' <remarks></remarks>
    Public Function GetSubtitleLanguage(ByVal stream As Object, ByVal subtitleLanguageIndex As Integer, ByRef subtitleLanguage As EnumLanguage) As IEXGateway._IEXResult
        Dim res As IEXGateway.IEXResult

        res = Invoke("GetSubtitleLanguage", stream, subtitleLanguageIndex, Me)
        Try
            subtitleLanguage = DirectCast(ReturnValues(0), EnumLanguage)
        Catch ex As Exception
        End Try

        Return res
    End Function

    ''' <summary>
    '''   Get current audio language (from channel bar)
    ''' </summary>
    ''' <returns>IEXGateway._IEXResult</returns>
    ''' <remarks></remarks>
    Public Function GetCurrentAudioLanguage(ByRef audioLanguage As EnumLanguage) As IEXGateway._IEXResult
        Dim res As IEXGateway.IEXResult

        res = Invoke("GetCurrentAudioLanguage", Me)
        Try
            audioLanguage = DirectCast(ReturnValues(0), EnumLanguage)
        Catch ex As Exception
        End Try

        Return res
    End Function

    ''' <summary>
    '''   Get current subtitle language (from channel bar)
    ''' </summary>
    ''' <returns>IEXGateway._IEXResult</returns>
    ''' <remarks></remarks>
    Public Function GetCurrentSubtitleLanguage(ByRef subtitleLanguage As EnumLanguage) As IEXGateway._IEXResult
        Dim res As IEXGateway.IEXResult

        res = Invoke("GetCurrentSubtitleLanguage", Me)
        Try
            subtitleLanguage = DirectCast(ReturnValues(0), EnumLanguage)
        Catch ex As Exception
        End Try

        Return res
    End Function

    ''' <summary>
    '''   Get current subtitle type (from channel bar)
    ''' </summary>
    ''' <returns>IEXGateway._IEXResult</returns>
    ''' <remarks></remarks>
    Public Function GetCurrentSubtitleType(ByRef subtitletype As EnumSubtitleType) As IEXGateway._IEXResult
        Dim res As IEXGateway.IEXResult

        res = Invoke("GetCurrentSubtitletype", Me)
        Try
            subtitletype = DirectCast(ReturnValues(0), EnumSubtitleType)
        Catch ex As Exception
        End Try

        Return res
    End Function

#End Region

#End Region
#Region "RMS"
    Public Function RMSLoginAndEnterBoxid(ByVal driver As FirefoxDriver, ByVal CPEId As String, ByVal TabId As String) As IEXGateway._IEXResult
        Return Invoke("RMSLoginAndEnterBoxid", driver, CPEId, TabId, Me)
    End Function
	 Public Function RMSLoginAndQuickActions(ByVal driver As FirefoxDriver, ByVal cpeId As String, ByVal quickActionId As String, ByVal quickActionConfirmId As String) As IEXGateway._IEXResult
        Return Invoke("RMSLoginAndQuickActions", driver, cpeId, quickActionId, quickActionConfirmId, Me)
    End Function
#End Region

#Region "UserLibFunctions"
    ''' <summary>
    '''   Replace The FailStep Function Of The Userlib
    ''' </summary>
    ''' <returns>IEXGateway._IEXResult</returns>
    ''' <remarks></remarks>
    Public Function FailStep() As IEXGateway._IEXResult
        Return Invoke("FailStep", Me)
    End Function

    ''' <summary>
    '''   Replace The TearDown Function Of The UserLib
    ''' </summary>
    ''' <returns>IEXGateway._IEXResult</returns>
    ''' <remarks></remarks>
    Public Function TearDown() As IEXGateway._IEXResult
        Return Invoke("TearDown", Me)
    End Function

#End Region

#Region "TelnetFunctions"

#Region "TelnetLogIn"

    ''' <summary>
    '''   Login To Telnet
    ''' </summary>
    ''' <param name="Telnet1">Optional Paramater Default = True, If True Send To Telnet 1 Else Sends To Telnet 2</param>
    ''' <returns>Boolean</returns>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>330 - TelnetFailure</para> 
    ''' <para>332 - NoValidParameters</para> 
    ''' </remarks>
    Public Function TelnetLogIn(Optional ByVal Telnet1 As Boolean = True, Optional ByVal LoginToGw As Boolean = True) As Boolean
        Dim iniFile As AMS.Profile.Ini
        Dim Host As String = ""
        Dim GwHost As String = ""
        Dim Prompt As String = ""
        Dim UserName As String = ""
        Dim NeedsLogin As String = ""
        Try
            iniFile = New AMS.Profile.Ini("C:\Program Files\IEX\Tests\TestsINI\IEX" + _iex.IEXServerNumber.ToString + "\Telnet.ini")
            Host = iniFile.GetValue("TELNET-iex" & _iex.IEXServerNumber.ToString, "unixServer").ToString
            GwHost = iniFile.GetValue("TELNET-iex" & _iex.IEXServerNumber.ToString, "gatewayIP").ToString
            Prompt = iniFile.GetValue("TELNET-iex" & _iex.IEXServerNumber.ToString, "prompt").ToString
            UserName = iniFile.GetValue("TELNET-iex" & _iex.IEXServerNumber.ToString, "username").ToString
            NeedsLogin = UI.Utils.GetValueFromProject("TELNET", "NEED_LOGIN")
        Catch ex As Exception
            ExceptionUtils.ThrowEx(New EAException(FailuresHandler.ExitCodes.INIFailure, "Problem Reading ini Values"))
        End Try

        Dim ResLogin As IEXGateway._IEXResult = Nothing
        Dim Res As IEXGateway._IEXResult = Nothing
        Dim ResConnect As IEXGateway._IEXResult = Nothing
        Dim Actual As String = ""
        Dim Msg As String = ""

        Dim Device As IEXGateway.DebugDevice
        If Telnet1 Then
            Device = IEXGateway.DebugDevice.Telnet
        Else
            Device = IEXGateway.DebugDevice.Telnet2
        End If

        ResLogin = _iex.Debug.BeginWaitForMessage(Prompt, 0, 120, Device)
        If Not ResLogin.CommandSucceeded Then
            UI.Utils.StartHideFailures("Trying To Close Begin Wait For Prompt")
            _iex.Debug.EndWaitForMessage(Prompt, Actual, "", Device)
            _iex.ForceHideFailure()
            ExceptionUtils.ThrowEx(New IEXException(ResLogin))
        End If

        If Telnet1 Then
            Res = _iex.Debug.ConnectTo(Host, IEXGateway.DebugDevice.Telnet)
            _iex.Wait(4)
            If Not Res.CommandSucceeded Then
                UI.Utils.StartHideFailures("Trying To Close Begin Wait For Prompt")
                _iex.Debug.EndWaitForMessage(Prompt, Actual, "", IEXGateway.DebugDevice.Telnet)
                _iex.ForceHideFailure()
                ExceptionUtils.ThrowEx(New IEXException(Res))
            Else
                Res = _iex.Debug.WriteLine(UserName, IEXGateway.DebugDevice.Telnet)
                _iex.Wait(2)
                If Not Res.CommandSucceeded Then
                    UI.Utils.StartHideFailures("Trying To Close Begin Wait For Prompt")
                    _iex.Debug.EndWaitForMessage(Prompt, Actual, "", IEXGateway.DebugDevice.Telnet)
                    _iex.ForceHideFailure()
                    ExceptionUtils.ThrowEx(New IEXException(Res))
                End If
            End If
        Else 'Only For Copy PCAT And Copy Logs
            Dim IP As String = ""

            If LoginToGw Then
                IP = GwHost
            Else
                IP = Host
            End If

            For i As Integer = 1 To 5

                ' Ping The box
                Dim reply As PingReply
                reply = New Ping().Send(IP)

                If reply.Status = IPStatus.Success Then
                    _iex.LogComment("Success Ping...!!!")
                Else
                    _iex.LogComment("Failed Ping...!!!")
                End If
                If NeedsLogin.ToUpper() = "TRUE" Then
                    ResLogin = _iex.Debug.BeginWaitForMessage("login", 0, 10, IEXGateway.DebugDevice.Telnet2)
                End If
                Res = _iex.Debug.ConnectTo(IP, IEXGateway.DebugDevice.Telnet2)

                If NeedsLogin = "True" Then
                    _iex.Debug.EndWaitForMessage("login", Actual, "", IEXGateway.DebugDevice.Telnet2)
                    _iex.LogComment("Actual log found::" + Actual)
                End If

                If Not Res.CommandSucceeded Then
                    ResConnect = _iex.Debug.Disconnect(IEXGateway.DebugDevice.Telnet2)
                    Continue For
                ElseIf NeedsLogin = "True" And (Actual = "") Then
                    Res = _iex.Debug.Disconnect(IEXGateway.DebugDevice.Telnet2)
                    Continue For
                Else
                    If NeedsLogin = "True" Then
                        Res = _iex.Debug.WriteLine(UserName, IEXGateway.DebugDevice.Telnet2)
                        _iex.Wait(2)
                        If Not Res.CommandSucceeded Then
                            ResConnect = _iex.Debug.Disconnect(IEXGateway.DebugDevice.Telnet2)
                            Continue For
                        Else
                            Exit For
                        End If
                    End If
                    Exit For
                End If
            Next
        End If

        Res = _iex.Debug.WriteLine("", Device)
        _iex.Wait(2)
        If Not Res.CommandSucceeded Then
            UI.Utils.StartHideFailures("Trying To Close Begin Wait For Prompt")
            _iex.Debug.EndWaitForMessage(Prompt, Actual, "", Device)
            _iex.LogComment("Actual log found::" + Actual)
            _iex.ForceHideFailure()
            ExceptionUtils.ThrowEx(New IEXException(Res))
        Else
            ResLogin = _iex.Debug.EndWaitForMessage(Prompt, Actual, "", Device)
            _iex.LogComment("Actual log found::" + Actual)
            If Not ResLogin.CommandSucceeded Then
                ExceptionUtils.ThrowEx(New EAException(FailuresHandler.ExitCodes.TelnetFailure, "Telnet Login Failed To Get Prompt : " & ResLogin.FailureReason + " Actual Line : " + Actual))
            End If
        End If

        Return True
    End Function

#End Region

#Region "SendCmd"
    ''' <summary>
    '''   Sends Command To Telnet
    ''' </summary>
    ''' <param name="Cmd">The Command To Send</param>
    ''' <param name="Telnet1">Optional Paramater Default = True, If True Send To Telnet 1 Else Sends To Telnet 2</param>
    ''' <param name="validateStrings">Optional Paramater Default = Nothing,Validates the set of strings</param>
    ''' <param name="Actuallines">Optional Paramater Default = Nothing, returns the actual logs for validateStrings param</param>
    ''' <returns>Boolean</returns>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>330 - TelnetFailure</para> 
    ''' </remarks>
    Public Function SendCmd(ByVal Cmd As String, Optional ByVal Telnet1 As Boolean = True, Optional ByRef ValidateStrings As String() = Nothing, Optional ByVal ActualLines As String() = Nothing) As Boolean
        Dim Res As IEXGateway._IEXResult
        Dim iniFile As New AMS.Profile.Ini("C:\Program Files\IEX\Tests\TestsINI\IEX" + _iex.IEXServerNumber.ToString + "\Telnet.ini")
        Dim Prompt As String = iniFile.GetValue("TELNET-iex" & _iex.IEXServerNumber.ToString, "prompt").ToString
        Dim ResPrompt As IEXGateway._IEXResult
        Dim ResValidateString As IEXGateway._IEXResult
        Dim Actual As String = ""
        Dim fail_flag As Boolean = False

        If Telnet1 = False Then
            ResPrompt = _iex.Debug.BeginWaitForMessage(Prompt, 0, 20, IEXGateway.DebugDevice.Telnet2)
            If Not IsNothing(ValidateStrings) Then
                For i As Integer = 0 To ValidateStrings.Length - 1
                    ResValidateString = _iex.Debug.BeginWaitForMessage(ValidateStrings(i), 0, 20, IEXGateway.DebugDevice.Telnet2)
                Next
            End If
        End If

        If Telnet1 Then
            Res = _iex.Debug.WriteLine(Cmd, IEXGateway.DebugDevice.Telnet)
        Else
            Res = _iex.Debug.WriteLine(Cmd, IEXGateway.DebugDevice.Telnet2)
        End If

        If Not Res.CommandSucceeded Then
            UI.Utils.LogCommentFail("Failed To Send Command : " & Res.FailureReason)
            fail_flag = True
        Else
            If Telnet1 = False And Not IsNothing(ValidateStrings) Then
                For j As Integer = 0 To ValidateStrings.Length - 1
                    ResValidateString = _iex.Debug.EndWaitForMessage(ValidateStrings(j), ActualLines(j), "", IEXGateway.DebugDevice.Telnet2)
                    If Not ResValidateString.CommandSucceeded Then
                        UI.Utils.LogCommentFail("Failed to validate string: " & ValidateStrings(j) & ResValidateString.FailureReason)
                        fail_flag = True
                    Else
                        UI.Utils.LogCommentInfo("Got Message:" & ActualLines(j))
                    End If
                Next
            End If
        End If

        If Telnet1 = False Then
            If Not fail_flag Then
                Res = _iex.Debug.WriteLine("", IEXGateway.DebugDevice.Telnet2)
                If Not Res.CommandSucceeded Then
                    UI.Utils.LogCommentFail("Failed to Send command: " & Res.FailureReason)
                    fail_flag = True
                End If
            End If

            ResPrompt = _iex.Debug.EndWaitForMessage(Prompt, Actual, "", IEXGateway.DebugDevice.Telnet2)

            If Not ResPrompt.CommandSucceeded Then
                ExceptionUtils.ThrowEx(New EAException(FailuresHandler.ExitCodes.TelnetFailure, "Failed To Get Prompt After Send Command : " & ResPrompt.FailureReason))
            ElseIf fail_flag = True Then
                ExceptionUtils.ThrowEx(New EAException(FailuresHandler.ExitCodes.TelnetFailure, "Failed To Send Command: " & Cmd))
            End If
        End If

        Return True

    End Function

#End Region

#Region "TelnetDisconnect"
    Public Sub TelnetDisconnect(Optional ByVal Telnet1 As Boolean = True)
        If Telnet1 Then
            _iex.Wait(10)
            _iex.Debug.Disconnect(IEXGateway.DebugDevice.Telnet)
        Else
            _iex.Debug.Disconnect(IEXGateway.DebugDevice.Telnet2)
        End If

    End Sub
#End Region

#End Region

#Region "Exceptions"

    Public Function IgnoreExceptions(ByVal Exceptions As String) As Boolean
        If FailuresHandler.ExceptionUtils.InsertToIgnoredExceptions(Exceptions) Then
            UI.Utils.LogCommentWarning("Ignoring FailureCodes : " + Exceptions)
            Return True
        End If

        Return False
    End Function

    Public Function IgnoreExceptions(ByVal ParamArray ExitCodes() As ExitCodes) As Boolean
        Dim Codes As String = ""

        If FailuresHandler.ExceptionUtils.InsertToIgnoredExceptions(ExitCodes) Then
            For Each eCode As String In ExitCodes
                Codes += eCode + ","
            Next
            UI.Utils.LogCommentWarning("Ignoring FailureCodes : " + Codes.Remove(Codes.LastIndexOf(","), 1))
            Return True
        End If

        Return False
    End Function

    Public Function ClearIgnoredExceptions() As Boolean
        If FailuresHandler.ExceptionUtils.ClearIgnoredExceptions Then
            UI.Utils.LogCommentWarning("Clearing Ignored Exceptions")
            Return True
        End If

        Return False
    End Function
#End Region

End Class

