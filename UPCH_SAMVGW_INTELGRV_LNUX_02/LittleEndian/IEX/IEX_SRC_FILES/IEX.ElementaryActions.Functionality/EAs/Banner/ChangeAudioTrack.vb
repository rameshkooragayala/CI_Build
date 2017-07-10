Imports FailuresHandler

Namespace EAImplementation

    ''' <summary>
    '''   Setting Audio Track On Action Bar
    ''' </summary>
    Public Class ChangeAudioTrack
        Inherits IEX.ElementaryActions.BaseCommand

        Private EPG As IEX.ElementaryActions.EPG.SF.UI
        Private _manager As IEX.ElementaryActions.Functionality.Manager

        Private _Language As EnumLanguage
        Private _Type As EnumAudioType
        Private _VerifyAudio As Boolean

        ''' <param name="Language">Requested Language</param>
        ''' <param name="Type">Requested Type - STEREO Or DOLBY DIGITAL</param>
        ''' <param name="VerifyAudio">Optional Parameter Default = False,If True Verifies Audio</param>
        ''' <param name="pManager">Manager</param>
        ''' <remarks>
        ''' Possible Error Codes:
        ''' <para>301 - DictionaryFailure</para> 
        ''' <para>332 - NoValidParameters</para> 
        ''' <para>336 - AudioNotPresent</para> 
        ''' </remarks>
        Sub New(ByVal Language As EnumLanguage, ByVal Type As EnumAudioType, ByVal VerifyAudio As Boolean, ByVal pManager As IEX.ElementaryActions.Functionality.Manager)
            Me._Language = Language
            Me._Type = Type
            Me._VerifyAudio = VerifyAudio

            Me._manager = pManager
            EPG = Me._manager.UI
        End Sub

        Protected Overrides Sub Execute()
            Dim Language As String
            Dim AudioString As String = ""
            Dim res As IEXGateway._IEXResult

            Language = EPG.Utils.TranslateLanguage(_Language.ToString)

            Select Case _Type
                Case EnumAudioType.STEREO
                    AudioString = Language + "-" + "STEREO"
                Case EnumAudioType.DOLBY_DIGITAL
                    AudioString = Language + "-" + "DOLBY DIGITAL"
                Case Else
                    ExceptionUtils.ThrowEx(New EAException(ExitCodes.NoValidParameters, "Type " + _Type.ToString + " Is Not Valid"))
            End Select

            EPG.Banner.SetAudioTrack(AudioString)

            If _VerifyAudio Then
                res = Me._manager.CheckForAudio(IsPresent:=True, Timeout:=5)
                If Not res.CommandSucceeded Then
                    ExceptionUtils.ThrowEx(New EAException(res.FailureCode, res.FailureReason))
                End If
            End If

        End Sub

    End Class

End Namespace