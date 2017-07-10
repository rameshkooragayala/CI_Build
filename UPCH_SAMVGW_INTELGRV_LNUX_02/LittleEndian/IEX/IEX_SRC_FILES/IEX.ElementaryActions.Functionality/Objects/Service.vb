Public Class Service
    Public LCN As String
    Public Name As String
	Public ServiceId As String
    Public Type As String
    Public IsDefault As String
    Public IsRecordable As String
    Public IsViewable As String
    Public IsEITAvailable As String
	Public IsSeries As String
    Public IsMinEventDuration As String
    Public IsConstantEventDuration As String
    Public EventDuration As String
    Public Encryption As String
    Public Encoding As String
    Public AudioEncoding As String
    Public Resolution As String
    Public PositionOnGuide As String
    Public PositionOnList As String
    Public HasChannelLogo As String
    Public ChannelLogo As String
    Public HasThumbnail As String
    Public HasSynopsis As String
    Public HasSubtitles As String
	Public SubtitleType As String
    Public NoOfSubtitleLanguages As String
    Public SubtitleLanguage As List(Of String)
    Public HasMultipleAudio As String
    Public NoOfAudioLanguages As String
    Public AudioLanguage As List(Of String)
    Public ParentalRating As String
End Class
