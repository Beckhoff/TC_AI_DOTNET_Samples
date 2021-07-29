using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TCatSysManagerLib;

namespace Tutorial_12_AddRemoveLibraryRepositories
{
    class Program
    {
        const string TUT12_TCTEMPLATEPATH = @"C:\TwinCAT\3.1\Components\Base\PrjTemplate\TwinCAT Project.tsproj";
        const string TUT12_BASEFOLDER = @"C:\AI_Training\TUT12";
        const string TUT12_SOLUTIONFOLDER = @"\TUT12_Solution";
        const string TUT12_NAME = "NewProject";
        const string TUT12_SLNNAME = @"\NewProject.sln";
        const string TUT12_PROGID = "VisualStudio.DTE.12.0";

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
             * Create new PLC Project, according to "Standard PLC Project" template (Default TwinCAT)
             * ------------------------------------------------------ */
            ITcSmTreeItem newProject = plc.CreateChild("NewPlcProject", 0, "", "Standard PLC Template");

            /* ------------------------------------------------------
             * Navigate to References node and cast to specific interface
             * ------------------------------------------------------ */
            ITcSmTreeItem references = _systemManager.LookupTreeItem("TIPC^NewPlcProject^NewPlcProject Project^References");
            ITcPlcLibraryManager libraryManager = (ITcPlcLibraryManager)references;
            
            /* ------------------------------------------------------
             * Iterate through all configured repositories
             * ------------------------------------------------------ */
            foreach (ITcPlcLibRepository repo in libraryManager.Repositories)
            {
                Console.WriteLine("Found repository " + repo.Name + " in folder " + repo.Folder);
            }

            /* ------------------------------------------------------
             * Create new repository
             * ------------------------------------------------------ */
            string newRepoPath = @"C:\AI_Training\TestRepository";
            Directory.CreateDirectory(newRepoPath);
            libraryManager.InsertRepository("TestRepository", newRepoPath, 0);

            /* ------------------------------------------------------
             * Install library into new repository
             * ------------------------------------------------------ */
            libraryManager.InstallLibrary("TestRepository", templateDir + "TUT12_PlcLibrary.library", true);

            /* ------------------------------------------------------
             * Uninstall library from repository
             * ------------------------------------------------------ */
            libraryManager.UninstallLibrary("TestRepository", "TUT12_PlcLibrary", "*", "Beckhoff Automation GmbH");

            /* ------------------------------------------------------
             * Remove repository from system
             * ------------------------------------------------------ */
            libraryManager.RemoveRepository("TestRepository");
            Directory.Delete(newRepoPath, true);

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
            Type t = System.Type.GetTypeFromProgID(TUT12_PROGID);
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
            DirectoryHelper.DeleteDirectory(TUT12_BASEFOLDER);
            Directory.CreateDirectory(TUT12_BASEFOLDER);
            Directory.CreateDirectory(TUT12_BASEFOLDER + TUT12_SOLUTIONFOLDER);

            /* ------------------------------------------------------
             * Create and save new solution
             * ------------------------------------------------------ */
            _solution = _dte.Solution;
            _solution.Create(TUT12_BASEFOLDER, TUT12_SOLUTIONFOLDER);
            _solution.SaveAs(TUT12_BASEFOLDER + TUT12_SOLUTIONFOLDER + TUT12_SLNNAME);
        }

        public static void createTcProject()
        {
            /* ------------------------------------------------------
             * Create new TwinCAT project, based on TwinCAT Project file (delivered with TwinCAT XAE)
             * ------------------------------------------------------ */
            _project = _solution.AddFromTemplate(TUT12_TCTEMPLATEPATH, TUT12_BASEFOLDER + TUT12_SOLUTIONFOLDER, TUT12_NAME);

            /* ------------------------------------------------------
             * Cast TwinCAT project object to ITcSysManager interface --> Automation Interface starts here
             * ------------------------------------------------------ */
            _systemManager = (ITcSysManager)_project.Object;
        }
    }
}
