using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TCatSysManagerLib;

namespace Tutorial_13_AddTcComObjects
{
    /* ======================================================
     * PLEASE NOTE: This tutorial requires that the ZIP file
     * from the Templates folder is being extracted to 
     * "C:\TwinCAT\3.1\Config\Modules\" BEFORE script execution.
     * ====================================================== */
    class Program
    {
        const string TUT13_TCTEMPLATEPATH = @"C:\TwinCAT\3.1\Components\Base\PrjTemplate\TwinCAT Project.tsproj";
        const string TUT13_BASEFOLDER = @"C:\AI_Training\TUT13";
        const string TUT13_SOLUTIONFOLDER = @"\TUT13_Solution";
        const string TUT13_NAME = "NewProject";
        const string TUT13_SLNNAME = @"\NewProject.sln";
        const string TUT13_PROGID = "VisualStudio.DTE.12.0";

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

            /* ======================================================
             * Place tutorial code below
             * ====================================================== */

            /* ------------------------------------------------------
             * Navigate to TcCOM Objects folder
             * ------------------------------------------------------ */
            ITcSmTreeItem tcom = _systemManager.LookupTreeItem("TIRC^TcCOM Objects");

            /* ------------------------------------------------------
             * GUIDs of TcCOM objects - this is how a TcCOM Object is being uniquely identified
             * ------------------------------------------------------ */            
            string guidTempController = "{8f5fdcff-ee4b-4ee5-80b1-25eb23bd1b45}";
            string guidContrSys = "{acd3c8a0-d974-4eb0-91e6-a4a0eb4db128}";

            /* ------------------------------------------------------
             * Add TcCOM modules
             * ------------------------------------------------------ */
            ITcSmTreeItem newTcCom1 = tcom.CreateChild("TemperatureControllerModule", 0, "", guidTempController);
            newTcCom1.Name = "TemperatureControllerModule";
            ITcSmTreeItem newTcCom2 = tcom.CreateChild("ControllerSystemModule", 0, "", guidContrSys);
            newTcCom2.Name = "ControllerSystemModule";

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
            Type t = System.Type.GetTypeFromProgID(TUT13_PROGID);
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
            DirectoryHelper.DeleteDirectory(TUT13_BASEFOLDER);
            Directory.CreateDirectory(TUT13_BASEFOLDER);
            Directory.CreateDirectory(TUT13_BASEFOLDER + TUT13_SOLUTIONFOLDER);

            /* ------------------------------------------------------
             * Create and save new solution
             * ------------------------------------------------------ */
            _solution = _dte.Solution;
            _solution.Create(TUT13_BASEFOLDER, TUT13_SOLUTIONFOLDER);
            _solution.SaveAs(TUT13_BASEFOLDER + TUT13_SOLUTIONFOLDER + TUT13_SLNNAME);
        }

        public static void createTcProject()
        {
            /* ------------------------------------------------------
             * Create new TwinCAT project, based on TwinCAT Project file (delivered with TwinCAT XAE)
             * ------------------------------------------------------ */
            _project = _solution.AddFromTemplate(TUT13_TCTEMPLATEPATH, TUT13_BASEFOLDER + TUT13_SOLUTIONFOLDER, TUT13_NAME);

            /* ------------------------------------------------------
             * Cast TwinCAT project object to ITcSysManager interface --> Automation Interface starts here
             * ------------------------------------------------------ */
            _systemManager = (ITcSysManager)_project.Object;
        }
    }
}
