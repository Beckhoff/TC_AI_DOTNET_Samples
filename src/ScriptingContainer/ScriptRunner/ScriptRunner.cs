using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ScriptingTest;
using System.Diagnostics;
using System.ComponentModel;
using System.Threading;

using System.Management.Automation;

namespace ScriptingTest.ScriptRunner
{
    [Cmdlet(VerbsCommon.Get, "TcScript")]
    public class GetScriptCmdlet : PSCmdlet
    {
        /// <summary>
        /// List of scripts
        /// </summary>
        IList<Script> _scripts = ScriptLoader.Scripts;
        
        /// <summary>
        /// DTE Info
        /// </summary>
        //DTEInfo _dteInfo = null;

        /// <summary>
        /// Processes the Cmdlet
        /// </summary>
        protected override void ProcessRecord()
        {
            _scripts = ScriptLoader.Scripts;
            WriteObject(_scripts);
        }
    }

    /// <summary>
    /// Powershell Cmdlet for Running <see cref="Script">Scripts</see> written for the ScriptContainer application.
    /// </summary>
    [Cmdlet(VerbsLifecycle.Start, "TcScript", DefaultParameterSetName = "SingleRun")]
    public class ScriptRunnerCmdlet : PSCmdlet
    {
        private bool _userControl;
        /// <summary>
        /// Gets or sets the user control Property on Visual Studio
        /// </summary>
        /// <value>
        /// The user mode.
        /// </value>
        [Parameter(Mandatory = false)]
        [Parameter(ParameterSetName = "SingleRun")]
        public SwitchParameter UserControl
        {
            get { return _userControl; }
            set { _userControl = value; }
        }

        private bool _visible;
        /// <summary>
        /// Gets or sets the visible property on Visual Studio
        /// </summary>
        /// <value>
        /// The visible.
        /// </value>
        [Parameter(Mandatory = false)]
        public SwitchParameter Visible
        {
            get { return _visible; }
            set { _visible = value; }
        }

        private bool _richOutput;
        /// <summary>
        /// Gets or sets the rich output on the <see cref="ScriptRunnerCmdlet"/>.
        /// </summary>
        /// <value>
        /// The rich output.
        /// </value>
        [Parameter(Mandatory = false)]
        public SwitchParameter RichOutput
        {
            get { return _richOutput; }
            set { _richOutput = value; }
        }

        private bool _suppressUI;
        /// <summary>
        /// Gets or sets the SuppressUI Property on Visual Studio
        /// </summary>
        /// <value>
        /// The suppress UI.
        /// </value>
        [Parameter(Mandatory = false)]
        [Parameter(ParameterSetName = "SingleRun")]
        public SwitchParameter SuppressUI
        {
            get { return _suppressUI; }
            set { _suppressUI = value; }
        }

        private int _repeats = 1;
        /// <summary>
        /// Gets or sets the script repeats that should be processed.
        /// </summary>
        /// <value>
        /// The repeats.
        /// </value>
        [Parameter(Mandatory = true)]
        [Parameter(ParameterSetName="RepeatedRun")]
        [ValidateRange(1,1000)]
        public int Repeats
        {
            get { return _repeats; }
            set { _repeats = value; }
        }

        private string[] _scriptNames;
        /// <summary>
        /// Gets or sets ScriptNames that should be Processed.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        [Parameter(Mandatory = true,Position=0)]
        public string[] Name
        {
            get { return _scriptNames; }
            set { _scriptNames = value; }
        }

        private string _dte;
        /// <summary>
        /// Gets or sets the DTE Identifier (COM AppId) to be used.
        /// </summary>
        /// <value>
        /// The DTE.
        /// </value>
        [Parameter(Mandatory = false)]
        public string DTE
        {
            get { return _dte; }
            set { _dte = value; }
        }

        /// <summary>
        /// The scripts to be processed.
        /// </summary>
        IList<Script> _scripts = ScriptLoader.Scripts;

        /// <summary>
        /// DTE Info
        /// </summary>
        DTEInfo _dteInfo = null;

        /// <summary>
        /// Handler before Cmdlet processing
        /// </summary>
        protected override void BeginProcessing()
        {
            int currentIndex = -1;
            int defaultIndex = -1;

            IDictionary<string, DTEInfo> progIdDict = ConfigurationFactory.GetVisualStudioProgIds(out currentIndex, out defaultIndex);
            List<DTEInfo> progIds = null;
            progIds = new List<DTEInfo>(progIdDict.Values);
            
            _dteInfo = progIds[currentIndex];
            _scripts = ScriptLoader.Scripts;

            WriteVerbose(string.Format("VS APPID: {0}", _dteInfo.ProgId));
            WriteVerbose(string.Format("FoundScripts: {0}", _scripts));
        }


        /// <summary>
        /// Processes the Cmdlet
        /// </summary>
        protected override void ProcessRecord()
        {
            List<Script> todo = new List<Script>();

            foreach (string name in _scriptNames)
            {
                WildcardPattern pattern = new WildcardPattern(name);

                foreach (Script script in _scripts)
                {
                    if (pattern.IsMatch(script.ScriptName))
                    {
                        if (!todo.Contains(script))
                        {
                            todo.Add(script);
                        }
                    }
                }
            }

            ScriptRunner executer = null;
            switch(ParameterSetName)
            {
                case "SingleRun":
                    executer = new ScriptRunner(new ScriptExecuteContext(this,_dteInfo,_visible, _userControl, _suppressUI));
                    break;

                case "RepeatedRun":
                    executer = new ScriptRunner(new ScriptExecuteContext(this,_dteInfo,_visible,_repeats));
                    break;
            }

            executer.Execute(todo);
        }
    }

    /// <summary>
    /// Information class describing the Result of the Script Excecution.
    /// </summary>
    public class ScriptResultInfo
    {
        /// <summary>
        /// Script Result
        /// </summary>
        public readonly ScriptResult Result;
        /// <summary>
        /// Script Name
        /// </summary>
        public readonly string ScriptName;

        /// <summary>
        /// Script Repeat Index
        /// </summary>
        public readonly int Repeat;

        /// <summary>
        /// Number of Repetitions that should be executed.
        /// </summary>
        public readonly int RepeatCount;

        /// <summary>
        /// Script execution duration.
        /// </summary>
        public readonly TimeSpan Duration;

        /// <summary>
        /// Retained Exception if Script failed.
        /// </summary>
        public readonly Exception Exception;

        /// <summary>
        /// Initializes a new instance of the <see cref="ScriptResultInfo"/> class.
        /// </summary>
        /// <param name="result">Script result.</param>
        /// <param name="name">Script name.</param>
        /// <param name="repeat">Current script repeat index.</param>
        /// <param name="repeats">Repeat count .</param>
        /// <param name="duration">Script execution duration.</param>
        /// <param name="ex">Exception on error</param>
        internal ScriptResultInfo(ScriptResult result, string name, int repeat, int repeats, TimeSpan duration, Exception ex)
        {
            this.Result = result;
            this.ScriptName = name;
            this.Repeat = repeat;
            this.RepeatCount = repeats;
            this.Duration = duration;
            this.Exception = ex;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ScriptResultInfo"/> class.
        /// </summary>
        /// <param name="script">Executed script</param>
        /// <param name="repeat">The repeat index.</param>
        /// <param name="repeats">Repetitions to be executed (all)</param>
        internal ScriptResultInfo(Script script, int repeat, int repeats)
            : this(script.Result,script.ScriptName,repeat,repeats,script.Duration,script.Exception)
        {
        }
    }


    public class ScriptRunner
    {
        /// <summary>
        /// Wait handle to report completion
        /// </summary>
        private ManualResetEvent _completedEvent = new ManualResetEvent(false);

        /// <summary>
        /// Wait handle to report availability of a ProgressRecord item in pr.
        /// </summary>
        private ManualResetEvent _progressEvent = new ManualResetEvent(false);

        private ManualResetEvent _statusEvent = new ManualResetEvent(false);


        private ManualResetEvent _progressStatusEvent = new ManualResetEvent(false);

        /// <summary>
        /// Array of the wait handles above (set in ProcessRecord) to perform WaitAny.
        /// </summary>
        private WaitHandle[] _waitHandles;

        /// <summary>
        /// Synchronization object for pr.
        /// </summary>
        private object pr_sync = new object();

        public ScriptRunner(ScriptExecuteContext context)
        {
            _context = context;
            _cmdlet = (ScriptRunnerCmdlet)this._context.Cmdlet;
        }

        ScriptExecuteContext _context = null;

        /// <summary>
        /// Background script worker
        /// </summary>
        IWorker _worker = null;

        ProgressRecord _progress = null;
        ProgressRecord _subProgress = null;

        ScriptRunnerCmdlet _cmdlet = null;

        int GetProgress(int scriptIndex, int scriptsCount, int repeatIndex, int repeats, int subProgress)
        {

            double step = 100 / (scriptsCount * repeats);
            double baseProgress = step * scriptIndex * repeatIndex;
            double ret = baseProgress + ((step * subProgress) / 100);

            return (int)ret;
        }

        public int Execute(IList<Script> scripts)
        {
            bool multipleRuns = this._context.Repeats > 1;


            // int progressId = 0;
            
            //
            // Construct wait handles array for WaitHandle.WaitAny calls.
            //
            _waitHandles = new WaitHandle[] { _completedEvent, _statusEvent, _progressEvent, _progressStatusEvent };
            
            //ScriptRunnerCmdlet cmdlet = (ScriptRunnerCmdlet)this._context.Cmdlet;
            int ret = 0;

            //progress.CurrentOperation = "Starting Scripts...";
            //progress.PercentComplete = 0;

            _progress = new ProgressRecord(1, "Overall", "Initializing");


            for (int scriptIndex = 0; scriptIndex < scripts.Count; scriptIndex++)
            {
                Script runningScript = scripts[scriptIndex];
                _progress.StatusDescription = string.Format("Running script '{0}' ({1} of {2})", runningScript, scriptIndex + 1, scripts.Count);

                for (int scriptRepeatIndex = 0; scriptRepeatIndex < _context.Repeats; scriptRepeatIndex++)
                {
                    try
                    {
                        string message = "Initializing ...";
                        //string activity;
                        string subActivity;

                        if (multipleRuns)
                        {
                            _subProgress = new ProgressRecord(2, string.Format("Processing script '{0}'  ({1} of {2})", runningScript.ScriptName, scriptRepeatIndex + 1, _context.Repeats), "Initializing ...");
                            subActivity = string.Format("'{0}' iteration {1} of {2} ...", runningScript.ScriptName, scriptRepeatIndex + 1, _context.Repeats);
                        }
                        else
                        {
                            _subProgress = new ProgressRecord(2, string.Format("Processing script '{0}'", runningScript.ScriptName), "Initializing ...");
                            subActivity = string.Format("Starting Script '{0}'",runningScript.ScriptName);
                        }

                        this.WriteStatus(subActivity);
                        WriteProgress(0, subActivity, null, scriptIndex, scripts.Count, scriptRepeatIndex);

                        //_subProgress.PercentComplete = (scriptIndex * scriptRepeatIndex * 100 / (scripts.Count * _context.Repeats));
                        //_subProgress.PercentComplete = 0;
                        //_progress.PercentComplete = GetProgress(scriptIndex, scripts.Count, 0, _context.Repeats, 0);

                        //string activity = string.Format("\r\nProcessing Script '{0}' ({1} of {2})", runningScript.ScriptName, scriptIndex + 1, scripts.Count);
                        
                        //_subProgress = new ProgressRecord(2, activity, "Starting Script ...");

                        //progress.StatusDescription = "StartingScript";
                        //cmdlet.WriteProgress(_subProgress);
                        //cmdlet.WriteVerbose(activity);

                        VsFactory vsFactory = new VsFactory();

                        ConfigurationFactory factory;
                        if (runningScript is ScriptEarlyBound)
                            factory = new EarlyBoundFactory(vsFactory);
                        else if (runningScript is ScriptLateBound)
                            factory = new LateBoundFactory(vsFactory);
                        else
                            throw new ApplicationException("Configuration Factory not found!");

                        runningScript.StatusChanged += new EventHandler<ScriptStatusChangedEventArgs>(script_StatusChanged);

                        Dictionary<string, dynamic> parameterSet = new Dictionary<string, dynamic>();

                        ScriptContext context = new ScriptContext(factory, null, parameterSet);

                        _worker = new ScriptBackgroundWorker(/*this._factory,*/ runningScript, context);
                        _worker.ProgressChanged += new ProgressChangedEventHandler(_worker_ProgressChanged);
                        _worker.ProgressStatusChanged += new EventHandler<ProgressStatusChangedArgs>(_worker_ProgressStatusChanged);
                        _worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(_worker_RunWorkerCompleted);
                        factory.AppID = this._context.DTEInfo.ProgId;
                        factory.IsIdeVisible = _context.ProgVisible;
                        factory.IsIdeUserControl = _context.UserControl;
                        factory.SuppressUI = _context.SupressUI;

                        _executing = true;

                        _worker.BeginScriptExecution();

                        bool leave = false;

                        while (/*this.IsExecuting*/ !leave)
                        {
                            //Thread.Sleep(500);
                            int handle = WaitHandle.WaitAny(_waitHandles);

                            lock (pr_sync)
                            {
                                switch (handle)
                                {
                                    case 0:
                                        if (runningScript.Result == ScriptResult.Ok)
                                        {
                                            _cmdlet.WriteVerbose(string.Format("Script '{0}' succeeded!", runningScript.ScriptName));

                                        }
                                        else if (runningScript.Result == ScriptResult.Fail)
                                        {
                                            ErrorRecord err = new ErrorRecord(runningScript.Exception, "MyScript", ErrorCategory.OperationStopped, runningScript);
                                            _cmdlet.WriteError(err);
                                            _cmdlet.WriteVerbose(string.Format("Script '{0}' failed!", runningScript.ScriptName));

                                        }
                                        else
                                        {
                                            Debug.Fail("");
                                        }
                                        _currentCompletedArgs = null;
                                        _completedEvent.Reset();
                                        Debug.Assert(!this.IsExecuting);
                                        leave = true;
                                        break;
                                    case 1:
                                        _cmdlet.WriteVerbose(string.Format("Changing script status to {0}!", _currentStatusArgs.NewState));
                                        _currentStatusArgs = null;
                                        _statusEvent.Reset();
                                        break;
                                    case 2:
                                        if (_currentProgressArgs != null)
                                        {
                                            _subProgress.PercentComplete = _currentProgressArgs.ProgressPercentage;
                                            _progress.PercentComplete = GetProgress(scriptIndex, scripts.Count, 0, _context.Repeats, _currentProgressArgs.ProgressPercentage);

                                            this._context.Cmdlet.WriteProgress(this._subProgress);
                                            this._context.Cmdlet.WriteProgress(this._progress);
                                        }
                                        _currentProgressArgs = null;
                                        _progressEvent.Reset();
                                        break;

                                    case 3:
                                        //cmdlet.WriteVerbose(_currentProgressStatusArgs.Status);
                                        //cmdlet.WriteCommandDetail(_currentProgressStatusArgs.Status);
                                        //this._context.Cmdlet.WriteCommandDetail
                                        this.WriteStatus(_currentProgressStatusArgs.Status);

                                        _currentProgressStatusArgs = null;
                                        _progressStatusEvent.Reset();
                                        break;
                                }
                            }
                        }
                        if (runningScript.Result == ScriptResult.Ok)
                        {
                            message = string.Format("Processing Script '{0}' ({1} of {2}, Repeat {3}) Succeeded!", runningScript.ScriptName, scriptIndex + 1, scripts.Count, scriptRepeatIndex + 1);
                            this.WriteStatus(message);

                            WriteProgress(100, message, null, scriptIndex, scripts.Count, scriptRepeatIndex);
                            
                            ret = 0;
                        }
                        else
                        {
                            message = string.Format("Processing Script '{0}' ({1} of {2},  Repeat {3}) FAILED!", runningScript.ScriptName, scriptIndex + 1, scripts.Count,scriptRepeatIndex + 1);
                            WriteStatus(message);
                            _subProgress.StatusDescription = message;
                            _subProgress.PercentComplete = 100;
                            _progress.PercentComplete = GetProgress(scriptIndex, scripts.Count, 0, _context.Repeats, 100);

                            ret = 1;
                        }
                        _cmdlet.WriteObject(new ScriptResultInfo(runningScript,scriptRepeatIndex + 1, _context.Repeats));

                        _worker.ProgressChanged -= new ProgressChangedEventHandler(_worker_ProgressChanged);
                        _worker.ProgressStatusChanged -= new EventHandler<ProgressStatusChangedArgs>(_worker_ProgressStatusChanged);
                        _worker.RunWorkerCompleted -= new RunWorkerCompletedEventHandler(_worker_RunWorkerCompleted);
                    }
                    catch (Exception ex)
                    {
                        //Console.WriteLine("Calling scripts failed!");
                        _context.Cmdlet.WriteError(new ErrorRecord(ex,"MyId",ErrorCategory.OperationStopped,this));
                        ret = 1;
                    }
                }
            }
            _progress.PercentComplete = 100;
            
            _subProgress = null;
            return ret;
        }

        private void WriteProgress(int subPercentComplete, string subProgressMessage, string progressMessage,int scriptIndex, int scriptCount, int repeatIndex)
        {
            if (progressMessage != null)
            {
                _progress.StatusDescription = progressMessage;
            }
            if (subProgressMessage != null)
            {
                _subProgress.StatusDescription = subProgressMessage;
            }
            _subProgress.PercentComplete = subPercentComplete;
            _progress.PercentComplete = GetProgress(scriptIndex, scriptCount,repeatIndex, _context.Repeats, _subProgress.PercentComplete);

            _cmdlet.WriteProgress(_subProgress);
            _cmdlet.WriteProgress(_progress);
        }

        private void WriteStatus(string message)
        {
            if (_cmdlet.RichOutput)
            {
                if (_cmdlet.Host != null && _cmdlet.Host.UI != null)
                    _cmdlet.Host.UI.WriteLine(message);
            }
            else
                _cmdlet.WriteVerbose(message);

            _subProgress.StatusDescription = message;
            _cmdlet.WriteProgress(_subProgress);
        }

        ProgressStatusChangedArgs _currentProgressStatusArgs = null;

        void _worker_ProgressStatusChanged(object sender, ProgressStatusChangedArgs e)
        {
            lock (this.pr_sync)
            {
                _currentProgressStatusArgs = e;
                _progressStatusEvent.Set();
            }
        }


        ScriptStatusChangedEventArgs _currentStatusArgs = null;

        /// <summary>
        /// Handles the StatusChanged event of the script control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="ScriptingTest.ScriptStatusChangedEventArgs"/> instance containing the event data.</param>
        void script_StatusChanged(object sender, ScriptStatusChangedEventArgs e)
        {
            lock (this.pr_sync)
            {
                _currentStatusArgs = e;
                _statusEvent.Set();
            }
        }

        RunWorkerCompletedEventArgs _currentCompletedArgs = null;

        /// <summary>
        /// Handles the RunWorkerCompleted event of the _worker control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.ComponentModel.RunWorkerCompletedEventArgs"/> instance containing the event data.</param>
        void _worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            lock (pr_sync)
            {
                _executing = false;
                _currentCompletedArgs = e;
                _completedEvent.Set();

            }
        }

        /// <summary>
        /// Indicates that the Script engine is running
        /// </summary>
        bool _executing = false;

        /// <summary>
        /// Gets a value indicating whether a script is executing.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if a script is executing; otherwise, <c>false</c>.
        /// </value>
        public bool IsExecuting
        {
            get
            {
                lock (this)
                {
                    return _executing;
                }
            }
        }

        ProgressChangedEventArgs _currentProgressArgs = null;

        /// <summary>
        /// Handles the ProgressChanged event of the _worker control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.ComponentModel.ProgressChangedEventArgs"/> instance containing the event data.</param>
        void _worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            lock (pr_sync)
            {
                Debug.Assert(e != null);
                _currentProgressArgs = e;
                _progressEvent.Set();

            }
        }

        ///// <summary>
        ///// Handles the Click event of the btnCancel control.
        ///// </summary>
        ///// <param name="sender">The source of the event.</param>
        ///// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        //private void btnCancel_Click(object sender, RoutedEventArgs e)
        //{
        //    if (this._worker != null)
        //    {
        //        this._worker.CancelRequest();
        //        this.enableDisableControls();
        //        //this._worker.CancelAndWait(TimeSpan.FromSeconds(0.1));
        //    }
        //}
    }

    public class ScriptExecuteContext
    {

        public readonly PSCmdlet Cmdlet = null;
        public readonly bool ProgVisible = true;
        public readonly bool UserControl = false;
        public readonly bool SupressUI = true;
        public readonly DTEInfo DTEInfo;
        public readonly int Repeats = 1;

        public ScriptExecuteContext(PSCmdlet cmdlet, DTEInfo dteInfo, bool visible, bool userControl, bool suppressUI)
        {
            this.Cmdlet = cmdlet;
            this.ProgVisible = visible;
            this.UserControl = userControl;
            this.SupressUI = suppressUI;
            this.DTEInfo = dteInfo;
            this.Repeats = 1;
        }

        public ScriptExecuteContext(PSCmdlet cmdlet, DTEInfo dteInfo, bool visible, int repeats)
        {
            this.Cmdlet = cmdlet;
            this.ProgVisible = visible;
            this.UserControl = false;
            this.SupressUI = true;
            this.DTEInfo = dteInfo;
            this.Repeats = repeats;
        }

        //public static ScriptExecuteContext Default
        //{
        //    get { return new ScriptExecuteContext(true,false,true); }
        //}

    }
}
