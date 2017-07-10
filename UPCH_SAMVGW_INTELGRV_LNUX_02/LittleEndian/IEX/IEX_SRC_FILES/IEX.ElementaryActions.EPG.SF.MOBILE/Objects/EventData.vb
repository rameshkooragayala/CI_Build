Public Class EventData
    Private _type As String
    Private _title As String
    Private _cord As String
    Private _chNa As String
    Private _chNum As String
    Private _evtNa As String
    Private _stTime As String
    Private _endTime As String
    Private _isFavor As String
    Private _isLock As String

    Public Property type As String
        Get
            Return _type
        End Get
        Set(ByVal value As String)
            _type = value
        End Set
    End Property

    Public Property title As String
        Get
            Return _title
        End Get
        Set(ByVal value As String)
            _title = value
        End Set
    End Property

    Public Property cord As String
        Get
            Return _cord
        End Get
        Set(ByVal value As String)
            _cord = value
        End Set
    End Property

    Public Property chNa As String
        Get
            Return _chNa
        End Get
        Set(ByVal value As String)
            _chNa = value
        End Set
    End Property

    Public Property chNum As String
        Get
            Return _chNum
        End Get
        Set(ByVal value As String)
            _chNum = value
        End Set
    End Property

    Public Property evtNa As String
        Get
            Return _evtNa
        End Get
        Set(ByVal value As String)
            _evtNa = value
        End Set
    End Property

    Public Property stTime As String
        Get
            Return _stTime
        End Get
        Set(ByVal value As String)
            _stTime = value
        End Set
    End Property

    Public Property endTime As String
        Get
            Return _endTime
        End Get
        Set(ByVal value As String)
            _endTime = value
        End Set
    End Property

    Public Property isFavor As String
        Get
            Return _isFavor
        End Get
        Set(ByVal value As String)
            _isFavor = value
        End Set
    End Property

    Public Property isLock As String
        Get
            Return _isLock
        End Get
        Set(ByVal value As String)
            _isLock = value
        End Set
    End Property

End Class
