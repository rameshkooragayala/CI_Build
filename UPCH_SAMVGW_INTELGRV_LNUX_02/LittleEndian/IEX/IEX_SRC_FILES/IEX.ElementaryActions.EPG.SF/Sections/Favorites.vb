Imports FailuresHandler

Public Class Favorites
    Inherits IEX.ElementaryActions.EPG.Favorites

    Dim _UI As UI
    Private _Utils As EPG.SF.Utils

    Sub New(ByVal _pIex As IEXGateway.IEX, ByVal UI As IEX.ElementaryActions.EPG.SF.UI)
        MyBase.New(_pIex, UI)
        _UI = UI
        _Utils = _UI.Utils
    End Sub

    ''' <summary>
    '''   Navigating To Favorites Settings
    ''' </summary>
    ''' <remarks></remarks>
    Public Overrides Sub Navigate()

        _Utils.StartHideFailures("Navigating To Favorites Settings")

        Try
            _Utils.EPG_Milestones_NavigateByName("STATE:SET FAVOURITES BY SETTINGS")
        Finally
            _iex.ForceHideFailure()
        End Try

    End Sub

    ''' <summary>
 '''  Remove all the channel from favoritelist
    ''' </summary>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>302 - EmptyEpgInfoFailure</para>
    ''' <para>304 - IRVerificationFailure</para> 
    ''' <para>314 - SetSettingsFailure</para>
    ''' </remarks>
    Public Overrides Sub RemoveAllFavourites()
        RemoveAllFavChannel()

    End Sub
    '''   Trying To Change Channel Favorite Status And Then Verify If Set Otherwise Change It Again And Verify Done
    ''' </summary>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>302 - EmptyEpgInfoFailure</para>
    ''' <para>304 - IRVerificationFailure</para> 
    ''' <para>314 - SetSettingsFailure</para>
    ''' </remarks>
    Public Overrides Sub SetChannelAsFavorite()
        SetChannel(Favorite)
    End Sub

    ''' <summary>
    '''   Trying To Change Channel Favorite Status And Then Verify If Unset Otherwise Change It Again And Verify Done
    ''' </summary>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>302 - EmptyEpgInfoFailure</para>
    ''' <para>304 - IRVerificationFailure</para> 
    ''' <para>314 - SetSettingsFailure</para>
    ''' </remarks>
    Public Overrides Sub UnsetChannelFromFavorite()
        SetChannel(NonFavorite)
    End Sub

    ''' <summary>
    '''   Trying To Set A List Of Channel Names To Favorites
    ''' </summary>
    ''' <remarks>
    ''' Possible Error Codes: 
    ''' <para>300 - NavigationFailure</para>
    ''' <para>302 - EmptyEpgInfoFailure</para>
    ''' <para>304 - IRVerificationFailure</para>
    ''' <para>314 - SetSettingsFailure</para>
    ''' </remarks>
    Public Overrides Sub SetChannelListAsFavorites(ByVal listChannelName As List(Of String))
        SetChannelList(listChannelName, SetAsFavorites:=True)
    End Sub

    ''' <summary>
    '''   Trying To Unset A List Of Channel Names From Favorites
    ''' </summary>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>300 - NavigationFailure</para>
    ''' <para>302 - EmptyEpgInfoFailure</para>
    ''' <para>304 - IRVerificationFailure</para>
    ''' <para>314 - SetSettingsFailure</para>
    ''' </remarks>
    Public Overrides Sub UnsetChannelListFromFavorites(ByVal listChannelName As List(Of String))
        SetChannelList(listChannelName, SetAsFavorites:=False)
    End Sub

    ''' <summary>
    '''   Trying To Set A List Of Channel Numbers To Favorites
    ''' </summary>
    ''' <remarks>
    ''' Possible Error Codes: 
    ''' <para>300 - NavigationFailure</para>
    ''' <para>302 - EmptyEpgInfoFailure</para>
    ''' <para>304 - IRVerificationFailure</para>
    ''' <para>314 - SetSettingsFailure</para>
    ''' </remarks>
    Public Overrides Sub SetChannelListAsFavorites(ByVal listChannelNum As List(Of Integer))
        SetChannelList(listChannelNum, SetAsFavorites:=True)
    End Sub

    ''' <summary>
    '''   Trying To Unset A List Of Channel Names From Favorites
    ''' </summary>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>300 - NavigationFailure</para>
    ''' <para>302 - EmptyEpgInfoFailure</para>
    ''' <para>304 - IRVerificationFailure</para>
    ''' <para>314 - SetSettingsFailure</para>
    ''' </remarks>
    Public Overrides Sub UnsetChannelListFromFavorites(ByVal listChannelNum As List(Of Integer))
        SetChannelList(listChannelNum, SetAsFavorites:=False)
    End Sub

    ''' <summary>
    '''   Confirming Current List Of Favorites
    ''' </summary>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>304 - IRVerificationFailure</para>
    ''' </remarks>
    Public Overrides Sub ConfirmFavoritesListNumber(ByVal ListChannelNumAsString As List(Of String), ByVal setAsFavorites As Boolean)
        Dim Value As String = ""
        Dim EpgText As String = ""
        Dim ListFavouriteChannels As String = ""
        ' Me._ChannelNumList = ChannelNumList

        _Utils.StartHideFailures("Navigating To Confirm Current List Of Favorites")

        Try

            _Utils.SendIR("SELECT_LEFT")

            EpgText = _Utils.GetValueFromDictionary("DIC_SETTINGS_CONFIRM_LIST")

            _Utils.GetEpgInfo("title", Value)

            If Value <> EpgText Then
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.NavigationFailure, "Failed To Navigate To " + EpgText))
            End If
        Finally
            _iex.ForceHideFailure()
        End Try

        _Utils.SendIR("SELECT", 500)
	
        _Utils.GetEpgInfo("title", EpgText)

         If EpgText.Trim() = "CONFIRM" Then
            EpgText = _Utils.GetValueFromDictionary("DIC_CONFIRM")
            _Utils.GetEpgInfo("title", Value)
            If Value <> EpgText Then
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.NavigationFailure, "Failed To Navigate To " + EpgText))
            End If
            _Utils.SendIR("SELECT", 500)
        End If

        Try
            _Utils.GetEpgInfo("FavouriteChannelNumber", ListFavouriteChannels)
        Catch ex As Exception

            _Utils.LogCommentInfo("Confirmed no channels in favourite list")

        End Try

        If ListFavouriteChannels.Trim() <> String.Empty Then

            If setAsFavorites Then
                For i As Integer = 0 To ListChannelNumAsString.Count - 1
                    If ListFavouriteChannels.Trim().Contains(ListChannelNumAsString.Item(i).Trim()) Then
                    Else
                        ExceptionUtils.ThrowEx(New EAException(ExitCodes.NavigationFailure, "Confimation list does not contain the service" + ListChannelNumAsString.Item(i)))
                    End If
                Next
            Else
                For i As Integer = 0 To ListChannelNumAsString.Count - 1
                    If ListFavouriteChannels.Trim().Contains(ListChannelNumAsString.Item(i)) Then
                        ExceptionUtils.ThrowEx(New EAException(ExitCodes.NavigationFailure, "Confimation list contain the service" + ListChannelNumAsString.Item(i)))
                    Else
                    End If
                Next
            End If

        End If

    End Sub

    ''' <summary>
    '''   Confirming Current List Of Favorites
    ''' </summary>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>304 - IRVerificationFailure</para>
    ''' </remarks>
    Public Overrides Sub ConfirmFavoritesListName(ByVal ListChannelName As List(Of String), ByVal setAsFavorites As Boolean)
        Dim Value As String = ""
        Dim EpgText As String = ""
        Dim ListFavouriteChannels As String = ""

        _Utils.StartHideFailures("Navigating To Confirm Current List Of Favorites")

        Try

            _Utils.SendIR("SELECT_LEFT")

            EpgText = _Utils.GetValueFromDictionary("DIC_SETTINGS_CONFIRM_LIST")

            _Utils.GetEpgInfo("title", Value)

            If Value <> EpgText Then
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.NavigationFailure, "Failed To Navigate To " + EpgText))
            End If
        Finally
            _iex.ForceHideFailure()
        End Try

        _Utils.SendIR("SELECT", 500)

        _Utils.GetEpgInfo("title", EpgText)

        If EpgText.Trim() = "CONFIRM" Then
            EpgText = _Utils.GetValueFromDictionary("DIC_CONFIRM")
            _Utils.GetEpgInfo("title", Value)
            If Value <> EpgText Then
                ExceptionUtils.ThrowEx(New EAException(ExitCodes.NavigationFailure, "Failed To Navigate To " + EpgText))
            End If
            _Utils.SendIR("SELECT", 500)
        End If

        Try
            _Utils.GetEpgInfo("FavouriteChannelName", ListFavouriteChannels)
        Catch ex As Exception
            _Utils.LogCommentInfo("Confirmed no channels in favourite list")

        End Try

        If ListFavouriteChannels.Trim() <> String.Empty Then
            If setAsFavorites Then
                For i As Integer = 0 To ListChannelName.Count - 1
                    If ListFavouriteChannels.Trim().Contains(ListChannelName.Item(i).Trim()) Then
                    Else
                        ExceptionUtils.ThrowEx(New EAException(ExitCodes.NavigationFailure, "Confimation list does not contain the service" + ListChannelName.Item(i)))
                    End If
                Next
            Else
                For i As Integer = 0 To ListChannelName.Count - 1
                    If ListFavouriteChannels.Trim().Contains(ListChannelName.Item(i)) Then
                        ExceptionUtils.ThrowEx(New EAException(ExitCodes.NavigationFailure, "Confimation list contain the service" + ListChannelName.Item(i)))
                    Else
                    End If
                Next
            End If
        End If

    End Sub

    ''' <summary>
    '''   Set Favorite from Action bar/Guide
    ''' </summary>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>304 - IRVerificationFailure</para>
    ''' </remarks>
    Public Overrides Sub SetFavoriteFromAction()
        Dim favouriteKey As String = ""


        _Utils.GetEpgInfo("IsFavourite", favouriteKey)
        'If already favourite 
        If favouriteKey <> "True" Then
            _Utils.EPG_Milestones_SelectMenuItem("MAKE FAVOURITE")
            _Utils.SendIR("SELECT", 500)
        Else
            _Utils.LogCommentInfo("Service is already favourite")
        End If

        'Verify for favorite is set
        _Utils.GetEpgInfo("IsFavourite", favouriteKey)

        If favouriteKey <> "True" Then
            ExceptionUtils.ThrowEx(New EAException(ExitCodes.EmptyEpgInfoFailure, "Failed to set as favourite"))
        Else
            _Utils.LogCommentInfo("Service set as favourite Successfully")
        End If
    End Sub
    ''' <summary>
    '''   UnSet Favorite from Action bar/Guide
    ''' </summary>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>304 - IRVerificationFailure</para>
    ''' </remarks>
    Public Overrides Sub UnsetFavoriteFromAction()
        Dim favouriteKey As String = ""

        _Utils.GetEpgInfo("isFavourite", favouriteKey)

        If favouriteKey <> "False" Then
            _Utils.EPG_Milestones_SelectMenuItem("REMOVE FROM FAVOURITES")
            _Utils.SendIR("SELECT", 500)
        Else
            _Utils.LogCommentInfo("Service is not a favourite")
        End If

        'Verify favorite is unset 
        _Utils.GetEpgInfo("isFavourite", favouriteKey)

        If favouriteKey <> "False" Then
            ExceptionUtils.ThrowEx(New EAException(ExitCodes.EmptyEpgInfoFailure, "Failed to Unset from favourite"))
        Else
            _Utils.LogCommentInfo("Service Unset from favourite Successfully")
        End If
    End Sub

    ''' <summary>
    '''   Setting A List Of Channel Names To Favorite/Non-Favorite
    ''' </summary>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>300 - NavigationFailure</para>
    ''' <para>302 - EmptyEpgInfoFailure</para>
    ''' <para>304 - IRVerificationFailure</para>
    ''' <para>314 - SetSettingsFailure</para>
    ''' </remarks>

    Overrides Sub SetChannelList(ByVal _listChannelName As List(Of String), ByVal SetAsFavorites As Boolean)

        Dim ChannelName As String = ""
        Dim listChannelName As New List(Of String)
        Dim msgHideFailures As String = If(SetAsFavorites, "Trying To Set Channel As Favorite", "Trying To Unset Channel From Being Favorite")

        _Utils.StartHideFailures(msgHideFailures)

        Try
            For Each channel As String In _listChannelName
                listChannelName.Add(channel)
            Next

            _Utils.GetEpgInfo("chname", ChannelName)

            Dim initialChannelName As String = ChannelName
            'if initial channel is favorite then loop twice (since in IPC EPG, (bug?) a channel set as favorite is automatically inserted into the same list causing it to appear twice)
            'so the real initial occurence of the initial channel will be surely reached
            'Dim numberOfIterations As Integer

            'If IsFavorite() Then
            '    numberOfIterations = 2
            'Else
            '    numberOfIterations = 1
            'End If

            Dim allChannelsWereHandled As Boolean = False

            Dim previousChannelName As String

            'For i As Integer = 1 To numberOfIterations
            'Loop over the channels in menu
            Do
                If listChannelName.Count = 0 Then
                    Exit Do
                End If
                'If listChannelName.Contains(ChannelName) Then 'if it IS one of the channels to handle (set/unset favorite)
                If ChannelName = listChannelName.Item(0) Then 'if it IS one of the channels to handle (set/unset favorite)
                    'handle it (set/unset favorite)
                    _Utils.LogCommentInfo("Trying To Handle Channel (ChannelName=" & ChannelName & ")")
                    If SetAsFavorites Then
                        SetChannelAsFavorite()
                    Else
                        UnsetChannelFromFavorite()
                    End If
                    'remove from input list
                    listChannelName.Remove(ChannelName) 'TODO may ameliorate by removing at index got from FindIndex instead of Contains
                    If listChannelName.Count = 0 Then 'if all items are done, exit
                        allChannelsWereHandled = True
                        Exit Do
                    End If
                    initialChannelName = ChannelName
                End If
                previousChannelName = ChannelName

                _Utils.SendIR("SELECT_DOWN") 'Goto next channel

                _Utils.GetEpgInfo("chname", ChannelName) 'Get the selected channel name

                If String.Equals(ChannelName, previousChannelName) Then
                    ExceptionUtils.ThrowEx(New EAException(ExitCodes.NavigationFailure, "Identical Channel Name Before and After SELECT_DOWN (channelName=" & ChannelName & ")"))
                End If

            Loop Until String.Equals(ChannelName, initialChannelName) 'While not all channels in menu already visited (i.e. while not back to the 1st one)
            'Next

            If Not allChannelsWereHandled Then
                Dim errorMsg = "The following channels were NOT handled: "
                For Each channel As String In listChannelName
                    errorMsg &= channel & " "
                Next

                ExceptionUtils.ThrowEx(New EAException(ExitCodes.SetSettingsFailure, errorMsg))
            End If


        Finally
            _iex.ForceHideFailure()
        End Try


    End Sub

    ''' <summary>
    '''   Setting A List Of Channel Numbers To Favorite/Non-Favorite
    ''' </summary>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>300 - NavigationFailure</para>
    ''' <para>302 - EmptyEpgInfoFailure</para>
    ''' <para>304 - IRVerificationFailure</para>
    ''' <para>314 - SetSettingsFailure</para>
    ''' </remarks>
    Overrides Sub SetChannelList(ByVal listChannelName As List(Of Integer), ByVal SetAsFavorites As Boolean)
        Try
            For Each Channel As Integer In listChannelName

                _UI.Utils.TypeKeys(Channel.ToString)

                If SetAsFavorites Then
                    SetChannelAsFavorite()
                Else
                    UnsetChannelFromFavorite()
                End If
            Next

        Finally
            _iex.ForceHideFailure()
        End Try

    End Sub

    ''' <summary>
    '''   Ensuring Channel Has Expected Milestone Value
    ''' </summary>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>302 - EmptyEpgInfoFailure</para>
    ''' <para>304 - IRVerificationFailure</para> 
    ''' <para>314 - SetSettingsFailure</para> 
    ''' </remarks>
 Private Sub RemoveAllFavChannel()



       Dim Value As String = ""
        Dim EpgText As String = ""

        Try
            Dim FavLists As String = ""

            _Utils.EPG_Milestones_NavigateByName("STATE:SET FAVOURITES BY SETTINGS")

            _Utils.SendIR("SELECT_LEFT")

            _Utils.GetEpgInfo("title", Value)



            If Value.ToUpper().Contains("CONFIRM") Then

                _Utils.SendIR("SELECT")

                Try
                    _Utils.GetEpgInfo("favouritechannelnumber", FavLists)
                Catch
                    _Utils.LogCommentInfo("No channels set as favourite")
                    FavLists = ""
                End Try



                If Not [String].IsNullOrEmpty(FavLists) OrElse FavLists.Trim() <> "" Then


                    Dim ListChannelNumAsString As New List(Of String)(FavLists.Split(New String() {","}, StringSplitOptions.RemoveEmptyEntries))
                    Dim ListChannelNum As New List(Of Integer)


                    'Trim All To Eliminate Any Superfluous Space(s) In Input
                    For i As Integer = 0 To ListChannelNumAsString.Count - 1
                        ListChannelNum.Add(CInt(ListChannelNumAsString.Item(i).ToString.Trim()))
                    Next

                    'Navigate to Settings
                    Navigate()

                    UnsetChannelListFromFavorites(ListChannelNum)

                    NavigateToConfirmationInFavorites()

                    ConfirmFavoritesListNumber(ListChannelNumAsString, False)

                    _Utils.LogCommentInfo("Confirmed no channels in favourite list")
                End If
            Else
                EpgText = _Utils.GetValueFromDictionary("DIC_RESET_FAV_CHANNELS")

                _Utils.GetEpgInfo("title", Value)

                If Value <> EpgText Then

                    _Utils.LogCommentInfo("No channels set as favourite")
                    Exit Sub

                End If

                'send the "SELECT" key to confirm the Favourist List
                _Utils.SendIR("SELECT")


                _Utils.GetEpgInfo("title", Value)

                If Value.Contains("CONFIRM") Then

                    _Utils.SendIR("SELECT")

                End If
            End If




            '_Utils.GetEpgInfo("title", Value)
            'If Value <> "SET FAVOURITES" Then

            '    ExceptionUtils.ThrowEx(New EAException(ExitCodes.SetSettingsFailure, "Fail to Remove all Favourites."))
            'Else
            '   _Utils.LogCommentImportant("All favourites are removed sucessfully")

            'End If

            '    Try
            '        _Utils.GetEpgInfo("favouritechannelnumber", FavLists)


            '    Catch

            '        _Utils.LogCommentInfo("No channels set as favourite")


            '    End Try



            '    If Not [String].IsNullOrEmpty(FavLists) OrElse FavLists.Trim() <> "" Then


            '        Dim ListChannelNumAsString As New List(Of String)(FavLists.Split(New String() {","}, StringSplitOptions.RemoveEmptyEntries))
            '        Dim ListChannelNum As New List(Of Integer)


            '        'Trim All To Eliminate Any Superfluous Space(s) In Input
            '        For i As Integer = 0 To ListChannelNumAsString.Count - 1
            '            ListChannelNum.Add(CInt(ListChannelNumAsString.Item(i).ToString.Trim()))
            '        Next

            '        'Navigate to Settings
            '        Navigate()

            '        UnsetChannelListFromFavorites(ListChannelNum)

            '        NavigateToConfirmationInFavorites()

            '        ConfirmFavoritesListNumber(ListChannelNumAsString, False)

            '        _Utils.LogCommentInfo("Confirmed no channels in favourite list")

            '    End If


        Catch ex As Exception

            ExceptionUtils.ThrowEx(New EAException(ExitCodes.SetSettingsFailure, "Fail to Remove all Favourites."))

        Finally
            _iex.ForceHideFailure()
        End Try
		
    End Sub

    Private Sub SetChannel(ByVal FavoriteValue As String)
        _Utils.StartHideFailures("Trying To Set/Unset Channel As/From Favorite")

        'Since the status (favorite or not) is given as (key) milestone only once changed
        'Changing it once and checking the status
        'If the actual status is not the target status, change it again
        'If the actual status is still not the target status, there is a clear problem and an exception is thrown

        Dim isSetAsFavorite As Boolean
        Dim channelStatus As String = ""

        Try
            _Utils.SendIR("SELECT")

            _Utils.GetEpgInfo("key", channelStatus)

            isSetAsFavorite = String.Equals(channelStatus, FavoriteValue)

            If Not isSetAsFavorite Then
                _Utils.SendIR("SELECT")

                _Utils.GetEpgInfo("key", channelStatus)

                isSetAsFavorite = String.Equals(channelStatus, FavoriteValue)

                If Not isSetAsFavorite Then
                    ExceptionUtils.ThrowEx(New EAException(ExitCodes.SetSettingsFailure, "Failed To Set/Unset Channel As/From Favorite"))
                End If
            End If
        Finally
            _iex.ForceHideFailure()
        End Try
    End Sub

    ''' <summary>
    '''   Checks If Channel Is Favorite
    ''' </summary>
    ''' <returns>Boolean</returns>
    ''' ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>302 - EmptyEpgInfoFailure</para>
    ''' <para>304 - IRVerificationFailure</para>
    ''' </remarks>
    Public Overrides Function IsFavorite() As Boolean
        Dim Msg As String = ""

        _Utils.StartHideFailures("Checking If Channel Is Favorite")

        Dim channelStatus As String = ""
        Dim wasFavoriteBeforeChange As Boolean
        Dim isFavoriteAfterChange As Boolean

        'Since the status (favorite or not) is given as (key) milestone only once changed
        'Changing it so that the status after change reflects the contrary of the initial status
        'Then changing it again to restore the initial status

        Try
            _Utils.SendIR("SELECT")

            _Utils.GetEpgInfo("key", channelStatus)
            isFavoriteAfterChange = String.Equals(channelStatus, "LockedChannel")
            wasFavoriteBeforeChange = Not isFavoriteAfterChange

            'restore initial status
            _Utils.SendIR("SELECT")
            Msg = If(wasFavoriteBeforeChange, "The Channel Is Favorite", "The Channel Is NOT Favorite")

            Return wasFavoriteBeforeChange

        Finally
            _iex.ForceHideFailure()
            If Msg <> "" Then
                _Utils.LogCommentInfo(Msg)
            End If
        End Try
    End Function

    ''' <summary>
    '''   Navigating From Favorite List Of Channels To Confirmation
    ''' </summary>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>304 - IRVerificationFailure</para>
    ''' </remarks>
    Public Overrides Sub NavigateToConfirmationInFavorites()
        _Utils.SendIR("SELECT_LEFT") 'Going to Confirm menu
    End Sub

End Class
