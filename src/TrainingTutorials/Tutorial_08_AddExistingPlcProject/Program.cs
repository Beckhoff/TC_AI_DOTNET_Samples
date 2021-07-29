using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TCatSysManagerLib;

namespace Tutorial_08_AddExistingPlcProject
{
    class Program
    {
        const string TUT08_TCTEMPLATEPATH = @"C:\TwinCAT\3.1\Components\Base\PrjTemplate\TwinCAT Project.tsproj";
        const string TUT08_BASEFOLDER = @"C:\AI_Training\TUT08";
        const string TUT08_SOLUTIONFOLDER = @"\TUT08_Solution";
        const string TUT08_NAME = "NewProject";
        const string TUT08_SLNNAME = @"\NewProject.sln";
        const string TUT08_PROGID = "VisualStudio.DTE.12.0";

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
             * Navigate to PLC node
             * ------------------------------------------------------ */
            ITcSmTreeItem plc = _systemManager.LookupTreeItem("TIPC");

            /* ------------------------------------------------------
             * Attach existing PLC Project, using its tpzip file
             * ------------------------------------------------------ */
            ITcSmTreeItem existingProject = plc.CreateChild("ExistingPlcProject", 0, "", templateDir + "TUT08_PlcProject.tpzip");

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
            Type t = System.Type.GetTypeFromProgID(TUT08_PROGID);
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
            DirectoryHelper.DeleteDirectory(TUT08_BASEFOLDER);
            Directory.CreateDirectory(TUT08_BASEFOLDER);
            Directory.CreateDirectory(TUT08_BASEFOLDER + TUT08_SOLUTIONFOLDER);

            /* ------------------------------------------------------
             * Create and save new solution
             * ------------------------------------------------------ */
            _solution = _dte.Solution;
            _solution.Create(TUT08_BASEFOLDER, TUT08_SOLUTIONFOLDER);
            _solution.SaveAs(TUT08_BASEFOLDER + TUT08_SOLUTIONFOLDER + TUT08_SLNNAME);
        }

        public static void createTcProject()
        {
            /* ------------------------------------------------------
             * Create new TwinCAT project, based on TwinCAT Project file (delivered with TwinCAT XAE)
             * ------------------------------------------------------ */
            _project = _solution.AddFromTemplate(TUT08_TCTEMPLATEPATH, TUT08_BASEFOLDER + TUT08_SOLUTIONFOLDER, TUT08_NAME);

            /* ------------------------------------------------------
             * Cast TwinCAT project object to ITcSysManager interface --> Automation Interface starts here
             * ------------------------------------------------------ */
            _systemManager = (ITcSysManager)_project.Object;
        }
    }
}
