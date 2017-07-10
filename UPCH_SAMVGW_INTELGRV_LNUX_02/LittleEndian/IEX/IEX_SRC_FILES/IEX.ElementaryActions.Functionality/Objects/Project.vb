Public Class Project
    Private _Name As String
    Private _GwHasEPG As Boolean
    Private _IsProduction As Boolean
    Private _CheckForVideoArea As Integer
    Private _IsEPGLikeCogeco As Boolean
    Private _CanSwitchRF As Boolean = False
    Private _MountLikeCogeco As Boolean = True
    Private _IsMobile As Boolean = False
    Private _HasFTIScreens As Boolean = True

#Region " Properties "

    Public Property Name() As String
        Get
            Return _Name
        End Get
        Set(ByVal value As String)
            _Name = value
        End Set
    End Property

    Public Property GwHasEPG() As Boolean
        Get
            Return _GwHasEPG
        End Get
        Set(ByVal value As Boolean)
            _GwHasEPG = value
        End Set
    End Property

    Public Property IsProduction() As Boolean
        Get
            Return _IsProduction
        End Get
        Set(ByVal value As Boolean)
            _IsProduction = value
        End Set
    End Property

    Public Property CheckForVideoArea() As Integer
        Get
            Return _CheckForVideoArea
        End Get
        Set(ByVal value As Integer)
            _CheckForVideoArea = value
        End Set
    End Property

    Public Property IsEPGLikeCogeco() As Boolean
        Get
            Return _IsEPGLikeCogeco
        End Get
        Set(ByVal value As Boolean)
            _IsEPGLikeCogeco = value
        End Set
    End Property

    Public Property MountLikeCogeco() As Boolean
        Get
            Return _MountLikeCogeco
        End Get
        Set(ByVal value As Boolean)
            _MountLikeCogeco = value
        End Set
    End Property

    Public Property CanSwitchRF() As Boolean
        Get
            Return _CanSwitchRF
        End Get
        Set(ByVal value As Boolean)
            _CanSwitchRF = value
        End Set
    End Property

    Public Property IsMoblie() As Boolean
        Get
            Return _IsMobile
        End Get
        Set(ByVal value As Boolean)
            _IsMobile = value
        End Set
    End Property

    Public Property HasFTIScreens() As Boolean
        Get
            Return _HasFTIScreens
        End Get
        Set(ByVal value As Boolean)
            _HasFTIScreens = value
        End Set
    End Property


#End Region
   
    Public Sub New(ByVal ProjectName As String)

        _IsProduction = If(ProjectName.ToLower.Contains(".prd"), True, False)

        If ProjectName.ToLower.Contains("cogeco") Then
            _Name = "COGECO"
            _GwHasEPG = False
            _CheckForVideoArea = 1
            _IsEPGLikeCogeco = True
            _MountLikeCogeco = True
        ElseIf ProjectName.ToLower = "upc" Then
            _Name = "UPC"
            _GwHasEPG = True
            _CheckForVideoArea = 4
            _IsEPGLikeCogeco = False
            _CanSwitchRF = True
            _MountLikeCogeco = False
        ElseIf ProjectName.ToLower = "ipc" Then
            _Name = "IPC"
            _GwHasEPG = True
            _CheckForVideoArea = 5
            _IsEPGLikeCogeco = False
            _MountLikeCogeco = False
        ElseIf ProjectName.ToLower = "get" Then
            _Name = "GET"
            _GwHasEPG = True
            _CheckForVideoArea = 1
            _IsEPGLikeCogeco = True
            _MountLikeCogeco = True
        ElseIf ProjectName.ToLower = "voo" Then
            _Name = "VOO"
            _GwHasEPG = True
            _CheckForVideoArea = 1
            _IsEPGLikeCogeco = False
            _MountLikeCogeco = False
            _HasFTIScreens = False
        ElseIf ProjectName.ToLower = "istb" Then
            _Name = "ISTB"
            _GwHasEPG = True
            _CheckForVideoArea = 1
            _IsEPGLikeCogeco = False
            _MountLikeCogeco = False
			_CanSwitchRF = True
        ElseIf ProjectName.ToLower = "cdigital" Then
            _Name = "CDIGITAL"
            _GwHasEPG = True
            _CheckForVideoArea = 1
            _IsEPGLikeCogeco = True
            _MountLikeCogeco = True
        ElseIf ProjectName.ToLower = "tn" Then
            _Name = "TN"
            _GwHasEPG = True
            _CheckForVideoArea = 1
            _IsEPGLikeCogeco = True
            _MountLikeCogeco = True
            _HasFTIScreens = False
        ElseIf ProjectName.ToLower = "mobile" Then
            _Name = "MOBILE"
            _CheckForVideoArea = 6
            _IsMobile = True
        ElseIf ProjectName.ToLower = "vgw" Then
            _Name = "VGW"
            _GwHasEPG = True
            _CheckForVideoArea = 1
            _IsEPGLikeCogeco = True
            _MountLikeCogeco = True
            _HasFTIScreens = False
		ElseIf ProjectName.ToLower = "ciscorefresh" Then
            _Name = "CISCOREFRESH"
            _GwHasEPG = True
            _CheckForVideoArea = 4
            _IsEPGLikeCogeco = False
            _CanSwitchRF = True
            _MountLikeCogeco = False
        End If
    End Sub

End Class
