using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using IEX.Utilities;
using IEX.IEXecuter.ExecutionEngine;
using IEXGateway;
using IEX.ElementaryActions.Functionality;

namespace IEX.Tests.Engine
{
    public abstract class _Test
    {
        #region Variables

        private string _testName;
        private TestStatus _testStatus;
        private int _testIdentifier = -1;
        private readonly ArrayList _steps = new ArrayList();
        private ResultXML _xmlLog;
        private List<PlatformContext> _testPlatforms = new List<PlatformContext>();
        private EnumMountAs _mountFlag = EnumMountAs.FORMAT;

        static readonly object GatewayLocker = new object(); //Lock for mount
        static readonly object LoggingLocker = new object();
        static bool startTracer;

        public int TestDuration = 0;

        public static readonly CancellationTokenSource CancellationToken = new CancellationTokenSource();
        public static bool IsCanceled;
        public static string CancelationReason = "";

        #endregion

        #region Properties
        public _Test()
        {
            _testName = this.ToString();
        }

        public TestStatus TestStatus
        {
            get { return _testStatus; }
        }

        public string TestName
        {
            get { return _testName; }
            set { _testName = value; }
        }

        public int TestIdentifier
        {
            get { return _testIdentifier; }
            set { _testIdentifier = value; }
        }

        public EnumMountAs MountFlag
        {
            get { return _mountFlag; }
            set { _mountFlag = value; }
        }

        public bool DebugMode { get; set; }

        public List<_Platform> Platforms
        {
            get { return (from p in TestPlatforms select p.Platform).ToList(); }
        }

        public bool SingleBoxTest
        {
            get { return (this.Platforms.Count == 1 ? true : false); }
        }

        internal List<PlatformContext> TestPlatforms
        {
            get { return _testPlatforms; }
            set { _testPlatforms = value; }
        }
        #endregion

        #region Init
        internal void Init()
        {
            try
            {
                this.StartLogging();
                this.PreExecute();
            }
            catch (Exception ex)
            {
                try
                {
                    this.PostExecute();			//Run Test Finalizer
                }
                finally
                {
                    throw ex;
                }
            }
        }
        #endregion

        #region Run
        internal void Run()
        {
            try
            {
                this.Execute();  				//Run Test Logic
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                try
                {
                    this.PostExecute();			//Run Test Finalizer
                }
                catch (Exception) { }
            }
        }
        #endregion

        #region StartLogging
        private void StartLogging()
        {
            string TestMainPath = Path.GetDirectoryName(GetClientInternal().EA.LogFile);
            TestMainPath = TestMainPath.Substring(0, TestMainPath.LastIndexOf("\\"));

            lock (LoggingLocker)
            {
                if (!startTracer)
                {
                    Tracer.StartSession(GetClientInternal().EA.LogFilePath, "ExecutionLog");
                    startTracer = true;
                }
            }
            _xmlLog = new ResultXML(TestMainPath, "IEX_Summary.xml");
            _xmlLog.LogFileName = _testName;
            _xmlLog.RegisterTestStarted();
        }
        #endregion

        #region PreExecute
        public virtual void PreExecute()
        {
            IEXGateway._IEXResult res;

            try
            {
                if (GatewayIsClient())
                {
                    res = MountSingle();
                }
                else
                {
                    res = MountGroup();
                }
            }
            catch (Exception ex)
            {
                throw new MountException(ex.Message);
            }

            if (res != null) //if equals to null => debug mode
            {
                if (!res.CommandSucceeded)
                {
                    this.FailTest("Mount Failed " + res.FailureReason);
                    throw new MountException("Mount Failed " + res.FailureReason);
                }
            }
        }
        #endregion

        #region Execute
        private void Execute()
        {
            CreateStructure();

            foreach (_Step s in _steps)
            {
                try
                {
                    if (CancellationToken.IsCancellationRequested)
                    {
                        AbortTest(CancelationReason);
                        throw new CancelationException(CancelationReason);
                    }

                    if (s.Status == StepStatus.SKIPPED) continue;

                    s.Start();
                    s.Execute();
                    s.Pass();
                }
                catch (FailureException ex)
                {
                    s.Fail();
                    FailTest(ex.Message);
                    throw ex;
                }
                catch (Exception ex)
                {
                    s.Abort();
                    AbortTest(ex.Message);
                    throw ex;
                }
            }
            PassTest();
        }
        #endregion

        #region PostExecute
        public virtual void PostExecute()
        {
        }
        #endregion

        #region CreateStructure
        public virtual void CreateStructure()
        {
        }
        #endregion

        #region AddStep
        public void AddStep(_Step testStep, string stepDescription)
        {
            if ((testStep != null))
            {
                testStep.Init(stepDescription, _steps.Count + 1, _xmlLog, GetClientsList());
                _steps.Add(testStep);
            }
        }
        #endregion

        #region Mount
        public _IEXResult MountSingle()
        {
            PlatformContext gatewayContext = GetGatewayIsClientContext();
            _Platform GW = gatewayContext.Platform;
            _IEXResult res = null;

            if (DebugMode)
            {
                string temp;
                GW.IEX.IR.SendIR("MENU", out temp, 10000);
                return null;
            }

            lock (GatewayLocker)
            {
                if (!gatewayContext.isMount && GW != null)
                {
                    res = GW.EA.MountGw(_mountFlag);
                    if (!res.CommandSucceeded)
                    {
                        GW.IEX.LogComment("Failed to Mount Gateway: " + res.FailureReason, isBold: true, size: "12", colour: "Red");
                        return res;
                    }
                }
                gatewayContext.isMount = true;
            }
            return res;
        }

        public _IEXResult MountGroup()
        {
            PlatformContext gatewayContext = GetGatewayContext();
            _Platform GW = gatewayContext.Platform;
            List<_Platform> Clients = GetClientsList();

            _IEXResult res = null;

            if (DebugMode)
            {
                foreach (_Platform client in Clients)
                {
                    string temp;
                    client.IEX.IR.SendIR("MENU", out temp, 10000);
                }
                return null;
            }

            int Retries = 0;
            bool HasFailure = false;

            do
            {
                HasFailure = false;
                lock (GatewayLocker)
                {
                    if (!gatewayContext.isMount && GW != null)
                    {
                        res = GW.EA.MountGw(_mountFlag);
                        if (!res.CommandSucceeded)
                        {
                            GW.IEX.LogComment("Failed to Mount Gateway: " + res.FailureReason, isBold: true, size: "12", colour: "Red");
                            return res;
                        }
                    }
                    gatewayContext.isMount = true;
                }

                try
                {
                    Parallel.ForEach(Clients, CL =>
                    {
                        res = CL.EA.MountClient(_mountFlag, 1);
                        if (!res.CommandSucceeded && HasFailure == false)
                        {
                            CL.IEX.LogComment("Failed to Mount Client: " + res.FailureReason, isBold: true, size: "12", colour: "Red");
                            HasFailure = true;
                        }
                    });

                }
                catch (Exception)
                {
                    HasFailure = true;
                }

                Retries++;

            } while (HasFailure && Retries < 3);

            return res;
        }
        #endregion

        #region GetGateway
        /// <summary>
        /// finds the test Gateway by its test identifier
        /// </summary>
        /// <returns></returns>
        protected _Platform GetGateway()
        {
            foreach (PlatformContext p in TestPlatforms)
            {
                foreach (TestData t in p.Tests)
                {
                    if (t.Identifier == TestIdentifier && t.Type == PlatformType.GW)
                    {
                        t.InUse = true;
                        return p.Platform;
                    }
                }
            }
            return null;
        }
        #endregion

        #region GetClient
        /// <summary>
        /// Get the next client platform and set the flag isInUse to true. This method called from within specific test
        /// if you want the client without setting the flag call getClientInternal
        /// </summary>
        /// <returns></returns>
        protected _Platform GetClient()
        {
            foreach (PlatformContext p in TestPlatforms)
            {
                foreach (TestData t in p.Tests)
                {
                    if (t.Type == PlatformType.CL && t.InUse == false && t.Identifier == TestIdentifier)
                    {
                        t.InUse = true;
                        return p.Platform;
                    }
                }
            }
            return null;
        }
        #endregion

        #region LogComments
        public void LogComment(_Platform platform, string text, bool isBold = false, string size = "", string colour = "Black")
        {
            platform.IEX.LogComment(text, isBold: isBold, size: size, colour: colour);
            Tracer.Write(Tracer.TraceLevel.INFO, text, "TEST");
        }

        public void LogCommentInfo(_Platform platform, string text)
        {
            platform.IEX.LogComment(text, isBold: false, size: "10", colour: "Blue");
            Tracer.Write(Tracer.TraceLevel.INFO, text, "TEST");
        }

        public void LogCommentImportant(_Platform platform, string text)
        {
            platform.IEX.LogComment(text, isBold: true, size: "10", colour: "Purple");
            Tracer.Write(Tracer.TraceLevel.MIL, text, "TEST");
        }

        public void LogCommentWarning(_Platform platform, string text)
        {
            platform.IEX.LogComment(text, isBold: true, size: "10", colour: "Orange");
            Tracer.Write(Tracer.TraceLevel.WARN, text, "TEST");
        }

        public void LogCommentFailure(_Platform platform, string text)
        {
            platform.IEX.LogComment(text, isBold: true, size: "10", colour: "Red");
            Tracer.Write(Tracer.TraceLevel.ERROR, text, "TEST");
        }
        #endregion

        #region Test Status
        public void PassTest()
        {
            _testStatus = TestStatus.PASSED;

            Tracer.Write(Tracer.TraceLevel.MIL, "TEST PASSED!!!", "TEST");
            _xmlLog.RegisterTestPassed();
            _xmlLog.RegisterTestExiting();

            foreach (var testPlatform in GetClientsList())
            {
                testPlatform.IEX.LogHeader("TEST PASSED!", isBold: true, size: byte.Parse("14"), colour: "Green");
            }
        }

        public void FailTest(string reason)
        {
            _testStatus = TestStatus.FAILED;

            Tracer.Write(Tracer.TraceLevel.ERROR, "TEST FAILED: " + reason, "TEST");
            _xmlLog.RegisterTestFailed(reason);
            _xmlLog.RegisterTestExiting();

            foreach (var testPlatform in GetClientsList())
            {
                testPlatform.IEX.LogComment(reason, isBold: true, size: "12", colour: "Red");
                testPlatform.IEX.LogComment("TEST FAILED!", isBold: true, size: "14", colour: "Red");
            }
        }

        public void AbortTest(string reason)
        {
            _testStatus = TestStatus.ABORTED;

            System.Diagnostics.StackTrace sTrace = new System.Diagnostics.StackTrace();
            Tracer.Write(Tracer.TraceLevel.FATAL, "TEST ABORTED: " + reason, "TEST");
            Tracer.Write(Tracer.TraceLevel.FATAL, "STACK TRACE: " + sTrace.ToString(), "TEST");
            _xmlLog.RegisterTestAborted(reason);
            _xmlLog.RegisterTestExiting();

            foreach (var testPlatform in GetClientsList())
            {
                testPlatform.IEX.LogComment(reason, isBold: true, size: "12", colour: "Red");
                testPlatform.IEX.LogComment("TEST ABORTED!", isBold: true, size: "14", colour: "Red");
            }

        }
        #endregion

        #region Private Gateway Client Requests

        #region GetGatewayContext
        private PlatformContext GetGatewayContext()
        {
            return (from p in TestPlatforms
                    from t in p.Tests
                    where t.Type == PlatformType.GW && t.InUse == false && t.Identifier == TestIdentifier
                    select p).FirstOrDefault();
        }
        #endregion

        #region GetGatewayIsClientContext
        private PlatformContext GetGatewayIsClientContext()
        {
            return (from p in TestPlatforms
                    from t in p.Tests
                    where t.GatewayIsClient && t.Identifier == TestIdentifier
                    select p).FirstOrDefault();
        }
        #endregion

        #region GatewayIsClient
        private bool GatewayIsClient()
        {
            return TestPlatforms.SelectMany(pContext => pContext.Tests)
                .Any(test => test.GatewayIsClient && test.Identifier == TestIdentifier);
        }
        #endregion

        #region GetClientsList
        // return a list of clients associated with the test
        private List<_Platform> GetClientsList()
        {
            return (from p in TestPlatforms
                    from t in p.Tests
                    where t.Type == PlatformType.CL && t.Identifier == TestIdentifier
                    select p.Platform).ToList();
        }
        #endregion

        #region GetClientInternal
        private _Platform GetClientInternal()
        {
            return (from p in TestPlatforms
                    from t in p.Tests
                    where t.Type == PlatformType.CL && t.Identifier == TestIdentifier
                    select p.Platform).FirstOrDefault();
        }
        #endregion

        #endregion
    }
}