<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class Form1
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Me.ButDeploy = New System.Windows.Forms.Button()
        Me.LblStatus = New System.Windows.Forms.Label()
        Me.Timer1 = New System.Windows.Forms.Timer(Me.components)
        Me.TabProject = New System.Windows.Forms.TabControl()
        Me.TabPage2 = New System.Windows.Forms.TabPage()
        Me.TabPage1 = New System.Windows.Forms.TabPage()
        Me.TabPage4 = New System.Windows.Forms.TabPage()
        Me.TabPage3 = New System.Windows.Forms.TabPage()
        Me.GroupBox5 = New System.Windows.Forms.GroupBox()
        Me.TxtLog = New System.Windows.Forms.TextBox()
        Me.GroupBox2 = New System.Windows.Forms.GroupBox()
        Me.RadOriginal = New System.Windows.Forms.RadioButton()
        Me.RadIEXTG = New System.Windows.Forms.RadioButton()
        Me.RadTG = New System.Windows.Forms.RadioButton()
        Me.Label10 = New System.Windows.Forms.Label()
        Me.TxtUserName = New System.Windows.Forms.TextBox()
        Me.LblUserName = New System.Windows.Forms.Label()
        Me.GrpBuilds = New System.Windows.Forms.GroupBox()
        Me.GroupBox3 = New System.Windows.Forms.GroupBox()
        Me.CmbClIEXNumber = New System.Windows.Forms.ComboBox()
        Me.Label6 = New System.Windows.Forms.Label()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.CmbClient = New System.Windows.Forms.ComboBox()
        Me.LblCurClient = New System.Windows.Forms.Label()
        Me.ButClient = New System.Windows.Forms.Button()
        Me.GroupBox1 = New System.Windows.Forms.GroupBox()
        Me.GrpClient = New System.Windows.Forms.GroupBox()
        Me.CmbMClIEXNumber = New System.Windows.Forms.ComboBox()
        Me.LblClientIEXNumber = New System.Windows.Forms.Label()
        Me.CmbMGwIEXNumber = New System.Windows.Forms.ComboBox()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.RadCOGECO = New System.Windows.Forms.RadioButton()
        Me.RadUPC = New System.Windows.Forms.RadioButton()
        Me.ButClDeploy = New System.Windows.Forms.Button()
        Me.ButGwDeploy = New System.Windows.Forms.Button()
        Me.TxtClBuildPath = New System.Windows.Forms.TextBox()
        Me.LblClient = New System.Windows.Forms.Label()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.TxtFrom = New System.Windows.Forms.TextBox()
        Me.TxtGwBuildPath = New System.Windows.Forms.TextBox()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.Label7 = New System.Windows.Forms.Label()
        Me.ButGateway = New System.Windows.Forms.Button()
        Me.CmbGwIEXNumber = New System.Windows.Forms.ComboBox()
        Me.CmbGateway = New System.Windows.Forms.ComboBox()
        Me.LblCurGateway = New System.Windows.Forms.Label()
        Me.ChkClean = New System.Windows.Forms.CheckBox()
        Me.TabProject.SuspendLayout()
        Me.GroupBox5.SuspendLayout()
        Me.GroupBox2.SuspendLayout()
        Me.GrpBuilds.SuspendLayout()
        Me.GroupBox3.SuspendLayout()
        Me.GroupBox1.SuspendLayout()
        Me.GrpClient.SuspendLayout()
        Me.SuspendLayout()
        '
        'ButDeploy
        '
        Me.ButDeploy.Location = New System.Drawing.Point(8, 406)
        Me.ButDeploy.Name = "ButDeploy"
        Me.ButDeploy.Size = New System.Drawing.Size(131, 41)
        Me.ButDeploy.TabIndex = 8
        Me.ButDeploy.Text = "Deploy Environment"
        Me.ButDeploy.UseVisualStyleBackColor = True
        '
        'LblStatus
        '
        Me.LblStatus.Font = New System.Drawing.Font("Wide Latin", 26.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LblStatus.ForeColor = System.Drawing.SystemColors.AppWorkspace
        Me.LblStatus.Location = New System.Drawing.Point(138, 393)
        Me.LblStatus.Name = "LblStatus"
        Me.LblStatus.Size = New System.Drawing.Size(405, 56)
        Me.LblStatus.TabIndex = 10
        Me.LblStatus.Text = "Copying"
        Me.LblStatus.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'Timer1
        '
        Me.Timer1.Enabled = True
        Me.Timer1.Interval = 300000
        '
        'TabProject
        '
        Me.TabProject.Controls.Add(Me.TabPage2)
        Me.TabProject.Controls.Add(Me.TabPage1)
        Me.TabProject.Controls.Add(Me.TabPage4)
        Me.TabProject.Controls.Add(Me.TabPage3)
        Me.TabProject.Location = New System.Drawing.Point(1, 2)
        Me.TabProject.Name = "TabProject"
        Me.TabProject.SelectedIndex = 0
        Me.TabProject.Size = New System.Drawing.Size(546, 22)
        Me.TabProject.TabIndex = 17
        '
        'TabPage2
        '
        Me.TabPage2.Location = New System.Drawing.Point(4, 22)
        Me.TabPage2.Name = "TabPage2"
        Me.TabPage2.Padding = New System.Windows.Forms.Padding(3)
        Me.TabPage2.Size = New System.Drawing.Size(538, 0)
        Me.TabPage2.TabIndex = 1
        Me.TabPage2.Text = "UPC"
        Me.TabPage2.UseVisualStyleBackColor = True
        '
        'TabPage1
        '
        Me.TabPage1.Location = New System.Drawing.Point(4, 22)
        Me.TabPage1.Name = "TabPage1"
        Me.TabPage1.Size = New System.Drawing.Size(538, 0)
        Me.TabPage1.TabIndex = 2
        Me.TabPage1.Text = "COGECO"
        Me.TabPage1.UseVisualStyleBackColor = True
        '
        'TabPage4
        '
        Me.TabPage4.Location = New System.Drawing.Point(4, 22)
        Me.TabPage4.Name = "TabPage4"
        Me.TabPage4.Padding = New System.Windows.Forms.Padding(3)
        Me.TabPage4.Size = New System.Drawing.Size(538, 0)
        Me.TabPage4.TabIndex = 4
        Me.TabPage4.Text = "CANAL"
        Me.TabPage4.UseVisualStyleBackColor = True
        '
        'TabPage3
        '
        Me.TabPage3.Location = New System.Drawing.Point(4, 22)
        Me.TabPage3.Name = "TabPage3"
        Me.TabPage3.Size = New System.Drawing.Size(538, 0)
        Me.TabPage3.TabIndex = 3
        Me.TabPage3.Text = "MANUAL"
        Me.TabPage3.UseVisualStyleBackColor = True
        '
        'GroupBox5
        '
        Me.GroupBox5.Controls.Add(Me.TxtLog)
        Me.GroupBox5.Location = New System.Drawing.Point(8, 317)
        Me.GroupBox5.Name = "GroupBox5"
        Me.GroupBox5.Size = New System.Drawing.Size(365, 83)
        Me.GroupBox5.TabIndex = 46
        Me.GroupBox5.TabStop = False
        Me.GroupBox5.Text = "Log :"
        '
        'TxtLog
        '
        Me.TxtLog.Location = New System.Drawing.Point(6, 19)
        Me.TxtLog.Multiline = True
        Me.TxtLog.Name = "TxtLog"
        Me.TxtLog.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
        Me.TxtLog.Size = New System.Drawing.Size(353, 58)
        Me.TxtLog.TabIndex = 16
        Me.TxtLog.WordWrap = False
        '
        'GroupBox2
        '
        Me.GroupBox2.Controls.Add(Me.RadOriginal)
        Me.GroupBox2.Controls.Add(Me.RadIEXTG)
        Me.GroupBox2.Controls.Add(Me.RadTG)
        Me.GroupBox2.Location = New System.Drawing.Point(379, 317)
        Me.GroupBox2.Name = "GroupBox2"
        Me.GroupBox2.Size = New System.Drawing.Size(162, 83)
        Me.GroupBox2.TabIndex = 45
        Me.GroupBox2.TabStop = False
        Me.GroupBox2.Text = "Streams"
        '
        'RadOriginal
        '
        Me.RadOriginal.AutoSize = True
        Me.RadOriginal.Checked = True
        Me.RadOriginal.Location = New System.Drawing.Point(6, 56)
        Me.RadOriginal.Name = "RadOriginal"
        Me.RadOriginal.Size = New System.Drawing.Size(76, 17)
        Me.RadOriginal.TabIndex = 4
        Me.RadOriginal.TabStop = True
        Me.RadOriginal.Text = "ORIGINAL"
        Me.RadOriginal.UseVisualStyleBackColor = True
        '
        'RadIEXTG
        '
        Me.RadIEXTG.AutoSize = True
        Me.RadIEXTG.Location = New System.Drawing.Point(6, 36)
        Me.RadIEXTG.Name = "RadIEXTG"
        Me.RadIEXTG.Size = New System.Drawing.Size(89, 17)
        Me.RadIEXTG.TabIndex = 3
        Me.RadIEXTG.Text = "IEX TIG GEN"
        Me.RadIEXTG.UseVisualStyleBackColor = True
        '
        'RadTG
        '
        Me.RadTG.AutoSize = True
        Me.RadTG.Location = New System.Drawing.Point(6, 17)
        Me.RadTG.Name = "RadTG"
        Me.RadTG.Size = New System.Drawing.Size(110, 17)
        Me.RadTG.TabIndex = 2
        Me.RadTG.Text = "TIG GEN SHORT"
        Me.RadTG.UseVisualStyleBackColor = True
        '
        'Label10
        '
        Me.Label10.AutoSize = True
        Me.Label10.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label10.Location = New System.Drawing.Point(10, 75)
        Me.Label10.Name = "Label10"
        Me.Label10.Size = New System.Drawing.Size(107, 13)
        Me.Label10.TabIndex = 42
        Me.Label10.Text = "GW IEX Number :"
        '
        'TxtUserName
        '
        Me.TxtUserName.Location = New System.Drawing.Point(9, 32)
        Me.TxtUserName.Name = "TxtUserName"
        Me.TxtUserName.Size = New System.Drawing.Size(158, 20)
        Me.TxtUserName.TabIndex = 43
        '
        'LblUserName
        '
        Me.LblUserName.AutoSize = True
        Me.LblUserName.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LblUserName.Location = New System.Drawing.Point(7, 16)
        Me.LblUserName.Name = "LblUserName"
        Me.LblUserName.Size = New System.Drawing.Size(77, 13)
        Me.LblUserName.TabIndex = 44
        Me.LblUserName.Text = "User Name :"
        '
        'GrpBuilds
        '
        Me.GrpBuilds.Controls.Add(Me.GroupBox3)
        Me.GrpBuilds.Controls.Add(Me.TxtUserName)
        Me.GrpBuilds.Controls.Add(Me.GroupBox1)
        Me.GrpBuilds.Controls.Add(Me.LblUserName)
        Me.GrpBuilds.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.GrpBuilds.Location = New System.Drawing.Point(5, 30)
        Me.GrpBuilds.Name = "GrpBuilds"
        Me.GrpBuilds.Size = New System.Drawing.Size(542, 280)
        Me.GrpBuilds.TabIndex = 40
        Me.GrpBuilds.TabStop = False
        Me.GrpBuilds.Text = "Builds"
        '
        'GroupBox3
        '
        Me.GroupBox3.Controls.Add(Me.CmbClIEXNumber)
        Me.GroupBox3.Controls.Add(Me.Label6)
        Me.GroupBox3.Controls.Add(Me.Label1)
        Me.GroupBox3.Controls.Add(Me.CmbClient)
        Me.GroupBox3.Controls.Add(Me.LblCurClient)
        Me.GroupBox3.Controls.Add(Me.ButClient)
        Me.GroupBox3.Location = New System.Drawing.Point(4, 166)
        Me.GroupBox3.Name = "GroupBox3"
        Me.GroupBox3.Size = New System.Drawing.Size(538, 105)
        Me.GroupBox3.TabIndex = 50
        Me.GroupBox3.TabStop = False
        Me.GroupBox3.Text = "Client"
        '
        'CmbClIEXNumber
        '
        Me.CmbClIEXNumber.FormattingEnabled = True
        Me.CmbClIEXNumber.Items.AddRange(New Object() {"1", "2", "3", "4", "5", "6", "7", "8"})
        Me.CmbClIEXNumber.Location = New System.Drawing.Point(121, 73)
        Me.CmbClIEXNumber.Name = "CmbClIEXNumber"
        Me.CmbClIEXNumber.Size = New System.Drawing.Size(43, 21)
        Me.CmbClIEXNumber.TabIndex = 47
        '
        'Label6
        '
        Me.Label6.AutoSize = True
        Me.Label6.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label6.Location = New System.Drawing.Point(7, 25)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(47, 13)
        Me.Label6.TabIndex = 26
        Me.Label6.Text = "Client :"
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label1.Location = New System.Drawing.Point(8, 77)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(101, 13)
        Me.Label1.TabIndex = 46
        Me.Label1.Text = "CL IEX Number :"
        '
        'CmbClient
        '
        Me.CmbClient.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.CmbClient.FormattingEnabled = True
        Me.CmbClient.Location = New System.Drawing.Point(7, 46)
        Me.CmbClient.Name = "CmbClient"
        Me.CmbClient.Size = New System.Drawing.Size(428, 21)
        Me.CmbClient.TabIndex = 25
        '
        'LblCurClient
        '
        Me.LblCurClient.AutoSize = True
        Me.LblCurClient.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LblCurClient.ForeColor = System.Drawing.SystemColors.ActiveCaption
        Me.LblCurClient.Location = New System.Drawing.Point(72, 25)
        Me.LblCurClient.Name = "LblCurClient"
        Me.LblCurClient.Size = New System.Drawing.Size(115, 13)
        Me.LblCurClient.TabIndex = 37
        Me.LblCurClient.Text = "50 50 50 50 50 50 "
        '
        'ButClient
        '
        Me.ButClient.Location = New System.Drawing.Point(441, 46)
        Me.ButClient.Name = "ButClient"
        Me.ButClient.Size = New System.Drawing.Size(93, 21)
        Me.ButClient.TabIndex = 24
        Me.ButClient.Text = "Deploy Build"
        Me.ButClient.UseVisualStyleBackColor = True
        '
        'GroupBox1
        '
        Me.GroupBox1.Controls.Add(Me.Label7)
        Me.GroupBox1.Controls.Add(Me.ButGateway)
        Me.GroupBox1.Controls.Add(Me.CmbGwIEXNumber)
        Me.GroupBox1.Controls.Add(Me.CmbGateway)
        Me.GroupBox1.Controls.Add(Me.Label10)
        Me.GroupBox1.Controls.Add(Me.LblCurGateway)
        Me.GroupBox1.Location = New System.Drawing.Point(4, 57)
        Me.GroupBox1.Name = "GroupBox1"
        Me.GroupBox1.Size = New System.Drawing.Size(538, 102)
        Me.GroupBox1.TabIndex = 49
        Me.GroupBox1.TabStop = False
        Me.GroupBox1.Text = "Gateway"
        '
        'GrpClient
        '
        Me.GrpClient.Controls.Add(Me.CmbMClIEXNumber)
        Me.GrpClient.Controls.Add(Me.LblClientIEXNumber)
        Me.GrpClient.Controls.Add(Me.CmbMGwIEXNumber)
        Me.GrpClient.Controls.Add(Me.Label3)
        Me.GrpClient.Controls.Add(Me.RadCOGECO)
        Me.GrpClient.Controls.Add(Me.RadUPC)
        Me.GrpClient.Controls.Add(Me.ButClDeploy)
        Me.GrpClient.Controls.Add(Me.ButGwDeploy)
        Me.GrpClient.Controls.Add(Me.TxtClBuildPath)
        Me.GrpClient.Controls.Add(Me.LblClient)
        Me.GrpClient.Controls.Add(Me.Label4)
        Me.GrpClient.Controls.Add(Me.TxtFrom)
        Me.GrpClient.Controls.Add(Me.TxtGwBuildPath)
        Me.GrpClient.Controls.Add(Me.Label2)
        Me.GrpClient.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.GrpClient.Location = New System.Drawing.Point(5, 30)
        Me.GrpClient.Name = "GrpClient"
        Me.GrpClient.Size = New System.Drawing.Size(542, 281)
        Me.GrpClient.TabIndex = 48
        Me.GrpClient.TabStop = False
        Me.GrpClient.Text = "Paths"
        '
        'CmbMClIEXNumber
        '
        Me.CmbMClIEXNumber.FormattingEnabled = True
        Me.CmbMClIEXNumber.Items.AddRange(New Object() {"1", "2", "3", "4", "5", "6", "7", "8"})
        Me.CmbMClIEXNumber.Location = New System.Drawing.Point(121, 193)
        Me.CmbMClIEXNumber.Name = "CmbMClIEXNumber"
        Me.CmbMClIEXNumber.Size = New System.Drawing.Size(43, 21)
        Me.CmbMClIEXNumber.TabIndex = 49
        '
        'LblClientIEXNumber
        '
        Me.LblClientIEXNumber.AutoSize = True
        Me.LblClientIEXNumber.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LblClientIEXNumber.Location = New System.Drawing.Point(8, 197)
        Me.LblClientIEXNumber.Name = "LblClientIEXNumber"
        Me.LblClientIEXNumber.Size = New System.Drawing.Size(101, 13)
        Me.LblClientIEXNumber.TabIndex = 48
        Me.LblClientIEXNumber.Text = "CL IEX Number :"
        '
        'CmbMGwIEXNumber
        '
        Me.CmbMGwIEXNumber.FormattingEnabled = True
        Me.CmbMGwIEXNumber.Items.AddRange(New Object() {"1", "2", "3", "4", "5", "6", "7", "8"})
        Me.CmbMGwIEXNumber.Location = New System.Drawing.Point(121, 113)
        Me.CmbMGwIEXNumber.Name = "CmbMGwIEXNumber"
        Me.CmbMGwIEXNumber.Size = New System.Drawing.Size(43, 21)
        Me.CmbMGwIEXNumber.TabIndex = 47
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label3.Location = New System.Drawing.Point(8, 117)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(107, 13)
        Me.Label3.TabIndex = 46
        Me.Label3.Text = "GW IEX Number :"
        '
        'RadCOGECO
        '
        Me.RadCOGECO.AutoSize = True
        Me.RadCOGECO.Location = New System.Drawing.Point(64, 228)
        Me.RadCOGECO.Name = "RadCOGECO"
        Me.RadCOGECO.Size = New System.Drawing.Size(70, 17)
        Me.RadCOGECO.TabIndex = 27
        Me.RadCOGECO.Text = "COGECO"
        Me.RadCOGECO.UseVisualStyleBackColor = True
        '
        'RadUPC
        '
        Me.RadUPC.AutoSize = True
        Me.RadUPC.Checked = True
        Me.RadUPC.Location = New System.Drawing.Point(11, 228)
        Me.RadUPC.Name = "RadUPC"
        Me.RadUPC.Size = New System.Drawing.Size(47, 17)
        Me.RadUPC.TabIndex = 26
        Me.RadUPC.TabStop = True
        Me.RadUPC.Text = "UPC"
        Me.RadUPC.UseVisualStyleBackColor = True
        '
        'ButClDeploy
        '
        Me.ButClDeploy.Location = New System.Drawing.Point(443, 163)
        Me.ButClDeploy.Name = "ButClDeploy"
        Me.ButClDeploy.Size = New System.Drawing.Size(93, 21)
        Me.ButClDeploy.TabIndex = 25
        Me.ButClDeploy.Text = "Deploy"
        Me.ButClDeploy.UseVisualStyleBackColor = True
        '
        'ButGwDeploy
        '
        Me.ButGwDeploy.Location = New System.Drawing.Point(443, 87)
        Me.ButGwDeploy.Name = "ButGwDeploy"
        Me.ButGwDeploy.Size = New System.Drawing.Size(93, 21)
        Me.ButGwDeploy.TabIndex = 24
        Me.ButGwDeploy.Text = "Deploy"
        Me.ButGwDeploy.UseVisualStyleBackColor = True
        '
        'TxtClBuildPath
        '
        Me.TxtClBuildPath.Location = New System.Drawing.Point(10, 163)
        Me.TxtClBuildPath.Name = "TxtClBuildPath"
        Me.TxtClBuildPath.Size = New System.Drawing.Size(428, 20)
        Me.TxtClBuildPath.TabIndex = 22
        '
        'LblClient
        '
        Me.LblClient.AutoSize = True
        Me.LblClient.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LblClient.Location = New System.Drawing.Point(6, 147)
        Me.LblClient.Name = "LblClient"
        Me.LblClient.Size = New System.Drawing.Size(153, 13)
        Me.LblClient.TabIndex = 23
        Me.LblClient.Text = "Client Build Path : (\NDS)"
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label4.Location = New System.Drawing.Point(8, 22)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(154, 13)
        Me.Label4.TabIndex = 21
        Me.Label4.Text = "IEX_AUTOMATION Path :"
        '
        'TxtFrom
        '
        Me.TxtFrom.Location = New System.Drawing.Point(10, 37)
        Me.TxtFrom.Name = "TxtFrom"
        Me.TxtFrom.Size = New System.Drawing.Size(526, 20)
        Me.TxtFrom.TabIndex = 20
        Me.TxtFrom.Text = "\\10.62.14.241\extra\mnt\users\IEX\"
        '
        'TxtGwBuildPath
        '
        Me.TxtGwBuildPath.Location = New System.Drawing.Point(10, 87)
        Me.TxtGwBuildPath.Name = "TxtGwBuildPath"
        Me.TxtGwBuildPath.Size = New System.Drawing.Size(427, 20)
        Me.TxtGwBuildPath.TabIndex = 18
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label2.Location = New System.Drawing.Point(7, 71)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(170, 13)
        Me.Label2.TabIndex = 19
        Me.Label2.Text = "Gateway Build Path : (\NDS)"
        '
        'Label7
        '
        Me.Label7.AutoSize = True
        Me.Label7.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label7.Location = New System.Drawing.Point(8, 24)
        Me.Label7.Name = "Label7"
        Me.Label7.Size = New System.Drawing.Size(64, 13)
        Me.Label7.TabIndex = 23
        Me.Label7.Text = "Gateway :"
        '
        'ButGateway
        '
        Me.ButGateway.Location = New System.Drawing.Point(441, 46)
        Me.ButGateway.Name = "ButGateway"
        Me.ButGateway.Size = New System.Drawing.Size(93, 21)
        Me.ButGateway.TabIndex = 21
        Me.ButGateway.Text = "Deploy Build"
        Me.ButGateway.UseVisualStyleBackColor = True
        '
        'CmbGwIEXNumber
        '
        Me.CmbGwIEXNumber.FormattingEnabled = True
        Me.CmbGwIEXNumber.Items.AddRange(New Object() {"1", "2", "3", "4", "5", "6", "7", "8"})
        Me.CmbGwIEXNumber.Location = New System.Drawing.Point(123, 71)
        Me.CmbGwIEXNumber.Name = "CmbGwIEXNumber"
        Me.CmbGwIEXNumber.Size = New System.Drawing.Size(43, 21)
        Me.CmbGwIEXNumber.TabIndex = 45
        '
        'CmbGateway
        '
        Me.CmbGateway.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.CmbGateway.FormattingEnabled = True
        Me.CmbGateway.Location = New System.Drawing.Point(10, 46)
        Me.CmbGateway.Name = "CmbGateway"
        Me.CmbGateway.Size = New System.Drawing.Size(425, 21)
        Me.CmbGateway.TabIndex = 22
        '
        'LblCurGateway
        '
        Me.LblCurGateway.AutoSize = True
        Me.LblCurGateway.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.LblCurGateway.ForeColor = System.Drawing.SystemColors.ActiveCaption
        Me.LblCurGateway.Location = New System.Drawing.Point(73, 25)
        Me.LblCurGateway.Name = "LblCurGateway"
        Me.LblCurGateway.Size = New System.Drawing.Size(115, 13)
        Me.LblCurGateway.TabIndex = 36
        Me.LblCurGateway.Text = "50 50 50 50 50 50 "
        '
        'ChkClean
        '
        Me.ChkClean.AutoSize = True
        Me.ChkClean.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.ChkClean.ForeColor = System.Drawing.Color.Red
        Me.ChkClean.Location = New System.Drawing.Point(11, 455)
        Me.ChkClean.Name = "ChkClean"
        Me.ChkClean.Size = New System.Drawing.Size(189, 17)
        Me.ChkClean.TabIndex = 47
        Me.ChkClean.Text = "DEPLOY TO NEW MACHINE"
        Me.ChkClean.UseVisualStyleBackColor = True
        '
        'Form1
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(548, 473)
        Me.Controls.Add(Me.GrpClient)
        Me.Controls.Add(Me.ChkClean)
        Me.Controls.Add(Me.GroupBox5)
        Me.Controls.Add(Me.GroupBox2)
        Me.Controls.Add(Me.GrpBuilds)
        Me.Controls.Add(Me.TabProject)
        Me.Controls.Add(Me.LblStatus)
        Me.Controls.Add(Me.ButDeploy)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow
        Me.Name = "Form1"
        Me.Text = "Deployer"
        Me.TabProject.ResumeLayout(False)
        Me.GroupBox5.ResumeLayout(False)
        Me.GroupBox5.PerformLayout()
        Me.GroupBox2.ResumeLayout(False)
        Me.GroupBox2.PerformLayout()
        Me.GrpBuilds.ResumeLayout(False)
        Me.GrpBuilds.PerformLayout()
        Me.GroupBox3.ResumeLayout(False)
        Me.GroupBox3.PerformLayout()
        Me.GroupBox1.ResumeLayout(False)
        Me.GroupBox1.PerformLayout()
        Me.GrpClient.ResumeLayout(False)
        Me.GrpClient.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents ButDeploy As System.Windows.Forms.Button
    Friend WithEvents LblStatus As System.Windows.Forms.Label
    Friend WithEvents Timer1 As System.Windows.Forms.Timer
    Friend WithEvents TabProject As System.Windows.Forms.TabControl
    Friend WithEvents TabPage2 As System.Windows.Forms.TabPage
    Friend WithEvents TabPage1 As System.Windows.Forms.TabPage
    Friend WithEvents GroupBox5 As System.Windows.Forms.GroupBox
    Friend WithEvents TxtLog As System.Windows.Forms.TextBox
    Friend WithEvents GroupBox2 As System.Windows.Forms.GroupBox
    Friend WithEvents Label10 As System.Windows.Forms.Label
    Friend WithEvents TxtUserName As System.Windows.Forms.TextBox
    Friend WithEvents LblUserName As System.Windows.Forms.Label
    Friend WithEvents GrpBuilds As System.Windows.Forms.GroupBox
    Friend WithEvents LblCurClient As System.Windows.Forms.Label
    Friend WithEvents LblCurGateway As System.Windows.Forms.Label
    Friend WithEvents Label6 As System.Windows.Forms.Label
    Friend WithEvents Label7 As System.Windows.Forms.Label
    Friend WithEvents ButClient As System.Windows.Forms.Button
    Friend WithEvents CmbGateway As System.Windows.Forms.ComboBox
    Friend WithEvents CmbClient As System.Windows.Forms.ComboBox
    Friend WithEvents ButGateway As System.Windows.Forms.Button
    Friend WithEvents RadIEXTG As System.Windows.Forms.RadioButton
    Friend WithEvents RadTG As System.Windows.Forms.RadioButton
    Friend WithEvents RadOriginal As System.Windows.Forms.RadioButton
    Friend WithEvents ChkClean As System.Windows.Forms.CheckBox
    Friend WithEvents TabPage3 As System.Windows.Forms.TabPage
    Friend WithEvents GrpClient As System.Windows.Forms.GroupBox
    Friend WithEvents ButClDeploy As System.Windows.Forms.Button
    Friend WithEvents ButGwDeploy As System.Windows.Forms.Button
    Friend WithEvents TxtClBuildPath As System.Windows.Forms.TextBox
    Friend WithEvents LblClient As System.Windows.Forms.Label
    Friend WithEvents Label4 As System.Windows.Forms.Label
    Friend WithEvents TxtFrom As System.Windows.Forms.TextBox
    Friend WithEvents TxtGwBuildPath As System.Windows.Forms.TextBox
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents RadCOGECO As System.Windows.Forms.RadioButton
    Friend WithEvents RadUPC As System.Windows.Forms.RadioButton
    Friend WithEvents GroupBox3 As System.Windows.Forms.GroupBox
    Friend WithEvents CmbClIEXNumber As System.Windows.Forms.ComboBox
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents GroupBox1 As System.Windows.Forms.GroupBox
    Friend WithEvents CmbGwIEXNumber As System.Windows.Forms.ComboBox
    Friend WithEvents CmbMClIEXNumber As System.Windows.Forms.ComboBox
    Friend WithEvents LblClientIEXNumber As System.Windows.Forms.Label
    Friend WithEvents CmbMGwIEXNumber As System.Windows.Forms.ComboBox
    Friend WithEvents Label3 As System.Windows.Forms.Label
    Friend WithEvents TabPage4 As System.Windows.Forms.TabPage

End Class
