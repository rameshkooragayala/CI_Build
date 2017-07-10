Public Class EpgChannel
    Public ChannelNumber As String
    Public ChannelXMLName As String
    Public ChannelEPGName As String
    Public ChannelEPGType As String

    Sub New(ByVal channelNumber As String)
        Me.New(channelNumber, "", "", "")
    End Sub

    Sub New(channelNumber As String, channelXMLName As String)
        Me.New(channelNumber, channelXMLName, "", "")
    End Sub

    Sub New(channelNumber As String, channelXMLName As String, channelEPGName As String)
        Me.New(channelNumber, channelXMLName, channelEPGName, "")
    End Sub

    Sub New(channelNumber As String, channelXMLName As String, channelEPGName As String, channelEPGType As String)
        Me.ChannelNumber = channelNumber
        Me.ChannelXMLName = channelXMLName
        Me.ChannelEPGName = channelEPGName
        Me.ChannelEPGType = channelEPGType
    End Sub

    Sub New()
        Me.ChannelNumber = String.Empty
        Me.ChannelXMLName = String.Empty
        Me.ChannelEPGName = String.Empty
        Me.ChannelEPGType = String.Empty
    End Sub

    Public Overrides Function ToString() As String
        Dim s As String
        s = "ChannelNumber= " & Me.ChannelNumber & vbTab
        If Not String.IsNullOrEmpty(Me.ChannelXMLName) Then
            s += "; ChannelXMLName= " & Me.ChannelXMLName & vbTab
        End If
        If Not String.IsNullOrEmpty(Me.ChannelEPGName) Then
            s += "; ChannelEPGName= " & Me.ChannelEPGName & vbTab
        End If
        If Not String.IsNullOrEmpty(Me.ChannelEPGType) Then
            s += "; ChannelEPGType= " & Me.ChannelEPGType
        End If
        Return s
    End Function

    Public Function IsSameChannel(ByVal compareToChannel As EPG.EpgChannel) As Boolean
        Dim actualChannel As EPG.EpgChannel = Me

        If Not actualChannel.channelNumber = compareToChannel.channelNumber Then Return False

        If Not String.IsNullOrEmpty(actualChannel.channelXMLName) Then
            If Not actualChannel.channelXMLName = compareToChannel.channelXMLName Then Return False
        End If
        If Not String.IsNullOrEmpty(actualChannel.channelEPGName) Then
            If Not actualChannel.channelEPGName = compareToChannel.channelEPGName Then Return False
        End If
        If Not String.IsNullOrEmpty(actualChannel.channelEPGType) Then
            If Not actualChannel.channelEPGType = compareToChannel.channelEPGType Then Return False
        End If
        Return True
    End Function


End Class
