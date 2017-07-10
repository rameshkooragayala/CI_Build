Imports System.Text
Imports System.IO
Imports System.Xml
Imports System.Xml.XPath
Imports FailuresHandler

Public Class XML
    Private _DictionariesPath As String
    Private _Dictionary As New Dictionary(Of String, String)
    Dim _iex As IEXGateway.IEX
    Dim _UI As EPG.UI
    Private _Utils As EPG.SF.Utils
    'XPath fields
    Private _XPathDoc As XPathDocument
    Private _xmlNav As XPathNavigator
    Private _xmlNamespaceManager As XmlNamespaceManager

    Sub New()

    End Sub

    Sub New(ByVal DictionariesPath As String, ByVal IEX As IEXGateway.IEX, ByVal pUI As EPG.UI)
        _DictionariesPath = DictionariesPath
        _iex = IEX
        _UI = pUI
    End Sub

#Region "iex.xml"

    ''' <summary>
    '''   Inits XPath For Parsing Of iex.xml
    ''' </summary>
    ''' <param name="fullPathToIexXml">Full File Path To iex.xml</param>
    Private Sub InitXPath(ByVal fullPathToIexXml As String)
        _XPathDoc = New XPathDocument(fullPathToIexXml)
        _xmlNav = _XPathDoc.CreateNavigator
        _xmlNamespaceManager = New XmlNamespaceManager(_xmlNav.NameTable)
        _xmlNamespaceManager.AddNamespace("x", "http://www.nds.com/iex")
    End Sub

    ''' <summary>
    '''   Evaluates XPath Expression Based On iex.xml File
    ''' </summary>
    ''' <param name="strXPathExpression">String XPath Expression</param>
    ''' <returns>String</returns>
    Private Function EvaluateStrXPathExpression(ByVal strXPathExpression As String) As String
        Return _xmlNav.Evaluate("string(" & strXPathExpression & ")", _xmlNamespaceManager)
    End Function

    ''' <summary>
    '''   Retrieves Full Path To Dictionary From iex.xml
    ''' </summary>
    ''' <param name="fullPathToIexXml">Full Path To iex.xml File</param>
    ''' <returns>String</returns>
    Private Function GetFullPathToDictionaryFromIexXml(ByVal fullPathToIexXml As String) As String
        InitXPath(fullPathToIexXml)
        Return EvaluateStrXPathExpression("//x:Param[@Description='Dictionary File']/@Value")
    End Function

    ''' <summary>
    '''   Retrieves Serial Baud Rate From iex.xml
    ''' </summary>
    ''' <param name="fullPathToIexXml">Full Path To iex.xml File</param>
    ''' <returns>String</returns>
    Public Function GetSerialBaudRateFromIexXml(ByVal fullPathToIexXml As String) As String
        InitXPath(fullPathToIexXml)
        Return EvaluateStrXPathExpression("//x:Instance[@Implementation='IEX.Server.Debug.SerialCommunication']/x:Param[@Description='Baudrate']/@Value")
    End Function
#End Region

#Region "Dictionary"
    ''' <summary>
    '''   Loads EPG Dictionary
    ''' </summary>
    ''' <returns>Boolean</returns>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>301 - DictionaryFailure</para> 
    ''' </remarks>
    Public Function LoadDictionary() As Boolean
        Dim sfile As String = ""
        Dim iniFile As AMS.Profile.Ini

        Dim DictionaryPath As String = ""

        _UI.Utils.LogCommentInfo("XML.LoadDictionaries : Trying To Load Dictionary From " + _DictionariesPath.ToString)
        If Directory.Exists(_DictionariesPath) Then

            Dim fullPathToIexXml As String = _UI.Utils.GetFullPathToIexXmlFile(_iex.IEXServerNumber)
            If File.Exists(fullPathToIexXml) Then
                DictionaryPath = GetFullPathToDictionaryFromIexXml(fullPathToIexXml)
            Else
                Dim fullPathToIexIni As String = _UI.Utils.GetFullPathToIexIniFile(_iex.IEXServerNumber)
                iniFile = New AMS.Profile.Ini(fullPathToIexIni)
                DictionaryPath = iniFile.GetValue("MILESTONES_EPG", "EPG_DICTIONARY_PATH").ToString
            End If

            DictionaryPath = DictionaryPath.Remove(0, DictionaryPath.LastIndexOf("\") + 1)

            sfile = _DictionariesPath + "\Dictionary\" + DictionaryPath

            _UI.Utils.LogCommentInfo("XML.LoadDictionaries : Loading XML -> " + sfile.ToString)

            LoadXml(sfile)

            _UI.Utils.StaticParam("Dictionary") = _Dictionary
            Return True

        End If

        ExceptionUtils.ThrowEx(New EAException(ExitCodes.DictionaryFailure, "Failed Loading Dictionary !!!"))

        Return True
    End Function

    ''' <summary>
    '''   Loads EPG Dictionary To EA Dictionary Structure
    ''' </summary>
    ''' <param name="sFile">File Path Of EPG Dictionary</param>
    ''' <returns></returns>
    ''' <remarks>
    ''' Possible Error Codes:
    ''' <para>301 - DictionaryFailure</para> 
    ''' </remarks>
    Private Function LoadXml(ByVal sFile As String) As Boolean
        Try
            Dim XmlString As String = File.ReadAllText(sFile)
            Dim XmlLines As String() = File.ReadAllLines(sFile)
            Dim NoErrors As Boolean = False
            Dim XMLDoc As System.Xml.XmlDocument = Nothing

            While True
                XMLDoc = New System.Xml.XmlDocument
                Try
                    XMLDoc.LoadXml(XmlString)
                    Exit While
                Catch ex As XmlException
                    _UI.Utils.LogCommentFail("LoadXml : Error In Dictionary XML Line " + ex.LineNumber.ToString + vbCrLf + " Line : " + XmlLines(ex.LineNumber - 1).ToString)
                    XmlString = XmlString.Replace(XmlLines(ex.LineNumber - 1), "")
                End Try
            End While

            For Each element As System.Xml.XmlElement In XMLDoc
                For Each childnode As System.Xml.XmlNode In element.ChildNodes
                    Try
                        If childnode.Name = "DIC_SETTINGS_SKIP_FWD_VALUES" Or childnode.Name = "DIC_SETTINGS_SKIP_BACK_VALUES" Or childnode.Name = "DIC_CHANNEL_BAR_TIMEOUT" Then
                            Dim Items As String = childnode.InnerXml
                            Try
                                Items = Items.Replace("</ITEM>", ",")
                                Items = Items.Replace("<ITEM>", "")
                                Items = Items.Remove(Items.Length - 1, 1)
                                childnode.InnerText = Items
                            Catch ex As Exception
                                childnode.InnerText = childnode.InnerText
                            End Try

                        End If
                        _Dictionary.Add(childnode.Name, childnode.InnerText)
                    Catch ex As Exception
                    End Try
                Next
            Next

            Return True

        Catch ex As Exception
            ExceptionUtils.ThrowEx(New EAException(ExitCodes.DictionaryFailure, ex.Message))
        End Try

        ExceptionUtils.ThrowEx(New EAException(ExitCodes.DictionaryFailure, "Load Dictionary Fatal Error"))

        Return True
    End Function

#End Region



End Class
