Imports FailuresHandler

Namespace EAImplementation
    ''' <summary>
    '''   Sets Series Link Channel Configuration
    ''' </summary>
    Public Class SetSeriesLinkChannels
        Inherits IEX.ElementaryActions.BaseCommand

        Private EPG As IEX.ElementaryActions.EPG.SF.UI
        Private _manager As IEX.ElementaryActions.Functionality.Manager
        Private _ChannelSelection As EnumChannelSelection

        ''' <param name="ChannelSelection">EnumChannelSelection : From_Single_Channel or From_All_Channels</param>
        ''' <param name="pManager">Manager</param>
        ''' <remarks>
        ''' Possible Error Codes:
        ''' <para>301 - DictionaryFailure</para> 
        ''' <para>302 - EmptyEpgInfoFailure</para> 
        ''' <para>304 - IRVerificationFailure</para> 
        ''' <para>314 - SetSettingsFailure</para> 
        ''' </remarks>
        Sub New(ByVal ChannelSelection As EnumChannelSelection, ByVal pManager As IEX.ElementaryActions.Functionality.Manager)
            Me._ChannelSelection = ChannelSelection
            Me._manager = pManager
            EPG = Me._manager.UI
        End Sub

        Protected Overrides Sub Execute()
            Dim Setting As String = ""

            Select Case _ChannelSelection
                Case EnumChannelSelection.From_Single_Channel
                    Setting = EPG.Settings.GetSettingsValueFromDictionary("DIC_SERIES_VAL_SINGLE_CHANNEL", "")
                Case EnumChannelSelection.From_All_Channels
                    Setting = EPG.Settings.GetSettingsValueFromDictionary("DIC_SERIES_VAL_ALL_CHANNEL", "")
            End Select

            EPG.Settings.NavigateToSeriesRecording()

            EPG.Settings.SetSetting(Setting, "RECORDINGS")

        End Sub

    End Class

End Namespace