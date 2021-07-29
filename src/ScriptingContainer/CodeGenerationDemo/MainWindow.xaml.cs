using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Reflection;
using System.ComponentModel;
using ScriptingTest;
using System.Diagnostics;
//using Scripting.CSharp;
using System.Xml;

namespace CodeGenerationDemo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>

    public partial class MainWindow : Window
    {
        /// <summary>
        /// Indicates that the Main window is initializing
        /// </summary>
        bool _initializing = false;

        public bool IsInitializing
        {
            get { return _initializing; }
        }

        public MainWindow()
        {
            _initializing = true;

            try
            {
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

                this.cBProgID.SelectedIndex = actualIndex;

                if (DesignerProperties.GetIsInDesignMode(this))
                {
                    // Design-mode specific functionality;
                }
                else
                {
                    //_model = new DataModel();
                    _model = (DataModel)this.LayoutRoot.DataContext;
                    _model.Load();
                }

#if DEBUG
            this.Topmost = false;
            this.cbProgVisible.Visibility = Visibility.Visible;
            this.cbSuppressUI.Visibility = Visibility.Visible;
            this.cbUserControl.Visibility = Visibility.Visible;
#endif

                this.cbProgVisible.IsChecked = true;
                this.cbUserControl.IsChecked = true;
                this.cbSuppressUI.IsChecked = true;

                _initializing = false;
                enableDisableControls(); // EnableDisable Visual Elements
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                throw;
            }
        }

        DataModel _model = null;

        public DataModel Model
        {
            get { return _model; }
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
        private void btnGenerate_Click(object sender, RoutedEventArgs e)
        {
            Debug.Assert(this._factory == null);

            try
            {
                OrderInfo order = this.SelectedOrder;
                Script script = ScriptLoader.GetScript(order.ConfigurationInfo.Script);

                if (script == null)
                    throw new ApplicationException(string.Format("Script '{0}' not found. Cannot start execution!", order.ConfigurationInfo.Script));

                _runningScript = script;
                _runningScript.StatusChanged += new EventHandler<ScriptStatusChangedEventArgs>(script_StatusChanged);

                VsFactory fact = new VsFactory();

                if (_runningScript is ScriptEarlyBound)
                    this._factory = new EarlyBoundFactory(fact);
                else if (_runningScript is ScriptLateBound)
                    this._factory = new LateBoundFactory(fact);

                if (this._factory == null)
                {
                    throw new ApplicationException("Generator not found!");
                }

                OrderScriptContext context = new OrderScriptContext(this._factory, order);

                _worker = new ScriptBackgroundWorker(/*this._factory,*/ _runningScript, context);
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
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
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
        /// <param name="e">The <see cref="TwinCAT3.ScriptingTest.ScriptStatusChangedEventArgs"/> instance containing the event data.</param>
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
                        statusBox.Background = Brushes.White;
                    else if (script.Result == ScriptResult.Fail)
                        statusBox.Background = Brushes.Red;
                    else if (script.Result == ScriptResult.Ok)
                        statusBox.Background = Brushes.Green;
                    break;
                case ScriptStatus.Initializing:
                case ScriptStatus.Cleanup:
                    this.statusBox.Background = Brushes.Silver;
                    break;
                case ScriptStatus.Executing:
                    this.statusBox.Background = Brushes.Yellow;
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
            bool scriptSelected = (this.SelectedOrder != null);
            bool isExecuting = this.IsExecuting;

            bool isCancelPending = this.IsExecuting && this._worker.CancellationPending;

            this.btnCancel.IsEnabled = isExecuting && !isCancelPending;

            this.btnGenerate.IsEnabled = !isExecuting && scriptSelected;
            this.cbProgVisible.IsEnabled = !isExecuting;
            this.cbSuppressUI.IsEnabled = !isExecuting;
            this.cbUserControl.IsEnabled = !isExecuting;
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
                Action<int> action = (i) => {this.progressBar1.Value = i;};
                this.Dispatcher.Invoke(action, new object[] { e.ProgressPercentage });
            }
        }

        /// <summary>
        /// Gets the currently selected script.
        /// </summary>
        /// <value>The selected script.</value>
        public OrderInfo SelectedOrder
        {
            get
            {
                OrderInfo selected = (OrderInfo)this.listBox1.SelectedItem;
                return selected;
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
                //this._worker.CancelAndWait(TimeSpan.FromSeconds(1.0));
                this.enableDisableControls();
            }
        }

        /// <summary>
        /// Gets the currently configured Prog ID for TwinCAT 3
        /// </summary>
        /// <value>The prog ID.</value>
        public DTEInfo DTEInfo
        {
            get { return (DTEInfo)cBProgID.SelectedValue; }
        }

        private void listBox1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            enableDisableControls();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            if (this._executing)
                this._worker.CancelAndWait(TimeSpan.FromSeconds(2.0));

            base.OnClosing(e);
        }

    }
}
