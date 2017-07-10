Imports FailuresHandler
Imports System.Globalization

Public Class Menu
    Inherits IEX.ElementaryActions.EPG.SF.Menu

    Dim _UI As UI

    Sub New(ByVal _pIex As IEXGateway.IEX, ByVal UI As IEX.ElementaryActions.EPG.SF.COGECO.UI)
        MyBase.New(_pIex, UI)
        _UI = UI
    End Sub

    ''' <summary>
    '''  Sets Manual Recording Channel On Channels List
    ''' </summary>
    ''' <param name="Channel">Requested Channel</param>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>300 - NavigationFailure</para> 
    ''' <para>302 - EmptyEpgInfoFailure</para> 
    ''' <para>304 - IRVerificationFailure</para> 
    ''' <para>328 - INIFailure</para> 
    ''' </remarks>
    Overrides Sub SetManualRecordingChannel(ByVal Channel As String)

        _UI.Utils.StartHideFailures("Setting Manual Recording Channel To -> " + Channel.ToString)

        Try
            Dim CurrentChannel As String = ""
            Dim CheckedChannel As String = "Empty"
            Dim FirstChannel As String = ""
            Dim SameItemTimes As Integer = 3

            _UI.Utils.GetEpgInfo("chname", FirstChannel)

            If FirstChannel = Channel Then
                _iex.Wait(2)
                Exit Sub
            End If

            Dim StopAtWarning As Boolean

            StopAtWarning = CBool(_UI.Utils.GetValueFromEnvironment("WarningAsError"))

            Do Until FirstChannel = CheckedChannel And (SameItemTimes = 2 Or SameItemTimes = 0)

                _UI.Utils.GetEpgInfo("chname", CurrentChannel)

                _UI.Utils.SendIR("SELECT_DOWN", 1000)

                _UI.Utils.GetEpgInfo("chname", CheckedChannel)


                If CurrentChannel = CheckedChannel Then
                    If StopAtWarning Then
                        ExceptionUtils.ThrowEx(New IEXWarnning(500, "MISSED_IR_FAILURE", "MISSED IR SELECT_DOWN ACTION BAR ACTION SAME AS BEFORE IR"))
                    Else
                        If SameItemTimes = 3 Then
                            SameItemTimes = -1
                        End If
                        SameItemTimes += 1
                        _UI.Utils.LogCommentWarning("WARNING : MISSED IR SELECT_DOWN CHANNEL SAME AS BEFORE IR")
                    End If
                Else
                    SameItemTimes = 0
                End If

                If CheckedChannel = Channel Then
                    Exit Sub
                End If
            Loop

            ExceptionUtils.ThrowEx(New EAException(ExitCodes.NavigationFailure, "Failed To Set Manual Recording Channel To : " + Channel))

        Finally
            _iex.ForceHideFailure()
        End Try


    End Sub

    ''' <summary>
    '''   Sets Manual Recording Date On Date List
    ''' </summary>
    ''' <param name="tDate">Requested Date</param>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>300 - NavigationFailure</para> 
    ''' <para>302 - EmptyEpgInfoFailure</para> 
    ''' <para>304 - IRVerificationFailure</para> 
    ''' <para>350 - ParsingFailure</para> 
    ''' </remarks>
    Public Overrides Sub SetManualRecordingDate(ByVal tDate As String)
        _UI.Utils.StartHideFailures("Setting Manual Recording Date To -> " + tDate.ToString)

        Try
            Dim CurrentDate As String = ""
            Dim CheckedDate As String = "Empty"
            Dim FirstDate As String = ""
            Dim SameItemTimes As Integer = 3

            _UI.Utils.GetEpgInfo("title", FirstDate)

            FirstDate = ParseDateToEPGFormat(FirstDate)

            If FirstDate.ToUpper = tDate.ToUpper Then
                _iex.Wait(2)
                Exit Sub
            End If

            Dim StopAtWarning As Boolean

            StopAtWarning = CBool(_UI.Utils.GetValueFromEnvironment("WarningAsError"))

            Do Until FirstDate.ToUpper = CheckedDate.ToUpper And (SameItemTimes = 2 Or SameItemTimes = 0)

                _UI.Utils.GetEpgInfo("title", CurrentDate)

                CurrentDate = ParseDateToEPGFormat(CurrentDate)

                _UI.Utils.SendIR("SELECT_DOWN")

                _UI.Utils.GetEpgInfo("title", CheckedDate)

                CheckedDate = ParseDateToEPGFormat(CheckedDate)

                If CurrentDate.ToUpper = CheckedDate.ToUpper Then
                    If StopAtWarning Then
                        ExceptionUtils.ThrowEx(New IEXWarnning(500, "MISSED_IR_FAILURE", "MISSED IR SELECT_DOWN ACTION BAR ACTION SAME AS BEFORE IR"))
                    Else
                        If SameItemTimes = 3 Then
                            SameItemTimes = -1
                        End If
                        SameItemTimes += 1
                        _UI.Utils.LogCommentWarning("WARNING : MISSED IR SELECT_DOWN DATE SAME AS BEFORE IR")
                    End If
                Else
                    SameItemTimes = 0
                End If

                If CheckedDate.ToUpper = tDate.ToUpper Then
                    Exit Sub
                End If
            Loop

            ExceptionUtils.ThrowEx(New EAException(ExitCodes.NavigationFailure, "Failed To Set Manual Recording Date To : " + tDate))

        Finally
            _iex.ForceHideFailure()
        End Try

    End Sub

    ''' <summary>
    ''' Parse Date to EPG Format
    ''' </summary>
    ''' <param name="dateToBeParsed">The date to be parsed in String</param>
    ''' <returns>The parsed date in String</returns>
    ''' <remarks></remarks>
    Private Function ParseDateToEPGFormat(ByVal dateToBeParsed As String) As String
        Dim DateFormatForEventHighlight As String = ""
        Try
            Try
                DateFormatForEventHighlight = _UI.Utils.GetValueFromProject("MANUAL_RECORDING", "DATE_FORMAT_FOR_EVENT_HIGHLIGHT")
            Catch ex As Exception
                _UI.Utils.LogCommentWarning("DATE_FORMAT_FOR_EVENT_HIGHLIGHT value not present in Project.ini. Taking EPG Date Format as Event Highlight format instead")
                DateFormatForEventHighlight = _UI.Utils.GetEpgDateFormat
            End Try
            Dim year As String = dateToBeParsed.Substring(dateToBeParsed.Length - 4)
            ' Parsing the date to strip off the GMT part
            dateToBeParsed = dateToBeParsed.Substring(0, dateToBeParsed.IndexOf(":") - 3) + " " + year

            ' Date format should have the year also to avoid parsing errors in case of recorded streams
            dateToBeParsed = DateTime.ParseExact(dateToBeParsed, DateFormatForEventHighlight + " yyyy", CultureInfo.InvariantCulture).ToString(_UI.Utils.GetEpgDateFormat)
        Catch ex As Exception
            ExceptionUtils.ThrowEx(New EAException(ExitCodes.ParsingFailure, "Failed To Parse Date From String " + dateToBeParsed))
        End Try

        Return dateToBeParsed

    End Function

End Class
