Imports FailuresHandler

Namespace EAImplementation

    ''' <summary>
    '''   Surfs Channel Up Or Down And Then To Next Event In ChannelBar
    ''' </summary>
    Public Class ChannelBarSurfFuture
        Inherits IEX.ElementaryActions.BaseCommand

        Private EPG As IEX.ElementaryActions.EPG.SF.UI
        Private _manager As IEX.ElementaryActions.Functionality.Manager
        Private _IsNext As Boolean
        Private _NumberOfPressesUpDown As Integer


        ''' <param name="IsNext">If True Surfs To Next Channel Else To Previous</param>
        ''' <param name="NumberOfPressesUpDown">Optional Parameter Default = 1 : Number Of Surfs</param>
        ''' <param name="pManager">Manager</param>
        ''' <remarks>
        ''' Possible Error Codes:
        ''' <para>328 - INIFailure</para> 
        ''' <para>347 - SelectEventFailure</para> 
        ''' <para>351 - SurfingFailure</para> 
        ''' </remarks>
        Sub New(ByVal IsNext As Boolean, ByVal NumberOfPressesUpDown As Integer, ByVal pManager As IEX.ElementaryActions.Functionality.Manager)
            Me._IsNext = IsNext
            Me._NumberOfPressesUpDown = NumberOfPressesUpDown
            Me._manager = pManager
            EPG = Me._manager.UI
        End Sub

        Protected Overrides Sub Execute()
            Dim res As New IEXGateway.IEXResult

            If _NumberOfPressesUpDown <= 0 Then
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.NoValidParameters, "NumberOfPressesUpDown Can't Be Both 0 Or Less"))
            End If

            EPG.ChannelBar.Navigate()
            EPG.ChannelBar.NextEvent(False)

            For i = 1 To Me._NumberOfPressesUpDown
                If Me._IsNext Then
                    EPG.ChannelBar.SurfChannelDown("None")
                Else
                    EPG.ChannelBar.SurfChannelUp("None")
                End If
            Next

        End Sub
    End Class

End Namespace