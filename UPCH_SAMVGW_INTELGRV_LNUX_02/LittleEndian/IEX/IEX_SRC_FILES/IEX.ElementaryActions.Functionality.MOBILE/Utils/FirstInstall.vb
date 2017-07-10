Imports FailuresHandler

Namespace EAImplementation
    ''' <summary>
    '''   Verify Event On Planner Screen
    ''' </summary>
    Public Class FirstInstall
        Inherits IEX.ElementaryActions.BaseCommand

        Private EPG As IEX.ElementaryActions.EPG.SF.MOBILE.UI
        Private _manager As IEX.ElementaryActions.Functionality.Manager

        ''' <param name="pManager">Manager</param>
        ''' <remarks>
        ''' Possible Error Codes:
        ''' <para>300 - NavigationFailure</para> 
        ''' <para>301 - DictionaryFailure</para> 
        ''' <para>304 - IRVerificationFailure</para> 
        ''' <para>306 - GetEventInfoFailure</para> 
        ''' <para>321 - VerifyChannelAttributeFailure</para> 
        ''' <para>328 - INIFailure</para> 
        ''' <para>332 - NoValidParameters</para> 	
        ''' <para>338 - EventNotExistsFailure</para> 
        ''' <para>346 - FindEventFailure</para> 
        ''' <para>347 - SelectEventFailure</para> 
        ''' </remarks>
        Sub New(ByVal pManager As IEX.ElementaryActions.Functionality.Manager)
            Me._manager = pManager
            EPG = Me._manager.UI
        End Sub

        Protected Overrides Sub Execute()
            EPG.Settings.HandleWelcomeScreen()
            EPG.Settings.HandleUserNameScreen()
            EPG.Settings.HandleSetTopBoxSelectionScreen()
            EPG.Settings.HandleSetProfileScreen()
        End Sub

    End Class

End Namespace