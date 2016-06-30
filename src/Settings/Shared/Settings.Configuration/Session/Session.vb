﻿Imports System.Text.RegularExpressions
Imports Microsoft.VisualBasic.CommandLine.Reflection
Imports Microsoft.VisualBasic.ComponentModel.Settings
Imports Microsoft.VisualBasic.Scripting.MetaData

Namespace Settings

    ''' <summary>
    ''' GCModeller program profile session.(GCModeller的应用程序配置会话) 
    ''' </summary>
    ''' <remarks></remarks>
    ''' 
#If ENABLE_API_EXPORT Then
    <[PackageNamespace]("GCModeller.Profile.Session",
                    Description:="The application profile data session for the GCModeller application modules.",
                    Publisher:="amethyst.asuka@gcmodeller.org")>
    Module Session
#Else
    Module Session
#End If

        Dim _Worksapce As String = My.Computer.FileSystem.SpecialDirectories.Temp & "/GCModeller/" & My.Application.Info.AssemblyName.ToUpper & ".EXE"
        Dim _Cache As String = My.Application.Info.DirectoryPath & "/.cache/" & My.Application.Info.AssemblyName.ToUpper & ".EXE"
        Dim _initFlag As Boolean
        Dim _ProfileData As Settings(Of Settings.File)

        Public ReadOnly Property ProfileData As Settings(Of Settings.File)
            Get
                Return Session._ProfileData
            End Get
        End Property

        Public ReadOnly Property SHA256Provider As SecurityString.SHA256 =
            New SecurityString.SHA256("http://gcmodeller.org/", "GCModeller")

        ''' <summary>
        ''' Temp data directory for this application session.(本应用程序会话的临时数据文件夹)
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property TEMP As String
            Get
                Return _Worksapce
            End Get
        End Property

        ''' <summary>
        ''' Templates directory of the GCModeller inputs data.
        ''' (在这个文件夹里面存放的是一些分析工具的输入的数据的模板文件)
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property Templates As String = App.HOME & "/Templates/"

        Public ReadOnly Property SettingsFile As Settings.File
            Get
                Return ProfileData.SettingsData
            End Get
        End Property

        Public Function GetSettingsFile() As Settings.File
            If Not Initialized Then
                Call Initialize()
            End If

            Return Session.SettingsFile
        End Function

        ''' <summary>
        ''' The cache data directory for this application session.(本应用程序的数据缓存文件夹)
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property DataCache As String
            Get
                Return _Cache
            End Get
        End Property

        ''' <summary>
        ''' This property indicates that whether this application session is initialized or not.(应用程序是否已经初始化完毕)
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property Initialized As Boolean
            Get
                Return Session._initFlag
            End Get
        End Property

        ''' <summary>
        ''' Directory for stores the application log file.(存放应用程序的日志文件的文件系统目录)
        ''' </summary>
        ''' <remarks></remarks>
        Private ReadOnly _LogDir As String = My.Application.Info.DirectoryPath & "/.logs/"

        ''' <summary>
        ''' Directory for stores the application log file.(存放应用程序的日志文件的文件系统目录)
        ''' </summary>
        ''' <remarks></remarks>
        Public ReadOnly Property LogDIR As String
            Get
                Return _LogDir
            End Get
        End Property

        ''' <summary>
        ''' Data storage directory for the program settings.(配置文件所存放的文件夹)
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property SettingsDIR As String
            Get
                Return My.Application.Info.DirectoryPath & "/Settings"
            End Get
        End Property

        ''' <summary>
        ''' Initialize the application session and get the program profile data.(初始化应用程序会话，并获取应用程序的配置数据)
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        ''' 
        <ExportAPI("Init()", Info:="Initialize this application session.")>
        Public Function Initialize(<Parameter("Mappings.From.Assembly", "")> Optional Mapping As System.Type = Nothing) As Settings.File

            If Not Mapping Is Nothing Then
                Dim Assembly As System.Reflection.Assembly = Mapping.Assembly
                Call __initializePath(Assembly)
            Else
                Session._Worksapce = My.Computer.FileSystem.SpecialDirectories.Temp & "/GCModeller/" & My.Application.Info.AssemblyName.ToUpper & ".EXE"
                Session._Cache = My.Application.Info.DirectoryPath & "/.cache/" & My.Application.Info.AssemblyName.ToUpper & ".EXE"
            End If

            Call FileIO.FileSystem.CreateDirectory(SettingsDIR)
            Call FileIO.FileSystem.CreateDirectory(_LogDir)
            Call FileIO.FileSystem.CreateDirectory(TEMP)
            Call FileIO.FileSystem.CreateDirectory(DataCache)

            Dim settings As String = Session.SettingsDIR & "/Settings.xml"
            Dim saveHwnd As Action(Of Settings.File, String) = Sub(profile, path) profile.GetXml.SaveTo(path)

            Session._ProfileData = Microsoft.VisualBasic.ComponentModel.Settings.Settings(Of Settings.File).LoadFile(settings, saveHwnd)
            Session._initFlag = True

            Return SettingsFile
        End Function

        ''' <summary>
        ''' 首先尝试通过配置文件得到脚本环境，假若没有配置这个值，则会尝试通过本身程序来测试，因为这个函数的调用可能是来自于Shoal脚本的
        ''' </summary>
        ''' <returns></returns>
        Public Function TryGetShoalShellBin() As String
            If Not Initialized Then
                Call Initialize()
            End If

            If Session.SettingsFile.ShoalShell.FileExists Then
                Return Session.SettingsFile.ShoalShell
            Else
                Call Console.WriteLine($"[DEBUG {Now.ToString}] The shoal shell bin in the settings file is not a valid path value, try to using self as interpreter....")
            End If

            '没有找到，由于这个函数本身可能就是从Shoal脚本之中调用的，则尝试使用自身作为解释器程序
            Dim App As String = My.Application.Info.DirectoryPath & "/" & My.Application.Info.AssemblyName & ".exe"
            Dim AskVersion = New Microsoft.VisualBasic.CommandLine.IORedirectFile(App, "--version")
            Call AskVersion.Run()

            If Not Regex.Match(AskVersion.StandardOutput, "Shoal Shell \d+(\.\d+)*").Success Then
                Call Console.WriteLine($"[DEBUG {Now.ToString}] Could not found the ShoalShell interpreter environment!")
                Return ""
            Else
                Call Console.WriteLine($"[DEBUG {Now.ToString}] Test self ""{App}"" as the ShoalShell interpreter!")
            End If

            Session.SettingsFile.ShoalShell = App
            Session.SettingsFile.Save()

            Return App
        End Function

        <ExportAPI("Install.Java", Info:="Just specific the java.exe installed location to running some external Java program in the GCModeller. 
The path value of the java program usually is in the location like: ""C:\Program Files\Java\jre<version>\bin\java.exe""")>
        Public Function InstallJavaBin(<Parameter("Java.exe", "The exe path of the java program.")> java As String) As String
            If java.FileExists Then

                If Not Initialized Then
                    Call Session.Initialize()
                End If

                SettingsFile.Java = java
                SettingsFile.Save()
                Call Console.WriteLine($"Set up java.exe path to {java.CliPath}")

                Return java
            Else
                Call Console.WriteLine($"The Java path is not exists on {java.CliPath}!")
                Return ""
            End If
        End Function

        <ExportAPI("Install.Python")>
        Public Function InstallPython(Python As String) As String
            If Python.FileExists Then

                If Not Initialized Then
                    Call Session.Initialize()
                End If

                SettingsFile.Python = Python
                SettingsFile.Save()
                Call $"Set up python.exe path to {Python.CliPath}".__DEBUG_ECHO

                Return Python
            Else
                Call $"The {NameOf(Python)} path is not exists on {Python.CliPath}!".__DEBUG_ECHO
                Return ""
            End If
        End Function

        Private Sub __initializePath(Schema As System.Reflection.Assembly)
            Dim [Module] As String = Global.System.IO.Path.GetFileName(Schema.EscapedCodeBase).ToUpper

            Session._Worksapce = My.Computer.FileSystem.SpecialDirectories.Temp & "/GCModeller/" & [Module]
            Session._Cache = My.Application.Info.DirectoryPath & "/.cache/" & [Module]
        End Sub

        ''' <summary>
        ''' Close the application session and save the settings file.
        ''' </summary>
        ''' <remarks></remarks>
        ''' 
        <ExportAPI("Finallize", Info:="Close the application session and save the settings file.")>
        Public Sub Finallize()
            Call ProfileData.Save()
        End Sub

        <ExportAPI("Get.Settings")>
        Public Function GetSettings(Key As String) As String
            Return ProfileData.GetSettings(Name:=Key)
        End Function

#If Not DISABLE_BUG_UNKNOWN Then
        <ExportAPI("List.Settings", Info:="Listing all of the settings data to the user console.")>
        Public Function List() As Dictionary(Of String, String)
            Dim ChunkBuffer As BindMapping() = ProfileData.AllItems
            Dim data As Dictionary(Of String, String) = New Dictionary(Of String, String)

            For Each x In ChunkBuffer
                Call data.Add(x.Name, x.Value)
            Next
            Call Console.WriteLine(ConfigEngine.Prints(ChunkBuffer))
            Return data
        End Function
#End If

        <ExportAPI("Set.Settings", Info:="Writes the settings data into the GCModeller profile sessions.")>
        Public Function SetValue(name As String, value As String) As Boolean
            Return ProfileData.Set(name, value)
        End Function

        <ExportAPI("Profile.Save")>
        Public Sub Save()
            Call ProfileData.Save()
        End Sub

        ''' <summary>
        ''' For unawareless of overrides the original data file, this function will automatcly add a .std_out extension to the STDOUT filepath.
        ''' (新构建一个Shoal实例运行一个分支脚本任务，在.NET之中线程的效率要比进程的效率要低，使用这种多线程的方法来实现并行的效率要高很多？？？？)
        ''' </summary>
        ''' <param name="Script">脚本文件的文件文本内容</param>
        ''' <param name="STDOUT">Standard output redirect to this file. Facility the GCModeller debugging.</param>
        ''' <returns></returns>
        ''' 
        <ExportAPI("Exec")>
        Public Function FolkShoalThread(Script As String, STDOUT As String) As Integer
            If Not Initialized Then
                Call Initialize()
            End If

            Dim ShoalShell As String = Session.TryGetShoalShellBin
            Dim ScriptPath As String = FileIO.FileSystem.GetTempFileName

            Call Script.SaveTo(ScriptPath)

            STDOUT = STDOUT & ".std_out"
            Call $"{NameOf(STDOUT)} >>> {STDOUT.ToFileURL}".__DEBUG_ECHO

            Return New Microsoft.VisualBasic.CommandLine.IORedirectFile(
            ShoalShell,
            argv:=ScriptPath.CliPath,
            stdRedirect:=STDOUT).Run
        End Function
    End Module
End Namespace