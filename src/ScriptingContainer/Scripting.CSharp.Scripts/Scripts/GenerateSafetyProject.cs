//using System;
//using System.IO;
//using EnvDTE;
//using EnvDTE100;
//using EnvDTE80;
//using TCatSysManagerLib;
//using TwinCAT.SystemManager;
//using System.Diagnostics;
//using System.Timers;
//using ScriptingTest;
//using System.Xml;
//using System.Collections.Generic;

//namespace Scripting.CSharp
//{
//    /// <summary>
//    /// Demonstrates the generation + compilation of PLC projects
//    /// </summary>
//    public class GenerateSafetyProject
//        : ScriptEarlyBound
//    {
//        ITcSysManager4 systemManager = null;
//        Project project = null;

//        /// <summary>
//        /// Handler function Initializing the Script (Configuration preparations)
//        /// </summary>
//        /// <param name="context"></param>
//        /// <remarks>Usually used to to the open a prepared or new XAE configuration</remarks>
//        protected override void OnInitialize(IContext context)
//        {
//            base.OnInitialize(context);
//            //CreateSolution();

//            //context.Worker.ProgressStatus = "Creating project ...";

//            //bool exists = File.Exists(VsXaeTemplatePath);

//            //this.project = this.solution.AddFromTemplate(VsXaeTemplatePath, this.ScriptRootFolder, this.ScriptName, false);
//            //this.systemManager = (ITcSysManager4)project.Object;
//        }

//        /// <summary>
//        /// Handler function called after the Solution object has been created.
//        /// </summary>
//        protected override void OnSolutionCreated()
//        {
//            this.project = (Project)CreateNewProject();
//            this.systemManager = (ITcSysManager4)project.Object;
//            base.OnSolutionCreated();
//        }

//        /// <summary>
//        /// Cleaning up the XAE configuration after script execution.
//        /// </summary>
//        /// <param name="worker">The worker.</param>
//        protected override void OnCleanUp(IWorker worker)
//        {
//            base.OnCleanUp(worker);

//            //Type tp = Type.GetTypeFromProgID(this.Context.Factory.AppID);
//            //dynamic dte2 = System.Activator.CreateInstance(tp, true);

//            //dte2.MainWindow.Visible = true;
//            //dte2.SuppressUI = true;
//            //dte2.UserControl = true;

//            //// Updating the Settings (there is a dependency between them)
//            //string sln = Path.Combine(this.ScriptRootFolder, this.ScriptName+".sln");
//            //dte2.Solution.Open(sln);
//        }


//        /// <summary>
//        /// Insertion Mode for creating PLC projects.
//        /// </summary>
//        public enum CreatePlcMode
//        {
//            /// <summary>
//            /// Copies a PLC Project
//            /// </summary>
//            Copy = 0,
//            /// <summary>
//            /// Moves a PLC Project
//            /// </summary>
//            Move = 1,
//            /// <summary>
//            /// References a PLC Project
//            /// </summary>
//            Reference = 2
//        }
//        /// <summary>
//        /// Handler function Executing the Script code.
//        /// </summary>
//        /// <param name="worker">The worker.</param>
//        protected override void OnExecute(IWorker worker)
//        {   
//            worker.Progress = 0;
//            if (worker.CancellationPending)
//                throw new Exception("Cancelled");

//            ITcSmTreeItem safConfig = systemManager.LookupTreeItem("TICP"); // Getting Safety Configuration item
//            ITcSmTreeItem devices = systemManager.LookupTreeItem("TIID"); // Getting IO-Configuration item

//            // Generate a PLC Project
//            worker.ProgressStatus = "Creating empty Safety Project ...";
//            ITcSmTreeItem safGenerated = safConfig.CreateChild("SafetyGenerated", 0, "", vsXaeSafePlcWizard);

//            worker.ProgressStatus = "Safety Project created ...";
//            worker.Progress = 10;

//            ITcSmTreeItem safProjectRootItem = systemManager.LookupTreeItem("TICP^SafetyGenerated"); // Plc Project Root (XAE Base side)
//            ITcSmTreeItem safProjectItem = systemManager.LookupTreeItem("TICP^SafetyGenerated^SafetyGenerated Project"); // PlcProject (PlcControl side)
//            ITcSmTreeItem safInstancesItem = systemManager.LookupTreeItem("TICP^SafetyGenerated^SafetyGenerated Instance"); // Instances

//            ITcSmTreeItem safGroup = systemManager.LookupTreeItem("TICP^SafetyGenerated^SafetyGenerated Project^TwinSafeGroup1");
//            ITcSmTreeItem safSal = systemManager.LookupTreeItem("TICP^SafetyGenerated^SafetyGenerated Project^TwinSafeGroup1^TwinSafeGroup1.sal");

//            //TODO: Show Consume Features !!!
//            browse(safProjectRootItem, worker);

//            // Navigate through subtrees

//            worker.Progress = 100;
//        }

//        private void browse(ITcSmTreeItem root,IWorker worker)
//        {
//            string name = root.Name;
//            string pathName = root.PathName;
//            TreeItemType itemType = (TreeItemType)root.ItemType;
//            string subTypeName = root.ItemSubTypeName;

//            worker.ProgressStatus = string.Format("Browsing node '{0}'", pathName);

//            string xml = root.ProduceXml();
            
//            // Iteration over each node and produce output for demo purposes
//            foreach (ITcSmTreeItem3 child in root)
//            {
//                browse(child, worker);
//            }
//        }

//        private void browse2(ITcSmTreeItem root, IWorker worker)
//        {
//            string name = root.Name;
//            string pathName = root.PathName;

//            worker.ProgressStatus = string.Format("Browsing node '{0}'", pathName);

//            string xml = root.ProduceXml();

//            //foreach (ITcSmTreeItem4 child in root)
//            //{
//            ////    browse2(child, worker);
//            //}
//        }



//        /// <summary>
//        /// Gets the Script description
//        /// </summary>
//        /// <value>The description.</value>
//        public override string Description
//        {
//            get { return "Demonstrates the creation of TwinCAT Safety Projects!"; }
//        }

//        /// <summary>
//        /// Gets the detailed description of the <see cref="Script"/> that is shown in the Method Tips.
//        /// </summary>
//        /// <value>The detailed description.</value>
//        public override string DetailedDescription
//        {
//            get
//            {
//                string test = @"Creation of a new TwinCAT Safety project from Scratch. DETAILS TO FILL OUT HERE!!!.";
//                return test;
//            }
//        }

//        /// <summary>
//        /// Gets the keywords, describing the Script features
//        /// </summary>
//        /// <value>The keywords.</value>
//        public override string Keywords
//        {
//            get
//            {
//                return "Create Safety project, Project management";
//            }
//        }

//        /// <summary>
//        /// Gets the Version number of TwinCAT that is necessary for script execution.
//        /// </summary>
//        /// <value>The TwinCAT version.</value>
//        public override Version TwinCATVersion
//        {
//            get
//            {
//                return new Version(3, 1);
//            }
//        }

        ///// <summary>
        ///// Gets the build number of TwinCAT that is necessary for script execution.
        ///// </summary>
        ///// <value>The TwinCAT build.</value>
        //public override string TwinCATBuild
        //{
        //    get
        //    {
        //        return "4013";
        //    }
        //}

        ///// <summary>
        ///// Gets the category of this script.
        ///// </summary>
        ///// <value>The script category.</value>
        //public override string Category
        //{
        //    get
        //    {
        //        return "Safety";
        //    }
        //}

//    }
//}
