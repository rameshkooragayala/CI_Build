Imports FailuresHandler

Namespace EAImplementation

    ''' <summary>
    '''   Browses In Guide Left Right Or Up Down Directions
    ''' </summary>
    Public Class BrowseGuideFuture
        Inherits IEX.ElementaryActions.BaseCommand

        Private EPG As IEX.ElementaryActions.EPG.SF.UI
        Private _manager As IEX.ElementaryActions.Functionality.Manager
        Private _MoveRight As Boolean
        Private _NumOfRightLeftPresses As Integer
        Private _MoveChannelUp As Boolean
        Private _NumOfUpDownPresses As Integer

        ''' <param name="MoveRight">If True Moves Right Else Moves Left</param>
        ''' <param name="NumOfRightLeftPresses">Optional Parameter Default = 1 : Number Of Moves Right Or Left</param>
        ''' <param name="MoveChannelUp">Optional Parameter Default = True : If True Moves Up Else Moves Down</param>
        ''' <param name="NumOfUpDownPresses">Optional Parameter Default = 0 : Number Of Moves Up OR Down</param>
        ''' <param name="pManager">Manager</param>
        ''' <remarks>
        ''' Possible Error Codes:
        ''' <para>300 - NavigationFailure</para> 
        ''' <para>351 - SurfingFailure</para> 
        ''' </remarks>
        Sub New(ByVal MoveRight As Boolean, ByVal NumOfRightLeftPresses As Integer, ByVal MoveChannelUp As Boolean, ByVal NumOfUpDownPresses As Integer, ByVal pManager As IEX.ElementaryActions.Functionality.Manager)
            Me._MoveRight = MoveRight
            Me._NumOfRightLeftPresses = NumOfRightLeftPresses
            Me._MoveChannelUp = MoveChannelUp
            Me._NumOfUpDownPresses = NumOfUpDownPresses
            Me._manager = pManager
            EPG = Me._manager.UI
        End Sub

        Protected Overrides Sub Execute()
            Dim res As New IEXGateway.IEXResult
            Dim NextEvent As Boolean = False
            Dim Direction As String = ""

            If _NumOfUpDownPresses <= 0 And _NumOfRightLeftPresses <= 0 Then
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.NoValidParameters, "NumOfRightLeftPresses And _NumOfUpDownPresses Can't Be Both 0 Or Less"))
            End If

            If Not EPG.Guide.IsGuide Then
                EPG.Guide.Navigate()
            End If

            If Me._MoveRight Then
                NextEvent = True
            Else
                NextEvent = False
            End If

            Dim StartTime As String = ""
            Dim CheckedTime As String = ""
            For i As Integer = 1 To Me._NumOfRightLeftPresses

                If NextEvent Then
                    EPG.Utils.StartHideFailures("Trying To Move To Next Event")
                    Try
                        EPG.Guide.NextEvent(1)
                    Catch ex As Exception
                        EPG.Guide.NextEvent(1)
                    End Try
                    _iex.ForceHideFailure()
                Else
                    EPG.Utils.StartHideFailures("Trying To Move To Previous Event")
                    Try
                        EPG.Guide.PreviousEvent(1)
                    Catch ex As Exception
                        EPG.Guide.PreviousEvent(1)
                    End Try
                    _iex.ForceHideFailure()
                End If
            Next

            For i As Integer = 1 To Me._NumOfUpDownPresses
                EPG.Utils.StartHideFailures("Trying To Move " + IIf(Me._MoveChannelUp, "UP", "DOWN").ToString)
                Try
                    EPG.Guide.MoveChannelUpDown(Me._MoveChannelUp)
                Catch ex As Exception
                    EPG.Guide.MoveChannelUpDown(Me._MoveChannelUp)
                End Try
                _iex.ForceHideFailure()
            Next

        End Sub

    End Class

End Namespace
