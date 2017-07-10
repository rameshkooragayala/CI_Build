using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Reflection;
using IEX.ElementaryActions.Functionality;

namespace IEX.Tests.Engine
{
    public class TestsScheduler
    {
        #region Variables

        private string _project = "";
        private TestsGroupStatus _testsGroupStatus = TestsGroupStatus.PASSED;
        private readonly XmlInputParser _xmlInputParser = new XmlInputParser();   //if the input is xml path help to parse the arguments from the xml.

        //Lockers
        private readonly object locker = new object();
        private readonly object tearDownLocker = new object();

        //Flags
        private bool _waitAfterMount = true;    //defualt wait after mount to other tests
        private int _testDuration = -1;
        private bool _debugMode;
        private bool _stopOnFailure;
        private bool _lastGroupOfTests = false;
        private const int MOUNT_FAILURE = -2;   //return in function RunTest when mount fails
        private EnumMountAs _mountFlag = EnumMountAs.FORMAT;

        //Data structures
        private readonly List<List<Dictionary<string, string>>> _allGroupsArgs = new List<List<Dictionary<string, string>>>();  //arguments for all test groups
        private TestsGroup _currentGroup = new TestsGroup();
        private List<TestsGroup> _groupsList = new List<TestsGroup>();
        private readonly HashSet<int> platformsAlreadyTearedDown = new HashSet<int>();  //used in the end of the execution to see if there are platforms that not teared down

        //Mount synchronization
        private int numberOfRunningTests;
        private int numberOfFinishedMount = 0;
        private readonly ManualResetEvent waitHandle = new ManualResetEvent(false);
        private readonly object afterMountLocker = new object();
        private bool allTestsFinishedMount = false;
        private bool setWasActivate = false;    //did we set the waitHandle already?

        #endregion

        #region Properties

        public string Project
        {
            get { return _project; }
        }

        public TestsGroupStatus TestsGroupStatus
        {
            get { return _testsGroupStatus; }
            set { _testsGroupStatus = value; }
        }
        #endregion

        #region == Arguments Parsing Functions ==
        #region ParseArgs
        /// <summary>
        /// Encapsulates the validation and parsing of tests arguments. call to CreatePlatforms
        /// </summary>
        /// <param name="args"></param>
        public void ParseArgs(string[] args)
        {
            ParseArgsInternal(args);

            foreach (var groupArgs in _allGroupsArgs)
            {
                foreach (var testArgs in groupArgs)
                {
                    ValidateTestArgs(testArgs);
                }

                _groupsList.Add(_currentGroup);
                _currentGroup = new TestsGroup();
            }

            CreatePlatforms();
        }
        #endregion

        #region ParseArgsInternal
        /// <summary>
        /// Parses all tests arguments and saves it in data structure
        /// </summary>
        /// <param name="args">all the tests arguments to parse or xml configuration file</param>
        private void ParseArgsInternal(string[] args)
        {
            if (args[0].EndsWith(".xml"))
            {
                GetDataFromXml(ref args);
            }

            const string groupsDelimiter = "+";  //separate between groups of tests (for sequence)
            const string testsDelimiter = "#"; //separate between tests (for parallel)
            int testIdentifier = 0;
            int clientNumber = 1;
            var testArgs = new Dictionary<string, string>(); //arguments for single test
            List<Dictionary<string, string>> groupArgs = new List<Dictionary<string, string>>(); //arguments for group
            int groupIndex = 0;

            for (int j = 0; j < args.Length; j++)
            {
                if (args[j].Equals("-g"))
                {
                    testArgs.Add(args[j], args[j + 2]); //add <"-g",IexNum> to dictionary
                    testArgs.Add("gatewayIP", args[j + 1]);   //add <"gatewayIP",IexServer> to dictionary
                }

                if (args[j].Equals("-s"))
                {
                    testArgs.Add(args[j] + clientNumber.ToString(), args[j + 2]); //add <"-s1",IexNum> to dictionary
                    testArgs.Add("clientIP" + clientNumber.ToString(), args[j + 1]);   //add <"clientIP1",IexServer> to dictionary
                    clientNumber++;
                }

                if (args[j].Equals("-t"))
                {
                    testArgs.Add(args[j], args[j + 1]);    //add <"-t",testName>
                }

                if (args[j].Equals("/d"))
                {
                    _debugMode = true;
                }

                if (args[j].Equals("/s") || args[j].Equals("/stopOnFailure"))
                {
                    _stopOnFailure = true;
                }

                //EnumMountAs options (format, no format, factory reset, no format no reboot)
                if (args[j].Equals("/f") || args[j].Equals("/n") || args[j].Equals("/r") || args[j].Equals("/b") || args[j].Equals("/nw"))
                {
                    _currentGroup.RequireMount = true;
                    testArgs.Add("mountFlag", args[j]);
                }

                if (args[j].Equals("/w"))
                {
                    _waitAfterMount = false;
                }

                //end of specific test parameters
                if (args[j].Equals(testsDelimiter) && j != args.Length - 1)
                {

                    testArgs.Add("identifier", testIdentifier.ToString());
                    testIdentifier++;
                    clientNumber = 1;

                    groupArgs.Add(new Dictionary<string, string>(testArgs));
                    testArgs.Clear();
                }

                //end of specific group parameters
                if (args[j].Equals(groupsDelimiter) || j == args.Length - 1)
                {
                    testArgs.Add("identifier", testIdentifier.ToString());
                    testIdentifier = 0;
                    clientNumber = 1;
                    groupIndex++;

                    groupArgs.Add(new Dictionary<string, string>(testArgs));
                    testArgs.Clear();

                    _allGroupsArgs.Add(new List<Dictionary<string, string>>(groupArgs));
                    groupArgs.Clear();
                }
            }
        }

        #region GetDataFromXml
        private void GetDataFromXml(ref string[] args)
        {
            var fi = new FileInfo(args[0]);
            if (!fi.Exists)
            {
                throw new Exception(args[0] + " not exists");
            }

            bool debugMode = false;
            bool stopOnFailure = false;
            bool waitAfterMount = true;

            var tests = _xmlInputParser.RestoreTests(args[0], ref debugMode, ref stopOnFailure, ref waitAfterMount, ref _testDuration);

            string builder = "";
            if (args.Length < 3) //default behavior from the xml file
            {
                for (int i = 0; i < tests.Count - 1; i++)
                {
                    builder += tests[i].args;
                    builder += "# ";
                }
                builder += tests[tests.Count - 1].args;
            }
            else
            {
                string gateway = "";
                if (args[1] == "-g")
                {
                    gateway += args[1] + " " + args[2] + " " + args[3] + " ";
                }
                else
                {
                    throw new Exception("Bad arguments: When override xml parameters args[1] should be -g");
                }

                int currentArg = 4;

                for (int i = 0; i < tests.Count; i++)
                {
                    int clientCounter = 1;
                    if (tests[i].argsDictionary.ContainsKey("-t"))
                    {
                        builder += "-t " + tests[i].argsDictionary["-t"] + " ";
                    }
                    else
                    {
                        throw new Exception("Wrong XML file parameters: Missed -t <TestName> in one of the tests");
                    }

                    if (tests[i].argsDictionary.ContainsKey("-g"))
                    {
                        builder += gateway;
                    }
                    else
                    {
                        throw new Exception("Wrong XML file parameters: Missed -g in one of the tests");
                    }

                    //add clients
                    try
                    {
                        while (true)
                        {
                            if (tests[i].argsDictionary.ContainsKey("-s" + clientCounter.ToString()))
                            {
                                builder += args[currentArg] + " " + args[currentArg + 1] + " " + args[currentArg + 2] + " ";
                                currentArg += 3;
                                clientCounter++;
                            }
                            else
                            {
                                break;
                            }
                        }
                    }
                    catch (Exception)
                    {
                        throw new Exception("Bad arguments: Not enough clients for test set " + args[0]);
                    }

                    if (i < tests.Count - 1)
                    {
                        builder += "# ";
                    }
                }

                while (currentArg < args.Length)
                {
                    if (args[currentArg].Equals("/d"))
                    {
                        debugMode = true;
                    }

                    if (args[currentArg].Equals("/s") || args[currentArg].Equals("/stopOnFailure"))
                    {
                        stopOnFailure = true;
                    }

                    if (args[currentArg].Equals("/w"))
                    {
                        waitAfterMount = false;
                    }
                    currentArg++;
                }
            }

            if (debugMode)
            {
                builder += " /d ";
            }
            if (stopOnFailure)
            {
                builder += " /s ";
            }
            if (!waitAfterMount)
            {
                builder += " /w ";
            }
            args = builder.Split(' ');
        }
        #endregion
        #endregion

        #region ValidateTestArgs
        /// <summary>
        /// Validate specific test args by examining its associated dictionary  and create platformContext if needed
        /// </summary>
        /// <param name="testArgs">dictionary where test args is stored</param>
        private void ValidateTestArgs(Dictionary<string, string> testArgs)
        {
            string testName = "";
            short iexNum = -1;
            System.Net.IPHostEntry remoteHost;
            System.Net.IPAddress ip;
            string Server;
            int testIdentifier = -1;

            if (testArgs.ContainsKey("identifier"))
            {
                testIdentifier = Convert.ToInt32(testArgs["identifier"]);
            }

            if (testArgs.ContainsKey("-t"))
            {
                testName = testArgs["-t"];
            }
            else
            {
                throw new FormatException("Test parameters must include: -t <testname>");
            }

            bool clientIsGateway = false;
            int clientNumber = 1;
            while (true)        //iterate throw all clients of single test. The clients appear in testArgs as testArgs[s1],testArgs[s2] etc...
            {
                if (testArgs.ContainsKey("-s" + clientNumber.ToString()))   //starts with testArgs[s1]
                {
                    if (!short.TryParse(testArgs["-s" + clientNumber.ToString()], out iexNum))
                    {
                        throw new FormatException("Type mismatch for IEX Server Number property");
                    }

                    try
                    {
                        remoteHost = System.Net.Dns.GetHostEntry(testArgs["clientIP" + clientNumber.ToString()]);
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }

                    if (!System.Net.IPAddress.TryParse(testArgs["clientIP" + clientNumber.ToString()], out ip))
                    {
                        Server = remoteHost.HostName;
                    }
                    else
                    {
                        Server = ip.ToString();
                    }

                    SaveClientContextData(iexNum, Server, testName, testIdentifier);
                }
                else
                {
                    if (clientNumber == 1)     //s1 not exists in dictionary => clientIsGateway
                        clientIsGateway = true;
                    break;
                }
                clientNumber++;
            }

            if (testArgs.ContainsKey("-g"))
            {
                if (!short.TryParse(testArgs["-g"], out iexNum))
                {
                    throw new FormatException("Type mismatch for IEX Server Number property");
                }

                try
                {
                    remoteHost = System.Net.Dns.GetHostEntry(testArgs["gatewayIP"]);
                }
                catch (Exception ex)
                {
                    throw ex;
                }

                if (!System.Net.IPAddress.TryParse(testArgs["gatewayIP"], out ip))
                {
                    Server = remoteHost.HostName;
                }
                else
                {
                    Server = ip.ToString();
                }

                SaveGatewayContextData(iexNum, clientIsGateway, testName, testIdentifier, Server);
            }
            else
            {
                throw new FormatException("Test parameters must include: -g <IP> <IEX number>");
            }

           // if (String.IsNullOrEmpty(_project))
           // {
           //     var iniFile = new AMS.Profile.Ini("C:\\Program Files\\IEX\\Tests\\TestsINI\\IEX" + iexNum.ToString() + "\\Environment.ini");
            //    _project = iniFile.GetValue("IEX" + iexNum.ToString(), "PROJECT").ToString();
           // }
        }
        #endregion
        #endregion

        #region CreatePlatforms
        private void CreatePlatforms()
        {
            int clientCounter = 1;
            var initializedPlatforms = new Dictionary<int, _Platform>();  //saves for each iex that make connect the platform object. 
            var testIdentifierLog = new Dictionary<int, string>();

            foreach (TestsGroup group in _groupsList)
            {
                foreach (PlatformContext pContext in group.pContextList)
                {
				
					var iniFile = new AMS.Profile.Ini("C:\\Program Files\\IEX\\Tests\\TestsINI\\IEX" + pContext.iexNumber.ToString() + "\\Environment.ini");
                    _project = iniFile.GetValue("IEX" + pContext.iexNumber.ToString(), "PROJECT").ToString();

                    if (initializedPlatforms.ContainsKey(pContext.iexNumber)) //checks if platform already initialized
                    {
                        if (pContext.Platform == null)
                        {
                            pContext.Platform = initializedPlatforms[pContext.iexNumber]; //platform with iex number pContext.IexNumber already initialized, reference to that platform.
                        }
                        continue;
                    }

                    string testName = pContext.Tests[0].TestName;
                    int testID = pContext.Tests[0].Identifier;
                    foreach (TestData t in pContext.Tests)
                    {
                        if (t.GatewayIsClient)
                        {
                            testName = t.TestName; //gateway log will be in the test where it acts as client
                            testID = t.Identifier;
                        }
                    }

                    pContext.Platform = new _Platform();
                    if (pContext.Tests[0].Type == PlatformType.CL)
                    {
                        pContext.Platform.ID = "Client" + clientCounter.ToString();
                        clientCounter++;
                    }
                    else
                    {
                        pContext.Platform.ID = "Gateway";
                    }

                    if (!testIdentifierLog.ContainsKey(testID))
                    {
                        string now;
                        DateTime date = DateTime.Now;
                        now = date.ToString("dd/MM/yyyy HH:mm:ss tt");
                        now = now.Replace("/", "-").Replace("\\", "-").Replace(":", ".").Replace(" ", "_");
                        testIdentifierLog.Add(testID, testName + "_" + now);
                    }

                    pContext.LogPath = testIdentifierLog[testID];
                    pContext.Platform.Init(pContext.iexServer, pContext.LogPath, _project);  //platform init
                    initializedPlatforms.Add(pContext.iexNumber, pContext.Platform);
                }
                testIdentifierLog.Clear();
            }
        }
        #endregion

        #region ExecuteTests
        /// <summary>
        /// Create the tests instances and run the tests in parallel
        /// </summary>
        public void ExecuteTests()
        {
            string callingAssembly = Assembly.GetCallingAssembly().ToString();

            //create tasks => each task represent single test to run
            for (int groupIndex = 0; groupIndex < _allGroupsArgs.Count; groupIndex++)
            {
                if (groupIndex == _allGroupsArgs.Count - 1)
                {
                    _lastGroupOfTests = true;
                }

                UpdatePlatformsLogPath(groupIndex);

                List<Dictionary<string, string>> groupArgs = _allGroupsArgs[groupIndex];
                var tasks = new Task[groupArgs.Count];

                InitializeNextIteration(groupArgs.Count);

                for (int i = 0; i < groupArgs.Count; i++)
                {
                    lock (locker)
                    {
                        int identifier = i;
                        tasks[identifier] = Task.Factory.StartNew<int>(() => RunTest(groupArgs[identifier], identifier, callingAssembly));
                    }
                }
                Task.WaitAll(tasks);

                if (MountFailure(tasks))
                    break;
            }

            ConfirmTearedDown();   //confirm that all platforms have teared down
        }
        #endregion

        #region RunTest
        private int RunTest(object _args, int identifier, string callingAssembly)
        {
            var args = (Dictionary<string, string>)_args;
            string testName = "";
            _Test t = null;

            if (args.ContainsKey("-t"))
            {
                testName = args["-t"];
            }
            else
            {
                Console.WriteLine(@"USAGE : <File.exe> -t TestName -g IEXServer IEXNumber [-s IEXServer IEXNumber]");
                return -1;
            }

            if (args.ContainsKey("mountFlag"))
            {
                switch (args["mountFlag"])
                {
                    case "/n":
                        _mountFlag = EnumMountAs.NOFORMAT;
                        break;
                    case "/r":
                        _mountFlag = EnumMountAs.FACTORY_RESET;
                        break;
                    case "/b":
                        _mountFlag = EnumMountAs.NOFORMAT_NOREBOOT;
                        break;
                    case "/nw":
                        _mountFlag = EnumMountAs.NOFORMAT_WAKEUP;
                        break;
                    case "/f":
                    default:
                        _mountFlag = EnumMountAs.FORMAT;
                        break;
                }
            }

            try
            {
                var testWrapper = Activator.CreateInstance(callingAssembly, testName);      //create test instance
                t = (_Test)testWrapper.Unwrap();
                t.TestIdentifier = identifier;

                InitializeTestParams(t);
                try
                {
                    t.Init();
                    SynchronizeTestsAfterMount();
                    t.Run();
                }
                catch (Exception ex)
                {
                    SetFailedMessage(t);
                    throw ex;
                }
                return 0;
            }
            catch (Exception ex)
            {
                if (ex is MountException)
                {
                    Console.WriteLine(@"Mount Failed " + ex.Message);
                    return MOUNT_FAILURE;
                }
                else if (!(ex is FailureException))
                {
                    Console.WriteLine(@"Exception Occurred While Executing The Program: " + ex.Message);
                }
                return -1;
            }
            finally
            {
                try
                {
                    foreach (var platformContext in t.TestPlatforms)
                    {
                        TearDownPlatform(platformContext.Platform);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(@"Exception Occurred While Executing Tear Down: " + ex.Message);
                }
            }
        }
        #endregion

        #region InitializeTestParams
        private void InitializeTestParams(_Test t)
        {
            t.TestPlatforms = GetAllPlatforms(t.TestIdentifier);
            t.DebugMode = _debugMode;
            t.TestDuration = _testDuration;
            t.MountFlag = _mountFlag;
        }
        #endregion

        #region SynchronizeTestsAfterMount
        private void SynchronizeTestsAfterMount()
        {
            if (!_waitAfterMount) return;

            lock (afterMountLocker)
            {
                numberOfFinishedMount++;
                if (numberOfFinishedMount == numberOfRunningTests)
                {
                    allTestsFinishedMount = true;
                }
            }

            //wait until all tests finished mount
            if (!allTestsFinishedMount)
            {
                waitHandle.WaitOne();
            }
            else
            {
                waitHandle.Set();
                setWasActivate = true;
            }
        }
        #endregion

        #region MountFailure
        /// <summary>
        /// Checks if in this group of tests was mount failure if yes not continue the execution of other groups
        /// </summary>
        /// <param name="tasks">the tasks to check their return value</param>
        /// <returns></returns>
        private bool MountFailure(Task[] tasks)
        {
            bool stopDueToMountFailure = false;

            for (int i = 0; i < tasks.Length; i++)
            {
                Task<int> task = (Task<int>)tasks[i];
                if (task.Result == MOUNT_FAILURE)
                {
                    stopDueToMountFailure = true;
                }
            }

            if (stopDueToMountFailure)
                return true;
            return false;
        }
        #endregion

        #region GetAllPlatforms
        /// <summary>
        /// return all the platforms that are related to specific test 
        /// </summary>
        /// <param name="testId">identifier of the test</param>
        /// <returns>List of platformContext that have a platform property each.</returns>
        private List<PlatformContext> GetAllPlatforms(int testId)
        {
            return (from p in _currentGroup.pContextList
                    from t in p.Tests
                    where t.Identifier == testId
                    select p).ToList();
        }
        #endregion

        #region PlatformAlreadyExists
        /// <summary>
        /// Checks by iex number if platform exists 
        /// </summary>
        /// <returns>If exists return the index in the _currentGroup.pContextList, if not -1</returns>
        private int PlatformAlreadyExists(short iexNum)
        {
            for (int i = 0; i < _currentGroup.pContextList.Count; i++)
            {
                if (_currentGroup.pContextList[i].iexNumber == iexNum)
                    return i;
            }
            return -1;
        }
        #endregion

        #region UpdatePlatformsLogPath
        private void UpdatePlatformsLogPath(int groupIndex)
        {
            _currentGroup.pContextList = _groupsList[groupIndex].pContextList;  //the platforms context corresponding to this group of tests

            if (groupIndex != 0)
            {
                foreach (var pContext in _currentGroup.pContextList)
                {
                    if (pContext.LogPath != null) continue;

                    string testName = pContext.Tests[0].TestName;
                    int testID = pContext.Tests[0].Identifier;
                    foreach (TestData t in pContext.Tests)
                    {
                        if (t.GatewayIsClient)
                        {
                            testName = t.TestName; //gateway log will be in the test where it acts as client
                            testID = t.Identifier;
                        }
                    }
                    string newTestName = (from p in _currentGroup.pContextList
                                          where p.Tests[0].Identifier == testID && p.LogPath != null
                                          select p.LogPath).FirstOrDefault();

                    if (newTestName == null)
                    {
                        string now;
                        DateTime date = DateTime.Now;
                        now = date.ToString("dd/MM/yyyy HH:mm:ss tt");
                        now = now.Replace("/", "-").Replace("\\", "-").Replace(":", ".").Replace(" ", "_");
                        newTestName = testName + "_" + now;
                    }

                    pContext.LogPath = newTestName;
                    pContext.Platform.EA.TestName = newTestName;
                    pContext.Platform.EA.ChangeLogFileName(pContext.Platform.EA.LogFilePath);
                }

                if (_currentGroup.RequireMount)
                {
                    _debugMode = false;
                }
                else
                {
                    _debugMode = true;
                }
            }
        }
        #endregion

        #region InitializeNextIteration
        private void InitializeNextIteration(int runningTestsCount)
        {
            numberOfRunningTests = runningTestsCount;
            waitHandle.Reset();
            numberOfFinishedMount = 0;
            setWasActivate = false;
            allTestsFinishedMount = false;
        }
        #endregion

        #region SetFailedMessage
        private void SetFailedMessage(_Test t)
        {
            _testsGroupStatus = TestsGroupStatus.FAILED;
            if (_stopOnFailure)
            {
                lock (locker)
                {
                    if (!_Test.IsCanceled)
                    {
                        _Test.IsCanceled = true;
                        _Test.CancellationToken.Cancel();
                        _Test.CancelationReason = "Cancel Test Because of " + t.TestName + " Failure";
                    }
                }
            }
        }
        #endregion

        #region SaveGatewayContextData
        private void SaveGatewayContextData(short iexNum, bool clientIsGateway, string testName, int testIdentifier, string Server)
        {
            TestData testData;

            if (!clientIsGateway)
            {
                testData = new TestData(testName, PlatformType.GW, testIdentifier);
            }
            else
            {
                testData = new TestData(testName, PlatformType.CL, testIdentifier);
                testData.GatewayIsClient = true;
            }

            int index = PlatformAlreadyExists(iexNum);
            if (index >= 0) //platform exists already
            {
                _currentGroup.pContextList[index].Tests.Add(testData);
                _currentGroup.pContextList[index].TestsCounter++;
                //   _currentGroup.pContextList[index].IexServer = Server + ":" + iexNum.ToString();
            }
            else
            {
                PlatformContext pNew = new PlatformContext();
                pNew.iexNumber = iexNum;
                pNew.iexServer = Server + ":" + iexNum.ToString();
                pNew.TestsCounter++;
                pNew.Tests.Add(testData);
                _currentGroup.pContextList.Add(pNew);
            }
        }
        #endregion

        #region SaveClientContextData
        private void SaveClientContextData(short iexNum, string Server, string testName, int testIdentifier)
        {
            TestData testData = new TestData(testName, PlatformType.CL, testIdentifier);

            int index = PlatformAlreadyExists(iexNum);
            if (index >= 0) //platform exists already
            {
                _currentGroup.pContextList[index].TestsCounter++;
                //  _currentGroup.pContextList[index].IexServer = Server + ":" + iexNum.ToString();
                _currentGroup.pContextList[index].Tests.Add(testData);
            }
            else
            {
                var pNew = new PlatformContext();
                pNew.iexNumber = iexNum;
                pNew.iexServer = Server + ":" + iexNum.ToString();
                pNew.TestsCounter++;
                pNew.Tests.Add(testData);
                _currentGroup.pContextList.Add(pNew);
            }
        }
        #endregion

        #region == Tear Down Functions ==
        #region TearDownPlatform
        /// <summary>
        /// checks if this is the last test to run on that platform, if yes teardown the ea and disconnect.
        /// if no decrement the test counter of that platform by one.(only when last group of tests)
        /// </summary>
        /// <param name="p">the platform to TearDown</param>
        private void TearDownPlatform(_Platform p)
        {
            if (!setWasActivate)
            {
                waitHandle.Set();
                setWasActivate = true;
            }

            if (!_lastGroupOfTests) return;

            foreach (PlatformContext pContext in _currentGroup.pContextList)
            {
                if (pContext.Platform == p)
                {
                    lock (tearDownLocker)
                    {
                        if (pContext.TestsCounter == 1)
                        {
                            TearDown(p);
                        }
                        else
                        {
                            pContext.TestsCounter--;
                        }
                    }
                }
            }
        }
        #endregion
        #region TearDown
        private void TearDown(_Platform p)
        {
            try
            {
                p.EA.TearDown();
                p.IEX.Disconnect();
                platformsAlreadyTearedDown.Add(p.IEX.IEXServerNumber);
                int timeOut = 2000;
                do
                {
                    Thread.Sleep(2000);
                    timeOut += 2000;
                } while (timeOut < 30000 && p.IEX.IsConnected);
            }
            finally
            {
                if (p.IEX.IsConnected)
                {
                    throw new Exception("Failed to Disconnect IEX Server " + p.IEX.IEXServerNumber.ToString());
                }
            }
        }
        #endregion
        #region ConfirmTearedDown
        private void ConfirmTearedDown()
        {
            foreach (var pContextList in _groupsList)
            {
                foreach (var platformContext in pContextList.pContextList)
                {
                    int iexNumber = platformContext.iexNumber;
                    if (platformsAlreadyTearedDown.Contains(iexNumber))
                        continue;
                    TearDown(platformContext.Platform);
                }
            }
        }
        #endregion
        #endregion

        #region Usage
        public void Usage()
        {
            Console.WriteLine("Usage:");
            Console.WriteLine("There are 3 options to execute tests: \n");
            Console.WriteLine("Option 1: Single Test Execution");
            Console.WriteLine("-t TestName -g IEXServer IEXNumber [-s IEXServer IEXNumber] [Flags]");
            Console.WriteLine("Example: -t Test1 -g 127.0.0.1 1 -s 127.0.0.1 2 \n");

            Console.WriteLine("Option 2: Parallel Tests Execution");
            Console.WriteLine("<Single Test Parameter> # <Single Test Parameter>");
            Console.WriteLine("Example: -t Test1 -g 127.0.0.1 1 -s 127.0.0.1 2 # -t Test2 -g 127.0.0.1 1 -s 127.0.0.1 3 \n");

            Console.WriteLine("Option 3: Sequential Tests Execution");
            Console.WriteLine("<Set1> + <Set2> + <Set3>");
            Console.WriteLine("Set can be single test or group of test (which will be execute in parallel) \n\n");

            Console.WriteLine("Flags:");
            Console.WriteLine("/d                       Debug mode, no need to do mount");
            Console.WriteLine("/s or /stopOnFailure     When one test fails abort all other tests");
            Console.WriteLine("/w                       When running number of test in parallel don't wait to other tests to finish mount");
            Console.WriteLine("/f                       Call mount with FORMAT flag");
            Console.WriteLine("/n                       Call mount with NO FORMAT flag");
            Console.WriteLine("/r                       Call mount with FACTORY RESET flag");
            Console.WriteLine("/b                       Call mount with NO FORMAT NO REBOOT flag");

            // Console.WriteLine("\nPlease press any key to continue");
            // Console.ReadKey();
        }
        #endregion
    }

    #region Private Classes
    #region PlatformContext
    /// <summary>
    /// Wrap the _Platform class and add extra info about it: list of tests that run on specific platform, iexNumber etc...
    /// </summary>
    internal class PlatformContext
    {
        internal List<TestData> Tests;
        internal string LogPath;
        internal bool isMount;

        internal _Platform Platform { get; set; }
        internal int TestsCounter { get; set; }
        internal short iexNumber { get; set; }
        internal string iexServer { get; set; }

        internal PlatformContext()
        {
            TestsCounter = 0;
            Tests = new List<TestData>();
            isMount = false;
            LogPath = null;
        }
    }
    #endregion

    #region TestData
    /// <summary>
    /// Contain data about specific test.
    /// 
    /// </summary>
    internal class TestData
    {
        private bool _gatewayIsClient = false;
        private bool _isInUse = false;

        internal bool GatewayIsClient
        {
            get { return _gatewayIsClient; }
            set { _gatewayIsClient = value; }
        }

        /// <summary>
        /// Tells if specific client is already running
        /// </summary>
        internal bool InUse
        {
            get { return _isInUse; }
            set { _isInUse = value; }
        }

        internal string TestName { get; set; }
        internal PlatformType Type { get; set; }
        internal int Identifier { get; set; }

        internal TestData(string testName, PlatformType type, int testIdentifier)
        {
            this.TestName = testName;
            this.Type = type;
            this.Identifier = testIdentifier;
        }
    }
    #endregion

    #region TestsGroup
    class TestsGroup
    {
        internal List<PlatformContext> pContextList = new List<PlatformContext>();
        private bool _requireMount = false;

        internal bool RequireMount
        {
            get { return _requireMount; }
            set { _requireMount = value; }
        }
    }
    #endregion
    #endregion

    #region PlatformType
    internal enum PlatformType
    {
        GW,
        CL
    }
    #endregion
}
