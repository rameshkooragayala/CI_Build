Namespace EAImplementation

    ''' <summary>
    '''   Get Current Event Time From Start In Seconds
    ''' </summary>
    Public Class GetCurrentEventTimeFromStart
        Inherits IEX.ElementaryActions.BaseCommand
        Private _manager As IEX.ElementaryActions.Functionality.Manager
        Private EPG As IEX.ElementaryActions.EPG.SF.UI
        Private _PayPerView As Boolean

        '''<param name="PayPerView"></param>
        ''' <param name="pManager">Manager</param>
        ''' <remarks>
        ''' Possible Error Codes:
        ''' <para>337 - ParseEventTimeFailure</para> 
        ''' <para>350 - ParsingFailure</para> 
        ''' </remarks>
        Sub New(ByVal PayPerView As Boolean, ByVal pManager As IEX.ElementaryActions.Functionality.Manager)
            _manager = pManager
            EPG = Me._manager.UI
            _PayPerView = PayPerView
        End Sub

        Protected Overrides Sub Execute()
            Dim TimePassed As Integer = -1

            If Not _PayPerView Then
                EPG.Banner.Navigate()
            End If

            EPG.Banner.GetEventTimePassed(TimePassed)

            SetReturnValues(New Object() {TimePassed})
        End Sub

    End Class

End Namespace
