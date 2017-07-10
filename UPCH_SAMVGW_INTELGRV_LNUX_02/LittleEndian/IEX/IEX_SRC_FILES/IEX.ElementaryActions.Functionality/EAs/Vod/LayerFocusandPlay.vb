Imports System.Threading.Tasks
Imports FailuresHandler
Imports System.IO
Imports System.Text
Imports System.Xml
Namespace EAImplementation

    Public Class LayerFocusandPlay

        Inherits IEX.ElementaryActions.BaseCommand

        Private EPG As IEX.ElementaryActions.EPG.SF.UI
        Private _manager As IEX.ElementaryActions.Functionality.Manager


        Sub New(ByVal pManager As IEX.ElementaryActions.Functionality.Manager)
            Me._manager = pManager
            EPG = Me._manager.UI

        End Sub

        Protected Overrides Sub Execute()
            Try
                EPG.Utils.LogCommentInfo("Call is in Layer Focus and Play API ")

                Dim highlightedAsset As String = ""
                'Collecting Highlighted Asset Details
                EPG.Utils.GetEpgInfo("focusedEvtName", highlightedAsset)

                EPG.Utils.SendIR("SELECT", 2000)

                'Calling Play Module API
                Try
                    EPG.Vod.PlayModule()
                Catch ex As EAException
                    ExceptionUtils.ThrowEx(New EAException(ExitCodes.VerificationFailure, "Failed to Get Play Option continuously on Three Different Asset"))
                End Try

                'Waiting for 3 minute in Play state of Asset
                _iex.Wait(180)

                highlightedAsset = ""
                'Checking for other functionality like Audio and Subtitle
                EPG.Utils.LogCommentInfo("Checking Audio and Subtitle")

                'Try
                '  EPG.Utils.SendIR("SELECT", 2000)
                '' _iex.Wait(5)

                ' EPG.Utils.EPG_Milestones_Navigate("A//V SETTINGS")

                'check for Menu Elements present 
                '  EPG.Utils.GetEpgInfo("title", highlightedAsset)
                ' EPG.Utils.LogCommentInfo("Highlighted title is : " & highlightedAsset)

                '  If highlightedAsset = "AUDIO" Then
                ' check for Audio update
                ' EPG.Utils.SendIR("SELECT", 2000)
                ' EPG.Utils.SendIR("SELECT", 2000)
                ' End If
                'Catch ex As Exception
                'EPG.Utils.LogCommentInfo("Unable to change Audio Stream of VOD asset")
                'End Try

                ' EPG.Utils.SendIR("RETOUR", 2000)
                'Setting Trick Mode Speeds in Both forward and reverse direction all speeds ,once operation is done stopping the playback for Asset
                EPG.Utils.LogCommentInfo("Setting Forward , Reverse Trick Modes and Stopping the Play Back Once operation is finished")

                'Calling Trick Module
                Try
                    EPG.Vod.TrickSpeed()
                Catch ex As EAException
                    ExceptionUtils.ThrowEx(New EAException(ExitCodes.VerificationFailure, "Failed to Set Trick Speeds while Playing Asset"))
                End Try

                'Stopping the PlayBack of Asset
                EPG.Utils.SendIR("STOP", 2000)
            Catch ex As EAException
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.VerificationFailure, ex.Message))
            End Try
        End Sub
    End Class
End Namespace


