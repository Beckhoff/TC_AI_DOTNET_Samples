using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TCatSysManagerLib;

namespace Tutorial_09_AddNewPou
{
    class Program
    {
        const string TUT09_TCTEMPLATEPATH = @"C:\TwinCAT\3.1\Components\Base\PrjTemplate\TwinCAT Project.tsproj";
        const string TUT09_BASEFOLDER = @"C:\AI_Training\TUT09";
        const string TUT09_SOLUTIONFOLDER = @"\TUT09_Solution";
        const string TUT09_NAME = "NewProject";
        const string TUT09_SLNNAME = @"\NewProject.sln";
        const string TUT09_PROGID = "VisualStudio.DTE.12.0";

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

            /* ------------------------------------------------------
             * Navigate to POUs folder
             * ------------------------------------------------------ */
            ITcSmTreeItem plcProjectPous = _systemManager.LookupTreeItem("TIPC^NewPlcProject^NewPlcProject Project^POUs");

            /* ------------------------------------------------------
             * Create new folder and then create some POUs below it
             * ------------------------------------------------------ */
            ITcSmTreeItem newFolder1 = plcProjectPous.CreateChild("Folder1", 601, "", null);

            ITcSmTreeItem newPou1 = newFolder1.CreateChild("FB_TEST1", 604, "", null);
            ITcSmTreeItem newPou2 = newFolder1.CreateChild("FB_TEST2", 604, "", null);
            ITcSmTreeItem newPou3 = newFolder1.CreateChild("FB_TEST3", 604, "", null);
            ITcSmTreeItem newPou4 = newFolder1.CreateChild("FB_TEST4", 604, "", null);
            ITcSmTreeItem newPou5 = newFolder1.CreateChild("FB_TEST5", 604, "", null);
            ITcSmTreeItem newPou6 = newFolder1.CreateChild("FB_TEST6", 604, "", null);
            ITcSmTreeItem newPou7 = newFolder1.CreateChild("FB_TEST7", 604, "", null);

            /* ------------------------------------------------------
             * Create new folder and then create some POUs with methods below it
             * ------------------------------------------------------ */
            ITcSmTreeItem newFolder2 = plcProjectPous.CreateChild("Folder2", 601, "", null);

            ITcSmTreeItem newPou8 = newFolder2.CreateChild("FB_TESTMethods1", 604, "", null);
            ITcSmTreeItem newPou9 = newFolder2.CreateChild("FB_TESTMethods2", 604, "", null);
            ITcSmTreeItem newPou10 = newFolder2.CreateChild("FB_TESTMethods3", 604, "", null);

            ITcSmTreeItem newMethod1 = newPou8.CreateChild("Method1", 609, "", null);
            ITcSmTreeItem newMethod2 = newPou8.CreateChild("Method2", 609, "", null);

            ITcSmTreeItem newMethod3 = newPou9.CreateChild("Method1", 609, "", null);
            ITcSmTreeItem newMethod4 = newPou9.CreateChild("Method2", 609, "", null);
            ITcSmTreeItem newMethod5 = newPou9.CreateChild("Method3", 609, "", null);

            ITcSmTreeItem newMethod6 = newPou10.CreateChild("Method1", 609, "", null);

            /* ------------------------------------------------------
             * Navigate to DUTs folder
             * ------------------------------------------------------ */
            ITcSmTreeItem plcProjectDuts = _systemManager.LookupTreeItem("TIPC^NewPlcProject^NewPlcProject Project^DUTs");

            /* ------------------------------------------------------
             * Create new folder and then create some STRUCTS below it
             * ------------------------------------------------------ */
            ITcSmTreeItem newFolder3 = plcProjectDuts.CreateChild("Folder3", 601, "", null);

            ITcSmTreeItem newDut1 = newFolder3.CreateChild("ST_TEST1", 606, "", null);
            ITcSmTreeItem newDut2 = newFolder3.CreateChild("ST_TEST2", 606, "", null);
            ITcSmTreeItem newDut3 = newFolder3.CreateChild("ST_TEST3", 606, "", null);

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
            Type t = System.Type.GetTypeFromProgID(TUT09_PROGID);
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
            DirectoryHelper.DeleteDirectory(TUT09_BASEFOLDER);
            Directory.CreateDirectory(TUT09_BASEFOLDER);
            Directory.CreateDirectory(TUT09_BASEFOLDER + TUT09_SOLUTIONFOLDER);

            /* ------------------------------------------------------
             * Create and save new solution
             * ------------------------------------------------------ */
            _solution = _dte.Solution;
            _solution.Create(TUT09_BASEFOLDER, TUT09_SOLUTIONFOLDER);
            _solution.SaveAs(TUT09_BASEFOLDER + TUT09_SOLUTIONFOLDER + TUT09_SLNNAME);
        }

        public static void createTcProject()
        {
            /* ------------------------------------------------------
             * Create new TwinCAT project, based on TwinCAT Project file (delivered with TwinCAT XAE)
             * ------------------------------------------------------ */
            _project = _solution.AddFromTemplate(TUT09_TCTEMPLATEPATH, TUT09_BASEFOLDER + TUT09_SOLUTIONFOLDER, TUT09_NAME);

            /* ------------------------------------------------------
             * Cast TwinCAT project object to ITcSysManager interface --> Automation Interface starts here
             * ------------------------------------------------------ */
            _systemManager = (ITcSysManager)_project.Object;
        }
    }
}
