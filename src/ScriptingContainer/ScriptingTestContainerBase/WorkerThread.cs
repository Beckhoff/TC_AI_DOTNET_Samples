using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Threading;
using System.Diagnostics;

namespace ScriptingTest
{
    //TODO:
    // The Script Context could maintain more than one Visual Studio. The Interface IContextProvider
    // is not prepared to this actually.

    /// <summary>
    /// Script context
    /// </summary>
    public interface IContext
    {
        /// <summary>
        /// Gets the DTE Object.
        /// </summary>
        /// <value>The DTE.</value>
        dynamic DTE { get; }
        /// <summary>
        /// Gets the solution object
        /// </summary>
        /// <value>The solution.</value>
        dynamic Solution { get; }

        /// <summary>
        /// Gets the Script Worker.
        /// </summary>
        /// <value>The worker.</value>
        IWorker Worker { get; }
        /// <summary>
        /// Gets the Configuration Factory
        /// </summary>
        /// <value>The factory.</value>
        IConfigurationFactory Factory { get; }

        /// <summary>
        /// Gets the Named parameters (Dictionary Name --> ParameterValue)
        /// </summary>
        /// <value>The _parameters.</value>
        IDictionary<string, dynamic> Parameters { get; }

        /// <summary>
        /// Gets the solution template.
        /// </summary>
        /// <value>The solution template.</value>
        string ProjectTemplate { get;  }
    }

    //TODO:
    // The Script Context could maintain more than one Visual Studio. The Interface IContextProvider
    // is not prepared to this actually.

    internal interface IContextProvider : IContext
    {
        void SetDTE(dynamic dte, dynamic solution);
        void SetWorker(IWorker worker);
    }


    /// <summary>
    /// Script context class
    /// </summary>
    public class ScriptContext : IContextProvider
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ScriptContext"/> class.
        /// </summary>
        /// <param name="factory">The factory.</param>
        /// <param name="projectTemplate">The project template.</param>
        /// <param name="parameters">The parameters.</param>
        public ScriptContext(IConfigurationFactory factory, string projectTemplate, /*IWorker worker,*/ IDictionary<string, dynamic> parameters)
        {
            if (factory == null) throw new ArgumentNullException("factory");

            _factory = factory;
            _dte = factory.DTE;
            _solution = factory.Solution;

            //_worker = worker;
            this.parameters = parameters;

            this.projectTemplate = projectTemplate;

            if (parameters == null)
                parameters = new Dictionary<string, dynamic>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ScriptContext"/> class.
        /// </summary>
        /// <param name="factory">The factory.</param>
        protected ScriptContext(IConfigurationFactory factory)
        {
            if (factory == null) throw new ArgumentNullException("factory");

            _factory = factory;
            _dte = factory.DTE;
            _solution = factory.Solution;

            //_worker = worker;
             this.parameters = new Dictionary<string,dynamic>();
        }

        #region IContext Members

        IWorker _worker;

        /// <summary>
        /// Gets the Script Worker.
        /// </summary>
        /// <value>The worker.</value>
        public IWorker Worker
        {
            get { return _worker; }
        }

        IConfigurationFactory _factory;
        /// <summary>
        /// Gets the Configuration Factory
        /// </summary>
        /// <value>The factory.</value>
        public IConfigurationFactory Factory
        {
            get { return _factory; }
        }

        dynamic _dte;
        /// <summary>
        /// Gets the DTE.
        /// </summary>
        /// <value>The DTE.</value>
        public dynamic DTE
        {
            get { return _dte; }
        }

        dynamic _solution;
        /// <summary>
        /// Gets the solution.
        /// </summary>
        /// <value>The solution.</value>
        public dynamic Solution
        {
            get { return _solution; }
        }

        /// <summary>
        /// Parameters dictionary
        /// </summary>
        protected IDictionary<string, dynamic> parameters;
        
        /// <summary>
        /// Gets the Named parameters (Dictionary Name --&gt; ParameterValue)
        /// </summary>
        /// <value>The _parameters.</value>
        public IDictionary<string, dynamic> Parameters
        {
            get { return parameters; }
        }


        /// <summary>
        /// Project template
        /// </summary>
        protected string projectTemplate = null;

        /// <summary>
        /// Gets the solution template.
        /// </summary>
        /// <value>The solution template.</value>
        public string ProjectTemplate
        {
            get { return projectTemplate; }
        }

        #endregion

        #region IContextProvider Members

        /// <summary>
        /// Sets the DTE.
        /// </summary>
        /// <param name="dte">The DTE.</param>
        /// <param name="solution">The solution.</param>
        public void SetDTE(dynamic dte, dynamic solution)
        {
            this._dte = dte;
            this._solution = solution;
        }

        /// <summary>
        /// Sets the worker.
        /// </summary>
        /// <param name="worker">The worker.</param>
        public void SetWorker(IWorker worker)
        {
            this._worker = worker;
        }

        #endregion
    }

    /// <summary>
    /// Worker interface wrapping the Worker thread used to execute the script.
    /// </summary>
    public interface IWorker
        : IProgressProvider
    {
        /// <summary>
        /// Gets a value indicating whether a cancellation is pending.
        /// </summary>
        /// <value><c>true</c> if [cancellation pending]; otherwise, <c>false</c>.</value>
        bool CancellationPending { get; }

        /// <summary>
        /// Occurs when the worker thread has been completed
        /// </summary>
        event RunWorkerCompletedEventHandler RunWorkerCompleted;

        /// <summary>
        /// Starts the Script execution asynchronously
        /// </summary>
        void BeginScriptExecution();

        /// <summary>
        /// Cancels the script asynchronously.
        /// </summary>
        bool CancelRequest();

        /// <summary>
        /// Requests the Execution Cancel and wait until the Execution is stopped.
        /// </summary>
        bool CancelAndWait(TimeSpan timeout);

        /// <summary>
        /// Occurs, when the Visual Studio configuration is fully initialized
        /// </summary>
        event EventHandler ConfigurationInitialized;
    }

    /// <summary>
    /// Worker class for asynchronous script execution (within STA)
    /// </summary>
    public class ScriptBackgroundWorker
        : IWorker
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ScriptBackgroundWorker"/> class.
        /// </summary>
        /// <param name="script">The script.</param>
        /// <param name="context">The context.</param>
        public ScriptBackgroundWorker(/*IConfigurationFactory generator,*/ Script script, /*Dictionary<string,dynamic> parametersL*/IContext context)
        {
            ((IContextProvider)context).SetWorker(this);
            this._factory = context.Factory;
            this._script = script;
            //this._parameters = parameters;
            this._scriptContext = context;
            this._script.SetResult(ScriptResult.None);
            //this.WorkerReportsProgress = true;
            //this.WorkerSupportsCancellation = true;
        }


        /// <summary>
        /// Configuration factory
        /// </summary>
        IConfigurationFactory _factory = null;

        /// <summary>
        /// Executed Script
        /// </summary>
        Script _script = null;

        ///// <summary>
        ///// The Script Invoke parameters
        ///// </summary>
        //IDictionary<string, dynamic> _parameters = null;

        IContext _scriptContext;

        bool _executing = false;

        private void OnDoWork()
        {
            try
            {
                this.Progress = 0;
                this.ProgressStatus = "Creating configuration ...";

                lock (this)
                {
                    this._executing = true;
                }

                _factory.Initialize(_scriptContext); // Initialization of Solution and projects
                OnConfigurationInitialized();

                _factory.ExecuteScript(_script /*,_parameters,this*/);   // Execution of Script
                this.ProgressStatus = "Closing configuration ...";
                _factory.Cleanup();  // Closing of configuration

                this.Progress = 100;
                this.ProgressStatus = "Finished";
            }
            //catch (ThreadAbortException tae)
            //{
            //    //this.ProgressStatus = string.Format("Error: {0}", tae.Message);
            //}
            //catch (Exception ex)
            //{
            //    //this.ProgressStatus = string.Format("Error: {0}", ex.Message);
            //}
            finally
            {
                lock (this)
                {
                    _executing = false;
                    _thread = null;
                    Debug.Assert(_script.Result != ScriptResult.None);
                }

                if (RunWorkerCompleted != null)
                    RunWorkerCompleted(this, new RunWorkerCompletedEventArgs(_script.Result, _script.Exception, this.CancellationPending));
            }
        }

        /// <summary>
        /// Occurs when the worker thread has been completed
        /// </summary>
        public event RunWorkerCompletedEventHandler RunWorkerCompleted;

        /// <summary>
        /// Starts the Script execution asynchronously
        /// </summary>
        public void BeginScriptExecution()
        {
            lock (this)
            {
                if (this._executing) throw new ApplicationException("Script Already executing!");


                DoWorkEventArgs args = new DoWorkEventArgs(null);

                ThreadStart threadStart = new ThreadStart(OnDoWork);
                _thread = new Thread(threadStart);
                _thread.SetApartmentState(ApartmentState.STA); // We need STA for the MessageFilter
                _thread.Start();
            }
        }

        private Thread _thread = null;

        /// <summary>
        /// Current status of the script execution
        /// </summary>
        string progressStatus = string.Empty;

        /// <summary>
        /// Gets or sets the status string.
        /// </summary>
        /// <value>The status.</value>
        public string ProgressStatus
        {
            set
            {
                //if (this.cancelationPending)
                //    throw new Exception("Script cancelled by user!");

                progressStatus = value;

                if (ProgressStatusChanged != null)
                    ProgressStatusChanged(this, new ProgressStatusChangedArgs(progressStatus));
            }
            get
            {
                return progressStatus;
            }
        }

        /// <summary>
        /// Occurs when the status has been changed.
        /// </summary>
        public event EventHandler<ProgressStatusChangedArgs> ProgressStatusChanged;

        /// <summary>
        /// Current Progress
        /// </summary>
        int _progress = 0;

        /// <summary>
        /// Sets the progress value (0-100%)
        /// </summary>
        /// <value>The progress.</value>
        public int Progress
        {
            set
            {
                //if (this.cancelationPending)
                //    throw new Exception("Script cancelled by user!");

                _progress = value;
                OnProgressChanged();
            }
        }

        /// <summary>
        /// Handler function fireing the <see cref="ProgressChanged"/> event.
        /// </summary>
        protected virtual void OnProgressChanged()
        {
            if (ProgressChanged != null)
            {
                ProgressChanged(this, new ProgressChangedEventArgs(_progress, null));
            }
        }

        /// <summary>
        /// Occurs when the progress has been changed.
        /// </summary>
        public event ProgressChangedEventHandler ProgressChanged;

        bool cancelationPending = false;
        /// <summary>
        /// Gets a value indicating whether a cancellation is pending.
        /// </summary>
        /// <value><c>true</c> if [cancellation pending]; otherwise, <c>false</c>.</value>
        public bool CancellationPending
        {
            get { return cancelationPending; }
        }

        /// <summary>
        /// Cancels the script asynchronously.
        /// </summary>
        public bool CancelRequest()
        {
            lock (this)
            {
                if (this._executing)
                {
                    this.cancelationPending = true;
                }
            }
            return this._executing;
        }

        /// <summary>
        /// Requests the Execution Cancel and wait until the Execution is stopped.
        /// </summary>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public bool CancelAndWait(TimeSpan timeout)
        {
            bool executing = false;

            lock (this)
            {
                executing = _executing;
            }
            if (!executing) return false;

            CancelRequest();
            DateTime stamp = DateTime.Now;

            while (executing)
            {
                Thread.Sleep(200);

                if (DateTime.Now - stamp > timeout)
                {
                    _thread.Abort();
                    Thread.Sleep(200);
                }
                else
                {
                    lock (this)
                    {
                        executing = _executing;
                    }
                }
            }
            return _executing;
        }

        /// <summary>
        /// Called when [configuration initialized].
        /// </summary>
        protected virtual void OnConfigurationInitialized()
        {
            if (ConfigurationInitialized != null)
                ConfigurationInitialized(this, new EventArgs());
        }

        /// <summary>
        /// Occurs, when the Visual Studio configuration is fully initialized
        /// </summary>
        public event EventHandler ConfigurationInitialized;
    }

}
