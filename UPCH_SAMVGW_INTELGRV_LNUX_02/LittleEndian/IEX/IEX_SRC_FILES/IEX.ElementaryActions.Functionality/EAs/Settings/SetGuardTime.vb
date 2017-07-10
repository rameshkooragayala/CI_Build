Imports FailuresHandler

Namespace EAImplementation
    ''' <summary>
    '''   Sets Start/End Guard Time On Stb Settings
    ''' </summary>
    Public Class SetGuardTime
        Inherits IEX.ElementaryActions.BaseCommand

        Private EPG As IEX.ElementaryActions.EPG.SF.UI
        Private _manager As IEX.ElementaryActions.Functionality.Manager
        Private _IsStart As Boolean
        Private _GTCurrentVal As EnumGuardTime
        Private _GTVlaueToSet As EnumGuardTime

        ''' <param name="IsStart">If True Sets START Else Sets End</param>
        ''' <param name="GTCurrentVal">The Current Value Expected On Guard Time</param>
        ''' <param name="GTValueToSet">If GTCurrent Is NOT_AVAILABLE Sets The Value</param>
        ''' <param name="pManager">Manager</param>
        ''' <remarks>
        ''' Possible Error Codes:
        ''' <para>301 - DictionaryFailure</para> 
        ''' <para>302 - EmptyEpgInfoFailure</para> 
        ''' <para>304 - IRVerificationFailure</para> 
        ''' <para>314 - SetSettingsFailure</para> 
        ''' <para>322 - VerificationFailure</para> 
        ''' <para>332 - NoValidParameters</para> 	
        ''' <para>334 - VideoNotPresent</para> 
        ''' <para>349 - ReturnToLiveFailure</para> 
        ''' </remarks>
        Sub New(ByVal IsStart As Boolean, ByVal GTCurrentVal As EnumGuardTime, ByVal GTValueToSet As EnumGuardTime, ByVal pManager As IEX.ElementaryActions.Functionality.Manager)
            Me._IsStart = IsStart
            Me._GTCurrentVal = GTCurrentVal
            Me._GTVlaueToSet = GTValueToSet
            Me._manager = pManager
            EPG = Me._manager.UI
        End Sub

        Protected Overrides Sub Execute()
            Dim MSG As String = ""
            Dim EnumValue As String = CType(_GTCurrentVal, EnumGuardTime)
            Dim GTString As String = ""

            If Me._IsStart Then
                MSG = "START"
                EPG.Settings.NavigateToStartGuardTime()
            Else
                MSG = "END"
                EPG.Settings.NavigateToEndGuardTime()
            End If

            If _GTCurrentVal <> EnumGuardTime.NOT_AVAILABLE Then
                GTString = GetGuardTimeString(_GTCurrentVal)
                EPG.Settings.VerifySetting(GTString)
            End If

            GTString = GetGuardTimeString(_GTVlaueToSet)

            EPG.Settings.SetSetting(GTString, "")

            EPG.Utils.ReturnToLiveViewing()

        End Sub

        Private Function GetGuardTimeString(ByVal Value As EnumGuardTime) As String
            Dim ReturnedString As String = ""
            Select Case Value
                Case EnumGuardTime.NONE
                    ReturnedString = EPG.Settings.GetSettingsValueFromDictionary("DIC_GUARD_TIME_0", "")
                    If ReturnedString = "" Then
                        ExceptionUtils.ThrowEx(New EAException(ExitCodes.DictionaryFailure, "Failed To Get DIC_GUARD_TIME_0 Value From Dictionary"))
                    End If
                Case EnumGuardTime._1
                    ReturnedString = EPG.Settings.GetSettingsValueFromDictionary("DIC_GUARD_TIME_1", "")
                    If ReturnedString = "" Then
                        ExceptionUtils.ThrowEx(New EAException(ExitCodes.DictionaryFailure, "Failed To Get DIC_GUARD_TIME_1 Value From Dictionary"))
                    End If
                Case EnumGuardTime._2
                    ReturnedString = EPG.Settings.GetSettingsValueFromDictionary("DIC_GUARD_TIME_2", "")
                    If ReturnedString = "" Then
                        ExceptionUtils.ThrowEx(New EAException(ExitCodes.DictionaryFailure, "Failed To Get DIC_GUARD_TIME_2 Value From Dictionary"))
                    End If
                Case EnumGuardTime._3
                    ReturnedString = EPG.Settings.GetSettingsValueFromDictionary("DIC_GUARD_TIME_3", "")
                    If ReturnedString = "" Then
                        ExceptionUtils.ThrowEx(New EAException(ExitCodes.DictionaryFailure, "Failed To Get DIC_GUARD_TIME_3 Value From Dictionary"))
                    End If
                Case EnumGuardTime._5
                    ReturnedString = EPG.Settings.GetSettingsValueFromDictionary("DIC_GUARD_TIME_5", "")
                    If ReturnedString = "" Then
                        ExceptionUtils.ThrowEx(New EAException(ExitCodes.DictionaryFailure, "Failed To Get DIC_GUARD_TIME_5 Value From Dictionary"))
                    End If
                Case EnumGuardTime._10
                    ReturnedString = EPG.Settings.GetSettingsValueFromDictionary("DIC_GUARD_TIME_10", "")
                    If ReturnedString = "" Then
                        ExceptionUtils.ThrowEx(New EAException(ExitCodes.DictionaryFailure, "Failed To Get DIC_GUARD_TIME_10 Value From Dictionary"))
                    End If
                Case EnumGuardTime._15
                    ReturnedString = EPG.Settings.GetSettingsValueFromDictionary("DIC_GUARD_TIME_15", "")
                    If ReturnedString = "" Then
                        ExceptionUtils.ThrowEx(New EAException(ExitCodes.DictionaryFailure, "Failed To Get DIC_GUARD_TIME_15 Value From Dictionary"))
                    End If
                Case EnumGuardTime._30
                    ReturnedString = EPG.Settings.GetSettingsValueFromDictionary("DIC_GUARD_TIME_30", "")
                    If ReturnedString = "" Then
                        ExceptionUtils.ThrowEx(New EAException(ExitCodes.DictionaryFailure, "Failed To Get DIC_GUARD_TIME_30 Value From Dictionary"))
                    End If
                Case EnumGuardTime._60
                    ReturnedString = EPG.Settings.GetSettingsValueFromDictionary("DIC_GUARD_TIME_60", "")
                    If ReturnedString = "" Then
                        ExceptionUtils.ThrowEx(New EAException(ExitCodes.DictionaryFailure, "Failed To Get DIC_GUARD_TIME_60 Value From Dictionary"))
                    End If
                Case EnumGuardTime._120
                    ReturnedString = EPG.Settings.GetSettingsValueFromDictionary("DIC_GUARD_TIME_120", "")
                    If ReturnedString = "" Then
                        ExceptionUtils.ThrowEx(New EAException(ExitCodes.DictionaryFailure, "Failed To Get DIC_GUARD_TIME_120 Value From Dictionary"))
                    End If
                Case EnumGuardTime.AUTOMATIC
                    ReturnedString = EPG.Settings.GetSettingsValueFromDictionary("DIC_GUARD_TIME_AUTO", "")
                    If ReturnedString = "" Then
                        ExceptionUtils.ThrowEx(New EAException(ExitCodes.DictionaryFailure, "Failed To Get DIC_GUARD_TIME_AUTO Value From Dictionary"))
                    End If
                Case Else
                    ExceptionUtils.ThrowEx(New EAException(ExitCodes.NoValidParameters, Value + " Is Not A Valid Parameter"))
            End Select

            Return ReturnedString
        End Function

    End Class

End Namespace