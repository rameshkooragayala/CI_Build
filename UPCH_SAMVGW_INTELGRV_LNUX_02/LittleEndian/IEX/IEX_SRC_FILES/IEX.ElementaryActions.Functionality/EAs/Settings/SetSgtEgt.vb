Imports FailuresHandler

Namespace EAImplementation
    ''' <summary>
    '''   Sets Start/End Guard Time On Stb Settings
    ''' </summary>
    Public Class SetSgtEgt
        Inherits IEX.ElementaryActions.BaseCommand

        Private EPG As IEX.ElementaryActions.EPG.SF.UI
        Private _manager As IEX.ElementaryActions.Functionality.Manager
        Private _IsStartToBeSet As Boolean
        Private _ValueToBeSet As String

        ''' <param name="isStartToBeSet">If True Sets SGT Else Sets EGT</param>
        ''' <param name="valueToBeSet">The value in String to be set as Guard Time</param>
        ''' <param name="pManager">Manager</param>
        ''' <remarks>
        ''' Possible Error Codes:
        ''' <para>301 - DictionaryFailure</para> 
        ''' <para>302 - EmptyEpgInfoFailure</para> 
        ''' <para>304 - IRVerificationFailure</para> 
        ''' <para>314 - SetSettingsFailure</para> 
        ''' <para>322 - VerificationFailure</para> 
        ''' <para>332 - NoValidParameters</para> 	
        ''' <para>334 - VideoNotPresent</para> 
        ''' <para>349 - ReturnToLiveFailure</para> 
        ''' </remarks>
        Sub New(ByVal isStartToBeSet As Boolean, ByVal valueToBeSet As String, ByVal pManager As IEX.ElementaryActions.Functionality.Manager)
            Me._IsStartToBeSet = isStartToBeSet
            Me._ValueToBeSet = valueToBeSet
            Me._manager = pManager
            EPG = Me._manager.UI
        End Sub

        Protected Overrides Sub Execute()

            If Me._IsStartToBeSet Then
                EPG.Settings.NavigateToStartGuardTime()
                _iex.LogComment("Setting Start Guard Time to - " + _ValueToBeSet)
            Else
                EPG.Settings.NavigateToEndGuardTime()
                _iex.LogComment("Setting End Guard Time to - " + _ValueToBeSet)
            End If

            EPG.Settings.SetSetting(_ValueToBeSet, "")

            EPG.Utils.ReturnToLiveViewing()

        End Sub

    End Class

End Namespace