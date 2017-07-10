Imports FailuresHandler
Imports System.Drawing
Imports System.IO
Imports System.Reflection


Public Class Utils
    Inherits IEX.ElementaryActions.EPG.SF.Utils

    Dim _UI As UI
    Private IR As CommandsDispatcher.CommandsDispatcher

    Sub New(ByVal _pIex As IEXGateway.IEX, ByVal pUI As IEX.ElementaryActions.EPG.SF.UI)
        MyBase.New(_pIex, pUI)
        _UI = pUI
        StaticParam("EPG_Info") = New Dictionary(Of String, String)
        DirectCast(StaticParam("EPG_Info"), Dictionary(Of String, String)).Add("BG", "600,500")
        DirectCast(StaticParam("EPG_Info"), Dictionary(Of String, String)).Add("CONTINUE", "327,576")
        DirectCast(StaticParam("EPG_Info"), Dictionary(Of String, String)).Add("SELECT A PROFILE", "427,576")
        StaticParam("TV.Live.Events") = New Dictionary(Of String, Dictionary(Of String, EventData))
        StaticParam("VOD.Events") = New Dictionary(Of String, Dictionary(Of String, EventData))
        StaticParam("ChannelBar.Events") = New Dictionary(Of String, Dictionary(Of String, EventData))
        StaticParam("Planner.Events") = New Dictionary(Of String, Dictionary(Of String, EventData))
        StaticParam("Archive.Events") = New Dictionary(Of String, Dictionary(Of String, EventData))
        StaticParam("ChannelList") = New Dictionary(Of String, Dictionary(Of String, EventData))
    End Sub

    Public Overrides Sub InitAgent()
        Try
            Dim AgentHost As String = DirectCast(StaticParam("Environment"), Dictionary(Of String, String))("MobileAgentHost")
            Dim AgentPort As String = DirectCast(StaticParam("Environment"), Dictionary(Of String, String))("MobileAgentPort")
            IR = New CommandsDispatcher.CommandsDispatcher(CInt(AgentPort), AgentHost)
            If Not IR.isConnected Then
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.IEXSystemError, "Unable To Initialize Mobile Agent"))
            End If
        Catch ex As Exception
            ExceptionUtils.ThrowEx(New EAException(ExitCodes.IEXSystemError, "Unable To Initialize Mobile Agent"))
        End Try
    End Sub

    ''' <summary>
    '''   Getting All EPG Texts From Dictionary
    ''' </summary>
    ''' <returns>Boolean</returns>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>301 - DictionaryFailure</para> 
    ''' </remarks>
    Public Overrides Function GetTextsFromDictionary() As Boolean
        Dim _Dictionary As New Dictionary(Of String, String)
        Dim _iexNumber As String = _iex.IEXServerNumber

        _UI.Utils.LogCommentInfo("GetTextsFromDictionary : Trying To Load Dictionary From C:\Program Files\IEX\Tests\TestsINI\IEX" + _iexNumber + "\Localizable.strings")

        Dim Texts As String() = File.ReadAllLines("C:\Program Files\IEX\Tests\TestsINI\IEX" + _iexNumber + "\Localizable.strings")
        Dim DIC As String = ""
        Dim Value As String = ""

        For Each l As String In Texts
            If l.StartsWith(Chr(34)) Then
                Try
                    l = l.Replace(Chr(34), "")
                    DIC = l.Substring(0, l.IndexOf(" ="))
                    l = l.Remove(0, l.IndexOf(" =") + 3)
                    Value = l.Substring(0, l.IndexOf(";"))
                    _Dictionary.Add(DIC, Value)
                Catch ex As Exception
                    LogCommentWarning("WARNING : Same Key Already Exist On Dictionary : " + DIC.ToString)
                End Try
            End If
        Next

        _UI.Utils.StaticParam("Dictionary") = _Dictionary
        Return True
    End Function

    Public Function GetMobileEPGInfo(ByVal Key As String) As String
        Dim ReturnedValue As String = ""

        If DirectCast(_UI.Utils.StaticParam("EPG_Info"), Dictionary(Of String, String)).Count <= 0 Then
            ExceptionUtils.ThrowEx(New EAException(ExitCodes.DictionaryFailure, "Problem With Mobile Dictionary No Keys Available!"))
        End If

        Try
            ReturnedValue = DirectCast(_UI.Utils.StaticParam("EPG_Info"), Dictionary(Of String, String))(Key)
            LogCommentInfo("Got " + ReturnedValue + " For Key " + Key + " From Mobile Dictionary")
        Catch ex As Exception
            ExceptionUtils.ThrowEx(New EAException(ExitCodes.DictionaryFailure, "Key " + Key + " Was Not Found In Mobile Dictionary"))
        End Try

        If ReturnedValue = "" Then
            ExceptionUtils.ThrowEx(New EAException(ExitCodes.DictionaryFailure, "Key " + Key + " Value Is Empty In Mobile Dictionary!"))
        End If

        Return ReturnedValue
    End Function

    Public Function GetMobileEventInfo(ByVal Key As String, ByVal Info As String, ByVal Source As String) As String
        Dim ReturnedValue As String = ""

        If DirectCast(_UI.Utils.StaticParam(Source), Dictionary(Of String, EventData)).Count <= 0 Then
            ExceptionUtils.ThrowEx(New EAException(ExitCodes.DictionaryFailure, "Problem With Mobile Events No Keys Available!"))
        End If

        Try
            Dim pi As PropertyInfo = DirectCast(_UI.Utils.StaticParam(Source), Dictionary(Of String, EventData))(Key).GetType.GetProperty(Info)

            ReturnedValue = pi.GetValue(DirectCast(_UI.Utils.StaticParam(Source), Dictionary(Of String, EventData))(Key), Nothing)
            LogCommentInfo("Got " + ReturnedValue + " For Info " + Info + " From Mobile Events Dictionary")
        Catch ex As Exception
            ExceptionUtils.ThrowEx(New EAException(ExitCodes.DictionaryFailure, "Info " + Info + " Was Not Found In Mobile Events"))
        End Try

        If ReturnedValue = "" Then
            ExceptionUtils.ThrowEx(New EAException(ExitCodes.DictionaryFailure, "info " + Info + " Value Is Empty In Mobile Events !"))
        End If

        Return ReturnedValue
    End Function

    Public Sub SwipeHorizontal(ByVal Source As String, ByVal IsSwipeRight As Boolean, ByVal WaitAfterSwipe As Integer, Optional ByVal VerifyAnimation As Boolean = True)
        Dim res As New IEXGateway.IEXResult
        Dim UDPBlock As String = ""
        Dim Coordinates As String = ""
        Dim X As Integer
        Dim Y As Integer
        Dim Distance As Integer
        Dim Direction As String

        If Source = "" Then
            If Not DirectCast(StaticParam("EPG_Info"), Dictionary(Of String, String)).ContainsKey("swArea") Then
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.SwipeNotAvaiable, "swArea Doesn't Exists In The EPG_Info"))
            End If
        End If

        Select Case Source
            Case "TV.Live.Events"
                Distance = 870
            Case Else
                Distance = 500
        End Select

        Coordinates = DirectCast(StaticParam("EPG_Info"), Dictionary(Of String, String))("swArea")

        GetSwipeXY(Coordinates, X, Y)

        If IsSwipeRight Then
            Direction = "RIGHT"
        Else
            Direction = "LEFT"
        End If

        StartHideFailures("Swiping Horizontal : X=" + X.ToString + " Y=" + Y.ToString + " Distance=" + Distance.ToString + " And Waiting After : " + WaitAfterSwipe.ToString)

        _iex.Debug.InsertTagIntoLog(IEXGateway.Engine.Commands.NewDebug.TagAction.Start, "Collecting Events", IEXGateway.DebugDevice.Udp)

        Dim ActualLines As New ArrayList

        Try

            If VerifyAnimation Then
                _UI.Utils.BeginWaitForDebugMessages("swArea", 10)
            End If

            Dim result As String = IR.swipe(New Point(X, Y), Direction, Distance)
            If result = "ERROR" Then
                Try
                    _UI.Utils.EndWaitForDebugMessages("swArea", ActualLines)
                Catch ex As Exception
                End Try
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.IRVerificationFailure, "Failed To Send Swipe X : " + X.ToString + " Y : " + Y.ToString + " Distance : " + Distance.ToString))
            End If

            If VerifyAnimation Then
                If Not _UI.Utils.EndWaitForDebugMessages("swArea", ActualLines) Then
                    ExceptionUtils.ThrowEx(New EAException(ExitCodes.SwipeNotAvaiable, "Failed To Verify swArea After Swipe"))
                End If
            End If

            _iex.Wait(WaitAfterSwipe / 1000)

            _iex.Debug.InsertTagIntoLog(IEXGateway.Engine.Commands.NewDebug.TagAction.End, "Collecting Events", IEXGateway.DebugDevice.Udp)

            res = _iex.Debug.ImportLogPart("Collecting Events", UDPBlock, IEXGateway.DebugDevice.Udp)

            ParseBlock(UDPBlock)

            ' ExceptionUtils.ThrowEx(New EAException(ExitCodes.IRVerificationFailure, "Failed To Verify Send IR " + IRKey.ToString))

        Finally
            _iex.ForceHideFailure()
        End Try

    End Sub

    Public Sub SwipeVertical(ByVal Source As String, ByVal IsSwipeUp As Boolean, ByVal WaitAfterSwipe As Integer, Optional ByVal VerifyAnimation As Boolean = True)
        Dim res As New IEXGateway.IEXResult
        Dim UDPBlock As String = ""
        Dim Coordinates As String = ""
        Dim X As Integer
        Dim Y As Integer
        Dim Distance As Integer
        Dim Direction As String


        Coordinates = DirectCast(StaticParam("EPG_Info"), Dictionary(Of String, String))("swArea")

        GetSwipeXY(Coordinates, X, Y)

        Select Case Source
            Case "ChannelBar.Events"
                X += 150
                Distance = 500
            Case Else
                Distance = 500
        End Select


        If IsSwipeUp Then
            Direction = "UP"
        Else
            Direction = "DOWN"
        End If

        StartHideFailures("Swiping Vertical : X=" + X.ToString + " Y=" + Y.ToString + " Distance=" + Distance.ToString + " And Waiting After : " + WaitAfterSwipe.ToString)

        _iex.Debug.InsertTagIntoLog(IEXGateway.Engine.Commands.NewDebug.TagAction.Start, "Collecting Events", IEXGateway.DebugDevice.Udp)

        Dim ActualLines As New ArrayList

        Try
            If VerifyAnimation Then
                _UI.Utils.BeginWaitForDebugMessages("swArea", 10)
            End If

            Dim result As String = IR.swipe(New Point(X, Y), Direction, Distance)
            If result = "ERROR" Then
                _UI.Utils.EndWaitForDebugMessages("swArea", ActualLines)
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.IRVerificationFailure, "Failed To Send Swipe X : " + X.ToString + " Y : " + Y.ToString + " Distance : " + Distance.ToString))
            End If

            If VerifyAnimation Then
                If Not _UI.Utils.EndWaitForDebugMessages("swArea", ActualLines) Then
                    ExceptionUtils.ThrowEx(New EAException(ExitCodes.SwipeNotAvaiable, "Failed To Verify swArea After Swipe"))
                End If
            End If

            _iex.Wait(WaitAfterSwipe / 1000)

            _iex.Debug.InsertTagIntoLog(IEXGateway.Engine.Commands.NewDebug.TagAction.End, "Collecting Events", IEXGateway.DebugDevice.Udp)

            res = _iex.Debug.ImportLogPart("Collecting Events", UDPBlock, IEXGateway.DebugDevice.Udp)

            ParseBlock(UDPBlock)

            ' ExceptionUtils.ThrowEx(New EAException(ExitCodes.IRVerificationFailure, "Failed To Verify Send IR " + IRKey.ToString))

        Finally
            _iex.ForceHideFailure()
        End Try

    End Sub

    Public Overloads Sub Tap(ByVal X As Integer, ByVal Y As Integer, ByVal WaitAfterTap As Integer)
        StartHideFailures("Tapping : X=" + X.ToString + " Y=" + Y.ToString + " And Waiting After : " + WaitAfterTap.ToString)
        Dim ActualLines As New ArrayList

        Try
            ' _UI.Utils.BeginWaitForDebugMessages("anEnd", 10)
            '"327,576"
            Dim result As String = IR.tapAtIndex(New Point(X, Y))
            If result = "ERROR" Then
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.IRVerificationFailure, "Failed To Send Tap X : " + X.ToString + " Y : " + Y.ToString))
            End If
            'If Not _UI.Utils.EndWaitForDebugMessages("anEnd", ActualLines) Then
            '    ExceptionUtils.ThrowEx(New EAException(ExitCodes.AnimationFailure, "Failed To Verify anEnd After Tap"))
            'End If

            _iex.Wait(WaitAfterTap / 1000)

            ' ExceptionUtils.ThrowEx(New EAException(ExitCodes.IRVerificationFailure, "Failed To Verify Send IR " + IRKey.ToString))

        Finally
            _iex.ForceHideFailure()
        End Try

    End Sub

    ''' <summary>
    '''   Tap Requested Area
    ''' </summary>
    ''' <param name="Item">The Key To Send</param>
    ''' <param name="VerifyAnimation">Optional Parameter Default True : If True Verifying Animation End After IR</param>
    ''' <param name="WaitAfterTap">Optional Parameter Default 1000 : Wait After Sending</param>
    ''' <remarks></remarks>
    Public Overloads Sub Tap(ByVal Item As String, Optional ByVal Source As String = "", Optional ByVal VerifyAnimation As Boolean = True, Optional ByVal WaitAfterTap As Integer = 2000)
        Dim res As New IEXGateway.IEXResult
        Dim ActualLines As New ArrayList
        Dim UDPBlock As String = ""
        Dim Coordinates As String = ""
        Dim X As Integer
        Dim Y As Integer

        If Item = "" Then
            ExceptionUtils.ThrowEx(New EAException(ExitCodes.NoValidParameters, "Tap Item Is Empty !!!"))
        End If

        If String.IsNullOrEmpty(Source) Then
            Coordinates = GetMobileEPGInfo(Item)
        Else
            Coordinates = GetMobileEventInfo(Item, "cord", Source)
        End If

        GetTapXY(Coordinates, X, Y)

        StartHideFailures("Tapping : " + Item + " X=" + X.ToString + " Y=" + Y.ToString + " And Waiting After : " + WaitAfterTap.ToString)
        Try
            _iex.Debug.InsertTagIntoLog(IEXGateway.Engine.Commands.NewDebug.TagAction.Start, "Collecting Items", IEXGateway.DebugDevice.Udp)

            If VerifyAnimation Then
                _UI.Utils.BeginWaitForDebugMessages("anEnd", 10)
            End If

            Dim result As String = IR.tapAtIndex(New Point(X, Y))
            If result = "ERROR" Then
                Try
                    _UI.Utils.EndWaitForDebugMessages("anEnd", ActualLines)
                Catch ex As Exception
                End Try
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.IRVerificationFailure, "Failed To Send Tap X : " + X.ToString + " Y : " + Y.ToString))
            End If

            If VerifyAnimation Then
                If Not _UI.Utils.EndWaitForDebugMessages("anEnd", ActualLines) Then
                    ExceptionUtils.ThrowEx(New EAException(ExitCodes.AnimationFailure, "Failed To Verify Animation End"))
                End If
            End If

            _iex.Wait(WaitAfterTap / 1000)

            _iex.Debug.InsertTagIntoLog(IEXGateway.Engine.Commands.NewDebug.TagAction.End, "Collecting Items", IEXGateway.DebugDevice.Udp)

            res = _iex.Debug.ImportLogPart("Collecting Items", UDPBlock, IEXGateway.DebugDevice.Udp)

            ParseBlock(UDPBlock)

            ' ExceptionUtils.ThrowEx(New EAException(ExitCodes.IRVerificationFailure, "Failed To Verify Send IR " + IRKey.ToString))

        Finally
            _iex.ForceHideFailure()
        End Try
    End Sub

    Public Sub GetTapXY(ByVal Cords As String, ByRef X As Integer, ByRef Y As Integer)
        Dim Xy As String() = Cords.Split(",")

        X = CInt(Xy(0))
        Y = CInt(Xy(1))
    End Sub

    Private Sub GetSwipeXY(ByVal Cords As String, ByRef X As Integer, ByRef Y As Integer)
        Dim Xy As String() = Cords.Split(",")

        X = CInt(Xy(0)) + (CInt(Xy(2)) \ 2)
        Y = CInt(Xy(1)) + (CInt(Xy(3)) \ 2)
    End Sub

    Public Sub EnterString(ByVal txt As String, Optional ByVal SecondsToWaitAfter As Integer = 4)
        Dim UDPBlock As String = ""

        StartHideFailures("Entering Text : " + txt)

        Try
            _iex.Debug.InsertTagIntoLog(IEXGateway.Engine.Commands.NewDebug.TagAction.Start, "Collecting Items", IEXGateway.DebugDevice.Udp)

            'Dim result As String = IR.doCommand("<Command><CommandType>ENTERTEXT</CommandType><Text>" + txt + "</Text></Command>")
            Dim result As String = IR.enterText(txt)
            If result = "ERROR" Then
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.IRVerificationFailure, "Failed To Send Text : " + txt))
            End If

            _iex.Wait(SecondsToWaitAfter)

            _iex.Debug.InsertTagIntoLog(IEXGateway.Engine.Commands.NewDebug.TagAction.End, "Collecting Items", IEXGateway.DebugDevice.Udp)

            _iex.Debug.ImportLogPart("Collecting Items", UDPBlock, IEXGateway.DebugDevice.Udp)

            ParseBlock(UDPBlock)

        Finally
            _iex.ForceHideFailure()
        End Try

    End Sub

    ''' <summary>
    '''   Entering PIN
    ''' </summary>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>300 - NavigationFailure</para> 
    ''' <para>301 - DictionaryFailure</para> 
    ''' <para>304 - IRVerificationFailure</para> 
    ''' <para>328 - INIFailure</para> 
    ''' </remarks>
    Public Overrides Sub EnterPin(ByVal PIN As String)
        Dim EpgText As String = ""
        Dim _PIN As String = ""
        Dim UDPBlock As String = ""

        If PIN = "" Then
            _PIN = GetValueFromEnvironment("DefaultPIN")

            If _PIN.Length <> 4 Then
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.INIFailure, "PIN Length Was Less Than 4 Digits Please Check DefaultPIN In Your Environment.ini"))
            End If
        Else
            _PIN = PIN
        End If

        StartHideFailures("Entering PIN : " + _PIN)

        Try
            EnterString(_PIN)
        Finally
            _iex.ForceHideFailure()
        End Try
    End Sub

    Private Function ParseEvents(ByVal Line As String, ByVal EventType As EnumEventType, Optional ByVal VodIndx As Integer = 1) As Dictionary(Of String, EventData)
        Dim Items As String()
        Dim _Dictonary As New Dictionary(Of String, EventData)

        Select Case EventType
            Case EnumEventType.ChannelList
                Line = Line.Remove(0, Line.IndexOf("title"))
            Case Else
                Line = Line.Remove(0, Line.IndexOf("type"))
        End Select

        Items = Line.Split(New String() {"@#$"}, System.StringSplitOptions.RemoveEmptyEntries)
        If Items.Contains("cord") Then
            Dim e As New EventData

            For i As Integer = 0 To Items.Count - 1 Step 2
                Dim pi As PropertyInfo = e.GetType().GetProperty(Items(i).ToString)
                pi.SetValue(e, Items(i + 1).ToString, Nothing)
            Next

            Select Case EventType
                Case EnumEventType.VODEvents
                    _Dictonary.Add(e.type + VodIndx.ToString, e)
                Case EnumEventType.TVEvents, EnumEventType.PlannerEvents, EnumEventType.ArchiveEvents
                    _Dictonary.Add(e.evtNa, e)
                Case EnumEventType.NowNextEvents
                    _Dictonary.Add(e.type, e)
                Case EnumEventType.ChannelList
                    _Dictonary.Add(e.title, e)
            End Select


        End If

        Return _Dictonary
    End Function

    Private Function ParseLine(ByVal Line As String, ByVal LineType As EnumLineType) As Dictionary(Of String, String)
        Dim Items As String()
        Dim _Dictonary As New Dictionary(Of String, String)

        Select Case LineType
            Case EnumLineType.MenuItem
                Line = Line.Remove(0, Line.IndexOf("title"))
                Items = Line.Split(New String() {"@#$"}, System.StringSplitOptions.RemoveEmptyEntries)
                _Dictonary.Add(Items(1).ToString, Items(3).ToString.TrimStart("@#$"))

            Case EnumLineType.Swipe
                Line = Line.Remove(0, Line.IndexOf("Selection ") + 10)
                Items = Line.Split(New String() {"@#$"}, System.StringSplitOptions.RemoveEmptyEntries)
                _Dictonary.Add(Items(0).ToString, Items(3).ToString.TrimStart("@#$"))
        End Select

        Return _Dictonary
    End Function

    Private Sub ParseBlock(ByVal UDPBlock As String)
        Dim VodIndx As Integer = 1
        Dim UDPLogItems As String()
        Dim _Dictonary As New Dictionary(Of String, String)
        _Dictonary.Add("BG", "600,500")
        _Dictonary.Add("CONTINUE", "411,576")
        _Dictonary.Add("SELECT A PROFILE", "427,576")
        Dim _TVDictonary As New Dictionary(Of String, EventData)
        Dim _VODDictonary As New Dictionary(Of String, EventData)
        Dim _ChannelBarDictonary As New Dictionary(Of String, EventData)
        Dim _PlannerDictonary As New Dictionary(Of String, EventData)
        Dim _ArchiveDictonary As New Dictionary(Of String, EventData)
        Dim EvResultDictionary As New Dictionary(Of String, EventData)
        Dim _ChannelListDictionary As New Dictionary(Of String, EventData)
        Dim LineResultDictionary As New Dictionary(Of String, String)

        UDPLogItems = UDPBlock.TrimStart(vbCrLf).TrimEnd(vbCrLf).Split(vbCrLf)

        For Each BlockLine As String In UDPLogItems

            If BlockLine.Contains("evtNa@#$(null)") Then
                Continue For
            End If

            If BlockLine.Contains("LiveEvent") Then

                StaticParam("TV.Live.Events") = New Dictionary(Of String, Dictionary(Of String, EventData))
                EvResultDictionary = ParseEvents(BlockLine, EnumEventType.TVEvents)
                _TVDictonary.Add(EvResultDictionary.Keys(0).ToString, EvResultDictionary.Values(0))

            ElseIf BlockLine.Contains("VodEvent") Then

                StaticParam("VOD.Events") = New Dictionary(Of String, Dictionary(Of String, EventData))
                EvResultDictionary = ParseEvents(BlockLine, EnumEventType.VODEvents, VodIndx)
                _VODDictonary.Add(EvResultDictionary.Keys(0).ToString, EvResultDictionary.Values(0))
                VodIndx += 1

            ElseIf BlockLine.Contains("PlannerEvent") Then

                StaticParam("Planner.Events") = New Dictionary(Of String, Dictionary(Of String, EventData))
                EvResultDictionary = ParseEvents(BlockLine, EnumEventType.PlannerEvents)
                _PlannerDictonary.Add(EvResultDictionary.Keys(0).ToString, EvResultDictionary.Values(0))

            ElseIf BlockLine.Contains("ArchiveEvent") Then

                StaticParam("Archive.Events") = New Dictionary(Of String, Dictionary(Of String, EventData))
                EvResultDictionary = ParseEvents(BlockLine, EnumEventType.ArchiveEvents)
                _ArchiveDictonary.Add(EvResultDictionary.Keys(0).ToString, EvResultDictionary.Values(0))

            ElseIf BlockLine.Contains("Selection swArea") Then

                LineResultDictionary = ParseLine(BlockLine, EnumLineType.Swipe)
                If _Dictonary.ContainsKey(LineResultDictionary.Keys(0)) Then
                    _Dictonary(LineResultDictionary.Keys(0)) = LineResultDictionary.Values(0)
                Else
                    _Dictonary.Add(LineResultDictionary.Keys(0), LineResultDictionary.Values(0))
                End If

            ElseIf BlockLine.Contains("NowEvent") OrElse BlockLine.Contains("NextEvent") Then

                StaticParam("ChannelBar.Events") = New Dictionary(Of String, Dictionary(Of String, EventData))
                EvResultDictionary = ParseEvents(BlockLine, EnumEventType.NowNextEvents)
                If Not _ChannelBarDictonary.ContainsKey(EvResultDictionary.Keys(0).ToString) Then
                    _ChannelBarDictonary.Add(EvResultDictionary.Keys(0).ToString, EvResultDictionary.Values(0))
                Else
                    _ChannelBarDictonary(EvResultDictionary.Keys(0).ToString) = EvResultDictionary.Values(0)
                End If

            ElseIf BlockLine.Contains("Selection title") AndAlso (BlockLine.Contains("isFavor") OrElse BlockLine.Contains("isLocked")) Then

                StaticParam("ChannelList") = New Dictionary(Of String, Dictionary(Of String, EventData))
                EvResultDictionary = ParseEvents(BlockLine, EnumEventType.ChannelList)
                _ChannelListDictionary.Add(EvResultDictionary.Keys(0).ToString, EvResultDictionary.Values(0))

            ElseIf BlockLine.Contains("Selection title") AndAlso Not BlockLine.Contains("CDLiveEventView") _
                AndAlso Not BlockLine.Contains("isFavor") AndAlso Not BlockLine.Contains("isLocked") Then

                LineResultDictionary = ParseLine(BlockLine, EnumLineType.MenuItem)
                If _Dictonary.ContainsKey(LineResultDictionary.Keys(0)) Then
                    _Dictonary(LineResultDictionary.Keys(0)) = LineResultDictionary.Values(0)
                Else
                    _Dictonary.Add(LineResultDictionary.Keys(0), LineResultDictionary.Values(0))
                End If

            End If
        Next

        StaticParam("Archive.Events") = _ArchiveDictonary
        StaticParam("Planner.Events") = _PlannerDictonary
        StaticParam("ChannelBar.Events") = _ChannelBarDictonary
        StaticParam("TV.Live.Events") = _TVDictonary
        StaticParam("ChannelList") = _ChannelListDictionary
        StaticParam("EPG_Info") = _Dictonary
    End Sub

    ''' <summary>
    '''   Return To Live Viewing
    ''' </summary>
    ''' <param name="CheckForVideo">Optional Parameter FALSE.If TRUE Checks For Video</param>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>301 - DictionaryFailure</para> 
    ''' <para>334 - VideoNotPresent</para> 
    ''' <para>349 - ReturnToLiveFailure</para> 
    ''' </remarks>
    Public Overrides Sub ReturnToLiveViewing(Optional ByVal CheckForVideo As Boolean = False)
        Dim Msg As String = ""
        Dim IsLive As Boolean = False

        StartHideFailures("Checking If Already In Live")

        Try
            If VerifyState("CanalDPlayerController", 5) Then
                Msg = "Already On Live"
                IsLive = True
            Else
                Msg = "NOT Already On Live"
                IsLive = False
            End If
        Finally
            _iex.ForceHideFailure()
            If Msg <> "" Then
                LogCommentInfo(Msg)
            End If
        End Try

        If IsLive Then
            StartHideFailures("Lunching Action Bar And Returning To Live To Get Swipe Area Milestone")
            Try
                _UI.Utils.Tap("BG", VerifyAnimation:=True, WaitAfterTap:=1000)
                If Not _UI.Banner.IsActionBar() Then
                    ExceptionUtils.ThrowEx(New EAException(ExitCodes.ReturnToLiveFailure, "Failed To Verify Action Bar Reached"))
                End If
                _UI.Utils.Tap("BG", VerifyAnimation:=False, WaitAfterTap:=2000)
                VerifyLiveReached()
                Exit Sub
            Finally
                _iex.ForceHideFailure()
            End Try
        End If
       
        StartHideFailures("Returning To Live Viewing")
        Try
            _UI.Utils.Tap("BG", VerifyAnimation:=False, WaitAfterTap:=2000)
            Try
                VerifyLiveReached()
            Catch ex As Exception
                _UI.Menu.Navigate()
                _UI.Utils.Tap("BG", VerifyAnimation:=False, WaitAfterTap:=2000)
                VerifyLiveReached()
            End Try
        Finally
            _iex.ForceHideFailure()
        End Try
    End Sub

    ''' <summary>
    '''   Waiting For Given State To Arrive Until Timeout And Delaying After Found
    ''' </summary>
    ''' <param name="State">Requested State</param>
    ''' <param name="TimeOutInSec">Optional Parameter Default = 15 : Time Out In Seconds</param>
    ''' <param name="Delay">Optional Parameter Default = 0 : How Much Seconds To Delay </param>
    ''' <returns>Boolean</returns>
    ''' <remarks></remarks>
    Public Overrides Function VerifyState(ByVal State As String, Optional ByVal TimeOutInSec As Integer = 15, Optional ByVal Delay As Integer = 0) As Boolean
        Dim Timeout As Date = Now.AddSeconds(TimeOutInSec)
        Dim CurrentState As String = ""

        StartHideFailures("Utils.VerifyState : Waiting For State " + State + " Timout Requested : " + TimeOutInSec.ToString + " Timeout Until-> " + Timeout.ToString)

        Try
            Do While DateDiff(DateInterval.Second, Now, Timeout) > 0
                Try
                    GetEpgInfo("state", CurrentState)
                    If CurrentState.Equals(State) Then
                        If Delay <> 0 Then
                            _iex.Wait(Delay)
                        End If
                        Return True
                    End If
                Catch ex As Exception
                End Try
            Loop

            LogCommentFail("VerifyState : Failed To Verify Active State Is " + State.ToString)
            Return False
        Finally
            _iex.ForceHideFailure()
        End Try
    End Function

    ''' <summary>
    '''   Parsing EPG Time From EPG
    ''' </summary>
    ''' <param name="DateTime">EPG Date Time</param>
    ''' <param name="ReturnedTime">ByRef The Returned Time</param>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>350 - ParsingFailure</para> 
    ''' </remarks>
    Overrides Sub ParseEPGTime(ByVal DateTime As String, ByRef ReturnedTime As String)
        Try
            Dim DatePart As String() = DateTime.Split(" ")
            Dim time As String = ""

            time = DatePart(1)

            ReturnedTime = time

        Catch ex As Exception
            ExceptionUtils.ThrowEx(New EAException(ExitCodes.ParsingFailure, "Failed To Parse EPG Time From " + DateTime + " : " + ex.Message))
        End Try
    End Sub

    ''' <summary>
    '''   Parsing Event Time From Returned EPG Time
    ''' </summary>
    ''' <param name="ReturnedTime">The Returned Time After Parsing</param>
    ''' <param name="TimeString">Time String From EPG</param>
    ''' <param name="IsStartTime">If True Returnes Start Time Else Returns End Time</param>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>350 - ParsingFailure</para> 
    ''' </remarks>
    Public Overrides Sub ParseEventTime(ByRef ReturnedTime As String, ByVal TimeString As String, ByVal IsStartTime As Boolean)
        Try
            Dim DatePart As String() = TimeString.Split(" ")
            Dim time As String = ""

            time = DatePart(1)

            ReturnedTime = time

        Catch ex As Exception
            ExceptionUtils.ThrowEx(New EAException(ExitCodes.ParsingFailure, "Failed To Parse Event Time From " + TimeString + " : " + ex.Message))
        End Try
    End Sub

    Public Sub FindInEvents(ByVal Source As String, ByVal Key As String)
        Dim LastEvent As String

        If DirectCast(_UI.Utils.StaticParam(Source), Dictionary(Of String, EventData)).ContainsKey(Key) Then
            Exit Sub
        End If
        If Source = "TV.Live.Events" Then
            If Not DirectCast(_UI.Utils.StaticParam("EPG_Info"), Dictionary(Of String, String)).ContainsKey("swArea") Then
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.SwipeNotAvaiable, "Swipe Not Available Can't Find Event"))
            End If
        End If

        LastEvent = DirectCast(_UI.Utils.StaticParam(Source), Dictionary(Of String, EventData))(DirectCast(_UI.Utils.StaticParam(Source), Dictionary(Of String, EventData)).Keys.Last()).ToString

        Do Until DirectCast(_UI.Utils.StaticParam(Source), Dictionary(Of String, EventData)).ContainsKey(LastEvent)
            If DirectCast(_UI.Utils.StaticParam(Source), Dictionary(Of String, EventData)).ContainsKey(Key) Then
                Exit Sub
            End If
            SwipeHorizontal(Source, False, 1000)
        Loop

        ExceptionUtils.ThrowEx(New EAException(ExitCodes.FindEventFailure, "Failed To Find Event With Key : " + Key))
    End Sub

    ''' <summary>
    '''   Verifies Live Reached
    ''' </summary>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>322 - VerificationFailure</para> 
    ''' </remarks>
    Public Overrides Sub VerifyLiveReached()
        If Not VerifyState("CanalDPlayerController", 10, 2) Then
            ExceptionUtils.ThrowEx(New EAException(ExitCodes.VerificationFailure, "Failed To Verify Live State Reached"))
        End If
    End Sub
End Class


