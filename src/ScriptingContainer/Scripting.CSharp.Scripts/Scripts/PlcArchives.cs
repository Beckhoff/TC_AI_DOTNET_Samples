using System;
using System.IO;
using EnvDTE;
using EnvDTE100;
using EnvDTE80;
using TCatSysManagerLib;
using TwinCAT.SystemManager;
using ScriptingTest;

namespace Scripting.CSharp
{
    /// <summary>
    /// Demonstrates the Import and Export of PLC Archives
    /// </summary>
    public class PlcArchives
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
        /// Insertion Mode for creating PLC projects.
        /// </summary>
        public enum CreatePlcMode
        {
            /// <summary>
            /// Copies a PLC Project
            /// </summary>
            Copy = 0,
            /// <summary>
            /// Moves a PLC Project
            /// </summary>
            Move = 1,
            /// <summary>
            /// References a PLC Project
            /// </summary>
            Reference = 2
        }
        /// <summary>
        /// Handler function Executing the Script code.
        /// </summary>
        /// <param name="worker">The worker.</param>
        protected override void OnExecute(IWorker worker)
        {
            worker.Progress = 0;
            if (worker.CancellationPending)
                throw new Exception("Cancel");

            ITcSmTreeItem plcConfig = systemManager.LookupTreeItem("TIPC"); // Getting NC Configuration

            // Generate a PLC Project
            worker.ProgressStatus = "Creating empty PLC Project ...";
            ITcSmTreeItem plcGenerated = plcConfig.CreateChild("PlcGenerated", 0, "", vsXaePlcStandardTemplateName);

            worker.ProgressStatus = "PLC Project created ...";
            worker.Progress = 25;

            ITcSmTreeItem plcProjectRootItem = systemManager.LookupTreeItem("TIPC^PlcGenerated"); // Plc Project Root (XAE Base side)
            ITcSmTreeItem plcProjectItem = systemManager.LookupTreeItem("TIPC^PlcGenerated^PlcGenerated Project"); // PlcProject (PlcControl side)
            ITcSmTreeItem plcInstancesItem = systemManager.LookupTreeItem("TIPC^PlcGenerated^PlcGenerated Instance"); // Instances
            ITcSmTreeItem pouRootItem = systemManager.LookupTreeItem("TIPC^PlcGenerated^PlcGenerated Project^POUs");

            ITcSmTreeItem pousItem = CreatePlcFolder(plcProjectItem, "MyPous","", worker);

            //Generating Project
            worker.ProgressStatus = "Generating POUs ...";
            worker.Progress = 30;

            IECLanguageType iecLanguage = IECLanguageType.ST;

            ITcSmTreeItem pouProgram = pousItem.CreateChild("POUProgram", TreeItemType.PlcPouFunctionBlock.AsInt32(), "", iecLanguage.ToString());
            worker.ProgressStatus = "POU Program added ...";
            worker.Progress = 35;

            ITcSmTreeItem pouFunctionBlock = pousItem.CreateChild("POUFunctionBlock", TreeItemType.PlcPouFunctionBlock.AsInt32(), "", iecLanguage.ToString());
            worker.ProgressStatus = "POU FunctionBlock added ...";
            worker.Progress = 40;

            string[] data = new string[2];
            data[0] = iecLanguage.ToString();
            data[1] = "DINT";
            ITcSmTreeItem pouPouFunction = pousItem.CreateChild("POUFunction", TreeItemType.PlcPouFunction.AsInt32(), "", data);
            worker.ProgressStatus = "POU Function added ...";
            worker.Progress = 50;

            // Import / Export in PlcOpen Format
            string plcOpenExportFile = Path.Combine(base.ScriptRootFolder,"PlcOpenExport.xml");
            worker.ProgressStatus = "Exporting PlcOpen format ...";

            ITcPlcOpenImportExport importExport = plcProjectItem as ITcPlcOpenImportExport;
            importExport.PlcOpenExport(plcOpenExportFile, "MyPous.POUProgram;MyPous.POUFunction;MyPous.POUFunctionBlock"); // Selections seperated by ';', relative Paths seperated by '.', PlcOpenExport

            pousItem.DeleteChild("POUProgram");
            pousItem.DeleteChild("POUFunctionBlock");   
            pousItem.DeleteChild("POUFunction");

            worker.ProgressStatus = "Importing PlcOpen format ...";
            worker.Progress = 60;

            importExport.PlcOpenImport(plcOpenExportFile, (int)PLCIMPORTOPTIONS.PLCIMPORTOPTIONS_NONE); // PlcOpenImport
            //importExport.PlcOpenImport(plcOpenExportFile, (int)PLC_IMPORT_OPTIONS.PLCOVERWRITE_RENAME);

            ITcPlcProject iecProjectRoot = (ITcPlcProject)plcProjectRootItem;
            
            // Creating projects from Template Archive (Importing as *.tpzip)
            worker.ProgressStatus = "Importing PlcProject from archive ...";
            string plcProjectArchive = Path.Combine(this.ConfigurationTemplatesFolder, "PlcArchive1.tpzip");

            worker.ProgressStatus = "Creating PLC from Template Archive ...";
            ITcSmTreeItem plcArchived = plcConfig.CreateChild("ArchivedPlc", (int)CreatePlcMode.Copy, "", plcProjectArchive);
            worker.ProgressStatus = "Plc Project created ...";
            worker.Progress = 90;

            plcArchived.Name = "ToExport";

            // Exporting Plc Project as *.tpzip
            plcConfig.ExportChild("ToExport", base.ScriptRootFolder);

            plcProjectRootItem = systemManager.LookupTreeItem("TIPC^ToExport"); // Plc Project Root (XAE Base side)
            plcProjectItem = systemManager.LookupTreeItem("TIPC^ToExport^ToExport Project"); // PlcProject (PlcControl side)
            plcInstancesItem = systemManager.LookupTreeItem("TIPC^ToExport^ToExport Instance"); // Instances

            worker.Progress = 100;
        }

        private ITcSmTreeItem CreatePlcFolder(ITcSmTreeItem parent, string folderName, string before, IWorker worker)
        {
            worker.ProgressStatus = string.Format("Creating Folder '{0}' ...", folderName);
            ITcSmTreeItem item = parent.CreateChild(folderName, TreeItemType.PlcFolder.AsInt32(), before, null);
            return item;
        }

        /// <summary>
        /// Gets the Script description
        /// </summary>
        /// <value>The description.</value>
        public override string Description
        {
            get { return "Import and Exports PlcArchives (*.tpzip), Use of PlcOpen Import/Export interfaces.";}
        }

        /// <summary>
        /// Gets the detailed description of the <see cref="Script"/> that is shown in the Method Tips.
        /// </summary>
        /// <value>The detailed description.</value>
        public override string DetailedDescription
        {
            get
            {
                string test = @"Demonstrates the Import/Export of TwinCAT Plc Subprojects and the use of the PlcOpen Import/Export interfaces.";
                return test;
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
                return "Import/Export PLC Archives / Templates, PlcOpen Import Export";
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
                return "PLC";
            }
        }
    }
}
