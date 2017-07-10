Imports FailuresHandler

Namespace EAImplementation
    ''' <summary>
    '''   Sets Parental Control Age Limit Settings
    ''' </summary>
    Public Class SetParentalControlAgeLimit
        Inherits IEX.ElementaryActions.BaseCommand

        Private EPG As IEX.ElementaryActions.EPG.SF.UI
        Private _manager As IEX.ElementaryActions.Functionality.Manager
        Private _Age As EnumParentalControlAge

        ''' <param name="Age">Age Can Be : 3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,16 18,G,PG,AP 12,Off</param>
        ''' <param name="pManager">Manager</param>
        ''' <remarks>
        ''' Possible Error Codes:
        ''' <para>300 - NavigationFailure</para> 
        ''' <para>301 - DictionaryFailure</para> 
        ''' <para>302 - EmptyEpgInfoFailure</para> 
        ''' <para>304 - IRVerificationFailure</para> 
        ''' <para>314 - SetSettingsFailure</para> 
        ''' <para>322 - VerificationFailure</para> 
        ''' <para>328 - INIFailure</para> 
        ''' <para>332 - NoValidParameters</para> 
        ''' </remarks>
        Sub New(ByVal Age As EnumParentalControlAge, ByVal pManager As IEX.ElementaryActions.Functionality.Manager)
            Me._Age = Age
            Me._manager = pManager
            EPG = Me._manager.UI
        End Sub

        Protected Overrides Sub Execute()
            Dim AgeString As String = ""
            Dim Res As IEXGateway._IEXResult
            Dim pin As String = ""

            Try
                pin = EPG.Utils.GetValueFromTestIni("TEST PARAMS", "PIN")
            Catch ex As Exception
                EPG.Utils.LogCommentWarning("Pin is not defined in the Test ini, taking Default Pin from Environment ini")
                pin = ""
            End Try

            If pin = "" Then
                pin = EPG.Utils.GetValueFromEnvironment("DefaultPIN")
            End If
            EPG.Settings.NavigateToParentalControlAgeLimit()

            Res = Me._manager.EnterPIN(pin, "LOCK PROGRAMMES BY AGE RATING")
            If Not Res.CommandSucceeded Then
                ExceptionUtils.ThrowEx(New EAException(Res.FailureCode, Res.FailureReason))
            End If

            AgeString = [Enum].Parse(_Age.GetType, _Age)

            If _Age = EnumParentalControlAge._16_18 Then
                AgeString = "16 18"
            ElseIf _Age = EnumParentalControlAge._15_18 Then
                AgeString = "15 18"
            ElseIf _Age = EnumParentalControlAge.Off Then
                AgeString = "-"
            ElseIf _Age = EnumParentalControlAge.AP_12 Then
                AgeString = "AP 12"
            ElseIf _Age = EnumParentalControlAge.Unlimited Then
                AgeString = "Unlimited"
            ElseIf _Age = EnumParentalControlAge.No_parental_control Then
                AgeString = "No parental control"
            ElseIf _Age = EnumParentalControlAge.UNLOCK_ALL Then
                AgeString = "UNLOCK ALL"
            ElseIf _Age = EnumParentalControlAge.C Then
                AgeString = "C"
            ElseIf _Age = EnumParentalControlAge.C8 Then
                AgeString = "C8"
            ElseIf _Age = EnumParentalControlAge.G Then
                AgeString = "G"
            ElseIf _Age = EnumParentalControlAge.PG Then
                AgeString = "PG"
            ElseIf _Age = EnumParentalControlAge.Adult Then
                AgeString = "Adult"
            ElseIf _Age = EnumParentalControlAge.FSK_6 Then
                AgeString = "FSK 6"
            ElseIf _Age = EnumParentalControlAge.FSK_12 Then
                AgeString = "FSK 12"
            ElseIf _Age = EnumParentalControlAge.FSK_16 Then
                AgeString = "FSK 16"
            ElseIf _Age = EnumParentalControlAge.FSK_18 Then
                AgeString = "FSK 18"
            End If

            If _manager.Project.IsEPGLikeCogeco Then
                If _Age = EnumParentalControlAge._14 Then
                    AgeString = "14+"
                ElseIf _Age = EnumParentalControlAge._18 Then
                    AgeString = "18+"
                End If
            End If
        
            EPG.Settings.SetSetting(AgeString, "")

        End Sub

    End Class

End Namespace