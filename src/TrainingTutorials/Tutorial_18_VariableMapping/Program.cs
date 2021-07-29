using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TCatSysManagerLib;

namespace Tutorial_18_VariableMapping
{
    class Program
    {
        const string TUT18_TCTEMPLATEPATH = @"C:\TwinCAT\3.1\Components\Base\PrjTemplate\TwinCAT Project.tsproj";
        const string TUT18_BASEFOLDER = @"C:\AI_Training\TUT18";
        const string TUT18_SOLUTIONFOLDER = @"\TUT18_Solution";
        const string TUT18_NAME = "NewProject";
        const string TUT18_SLNNAME = @"\NewProject.sln";
        const string TUT18_PROGID = "VisualStudio.DTE.12.0";

        static EnvDTE.DTE _dte;
        static EnvDTE.Solution _solution;
        static EnvDTE.Project _project;

        static ITcSysManager _systemManager;

        [STAThread]
        static void Main(string[] args)
        {
            MessageFilter.Register();

            createVsInstance(false, true, true);
            createVsSolution();
            createTcProject();

            string templateDir = Directory.GetCurrentDirectory() + @"\Templates\";

            /* ======================================================
             * Place tutorial code below
             * ====================================================== */

            /* ------------------------------------------------------
             * Navigate to TcCOM, PLC and I/O node
             * ------------------------------------------------------ */
            ITcSmTreeItem tcom = _systemManager.LookupTreeItem("TIRC^TcCOM Objects");
            ITcSmTreeItem plc = _systemManager.LookupTreeItem("TIPC");
            ITcSmTreeItem io = _systemManager.LookupTreeItem("TIID");
            
            /* ------------------------------------------------------
             * Attach existing PLC Project, using its tpzip file
             * ------------------------------------------------------ */
            ITcSmTreeItem existingProject = plc.CreateChild("ExistingPlcProject", 0, "", templateDir + "TUT18_PlcProject.tpzip");

            /* ------------------------------------------------------
             * Compile PLC project to generate module data
             * ------------------------------------------------------ */
            _dte.Solution.SolutionBuild.Build(true);

            /* ------------------------------------------------------
             * Add TcCOM Object
             * ------------------------------------------------------ */
            string guidTempController = "{8f5fdcff-ee4b-4ee5-80b1-25eb23bd1b45}";
            ITcSmTreeItem newTcCom1 = tcom.CreateChild("TemperatureControllerModule", 0, "", guidTempController);
            newTcCom1.Name = "TemperatureControllerModule";

            /* ------------------------------------------------------
             * Attach I/O configuration, based on xti-file (template)
             * Template file contains EtherCAT Master, EK1100, EL1004 and EL2004
             * ------------------------------------------------------ */
            ITcSmTreeItem iodevices = io.ImportChild(templateDir + "TUT18_IOdevices.xti", "", true);

            /* ------------------------------------------------------
             * Create variable mapping between I/Os and PLC inputs/outputs
             * ------------------------------------------------------ */
            string plcInputsPath = "TIPC^ExistingPlcProject^ExistingPlcProject Instance^PlcTask Inputs";
            string plcOutputsPath = "TIPC^ExistingPlcProject^ExistingPlcProject Instance^PlcTask Outputs";
            string ioInputsPath = "TIID^Device 1 (EtherCAT)^Term 1 (EK1100)^Term 2 (EL1004)";
            string ioOutputsPath = "TIID^Device 1 (EtherCAT)^Term 1 (EK1100)^Term 3 (EL2004)";

            _systemManager.LinkVariables(plcInputsPath + "^GVL.bInput1", ioInputsPath + "^Channel 1^Input");
            _systemManager.LinkVariables(plcInputsPath + "^GVL.bInput2", ioInputsPath + "^Channel 2^Input");
            _systemManager.LinkVariables(plcInputsPath + "^GVL.bInput3", ioInputsPath + "^Channel 3^Input");
            _systemManager.LinkVariables(plcInputsPath + "^GVL.bInput4", ioInputsPath + "^Channel 4^Input");

            _systemManager.LinkVariables(plcOutputsPath + "^GVL.bOutput1", ioOutputsPath + "^Channel 1^Output");
            _systemManager.LinkVariables(plcOutputsPath + "^GVL.bOutput2", ioOutputsPath + "^Channel 2^Output");
            _systemManager.LinkVariables(plcOutputsPath + "^GVL.bOutput3", ioOutputsPath + "^Channel 3^Output");
            _systemManager.LinkVariables(plcOutputsPath + "^GVL.bOutput4", ioOutputsPath + "^Channel 4^Output");

            /* ------------------------------------------------------
             * Create variable mapping between PLC inputs/outputs and TcCOM object
             * ------------------------------------------------------ */
            string tcComInputsPath = "TIRC^TcCOM Objects^TemperatureControllerModule^Input";
            string tcComOutputsPath = "TIRC^TcCOM Objects^TemperatureControllerModule^Output";

            _systemManager.LinkVariables(plcInputsPath + "^GVL.bInputTcCom1", tcComOutputsPath + "^HeaterOn");
            _systemManager.LinkVariables(plcInputsPath + "^GVL.bInputTcCom2", tcComOutputsPath + "^CoolerOn");

            _systemManager.LinkVariables(plcOutputsPath + "^GVL.nOutputTcCom1", tcComInputsPath + "^ExternalSetpoint");
            _systemManager.LinkVariables(plcOutputsPath + "^GVL.nOutputTcCom2", tcComInputsPath + "^FeedbackTemp");

            /* ------------------------------------------------------
             * Save mapping information
             * ------------------------------------------------------ */
            ITcSysManager3 systemManager2 = (ITcSysManager3)_systemManager;
            string mappingInfo = systemManager2.ProduceMappingInfo();

            /* ------------------------------------------------------
             * Clear mapping information
             * ------------------------------------------------------ */
            systemManager2.ClearMappingInfo();

            /* ------------------------------------------------------
             * Restore previously saved mapping information
             * ------------------------------------------------------ */
            systemManager2.ConsumeMappingInfo(mappingInfo);

            /* ======================================================
             * Place tutorial code above
             * ====================================================== */

            MessageFilter.Revoke();
        }

        public static void createVsInstance(bool suppressUI, bool userControl, bool visible)
        {
            /* ------------------------------------------------------
             * Create Visual Studio instance and make VS window visible
             * ------------------------------------------------------ */
            Type t = System.Type.GetTypeFromProgID(TUT18_PROGID);
            _dte = (EnvDTE.DTE)System.Activator.CreateInstance(t);
            _dte.SuppressUI = suppressUI;
            _dte.UserControl = userControl; // true = leaves VS window open after code execution
            _dte.MainWindow.Visible = visible;
        }

        public static void createVsSolution()
        {
            /* ------------------------------------------------------
             * Create directories for new Visual Studio solution
             * ------------------------------------------------------ */
            DirectoryHelper.DeleteDirectory(TUT18_BASEFOLDER);
            Directory.CreateDirectory(TUT18_BASEFOLDER);
            Directory.CreateDirectory(TUT18_BASEFOLDER + TUT18_SOLUTIONFOLDER);

            /* ------------------------------------------------------
             * Create and save new solution
             * ------------------------------------------------------ */
            _solution = _dte.Solution;
            _solution.Create(TUT18_BASEFOLDER, TUT18_SOLUTIONFOLDER);
            _solution.SaveAs(TUT18_BASEFOLDER + TUT18_SOLUTIONFOLDER + TUT18_SLNNAME);
        }

        public static void createTcProject()
        {
            /* ------------------------------------------------------
             * Create new TwinCAT project, based on TwinCAT Project file (delivered with TwinCAT XAE)
             * ------------------------------------------------------ */
            _project = _solution.AddFromTemplate(TUT18_TCTEMPLATEPATH, TUT18_BASEFOLDER + TUT18_SOLUTIONFOLDER, TUT18_NAME);

            /* ------------------------------------------------------
             * Cast TwinCAT project object to ITcSysManager interface --> Automation Interface starts here
             * ------------------------------------------------------ */
            _systemManager = (ITcSysManager)_project.Object;
        }
    }
}
