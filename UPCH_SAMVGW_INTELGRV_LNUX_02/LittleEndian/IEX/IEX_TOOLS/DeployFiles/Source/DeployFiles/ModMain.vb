Imports System.IO
Imports System.Xml
Imports System
Imports System.Reflection

Module ModMain
    Dim RootFolder As String = ""
    Dim IEXNumber As String = ""
    Dim PlatformType As String = ""
    Dim IEXPATH As String = ""
    Dim IEX_PCAT_MODIFIER_Dest As String = ""
    Dim IEX_TESTS_Dest As String = ""
    Dim IEX_IR_Dest As String = ""
    Dim IEX_INI_FILES_Dest As String = ""
    Dim IEX_DICTIONARY_Dest As String = ""
    Dim IEX_ELEMENTARYACTIONS_Dest As String = ""
    Dim PROJECT_NAME As String = ""
    Dim RESOURCES_PATH As String = ""
    Dim BUILD_PATH As String = ""
    Dim IEX_SPMCONFIG As String = ""
    Dim IEX_FREQCONFIG As String = ""
    Dim PROJECT As String = ""
    Dim IEX_BUILDLOCATION As String = ""
    Dim RFPort As String = ""
    Dim IEX_BUILD_Dest As String = ""
    Dim SnD_Build_Path As String
    Dim FUNCTIONAL_GROUP As String = ""
    Dim BinaryType As String = ""
    Dim NumberOfMinutes As String = ""
    Dim TestName As String = ""
    Dim MemoryDump As String = ""
    Dim Target As String = ""
    Dim MaxErrors As String = ""
    Dim TestGroup As String = ""
    Dim ProcessPathDrive As String = ""
    Dim ProcessPath As String = ""
    Dim StartCommand As String = ""
    Dim ipvTestIniParam As String = ""
    Dim ImageName As String = ""
    Dim FlashBuildPath As String = ""

    Sub Main(ByVal Args As String())
        Dim Msg As String = ""
        Dim passed As Boolean = True

        If Args.Length <> 4 Then
            Msg = "Error In Arguments Please Check. Please Use DeployFiles <Path To IEX_AUTOMATION Folder> <IEX Instance Number> <Platform Type> <Build Path>"
            Console.WriteLine("Please Use DeployFiles <Path To IEX_AUTOMATION Folder> <IEX Instance Number> <Platform Type>")
            Console.WriteLine("FAIL " + Msg)
            End
        End If

        If Args(0) = "" Then
            Msg = "Error In First Argument Please Check. Please Use DeployFiles <Path To IEX_AUTOMATION Folder> <IEX Instance Number> <Platform Type> <Build Path>"
            Console.WriteLine("Error In First Argument Please Check.")
            Console.WriteLine("Please Use DeployFiles <Path To IEX_AUTOMATION Folder> <IEX Instance Number> <Platform Type>")
            Console.WriteLine("FAIL " + Msg)
            End
        Else
            RootFolder = Args(0)
            If RootFolder.EndsWith("\") = False Then
                RootFolder = RootFolder + "\"
            End If
        End If

        If Args(1) = "" Then
            Msg = "Error In Second Argument Please Check. Please Use DeployFiles <Path To IEX_AUTOMATION Folder> <IEX Instance Number> <Platform Type> <Build Path>"
            Console.WriteLine("Error In Second Argument Please Check.")
            Console.WriteLine("Please Use DeployFiles <Path To IEX_AUTOMATION Folder> <IEX Instance Number> <Platform Type>")
            Console.WriteLine("FAIL " + Msg)
            End
        Else
            IEXNumber = Args(1)
        End If

        '<MASTER OR SLAVE>
        If Args(2) = "" Then
            Msg = "Error In Third Argument Please Check. Please Use DeployFiles <Path To IEX_AUTOMATION Folder> <IEX Instance Number> <Platform Type> <Build Path>"
            Console.WriteLine("Error In Third Argument Please Check.")
            Console.WriteLine("Please Use DeployFiles <Path To IEX_AUTOMATION Folder> <IEX Instance Number> <Platform Type>")
            Console.WriteLine("FAIL " + Msg)
            End
        Else
            PlatformType = Args(2)
        End If

        If Args(3) = "" Then
            Msg = "Error In Forth Argument Please Check. Please Use DeployFiles <Path To IEX_AUTOMATION Folder> <IEX Instance Number> <Platform Type> <Build Path>"
            Console.WriteLine("Error In Third Argument Please Check.")
            Console.WriteLine("Please Use DeployFiles <Path To IEX_AUTOMATION Folder> <IEX Instance Number> <Platform Type>")
            Console.WriteLine("FAIL " + Msg)
            End
        Else
            BUILD_PATH = Args(3)
        End If

        RESOURCES_PATH = BUILD_PATH + "\resources\"
        IEX_SPMCONFIG = BUILD_PATH + "\config\spm.cfg"
        IEX_FREQCONFIG = BUILD_PATH + "\config\freqHunt.txt"


        passed = ReadDeployerIni()
        If passed = False Then
            Msg = "Error Reading Ini File From " + RootFolder + "\IEX_TOOLS\DeployFiles\Deployer.ini"
            Console.WriteLine("FAIL " + Msg)
            End
        End If

        '************************** Copy IEX_INI_FILES ************************
        Try
            passed = deleteDirectory(IEX_INI_FILES_Dest)
            If Not passed Then
                Msg = "Error Deleteing " + IEX_INI_FILES_Dest
                Console.WriteLine("FAIL " + Msg)
                End
            End If

            passed = CopyDirectory(RootFolder + "IEX_PROJECT_FILES\IEX_INI_FILES\", IEX_INI_FILES_Dest)
            If Not passed Then
                Msg = "Error Copying IEX_INI_FILES"
                Console.WriteLine("FAIL " + Msg)
                End
            End If
        Catch ex As Exception
            Msg = "Error Copying IEX_INI_FILES"
            Console.WriteLine("FAIL " + Msg)
            End
        End Try



        Try
            passed = RemoveReadonly(IEX_INI_FILES_Dest)
            If Not passed Then
                Msg = "Error Removing Readonly Attributes For IEX_INI_FILES"
                Console.WriteLine("FAIL " + Msg)
                End
            End If
        Catch ex As Exception
            Msg = "Error Removing Readonly Attributes For IEX_INI_FILES"
            Console.WriteLine("FAIL " + Msg)
            End
        End Try


        '************************** Read Environment.ini *********************
        passed = ReadEviromentIni()
        If passed = False Then
            Msg = "Error Reading Environment.Ini File From " + IEX_INI_FILES_Dest + "\Environment.ini"
            Console.WriteLine("FAIL " + Msg)
            End
        End If

        'If PROJECT.ToLower = "cogeco" Then
        '    '************************** Set Project Name ****************************
        '    PROJECT_NAME = "Arris"

        '    '************************* Configure TIG-GEN ****************************
        '    'passed = ConfigureCogecoSpm()
        '    'If passed = False Then
        '    '    Msg = "Error Configuring Cogeco spm.config"
        '    '    Console.WriteLine("FAIL " + Msg)
        '    '    End
        '    'End If
        'ElseIf PROJECT.ToLower = "upc" Then
        '    '************************** Set Project Name ****************************
        '    PROJECT_NAME = "UPC_PVR"

        '    '************************* Configure TIG-GEN ****************************
        '    passed = ConfigureTigGen()
        '    If passed = False Then
        '        Msg = "Error Configuring Stream To TIG-GEN"
        '        Console.WriteLine("FAIL " + Msg)
        '        End
        '    End If
        'ElseIf PROJECT.ToLower = "cdigital" Then
        '    PROJECT_NAME = "Kaon"
        'End If

        'IEX_IR_Dest = IEX_IR_Dest + PROJECT_NAME + "\"

        If PROJECT.ToLower = "cogeco" And PlatformType.ToLower = "master" Then
            Console.WriteLine("PASS")
            End
        End If






        ' -------------------------------------------------------- COGECO GATEWAY END -------------------------------------------------------------------------

        '*************************** Copy IEX_PCAT_MODIFIER *******************
        Try
            passed = deleteDirectory(IEX_PCAT_MODIFIER_Dest)
            If Not passed Then
                Msg = "Error Deleteing " + IEX_PCAT_MODIFIER_Dest
                Console.WriteLine("FAIL " + Msg)
                End
            End If
            passed = CopyDirectory(RootFolder + "IEX_PCAT_MODIFIER\", IEX_PCAT_MODIFIER_Dest)
            If Not passed Then
                Msg = "Error Copying IEX_PCAT_MODIFIER"
                Console.WriteLine("FAIL " + Msg)
                End
            End If
        Catch ex As Exception
            Msg = "Error Copying IEX_PCAT_MODIFIER"
            Console.WriteLine("FAIL " + Msg)
            End
        End Try

        Try
            passed = RemoveReadonly(IEX_PCAT_MODIFIER_Dest)
        Catch ex As Exception
            Msg = "Error Removing Readonly Attributes For IEX_PCAT_MODIFIER"
            Console.WriteLine("FAIL " + Msg)
            End
        End Try

        ''*************************** Copy IEX_TESTS ***************************
        'Try
        '    passed = deleteDirectory(IEX_TESTS_Dest)
        '    If Not passed Then
        '        Msg = "Error Deleteing " + IEX_TESTS_Dest
        '        Console.WriteLine("FAIL " + Msg)
        '        End
        '    End If
        '    passed = CopyDirectory(RootFolder + "IEX_PROJECT_FILES\IEX_TESTS\", IEX_TESTS_Dest)
        'Catch ex As Exception
        '    Msg = "Error Copying IEX_TESTS"
        '    Console.WriteLine("FAIL " + Msg)
        '    End
        'End Try
        'Try
        '    passed = RemoveReadonly(IEX_TESTS_Dest)
        '    If Not passed Then
        '        Msg = "Error Removing Readonly Attributes For IEX_TESTS"
        '        Console.WriteLine("FAIL " + Msg)
        '        End
        '    End If
        'Catch ex As Exception
        '    Msg = "Error Removing Readonly Attributes For IEX_TESTS"
        '    Console.WriteLine("FAIL " + Msg)
        '    End
        'End Try

        '*************************** Copy IR **************************
        'Try
        '    passed = CopyDirectory(RootFolder + "IEX_PROJECT_FILES\IEX_IR\", IEX_IR_Dest)
        '    If Not passed Then
        '        Msg = "Error Copying IR"
        '        Console.WriteLine("FAIL " + Msg)
        '        End
        '    End If
        'Catch ex As Exception
        '    Msg = "Error Copying IR"
        '    Console.WriteLine("FAIL " + Msg)
        '    End
        'End Try
        'Try
        '    passed = RemoveReadonly(IEX_IR_Dest)
        '    If Not passed Then
        '        Msg = "Error Removing Readonly Attributes For IR"
        '        Console.WriteLine("FAIL " + Msg)
        '        End
        '    End If
        'Catch ex As Exception
        '    Msg = "Error Removing Readonly Attributes For IR"
        '    Console.WriteLine("FAIL " + Msg)
        '    End
        'End Try

        '************************** Copy IEX_DICTIONARY ************************
        Try
            passed = deleteDirectory(IEX_DICTIONARY_Dest)
            If Not passed Then
                Msg = "Error Deleteing " + IEX_DICTIONARY_Dest
                Console.WriteLine("FAIL " + Msg)
                End
            End If
            passed = CopyDirectory(RootFolder + "UNITY_FLASH_EPG_UI_XML\", IEX_DICTIONARY_Dest, True)
            If Not passed Then
                Msg = "Error Copying DICTIONARY"
                Console.WriteLine("FAIL " + Msg)
                End
            End If
        Catch ex As Exception
            Msg = "Error Copying DICTIONARY"
            Console.WriteLine("FAIL " + Msg)
            End
        End Try
        Try
            passed = RemoveReadonly(IEX_DICTIONARY_Dest)
            If Not passed Then
                Msg = "Error Copying DICTIONARY"
                Console.WriteLine("FAIL " + Msg)
                End
            End If
        Catch ex As Exception
            Msg = "Error Removing Readonly Attributes For IEX_DICTIONARY"
            Console.WriteLine("FAIL " + Msg)
            End
        End Try
        '**********************SnD Robustness UPC************************
        'Fetching Functional group from Test ini for SnD Robustness scripts
        Try
            passed = ReadTestIni()
            If Not passed Then
                Msg = "Error Reading Test ini"
                Console.WriteLine("FAIL " + Msg)
                End
            End If
        Catch ex As Exception
            Msg = "Error Reading Test Ini"
            Console.WriteLine("FAIL " + Msg)
            End
        End Try

        'If the Functional group is Snd Robustness the we are copying the image locally for flashing

        If FUNCTIONAL_GROUP.ToUpper = "SNDROBUSTNESS" Then
            '************************** Copy Drivers ************************
            Try

                IEX_BUILDLOCATION = BUILD_PATH + "/config/version.cfg"
                BUILD_PATH = FlashBuildPath
                'Copies the ImageName from build path to the targetted location which later will be used for flashing
                FileCopy(BUILD_PATH + ImageName, "D:/Downloads/IEX" + IEXNumber + "/" + ImageName)
            Catch ex As Exception
                Msg = "Error Copying Drivers"
                Console.WriteLine("FAIL " + ex.Message)
                End
            End Try
            'Updates the EdIni file with logs path and test name
            SnD_updateEdini()
            ''Updates the bzimaze.xml file with the Software version and binary type
            SnD_updateBzImageXML()

            If PROJECT.ToLower = "upc" Or PROJECT.ToLower = "istb" Or PROJECT.ToLower = "ipc" Or PROJECT.ToLower = "CISCOREFRESH" Then

                SnD_copyBzImage()
            End If
        End If
        'If the Functional group is Fitness we are launching ANT start

        If TestGroup.ToUpper = "FITNESSE" Then

            StartProcess("" & ProcessPathDrive & vbCr & vbLf &
                         ProcessPath & vbCr & vbLf &
                         StartCommand, "ERROR")

        End If
        ''************************** Update Ini File ************************
        Dim IEXPATH As String = "C:\Program Files\IEX\IEX_" + IEXNumber + "\"

        Try
            passed = UpdateIEXIni(IEXPATH)
            If Not passed Then
                Msg = "Error Updating IEX Ini File"
                Console.WriteLine("FAIL " + Msg)
                End
            End If
        Catch ex As Exception
            Msg = "Error Updating IEX Ini File"
            Console.WriteLine("FAIL " + Msg)
            End
        End Try

        '************************** Configure EPG_properties.xml *****************
        passed = ConfigureEpgProperties()
        If passed = False Then
            Msg = "Error Configuring EPG_properties.xml From " + RESOURCES_PATH + "EPG_properties.xml"
            Console.WriteLine("FAIL " + Msg)
            End
        End If

        '*********************** Configure TracerConfig.xml *****************
        Try
            passed = ConfigureTracer()
            If passed Then
                Console.WriteLine("PASS")
                End
            Else
                Msg = "Error Configuring TracerConfigMap.xml From " + RESOURCES_PATH + "TracerConfigMap.xml"
                Console.WriteLine("FAIL " + Msg)
                End
            End If
        Catch ex As Exception
            Msg = "Error Configuring TracerConfigMap.xml From " + RESOURCES_PATH + "TracerConfigMap.xml"
            Console.WriteLine("FAIL " + Msg)
            End
        End Try

    End Sub

    Private Function SnD_updateEdini() As Boolean
        Dim Logspath As String
        Dim iniFile As AMS.Profile.Ini
        Dim edini_path As String
        Try
            Console.WriteLine("Inside method SnD_updateEdini() ")
            edini_path = "C:\Program Files\IEX\Tests\TestsINI\" + System.Environment.MachineName + "_IEX" + IEXNumber + "_TestsINI.xml"
            Console.WriteLine("Edini_path: " + edini_path)
            iniFile = New AMS.Profile.Ini(IEX_INI_FILES_Dest + "Environment.ini")
            Logspath = iniFile.GetValue("IEX" + IEXNumber.ToString, "LogDirectory").ToString
            ' Logspath = Logspath.Replace("\\", "\")
            Dim xdoc As XmlDocument = New XmlDocument
            xdoc.Load(edini_path)
            Dim attr As XmlAttribute
            Console.WriteLine("Logspath: " + Logspath)
            attr = xdoc.SelectSingleNode("/IEX/GlobalParameters/Param[@Name = 'ini_sImhotepLogsPath']/@Value")
            attr.Value = Logspath + "\\IEXLOG"
            Console.WriteLine("ini_sImhotepLogsPath: " + attr.Value)
            attr = xdoc.SelectSingleNode("/IEX/GlobalParameters/Param[@Name = 'ini_sCommonFolder']/@Value")
            attr.Value = Logspath
            Console.WriteLine("ini_sCommonFolder: " + attr.Value)
            attr = xdoc.SelectSingleNode("/IEX/GlobalParameters/Param[@Name = 'ini_sReportToHPQCFolderForManualReport']/@Value")
            attr.Value = Logspath
            Console.WriteLine("ini_sReportToHPQCFolderForManualReport: " + attr.Value)
            attr = xdoc.SelectSingleNode("/IEX/Test[@Name = '" + TestName + "']/Param[@Name='ini_iExpectedDurationInMin']/@Value")
            attr.Value = NumberOfMinutes
            Console.WriteLine("ini_iExpectedDurationInMin: " + attr.Value)
            attr = xdoc.SelectSingleNode("/IEX/Test[@Name = '" + TestName + "']/Param[@Name='ini_sMemoryDump']/@Value")
            attr.Value = MemoryDump
            Console.WriteLine("ini_sMemoryDump: " + attr.Value)
            attr = xdoc.SelectSingleNode("/IEX/Test[@Name = '" + TestName + "']/Param[@Name='ini_iTarget']/@Value")
            attr.Value = Target
            Console.WriteLine("ini_iTarget: " + attr.Value)
            attr = xdoc.SelectSingleNode("/IEX/Test[@Name = '" + TestName + "']/Param[@Name='ini_iMaxErrors']/@Value")
            attr.Value = MaxErrors
            Console.WriteLine("ini_iMaxErrors: " + attr.Value)

            xdoc.Save(edini_path)

        Catch ex As Exception
            Console.WriteLine("FAIL" + ex.Message)
        End Try
        Return True

    End Function
    Private Function SnD_updateBzImageXML() As Boolean
        Dim softwareVersion As String
        Dim iniFile As AMS.Profile.Ini
        Dim bzXMLPath As String
        Dim phase As String
        Dim softwareBranch As String
        'bzXMLPath = "D:/Downloads/IEX" + IEXNumber + "/" + "bzImage.xml"
        Try
            Console.WriteLine("Inside method SnD_updateBzImageXML() ")
            iniFile = New AMS.Profile.Ini(IEX_BUILDLOCATION)
            softwareVersion = iniFile.GetValue("VERSION", "NDS_SW_VERSION").ToString
            Console.WriteLine("Software version is " + softwareVersion)
            Console.WriteLine("ImageName is " + ImageName)
            bzXMLPath = "D:/Downloads/IEX" + IEXNumber + "/" + ImageName + ".xml"
            Console.WriteLine("bzXMLPath is " + bzXMLPath)
            Dim xdoc As XmlDocument = New XmlDocument
            xdoc.Load(bzXMLPath)
            Dim attrSoftwareVersion As XmlAttribute
            Dim attrBinaryType As XmlAttribute
            Dim attrSoftwareBranch As XmlAttribute
            Dim attrPhase As XmlAttribute
            attrSoftwareVersion = xdoc.SelectSingleNode("/Build/Param[@Name = 'ini_sSoftwareVersion']/@Value")
            attrSoftwareVersion.Value = softwareVersion
            attrBinaryType = xdoc.SelectSingleNode("/Build/Param[@Name = 'ini_sBinaryType']/@Value")
            If (BinaryType.StartsWith("release_SnD")) Then
                attrBinaryType.Value = "REL"
            Else
                attrBinaryType.Value = "RLDBG"
            End If

            If PROJECT.ToLower = "upc" Or PROJECT.ToLower = "istb" Or PROJECT.ToLower = "ipc" Or PROJECT.ToLower = "CISCOREFRESH" Then
                'Code to get the first two digits after the - in software version
                Dim arrphase As String() = softwareVersion.Split("-")
                phase = arrphase(1)
                Console.WriteLine("Phase " + phase)
                If phase.StartsWith("b") Then
                    phase = "Phase" + phase.Substring(1, 1) + "." + phase.Substring(2, 1)
                    softwareBranch = "BRANCH"
                Else
                    'phase = "Phase" + phase.Substring(0, 1) + "." + phase.Substring(1, 1)
                    phase = "Phase3.3"
                    softwareBranch = "MAIN_3"
                End If

                If ipvTestIniParam.Contains("IPv6") Then
                    softwareBranch = softwareBranch + "_" + ipvTestIniParam
                End If

                attrPhase = xdoc.SelectSingleNode("/Build/Param[@Name = 'ini_sPhase']/@Value")
                attrPhase.Value = phase
                attrSoftwareBranch = xdoc.SelectSingleNode("/Build/Param[@Name = 'ini_sSoftwareBranch']/@Value")
                attrSoftwareBranch.Value = softwareBranch
                End
            End If
            xdoc.Save(bzXMLPath)

        Catch ex As Exception
            Console.WriteLine("FAIL" + ex.Message)
        End Try
        Return True

    End Function


    Private Function SnD_copyBzImage() As Boolean
        Try
            Console.WriteLine("Inside method SnD_copyBzImage() ")
            Dim bzImagePath As String
            Dim bzImageXMLPath As String
            bzImagePath = "D:/IEX" + IEXNumber + "_" + ImageName
            Console.WriteLine("bzImagePath " + bzImagePath)
            bzImageXMLPath = "D:/IEX" + IEXNumber + "_" + ImageName + ".xml"
            Console.WriteLine("bzImageXMLPath " + bzImageXMLPath)
            'Copying ImageName.xml and ImageName from subfolders of D to the D folder as we are facing some issues while copying any files from Subfolders
            FileCopy("D:/Downloads/IEX" + IEXNumber + "/" + ImageName, "D:/IEX" + IEXNumber + "_" + ImageName)
            FileCopy("D:/Downloads/IEX" + IEXNumber + "/" + ImageName + ".xml", "D:/IEX" + IEXNumber + "_" + ImageName + ".xml")
            'opeing an WinScp Process and copying the Files from D folder to the Linux Machine
            Dim winscp As Process = New Process()
            winscp.StartInfo.FileName = "C:/Program Files/WinSCP/WinSCP.com" ' call winscp.com
            winscp.StartInfo.UseShellExecute = False
            winscp.StartInfo.RedirectStandardInput = True
            winscp.StartInfo.RedirectStandardOutput = True
            winscp.StartInfo.CreateNoWindow = False
            winscp.Start()
            winscp.StandardInput.WriteLine("option batch abort")
            winscp.StandardInput.WriteLine("option confirm off")
            winscp.StandardInput.WriteLine("open upchz:51038739@172.21.244.1")
            winscp.StandardInput.WriteLine("cd /vol/stb/upchz/UPC_DMZ/users/FRUPCINTEG/OfficialBinary/" + System.Environment.MachineName + "/IEX" + IEXNumber + "/")
            winscp.StandardInput.WriteLine("put D:/IEX" + IEXNumber + "_" + ImageName + ImageName)
            Console.WriteLine("put D:/IEX" + IEXNumber + "_" + ImageName + ImageName)
            winscp.StandardInput.WriteLine("put D:/IEX" + IEXNumber + "_" + ImageName + ".xml" + ImageName + ".xml")
            Console.WriteLine("put D:/IEX" + IEXNumber + "_" + ImageName + ".xml" + ImageName + ".xml")
            winscp.StandardInput.Close()
            ' Wait until WinSCP finishes
            winscp.WaitForExit()
        Catch ex As Exception
            Console.WriteLine("FAIL" + ex.Message)
        End Try
        Return True
    End Function



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
            Console.WriteLine("FAIL Exception Occured In ConfigureCogecoSpm : " + ex.Message.ToString)
            End
        End Try

    End Function

    Private Function ConfigureTigGen() As Boolean
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
                        FreqNewContent += "242000:6900000:6952000:10:8" + vbCrLf
                        First = False
                    End If

                    FreqNewContent += line + vbCrLf
                Next

                File.WriteAllText(IEX_FREQCONFIG, FreqNewContent)

                Return True
            End If

            Return False
        Catch ex As Exception
            Console.WriteLine("FAIL Exception Occured In ConfigureTigGen : " + ex.Message.ToString)
            End
        End Try
    End Function

    Private Function ConfigureEpgProperties() As Boolean
        Try

            If File.Exists(RESOURCES_PATH + "EPG_properties.xml") Then
                Dim EpgPropertiesContent As String = File.ReadAllText(RESOURCES_PATH + "EPG_properties.xml")

                EpgPropertiesContent = EpgPropertiesContent.Replace("<prop enableWizard=" + Chr(34) + "true" + Chr(34) + "></prop>", "<prop enableWizard=" + Chr(34) + "false" + Chr(34) + "></prop>")

                File.WriteAllText(RESOURCES_PATH + "EPG_properties.xml", EpgPropertiesContent)
                Return True
            Else
                Console.WriteLine("FAIL Exception Occured In ConfigureEpgProperties : EPG_properties.xml Was Not Found On " + RESOURCES_PATH)
                End
            End If

            Return False

        Catch ex As Exception
            Console.WriteLine("FAIL Exception Occured In ConfigureEpgProperties : " + ex.Message.ToString)
            End
        End Try
    End Function

    Private Function ConfigureTracer() As Boolean
        Try

            If File.Exists(RESOURCES_PATH + "TracerConfigMap.xml") Then
                Dim TracerContent As String = File.ReadAllText(RESOURCES_PATH + "TracerConfigMap.xml")

                TracerContent = TracerContent.Replace(Chr(34) + "1" + Chr(34), Chr(34) + "0" + Chr(34))
                TracerContent = TracerContent.Replace("WARNING_OUTPUT_MASK", "APP_OUTPUT_MASK")
                TracerContent = TracerContent.Replace("DEBUG_ONBOOT=" + Chr(34) + "0" + Chr(34), "DEBUG_ONBOOT=" + Chr(34) + "1" + Chr(34))
                TracerContent = TracerContent.Replace("DEBUG_IEX" + Chr(34) + " enable=" + Chr(34) + "0" + Chr(34), "DEBUG_IEX" + Chr(34) + " enable=" + Chr(34) + "1" + Chr(34))

                File.WriteAllText(RESOURCES_PATH + "TracerConfigMap.xml", TracerContent)
                Return True
            End If

            Return False
        Catch ex As Exception
            Console.WriteLine("FAIL Exception Occured In ConfigureTracer : " + ex.Message.ToString)
            End
        End Try
    End Function

    Private Function UpdateIEXIni(ByVal IEXPath As String) As Boolean
        Dim iniFile As String = ""
        Dim objReader As StreamReader = Nothing
        Dim objWriter As StreamWriter
        Dim iniContent As String = ""
        Dim newIniContent As String = ""

        Try
            iniFile = IEXPath + "iex.ini"
            File.Copy(iniFile, IEXPath + "iex.backup", True)

            objReader = New StreamReader(iniFile)

            Dim i As Integer = 1
            Do Until objReader.EndOfStream = True

                iniContent = objReader.ReadLine()

                If iniContent.Contains("EPG_DICTIONARY_PATH=") Then
                    If RFPort.ToString.ToLower = "um" Then
                        newIniContent += "EPG_DICTIONARY_PATH=" + IEX_DICTIONARY_Dest + "dictionary_eng-deu.xml" + vbCrLf
                    ElseIf RFPort.ToLower = "nl" Then
                        newIniContent += "EPG_DICTIONARY_PATH=" + IEX_DICTIONARY_Dest + "dictionary_eng-nld.xml" + vbCrLf
                    Else
                        newIniContent += "EPG_DICTIONARY_PATH=" + IEX_DICTIONARY_Dest + "dictionary_eng.xml" + vbCrLf
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
            Console.WriteLine("FAIL Exception Occured In UpdateIEXIni : " + ex.Message.ToString)
            End
        End Try

        Return True

    End Function

    Private Function deleteDirectory(ByVal Dst As String) As Boolean
        Try
            If Directory.Exists(Dst) Then
                Directory.Delete(Dst, True)
            End If
        Catch ex As Exception
            RemoveReadonly(Dst)
            Try
                Directory.Delete(Dst, True)
            Catch ex1 As Exception
                Return False
            End Try
        End Try

        Return True
    End Function

    Private Function CopyDirectory(ByVal Src As String, ByVal Dst As String, Optional ByVal IsXML As Boolean = False) As Boolean
        Try
            If IsXML Then

                Directory.CreateDirectory(Dst)

                For Each newPath As String In Directory.GetFiles(Src, "*.xml", SearchOption.TopDirectoryOnly)
                    File.Copy(newPath, newPath.Replace(Src, Dst), True)
                Next
            Else
                Directory.CreateDirectory(Dst)

                For Each dirPath As String In Directory.GetDirectories(Src, "*", SearchOption.AllDirectories)
                    Directory.CreateDirectory(dirPath.Replace(Src, Dst))
                Next

                For Each newPath As String In Directory.GetFiles(Src, "*.*", SearchOption.AllDirectories)
                    File.Copy(newPath, newPath.Replace(Src, Dst), True)
                Next
            End If

        Catch ex As Exception
            Console.WriteLine("FAIL Exception Occured In CopyDirectory Src : " + Src.ToString + " Dst : " + Dst + " Exception : " + ex.Message.ToString)
            End
        End Try

        Return True
    End Function

    Private Function ReadDeployerIni() As Boolean
        Try

            Dim iniFile As AMS.Profile.Ini

            iniFile = New AMS.Profile.Ini(RootFolder + "\IEX_TOOLS\DeployFiles\Deployer.ini")

            IEX_PCAT_MODIFIER_Dest = iniFile.GetValue("PATHS", "PCAT_Modifier").ToString + "IEX" + IEXNumber + "\"
            IEX_TESTS_Dest = iniFile.GetValue("PATHS", "Tests").ToString + "IEX" + IEXNumber + "\"
            IEX_IR_Dest = iniFile.GetValue("PATHS", "IR").ToString
            IEX_INI_FILES_Dest = iniFile.GetValue("PATHS", "IniFiles").ToString + "IEX" + IEXNumber + "\"
            IEX_DICTIONARY_Dest = IEX_INI_FILES_Dest + "Dictionary\"
            IEX_ELEMENTARYACTIONS_Dest = iniFile.GetValue("PATHS", "ElementaryActions").ToString + "IEX" + IEXNumber + "\"
            IEX_BUILD_Dest = iniFile.GetValue("PATHS", "Buildlocation").ToString + "IEX" + IEXNumber + "\"
        Catch ex As Exception
            Console.WriteLine("FAIL Exception Occured In ReadDeployerIni : " + ex.Message.ToString)
            End
        End Try

        Return True

    End Function

    Private Function ReadEviromentIni() As Boolean
        Try

            Dim iniFile As AMS.Profile.Ini
            iniFile = New AMS.Profile.Ini(IEX_INI_FILES_Dest + "Environment.ini")
            PROJECT = iniFile.GetValue("IEX" + IEXNumber.ToString, "Project").ToString
            ImageName = iniFile.GetValue("IEX" + IEXNumber.ToString, "ImageName").ToString
            If ImageName = "" Then
                ImageName = "bzimage"
            Else
                ImageName = ImageName
            End If
            FlashBuildPath = iniFile.GetValue("IEX" + IEXNumber.ToString, "FlashBuildPath").ToString
            Try
                RFPort = iniFile.GetValue("IEX" + IEXNumber.ToString, "RFPort").ToString
            Catch ex As Exception
                RFPort = ""
            End Try
        Catch ex As Exception
            Console.WriteLine("FAIL Exception Occured In ReadEviromentIni : " + ex.Message.ToString)
            End
        End Try

        Return True

    End Function
    Private Function ReadTestIni() As Boolean
        Try

            Dim iniFile As AMS.Profile.Ini
            iniFile = New AMS.Profile.Ini(IEX_INI_FILES_Dest + "Test.ini")
            Try
                FUNCTIONAL_GROUP = iniFile.GetValue("IEX", "functional-group").ToString
            Catch ex As Exception
                FUNCTIONAL_GROUP = ""
            End Try
            Try
                BinaryType = iniFile.GetValue("IEX", "binaries").ToString
            Catch ex As Exception
                BinaryType = ""
            End Try
            Try
                TestName = iniFile.GetValue("IEX", "testnames").ToString
            Catch ex As Exception
                TestName = ""
            End Try
            Try
                NumberOfMinutes = iniFile.GetValue("TEST PARAMS", "NumberOfMinutes").ToString
            Catch ex As Exception
                NumberOfMinutes = "720"
            End Try
            Try
                MemoryDump = iniFile.GetValue("TEST PARAMS", "MemoryDump").ToString
            Catch ex As Exception
                MemoryDump = "NO"
            End Try
            Try
                Target = iniFile.GetValue("TEST PARAMS", "iTarget").ToString
            Catch ex As Exception
                Target = "1000"
            End Try
            Try
                MaxErrors = iniFile.GetValue("TEST PARAMS", "iMaxErrors").ToString
            Catch ex As Exception
                MaxErrors = "1000"
            End Try
            Try
                TestGroup = iniFile.GetValue("IEX", "test_group").ToString
            Catch ex As Exception
                TestGroup = ""
            End Try
            Try
                ProcessPath = iniFile.GetValue("TEST PARAMS", "ProcessPath").ToString
            Catch ex As Exception
                ProcessPath = ""
            End Try
            Try
                ProcessPathDrive = iniFile.GetValue("TEST PARAMS", "ProcessPathDrive").ToString
            Catch ex As Exception
                ProcessPathDrive = ""
            End Try
            Try
                StartCommand = iniFile.GetValue("TEST PARAMS", "StartCommand").ToString
            Catch ex As Exception
                StartCommand = ""
            End Try
            Try
                ipvTestIniParam = iniFile.GetValue("IEX", "requires").ToString
            Catch ex As Exception
                ipvTestIniParam = ""
            End Try
        Catch ex As Exception
            Console.WriteLine("FAIL Exception Occured In Reading TestIni : " + ex.Message.ToString)
            End

        End Try

        Return True

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
            Console.WriteLine("FAIL Exception Occured In RemoveReadonly : " + ex.Message.ToString)
            End
        End Try
    End Function


    Private Sub WriteResult(ByVal Msg As String)
        Dim sPath As String = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly.Location)

        Try
            If File.Exists(sPath + "\DeliverResults.txt") Then
                File.Delete(sPath + "\DeliverResults.txt")
            End If

            File.WriteAllText(sPath + "\DeliverResults.txt", Msg)
        Catch ex As Exception
            Console.WriteLine("FAIL Exception Occured In WriteResult : " + ex.Message.ToString)
            End
        End Try
    End Sub
    'Function to start Process
    Public Function StartProcess(ByVal command As String, ByVal outputfrm As String) As String
        Dim finaloutput As String = ""
        Try
            Dim SW As System.IO.StreamWriter = New System.IO.StreamWriter("VirtualBAT.bat")

            SW.WriteLine(command)

            SW.Flush()
            SW.Close()
            SW.Dispose()
            SW = Nothing
            ' System.Diagnostics.Process.Start("VirtualBAT.bat")

            ' starting process accoriding to command in VirtualBAT.bat
            Dim myProcess As Process = New Process()
            Dim myProcessStartInfo As ProcessStartInfo = New ProcessStartInfo("VirtualBAT.bat")
            myProcessStartInfo.UseShellExecute = False
            myProcessStartInfo.RedirectStandardInput = True
            myProcessStartInfo.RedirectStandardOutput = True
            myProcessStartInfo.RedirectStandardError = True

            myProcess.StartInfo = myProcessStartInfo

            myProcess.Start() ' start process

            myProcess.WaitForExit() 'wait for process to complete

            finaloutput = FetchFinalOutput(myProcess, outputfrm) ' calling final output function to fetch final output

        Catch ex As Exception
            finaloutput = ex.Message
            Console.WriteLine("Failed to Start process : " + finaloutput)
        End Try

        Return finaloutput
    End Function

    ' Function to fetch final output from logs

    Private Function FetchFinalOutput(ByVal process As Process, ByVal output As String) As String
        Dim finaloutput As String = ""
        If output.ToUpper() = "OUTPUT" Then
            While Not process.StandardOutput.EndOfStream ' fetching logs till end of stream
                finaloutput = process.StandardOutput.ReadLine()
            End While
            Return finaloutput
        Else
            finaloutput = process.StandardError.ReadToEnd()
            Return finaloutput
        End If

    End Function
End Module
