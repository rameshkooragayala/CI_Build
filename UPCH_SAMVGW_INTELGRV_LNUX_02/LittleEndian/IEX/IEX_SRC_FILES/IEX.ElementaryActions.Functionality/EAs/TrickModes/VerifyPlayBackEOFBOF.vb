Imports FailuresHandler

Namespace EAImplementation

    ''' <summary>
    '''  Verifies EOF/BOF Of Playback Event That Is Currently Playing 
    ''' </summary>
    Public Class VerifyPlaybackEOFBOF
        Inherits IEX.ElementaryActions.BaseCommand
        Private EPG As IEX.ElementaryActions.EPG.SF.UI
        Private _manager As IEX.ElementaryActions.Functionality.Manager
        Private _DurationInSec As Long
        Private _Speed As Double
        Private _Eof As Boolean

        ''' <param name="DurationInSec">Duration Of The Event Or Played Duration</param>
        ''' <param name="Speed">Speed Of Trickmode</param>
        ''' <param name="Eof">Optional Parameter Default = False : If True EOF Else BOF</param>
        ''' <param name="pManager">Manager</param>
        ''' <remarks>
        ''' Possible Error Codes:
        ''' <para>320 - VerifyEofBofFailure</para> 
        ''' <para>322 - VerificationFailure</para> 
        ''' <para>328 - INIFailure</para> 
        ''' </remarks>
        Sub New(ByVal DurationInSec As Long, ByVal Speed As Double, ByVal Eof As Boolean, ByVal pManager As IEX.ElementaryActions.Functionality.Manager)
            Me._manager = pManager
            EPG = Me._manager.UI
            Me._DurationInSec = DurationInSec
            Me._Speed = Speed
            Me._Eof = Eof
        End Sub

        Protected Overrides Sub Execute()
            Dim res As New IEXGateway.IEXResult

            EPG.TrickModes.VerifyEofBof(Me._DurationInSec, Me._Speed, False, False, Me._Eof)

        End Sub

    End Class

End Namespace