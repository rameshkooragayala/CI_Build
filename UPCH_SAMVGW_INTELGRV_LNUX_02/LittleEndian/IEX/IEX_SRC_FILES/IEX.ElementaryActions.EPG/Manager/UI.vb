Imports System.Runtime.InteropServices

<ClassInterface(ClassInterfaceType.AutoDual)> _
Public MustInherit Class UI
    'Public Class UI
#Region "Public"
    Protected _ArchiveRecordings As ArchiveRecordings
    Protected _Banner As Banner
    Protected _ChannelBar As ChannelBar
    Protected _CurrentRecordings As CurrentRecordings
    Protected _FutureRecordings As FutureRecordings
    Protected _PlannerBase As PlannerBase
    Protected _Guide As Guide
    Protected _Interactive As Interactive
    Protected _ManualRecording As ManualRecording
    Protected _Menu As Menu
    Protected _Mount As Mount
    Protected _ParentalRating As ParentalRating
    Protected _Settings As Settings
    Protected _TrickModes As TrickModes
    Protected _Live As Live
    Protected _ChannelLineup As ChannelLineup
    Protected _Utils As Utils
    Protected _Favorites As Favorites
    Protected _VOD As VOD
    Protected _MediaCentre As MediaCentre
    Protected _OSD As Osd
    Protected _OTA As OTA
    Protected _OSD_Reminder As OSD_Reminder
    Protected _Project As String
    Protected _IEXException As FailuresHandler.IEXException
    Protected _PowerMgmt As PowerManagement
    Protected _RMS As RMS
    Public ChannelsList As EPG.EpgChannel()
    Public channel As EPG.EpgChannel = New EPG.EpgChannel("")
    Public Events As New Dictionary(Of String, IEX.ElementaryActions.EPG.EpgEvent)
#End Region

    Protected _iex As IEXGateway.IEX


#Region "Property"

    Property Project() As String
        Get
            Return _Project
        End Get
        Set(ByVal value As String)
            _Project = value
        End Set
    End Property

    ReadOnly Property ArchiveRecordings() As ArchiveRecordings
        Get
            Return _ArchiveRecordings
        End Get
    End Property

    ReadOnly Property PlannerBase() As PlannerBase
        Get
            Return _PlannerBase
        End Get
    End Property

    ReadOnly Property Banner() As Banner
        Get
            Return _Banner
        End Get
    End Property

    ReadOnly Property ChannelLineup() As ChannelLineup
        Get
            Return _ChannelLineup
        End Get
    End Property

    ReadOnly Property ChannelBar() As ChannelBar
        Get
            Return _ChannelBar
        End Get
    End Property

    ReadOnly Property CurrentRecordings() As CurrentRecordings
        Get
            Return _CurrentRecordings
        End Get
    End Property

    ReadOnly Property FutureRecordings() As FutureRecordings
        Get
            Return _FutureRecordings
        End Get
    End Property

    ReadOnly Property Favorites() As Favorites
        Get
            Return _Favorites
        End Get
    End Property

    ReadOnly Property Guide() As Guide
        Get
            Return _Guide
        End Get
    End Property

    ReadOnly Property Interactive() As Interactive
        Get
            Return _Interactive
        End Get
    End Property

    ReadOnly Property TrickModes() As TrickModes

        Get
            Return _TrickModes
        End Get
    End Property

    ReadOnly Property Vod() As VOD
        Get
            Return _VOD
        End Get

    End Property

    ReadOnly Property OSD() As Osd
        Get
            Return _OSD
        End Get

    End Property

    ReadOnly Property OSD_Reminder() As OSD_Reminder
        Get
            Return _OSD_Reminder
        End Get

    End Property

    ReadOnly Property ManualRecording() As ManualRecording
        Get
            Return _ManualRecording
        End Get
    End Property

    ReadOnly Property Live() As Live
        Get
            Return _Live
        End Get
    End Property

    
    ReadOnly Property Utils() As Utils
        Get
            Return _Utils
        End Get
    End Property

    ReadOnly Property Menu() As Menu
        Get
            Return _Menu
        End Get
    End Property

    ReadOnly Property Mount() As Mount
        Get
            Return _Mount
        End Get
    End Property

    ReadOnly Property Settings() As Settings
        Get
            Return _Settings
        End Get
    End Property

    ReadOnly Property PowerManagement() As PowerManagement
        Get
            Return _PowerMgmt
        End Get
    End Property

#End Region

    Sub New(ByVal _pIex As IEXGateway.IEX)
        _Utils = New Utils(_pIex, Me)
        _Live = New Live(_pIex, Me)
        _ArchiveRecordings = New ArchiveRecordings(_pIex, Me)
        _Banner = New Banner(_pIex, Me)
        _ChannelLineup = New ChannelLineup(_pIex, Me)
        _ChannelBar = New ChannelBar(_pIex, Me)
        _CurrentRecordings = New CurrentRecordings(_pIex, Me)
        _FutureRecordings = New FutureRecordings(_pIex, Me)
        _Favorites = New Favorites(_pIex, Me)
        _Guide = New Guide(_pIex, Me)
        _Interactive = New Interactive(_pIex, Me)
        _ManualRecording = New ManualRecording(_pIex, Me)
        _Menu = New Menu(_pIex, Me)
        _Mount = New Mount(_pIex, Me)
        _ParentalRating = New ParentalRating(_pIex, Me)
        _Settings = New Settings(_pIex, Me)
        _TrickModes = New TrickModes(_pIex, Me)
        _OSD = New Osd(_pIex, Me)
        _OSD_Reminder = New OSD_Reminder(_pIex, Me)
        _VOD = New VOD(_pIex, Me)
        _PowerMgmt = New PowerManagement(_pIex, Me)
    End Sub
End Class
