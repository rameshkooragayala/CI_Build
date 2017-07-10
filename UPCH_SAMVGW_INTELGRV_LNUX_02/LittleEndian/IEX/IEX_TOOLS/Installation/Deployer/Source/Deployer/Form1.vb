Imports System.IO

Public Class Form1
    Dim IEX_PCAT_MODIFIER_Dest As String
    Dim IEX_INI_FILES_Dest As String
    Dim IEX_ELEMENTARYACTIONS_Dest As String
    Dim IEX_USERLIB As String = ""
    Dim IEX_MILESTONES As String = ""
    Dim IEX_ENVIRONMENT As String = ""
    Dim IEX_CHANNELS As String = ""
    Dim IEX_TELNET As String = ""
    Dim IEX_ELEMENTARYACTIONS As String = ""
    Dim IEX_TRACERCONFIG As String = ""
    Dim IEX_SPMCONFIG As String = ""
    Dim IEX_FREQCONFIG As String = ""
    Dim IEX_EPGPROPERTIES As String = ""
    Dim IEX_DIAGSTREAMS As String = ""
    Dim IEX_DICTIONARY As String = ""
    Dim IEX_STATEMACHINE As String = ""
    Dim IEX_IR As String = ""
    Dim ROOTPATH As String = ""
    Dim CogecoGateway As String = ""
    Dim CogecoClient As String = ""
    Dim CogecoGWIEXNumber As String = ""
    Dim CogecoClIEXNumber As String = ""
    Dim UpcGateway As String = ""
    Dim UpcGWIEXNumber As String = ""
    Dim UpcClIEXNumber As String = ""
    Dim UpcClient As String = ""
    Dim UpcTGShortStream As Boolean
    Dim UpcIEXTGStream As Boolean
    Dim UpcOriginalStream As Boolean
    Dim CogecoTGShortStream As Boolean
    Dim CogecoOriginalStream As Boolean
    Dim CogecoIEXTGStream As Boolean
    Dim ManualTGShortStream As Boolean
    Dim ManualOriginalStream As Boolean
    Dim ManualIEXTGStream As Boolean
    Dim ManualGWIEXNumber As String = ""
    Dim ManualClIEXNumber As String = ""
    Dim ToConfigureSPM As Boolean = True
    Dim TabChange As Boolean = False
    Dim Project As String = ""
    Public UserName As String = ""
    Public Password As String = ""
    Dim GwNumber As String = ""

    Private Sub SetPaths(ByVal IEXNumber As String, Optional ByVal IsGateway As Boolean = True)
        If TabProject.SelectedIndex = 0 OrElse TabProject.SelectedIndex = 1 Then
            If Project = "UPC" Then
                ROOTPATH = "\\10.62.14.241\extra\mnt\users\IEX\"
                IEX_TRACERCONFIG = "\\10.62.14.241\extra\mnt\users\" + TxtUserName.Text + "\IEX" + IEXNumber + "\NDS\resources\TracerConfigMap.xml"
                IEX_SPMCONFIG = "\\10.62.14.241\extra\mnt\users\" + TxtUserName.Text + "\IEX" + IEXNumber + "\NDS\config\spm.cfg"
                IEX_FREQCONFIG = "\\10.62.14.241\extra\mnt\users\" + TxtUserName.Text + "\IEX" + IEXNumber + "\NDS\config\freqHunt.txt"
                IEX_EPGPROPERTIES = "\\10.62.14.241\extra\mnt\users\" + TxtUserName.Text + "\IEX" + IEXNumber + "\NDS\resources\EPG_properties.xml"
                IEX_DICTIONARY = "\\10.62.14.241\extra\mnt\users\" + TxtUserName.Text + "\IEX" + IEXNumber + "\NDS\resources"
            ElseIf Project = "COGECO" Then
                ROOTPATH = "\\pooh\home_nfs\IEX\"
                IEX_TRACERCONFIG = "\\pooh\home_nfs\" + TxtUserName.Text + "\IEX" + IEXNumber + "\NDS\resources\TracerConfigMap.xml"
                IEX_SPMCONFIG = "\\pooh\home_nfs\" + TxtUserName.Text + "\IEX" + IEXNumber + "\NDS\config\spm.cfg"
                IEX_FREQCONFIG = "\\pooh\home_nfs\" + TxtUserName.Text + "\IEX" + IEXNumber + "\NDS\config\freqHunt.txt"
                IEX_EPGPROPERTIES = "\\pooh\home_nfs\" + TxtUserName.Text + "\IEX" + IEXNumber + "\NDS\resources\EPG_properties.xml"
                IEX_DICTIONARY = "\\pooh\home_nfs\" + TxtUserName.Text + "\IEX" + IEXNumber + "\NDS\resources"
            End If

            IEX_USERLIB = ROOTPATH + "IEX_INI_FILES\Userlib.vbs"
            IEX_MILESTONES = ROOTPATH + "IEX_INI_FILES\Milestones.ini"
            IEX_ENVIRONMENT = ROOTPATH + "IEX_INI_FILES\Environment.ini"
            IEX_CHANNELS = ROOTPATH + "IEX_INI_FILES\Channels.ini"
            IEX_TELNET = ROOTPATH + "IEX_INI_FILES\Telnet.ini"
            IEX_IR = ROOTPATH + "IEX_GW_IAL\IR\"
            IEX_STATEMACHINE = TxtFrom.Text + "IEX_INI_FILES\EpgStateMachineConfiguration.xml"

        ElseIf TabProject.SelectedIndex = 2 Then
            If TxtFrom.Text.EndsWith("\") = False Then
                TxtFrom.Text = TxtFrom.Text + "\"
            End If
            If TxtGwBuildPath.Text.EndsWith("\") = False Then
                TxtGwBuildPath.Text = TxtGwBuildPath.Text + "\"
            End If
            If TxtClBuildPath.Text.EndsWith("\") = False Then
                TxtClBuildPath.Text = TxtClBuildPath.Text + "\"
            End If

            If Project = "UPC" Then
                ROOTPATH = TxtGwBuildPath.Text
            ElseIf Project = "COGECO" Then
                If IsGateway Then
                    ROOTPATH = TxtGwBuildPath.Text
                Else
                    ROOTPATH = TxtClBuildPath.Text
                End If
            End If

            IEX_TRACERCONFIG = ROOTPATH + "resources\TracerConfigMap.xml"
            IEX_SPMCONFIG = ROOTPATH + "config\spm.cfg"
            IEX_FREQCONFIG = ROOTPATH + "config\freqHunt.txt"
            IEX_EPGPROPERTIES = ROOTPATH + "resources\EPG_properties.xml"
            IEX_DIAGSTREAMS = ROOTPATH + "config\diag_streams.cfg"
            IEX_DICTIONARY = ROOTPATH + "resources"

            ROOTPATH = TxtFrom.Text
            IEX_USERLIB = TxtFrom.Text + "IEX_INI_FILES\Userlib.vbs"
            IEX_MILESTONES = TxtFrom.Text + "IEX_INI_FILES\Milestones.ini"
            IEX_ENVIRONMENT = TxtFrom.Text + "IEX_TOOLS\Installation\Dev EnvironmentINI\Environment.ini"
            IEX_CHANNELS = TxtFrom.Text + "IEX_INI_FILES\Channels.ini"
            IEX_TELNET = TxtFrom.Text + "IEX_INI_FILES\Telnet.ini"
            IEX_IR = TxtFrom.Text + "IEX_GW_IAL\IR\"
            IEX_STATEMACHINE = TxtFrom.Text + "IEX_INI_FILES\EpgStateMachineConfiguration.xml"
        End If
    End Sub

    Private Sub ButDeploy_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ButDeploy.Click
        If Project = "UPC" Then
            GwNumber = ""
            UpcGWIEXNumber = CmbGwIEXNumber.Text
            DeployEnvironment(CmbGwIEXNumber.Text, True)
        ElseIf Project = "COGECO" Then
            GwNumber = CmbGwIEXNumber.Text
            CogecoGWIEXNumber = CmbGwIEXNumber.Text
            DeployEnvironment(CmbGwIEXNumber.Text, True)
            CogecoClIEXNumber = CmbClIEXNumber.Text
            DeployEnvironment(CmbClIEXNumber.Text, True)
        End If

        If ChkClean.Checked Then
            ShowNewMachineMessage()
        End If
    End Sub

    Private Sub DeployEnvironment(ByVal IEXNumber As String, Optional ByVal ToSetPaths As Boolean = True)

        If ToSetPaths Then
            ToConfigureSPM = True
            SetPaths(IEXNumber, True)
        Else
            ToConfigureSPM = False
        End If

        IEX_ELEMENTARYACTIONS = "C:\ElementaryActionsProxy\"
        IEX_PCAT_MODIFIER_Dest = "C:\PCAT_Modifier\IEX" + IEXNumber + "\"
        IEX_INI_FILES_Dest = "C:\Program Files\IEX\Tests\TestsINI\IEX" + IEXNumber + "\"
        IEX_ELEMENTARYACTIONS_Dest = "C:\ElementaryActionsProxy\IEX" + IEXNumber + "\"

        Dim passed As Boolean = True
        LblStatus.Visible = False

        LblStatus.Text = "Copying"
        LblStatus.ForeColor = Color.Black
        LblStatus.Visible = True
        Application.DoEvents()
        LblStatus.Refresh()

        TxtLog.Text = ""

        TxtLog.Text += "Creating Test INI's Directory.............."
        Application.DoEvents()

        If Directory.Exists("C:\Program Files\IEX\Tests\TestsINI\IEX" + IEXNumber) = False Then
            Try
                Directory.CreateDirectory("C:\Program Files\IEX\Tests\TestsINI\IEX" + IEXNumber)
            Catch ex As Exception
                LblStatus.Text = "Failed"
                MsgBox("Error Creating Directory : C:\Program Files\IEX\Tests\TestsINI\IEX" + IEXNumber)
                LblStatus.ForeColor = Color.Red
                LblStatus.Visible = True
                Exit Sub
            End Try
        End If
        TxtLog.Text += "PASSED" + vbCrLf
        Application.DoEvents()

        If ChkClean.Checked Then
            TxtLog.Text += "Copying Environment.ini.............."
            Application.DoEvents()
            passed = CopyFile(IEX_ENVIRONMENT, "C:\Program Files\IEX\Tests\TestsINI\IEX" + IEXNumber + "\Environment.ini")
            If passed = False Then
                LblStatus.Text = "Failed"
                MsgBox("Problem Copying Environment.ini Please Make Sure It Isn't Open !!!")
                LblStatus.ForeColor = Color.Red
                LblStatus.Visible = True
                Exit Sub
            End If

            TxtLog.Text += "PASSED" + vbCrLf
            Application.DoEvents()

            TxtLog.Text += "Copying Channels.ini.............."
            Application.DoEvents()
            passed = CopyFile(IEX_CHANNELS, "C:\Program Files\IEX\Tests\TestsINI\IEX" + IEXNumber + "\Channels.ini")
            If passed = False Then
                LblStatus.Text = "Failed"
                MsgBox("Problem Copying Channels.ini Please Make Sure It Isn't Open !!!")
                LblStatus.ForeColor = Color.Red
                LblStatus.Visible = True
                Exit Sub
            End If

            TxtLog.Text += "PASSED" + vbCrLf
            Application.DoEvents()

            TxtLog.Text += "Copying Telnet.ini.............."
            Application.DoEvents()
            passed = CopyFile(IEX_TELNET, "C:\Program Files\IEX\Tests\TestsINI\IEX" + IEXNumber + "\Telnet.ini")
            If passed = False Then
                LblStatus.Text = "Failed"
                MsgBox("Problem Copying Telnet.ini Please Make Sure It Isn't Open !!!")
                LblStatus.ForeColor = Color.Red
                LblStatus.Visible = True
                Exit Sub
            End If

            TxtLog.Text += "PASSED" + vbCrLf
            Application.DoEvents()

            TxtLog.Text += "Copying EpgStateMachineConfiguration.xml.............."
            Application.DoEvents()
            passed = CopyFile(IEX_STATEMACHINE, "C:\Program Files\IEX\Tests\TestsINI\IEX" + IEXNumber + "\EpgStateMachineConfiguration.xml")
            If passed = False Then
                LblStatus.Text = "Failed"
                MsgBox("Problem Copying EpgStateMachineConfiguration.xml Please Make Sure It Isn't Open !!!")
                LblStatus.ForeColor = Color.Red
                LblStatus.Visible = True
                Exit Sub
            End If

            TxtLog.Text += "PASSED" + vbCrLf
            Application.DoEvents()

            TxtLog.Text += "Configuring IEX.ini.............."
            Application.DoEvents()
            passed = UpdateIEXIni(IEXNumber)
            If passed = False Then
                LblStatus.Text = "Failed"
                MsgBox("Problem Configuring IEX " + IEXNumber.ToString + " ini Please Make Sure It Isn't Open !!!")
                LblStatus.ForeColor = Color.Red
                LblStatus.Visible = True
                Exit Sub
            End If

            TxtLog.Text += "PASSED" + vbCrLf
            Application.DoEvents()

            TxtLog.Text += "Disabling Remote Reporting .............."
            Application.DoEvents()

            If Not DisableRemoteReporting() Then
                LblStatus.Text = "Failed"
                MsgBox("Error Disabling Remote Reporting")
                LblStatus.ForeColor = Color.Red
                LblStatus.Visible = True
                Exit Sub
            End If

            TxtLog.Text += "PASSED" + vbCrLf
            Application.DoEvents()

            TxtLog.Text += "Copying IR.............."
            Application.DoEvents()

            If Project = "UPC" Then
                passed = CopyAllFiles(IEX_IR, "C:\Program Files\IEX\Projects\UPC_PVR\")
            Else
                passed = CopyAllFiles(IEX_IR, "C:\Program Files\IEX\Projects\Arris\")
            End If

            If passed = False Then
                LblStatus.Text = "Failed"
                MsgBox("Problem Copying IR Files")
                LblStatus.ForeColor = Color.Red
                LblStatus.Visible = True
                Exit Sub
            End If

            passed = RemoveReadonly("C:\Program Files\IEX\Projects\")
            If passed = False Then
                LblStatus.Text = "Failed"
                MsgBox("Failed To Remove Readonly Attribute From C:\Program Files\IEX\Projects\")
                LblStatus.ForeColor = Color.Red
                LblStatus.Visible = True
                Exit Sub
            End If

            TxtLog.Text += "PASSED" + vbCrLf
            Application.DoEvents()

        End If

        passed = RemoveReadonly("C:\Program Files\IEX\Tests\TestsINI\IEX" + IEXNumber + "\")
        If passed = False Then
            LblStatus.Text = "Failed"
            MsgBox("Failed To Remove Readonly Attribute From C:\Program Files\IEX\Tests\TestsINI\IEX" + IEXNumber + "\")
            LblStatus.ForeColor = Color.Red
            LblStatus.Visible = True
            Exit Sub
        End If

        If Project = "COGECO" And ToConfigureSPM Then
            'TxtLog.Text += "Configuring SPM CONFIG........"
            'Application.DoEvents()

            'passed = ConfigureCogecoSpm()
            'If passed = False Then
            '    LblStatus.Text = "Failed"
            '    MsgBox("Failed To Config SPM CONFIG")
            '    LblStatus.ForeColor = Color.Red
            '    LblStatus.Visible = True
            '    Exit Sub
            'End If

            'TxtLog.Text += "PASSED" + vbCrLf
            'Application.DoEvents()
        End If

        If RadTG.Checked And ToConfigureSPM Then
            TxtLog.Text += "Configuring TIG GEN SHORT STREAM........"
            Application.DoEvents()

            passed = ConfigureTigGen(False)
            If passed = False Then
                LblStatus.Text = "Failed"
                MsgBox("Failed To Config TIG GEN SHORT STREAM")
                LblStatus.ForeColor = Color.Red
                LblStatus.Visible = True
                Exit Sub
            End If

            TxtLog.Text += "PASSED" + vbCrLf
            Application.DoEvents()
        ElseIf RadIEXTG.Checked And ToConfigureSPM Then
            TxtLog.Text += "Configuring IEX TIG GEN STREAM........"
            Application.DoEvents()

            passed = ConfigureTigGen(True)
            If passed = False Then
                LblStatus.Text = "Failed"
                MsgBox("Failed To Config IEX TIG GEN STREAM")
                LblStatus.ForeColor = Color.Red
                LblStatus.Visible = True
                Exit Sub
            End If

            TxtLog.Text += "PASSED" + vbCrLf
            Application.DoEvents()
        End If

        TxtLog.Text += "Copying Milestones.............."
        Application.DoEvents()
        passed = CopyFile(IEX_MILESTONES, "C:\Program Files\IEX\Tests\TestsINI\IEX" + IEXNumber + "\Milestones.ini")
        If passed = False Then
            LblStatus.Text = "Failed"
            MsgBox("Problem Copying Milestones.ini Please Make Sure It Isn't Open !!!")
            LblStatus.ForeColor = Color.Red
            LblStatus.Visible = True
            Exit Sub
        End If

        TxtLog.Text += "PASSED" + vbCrLf
        Application.DoEvents()

        If Project = "COGECO" And IEXNumber = GwNumber Then
            LblStatus.Text = "Success"
            LblStatus.ForeColor = Color.Green
            LblStatus.Visible = True
            Exit Sub
        End If

        '----------------------------------------------------------------------COGECO GATEWAY UNTIL HERE ----------------------------------------------------------------
        If IEXNumber <> ManualGWIEXNumber Then
            TxtLog.Text += "Copying Dictionary.............."
            Application.DoEvents()
            passed = CopyDirectory(IEX_DICTIONARY, IEX_INI_FILES_Dest + "Dictionary", True)
            If passed = False Then
                LblStatus.Text = "Failed"
                MsgBox("Failed To Copy Dictionary Please Check Path : " + IEX_DICTIONARY)
                LblStatus.ForeColor = Color.Red
                LblStatus.Visible = True
                Exit Sub
            End If
            TxtLog.Text += "PASSED" + vbCrLf
            Application.DoEvents()
        End If

        TxtLog.Text += "Copying Dll's..................."
        Application.DoEvents()
        passed = CopyDirectory(ROOTPATH + "IEX_ELEMENTARYACTIONS\", IEX_ELEMENTARYACTIONS_Dest)
        If passed = False Then
            LblStatus.Text = "Failed"
            MsgBox("Please Close Test Creator/MultiRunner !!!")
            LblStatus.ForeColor = Color.Red
            LblStatus.Visible = True
            Exit Sub
        End If

        passed = CopyDirectory(ROOTPATH + "IEX_ELEMENTARYACTIONS\", IEX_ELEMENTARYACTIONS)
        If passed = False Then
            LblStatus.Text = "Failed"
            MsgBox("Please Close Test Creator/MultiRunner !!!")
            LblStatus.ForeColor = Color.Red
            LblStatus.Visible = True
            Exit Sub
        End If
        TxtLog.Text += "PASSED" + vbCrLf
        Application.DoEvents()

        passed = RemoveReadonly(IEX_ELEMENTARYACTIONS)
        If passed = False Then
            LblStatus.Text = "Failed"
            MsgBox("Failed To Remove Readonly Attribute From " + IEX_ELEMENTARYACTIONS)
            LblStatus.ForeColor = Color.Red
            LblStatus.Visible = True
            Exit Sub
        End If

        TxtLog.Text += "Copying PCAT Modifier..........."
        Application.DoEvents()
        passed = CopyDirectory(ROOTPATH + "IEX_PCAT_MODIFIER\", IEX_PCAT_MODIFIER_Dest)
        If passed = False Then
            LblStatus.Text = "Failed"
            MsgBox("Problem Copying PCAT_Modifier !!!")
            LblStatus.ForeColor = Color.Red
            LblStatus.Visible = True
            Exit Sub
        End If
        TxtLog.Text += "PASSED" + vbCrLf
        Application.DoEvents()

        passed = RemoveReadonly(IEX_PCAT_MODIFIER_Dest)
        If passed = False Then
            LblStatus.Text = "Failed"
            MsgBox("Failed To Remove Readonly Attribute From " + IEX_PCAT_MODIFIER_Dest)
            LblStatus.ForeColor = Color.Red
            LblStatus.Visible = True
            Exit Sub
        End If

        TxtLog.Text += "Copying UserLib................."
        Application.DoEvents()
        passed = CopyFile(IEX_USERLIB, "C:\Program Files\IEX\tc2_header_footer\UserLib.vbs")
        If passed = False Then
            LblStatus.Text = "Failed"
            MsgBox("Problem Copying UserLib.vbs !!!")
            LblStatus.ForeColor = Color.Red
            LblStatus.Visible = True
            Exit Sub
        End If
        TxtLog.Text += "PASSED" + vbCrLf
        Application.DoEvents()

        passed = RemoveReadonly("C:\Program Files\IEX\tc2_header_footer\")
        If passed = False Then
            LblStatus.Text = "Failed"
            MsgBox("Failed To Remove Readonly Attribute From C:\Program Files\IEX\tc2_header_footer\")
            LblStatus.ForeColor = Color.Red
            LblStatus.Visible = True
            Exit Sub
        End If

        If IEXNumber <> ManualGWIEXNumber Then
            TxtLog.Text += "Configuring TracerConfig........"
            Application.DoEvents()

            passed = ConfigureTracer(True)
            If passed = False Then
                LblStatus.Text = "Failed"
                MsgBox("Failed To Config " + IEX_TRACERCONFIG + " To Use EPG Milestones")
                LblStatus.ForeColor = Color.Red
                LblStatus.Visible = True
                Exit Sub
            End If

            TxtLog.Text += "PASSED" + vbCrLf
            Application.DoEvents()


            TxtLog.Text += "Configuring EPG Properties......"
            Application.DoEvents()
            passed = ConfigureEpgProperties()
            If passed = False Then
                LblStatus.Text = "Failed"
                MsgBox("Failed To Change Wizard Property In " + IEX_EPGPROPERTIES)
                LblStatus.ForeColor = Color.Red
                LblStatus.Visible = True
                Exit Sub
            End If

            TxtLog.Text += "PASSED" + vbCrLf
        End If


        If passed Then
            LblStatus.Text = "Success"
            LblStatus.ForeColor = Color.Green
            LblStatus.Visible = True
        Else
            LblStatus.Text = "Failed"
            LblStatus.ForeColor = Color.Red
            LblStatus.Visible = True
        End If
    End Sub

    Private Function ConfigureCogecoSpm() As Boolean
        Try

            If File.Exists(IEX_SPMCONFIG) Then
                Dim SpmContent As String() = File.ReadAllLines(IEX_SPMCONFIG)
                Dim SpmNewContent As String = ""
                Dim ReachedSection As Boolean = False

                For Each line As String In SpmContent
                    If line.Contains("[DVBC_SCAN]") Then
                        ReachedSection = True
                    End If
                    If line.Contains("BOUQUET_ID=") And ReachedSection Then
                        line = "BOUQUET_ID=0x500"
                    ElseIf line.Contains("FREQUENCY_PLAN_ID=") And ReachedSection Then
                        line = "FREQUENCY_PLAN_ID=0x02"
                        ReachedSection = False
                    ElseIf line.Contains("PS_SERVER_URI=" + Chr(34) + "http:") Then
                        line = "PS_SERVER_URI=" + Chr(34) + "http://10.63.110.6:6040/" + Chr(34)
                    ElseIf line.Contains("OOB_SI_MULTICAST_ADDRESS=") Then
                        line = "OOB_SI_MULTICAST_ADDRESS=" + Chr(34) + "multicast://239.255.253.135:9051" + Chr(34)
                    ElseIf line.Contains("REGISTRATION=") Then
                        line = "REGISTRATION=TRUE"
                    End If
                    SpmNewContent += line + vbCrLf
                Next

                File.WriteAllText(IEX_SPMCONFIG, SpmNewContent)

            End If

            If File.Exists(IEX_FREQCONFIG) Then
                Dim FreqContent As String() = File.ReadAllLines(IEX_FREQCONFIG)
                Dim FreqNewContent As String = ""
                Dim First As Boolean = True

                For Each line As String In FreqContent
                    If Char.IsDigit(line.Substring(0, 1)) And First Then
                        FreqNewContent += "495000:5361000:5361000:10:10" + vbCrLf
                        First = False
                    End If

                    FreqNewContent += line + vbCrLf
                Next

                File.WriteAllText(IEX_FREQCONFIG, FreqNewContent)

                Return True
            End If

            Return False
        Catch ex As Exception
            Return False
        End Try

    End Function

    Private Function ConfigureTigGen(ByVal IEXStream As Boolean) As Boolean
        Try

            If File.Exists(IEX_SPMCONFIG) Then
                Dim SpmContent As String() = File.ReadAllLines(IEX_SPMCONFIG)
                Dim SpmNewContent As String = ""
                Dim ReachedSection As Boolean = False

                For Each line As String In SpmContent
                    If line.Contains("[DVBC_SCAN]") Then
                        ReachedSection = True
                    End If
                    If line.Contains("NETWORK_ID") And ReachedSection Then
                        line = "NETWORK_ID=4096"
                        ReachedSection = False
                    ElseIf line.Contains("PROVINCE_CODE") Then
                        line = "PROVINCE_CODE=0x01"
                    ElseIf line = "[MISC]" Then
                        line = line + vbCrLf + "NVRAM_NAME=" + Chr(34) + "NVRAM" + Chr(34)
                    ElseIf line.Contains("CITY_CODE=") Then
                        line = "CITY_CODE=0x0001"
                    End If
                    SpmNewContent += line + vbCrLf
                Next

                File.WriteAllText(IEX_SPMCONFIG, SpmNewContent)

            End If

            If File.Exists(IEX_FREQCONFIG) Then
                Dim FreqContent As String() = File.ReadAllLines(IEX_FREQCONFIG)
                Dim FreqNewContent As String = ""
                Dim First As Boolean = True

                For Each line As String In FreqContent
                    If Char.IsDigit(line.Substring(0, 1)) And First Then
                        If IEXStream Then
                            FreqNewContent += "242000:6900000:6952000:10:8" + vbCrLf
                            First = False
                        Else
                            FreqNewContent += "356000:6875000:6887000:10:8" + vbCrLf
                            First = False
                        End If
                    End If

                    FreqNewContent += line + vbCrLf
                Next

                File.WriteAllText(IEX_FREQCONFIG, FreqNewContent)

                Return True
            End If

            Return False
        Catch ex As Exception
            Return False
        End Try
    End Function

    Private Function ConfigureEpgProperties() As Boolean
        Try

            If File.Exists(IEX_EPGPROPERTIES) Then
                Dim EpgPropertiesContent As String = File.ReadAllText(IEX_EPGPROPERTIES)

                EpgPropertiesContent = EpgPropertiesContent.Replace("<prop enableWizard=" + Chr(34) + "true" + Chr(34) + "></prop>", "<prop enableWizard=" + Chr(34) + "false" + Chr(34) + "></prop>")

                File.WriteAllText(IEX_EPGPROPERTIES, EpgPropertiesContent)
                Return True
            End If

            Return False
        Catch ex As Exception
            Return False
        End Try
    End Function

    Private Function ConfigureTracer(ByVal WithMilestones As Boolean) As Boolean
        Try

            If File.Exists(IEX_TRACERCONFIG) Then
                Dim TracerContent As String = File.ReadAllText(IEX_TRACERCONFIG)

                If WithMilestones Then
                    TracerContent = TracerContent.Replace(Chr(34) + "1" + Chr(34), Chr(34) + "0" + Chr(34))
                    TracerContent = TracerContent.Replace("WARNING_OUTPUT_MASK", "APP_OUTPUT_MASK")
                    TracerContent = TracerContent.Replace("DEBUG_ONBOOT=" + Chr(34) + "0" + Chr(34), "DEBUG_ONBOOT=" + Chr(34) + "1" + Chr(34))
                    TracerContent = TracerContent.Replace("DEBUG_IEX" + Chr(34) + " enable=" + Chr(34) + "0" + Chr(34), "DEBUG_IEX" + Chr(34) + " enable=" + Chr(34) + "1" + Chr(34))
                Else
                    TracerContent = TracerContent.Replace(Chr(34) + "1" + Chr(34), Chr(34) + "0" + Chr(34))
                End If

                File.WriteAllText(IEX_TRACERCONFIG, TracerContent)
                Return True
            End If
            Return False
        Catch ex As Exception
            Return False
        End Try
    End Function

    Private Function RemoveReadonly(ByVal sPath As String) As Boolean
        Try
            For Each tfile As String In Directory.GetFiles(sPath, "*.*", IO.SearchOption.AllDirectories)
                If File.GetAttributes(tfile) = FileAttributes.ReadOnly Or (FileAttributes.ReadOnly + FileAttributes.Archive) Then
                    File.SetAttributes(tfile, FileAttributes.Normal)
                End If
            Next

            Return True
        Catch ex As Exception
            Return False
        End Try
    End Function

    Private Function CopyFile(ByVal ffrom As String, ByVal fTo As String) As Boolean
        Try

            File.Copy(ffrom, fTo, True)
            Return True

        Catch ex As Exception
            MsgBox(ex.Message)
            Return False
        End Try

    End Function

    Private Function CopyDirectory(ByVal Src As String, ByVal Dst As String, Optional ByVal IsXML As Boolean = False) As Boolean
        Try
            If IsXML Then

                Try
                    Directory.CreateDirectory(Dst)
                Catch ex As Exception
                End Try

                For Each newPath As String In Directory.GetFiles(Src, "*.xml", SearchOption.TopDirectoryOnly)
                    File.Copy(newPath, newPath.Replace(Src, Dst), True)
                Next

            Else
                Try
                    Directory.CreateDirectory(Dst)
                Catch ex As Exception
                End Try

                For Each dirPath As String In Directory.GetDirectories(Src, "*", SearchOption.AllDirectories)
                    Directory.CreateDirectory(dirPath.Replace(Src, Dst))
                Next

                For Each newPath As String In Directory.GetFiles(Src, "*.*", SearchOption.AllDirectories)
                    File.Copy(newPath, newPath.Replace(Src, Dst), True)
                Next
            End If

        Catch ex As Exception
            MsgBox("Exception Occured : " + ex.Message.ToString)
            Return False
        End Try

        Return True
    End Function

    Private Function CopyAllFiles(ByVal From As String, ByVal fTo As String) As Boolean
        If Directory.Exists(fTo) = False Then
            Directory.CreateDirectory(fTo)
        End If

        Dim sfiles As String() = Directory.GetFiles(From, "*.*")
        Try

            For Each sfile As String In sfiles
                File.Copy(sfile, fTo + "\" + sfile.Substring(sfile.LastIndexOf("\") + 1, sfile.Length - (sfile.LastIndexOf("\") + 1)), True)
            Next
            Return True
        Catch ex As Exception
            MsgBox(ex.Message)
            Return False
        End Try
    End Function

    Private Function CopyFiles(ByVal From As String, ByVal fTo As String) As Boolean
        If Directory.Exists(fTo) = False Then
            Directory.CreateDirectory(fTo)
        End If

        Dim sfiles As String() = Directory.GetFiles(From, "*.dll")
        Try

            For Each sfile As String In sfiles
                File.Copy(sfile, fTo + "\" + sfile.Substring(sfile.LastIndexOf("\") + 1, sfile.Length - (sfile.LastIndexOf("\") + 1)), True)
            Next
            Return True
        Catch ex As Exception
            MsgBox(ex.Message)
            Return False
        End Try
    End Function

    Private Sub Form1_FormClosing(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
        If Directory.Exists("C:\Temp") = False Then
            Directory.CreateDirectory("C:\Temp")
        End If
        If File.Exists("C:\Temp\DeployerConfig.txt") Then
            File.Delete("C:\Temp\DeployerConfig.txt")
        End If

        Dim sFile As String = ""

        sFile = TxtUserName.Text + "," + UpcGateway + "," + UpcGWIEXNumber + "," + UpcClient + "," + UpcClIEXNumber + _
                IIf(UpcTGShortStream, ",TRUE", ",FALSE") + IIf(UpcIEXTGStream, ",TRUE", ",FALSE") + IIf(UpcOriginalStream, ",TRUE", ",FALSE") + "," + _
                CogecoGateway + "," + CogecoGWIEXNumber + "," + CogecoClient + "," + CogecoClIEXNumber + _
                IIf(CogecoTGShortStream, ",TRUE", ",FALSE") + IIf(CogecoIEXTGStream, ",TRUE", ",FALSE") + IIf(CogecoOriginalStream, ",TRUE", ",FALSE") + "," + _
                TxtFrom.Text + "," + TxtGwBuildPath.Text + "," + ManualGWIEXNumber + "," + TxtClBuildPath.Text + "," + ManualClIEXNumber + _
                IIf(ManualTGShortStream, ",TRUE", ",FALSE") + IIf(ManualIEXTGStream, ",TRUE", ",FALSE") + IIf(ManualOriginalStream, ",TRUE", ",FALSE") + _
                IIf(RadUPC.Checked, ",TRUE", ",FALSE") + IIf(RadCOGECO.Checked, ",TRUE", ",FALSE") + "," + _
                UserName + "," + Password + "," + GwNumber + "," + TabProject.SelectedIndex.ToString

        File.WriteAllText("C:\Temp\DeployerConfig.txt", sFile)
    End Sub

    Private Sub Form1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        LblStatus.Visible = False
        If File.Exists("C:\Temp\DeployerConfig.txt") Then
            Dim Fields As String() = File.ReadAllText("C:\Temp\DeployerConfig.txt").Split(",")

            TxtUserName.Text = Fields(0)

            'UPC TAB
            Try
                UpcGateway = IIf(Fields(1) = "", "Unavailable", Fields(1))
            Catch ex As Exception
                UpcGateway = "Unavailable"
            End Try
            Try
                UpcGWIEXNumber = Fields(2)
            Catch ex As Exception
                UpcGWIEXNumber = "1"
            End Try
            Try
                UpcClient = IIf(Fields(3) = "", "Unavailable", Fields(3))
            Catch ex As Exception
                UpcClient = "Unavailable"
            End Try
            Try
                UpcClIEXNumber = Fields(4)
            Catch ex As Exception
                UpcClIEXNumber = "1"
            End Try
            Try
                UpcTGShortStream = CBool(Fields(5))
            Catch ex As Exception
                UpcTGShortStream = False
            End Try
            Try
                UpcIEXTGStream = CBool(Fields(6))
            Catch ex As Exception
                UpcIEXTGStream = False
            End Try
            Try
                UpcOriginalStream = CBool(Fields(7))
            Catch ex As Exception
                UpcOriginalStream = False
            End Try

            'COGECO TAB
            Try
                CogecoGateway = IIf(Fields(8) = "", "Unavailable", Fields(8))
            Catch ex As Exception
                CogecoGateway = "Unavailable"
            End Try
            Try
                CogecoGWIEXNumber = Fields(9)
            Catch ex As Exception
                CogecoGWIEXNumber = "1"
            End Try
            Try
                CogecoClient = IIf(Fields(10) = "", "Unavailable", Fields(10))
            Catch ex As Exception
                CogecoClient = "Unavailable"
            End Try
            Try
                CogecoClIEXNumber = Fields(11)
            Catch ex As Exception
                CogecoClIEXNumber = "1"
            End Try
            Try
                CogecoTGShortStream = CBool(Fields(12))
            Catch ex As Exception
                CogecoTGShortStream = False
            End Try
            Try
                CogecoIEXTGStream = CBool(Fields(13))
            Catch ex As Exception
                CogecoIEXTGStream = False
            End Try
            Try
                CogecoOriginalStream = CBool(Fields(14))
            Catch ex As Exception
                CogecoOriginalStream = False
            End Try

            'MANUAL TAB

            Try
                TxtFrom.Text = Fields(15)
            Catch ex As Exception
                TxtFrom.Text = ""
            End Try
            Try
                TxtGwBuildPath.Text = Fields(16)
            Catch ex As Exception
                TxtGwBuildPath.Text = ""
            End Try
            Try
                ManualGWIEXNumber = Fields(17)
            Catch ex As Exception
                ManualGWIEXNumber = "9"
            End Try
            Try
                TxtClBuildPath.Text = Fields(18)
            Catch ex As Exception
                TxtClBuildPath.Text = ""
            End Try
            Try
                ManualClIEXNumber = Fields(19)
            Catch ex As Exception
                ManualClIEXNumber = "9"
            End Try
            Try
                ManualTGShortStream = CBool(Fields(20))
            Catch ex As Exception
                ManualTGShortStream = False
            End Try
            Try
                ManualIEXTGStream = CBool(Fields(21))
            Catch ex As Exception
                ManualIEXTGStream = False
            End Try
            Try
                ManualOriginalStream = CBool(Fields(22))
            Catch ex As Exception
                ManualOriginalStream = False
            End Try
            Try
                RadUPC.Checked = CBool(Fields(23))
            Catch ex As Exception
                RadUPC.Checked = False
            End Try
            Try
                RadCOGECO.Checked = CBool(Fields(24))
            Catch ex As Exception
                RadCOGECO.Checked = False
            End Try
         
           
            Try
                UserName = Fields(25)
            Catch ex As Exception
                UserName = ""
            End Try
            Try
                Password = Fields(26)
            Catch ex As Exception
                Password = ""
            End Try
            Try
                GwNumber = Fields(27)
            Catch ex As Exception
                GwNumber = ""
            End Try
            Try
                TabProject.SelectedIndex = CInt(Fields(28))
            Catch ex As Exception
                TabProject.SelectedIndex = 0
            End Try
          
        End If

        Select Case TabProject.SelectedIndex
            Case 0
                GrpClient.Visible = False
                GrpBuilds.Visible = True
                LblUserName.Visible = True
                TxtUserName.Visible = True
                ButDeploy.Visible = True
                Project = "UPC"
                LblCurGateway.Text = UpcGateway
                LblCurClient.Text = UpcClient
                CmbGwIEXNumber.Text = UpcGWIEXNumber
                CmbClIEXNumber.Text = UpcClIEXNumber
                RadTG.Checked = UpcTGShortStream
                RadIEXTG.Checked = UpcIEXTGStream
                RadOriginal.Checked = UpcOriginalStream
                IEX_INI_FILES_Dest = "C:\Program Files\IEX\Tests\TestsINI\IEX" + UpcGWIEXNumber + "\"
                UpdateBuilds()
            Case 1
                GrpClient.Visible = False
                GrpBuilds.Visible = True
                LblUserName.Visible = True
                TxtUserName.Visible = True
                ButDeploy.Visible = True
                Project = "COGECO"
                LblCurGateway.Text = CogecoGateway
                LblCurClient.Text = CogecoClient
                CmbGwIEXNumber.Text = CogecoGWIEXNumber
                CmbClIEXNumber.Text = CogecoClIEXNumber
                RadTG.Checked = CogecoTGShortStream
                RadIEXTG.Checked = CogecoIEXTGStream
                RadOriginal.Checked = CogecoOriginalStream
                IEX_INI_FILES_Dest = "C:\Program Files\IEX\Tests\TestsINI\IEX" + CogecoGWIEXNumber + "\"
                UpdateBuilds()
            Case 2
                GrpClient.Visible = True
                GrpBuilds.Visible = False
                LblUserName.Visible = False
                TxtUserName.Visible = False
                ButDeploy.Visible = False
                CmbMGwIEXNumber.Text = ManualGWIEXNumber
                CmbMClIEXNumber.Text = ManualClIEXNumber
                RadTG.Checked = ManualTGShortStream
                RadIEXTG.Checked = ManualIEXTGStream
                RadOriginal.Checked = ManualOriginalStream
                TabChange = False
        End Select

        Try
            CmbGateway.SelectedIndex = 0
        Catch ex As Exception
        End Try

        Try
            CmbClient.SelectedIndex = 0
        Catch ex As Exception
        End Try
    End Sub

    Private Sub Timer1_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Timer1.Tick
        UpdateBuilds()
    End Sub

    Private Sub UpdateBuilds()
        Try
            Dim Dic As New Dictionary(Of DateTime, String)
            Dim GatewayDirectory As DirectoryInfo
            Dim ClientDirectory As DirectoryInfo

            CmbGateway.Items.Clear()
            CmbClient.Items.Clear()

            If TabProject.SelectedIndex = 0 Then
                GatewayDirectory = New DirectoryInfo("\\10.62.14.241\extra\mnt\users\IEX\CI\UPC\gateway")
                ClientDirectory = New DirectoryInfo("\\10.62.14.241\extra\mnt\users\IEX\CI\UPC\client")
            Else
                GatewayDirectory = New DirectoryInfo("\\pooh\home_nfs\IEX\CI\COGECO\HR007\")
                ClientDirectory = New DirectoryInfo("\\pooh\home_nfs\IEX\CI\COGECO\HR007\")
            End If


            For Each d As DirectoryInfo In GatewayDirectory.GetDirectories
                Try
                    Dic.Add(d.CreationTime, d.Name)
                Catch ex As Exception
                End Try
            Next

            For i As Integer = 0 To Dic.Count - 1
                Dim Lowest As Integer = 0
                Dim Item As String = ""
                Dim FirstDate As New Date(2000, 1, 1)
                For Each it As Date In Dic.Keys
                    If DateDiff(DateInterval.Minute, it, FirstDate) < 0 Then
                        FirstDate = it
                        Item = Dic(FirstDate)
                    End If
                Next
                Dic.Remove(FirstDate)
                CmbGateway.Items.Add(Item)
            Next

            Dic = New Dictionary(Of DateTime, String)

            For Each d As DirectoryInfo In ClientDirectory.GetDirectories
                Try
                    Dic.Add(d.CreationTime, d.Name)
                Catch ex As Exception
                End Try
            Next

            For i As Integer = 0 To Dic.Count - 1
                Dim Lowest As Integer = 0
                Dim Item As String = ""
                Dim FirstDate As New Date(2000, 1, 1)
                For Each it As Date In Dic.Keys
                    If DateDiff(DateInterval.Minute, it, FirstDate) < 0 Then
                        FirstDate = it
                        Item = Dic(FirstDate)
                    End If
                Next
                Dic.Remove(FirstDate)
                CmbClient.Items.Add(Item)
            Next

            Try
                CmbGateway.SelectedIndex = 0
            Catch ex As Exception
            End Try

            Try
                CmbClient.SelectedIndex = 0
            Catch ex As Exception
            End Try

        Catch ex As Exception
        End Try
    End Sub

    Private Sub TabProject_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles TabProject.SelectedIndexChanged
        TabChange = True
        If TabProject.SelectedIndex = 0 Then
            GrpClient.Visible = False
            GrpBuilds.Visible = True
            LblUserName.Visible = True
            TxtUserName.Visible = True
            ButDeploy.Visible = True
            Project = "UPC"
            LblCurGateway.Text = UpcGateway
            LblCurClient.Text = UpcClient
            CmbGwIEXNumber.Text = UpcGWIEXNumber
            CmbClIEXNumber.Text = UpcClIEXNumber
            RadTG.Checked = UpcTGShortStream
            RadIEXTG.Checked = UpcIEXTGStream
            RadOriginal.Checked = UpcOriginalStream
            TxtLog.Text = ""
        ElseIf TabProject.SelectedIndex = 1 Then
            GrpClient.Visible = False
            GrpBuilds.Visible = True
            LblUserName.Visible = True
            TxtUserName.Visible = True
            ButDeploy.Visible = True
            Project = "COGECO"
            LblCurGateway.Text = CogecoGateway
            LblCurClient.Text = CogecoClient
            CmbGwIEXNumber.Text = CogecoGWIEXNumber
            CmbClIEXNumber.Text = CogecoClIEXNumber
            RadTG.Checked = CogecoTGShortStream
            RadIEXTG.Checked = CogecoIEXTGStream
            RadOriginal.Checked = CogecoOriginalStream
            TxtLog.Text = ""
        Else
            GrpClient.Visible = True
            GrpBuilds.Visible = False
            LblUserName.Visible = False
            TxtUserName.Visible = False
            ButDeploy.Visible = False
            CmbGwIEXNumber.Text = ManualGWIEXNumber
            CmbClIEXNumber.Text = ManualClIEXNumber
            RadTG.Checked = ManualTGShortStream
            RadIEXTG.Checked = ManualIEXTGStream
            RadOriginal.Checked = ManualOriginalStream
            TabChange = False
            TxtLog.Text = ""
            Exit Sub
        End If

        UpdateBuilds()

        TabChange = False
    End Sub

    Private Sub RadTG_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles RadTG.CheckedChanged, RadOriginal.CheckedChanged, RadIEXTG.CheckedChanged
        If TabChange = False Then
            If TabProject.SelectedIndex = 0 Then
                UpcTGShortStream = RadTG.Checked
                UpcIEXTGStream = RadIEXTG.Checked
                UpcOriginalStream = RadOriginal.Checked
            ElseIf TabProject.SelectedIndex = 1 Then
                CogecoTGShortStream = RadTG.Checked
                CogecoIEXTGStream = RadIEXTG.Checked
                CogecoOriginalStream = RadOriginal.Checked
            ElseIf TabProject.SelectedIndex = 2 Then
                ManualTGShortStream = RadTG.Checked
                ManualIEXTGStream = RadIEXTG.Checked
                ManualOriginalStream = RadOriginal.Checked
            End If
        End If
    End Sub

    Private Sub ButGateway_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ButGateway.Click
        If Project = "UPC" Then
            DeployUPCGWBuild()
        ElseIf Project = "COGECO" Then
            DeployCOGECOGWBuild()
        End If
    End Sub

    Private Sub ButClient_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ButClient.Click
        If Project = "UPC" Then
            DeployUPCCLBuild()
        ElseIf Project = "COGECO" Then
            DeployCOGECOCLBuild()
        End If
    End Sub

    Private Sub DeployUPCGWBuild()
        Dim _ssh As New SSHM.SSHManager
        Dim env As New SSHM.Objects.EnvironmentParameters
        Dim Data As String = ""

        TxtLog.Text = ""

        LblStatus.Text = "Deploying"
        LblStatus.ForeColor = Color.Black
        LblStatus.Visible = True
        Application.DoEvents()
        LblStatus.Refresh()

        env.UnixServerName = "10.62.14.241"
        env.LoginName = "stb"
        env.UserName = "stb"
        env.Password = "STB1q2w3e4r"
        env.TimeOut = 30

        TxtLog.Text += "Doing Telnet Login To DMZ.............."
        Application.DoEvents()

        If Not _ssh.Login(env) Then
            _ssh.SSHLogOff()
            LblStatus.Text = "Failed"
            LblStatus.ForeColor = Color.Red
            LblStatus.Visible = True
            MsgBox("Failed To Login To DMZ")
            Exit Sub
        End If

        TxtLog.Text += "PASSED" + vbCrLf
        Application.DoEvents()

        TxtLog.Text += "Executing Copy_CI_Build.sh.............."
        Application.DoEvents()
        'Stab Problem With SSH
        _ssh.SendCommandExpected("cd", "", Data)
        If Not _ssh.SendCommandExpected("export PS1=IEX""->""", "IEX->", Data) Then
            _ssh.SSHLogOff()
            LblStatus.Text = "Failed"
            LblStatus.ForeColor = Color.Red
            LblStatus.Visible = True
            MsgBox("Failed To Set Prompt To IEX-> DMZ data :" + Data)
            Exit Sub
        End If

        If Not _ssh.SendCommandExpected("cd /extra/mnt/users/IEX/CI", "IEX->", Data) Then
            _ssh.SSHLogOff()
            LblStatus.Text = "Failed"
            LblStatus.ForeColor = Color.Red
            LblStatus.Visible = True
            MsgBox("Failed To cd /extra/mnt/users/IEX/CI On DMZ")
            Exit Sub
        End If

        Dim Platform As String = ""
        If TabProject.SelectedIndex = 0 Then
            UpcGateway = CmbGateway.Text
            UpcGWIEXNumber = CmbGwIEXNumber.Text
            Platform = "upc_gateway"
        Else
            CogecoGateway = CmbGateway.Text
            CogecoGWIEXNumber = CmbGwIEXNumber.Text
            Platform = "cogeco_gateway"
        End If

        If Not _ssh.SendCommandExpected("sudo ./Copy_CI_Build.sh " + Platform + " IEX" + CmbGwIEXNumber.Text + " " + CmbGateway.Text + " " + TxtUserName.Text, "IEX->", Data) Then
            _ssh.SSHLogOff()
            LblStatus.Text = "Failed"
            LblStatus.ForeColor = Color.Red
            LblStatus.Visible = True
            MsgBox("Failed To Run Copy CI Script On DMZ")
            Exit Sub
        End If

        _ssh.SSHLogOff()

        If Directory.Exists("\\10.62.14.241\extra\mnt\users\" + TxtUserName.Text + "\IEX" + CmbGwIEXNumber.Text + "\NDS") = False Then
            LblStatus.Text = "Failed"
            TxtLog.Text += "FAILED" + vbCrLf
            TxtLog.Text += "Failed To Run Copy CI Script On DMZ Data Returned : " + Data.ToString
            Application.DoEvents()
            LblStatus.ForeColor = Color.Red
            LblStatus.Visible = True
            Exit Sub
        End If

        TxtLog.Text += "PASSED" + vbCrLf

        LblStatus.Text = "Success"
        LblStatus.ForeColor = Color.Green
        LblStatus.Visible = True

        LblCurGateway.Text = UpcGateway

    End Sub

    Private Sub DeployCOGECOGWBuild()
        Dim _ssh As New SSHM.SSHManager
        Dim env As New SSHM.Objects.EnvironmentParameters
        Dim Data As String = ""
        Dim PassedLogin As Boolean = False

        GwNumber = CmbGwIEXNumber.Text
        TxtLog.Text = ""

        LblStatus.Text = "Deploying"
        LblStatus.ForeColor = Color.Black
        LblStatus.Visible = True
        Application.DoEvents()
        LblStatus.Refresh()

        TxtLog.Text += "Doing Telnet Login To Pooh.............."
        Application.DoEvents()

        For i As Integer = 0 To 2
            env.UnixServerName = "10.63.2.10"
            env.LoginName = UserName
            env.UserName = UserName
            env.Password = Password
            env.TimeOut = 30

            If Not _ssh.Login(env) Then
                Try
                    _ssh.SSHLogOff()
                Catch ex As Exception
                End Try
                My.Forms.Form2.ShowDialog(Me)
            Else
                PassedLogin = True
            End If
        Next

        If PassedLogin = False Then
            LblStatus.Text = "Failed"
            LblStatus.ForeColor = Color.Red
            LblStatus.Visible = True
            Exit Sub
        End If

        TxtLog.Text += "PASSED" + vbCrLf
        Application.DoEvents()

        TxtLog.Text += "Executing Copy_CI_Build.sh.............."
        Application.DoEvents()
        'Stab Problem With SSH
        _ssh.SendCommandExpected("pwd", "", Data)
        If Not _ssh.SendCommandExpected("cd /extra/nfs/IEX/CI", "/extra/nfs/IEX/CI", Data) Then
            _ssh.SSHLogOff()
            LblStatus.Text = "Failed"
            LblStatus.ForeColor = Color.Red
            LblStatus.Visible = True
            MsgBox("Failed To cd /extra/nfs/IEX/CI On Pooh")
            Exit Sub
        End If

        Dim Platform As String = ""

        CogecoGateway = CmbGateway.Text
        CogecoGWIEXNumber = CmbGwIEXNumber.Text
        Platform = "cogeco_gateway"

        If Not _ssh.SendCommandExpected("./Copy_CI_Build.sh " + Platform + " IEX" + CmbGwIEXNumber.Text + " " + CmbGateway.Text + " " + TxtUserName.Text, "/extra/nfs/IEX/CI", Data) Then
            _ssh.SSHLogOff()
            LblStatus.Text = "Failed"
            LblStatus.ForeColor = Color.Red
            LblStatus.Visible = True
            MsgBox("Failed To Run Copy CI Script On Pooh")
            Exit Sub
        End If

        _ssh.SSHLogOff()

        If Directory.Exists("\\pooh\home_nfs\" + TxtUserName.Text + "\IEX" + CmbGwIEXNumber.Text + "\NDS") = False Then
            LblStatus.Text = "Failed"
            TxtLog.Text += "FAILED" + vbCrLf
            TxtLog.Text += "Failed To Run Copy CI Script On Pooh Data Returned : " + Data.ToString
            Application.DoEvents()
            LblStatus.ForeColor = Color.Red
            LblStatus.Visible = True
            Exit Sub
        End If

        TxtLog.Text += "PASSED" + vbCrLf

        LblStatus.Text = "Success"
        LblStatus.ForeColor = Color.Green
        LblStatus.Visible = True
        LblCurGateway.Text = CogecoGateway
    End Sub

    Private Sub DeployUPCCLBuild()
        Dim _ssh As New SSHM.SSHManager
        Dim env As New SSHM.Objects.EnvironmentParameters
        Dim Data As String = ""

        TxtLog.Text = ""

        LblStatus.Text = "Deploying"
        LblStatus.ForeColor = Color.Black
        LblStatus.Visible = True
        Application.DoEvents()
        LblStatus.Refresh()

        env.UnixServerName = "10.62.14.241"
        env.LoginName = "stb"
        env.UserName = "stb"
        env.Password = "STB1q2w3e4r"
        env.TimeOut = 30

        TxtLog.Text += "Doing Telnet Login To DMZ.............."
        Application.DoEvents()

        If Not _ssh.Login(env) Then
            _ssh.SSHLogOff()
            LblStatus.Text = "Failed"
            LblStatus.ForeColor = Color.Red
            LblStatus.Visible = True
            MsgBox("Failed To Login To DMZ")
            Exit Sub
        End If

        TxtLog.Text += "PASSED" + vbCrLf
        Application.DoEvents()

        TxtLog.Text += "Executing Copy_CI_Build.sh.............."
        Application.DoEvents()
        'Stab Problem With SSH
        _ssh.SendCommandExpected("cd", "", Data)
        If Not _ssh.SendCommandExpected("export PS1=IEX""->""", "IEX->", Data) Then
            _ssh.SSHLogOff()
            LblStatus.Text = "Failed"
            LblStatus.ForeColor = Color.Red
            LblStatus.Visible = True
            MsgBox("Failed To Set Prompt To IEX-> DMZ data :" + Data)
            Exit Sub
        End If

        If Not _ssh.SendCommandExpected("cd /extra/mnt/users/IEX/CI", "IEX->", Data) Then
            _ssh.SSHLogOff()
            LblStatus.Text = "Failed"
            LblStatus.ForeColor = Color.Red
            LblStatus.Visible = True
            MsgBox("Failed To cd /extra/mnt/users/IEX/CI On DMZ")
            Exit Sub
        End If

        Dim Platform As String = ""
        If TabProject.SelectedIndex = 0 Then
            UpcClient = CmbClient.Text
            UpcClIEXNumber = CmbClIEXNumber.Text
            Platform = "upc_client"
        Else
            CogecoClient = CmbClient.Text
            CogecoGWIEXNumber = CmbGwIEXNumber.Text
            Platform = "cogeco_client"
        End If

        If Not _ssh.SendCommandExpected("sudo ./Copy_CI_Build.sh " + Platform + " IEX" + CmbClIEXNumber.Text + " " + CmbClient.Text + " " + TxtUserName.Text, "IEX->", Data) Then
            _ssh.SSHLogOff()
            LblStatus.Text = "Failed"
            LblStatus.ForeColor = Color.Red
            LblStatus.Visible = True
            MsgBox("Failed To Run Copy CI Script On DMZ")
            Exit Sub
        End If

        _ssh.SSHLogOff()

        If Directory.Exists("\\10.62.14.241\extra\mnt\users\" + TxtUserName.Text + "\IEX" + CmbClIEXNumber.Text + "\NDS") = False Then
            LblStatus.Text = "Failed"
            TxtLog.Text += "FAILED" + vbCrLf
            TxtLog.Text += "Failed To Run Copy CI Script On DMZ Data Returned : " + Data.ToString
            Application.DoEvents()
            LblStatus.ForeColor = Color.Red
            LblStatus.Visible = True
            Exit Sub
        End If

        TxtLog.Text += "PASSED" + vbCrLf

        LblStatus.Text = "Success"
        LblStatus.ForeColor = Color.Green
        LblStatus.Visible = True
        LblCurClient.Text = UpcClient
    End Sub

    Private Sub DeployCOGECOCLBuild()
        Dim _ssh As New SSHM.SSHManager
        Dim env As New SSHM.Objects.EnvironmentParameters
        Dim Data As String = ""
        Dim PassedLogin As Boolean = False

        TxtLog.Text = ""

        LblStatus.Text = "Deploying"
        LblStatus.ForeColor = Color.Black
        LblStatus.Visible = True
        Application.DoEvents()
        LblStatus.Refresh()

        TxtLog.Text += "Doing Telnet Login To Pooh.............."
        Application.DoEvents()

        For i As Integer = 0 To 2
            env.UnixServerName = "10.63.2.10"
            env.LoginName = UserName
            env.UserName = UserName
            env.Password = Password
            env.TimeOut = 30

            If Not _ssh.Login(env) Then
                Try
                    _ssh.SSHLogOff()
                Catch ex As Exception
                End Try
                My.Forms.Form2.ShowDialog(Me)
            Else
                PassedLogin = True
            End If
        Next

        If PassedLogin = False Then
            LblStatus.Text = "Failed"
            LblStatus.ForeColor = Color.Red
            LblStatus.Visible = True
            Exit Sub
        End If

        TxtLog.Text += "PASSED" + vbCrLf
        Application.DoEvents()

        TxtLog.Text += "Executing Copy_CI_Build.sh.............."
        Application.DoEvents()
        'Stab Problem With SSH
        _ssh.SendCommandExpected("pwd", "", Data)
        If Not _ssh.SendCommandExpected("cd /extra/nfs/IEX/CI", "/extra/nfs/IEX/CI", Data) Then
            _ssh.SSHLogOff()
            LblStatus.Text = "Failed"
            LblStatus.ForeColor = Color.Red
            LblStatus.Visible = True
            MsgBox("Failed To cd /extra/nfs/IEX/CI On Pooh")
            Exit Sub
        End If

        Dim Platform As String = ""

        CogecoClient = CmbClient.Text
        CogecoClIEXNumber = CmbClIEXNumber.Text
        Platform = "cogeco_client"


        If Not _ssh.SendCommandExpected("./Copy_CI_Build.sh " + Platform + " IEX" + CmbClIEXNumber.Text + " " + CmbClient.Text + " " + TxtUserName.Text, "/extra/nfs/IEX/CI", Data) Then
            _ssh.SSHLogOff()
            LblStatus.Text = "Failed"
            LblStatus.ForeColor = Color.Red
            LblStatus.Visible = True
            MsgBox("Failed To Run Copy CI Script On Pooh")
            Exit Sub
        End If

        _ssh.SSHLogOff()

        If Directory.Exists("\\pooh\home_nfs\" + TxtUserName.Text + "\IEX" + CmbClIEXNumber.Text + "\NDS") = False Then
            LblStatus.Text = "Failed"
            TxtLog.Text += "FAILED" + vbCrLf
            TxtLog.Text += "Failed To Run Copy CI Script On Pooh Data Returned : " + Data.ToString
            Application.DoEvents()
            LblStatus.ForeColor = Color.Red
            LblStatus.Visible = True
            Exit Sub
        End If

        TxtLog.Text += "PASSED" + vbCrLf

        LblStatus.Text = "Success"
        LblStatus.ForeColor = Color.Green
        LblStatus.Visible = True

        LblCurClient.Text = CogecoClient

    End Sub

    Private Sub ButGwDeploy_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ButGwDeploy.Click
        Dim es As New EventArgs
        es = EventArgs.Empty
        SetPaths("")
        TxtLog.Clear()

        TxtLog.Text += "Configuring IEX " + CmbGwIEXNumber.Text.ToString + "  INI .............."
        Application.DoEvents()

        If Not UpdateIEXIni(CmbMGwIEXNumber.Text) Then
            LblStatus.Text = "Failed"
            MsgBox("Error Updating IEX " + CmbMGwIEXNumber.Text.ToString + " Ini File")
            LblStatus.ForeColor = Color.Red
            LblStatus.Visible = True
            Exit Sub
        Else
            TxtLog.Text += "PASSED" + vbCrLf
            Application.DoEvents()
        End If

        TxtLog.Text += "Disabling Remote Reporting .............."
        Application.DoEvents()

        If Not DisableRemoteReporting() Then
            LblStatus.Text = "Failed"
            MsgBox("Error Disabling Remote Reporting")
            LblStatus.ForeColor = Color.Red
            LblStatus.Visible = True
            Exit Sub
        Else
            TxtLog.Text += "PASSED" + vbCrLf
            Application.DoEvents()
        End If

        TxtLog.Text += "Copying Diag .............."
        Application.DoEvents()

        If Not CopyDiag() Then
            LblStatus.Text = "Failed"
            MsgBox("Problem Copying Diag_Config.txt To " + IEX_DIAGSTREAMS + " Please Make Sure It Isn't Open !!!")
            LblStatus.ForeColor = Color.Red
            LblStatus.Visible = True
            Exit Sub
        Else
            TxtLog.Text += "PASSED" + vbCrLf
            Application.DoEvents()
        End If

        ManualGWIEXNumber = CmbMGwIEXNumber.Text
        DeployEnvironment(CmbMGwIEXNumber.Text, ToSetPaths:=False)
    End Sub

    Private Sub ButClDeploy_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ButClDeploy.Click
        Dim es As New EventArgs
        es = EventArgs.Empty
        SetPaths("", False)
        TxtLog.Clear()

        TxtLog.Text += "Configuring IEX " + CmbClIEXNumber.ToString + "  INI .............."
        Application.DoEvents()

        If Not UpdateIEXIni(CmbMClIEXNumber.Text) Then
            LblStatus.Text = "Failed"
            MsgBox("Error Updating IEX " + CmbMClIEXNumber.ToString + " Ini File")
            LblStatus.ForeColor = Color.Red
            LblStatus.Visible = True
            Exit Sub
        Else
            TxtLog.Text += "PASSED" + vbCrLf
            Application.DoEvents()
        End If

        TxtLog.Text += "Disabling Remote Reporting .............."
        Application.DoEvents()

        If Not DisableRemoteReporting() Then
            LblStatus.Text = "Failed"
            MsgBox("Error Disabling Remote Reporting")
            LblStatus.ForeColor = Color.Red
            LblStatus.Visible = True
            Exit Sub
        Else
            TxtLog.Text += "PASSED" + vbCrLf
            Application.DoEvents()
        End If

        TxtLog.Text += "Copying Diag .............."
        Application.DoEvents()

        If Not CopyDiag() Then
            LblStatus.Text = "Failed"
            MsgBox("Problem Copying Diag_Config.txt To " + IEX_DIAGSTREAMS + " Please Make Sure It Isn't Open !!!")
            LblStatus.ForeColor = Color.Red
            LblStatus.Visible = True
            Exit Sub
        Else
            TxtLog.Text += "PASSED" + vbCrLf
            Application.DoEvents()
        End If
        ManualClIEXNumber = CmbMClIEXNumber.Text
        DeployEnvironment(CmbMClIEXNumber.Text, ToSetPaths:=False)
    End Sub

    Private Sub RadUPC_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles RadUPC.CheckedChanged
        If RadUPC.Checked Then
            LblClient.Visible = False
            TxtClBuildPath.Visible = False
            ButClDeploy.Visible = False
            LblClientIEXNumber.Visible = False
            CmbMClIEXNumber.Visible = False
            Project = "UPC"
        End If
    End Sub

    Private Sub RadCOGECO_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles RadCOGECO.CheckedChanged
        If RadCOGECO.Checked Then
            LblClient.Visible = True
            TxtClBuildPath.Visible = True
            ButClDeploy.Visible = True
            LblClientIEXNumber.Visible = True
            CmbMClIEXNumber.Visible = True
            Project = "COGECO"
        End If
    End Sub

    Private Sub ChkClean_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ChkClean.CheckedChanged
        If ChkClean.Checked Then
            Dim res As MsgBoxResult = MsgBox("Are You Sure You Want To Deploy As Clean Machine ?", MsgBoxStyle.OkCancel, "Deploy To Clean Machine Warnning")
            If res = MsgBoxResult.Cancel Then
                ChkClean.Checked = False
                Exit Sub
            End If
        End If
    End Sub

    Private Sub CmbGwIEXNumber_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CmbGwIEXNumber.SelectedIndexChanged
        Select Case TabProject.SelectedIndex
            Case 0
                UpcGWIEXNumber = CmbGwIEXNumber.Text
            Case 1
                CogecoGWIEXNumber = CmbGwIEXNumber.Text
        End Select
    End Sub

    Private Sub CmbClIEXNumber_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CmbClIEXNumber.SelectedIndexChanged
        Select Case TabProject.SelectedIndex
            Case 0
                UpcClIEXNumber = CmbClIEXNumber.Text
            Case 1
                CogecoClIEXNumber = CmbClIEXNumber.Text
        End Select
    End Sub

    Private Sub CmbMGwIEXNumber_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CmbMGwIEXNumber.SelectedIndexChanged
        ManualGWIEXNumber = CmbMGwIEXNumber.Text
    End Sub

    Private Sub CmbMClIEXNumber_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CmbMClIEXNumber.SelectedIndexChanged
        ManualClIEXNumber = CmbMClIEXNumber.Text
    End Sub

    Private Function UpdateIEXIni(ByVal IEX_Number As String) As Boolean
        Dim iniFile As String = ""
        Dim objReader As StreamReader = Nothing
        Dim objWriter As StreamWriter
        Dim iniContent As String = ""
        Dim newIniContent As String = ""
        Dim IEXPath As String = "C:\Program Files\IEX\IEX_" + IEX_Number + "\"
        Try
            iniFile = IEXPath + "iex.ini"
            File.Copy(iniFile, IEXPath + "iex.backup", True)

            objReader = New StreamReader(iniFile)
            Do Until objReader.EndOfStream = True
                iniContent = objReader.ReadLine()
                If iniContent.Contains("IAL=") Then
                    newIniContent += "IAL=OFF" + vbCrLf
                ElseIf iniContent.Contains("DEBUG_OUTPUT=") Then
                    newIniContent += "DEBUG_OUTPUT=ON" + vbCrLf
                ElseIf iniContent.Contains("DEBUG_EPG=") Then
                    newIniContent += "DEBUG_EPG=OFF" + vbCrLf
                ElseIf iniContent.Contains("MILESTONES_EPG=") Then
                    newIniContent += "MILESTONES_EPG=ON" + vbCrLf
                ElseIf iniContent.Contains("BAUDRATE=") Then
                    newIniContent += "BAUDRATE=115200" + vbCrLf
                ElseIf iniContent.Contains("WRITE_UDP_TO_FILE=") Then
                    newIniContent += "WRITE_UDP_TO_FILE=True" + vbCrLf
                ElseIf iniContent.Contains("INSTANCE1_TELNET_IP=") Then
                    newIniContent += "INSTANCE1_TELNET_IP=127.0.0.1" + vbCrLf
                ElseIf iniContent.Contains("INSTANCE2_TELNET_IP=") Then
                    newIniContent += "INSTANCE2_TELNET_IP=127.0.0.1" + vbCrLf
                ElseIf iniContent.Contains("WAIT_BETWEEN_IR_CMD=") Then
                    newIniContent += "WAIT_BETWEEN_IR_CMD=0" + vbCrLf
                ElseIf iniContent.Contains("IR_RESPONSE_TIME=") Then
                    newIniContent += "IR_RESPONSE_TIME=0.2" + vbCrLf
                ElseIf iniContent.Contains("CONFIGURATION_FILE_PATH=") Then
                    newIniContent += "CONFIGURATION_FILE_PATH=C:\Program Files\IEX\EpgStateMachineConfiguration.xml" + vbCrLf
                ElseIf iniContent.Contains("EPG_DICTIONARY_PATH=") Then
                    newIniContent += "EPG_DICTIONARY_PATH=C:\Program Files\IEX\Tests\TestsINI\IEX" + IEX_Number + "\Dictionary\dictionary_eng.xml" + vbCrLf
                ElseIf iniContent.Contains("PROJECT_NAME=") Then
                If Project = "UPC" Then
                    newIniContent += "PROJECT_NAME=UPC_PVR" + vbCrLf
                ElseIf Project = "COGECO" Then
                    newIniContent += "PROJECT_NAME=Arris" + vbCrLf
                End If
                Else
                newIniContent += iniContent + vbCrLf
                End If
            Loop

            objReader.Close()

            If File.Exists(iniFile) Then
                File.Delete(iniFile)
            End If

            objWriter = New StreamWriter(iniFile)
            objWriter.Write(newIniContent)
            objWriter.Close()

        Catch ex As Exception
            Try
                objReader.Close()
            Catch ex1 As Exception
            End Try
            Return False
        End Try

        Return True

    End Function

    Private Function DisableRemoteReporting() As Boolean
        Dim XmlFile As String = "C:\Program Files\IEX\TracerConfiguration.xml"
        Dim objReader As StreamReader = Nothing
        Dim objWriter As StreamWriter
        Dim XmlContent As String = ""
        Dim newXmlContent As String = ""

        Try
            objReader = New StreamReader(XmlFile)
            Do Until objReader.EndOfStream = True
                XmlContent = objReader.ReadLine()
                If XmlContent.Contains("<EnableRemoteReporting>") Then
                    newXmlContent += "<EnableRemoteReporting>false</EnableRemoteReporting>" + vbCrLf
                ElseIf XmlContent.Contains("<EnableOcrReporting>") Then
                    newXmlContent += "<EnableOcrReporting>false</EnableOcrReporting>" + vbCrLf
                Else
                    newXmlContent += XmlContent + vbCrLf
                End If
            Loop

            objReader.Close()

            If File.Exists(XmlFile) Then
                File.Delete(XmlFile)
            End If

            objWriter = New StreamWriter(XmlFile)
            objWriter.Write(newXmlContent)
            objWriter.Close()

        Catch ex As Exception
            Try
                objReader.Close()
            Catch ex1 As Exception
            End Try
            Return False
        End Try

        Return True

    End Function

    Private Function CopyDiag() As Boolean
        Dim passed As Boolean
        TxtLog.Text += "Copying Diag .............."
        Application.DoEvents()
        passed = CopyFile(Application.StartupPath + "\Diag_Config.txt", IEX_DIAGSTREAMS)
        If passed = False Then
            Return False
        End If

        Return True
    End Function

    Private Sub ShowNewMachineMessage()
        MessageBox.Show("Don't Forget To Follow These Steps : " + vbCrLf + _
                "- Configure Macro Security On Microsoft Excel" + vbCrLf + _
                "- Configure IEX Equipment Coms + UDP Port On IEX.ini" + vbCrLf + _
                "- Configure Gateway IP On Telnet.ini (COGECO Only)" + vbCrLf + _
                "- Configure Environment.ini")
    End Sub

End Class
