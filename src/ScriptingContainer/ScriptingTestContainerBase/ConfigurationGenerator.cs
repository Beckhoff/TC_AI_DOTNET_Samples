using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;
using Microsoft.Win32;
using System.Text.RegularExpressions;

namespace ScriptingTest
{

    /// <summary>
    /// DTE Info Object
    /// </summary>
    public class DTEInfo
    {
        /// <summary>
        /// DTE Name
        /// </summary>
        public readonly string Name;
        /// <summary>
        /// DTE Prog ID
        /// </summary>
        public readonly string ProgId;
        /// <summary>
        /// DTE Guid
        /// </summary>
        public readonly Guid Guid;

        /// <summary>
        /// Initializes a new instance of the <see cref="DTEInfo"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="progId">The prog id.</param>
        /// <param name="guid">The GUID.</param>
        public DTEInfo(string name, string progId, Guid guid)
        {
            this.Name = name;
            this.ProgId = progId;
            this.Guid = guid;
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return this.Name;
        }
    }

    /// <summary>
    /// Visual Studio Factory interface
    /// </summary>
    public interface IVsFactory
    {
        /// <summary>
        /// Creates the DTE.
        /// </summary>
        /// <param name="appId">Application ID to use.</param>
        /// <param name="ideVisible">if set to <c>true</c> the ide will be visible.</param>
        /// <param name="suppressUI">if set to <c>true</c> UI will be suppressed.</param>
        /// <param name="userControl">if set to <c>true</c> Visual Studio will be started with user control.</param>
        /// <returns></returns>
        dynamic CreateDTE(string appId, bool ideVisible, bool suppressUI, bool userControl);
        /// <summary>
        /// Creates a new Visual Studio project.
        /// </summary>
        /// <param name="dte">DTE object.</param>
        /// <param name="projectName">Name of the project.</param>
        /// <param name="templatePath">The template path.</param>
        /// <param name="projectRootFolder">The project root folder.</param>
        /// <returns></returns>
        dynamic CreateNewProject(dynamic dte, string projectName, string templatePath, string projectRootFolder);
    }

    /// <summary>
    /// Visual Studio Factory implementation
    /// </summary>
    public class VsFactory : IVsFactory
    {
        #region IVsFactory Members

        /// <summary>
        /// Creates the DTE.
        /// </summary>
        /// <param name="appId">Application ID to use.</param>
        /// <param name="ideVisible">if set to <c>true</c> the ide will be visible.</param>
        /// <param name="suppressUI">if set to <c>true</c> UI will be suppressed.</param>
        /// <param name="userControl">if set to <c>true</c> Visual Studio will be started with user control.</param>
        /// <returns></returns>
        public dynamic CreateDTE(string appId, bool ideVisible, bool suppressUI, bool userControl)
        {
            Type tp = Type.GetTypeFromProgID(appId);

            if (tp == null)
                throw new ApplicationException("AppID '{0}' not found!");

            dynamic dte = System.Activator.CreateInstance(tp, true);

            // Register the IOleMessageFilter to handle any threading 
            // errors.

            if (!MessageFilter.IsRegistered)
                MessageFilter.Register();

            dte.MainWindow.WindowState = 0;
            dte.MainWindow.Visible = ideVisible;
            dte.SuppressUI = suppressUI;
            dte.UserControl = userControl;
            return dte;
        }

        /// <summary>
        /// Creates the new project.
        /// </summary>
        /// <param name="dte">The DTE Object.</param>
        /// <param name="projectName">Name of the project.</param>
        /// <param name="templatePath">The template path.</param>
        /// <param name="scriptRootFolder">The script root folder.</param>
        /// <returns></returns>
        public dynamic CreateNewProject(dynamic dte, string projectName, string templatePath, string scriptRootFolder)
        {
            dynamic project = dte.Solution.AddFromTemplate(templatePath, scriptRootFolder, projectName, false);
            return project;
        }

        #endregion
    }

    /// <summary>
    /// Abstract implementation of the <see cref="IConfigurationFactory"/> interface.
    /// </summary>
    public abstract class ConfigurationFactory : IConfigurationFactory
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigurationFactory"/> class.
        /// </summary>
        protected ConfigurationFactory(IVsFactory vsFactory)
        {
            this._vsFactory = vsFactory;
        }

        IContextProvider _scriptContext = null;
        IVsFactory _vsFactory = null;

        /// <summary>
        /// Gets the currently used application path.
        /// </summary>
        /// <value>The application path.</value>
        public static string ApplicationPath
        {
            get
            {
                //return Path.GetDirectoryName(Application.ExecutablePath);
                
                //Doesn't work with unit tests
                //string path = System.Reflection.Assembly.GetEntryAssembly().Location;

                //ScirptingTestContainerBase should be in the right place.
                string path = System.Reflection.Assembly.GetExecutingAssembly().Location;
                return Path.GetDirectoryName(path);
            }
        }

        /// <summary>
        /// Handler function creating the Visual Studio instance.
        /// </summary>
        /// <returns></returns>
        protected virtual dynamic OnCreateVS()
        {
            dynamic dte = _vsFactory.CreateDTE(this.appID, this.IsIdeVisible, this.SuppressUI, this.IsIdeUserControl);

            // Updating the Settings (there is a dependency between them)
            this.IsIdeVisible = dte.MainWindow.Visible;
            this.SuppressUI = dte.SuppressUI;
            this.IsIdeUserControl = dte.UserControl;

            return dte;
        }

        /// <summary>
        /// Creates the solution and project configuration for the script.
        /// </summary>
        public void Initialize(IContext context)
        {
            this._scriptContext = (IContextProvider)context;
            dynamic dte = OnCreateVS();
            dynamic solution = dte.Solution;

            ((IContextProvider)context).SetDTE(dte, solution);

            OnCreateConfiguration();

            this.isConfigurationOpen = true;

            if (Initialized != null)
            {
                Initialized(this, new EventArgs());
            }
        }

        /// <summary>
        /// Occurs when the Visual Studio instance is initialized and the configuration(s) are open.
        /// </summary>
        public event EventHandler Initialized;

        /// <summary>
        /// Handler function to create the script configuration.
        /// </summary>
        protected virtual void OnCreateConfiguration(/*string projectTemplate*/)
        {
            //Debug.Fail("");
        }

        /// <summary>
        /// Executes the specified script with the specified worker context.
        /// </summary>
        /// <param name="script">The script to execute.</param>
        public void ExecuteScript(Script script)
        {
            this.executing = true;

            try
            {
                script.SetStatus(ScriptStatus.Initializing);

                //ScriptContext context = new ScriptContext(this.dte, this.Solution, this, worker,parameters);
                script.Initialize(/*this.dte, this.Solution, worker*/_scriptContext);
                script.SetStatus(ScriptStatus.Executing);
                script.Execute(_scriptContext.Worker);
            }
            catch (Exception ex)
            {
                Debug.Assert(script.Exception == ex);
                Debug.Assert(script.Result == ScriptResult.Fail);
                _scriptContext.Worker.ProgressStatus = string.Format("Error: {0}", ex.Message);
            }
            finally
            {
                script.SetStatus(ScriptStatus.Cleanup);
                script.CleanUp(_scriptContext.Worker);

                this.executing = false;
                script.SetStatus(ScriptStatus.None);
            }
        }

        /// <summary>
        /// Closes the configuration / Configuration Cleanup
        /// </summary>
        public void Cleanup()
        {
            OnClosingConfiguration();

            isConfigurationOpen = false;

            _scriptContext.SetDTE(null, null);

            //and turn off the IOleMessageFilter.
            MessageFilter.Revoke();
        }

        /// <summary>
        /// Handler function for Configuration cleanup.
        /// </summary>
        protected virtual void OnClosingConfiguration()
        {
        }

        //Obsolete public const string ProgID_XAE = "TwinCATXAE.DTE.3.0";
        /// <summary>
        /// ProgID of the VisualStudio.DTE object
        /// </summary>
        //public const string ProgID_VS = "VisualStudio.DTE.10.0";
        const string progIdRegEx = @"^VisualStudio\.DTE\.(?<Version>\d*\.\d*)$";


        /// <summary>
        /// Gets a dictionary of the currently installed Visual Studio ProgIds.
        /// </summary>
        /// <param name="currentIdx">Current index.</param>
        /// <param name="defaultIdx">Default index.</param>
        /// <returns></returns>
        public static IDictionary<string,DTEInfo> GetVisualStudioProgIds(out int currentIdx, out int defaultIdx)
        {
            Dictionary<string, DTEInfo> ret = new Dictionary<string, DTEInfo>();

            currentIdx = -1;
            defaultIdx = -1;

            string currentProgId = null;

            Version defVersion = new Version(10, 0);
            Regex regex = new Regex(progIdRegEx,RegexOptions.Singleline | RegexOptions.IgnoreCase | RegexOptions.Compiled);

            using (RegistryKey root = Registry.ClassesRoot)
            {
                using (RegistryKey dte = root.OpenSubKey("VisualStudio.DTE"))
                {
                    using (RegistryKey curVer = dte.OpenSubKey("CurVer"))
                    {
                        currentProgId = (string)curVer.GetValue("");
                    }

                    List<string> dteKeys = new List<string>(root.GetSubKeyNames().Where<string>(name => regex.IsMatch(name)));

                    int idx = 0;

                    for (int i = 0; i<dteKeys.Count; i++)
                    {
                        string progId = dteKeys[i];
                        string vsName = string.Empty;

                        if ((string.CompareOrdinal(currentProgId,progId) == 0))
                        {
                            currentIdx = idx;
                        }
                        Match match = regex.Match(progId);

                        Group versionGroup = match.Groups["Version"];
                        Version version = new Version((string)versionGroup.Value);

                        if (version == defVersion)
                            defaultIdx = idx;

                        switch (version.Major)
                        {
                            case 9:
                                vsName = "Visual Studio 2008";
                                break;
                            case 10:
                                vsName = "Visual Studio 2010";
                                break;
                            case 11:
                                vsName = "Visual Studio 2012";
                                break;
                            case 12:
                                vsName = "Visual Studio 2013";
                                break;
                            case 14:
                                vsName = "Visual Studio 2015";
                                break;
                            default:
                                //Debug.Fail("VS version unknown!");
                                vsName = string.Format("Visual Studio ({0}.{1})",version.Major,version.Minor);
                                break;
                        }

                        if (version >= new Version(10, 0)) // Only Version > 10.0 are allowed
                        {
                            Guid clsid = Guid.Empty;

                            using (RegistryKey dteVersioned = root.OpenSubKey(progId))
                            {
                                using (RegistryKey classId = dteVersioned.OpenSubKey("CLSID"))
                                {
                                    clsid = new Guid((string)classId.GetValue(""));
                                    
                                    DTEInfo info =new DTEInfo(vsName,progId,clsid);
                                    ret.Add(progId, info);
                                    idx++;
                                }
                            }
                        }
                    }
                }
            }
            return ret;
        }


        /// <summary>
        /// Application ID of the Program to be created
        /// </summary>
        protected string appID = null;

        /// <summary>
        /// Gets or sets the Application ID
        /// </summary>
        /// <value>The app ID.</value>
        public string AppID
        {
            get { return appID; }

            set
            {
                appID = value;
            }
        }

        /// <summary>
        /// Indicates that the configuration is open.
        /// </summary>
        protected bool isConfigurationOpen = false;

        /// <summary>
        /// Gets a value indicating whether a configuration is opened.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is configuration open; otherwise, <c>false</c>.
        /// </value>
        public bool IsConfigurationOpen
        {
            get { return isConfigurationOpen; }
        }

        /// <summary>
        /// Indicates that a script is executing
        /// </summary>
        protected bool executing = false;

        /// <summary>
        /// Gets a value indicating whether a script created by this Factory is Executing.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is generating; otherwise, <c>false</c>.
        /// </value>
        public bool IsExecuting
        {
            get { return executing; }
        }

        ///// <summary>
        ///// VisualStudio.DTE object created by this factory
        ///// </summary>
        //protected dynamic dte = null;

        ///// <summary>
        ///// VisualStudio Solution created by this factory.
        ///// </summary>
        //protected dynamic sln = null;

        /// <summary>
        ///  VisualStudio.DTE object created by this factory.
        /// </summary>
        /// <value>The DTE.</value>
        public dynamic DTE
        {
            get
            {
                if (_scriptContext != null)
                    return _scriptContext.DTE;
                else
                    return null;
            }
        }

        /// <summary>
        /// VisualStudio Solution created by this factory.
        /// </summary>
        /// <value>The solution.</value>
        public dynamic Solution
        {
            get
            {
                if (_scriptContext != null)
                    return _scriptContext.Solution;
                else
                    return null;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the IDE will be shown during Script processing
        /// </summary>
        /// <value><c>true</c> if [show application]; otherwise, <c>false</c>.</value>
        public bool IsIdeVisible { get; set; }
        
        /// <summary>
        /// Gets or sets a value indicating whether this instance is IDE user control.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is IDE user control; otherwise, <c>false</c>.
        /// </value>
        public bool IsIdeUserControl { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether UI in Visual Studio is suppressed.
        /// </summary>
        /// <value><c>true</c> if UI is suppressed; otherwise, <c>false</c>.</value>
        public bool SuppressUI { get; set; }

        #region IConfigurationFactory Members


        /// <summary>
        /// Gets the Visual Studio Factory
        /// </summary>
        /// <value>
        /// The Visual Studio Factory
        /// </value>
        public IVsFactory VsFactory
        {
            get { return _vsFactory; }
        }

        #endregion

        #region IConfigurationFactory Members


        /// <summary>
        /// Gets the Visual Studio Process ID.
        /// </summary>
        /// <value>The Visual Studio process identifier.</value>
        public int VsProcessId
        {
            get
            {
                dynamic dte = this.DTE;

                if (dte != null)
                {
                    Dictionary<ROTDteInfo, EnvDTE.DTE> dict = ROTAccess.GetRunningDTETable();

                    foreach (KeyValuePair<ROTDteInfo, EnvDTE.DTE> pair in dict)
                    {
                        ROTDteInfo info = pair.Key;
                        EnvDTE.DTE infoDTE = pair.Value;

                        if (dte == infoDTE)
                        {
                            return info.ProcessId;
                        }
                    }
                }
                return -1;
            }
        }

        #endregion
    }
}
