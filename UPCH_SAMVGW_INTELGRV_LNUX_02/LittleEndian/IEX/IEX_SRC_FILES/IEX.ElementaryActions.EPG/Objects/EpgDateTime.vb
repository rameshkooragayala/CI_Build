Imports System.DateTime

Public Class EpgDateTime
    Dim _iex As IEXGateway.IEX
    Dim res As IEXGateway.IEXResult
    Dim returnValue As String

    Sub New(ByVal pIex As IEXGateway.IEX)
        _iex = pIex
    End Sub

    Overridable Function IsDateTime(stringTime As String, ByRef rDateTime As String) As Boolean
        Try
            'deleting non relevant additions in the end of the time string
            If stringTime.IndexOf(vbLf) > 0 Then
                stringTime = stringTime.Remove(0, stringTime.IndexOf(vbLf) - 1)
            End If
            stringTime = stringTime.Trim
            stringTime = stringTime.Replace(".", "").Replace(" ", "")
            Do While stringTime.Length > 5
                stringTime = Mid(stringTime, 1, stringTime.Length - 1)
            Loop
            If stringTime.IndexOf(":") < 0 Then
                stringTime = stringTime.Insert(2, ":")
            End If

            _iex.LogComment("The time is: " & stringTime, False, False, False, CByte(8), "GREEN")
            If stringTime.Length < 4 Then
                Return False
            End If
            Try
                rDateTime = stringTime

                Return True 'found the time
            Catch ex As Exception
                Return False
            End Try

        Catch ex As Exception
            Return False
        End Try
    End Function

    Overridable Function Subtract(newDate As DateTime, oldDate As DateTime) As Integer

        Dim iSubtract As Integer

        Dim Newer = New DateTime(DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day, newDate.TimeOfDay.Hours, newDate.TimeOfDay.Minutes, 0)

        oldDate = New DateTime(DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day, oldDate.TimeOfDay.Hours, oldDate.TimeOfDay.Minutes, 0)

        iSubtract = Newer.Subtract(oldDate).TotalMinutes()

        'if midnight based on 4 hours buffer asuming there are no events from previous of 8:00pm that are ending after 4:00am

        If (newDate.TimeOfDay.TotalMinutes() >= New Date(2000, 1, 1, 0, 0, 0).TimeOfDay.TotalMinutes) And (newDate.TimeOfDay.TotalMinutes < New Date(2000, 1, 1, 4, 0, 0).TimeOfDay.TotalMinutes) Then

            If (oldDate.TimeOfDay.TotalMinutes <= New Date(2000, 1, 1, 23, 59, 59).TimeOfDay.TotalMinutes) And (oldDate.TimeOfDay.TotalMinutes > New Date(2000, 1, 1, 20, 0, 0).TimeOfDay.TotalMinutes) Then

                iSubtract = 24 * 60 + iSubtract

            End If

        End If

        If (oldDate.TimeOfDay.TotalMinutes >= New Date(2000, 1, 1, 0, 0, 0).TimeOfDay.TotalMinutes) And (oldDate.TimeOfDay.TotalMinutes < New Date(2000, 1, 1, 4, 0, 0).TimeOfDay.TotalMinutes) Then

            If (newDate.TimeOfDay.TotalMinutes <= New Date(2000, 1, 1, 23, 59, 59).TimeOfDay.TotalMinutes) And (newDate.TimeOfDay.TotalMinutes > New Date(2000, 1, 1, 20, 0, 0).TimeOfDay.TotalMinutes) Then

                iSubtract = iSubtract - (24 * 60)

            End If

        End If


        Return iSubtract

    End Function

    Overridable Function SubtractInSec(newDate As DateTime, oldDate As DateTime) As Integer

        Dim iSubtract As Integer
        Dim Newer = New DateTime(Today.Year, Today.Month, Today.Day, newDate.TimeOfDay.Hours, newDate.TimeOfDay.Minutes, newDate.TimeOfDay.Seconds)

        oldDate = New DateTime(Today.Year, Today.Month, Today.Day, oldDate.TimeOfDay.Hours, oldDate.TimeOfDay.Minutes, oldDate.TimeOfDay.Seconds)
        iSubtract = Newer.Subtract(oldDate).TotalSeconds()

        'if midnight based on 4 hours buffer asuming there are no events from previous of 8:00pm that are ending after 4:00am

        If (newDate.TimeOfDay.TotalSeconds() >= New Date(2000, 1, 1, 0, 0, 0).TimeOfDay.TotalSeconds) And (newDate.TimeOfDay.TotalSeconds < New Date(2000, 1, 1, 4, 0, 0).TimeOfDay.TotalSeconds) Then
            If (oldDate.TimeOfDay.TotalSeconds <= New Date(2000, 1, 1, 23, 59, 59).TimeOfDay.TotalSeconds) And (oldDate.TimeOfDay.TotalSeconds > New Date(2000, 1, 1, 20, 0, 0).TimeOfDay.TotalSeconds) Then
                iSubtract = 24 * 60 * 60 + iSubtract
            End If
        End If

        If (oldDate.TimeOfDay.TotalSeconds >= New Date(2000, 1, 1, 0, 0, 0).TimeOfDay.TotalSeconds) And (oldDate.TimeOfDay.TotalSeconds < New Date(2000, 1, 1, 4, 0, 0).TimeOfDay.TotalSeconds) Then
            If (newDate.TimeOfDay.TotalSeconds <= New Date(2000, 1, 1, 23, 59, 59).TimeOfDay.TotalSeconds) And (newDate.TimeOfDay.TotalSeconds > New Date(2000, 1, 1, 20, 0, 0).TimeOfDay.TotalSeconds) Then
                iSubtract = iSubtract - (24 * 60 * 60)
            End If
        End If

        Return iSubtract

    End Function

    Overridable Function GetTimeFrom(template As String, column As String, ByRef theTime As String) As Boolean
        Dim StringTime As String = ""
        Dim Counter As Integer = 0
        Do
            Counter = Counter + 1
            res = _iex.IAL.GetItem(template, "", column, StringTime)
            'deleting non relevant additions in the end of the time string
            If Me.IsDateTime(StringTime, theTime) Then Return True
            If Counter = 4 Then
                Return False
            End If
        Loop
    End Function

    Overridable Function FixTime(ByRef timeValue As String) As Boolean

        If String.IsNullOrEmpty(timeValue) Then Return False
        timeValue = timeValue.Trim().Replace(" ", "").Replace(".", ":").Replace("9m", "pm")
        If timeValue.Length < 5 Then Return False
        If Not timeValue.Contains(":") Then
            timeValue = Strings.Left(timeValue, timeValue.Length - 4) & ":" & Strings.Right(timeValue, 4)
        End If
        Return True

    End Function

    Public Overridable Function Subtract(newTime As String, oldTime As String) As Integer

        Dim intNew As Integer
        Dim intOld As Integer
        Dim newHour As Integer
        Dim oldHour As Integer
        Dim newMin As Integer
        Dim oldMin As Integer
        Dim intHour As Integer
        Dim intMin As Integer

        'cleaning sgins in time string
        newTime = CleanTimeString(newTime)
        oldTime = CleanTimeString(oldTime)

        'changing all time templates to 24 hours time template
        newTime = SetTimeTemplate(newTime)
        oldTime = SetTimeTemplate(oldTime)

        'Convert to int
        intNew = Int(newTime)
        intOld = Int(oldTime)

        'seperate time and hour from integers values
        newHour = Math.Floor(intNew / 100)
        newMin = intNew - (newHour * 100)
        oldHour = Math.Floor(intOld / 100)
        oldMin = intOld - (oldHour * 100)

        'in case we are between 20:00 and 5:00  also new value is smaller time is handelled in positive values
        If intNew < 500 And intOld > 2000 Then
            intHour = 24 - oldHour + newHour
            If oldMin > newMin Then
                intHour = intHour - 1
                intMin = 60 - (oldMin - newMin)
            Else
                intMin = newMin - oldMin
            End If
            'otherwise will get negetive value for new time that is smaller then compared time
        Else
            intHour = newHour - oldHour
            If oldMin > newMin Then
                intHour = intHour - 1
                intMin = 60 - (oldMin - newMin)
            Else
                intMin = newMin - oldMin
            End If
        End If

        Return (intHour * 60) + intMin
    End Function

    Public Function CleanTimeString(timeString As String) As String
        timeString = timeString.Replace(" ", "").Replace(".", "").Replace(":", "").Replace(";", "").Replace("-", "").Replace("_", "").Replace("|", "")
        Return timeString
    End Function

    Public Function SetTimeTemplate(timeString As String) As String
        Dim isPM As Boolean = False
        Dim isAMPMTemplate As Boolean = False
        Dim length = timeString.Length
        Dim returnValue
        Dim hours As String = ""
        Dim minutes As String = ""

        'check if time is am\pm
        Try
            If timeString.ToLower.Substring(timeString.Length - 3).Contains("pm") Or timeString.ToLower.Substring(timeString.Length - 2).Contains("p") Then
                isPM = True
                isAMPMTemplate = True
            Else
                If timeString.ToLower.Substring(timeString.Length - 3).Contains("am") Or timeString.ToLower.Substring(timeString.Length - 2).Contains("a") Then
                    isAMPMTemplate = True
                End If
            End If

            'remove am\pm from string
            returnValue = timeString.ToLower.Replace("a", "").Replace("m", "").Replace("p", "")

            If returnValue.Length = 3 Then
                returnValue = "0" & timeString
            End If

            hours = returnValue.Substring(0, 2)
            minutes = returnValue.Substring(2, 2)

            'check for 12am to become 00:00
            If isAMPMTemplate = True And isPM = False And Int(hours) = 12 Then
                hours = "00"
            End If

            If isPM = True And (Not hours = "12") Then
                hours = Int(hours) + 12
            End If

            If Int(hours) > 23 Or Int(minutes) > 59 Then Throw New Exception("String is not a known recognized time: " & timeString)
        Catch ex As Exception

        End Try

        Return hours & minutes
    End Function

End Class


