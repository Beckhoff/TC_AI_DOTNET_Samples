using System;
using System.IO;
using EnvDTE;
using EnvDTE100;
using EnvDTE80;
using TCatSysManagerLib;
using TwinCAT.SystemManager;
using System.Diagnostics;
using System.Timers;
using ScriptingTest;
using System.Xml;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Scripting.CSharp
{
    /// <summary>
    /// Demonstrates the generation + compilation of PLC projects
    /// </summary>
    public class AccessRunningVS
        : ScriptEarlyBound
    {
        ITcSysManager4 systemManager = null;
        Project project = null;

        /// <summary>
        /// Handler function Initializing the Script (Configuration preparations)
        /// </summary>
        /// <param name="context"></param>
        /// <remarks>Usually used to to the open a prepared or new XAE configuration</remarks>
        protected override void OnInitialize(IContext context)
        {
            base.OnInitialize(context);
        }

        /// <summary>
        /// Handler function called after the Solution object has been created.
        /// </summary>
        protected override void OnSolutionCreated()
        {
            this.project = (Project)CreateNewProject();
            this.systemManager = (ITcSysManager4)project.Object;
            base.OnSolutionCreated();
        }

        /// <summary>
        /// Cleaning up the XAE configuration after script execution.
        /// </summary>
        /// <param name="worker">The worker.</param>
        protected override void OnCleanUp(IWorker worker)
        {
            base.OnCleanUp(worker);
        }


        /// <summary>
        /// Handler function Executing the Script code.
        /// </summary>
        /// <param name="worker">The worker.</param>
        protected override void OnExecute(IWorker worker)
        {   
            worker.Progress = 0;
            if (worker.CancellationPending)
                throw new Exception("Cancelled");

            //The Solution must be saved one time, otherwise the SolutionPath is not stored properly
            //within the DTE Object.
            string fullName1 = this.dte.Solution.FullName;
            
            //Access COM Running Objects Table
            Dictionary<string,List<object>> rot = ROTAccess.GetRunningObjectTable();
            Dictionary<ROTDteInfo,DTE> dteTable = ROTAccess.GetRunningDTETable();

            DTE dte = ROTAccess.GetActiveDTE(this.ScriptName); // Getting DTE of the currently Script-Opened project

            Debug.Assert(dte == (DTE)this._context.DTE); // Should be the same Object!
        }

        /// <summary>
        /// Gets the Script description
        /// </summary>
        /// <value>The description.</value>
        public override string Description
        {
            get { return "Get Active DTE Object."; }
        }

        /// <summary>
        /// Gets the detailed description of the <see cref="Script"/> that is shown in the Method Tips.
        /// </summary>
        /// <value>The detailed description.</value>
        public override string DetailedDescription
        {
            get
            {
                return "Accessing Running Objects Table and determining already started VisualStudio instances."; 
            }
        }

        /// <summary>
        /// Gets the keywords, describing the Script features
        /// </summary>
        /// <value>The keywords.</value>
        public override string Keywords
        {
            get
            {
                return "RunningObjectsTable, ROT, GetActive DTE-Object";
            }
        }

        /// <summary>
        /// Gets the Version number of TwinCAT that is necessary for script execution.
        /// </summary>
        /// <value>The TwinCAT version.</value>
        public override Version TwinCATVersion
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
        public override string TwinCATBuild
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
        public override string Category
        {
            get
            {
                return "Basics";
            }
        }
    }
}
