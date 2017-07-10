using System;
using System.Collections.Generic;
using IEX.Utilities;
using IEX.IEXecuter.ExecutionEngine;

namespace IEX.Tests.Engine
{
    public abstract class _Step
    {
        #region Variables

        private string _description = null;
        private int _stepNumber;
        private ResultXML _xmlLog;
        private StepStatus _status = StepStatus.PENDING;
        private List<_Platform> _stepPlatforms;

        protected IEXGateway._IEXResult res;

        public abstract void Execute();

        #endregion

        #region Events
        public event DoStartEventHandler DoStart;
        public delegate void DoStartEventHandler(object sender, EventArgs e);
        public event DoPassEventHandler DoPass;
        public delegate void DoPassEventHandler(object sender, EventArgs e);
        public event DoFailEventHandler DoFail;
        public delegate void DoFailEventHandler(object sender, EventArgs e);
        public event DoAbortEventHandler DoAbort;
        public delegate void DoAbortEventHandler(object sender, EventArgs e);
        public event DoSkipEventHandler DoSkip;
        public delegate void DoSkipEventHandler(object sender, EventArgs e);
        public event DoPauseEventHandler DoPause;
        public delegate void DoPauseEventHandler(object sender, EventArgs e);
        public event DoPlayEventHandler DoPlay;
        public delegate void DoPlayEventHandler(object sender, EventArgs e);
        #endregion

        #region Properties
        public int StepNumber
        {
            get { return _stepNumber; }
            set { _stepNumber = value; }
        }

        public string Description
        {
            get
            {
                if (string.IsNullOrEmpty(_description))
                {
                    _description = string.Empty;
                }
                return _description;
            }
            set
            {
                _description = value;
                if (string.IsNullOrEmpty(_description))
                {
                    _description = string.Empty;
                }
            }
        }

        public StepStatus Status
        {
            get { return _status; }
            set { _status = value; }
        }
        #endregion

        #region Init
        internal void Init(string stepDescription, int stepNumber, ResultXML xmlLog, List<_Platform> platforms)
        {
            Description = stepDescription;
            _stepNumber = stepNumber;
            _xmlLog = xmlLog;
            _stepPlatforms = platforms;
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

        #region Step Status
        public void StartStep()
        {
            Tracer.Write(Tracer.TraceLevel.MIL, "Start Step: " + _description, "TEST");
            _xmlLog.StartStep(_stepNumber, _description);
            Console.WriteLine("Step " + _stepNumber + " Started");

            foreach (var stepPlatform in _stepPlatforms)
            {
                stepPlatform.IEX.StartStep(_description, isBold: true, size: byte.Parse("16"), colour: "Purple");
                stepPlatform.IEX.Debug.InsertCommentIntoLog("========" + "Start Step: " + _description + "========", IEXGateway.DebugDevice.Udp);
            }
        }

        public void PassStep()
        {
            Tracer.Write(Tracer.TraceLevel.MIL, "Step: " + _description + " PASSED", "TEST");
            _xmlLog.PassStep(_stepNumber - 1, "");
            Console.WriteLine("Step " + _stepNumber + " Passed");

            foreach (var stepPlatform in _stepPlatforms)
            {
                stepPlatform.IEX.LogComment(_description + " Passed", isBold: true, size: "12", colour: "Green");
                stepPlatform.IEX.Debug.InsertCommentIntoLog("========" + "Passed Step: " + _description + "========", IEXGateway.DebugDevice.Udp);
            }
        }

        public void FailStep(_Platform platform, IEXGateway._IEXResult res, string reason = "", bool exitTest = true)
        {
            string msg = "[" + res.FailureCode + "] " + reason + " " + res.FailureReason;
            this.FailStep(platform, msg, exitTest);
        }

        public void FailStep(_Platform platform, string reason, bool exitTest = true)
        {
            Tracer.Write(Tracer.TraceLevel.ERROR, platform.ID + " Step: " + _description + " FAILED", "TEST");
            Tracer.Write(Tracer.TraceLevel.ERROR, platform.ID + " Failure Reason: " + reason, "TEST");
            _xmlLog.FailStep(_stepNumber - 1, reason);
            Console.WriteLine(platform.ID + " Step " + _stepNumber + " Failed: " + reason);

            platform.IEX.GetSnapshot("FAILED " + reason);
            platform.IEX.LogComment("Step Failed. Reason:" + reason, isBold: true, size: "12", colour: "Red");
            platform.IEX.Debug.InsertCommentIntoLog("========" + "Failed Step: " + _description + "========", IEXGateway.DebugDevice.Udp);

            if (exitTest)
            {
                foreach (var stepPlatform in _stepPlatforms)
                {
                    if (stepPlatform != platform)
                    {
                        stepPlatform.IEX.LogComment("Step Aborted Becuase " + platform.ID + " Failure", isBold: true, size: "12", colour: "Red");
                        stepPlatform.IEX.Debug.InsertCommentIntoLog("========" + "Step Aborted Becuase " + platform.ID + " Failure" + "========", IEXGateway.DebugDevice.Udp);
                    }
                }

                throw new FailureException(reason);
            }
        }

        public void AbortStep(string reason)
        {
            Tracer.Write(Tracer.TraceLevel.ERROR, "Step " + _stepNumber + ": " + _description + " ABORTED", "TEST");
            _xmlLog.AbortStep(_stepNumber - 1, reason);
            Console.WriteLine("Step " + _stepNumber + " Aborted: " + reason);

            foreach (var stepPlatform in _stepPlatforms)
            {
                stepPlatform.IEX.LogComment("Step Aborted. Reason:" + reason, isBold: true, size: "12", colour: "Red");
                stepPlatform.IEX.Debug.InsertCommentIntoLog("========" + "Step Aborted. Reason:" + reason + "========", IEXGateway.DebugDevice.Udp);
            }

            throw new AbortException(reason);
        }
        #endregion

        #region Status Handeling
        internal void Start()
        {
            this.Status = StepStatus.RUNNING;
            if (DoStart != null)
            {
                DoStart(this, new EventArgs());
            }
        }

        internal void Pass()
        {
            this.Status = StepStatus.PASSED;
            if (DoPass != null)
            {
                DoPass(this, new EventArgs());
            }
        }

        internal void Fail()
        {
            this.Status = StepStatus.FAILED;
            if (DoFail != null)
            {
                DoFail(this, new EventArgs());
            }
        }

        internal void Abort()
        {
            this.Status = StepStatus.ABORTED;
            if (DoAbort != null)
            {
                DoAbort(this, new EventArgs());
            }
        }

        internal void Skip()
        {
            this.Status = StepStatus.SKIPPED;
            if (DoSkip != null)
            {
                DoSkip(this, new EventArgs());
            }
        }

        internal void Pause()
        {
            this.Status = StepStatus.PAUSED;
            if (DoPause != null)
            {
                DoPause(this, new EventArgs());
            }
        }

        internal void Play()
        {
            this.Status = StepStatus.RESUMED;
            if (DoPlay != null)
            {
                DoPlay(this, new EventArgs());
            }
        }
        #endregion
    }
}
