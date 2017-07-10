Imports System.Runtime.InteropServices

<ClassInterface(ClassInterfaceType.AutoDual)> _
Public Class Manager
    Inherits IEX.ElementaryActions.Functionality.Manager
    <ComVisible(False)> _
    Private _projectName As String
    'Protected Friend Shadows UI As IEX.ElementaryActions.EPG.SF.MOBILE.UI
    Public Shadows UI As IEX.ElementaryActions.EPG.SF.MOBILE.UI

#Region "Local Veriables"
    Dim _LogFilePath As String
    Dim _PVR As PVR
    Dim _PCAT As PCAT
    Dim _MEM As MEM
    Dim _STBSettings As STBSettings

#End Region

#Region "Properties"

    Public Shadows Property PVR() As PVR
        Get
            Return _PVR
        End Get
        Set(ByVal value As PVR)
            _PVR = value
        End Set
    End Property

    Public Shadows Property PCAT() As PCAT
        Get
            Return _PCAT
        End Get
        Set(ByVal value As PCAT)
            _PCAT = value
        End Set
    End Property

    Public Shadows Property MEM() As MEM
        Get
            Return _MEM
        End Get
        Set(ByVal value As MEM)
            _MEM = value
        End Set
    End Property

    Public Shadows Property STBSettings() As STBSettings
        Get
            Return _STBSettings
        End Get
        Set(ByVal value As STBSettings)
            _STBSettings = value
        End Set
    End Property

#End Region

#Region "Constructors"

    Public Sub New()

    End Sub

    Overrides Function Init(ByRef _iex As IEXGateway._IEX, ByVal pName As String, Optional ByRef errorDescription As String = "") As Boolean
        Try
            Try
                MyBase.Init(_iex, "MOBILE", errorDescription)
            Catch ex As Exception
                _iex.LogComment("Manager Init : ERROR Failed To Initialize EA Object Exception Occured ! : " + ex.Message.ToString, False, False, False, CByte(10), "red")
                errorDescription = "Manager Init : ERROR Failed To Initialize EA Object Exception Occured ! : " + ex.Message.ToString
                Return False
            End Try

            UI = MyBase.UI

            Try
                PVR = New PVR(_iex, Me)
            Catch ex As Exception
                _iex.ForceHideFailure()
                UI.Utils.LogCommentFail("Manager Init : Failed To Create PVR Object Instance")
                errorDescription = "Manager Init : Failed To Create PVR Object Instance"
                Return False
            End Try

            Try
                PCAT = New PCAT(_iex, Me)
            Catch ex As Exception
                _iex.ForceHideFailure()
                UI.Utils.LogCommentFail("Manager Init : Failed To Create PCAT Object Instance")
                errorDescription = "Manager Init : Failed To Create PCAT Object Instance"
                Return False
            End Try

            Try
                STBSettings = New STBSettings(_iex, Me)
            Catch ex As Exception
                _iex.ForceHideFailure()
                UI.Utils.LogCommentFail("Manager Init : Failed To Create STBSettings Object Instance")
                errorDescription = "Manager Init : Failed To Create STBSettings Object Instance"
                Return False
            End Try

            Try
                MEM = New MEM(_iex, Me)
            Catch ex As Exception
                _iex.ForceHideFailure()
                UI.Utils.LogCommentFail("Manager Init : Failed To Create MEM Object Instance")
                errorDescription = "Manager Init : Failed To Create MEM Object Instance"
                Return False
            End Try

            If Not UI.Utils.GetTextsFromDictionary() Then
                _iex.ForceHideFailure()
                UI.Utils.LogCommentFail("Manager Init : ERROR Failed To Get Dictionary")
                errorDescription = "Manager Init : ERROR Failed To Get Dictionary"
                Return False
            End If

            If Not UI.Utils.GetMilestonesFromIni() Then
                _iex.ForceHideFailure()
                UI.Utils.LogCommentFail("Manager Init : ERROR Failed To Get Milestones")
                errorDescription = "Manager Init : ERROR Failed To Get Milestones"
                Return False
            End If

        Catch ex As Exception
            _iex.ForceHideFailure()
            UI.Utils.LogCommentFail("Manager Init : ERROR Exception Occured ! : " + ex.Message.ToString)
            errorDescription = "Manager Init : ERROR Exception Occured ! : " + ex.Message.ToString
            Return False
        End Try

        _iex.ForceHideFailure()
        Return True

    End Function
#End Region

#Region "API"
    ''' <summary>
    '''    Dummy function to test the EA
    ''' </summary>
    ''' <returns>IEXGateway._IEXResult</returns>
    ''' <remarks></remarks>
    <ComVisible(False)> _
    Public Function DUMMY_MOBILE(ByVal keyname As String, ByVal toSet As Boolean) As IEXGateway._IEXResult
        Return Invoke("Dummy", keyname, toSet, Me)
    End Function

    ''' <summary>
    '''   Tunning To Channel From TV.Live
    ''' </summary>
    ''' <param name="ChannelName">The Channel Name To Tune To</param>
    ''' <returns>IEXGateway._IEXResult</returns>
    ''' <remarks></remarks>
    Public Function TuneToChannelFromMenu(ByVal ChannelName As String) As IEXGateway._IEXResult
        Return Invoke("TuneToChannelFromMenu", ChannelName, Me)
    End Function

    ''' <summary>
    '''   Performs Fisrt Install Of Canal D Application On IPAD
    ''' </summary>
    ''' <returns>IEXGateway._IEXResult</returns>
    ''' <remarks></remarks>
    Public Function SignIn() As IEXGateway._IEXResult
        Return Invoke("SignIn", Me)
    End Function

#End Region

End Class

