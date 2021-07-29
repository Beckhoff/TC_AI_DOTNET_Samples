using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TCatSysManagerLib;

namespace Tutorial_07_AddNewPlcProject
{
    class Program
    {
        const string TUT07_TCTEMPLATEPATH = @"C:\TwinCAT\3.1\Components\Base\PrjTemplate\TwinCAT Project.tsproj";
        const string TUT07_BASEFOLDER = @"C:\AI_Training\TUT07";
        const string TUT07_SOLUTIONFOLDER = @"\TUT07_Solution";
        const string TUT07_NAME = "NewProject";
        const string TUT07_SLNNAME = @"\NewProject.sln";
        const string TUT07_PROGID = "VisualStudio.DTE.12.0";

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
             * Navigate to PLC node
             * ------------------------------------------------------ */
            ITcSmTreeItem plc = _systemManager.LookupTreeItem("TIPC");

            /* ------------------------------------------------------
             * Create new PLC Project, according to "Standard PLC Project" template (Default TwinCAT)
             * ------------------------------------------------------ */
            ITcSmTreeItem newProject = plc.CreateChild("NewPlcProject", 0, "", "Standard PLC Template");

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
            Type t = System.Type.GetTypeFromProgID(TUT07_PROGID);
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
            DirectoryHelper.DeleteDirectory(TUT07_BASEFOLDER);
            Directory.CreateDirectory(TUT07_BASEFOLDER);
            Directory.CreateDirectory(TUT07_BASEFOLDER + TUT07_SOLUTIONFOLDER);

            /* ------------------------------------------------------
             * Create and save new solution
             * ------------------------------------------------------ */
            _solution = _dte.Solution;
            _solution.Create(TUT07_BASEFOLDER, TUT07_SOLUTIONFOLDER);
            _solution.SaveAs(TUT07_BASEFOLDER + TUT07_SOLUTIONFOLDER + TUT07_SLNNAME);
        }

        public static void createTcProject()
        {
            /* ------------------------------------------------------
             * Create new TwinCAT project, based on TwinCAT Project file (delivered with TwinCAT XAE)
             * ------------------------------------------------------ */
            _project = _solution.AddFromTemplate(TUT07_TCTEMPLATEPATH, TUT07_BASEFOLDER + TUT07_SOLUTIONFOLDER, TUT07_NAME);

            /* ------------------------------------------------------
             * Cast TwinCAT project object to ITcSysManager interface --> Automation Interface starts here
             * ------------------------------------------------------ */
            _systemManager = (ITcSysManager)_project.Object;
        }
    }
}
