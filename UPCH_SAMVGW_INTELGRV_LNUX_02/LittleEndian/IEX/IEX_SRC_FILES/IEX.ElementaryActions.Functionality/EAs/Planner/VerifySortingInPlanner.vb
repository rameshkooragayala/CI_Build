Imports FailuresHandler

Namespace EAImplementation

    ''' <summary>
    '''   Verify Sorting On Archive Screen
    ''' </summary>
    Public Class VerifySortingInPlanner
        Inherits IEX.ElementaryActions.BaseCommand

        Private EPG As IEX.ElementaryActions.EPG.SF.UI
        Private _manager As IEX.ElementaryActions.Functionality.Manager
        Private _SortBy As EnumSortBy

        ''' <param name="SortBy">Enum Can Be SortBy Time Or A-Z</param>
        ''' <param name="pManager">Manager</param>
        ''' <remarks>
        ''' Possible Error Codes:
        ''' <para>300 - NavigationFailure</para> 
        ''' <para>301 - DictionaryFailure</para> 
        ''' <para>304 - IRVerificationFailure</para> 
        ''' <para>306 - GetEventInfoFailure</para> 
        ''' <para>328 - INIFailure</para> 
        ''' <para>321 - VerifyChannelAttributeFailure</para> 
        ''' <para>332 - NoValidParameters</para> 	
        ''' <para>338 - EventNotExistsFailure</para> 
        ''' <para>346 - FindEventFailure</para> 
        ''' <para>347 - SelectEventFailure</para> 
        ''' </remarks>
        Sub New(ByVal SortBy As EnumSortBy, ByVal pManager As IEX.ElementaryActions.Functionality.Manager)
            Me._SortBy = SortBy
            Me._manager = pManager
            EPG = Me._manager.UI
        End Sub

        Protected Overrides Sub Execute()
            Select Case _SortBy
                Case EnumSortBy.ByA_Z
                    EPG.FutureRecordings.VerifySorting(ByTime:=False)
                Case EnumSortBy.ByTime
                    EPG.FutureRecordings.VerifySorting(ByTime:=True)
            End Select
        End Sub

    End Class


End Namespace