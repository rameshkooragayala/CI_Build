Imports FailuresHandler

Public Class ManualRecording
    Inherits IEX.ElementaryActions.EPG.SF.COGECO.ManualRecording

    Dim _UI As IEX.ElementaryActions.EPG.SF.CDIGITAL.UI

    Sub New(ByVal _pIex As IEXGateway.IEX, ByVal UI As IEX.ElementaryActions.EPG.SF.UI)
        MyBase.New(_pIex, UI)
        _UI = UI
    End Sub


    ''' <summary>
    '''   Navigating To CHANNELS On Manual Recording Menu
    ''' </summary>
    ''' <remarks></remarks>
    Public Overrides Sub NavigateToChannel(Optional ByVal isModify As Boolean = False)
        _UI.Utils.StartHideFailures("Navigating To CHANNEL On Manual Recording Menu")
        Try
            _UI.Utils.EPG_Milestones_SelectMenuItem("CHANNEL")
        Finally
            _iex.ForceHideFailure()
        End Try
    End Sub
    ''' <summary>
    '''   Navigating To DATE On Manual Recording Menu
    ''' </summary>
    ''' <remarks></remarks>
    Public Overrides Sub NavigateToDate(Optional ByVal isModify As Boolean = False)
        _UI.Utils.StartHideFailures("Navigating To DATE On Manual Recording Menu")

        Try
            _UI.Utils.EPG_Milestones_SelectMenuItem("DATE")
            _UI.Utils.SendIR("SELECT")
        Finally
            _iex.ForceHideFailure()
        End Try

    End Sub

    ''' <summary>
    '''   Navigating To START TIME On Manual Recording Menu
    ''' </summary>
    ''' <remarks></remarks>
    Public Overrides Sub NavigateToStartTime(Optional ByVal isModify As Boolean = False)
        _UI.Utils.StartHideFailures("Navigating To START TIME On Manual Recording Menu")

        Try
            _UI.Utils.EPG_Milestones_SelectMenuItem("START TIME")
            _UI.Utils.SendIR("SELECT")
        Finally
            _iex.ForceHideFailure()
        End Try

    End Sub

    ''' <summary>
    '''   Navigating To END TIME On Manual Recording Menu
    ''' </summary>
    ''' <remarks></remarks>
    Public Overrides Sub NavigateToEndTime(Optional ByVal isModify As Boolean = False)
        _UI.Utils.StartHideFailures("Navigating To END TIME On Manual Recording Menu")

        Try
            _UI.Utils.EPG_Milestones_SelectMenuItem("END TIME")
            _UI.Utils.SendIR("SELECT")
        Finally
            _iex.ForceHideFailure()
        End Try

    End Sub

    ''' <summary>
    '''   Navigating To FREQUENCY On Manual Recording Menu
    ''' </summary>
    ''' <remarks></remarks>
    Public Overrides Sub NavigateToFrequency(Optional ByVal isModify As Boolean = False)
        _UI.Utils.StartHideFailures("Navigating To FREQUENCY On Manual Recording Menu")

        Try
            _UI.Utils.EPG_Milestones_SelectMenuItem("FREQUENCY")
            _UI.Utils.SendIR("SELECT")
        Finally
            _iex.ForceHideFailure()
        End Try
    End Sub


    ''' <summary>
    '''   Setting Start Time In Start Time Field
    ''' </summary>
    ''' <param name="StartTime">Requested Start Time To Set</param>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>300 - NavigationFailure</para> 
    ''' <para>301 - DictionaryFailure</para> 
    ''' <para>302 - EmptyEpgInfoFailure</para> 
    ''' <para>304 - IRVerificationFailure</para> 
    ''' <para>322 - VerificationFailure</para> 
    ''' <para>328 - INIFailure</para> 
    ''' </remarks>
    Public Overrides Sub SetStartTime(ByVal StartTime As String)
        Dim selectedTime As String = ""
        _UI.Utils.StartHideFailures("Setting Start Time To " + StartTime)
        Try
            StartTime = Convert.ToDateTime(StartTime).ToString("HHmm")
            SetTime(StartTime)
            _UI.Utils.EPG_Milestones_SelectMenuItem("START TIME")
        Finally
            _iex.ForceHideFailure()
        End Try

    End Sub


    ''' <summary>
    '''   Setting End Time In End Time Field
    ''' </summary>
    ''' <param name="EndTime">Requested End Time To Set</param>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>300 - NavigationFailure</para> 
    ''' <para>301 - DictionaryFailure</para> 
    ''' <para>302 - EmptyEpgInfoFailure</para> 
    ''' <para>304 - IRVerificationFailure</para> 
    ''' <para>322 - VerificationFailure</para> 
    ''' <para>328 - INIFailure</para> 
    ''' </remarks>
    Public Overrides Sub SetEndTime(ByVal EndTime As String)
        Dim selectedTime As String = ""
        _UI.Utils.StartHideFailures("Setting End Time To " + EndTime)

        Try
            EndTime = Convert.ToDateTime(EndTime).ToString("HHmm")
            SetTime(EndTime)
            _UI.Utils.EPG_Milestones_SelectMenuItem("END TIME")
        Finally
            _iex.ForceHideFailure()
        End Try

    End Sub

    Public Sub SetTime(ByVal Time As String)
        Dim i As Integer
        Dim selectedNum As String = ""
        Dim numToSet As String = ""
        _UI.Utils.StartHideFailures("Setting Time To " + Time.ToString)
        Try
            For i = 1 To Time.Length
                numToSet = Mid(Time, i, 1)
                _UI.Utils.GetEpgInfo("title", selectedNum)
                While selectedNum <> numToSet
                    _UI.Utils.ClearEPGInfo()
                    If CInt(numToSet) > CInt(selectedNum) Then
                        _UI.Utils.SendIR("SELECT_RIGHT")
                    Else
                        _UI.Utils.SendIR("SELECT_LEFT")
                    End If
                    _UI.Utils.GetEpgInfo("title", selectedNum)
                End While
                _UI.Utils.SendIR("SELECT")
            Next

            'Changes as per CQ # 2227707 - Not a UI Bug New Implementation for TBR 

            _UI.Utils.SendIR("SELECT", 1000)

        Finally
            _iex.ForceHideFailure()
        End Try
    End Sub

    ''' <summary>
    '''   Setting Date In Date List
    ''' </summary>
    ''' <param name="tDate">Requested Date To Set</param>
    ''' <param name="DefaultValue">If True Only For Logging Purposes Writes Default Value</param>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>301 - DictionaryFailure</para> 
    ''' <para>302 - EmptyEpgInfoFailure</para> 
    ''' <para>304 - IRVerificationFailure</para> 
    ''' <para>322 - VerificationFailure</para> 
    ''' </remarks>
    Public Overrides Sub SetDate(ByVal tDate As String, ByVal DefaultValue As Boolean)
        Dim EpgText As String = ""

        If DefaultValue Then
            _UI.Utils.StartHideFailures("Setting Date To Default Value")

            Try
                EpgText = _UI.Utils.GetValueFromDictionary("DIC_DAY_TODAY")

                Dim title As String = ""

                _UI.Utils.GetEpgInfo("title", title)

                If Not title.Contains(EpgText) Then
                    ExceptionUtils.ThrowEx(New EAException(ExitCodes.VerificationFailure, "Failed To Verify Date Is TODAY"))
                End If

                _UI.Utils.SendIR("SELECT")

                Exit Sub

            Finally
                _iex.ForceHideFailure()
            End Try
        End If

        _UI.Utils.StartHideFailures("Setting Date To " + tDate.ToUpper.ToString)

        Try
            _UI.Menu.SetManualRecordingDate(tDate.ToUpper)

            Dim ReturnedValues As New Dictionary(Of String, String)
            Dim data As String = ""

            _UI.Utils.SendIR("SELECT", 4000)
        Finally
            _iex.ForceHideFailure()
        End Try
    End Sub


    ''' <summary>
    '''   Booking Manual Recording After All Parameters Were Entered
    ''' </summary>
    ''' <param name="IsCurrent">Optional Parameter Default = False : If True Verifies Current Booking Milestones Else Future Booking Milestones</param>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>304 - IRVerificationFailure</para> 
    ''' <para>328 - INIFailure</para> 
    ''' <para>339 - RecordEventFailure</para> 
    ''' </remarks>
    Public Overrides Sub SaveAndEnd(Optional ByVal IsCurrent As Boolean = False, Optional ByVal isModify As Boolean = False)
        Dim Milestones As String = ""
        Dim ActualLines As New ArrayList
        Dim Msg As String = ""

        _UI.Utils.StartHideFailures("Saving Manual Record")

        Try

            If IsCurrent Then
                Msg = "ManualCurrentRecord"
                Milestones = _UI.Utils.GetValueFromMilestones("ManualCurrentRecord")

            Else
                Msg = "ManualFutureRecord"
                Milestones = _UI.Utils.GetValueFromMilestones("ManualFutureRecord")
            End If

            _UI.Utils.BeginWaitForDebugMessages(Milestones, 10)

            _UI.Utils.EPG_Milestones_SelectMenuItem("CONFIRM RECORDING")

            _UI.Utils.SendIR("SELECT")

            If Not _UI.Utils.EndWaitForDebugMessages(Milestones, ActualLines) Then
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.RecordEventFailure, "Failed To Verify " + Msg + " Milestones : " + Milestones))
            End If
        Finally
            _iex.ForceHideFailure()
        End Try
    End Sub
End Class
