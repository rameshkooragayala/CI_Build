Imports System.Runtime.InteropServices

<ClassInterface(ClassInterfaceType.AutoDual)> _
Public Class UI
    Inherits IEX.ElementaryActions.EPG.SF.UPC.UI

#Region "Property"

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

#End Region

    Sub New(ByVal _pIex As IEXGateway.IEX)
        MyBase.New(_pIex)
        MyBase._Utils = New Utils(_pIex, Me)
        MyBase._Live = New Live(_pIex, Me)
        MyBase._Utils._DateTime = New EpgDateTime(_pIex)
        MyBase._PlannerBase = New PlannerBase(_pIex, Me)
        MyBase._ArchiveRecordings = New ArchiveRecordings(_pIex, Me)
        MyBase._FutureRecordings = New FutureRecordings(_pIex, Me)
        MyBase._Banner = New Banner(_pIex, Me)
        MyBase._ChannelLineup = New ChannelLineup(_pIex, Me)
        MyBase._ChannelBar = New ChannelBar(_pIex, Me)
        MyBase._OSD_Reminder = New OSD_Reminder(_pIex, Me)
        MyBase._Guide = New Guide(_pIex, Me)
        MyBase._Menu = New Menu(_pIex, Me)
        MyBase._TrickModes = New TrickModes(_pIex, Me)
        MyBase._ManualRecording = New ManualRecording(_pIex, Me)
        MyBase._Settings = New Settings(_pIex, Me)
        MyBase._VOD = New VOD(_pIex, Me)
        MyBase._Mount = New Mount(_pIex, Me)
		MyBase._OTA = New OTA(_pIex, Me)
        'MyBase._Favorites = New Favorites(_iex, Me)

    End Sub
End Class
