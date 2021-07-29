using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;
using ScriptingTest;

namespace ScriptingTest
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Indicates, that the main window is initializing.
        /// </summary>
        bool _initializing = false;

        /// <summary>
        /// Gets a value Indicating that the <see cref="MainWindow"/> is currently initializing.
        /// </summary>
        /// <value>
        ///   <c>true</c> if [is initializing]; otherwise, <c>false</c>.
        /// </value>
        public bool IsInitializing
        {
            get { return _initializing; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindow"/> class.
        /// </summary>
        public MainWindow()
        {
            _initializing = true;

            InitializeComponent();

            int defaultIndex = -1;
            int currentIndex = -1;
            IDictionary<string, DTEInfo> progIdDict = ConfigurationFactory.GetVisualStudioProgIds(out currentIndex, out defaultIndex);

            //List<string> progIds = new List<string>(progIdDict.Keys);

            
            foreach (DTEInfo key in progIdDict.Values)
            {
                this.cBProgID.Items.Add(key);
            }

            int actualIndex = defaultIndex;

            if (actualIndex == -1)
                actualIndex = currentIndex;


            //obsolete this.cBProgID.Items.Add(ConfigurationGenerator.ProgID_XAE);

            this.cBProgID.SelectedIndex = actualIndex;

            this.cbProgVisible.IsChecked = true;
            this.cbUserControl.IsChecked = true;
            this.cbSuppressUI.IsChecked = true;

            _initializing = false;

            // Setting Script filter (Filter Out all Exes)

            ScriptLoader.ScriptFilter = new Func<Type, bool>(

                (type) =>
                {
                    return (type.Assembly.EntryPoint == null);
                }
            );

            Version version = typeof(ScriptLoader).Assembly.GetName().Version;
            this.Title = string.Format("TwinCAT3 Script Container (TwinCAT XAE Base Type Library '{0}')", version.ToString(2));

            setScripts(ScriptLoader.Scripts); // Setting the Scripts to the ListView
            enableDisableControls(); // EnableDisable Visual Elements
        }

        /// <summary>
        /// Sets the scripts within the ListView
        /// </summary>
        /// <param name="scripts">The scripts.</param>
        private void setScripts(IList<Script> scripts)
        {
            //this.lVScripts.DataContext = scripts;
            this.lVScripts.ItemsSource = scripts;
            CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(lVScripts.ItemsSource);
            view.SortDescriptions.Add(new SortDescription("Category", ListSortDirection.Ascending));

        }

        /// <summary>
        /// Gets the currently configured Prog ID for TwinCAT 3
        /// </summary>
        /// <value>The prog ID.</value>
        public DTEInfo DTEInfo
        {
            get { return (DTEInfo)cBProgID.SelectedValue; }
        }

        /// <summary>
        /// Gets a value indicating whether the IDE is visible during script
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is IDE visible; otherwise, <c>false</c>.
        /// </value>
        public bool IsIDEVisible
        {
            get { return cbProgVisible.IsChecked.Value; }
        }

        /// <summary>
        /// Gets a value indicating whether the IDE is controlled by User
        /// </summary>
        /// <value>
        /// </value>
        public bool IsIDEUserControl
        {
            get { return cbUserControl.IsChecked.Value; }
        }

        /// <summary>
        /// Gets a value indicating whether VisualStudio UI is suppressed.
        /// </summary>
        /// <value>
        /// </value>
        public bool SupressUI
        {
            get { return cbSuppressUI.IsChecked.Value; }
        }

        /// <summary>
        /// Background script worker
        /// </summary>
        IWorker _worker = null;

        /// <summary>
        /// Configuration Generator
        /// </summary>
        ConfigurationFactory _factory = null;

        /// <summary>
        /// Currently runnig script
        /// </summary>
        Script _runningScript = null;

        /// <summary>
        /// Handles the Click event of the btnExecute control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void btnExecute_Click(object sender, RoutedEventArgs e)
        {
            Debug.Assert(this._factory == null);

            if (this.SelectedScript != null)
            {
                _runningScript = this.SelectedScript;
                _runningScript.StatusChanged += new EventHandler<ScriptStatusChangedEventArgs>(script_StatusChanged);

                VsFactory vsFactory = new VsFactory();

                if (_runningScript is ScriptEarlyBound)
                    this._factory = new EarlyBoundFactory(vsFactory);
                else if (_runningScript is ScriptLateBound)
                    this._factory = new LateBoundFactory(vsFactory);

                if (this._factory == null)
                {
                    throw new ApplicationException("Generator not found!");
                }

                Dictionary<string, dynamic> parameterSet = new Dictionary<string, dynamic>();
                ScriptContext context = new ScriptContext(_factory, null, parameterSet);
                
                _worker = new ScriptBackgroundWorker(/*this._factory,*/ _runningScript,context);
                this.Update(_runningScript);
                _worker.ConfigurationInitialized += new EventHandler(_worker_ConfigurationInitialized);
                _worker.ProgressChanged += new ProgressChangedEventHandler(_worker_ProgressChanged);
                _worker.ProgressStatusChanged += new EventHandler<ProgressStatusChangedArgs>(_worker_ProgressStatusChanged);
                _worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(_worker_RunWorkerCompleted);
                _factory.AppID = this.DTEInfo.ProgId;
                _factory.IsIdeVisible = this.IsIDEVisible;
                _factory.IsIdeUserControl = this.IsIDEUserControl;
                _factory.SuppressUI = this.SupressUI;
                SetExecution(true);
                _worker.BeginScriptExecution();
            }
        }

        void _worker_ConfigurationInitialized(object sender, EventArgs e)
        {
            Action action = () =>
                {
                    this.cbProgVisible.IsChecked = _factory.IsIdeVisible;
                    this.cbSuppressUI.IsChecked = _factory.SuppressUI;
                    this.cbUserControl.IsChecked = _factory.IsIdeUserControl;
                };

            this.Dispatcher.Invoke(action, new object[] { });
        }

        void _worker_ProgressStatusChanged(object sender, ProgressStatusChangedArgs e)
        {
            if (this.Dispatcher.CheckAccess())
            {
                int index = this.lvMessages.Items.Add(e.Status);
                object item = this.lvMessages.Items[index];
                this.lvMessages.ScrollIntoView(item);
            }
            else
            {
                Action<string> action = (str) =>
                {
                    int index = this.lvMessages.Items.Add(str);
                    object item = this.lvMessages.Items[index];
                    this.lvMessages.ScrollIntoView(item);
                };

                this.Dispatcher.Invoke(action, new object[] { e.Status });
            }
        }

        /// <summary>
        /// Handles the StatusChanged event of the script control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="ScriptingTest.ScriptStatusChangedEventArgs"/> instance containing the event data.</param>
        void script_StatusChanged(object sender, ScriptStatusChangedEventArgs e)
        {
            Action<Script> action = (script) =>
            {
                if (e.NewState == ScriptStatus.Initializing)
                {
                    this.lvMessages.Items.Clear();
                }

                Update(script);
            };

            this.Dispatcher.Invoke(action, new object[] { this._runningScript });
        }

        /// <summary>
        /// Updates the Main window with the Script status
        /// </summary>
        /// <param name="script">The script.</param>
        private void Update(Script script)
        {
            switch (script.Status)
            {
                case ScriptStatus.None:
                    if (script.Result == ScriptResult.None)
                        statusBox.Background = Brushes.Silver;
                    else if (script.Result == ScriptResult.Fail)
                        statusBox.Background = Brushes.Red;
                    else if (script.Result == ScriptResult.Ok)
                        statusBox.Background = Brushes.Green;
                    break;
                case ScriptStatus.Initializing:
                case ScriptStatus.Cleanup:
                    statusBox.Background = Brushes.Gray;
                    break;
                case ScriptStatus.Executing:
                    statusBox.Background = Brushes.Yellow;
                    break;
            }
        }

        /// <summary>
        /// Handles the RunWorkerCompleted event of the _worker control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.ComponentModel.RunWorkerCompletedEventArgs"/> instance containing the event data.</param>
        void _worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            Action<Script> action = (script) =>
            {
                SetExecution(false);
                _factory = null;
                Update(script);
                _runningScript.StatusChanged -= new EventHandler<ScriptStatusChangedEventArgs>(script_StatusChanged);
                _runningScript = null;
            };

            this.Dispatcher.BeginInvoke(action, new object[] { this._runningScript });
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
            get { return _executing; }
        }

        /// <summary>
        /// Sets the execution flag.
        /// </summary>
        /// <param name="set">if set to <c>true</c> [set].</param>
        private void SetExecution(bool set)
        {
            _executing = set;
            enableDisableControls();
        }

        /// <summary>
        /// Enables / Disables (updates) the Main form controls
        /// </summary>
        private void enableDisableControls()
        {
            bool scriptSelected = (this.SelectedScript != null);
            bool isExecuting = this.IsExecuting;
            bool isCancelPending = this.IsExecuting && this._worker.CancellationPending;

            this.btnCancel.IsEnabled = isExecuting && !isCancelPending;
            this.btnExecute.IsEnabled = !isExecuting && scriptSelected;
            //this.gbBinding.IsEnabled = !isExecuting;
            this.gbProgID.IsEnabled = !isExecuting;
            this.lVScripts.IsEnabled = !isExecuting;

            gBSettings.IsEnabled = !isExecuting;
        }

        /// <summary>
        /// Handles the ProgressChanged event of the _worker control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.ComponentModel.ProgressChangedEventArgs"/> instance containing the event data.</param>
        void _worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (this.Dispatcher.CheckAccess())
            {
                this.progressBar1.Value = e.ProgressPercentage;
            }
            else
            {
                Action<int> action = (i) =>
                {
                    this.progressBar1.Value = i;
                };

                this.Dispatcher.Invoke(action, new object[] { e.ProgressPercentage });
            }
        }

        /// <summary>
        /// Gets the currently selected script.
        /// </summary>
        /// <value>The selected script.</value>
        public Script SelectedScript
        {
            get
            {
                return (Script)this.lVScripts.SelectedItem;
            }
        }

        /// <summary>
        /// Handles the Click event of the btnCancel control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            if (this._worker != null)
            {
                this._worker.CancelRequest();
                this.enableDisableControls();
                //this._worker.CancelAndWait(TimeSpan.FromSeconds(0.1));
            }
        }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Window.Closing"/> event.
        /// </summary>
        /// <param name="e">A <see cref="T:System.ComponentModel.CancelEventArgs"/> that contains the event data.</param>
        protected override void OnClosing(CancelEventArgs e)
        {
            if (this._worker != null)
                this._worker.CancelAndWait(TimeSpan.FromSeconds(1.0));

            base.OnClosing(e);
        }

        /// <summary>
        /// Handles the SelectionChanged event of the lVScripts control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Controls.SelectionChangedEventArgs"/> instance containing the event data.</param>
        private void lVScripts_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ToolTip toolTip = new ToolTip();

            Script selected = this.SelectedScript;
            if (selected != null)
                toolTip.Content = selected.DetailedDescription;
            else
                toolTip.Content = string.Empty;

            lVScripts.ToolTip = toolTip;
            enableDisableControls();
            Update(selected);
        }
    }
}
