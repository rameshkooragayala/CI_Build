Namespace EAImplementation

    ''' <summary>
    '''   Get Current Event Left Time In Seconds To End Time Of The Event
    ''' </summary>
    Public Class GetCurrentEventLeftTime
        Inherits IEX.ElementaryActions.BaseCommand
        Private _manager As IEX.ElementaryActions.Functionality.Manager
        Private EPG As IEX.ElementaryActions.EPG.SF.UI
        Private _IsPayPerView As Boolean

        ''' <param name="IsPayPerView">True Only For Pay Per View Event, False Otherwise</param>
        ''' <param name="pManager">Manager</param>
        ''' <remarks>
        ''' Possible Error Codes:
        ''' <para>337 - ParseEventTimeFailure</para> 
        ''' <para>350 - ParsingFailure</para> 
        ''' </remarks>
        Sub New(ByVal IsPayPerView As Boolean, ByVal pManager As IEX.ElementaryActions.Functionality.Manager)
            _manager = pManager
            EPG = Me._manager.UI
            _IsPayPerView = IsPayPerView
        End Sub

        Protected Overrides Sub Execute()
            Dim TimeLeft As Integer = -1

            If Not _IsPayPerView Then
                EPG.Banner.Navigate()
            End If

            EPG.Banner.GetEventTimeLeft(TimeLeft)

            SetReturnValues(New Object() {TimeLeft})
        End Sub

    End Class

End Namespace
