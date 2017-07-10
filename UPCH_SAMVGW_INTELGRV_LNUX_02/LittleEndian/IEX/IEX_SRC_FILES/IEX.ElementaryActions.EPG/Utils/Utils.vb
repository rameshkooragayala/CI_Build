Imports System.IO
Imports IEX.Utilities


Public Class Utils
    Protected _iex As IEXGateway.IEX
    Protected returnValue As String
    Protected ActualPercentage As Short
    Protected OffLinePath As String
    Public iexlogFolder, ialOfflineFolder As String
    Public _DateTime As EpgDateTime
    Public _Event As EpgEvent
    Public _Channel As EPG.EpgChannel
    Public Warning As String = ""
    Public _StaticParam As New Dictionary(Of String, Object)
    Dim _UI As EPG.UI

    Sub New(ByVal pIex As IEXGateway.IEX, ByVal pUI As EPG.UI)
        _iex = pIex
        _UI = pUI
        _DateTime = New EpgDateTime(_iex)
    End Sub

    Public Property StaticParam(key As String) As Object
        Get
            If _StaticParam.ContainsKey(key) Then
                'if the Key existing
                Return _StaticParam.Item(key)
            Else
                Return Nothing
            End If
        End Get
        Set(ByVal value As Object)
            Try
                If _StaticParam.ContainsKey(key) Then
                    'if the Key existing
                    _StaticParam.Item(key) = Value
                Else
                    _StaticParam.Add(key, Value)
                End If
            Catch ex As Exception
            End Try
        End Set
    End Property

#Region "Get Subs"

    Overridable Sub GetItem(template As String, criteria As String, column As String, ByRef returnedValue As String, numberOfMatches As Integer, numberOfretries As Integer)
        _iex.LogComment("This UI.Utils function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

    Overridable Sub ParseEventTime(ByRef returnedTime As String, timeString As String, isStartTime As Boolean)
        _iex.LogComment("This UI.Utils function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

    Overridable Function GetEpgDateFormat() As String
        _iex.LogComment("This UI.Utils function isn't implemented under the general implementation. Please implement locally in project.")
        Return ""
    End Function

    Overridable Function GetEpgDateFormatDefaultValue() As String
        _iex.LogComment("This UI.Utils function isn't implemented under the general implementation. Please implement locally in project.")
        Return ""
    End Function

    Overridable Function GetDateFormatForEventDictionary() As String
        _iex.LogComment("This UI.Utils function isn't implemented under the general implementation. Please implement locally in project.")
        Return ""
    End Function

    Protected Overridable Function GetDateFormatForEventDicDefaultValue() As String
        _iex.LogComment("This UI.Utils function isn't implemented under the general implementation. Please implement locally in project.")
        Return ""
    End Function

    Protected Overridable Function GetEpgTimeDelimiterDefaultValue() As String
        _iex.LogComment("This UI.Utils function isn't implemented under the general implementation. Please implement locally in project.")
        Return ""
    End Function

    Overridable Function GetEpgTimeDelimiter() As String
        _iex.LogComment("This UI.Utils function isn't implemented under the general implementation. Please implement locally in project.")
        Return ""
    End Function

    Overridable Function GetExpectedEventTimeLength() As Byte
        _iex.LogComment("This UI.Utils function isn't implemented under the general implementation. Please implement locally in project.")
        Return 0
    End Function

    Overridable Function GetBaudRateFromConfigFile(iexServerNumber As Short) As String
        _iex.LogComment("This UI.Utils function isn't implemented under the general implementation. Please implement locally in project.")
        Return ""
    End Function

    Overridable Function GetFullPathToIexIniFile(iexServerNumber As Short) As String
        _iex.LogComment("This UI.Utils function isn't implemented under the general implementation. Please implement locally in project.")
        Return ""
    End Function

    Overridable Function GetFullPathToIexXmlFile(iexServerNumber As Short) As String
        _iex.LogComment("This UI.Utils function isn't implemented under the general implementation. Please implement locally in project.")
        Return ""
    End Function

    Protected Overridable Function GetEventTimeSeparator() As String
        _iex.LogComment("This UI.Utils function isn't implemented under the general implementation. Please implement locally in project.")
        Return ""
    End Function

    Overridable Function GetMinScrollsBetweenServices(currentChannelPosition As Integer, targetChannelPosition As Integer, totalChannel As Integer, ByRef isNext As Boolean) As Integer
        _iex.LogComment("This UI.Utils function isn't implemented under the general implementation. Please implement locally in project.")
        Return False
    End Function

    Overridable Function TranslateLanguage(ByVal Language As String, Optional ByVal fromEnglish As Boolean = True) As String
        _iex.LogComment("This UI.Utils function isn't implemented under the general implementation. Please implement locally in project.")
        Return ""
    End Function
 Overridable Function GetHDDUsagePercentage(Optional ByVal isClearEPG As Boolean = True) As Integer
        _iex.LogComment("This UI.Utils function isn't implemented under the general implementation. Please implement locally in project.")
        Return ""
    End Function

#End Region

#Region "Set Subs"

    Overridable Sub PreRecordRECkey(ByVal isCurrent As Boolean, ByVal isConflict As Boolean, Optional ByVal IsSeries As Boolean = False)
        _iex.LogComment("This UI.Utils function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

    Overridable Sub StopRecordingSTOPkey(Optional ByVal IsSeries As Boolean = False, Optional ByVal IsStopRecording As Boolean = True, Optional ByVal IsCurrent As Boolean = True, Optional ByVal IsTBR As Boolean = False)
        _iex.LogComment("This UI.Utils function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub
	
    Overridable Sub TypeKeys(stringToType As String, Optional waitAfter As Integer = 2000)
        _iex.LogComment("This UI.Utils function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

    Overridable Sub TypeManualRecordingKeys(stringToType As String)
        _iex.LogComment("This UI.Utils function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

    Overridable Sub ResolveClash(resolveAction As String)
        _iex.LogComment("This UI.Utils function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

    Overridable Sub ExitToLive()
        _iex.LogComment("This UI.Utils function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

    Overridable Sub ReloadDB()
        _iex.LogComment("This UI.Utils function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

    Overridable Sub EnterPin(pin As String)
        _iex.LogComment("This UI.Utils function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

    Overridable Sub Standby(isOn As Boolean)
        _iex.LogComment("This UI.Utils function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

    Overridable Sub ReturnToLiveViewing(Optional checkForVideo As Boolean = False)
        _iex.LogComment("This UI.Utils function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

    Overridable Sub ReturnToPlaybackViewing(Optional checkForVideo As Boolean = False)
        _iex.LogComment("This UI.Utils function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

    Overridable Sub SendIR(irKey As String, Optional waitAfterIR As Integer = 2000)
        _iex.LogComment("This UI.Utils function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

    Overridable Sub SendChannelAsIRSequence(channelNumber As String, Optional msBetweenSending As Integer = 500, Optional type As String = "Ignore")
        _iex.LogComment("This UI.Utils function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

    Overridable Sub InsertEventToCollection(eventKeyName As String, eventName As String, eventSource As String, startTime As String, _
                                            endTime As String, channel As String, duration As Long, originalDuration As Long, tDate As String, _
                                            frequency As String, recurrence As Integer, eventConvertedDate As String, Optional isModify As Boolean = False, Optional ByVal isSeries As Boolean = False)

        _iex.LogComment("This UI.Utils function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

    Overridable Function HighlightOption(ByVal stateInWhichOptionIsPresent As EpgState, ByVal dictionary As Dictionary(Of EnumEpgKeys, String)) As Boolean
        _iex.LogComment("This UI.Utils function isn't implemented under the general implementation. Please implement locally in project.")
        Return False
    End Function

    Overridable Function ParseMileStoneTimeFromLogArray(selectionItem As ArrayList) As List(Of Double)
        Return Nothing
    End Function

    Overridable Function ParseMileStoneTime(selectionItem As String) As Double
        Return 0
    End Function

    Overridable Sub StreamSync(epgTime As String)
        _iex.LogComment("This UI.Utils function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

    Overridable Function FormatEventTime(startTime As String, endTime As String) As String
        _iex.LogComment("This UI.Utils function isn't implemented under the general implementation. Please implement locally in project.")
        Return ""
    End Function

    Overridable Sub CreateandWritePerformanceResultXML(ByVal obj As Performance)
        _iex.LogComment("This UI.Utils function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub
#End Region

#Region "Verify Subs"

    Overridable Sub StbReady()
        _iex.LogComment("This UI.Utils function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

    Overridable Sub CheckScreenOffLine(secondsToCheck As Integer, secondsToPoll As Integer, screen As String, dvt As String)
        _iex.LogComment("This UI.Utils function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

    Overridable Function IsVideo(Optional ByVal coordinates As String = "") As Boolean
        _iex.LogComment("This UI.Utils function isn't implemented under the general implementation. Please implement locally in project.")
        Return False
    End Function

    Overridable Function IsStbCrash() As Boolean
        _iex.LogComment("This UI.Utils function isn't implemented under the general implementation. Please implement locally in project.")
        Return False
    End Function

    Overridable Sub VerifyLiveReached()
        _iex.LogComment("This UI.Utils function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

Overridable Sub VerifyHDDIndicator(ByVal isVisible As Boolean)
        _iex.LogComment("This UI.Utils function isn't implemented under the general implementation. Please implement locally in project.")
	 
End Sub
   
#End Region

#Region "RF"
    Overridable Sub SwitchRF()
        _iex.LogComment("This UI.Utils function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub
#End Region

#Region "Log Comments"

    Function LogCommentInfo(info As String) As Boolean
        Dim res As IEXGateway.IEXResult

        res = _iex.LogComment(info, False, False, False, CByte(10), "Blue")
        Try
            Tracer.Write(IEX.Utilities.Tracer.TraceLevel.INFO, info, "EA")
        Catch ex As Exception
        End Try

        Return res.CommandSucceeded

    End Function

    Function LogCommentImportant(info As String) As Boolean
        Dim res As IEXGateway.IEXResult

        res = _iex.LogComment(info, True, False, False, CByte(10), "Purple")

        Try
            Tracer.Write(IEX.Utilities.Tracer.TraceLevel.EVENT, Info, "EA")
        Catch ex As Exception
        End Try

        Return res.CommandSucceeded
    End Function

    Function LogCommentWarning(info As String) As Boolean
        Dim res As IEXGateway.IEXResult

        '_iex.ForceShowFailure()
        res = _iex.LogComment(info, True, False, False, CByte(10), "Orange")
        Try
            Tracer.Write(IEX.Utilities.Tracer.TraceLevel.WARN, Info, "EA")
        Catch ex As Exception
        End Try

        Return res.CommandSucceeded
    End Function

    Function LogCommentFail(info As String) As Boolean
        Dim res As IEXGateway.IEXResult

        res = _iex.LogComment(info, True, False, False, CByte(10), "Red")
        Try
            Tracer.Write(IEX.Utilities.Tracer.TraceLevel.ERROR, Info, "EA")
        Catch ex As Exception
        End Try

        Return res.CommandSucceeded
    End Function

    Function LogCommentBlack(info As String) As Boolean
        Dim res As IEXGateway.IEXResult

        res = _iex.LogComment(info, True, False, False, CByte(10), "Black")
        Try
            Tracer.Write(IEX.Utilities.Tracer.TraceLevel.INFO, info, "EA")
        Catch ex As Exception
        End Try

        Return res.CommandSucceeded
    End Function

    Function StartHideFailures(info As String) As Boolean
        Dim res As IEXGateway.IEXResult

        res = _iex.StartHideFailures(info, False, False, False, CByte(10), "Blue")
        Return res.CommandSucceeded
    End Function

#End Region

#Region "Hex2Dec"
    ''' <summary>
    ''' The function converts Hex string to Decimal.
    ''' </summary>
    ''' <param name="hex">Hex string to be converted</param>
    ''' <param name="dec">conversion of the given arguments</param>
    ''' <returns>Decimal representation  of the hex arguments</returns>
    ''' <remarks></remarks>
    Public Function Hex2Dec(hex As String, ByRef dec As String) As Boolean
        Try
            dec = Integer.Parse(hex, System.Globalization.NumberStyles.HexNumber).ToString()
        Catch ex As Exception
            dec = ""
            Return False
        End Try

        Return True

    End Function
#End Region

#Region "Dec2Hex"
    ''' <summary>
    ''' Convert Decimal number to Hex
    ''' </summary>
    ''' <param name="Dec">Decimal number to convert</param>
    ''' <param name="hex">ByRef param, Hex number to return</param>
    ''' <returns>IEXResult</returns>
    ''' <remarks></remarks>
    Public Function Dec2Hex(dec As String, ByRef hex As String) As Boolean
        Try
            hex = System.Convert.ToString(System.Convert.ToInt32(dec), 16)
        Catch ex As Exception
            hex = ""
            Return False
        End Try
        Return True
    End Function
#End Region

#Region "IncreasTimeByParam"
    ''' <summary>
    ''' Increases a given string represents time with the given factor
    ''' </summary>
    ''' <param name="baseTime">Should be in format "XX:YY"</param>
    ''' <param name="timePart">Can be either: "h" - for hours, or "n" - for minutes</param>
    ''' <param name="factor">baseTime + factor * field = newTime</param>
    ''' <param name="result">new time according to the calculation above</param>
    ''' <returns>result in the format "XX:YY"</returns>
    ''' <remarks></remarks>
    Public Function IncreaseTimeByParam(baseTime As String, timePart As String, _
                                       factor As Integer, ByRef result As String) As Boolean
        StartHideFailures("Increasing Time by Params: Field = " & timePart & " factor = " & factor.ToString())
        If String.IsNullOrEmpty(baseTime) Then
            _iex.ForceHideFailure()
            _UI.Utils.LogCommentFail("baseTime is empty !!!")
            Return False
        End If
        baseTime = baseTime.Replace(" ", Nothing)
        If Not System.Text.RegularExpressions.Regex.IsMatch(baseTime, "^([0-1]?\d|2[0-3]):[0-5]\d$") Then
            _iex.ForceHideFailure()
            _UI.Utils.LogCommentFail("baseTime in wrong format!!!")
            Return False
        End If
        If String.IsNullOrEmpty(timePart) Then
            _iex.ForceHideFailure()
            _UI.Utils.LogCommentFail("timePart is empty !!!")
            Return False
        End If

        If factor <= 0 Then
            _iex.ForceHideFailure()
            _UI.Utils.LogCommentFail("factor is less than 0 !!!")
            Return False
        End If
        Select Case timePart   ' Must be a primitive data type
            Case "h", "n"
            Case Else
                _iex.ForceHideFailure()
                _UI.Utils.LogCommentFail("Ilegal arguments !!!")
                Return False
        End Select


        Dim t2 As System.TimeSpan = New System.TimeSpan(Integer.Parse(baseTime.Substring(0, 2)), Integer.Parse(baseTime.Substring(3, 2)), 0)
        Dim t3 As System.TimeSpan
        Select Case timePart   ' Must be a primitive data type
            Case "h"
                t3 = New System.TimeSpan(factor, 0, 0)
            Case "n"
                t3 = New System.TimeSpan(0, factor, 0)
            Case Else
                t3 = New System.TimeSpan(0, 0, 0)
        End Select
        Dim MyHours As String = t2.Add(t3).Hours
        Dim MyMinutes As String = t2.Add(t3).Minutes

        If String.IsNullOrEmpty(MyHours) Or String.IsNullOrEmpty(MyMinutes) Then
            _iex.ForceHideFailure()
            _UI.Utils.LogCommentFail("Encountered a failure while trying to convert time")
            Return False
        Else
            If MyHours.Length = 1 Then MyHours = "0" & MyHours
            If MyMinutes.Length = 1 Then MyMinutes = "0" & MyMinutes
            result = New String(MyHours & ":" & MyMinutes)
        End If
        _iex.ForceHideFailure()
        Return True
    End Function
#End Region

#Region "Mobile"
    Overridable Sub InitAgent()
        _iex.LogComment("This UI.Utils function isn't implemented under the general implementation. Please implement locally in project.")
    End Sub

#End Region

End Class
