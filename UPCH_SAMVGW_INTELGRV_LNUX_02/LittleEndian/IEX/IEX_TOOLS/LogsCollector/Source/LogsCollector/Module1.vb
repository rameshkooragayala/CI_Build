Imports System.IO

Module Module1
    Dim TestLogPath As String = ""
    Dim IEXNumber As String = ""
    Dim ExecutionEnginePath As String = ""
    Dim ServerLogsPath As String = ""
    Dim UdpLogPath As String = ""

    Sub Main(ByVal Args As String())
        Dim Msg As String = ""
        Dim passed As Boolean = True

        If Args.Length = 0 Then
            Msg = "Error In Arguments Please Check. Please Use LogCollector <Path To Test Log Folder> <IEX Instance Number>"
            Console.WriteLine("Please Use LogCollector <Path To Test Log Folder> <IEX Instance Number>")
            Console.WriteLine("FAIL " + Msg)
            End
        End If

        If Args(0) = "" Then
            Msg = "Error In First Argument Please Check. Please Use LogCollector <Path To Test Log Folder> <IEX Instance Number>"
            Console.WriteLine("Error In First Argument Please Check.")
            Console.WriteLine("Please Use LogCollector <Path To Test Log Folder> <IEX Instance Number>")
            Console.WriteLine("FAIL " + Msg)
            End
        Else
            TestLogPath = Args(0)
            If TestLogPath.EndsWith("\") = False Then
                TestLogPath = TestLogPath + "\"
            End If
        End If

        If Args(1) = "" Then
            Msg = "Error In Second Argument Please Check. Please Use LogCollector <Path To Test Log Folder> <IEX Instance Number>"
            Console.WriteLine("Error In Second Argument Please Check.")
            Console.WriteLine("Please Use LogCollector <Path To Test Log Folder> <IEX Instance Number>")
            Console.WriteLine("FAIL " + Msg)
            End
        Else
            IEXNumber = Args(1)
        End If

        '*************************** Copy UDP Log Files *******************

        UdpLogPath = "C:\Program Files\IEX\IEX_" + IEXNumber + "\UdpLog\"

        Try
            passed = CopyLogFiles(UdpLogPath, TestLogPath)
            If Not passed Then
                Msg = "Error Copying IEX UDP Log Files From : " + UdpLogPath.ToString + " To : " + TestLogPath
                Console.WriteLine("FAIL " + Msg)
                End
            End If
        Catch ex As Exception
            Msg = "Error Copying IEX UDP Log Files From : " + UdpLogPath.ToString + " To : " + TestLogPath
            Console.WriteLine("FAIL " + Msg)
            End
        End Try

        '*************************** Copy IEX Server Logs Files *******************

        ServerLogsPath = "C:\Program Files\IEX\IEX_" + IEXNumber + "\IEX Server Logs\"

        Try
            passed = CopyLogFiles(ServerLogsPath, TestLogPath)
            If Not passed Then
                Msg = "Error Copying IEX Server Logs Files From : " + ServerLogsPath.ToString + " To : " + TestLogPath
                Console.WriteLine("FAIL " + Msg)
                End
            End If
        Catch ex As Exception
            Msg = "Error Copying IEX Server Logs Files From : " + ServerLogsPath.ToString + " To : " + TestLogPath
            Console.WriteLine("FAIL " + Msg)
            End
        End Try

        '*************************** Copy ExecutionEngine Log Files *******************

        ExecutionEnginePath = "C:\Program Files\IEX\ExecutionEngine logs\Server_127.0.0.1_Port_" + IEXNumber + "\"

        Try
            passed = CopyLogFiles(ExecutionEnginePath, TestLogPath)
            'If Not passed Then
            '    Msg = "Error Copying ExecutionEngine Log Files From : " + ExecutionEnginePath.ToString + " To : " + TestLogPath
            '    Console.WriteLine("FAIL " + Msg)
            '    End
            'End If
        Catch ex As Exception
            'Msg = "Error Copying ExecutionEngine Log Files From : " + ExecutionEnginePath.ToString + " To : " + TestLogPath
            'Console.WriteLine("FAIL " + Msg)
            'End
        End Try


        '*************************** Renaming All Other Files *******************
        Try
            passed = RenameAllFiles(TestLogPath)
            If Not passed Then
                Msg = "Error Renaming All Other Files In : " + TestLogPath
                Console.WriteLine("FAIL " + Msg)
                End
            End If
        Catch ex As Exception
            Msg = "Error Renaming All Other Files In : " + TestLogPath
            Console.WriteLine("FAIL " + Msg)
            End
        End Try

        '*************************** Delete Directories *******************
        Try
            passed = DeleteDirectory(ExecutionEnginePath)
            If Not passed Then
                'Msg = "Error Deleting ExecutionEngine Log Path"
                'Console.WriteLine("FAIL " + Msg)
                'End
            End If
        Catch ex As Exception
            'Msg = "Error Deleting ExecutionEngine Log Path"
            'Console.WriteLine("FAIL " + Msg)
            'End
        End Try

        Try
            passed = DeleteDirectory(ServerLogsPath)
            If Not passed Then
                Msg = "Error Deleting IEX Server Logs Path"
                Console.WriteLine("FAIL " + Msg)
                End
            End If
        Catch ex As Exception
            Msg = "Error Deleting IEX Server Logs Path"
            Console.WriteLine("FAIL " + Msg)
            End
        End Try

        Try
            passed = DeleteDirectory(UdpLogPath)
            If passed Then
                Console.WriteLine("PASS")
                End
            Else
                Msg = "Error Deleting Udp Log Path"
                Console.WriteLine("FAIL " + Msg)
                End
            End If
        Catch ex As Exception
            Msg = "Error Deleting Udp Log Path"
            Console.WriteLine("FAIL " + Msg)
            End
        End Try

    End Sub

    Private Function RenameAllFiles(ByVal Dst As String) As Boolean
        Try
            Dim filePaths As String() = Nothing

            filePaths = Directory.GetFiles(Dst, "*.log", SearchOption.AllDirectories)
            If filePaths IsNot Nothing Then
                For Each f As String In filePaths
                    Dim fi As New FileInfo(f)
                    My.Computer.FileSystem.RenameFile(fi.FullName, fi.Name.Replace(".log", ".txt"))
                Next
            End If
        Catch ex As Exception
            Return False
        End Try

        Return True
    End Function

    Private Function DeleteDirectory(ByVal Dst As String) As Boolean
        Try
            If Directory.Exists(Dst) Then
                Directory.Delete(Dst, True)
            End If
        Catch ex As Exception
            Return False
        End Try
        Return True
    End Function

    Private Function CopyLogFiles(ByVal RootDir As String, ByVal fTo As String) As Boolean
        Try
            Dim filePaths As String() = Nothing

            filePaths = Directory.GetFiles(RootDir, "*.log", SearchOption.AllDirectories)
            If filePaths IsNot Nothing Then
                For Each f As String In filePaths
                    Dim fi As New FileInfo(f)
                    File.Copy(fi.FullName, fTo + fi.Name.Replace(".log", ".txt"), True)
                Next
            End If
        Catch ex As Exception
            Return False
        End Try

        Return True
    End Function

End Module
