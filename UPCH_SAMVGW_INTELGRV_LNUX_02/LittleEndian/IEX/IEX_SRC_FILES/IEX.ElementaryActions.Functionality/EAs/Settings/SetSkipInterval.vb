Imports FailuresHandler

Namespace EAImplementation
    ''' <summary>
    '''   Sets Skip Interval On Stb Settings
    ''' </summary>
    Public Class SetSkipInterval
        Inherits IEX.ElementaryActions.BaseCommand

        Private EPG As IEX.ElementaryActions.EPG.SF.UI
        Private _manager As IEX.ElementaryActions.Functionality.Manager
        Private _SkipIntervalInSec As EnumVideoSkip
        Private _IsForward As Boolean

        ''' <param name="SkipIntervalInSec">Optional Parameter Default = BOOKMARKMODE : Skip Interval Can Be : BOOKMARKMODE,10,30,60,300,600</param>
        ''' <param name="IsForward">If True Set Skip Forward Else Skip Backwards Interval</param>
        ''' <param name="pManager">Manager</param>
        ''' <remarks>
        ''' Possible Error Codes:
        ''' <para>301 - DictionaryFailure</para> 
        ''' <para>302 - EmptyEpgInfoFailure</para> 
        ''' <para>304 - IRVerificationFailure</para> 
        ''' <para>314 - SetSettingsFailure</para> 
        ''' <para>334 - VideoNotPresent</para> 
        ''' <para>349 - ReturnToLiveFailure</para> 
        ''' </remarks>
        Sub New(ByVal SkipIntervalInSec As EnumVideoSkip, ByVal IsForward As Boolean, ByVal pManager As IEX.ElementaryActions.Functionality.Manager)
            Me._SkipIntervalInSec = SkipIntervalInSec
            Me._IsForward = IsForward
            Me._manager = pManager
            EPG = Me._manager.UI
        End Sub

        Protected Overrides Sub Execute()
            Dim Interval As String = ""
            Dim EnumValue As String = CType(_SkipIntervalInSec, EnumVideoSkip)

            If Me._SkipIntervalInSec = EnumVideoSkip.BOOKMARKMODE Then
                Interval = EPG.Settings.GetSettingsValueFromDictionary("DIC_BOOKMARK_MODE", "")
            Else
                If _IsForward Then
                    Interval = EPG.Settings.GetSettingsValueFromDictionary("DIC_SETTINGS_SKIP_FWD_VALUES", EnumValue)
                Else
                    Interval = EPG.Settings.GetSettingsValueFromDictionary("DIC_SETTINGS_SKIP_BACK_VALUES", EnumValue)
                End If
            End If

            EPG.Settings.NavigateToSkipInterval(Me._IsForward)

            EPG.Settings.SetSetting(Interval, "")

            EPG.Utils.ReturnToLiveViewing()

        End Sub

    End Class

End Namespace