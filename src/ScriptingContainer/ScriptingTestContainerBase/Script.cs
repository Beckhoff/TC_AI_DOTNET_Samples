using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;
using EnvDTE100;
using EnvDTE80;
using TCatSysManagerLib;
using System.Diagnostics;
using EnvDTE;
using Microsoft.Win32;

namespace ScriptingTest
{
    /// <summary>
    /// Actual status of the Script
    /// </summary>
    public enum ScriptStatus
    {
        /// <summary>
        /// None / Uninitialized
        /// </summary>
        None,
        /// <summary>
        /// Initializing the Script and its initial configuration
        /// </summary>
        Initializing,
        /// <summary>
        /// Cleaning up the Script / Configuration
        /// </summary>
        Cleanup,
        /// <summary>
        /// Script currently executing.
        /// </summary>
        Executing,
    }

    /// <summary>
    /// Abstract base class for scripts
    /// </summary>
    public abstract class Script
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Script"/> class.
        /// </summary>
        protected Script()
        {
        }

        /// <summary>
        /// Script context
        /// </summary>
        protected IContext _context = null;

        /// <summary>
        /// Gets the context.
        /// </summary>
        /// <value>The context.</value>
        public IContext Context
        {
            get { return _context; }
        }

        /// <summary>
        /// Initializes the script with DTE, Solution and Script Worker object
        /// </summary>
        /// <param name="context">The Script Context.</param>
        public void Initialize(IContext context)
        {
            try
            {
                _context = context;
                OnBeforeInitialize(context);
                OnInitialize(context);

                context.Worker.ProgressStatus = "Creating solution ...";
                this.CreateSolution();

                OnSolutionCreated();
            }
            catch (Exception ex)
            {
                this._ex = ex;
                this._result = ScriptResult.Fail;
                throw ex;
            }   
        }

        /// <summary>
        /// Script status
        /// </summary>
        ScriptStatus _status;

        /// <summary>
        /// Gets the Script Status
        /// </summary>
        /// <value>The status.</value>
        public ScriptStatus Status
        {
            get { return _status; }
        }

        /// <summary>
        /// Executes the script with worker context.
        /// </summary>
        /// <param name="worker">The worker.</param>
        public void Execute(IWorker worker)
        {
            try
            {
                this._result = ScriptResult.None;
                this._ex = null;

                SetStartTime();
                OnExecute(worker);

                this._result = ScriptResult.Ok;
                this._ex = null;
            }
            catch (Exception ex)
            {
                this._result = ScriptResult.Fail;
                this._ex = ex;  
                throw ex;
            }
            finally
            {
                SetStopTime();
                worker.Progress = 100; ;
            }
        }

        /// <summary>
        /// Cleans the Script up after usage.
        /// </summary>
        /// <param name="worker">The worker.</param>
        public void CleanUp(IWorker worker)
        {
            try
            {
                OnCleanUp(worker);
                _context = null;
            }
            catch (Exception ex)
            {
                this._ex = ex;
                this._result = ScriptResult.Fail;
                //throw ex;
            }
        }


        /// <summary>
        /// Script Start time
        /// </summary>
        DateTime _startTime = DateTime.MinValue;

        /// <summary>
        /// Gets the start time of the Script
        /// </summary>
        /// <value>
        /// The start time.
        /// </value>
        protected DateTime StartTime
        { 
            get { return _startTime; }
        }

        /// <summary>
        /// Sets the start time.
        /// </summary>
        private void SetStartTime()
        {
            _startTime = DateTime.Now;
        }

        /// <summary>
        /// Script stop time
        /// </summary>
        DateTime _stopTime = DateTime.MinValue;

        /// <summary>
        /// Gets the Script stop time
        /// </summary>
        /// <value>
        /// The stop time.
        /// </value>
        protected DateTime StopTime
        { 
            get { return _stopTime; }
        }

        /// <summary>
        /// Sets the stop time.
        /// </summary>
        private void SetStopTime()
        {
            _stopTime = DateTime.Now;
        }

        /// <summary>
        /// Gets the Scripts execution duration.
        /// </summary>
        /// <value>
        /// The duration.
        /// </value>
        public TimeSpan Duration { get { return (StopTime - StartTime); } }

        /// <summary>
        /// Handler function Before Initializing the Script (Configuration preparations)
        /// </summary>
        /// <param name="context">The Script Context</param>
        /// <remarks>Usually used to to the open a prepared or new XAE configuration</remarks>
        protected abstract void OnBeforeInitialize(IContext context);

        /// <summary>
        /// Handler function Initializing the Script (Configuration preparations)
        /// </summary>
        /// <param name="context">The Script Context</param>
        /// <remarks>Usually used to to the open a prepared or new XAE configuration</remarks>
        protected abstract void OnInitialize(IContext context);
        
        /// <summary>
        /// Handler function Executing the Script code.
        /// </summary>
        /// <param name="worker">The worker.</param>
        protected abstract void OnExecute(IWorker worker);

        /// <summary>
        /// Cleaning up the XAE configuration after script execution.
        /// </summary>
        /// <param name="worker">The worker.</param>
        protected abstract void OnCleanUp(IWorker worker);

        /// <summary>
        /// Gets the Script description
        /// </summary>
        /// <value>The description.</value>
        public abstract string Description { get; }

        /// <summary>
        /// Gets the detailed description of the <see cref="Script"/> that is shown in the Method Tips.
        /// </summary>
        /// <value>The detailed description.</value>
        public virtual string DetailedDescription
        {
            get { return this.Description; }
        }

        /// <summary>
        /// Gets the keywords, describing the Script features
        /// </summary>
        /// <value>The keywords.</value>
        public virtual string Keywords
        {
            get { return string.Empty;  }
        }


        /// <summary>
        /// Gets the programming language of the Script
        /// </summary>
        /// <value>The programming language.</value>
        public virtual string Language
        {
            get { return "C#";  }
        }

        /// <summary>
        /// Gets the Version number of TwinCAT that is necessary for script execution.
        /// </summary>
        /// <value>The TwinCAT version.</value>
        public virtual Version TwinCATVersion
        {
            get
            {
                return new Version(3, 1);
            }
        }

        /// <summary>
        /// Gets the build number of TwinCAT that is necessary for script execution.
        /// </summary>
        /// <value>The TwinCAT build.</value>
        public virtual string TwinCATBuild
        {
            get
            {
                return "4012";
            }
        }

        /// <summary>
        /// Gets the category of this script.
        /// </summary>
        /// <value>The script category.</value>
        public virtual string Category
        {
            get
            {
                return "";
            }
        }

        /// <summary>
        /// Gets the binding type of the script
        /// </summary>
        /// <value>The programming language.</value>
        public abstract string Binding { get; }

        /// <summary>
        /// Gets the base Application Directory, where the ScriptingTestContainer base Dlls are residing.
        /// </summary>
        /// <value>The application path.</value>
        public static string ApplicationDirectory
        {
            get
            {
                return Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            }
        }

        Exception _ex = null;
        /// <summary>
        /// Gets the last execution Exception.
        /// </summary>
        /// <value>The exception.</value>
        public Exception Exception
        {
            get
            {
                return _ex;
            }
        }

        /// <summary>
        /// Gets the name of the Script.
        /// </summary>
        /// <value>The name.</value>
        public virtual string ScriptName
        {
            get { return this.GetType().Name; }
        }

        /// <summary>
        /// Gets the script templates folder used for configuration templates.
        /// </summary>
        /// <value>The script templates folder.</value>
        public string ConfigurationTemplatesFolder
        {
            get { return Path.Combine(ApplicationDirectory, "Templates"); }
        }

        /// <summary>
        /// XAE Base Template (TwinCAT 30
        /// </summary>
        public static string vsXaeTemplateName30 = "TwinCAT Project.tsp";

        /// <summary>
        /// XAE Base Template new (>= TwinCAT 3.1)
        /// </summary>
        public static string vsXaeTemplateName = "TwinCAT Project.tsproj";

        /// <summary>
        /// Gets the Path of the TwinCAT XAE Base Template
        /// </summary>
        /// <value>The vs xae template path.</value>
        public static string VsXaeTemplatePath
        {
            get
            {

                string path = Path.Combine(TwinCATInstallDir, @"Components\Base\PrjTemplate", vsXaeTemplateName);

                if (!File.Exists(path))
                {
                    // Try out the Twincat30 "*.tsProj"
                    string path2 = Path.Combine(TwinCATInstallDir, @"Components\Base\PrjTemplate", vsXaeTemplateName30);

                    if (File.Exists(path2))
                        return path2;
                }
                return path;
            }
        }

        /// <summary>
        /// Template Name for the XAE PLC Empty template
        /// </summary>
        public static string vsXaePlcEmptyTemplateName = "Empty PLC Template.plcproj";
        /// <summary>
        /// Template Name for the XAE PLC Standard template
        /// </summary>
        public static string vsXaePlcStandardTemplateName = "Standard PLC Template.plcproj";

        /// <summary>
        /// Template Name for the XAE Saftey Standard template
        /// </summary>
        /// 
        //public static string vsXaeSafTemplateName = "TcTemplateDrv.splcproj";

        public static string vsXaeSafePlcWizard = "TcSafePlcWizard.vsz";


        /// <summary>
        /// Gets the TwinCAT Installation Directory
        /// </summary>
        /// <value>The twin CAT install dir.</value>
        public static string TwinCATInstallDir
        {
            get
            {
                string ret = null;

                Version tcVer = CurrentTwinCATVersion;

                string path;

                if (TcEnvironment.Is64BitProcess)
                    path = "Software\\Wow6432Node\\Beckhoff\\TwinCAT3";
                else
                    path = "Software\\Beckhoff\\TwinCAT3";

                using (RegistryKey key = Registry.LocalMachine.OpenSubKey(string.Format("{0}\\{1}",path,tcVer.ToString(2))))
                {
                    ret = (string)key.GetValue("InstallDir");
                }
                return ret;
            }
        }

        /// <summary>
        /// Gets the actual activated TwinCAT Version
        /// </summary>
        /// <value>The current twin CAT version.</value>
        public static Version CurrentTwinCATVersion
        {
            get
            {
                string ret = null;

                string path = string.Empty;

                if (TcEnvironment.Is64BitProcess)
                    path = "Software\\Wow6432Node\\Beckhoff\\TwinCAT3";
                else
                    path = "Software\\Beckhoff\\TwinCAT3";

                using (RegistryKey key = Registry.LocalMachine.OpenSubKey(path))
                {
                    ret = (string)key.GetValue("CurrentVersion");
                }

                if (ret == null) throw new ApplicationException("Could not determine actual TwinCAT Version!");

                return new Version(ret);
            }
        }

        /// <summary>
        /// Gets the working folder.
        /// </summary>
        /// <value>The working folder.</value>
        public string WorkingFolder
        {
            get { return Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location); }
        }

        /// <summary>
        /// Gets the Script Root Folder
        /// </summary>
        /// <value>The solution folder.</value>
        public string ScriptRootFolder
        {
            get
            {
                string solutionPath = Path.Combine(WorkingFolder, this.ScriptName);
                return solutionPath;
            }
        }

        /// <summary>
        /// Deletes the solution folder.
        /// </summary>
        public void DeleteSolutionFolder()
        {
            DeleteDirectory(this.ScriptRootFolder);
        }

        /// <summary>
        /// Gets the temp folder (Folder within the Script Root folder (SolutionFolder)
        /// </summary>
        /// <value>The temp folder.</value>
        public string ScriptTempFolder
        {
            get
            {
                return Path.Combine(this.ScriptRootFolder, "Temp");
            }
        }

        /// <summary>
        /// Creates an empty Visual Studio Solution
        /// </summary>
        protected abstract void CreateSolution();

        /// <summary>
        /// Handler function called after the Solution object has been created.
        /// </summary>
        protected virtual void OnSolutionCreated()
        {
        }


        /// <summary>
        /// Creates the new project.
        /// </summary>
        /// <param name="projectName">Name of the project.</param>
        /// <returns></returns>
        protected abstract object CreateNewProject(string projectName);


        /// <summary>
        /// Creates the new project.
        /// </summary>
        /// <returns></returns>
        protected object CreateNewProject()
        {
            return CreateNewProject(this.ScriptName);
        }

        /// <summary>
        /// Creates the new project.
        /// </summary>
        /// <param name="projectName">Name of the project.</param>
        /// <param name="templatePath">The template path.</param>
        /// <returns></returns>
        protected abstract object CreateNewProject(string projectName, string templatePath);

        ///// <summary>
        ///// Opens the Visual Studio Solution
        ///// </summary>
        //public abstract void OpenSolution();

        

        ///// <summary>
        ///// Deletes the specified directory
        ///// </summary>
        ///// <param name="target_dir">The target_dir.</param>
        //public static void DeleteDirectory(string target_dir)
        //{
        //      if (Directory.Exists(target_dir))
        //      {
        //          DeleteDirectoryFiles(target_dir);
        //          while (Directory.Exists(target_dir))
        //          {
        //              //lock (_lock)
        //              {
        //                  DeleteDirectoryDirs(target_dir);
        //              }
        //          }
        //      }
        //}

        ///// <summary>
        ///// Deletes all Directories recursively
        ///// </summary>
        ///// <param name="target_dir">The target_dir.</param>
        //private static void DeleteDirectoryDirs(string target_dir)
        //{
        //    System.Threading.Thread.Sleep(100);

        //    if (Directory.Exists(target_dir))
        //    {
        //        string[] dirs = Directory.GetDirectories(target_dir);

        //        if (dirs.Length == 0)
        //            Directory.Delete(target_dir, false);
        //        else
        //            foreach (string dir in dirs)
        //                DeleteDirectoryDirs(dir);
        //    }
        //}

        /// <summary>
        /// Deletes all files in the subtree
        /// </summary>
        /// <param name="target_dir">Target directory.</param>
        private static bool DeleteDirectory(string target_dir)
        {
            if (string.IsNullOrEmpty(target_dir)) throw new ArgumentOutOfRangeException("target_dir");

            if (!Directory.Exists(target_dir)) return false;

            string[] files = Directory.GetFiles(target_dir);
            string[] dirs = Directory.GetDirectories(target_dir);

            foreach (string dir in dirs)
            {
                DeleteDirectory(dir);
            }

            foreach (string file in files)
            {
                File.SetAttributes(file, FileAttributes.Normal);
                File.Delete(file);
            }
            Directory.Delete(target_dir, true);
            return true;
        }

        //public void CopyTemplateToTemp()
        //{
        //    Debug.Fail("Sollte das nicht ins Script?");
        //    DirectoryInfo targetDir = new DirectoryInfo(WorkingFolder);

        //    string workingSolutionPath = Path.Combine(WorkingFolder, "DemoProject");
        //    string workingPlcTemplatePath = Path.Combine(WorkingFolder, "PlcTemplate1");

        //    if (!targetDir.Exists)
        //        targetDir.Create();

        //    if (Directory.Exists(workingSolutionPath))
        //        Script.DeleteDirectory(workingSolutionPath);

        //    if (Directory.Exists(workingPlcTemplatePath))
        //        Script.DeleteDirectory(workingPlcTemplatePath);

        //    CopyDirectory(Path.Combine(this.ConfigurationTemplatesFolder, "DemoProject"), workingSolutionPath);
        //    CopyDirectory(Path.Combine(this.ConfigurationTemplatesFolder, "PlcTemplate1"), workingPlcTemplatePath);
        //}

        /// <summary>
        /// Copies a directory from Source to Destination
        /// </summary>
        /// <param name="src">Source Folder</param>
        /// <param name="dst">Destingation Folder</param>
        public static void CopyDirectory(string src, string dst)
        {
            String[] files;

            if (dst[dst.Length - 1] != Path.DirectorySeparatorChar)
                dst += Path.DirectorySeparatorChar;

            if (!Directory.Exists(dst))
                Directory.CreateDirectory(dst);

            files = Directory.GetFileSystemEntries(src);

            foreach (string element in files)
            {
                // Sub directories

                if (Directory.Exists(element))
                    CopyDirectory(element, dst + Path.GetFileName(element));
                // Files in directory

                else
                    File.Copy(element, dst + Path.GetFileName(element), true);
            }
        }


        /// <summary>
        /// Sets the Script Status
        /// </summary>
        /// <param name="scriptStatus">The script status.</param>
        public void SetStatus(ScriptStatus scriptStatus)
        {
            if (scriptStatus != this._status)
            {
                ScriptStatus oldStatus = this._status;
                this._status = scriptStatus;
                OnStatusChanged(oldStatus, this._status);
            }
        }

        /// <summary>
        /// Handler function fireing the <see cref="StatusChanged"/> event.
        /// </summary>
        /// <param name="oldStatus">The old status.</param>
        /// <param name="scriptStatus">The script status.</param>
        private void OnStatusChanged(ScriptStatus oldStatus, ScriptStatus scriptStatus)
        {
            if (StatusChanged != null)
                StatusChanged(this, new ScriptStatusChangedEventArgs(oldStatus, scriptStatus));
        }

        /// <summary>
        /// Occurs when the <see cref="ScriptStatus"/> has changed.
        /// </summary>
        public event EventHandler<ScriptStatusChangedEventArgs> StatusChanged;

        /// <summary>
        /// Script Result
        /// </summary>
        private ScriptResult _result;
        /// <summary>
        /// Gets the Script Result
        /// </summary>
        /// <value>The result.</value>
        public ScriptResult Result
        {
            get { return _result; }
        }


        /// <summary>
        /// Sets the result.
        /// </summary>
        /// <param name="scriptResult">The script result.</param>
        public void SetResult(ScriptResult scriptResult)
        {
            if (this._result != scriptResult)
            {
                _result = scriptResult;
            }
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return this.ScriptName;
        }
    }

    /// <summary>
    /// Script Result enumeration
    /// </summary>
    public enum ScriptResult
    {
        /// <summary>
        /// None / Initialized / Script not processed
        /// </summary>
        None,
        /// <summary>
        /// Script Succeeded
        /// </summary>
        Ok,
        /// <summary>
        /// Script Failed
        /// </summary>
        Fail
    }

    /// <summary>
    /// Event arguments fired with the <see cref="Script.StatusChanged"/> event.
    /// </summary>
    public class ScriptStatusChangedEventArgs
        : EventArgs
    {
        /// <summary>
        /// Old State
        /// </summary>
        public readonly ScriptStatus OldState;

        /// <summary>
        /// New State
        /// </summary>
        public readonly ScriptStatus NewState;

        /// <summary>
        /// Initializes a new instance of the <see cref="ScriptStatusChangedEventArgs"/> class.
        /// </summary>
        /// <param name="oldState">The old state.</param>
        /// <param name="newState">The new state.</param>
        public ScriptStatusChangedEventArgs(ScriptStatus oldState, ScriptStatus newState)
            : base()
        {
            this.OldState = oldState;
            this.NewState = newState;
        }
    }


    /// <summary>
    /// Late Bound Script (uses only late bound variables)
    /// </summary>
    /// <remarks>Take take not to use Referenced Assemblies within derived classes.
    /// </remarks>
    public abstract class ScriptLateBound
        : Script
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ScriptLateBound"/> class.
        /// </summary>
        protected ScriptLateBound()
            : base()
        {
        }

        /// <summary>
        /// DTE Object (Late Bound)
        /// </summary>
        protected dynamic dte = null;

        /// <summary>
        /// Solution object (Late Bound)
        /// </summary>
        protected dynamic solution = null;

        /// <summary>
        /// Gets the binding type of the script
        /// </summary>
        /// <value>The programming language.</value>
        public override string Binding
        {
            get { return "LateBound"; }
        }

        /// <summary>
        /// Handler function Before Initializing the Script (Configuration preparations)
        /// </summary>
        /// <param name="context">The Script Context</param>
        /// <remarks>Usually used to to the open a prepared or new XAE configuration</remarks>
        protected override void OnBeforeInitialize(IContext context)
        {
            this.dte = context.DTE;
            this.solution = context.Solution;
        }

        /// <summary>
        /// Handler function Initializing the Script (Configuration preparations)
        /// </summary>
        /// <param name="context">The Script Context</param>
        /// <remarks>Usually used to to the open a prepared or new XAE configuration</remarks>
        protected override void OnInitialize(/*dynamic dte, dynamic solution, IWorker worker*/IContext context)
        {
        }

        /// <summary>
        /// Creates an empty Visual Studio Solution
        /// </summary>
        protected override void CreateSolution()
        {
            DeleteSolutionFolder();
            this.solution.Create(this.ScriptRootFolder, this.ScriptName);
        }

        /// <summary>
        /// Creates the new project.
        /// </summary>
        /// <param name="projectName">Name of the project.</param>
        /// <returns></returns>
        protected override object CreateNewProject(string projectName)
        {
            return CreateNewProject(projectName, VsXaeTemplatePath);
        }

        /// <summary>
        /// Creates the new project.
        /// </summary>
        /// <param name="projectName">Name of the project.</param>
        /// <param name="templatePath">The template path.</param>
        /// <returns></returns>
        protected override object CreateNewProject(string projectName, string templatePath)
        {
            _context.Worker.ProgressStatus = string.Format("Creating project {0} from template ...",projectName, templatePath);
            //bool exists = File.Exists(VsXaeTemplatePath);

            //dynamic project = this.solution.AddFromTemplate(templatePath, this.ScriptRootFolder, projectName, false);
            //return project;
            //return CreateNewProject(projectName, templatePath, this.ScriptRootFolder);
            return this.Context.Factory.VsFactory.CreateNewProject(this.Context.DTE,projectName, templatePath, this.ScriptRootFolder);
        }

        /// <summary>
        /// Cleaning up the XAE configuration after script execution.
        /// </summary>
        /// <param name="worker">The worker.</param>
        protected override void OnCleanUp(IWorker worker)
        {
            foreach (Project project in this.solution.Projects)
            {
                project.Save();
            }
            this.dte = null;
            this.solution = null;
        }
    }

    /// <summary>
    /// Early Bound Script
    /// </summary>
    /// <remarks>The base class for the Script type uses the typed versions of dte and solution objects directly referencing the VisualStudio TypeLibrary.
    /// References to TwinCAT XAE Connectivity classes like ITcSysManager ITcTreeItem are allowed to use here.</remarks>
    public abstract class ScriptEarlyBound
        : Script
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ScriptEarlyBound"/> class.
        /// </summary>
        protected ScriptEarlyBound()
            : base()
        {
        }

        /// <summary>
        /// Early Bound DTE Object
        /// </summary>
        protected DTE2 dte = null;

        /// <summary>
        /// Early Bound Solution Object
        /// </summary>
        protected Solution4 solution = null;

        /// <summary>
        /// Handler function Before Initializing the Script (Configuration preparations)
        /// </summary>
        /// <param name="context">The Script Context</param>
        /// <remarks>Usually used to to the open a prepared or new XAE configuration</remarks>
        protected override void OnBeforeInitialize(IContext context)
        {
            this.dte = (DTE2)context.DTE;
            this.solution = (Solution4)context.Solution;
        }

        /// <summary>
        /// Handler function Initializing the Script (Configuration preparations)
        /// </summary>
        /// <param name="context">The Script Context</param>
        /// <remarks>Usually used to to the open a prepared or new XAE configuration</remarks>
        protected override void OnInitialize(/*dynamic dte, dynamic solution, IWorker worker*/IContext context)
        {
        }

        /// <summary>
        /// Cleaning up the XAE configuration after script execution.
        /// </summary>
        /// <param name="worker">The worker.</param>
        /// <remarks>Saves all open projects and Frees the internal DTE and Solution references</remarks>
        protected override void OnCleanUp(IWorker worker)
        {
            // TODO: If dynamic is used, Measurement projects throw ArgumentException after project.Save()
            foreach (Project project in this.solution.Projects)
            {
                project.Save();
            }

            this.dte = null;
            this.solution = null;
        }

        /// <summary>
        /// Gets the binding type of the script
        /// </summary>
        /// <value>The programming language.</value>
        public override string Binding
        {
            get { return "EarlyBound"; }
        }

        /// <summary>
        /// Creates an empty Visual Studio Solution
        /// </summary>
        protected override void CreateSolution()
        {
            DeleteSolutionFolder();

            this.solution.Create(this.ScriptRootFolder, this.ScriptName);

            // Because the Solution Object doesn't show its Solution.FullPath property before calling save,
            // We have to call SaveAs one time.
            string solutionPath = Path.Combine(this.ScriptRootFolder, string.Format("{0}.sln", this.ScriptName));
            Directory.CreateDirectory(this.ScriptRootFolder);
            this.solution.SaveAs(solutionPath);
        }

        /// <summary>
        /// Creates the new project.
        /// </summary>
        /// <param name="projectName">Name of the project.</param>
        /// <returns></returns>
        protected override object CreateNewProject(string projectName)
        {
            return CreateNewProject(projectName, VsXaeTemplatePath);
        }

        /// <summary>
        /// Creates the new project.
        /// </summary>
        /// <param name="projectName">Name of the project.</param>
        /// <param name="templatePath">The template path.</param>
        /// <returns></returns>
        protected override object CreateNewProject(string projectName, string templatePath)
        {
            dynamic project = null;

            // Determine installed TwinCAT version (major, minor and build)
            // Scripts have version requirements, these will be checked in the following
            FileVersionInfo sysSvcInfo = FileVersionInfo.GetVersionInfo(TwinCATInstallDir + @"System\TCATSysSrv.exe");

            Version sysSvcVersion = new Version(sysSvcInfo.ProductMajorPart, sysSvcInfo.ProductMinorPart);

            if (sysSvcVersion >= this.TwinCATVersion)
            {
                _context.Worker.ProgressStatus = string.Format("Creating project {0} from template ...", projectName, templatePath);
                bool exists = File.Exists(VsXaeTemplatePath);
                project = this.solution.AddFromTemplate(templatePath, this.ScriptRootFolder, projectName, false);
            }
            else
            {
                throw new Exception("System requirements not met! You need a higher TwinCAT Build to execute this script!");
            }

            return project;
        }

        //public override void OpenSolution()
        //{
        //    string sln = Path.Combine(this.ScriptRootFolder, this.ScriptName);
        //    this.solution.Open(sln);
        //}
    }

    /// <summary>
    /// Class implements additional OS Environment settings
    /// </summary>
    public static class TcEnvironment
    {
        /// <summary>
        /// Gets a value indicating whether this code is running on a 64 Bit Operating system.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [is64 bit operating system]; otherwise, <c>false</c>.
        /// </value>
        public static bool Is64BitOperatingSystem
        {

            get
            {
                return Environment.Is64BitOperatingSystem;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this code is running within a Wow64 Process (32-Bit Processon 64-Bit Operating system)
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is wow64 process; otherwise, <c>false</c>.
        /// </value>
        public static bool IsWow64
        {
            get
            {
                return Environment.Is64BitOperatingSystem && !Environment.Is64BitProcess;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this code is running in a native 64 Bit Process.
        /// </summary>
        /// <value><c>true</c> if [is64 bit process]; otherwise, <c>false</c>.</value>
        public static bool Is64BitProcess
        {
            get
            {
                return Environment.Is64BitProcess;
            }
        }
    }
}

