Imports FailuresHandler

Namespace EAImplementation

    ''' <summary>
    '''   Enters The PIN
    ''' </summary>
    Public Class EnterPIN
        Inherits IEX.ElementaryActions.BaseCommand

        Private EPG As IEX.ElementaryActions.EPG.SF.UI
        Private _manager As IEX.ElementaryActions.Functionality.Manager
        Private _PIN As String
        Private _NextState As String

        ''' <param name="PIN">PIN Requested To Enter</param>
        ''' <param name="NextState">The Next State After Entering PIN</param>
        ''' <param name="pManager">Manager</param>
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
        Sub New(ByVal PIN As String, ByVal NextState As String, ByVal pManager As IEX.ElementaryActions.Functionality.Manager)
            _PIN = PIN
            _NextState = NextState
            Me._manager = pManager
            EPG = Me._manager.UI
        End Sub

        Protected Overrides Sub Execute()

            If Me._NextState = "" Then
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.NoValidParameters, "NextState Can't Be Empty Please Check"))
            End If

            EPG.Utils.EnterPin(_PIN)


            If Not EPG.Utils.VerifyState(_NextState, 25) Then
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.VerificationFailure, "Failed To Verify State Is " + _NextState + " After Entering PIN"))
            End If
        End Sub

    End Class

End Namespace