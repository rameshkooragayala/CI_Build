Imports System.Runtime.InteropServices

<ClassInterface(ClassInterfaceType.AutoDual)> _
Public Class UI
    Inherits IEX.ElementaryActions.EPG.UI


#Region "Property"

    Shadows Property Project() As String
        Set(ByVal value As String)
            _Project = value
        End Set
        Get
            Return _Project
        End Get
    End Property

    Shadows ReadOnly Property ArchiveRecordings() As ArchiveRecordings
        Get
            Return _ArchiveRecordings
        End Get
    End Property

    Shadows ReadOnly Property PlannerBase() As PlannerBase
        Get
            Return _PlannerBase
        End Get
    End Property

    Shadows ReadOnly Property Banner() As Banner
        Get
            Return _Banner
        End Get
    End Property

    Shadows ReadOnly Property ChannelLineup() As ChannelLineup
        Get
            Return _ChannelLineup
        End Get
    End Property

    Shadows ReadOnly Property ChannelBar() As ChannelBar
        Get
            Return _ChannelBar
        End Get
    End Property

    Shadows ReadOnly Property CurrentRecordings() As CurrentRecordings
        Get
            Return _CurrentRecordings
        End Get
    End Property

    Shadows ReadOnly Property FutureRecordings() As FutureRecordings
        Get
            Return _FutureRecordings
        End Get
    End Property

    Shadows ReadOnly Property Favorites() As Favorites
        Get
            Return _Favorites
        End Get
    End Property

    Shadows ReadOnly Property Guide() As Guide
        Get
            Return _Guide
        End Get
    End Property

    Shadows ReadOnly Property Interactive() As Interactive
        Get
            Return _Interactive
        End Get
    End Property

    Shadows ReadOnly Property TrickModes() As TrickModes

        Get
            Return _TrickModes
        End Get
    End Property

    Shadows ReadOnly Property Vod() As VOD
        Get
            Return _VOD
        End Get

    End Property

    Shadows ReadOnly Property OSD() As Osd
        Get
            Return _OSD
        End Get

    End Property

    Shadows ReadOnly Property OSD_Reminder() As OSD_Reminder
        Get
            Return _OSD_Reminder
        End Get

    End Property

    Shadows ReadOnly Property ManualRecording() As ManualRecording
        Get
            Return _ManualRecording
        End Get
    End Property

    Shadows ReadOnly Property Utils() As Utils
        Get
            Return _Utils
        End Get
    End Property

    Shadows ReadOnly Property Live() As Live
        Get
            Return _Live
        End Get
    End Property

    Shadows ReadOnly Property Menu() As Menu
        Get
            Return _Menu
        End Get
    End Property

    Shadows ReadOnly Property Mount() As Mount
        Get
            Return _Mount
        End Get
    End Property

    Shadows ReadOnly Property Settings() As Settings
        Get
            Return _Settings
        End Get
    End Property

    Shadows ReadOnly Property PowerManagement() As PowerManagement
        Get
            Return _PowerMgmt
        End Get
    End Property

    Shadows ReadOnly Property MediaCentre() As MediaCentre
        Get
            Return _MediaCentre
        End Get
    End Property

    Shadows ReadOnly Property OTA() As OTA
        Get
            Return _OTA
        End Get
    End Property
	
 Shadows ReadOnly Property RMS() As RMS
        Get
            Return _RMS
        End Get
    End Property

#End Region

    Sub New(ByVal _pIex As IEXGateway.IEX)
        MyBase.New(_pIex)
        _Utils = New Utils(_pIex, Me)
        _Utils._DateTime = New EpgDateTime(_pIex)
        _PlannerBase = New PlannerBase(_pIex, Me)
        _ArchiveRecordings = New ArchiveRecordings(_pIex, Me)
        _FutureRecordings = New FutureRecordings(_pIex, Me)
        _Favorites = New Favorites(_pIex, Me)
        _Banner = New Banner(_pIex, Me)
        _ChannelLineup = New ChannelLineup(_pIex, Me)
        _ChannelBar = New ChannelBar(_pIex, Me)
        _Live = New Live(_pIex, Me)
        _OSD_Reminder = New OSD_Reminder(_pIex, Me)
        _Guide = New Guide(_pIex, Me)
        _Menu = New Menu(_pIex, Me)
        _Mount = New Mount(_pIex, Me)
        _TrickModes = New TrickModes(_pIex, Me)
        _ManualRecording = New ManualRecording(_pIex, Me)
        _Settings = New Settings(_pIex, Me)
        _VOD = New VOD(_pIex, Me)
        _PowerMgmt = New PowerManagement(_pIex, Me)
        _MediaCentre = New MediaCentre(_pIex, Me)
        _OTA = New OTA(_pIex, Me)
		_RMS = New RMS(_pIex, Me)
    End Sub

End Class
