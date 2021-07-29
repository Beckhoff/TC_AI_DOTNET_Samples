using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TCatSysManagerLib;

namespace Tutorial_11_AddRemoveLibraryReferences
{
    class Program
    {
        const string TUT11_TCTEMPLATEPATH = @"C:\TwinCAT\3.1\Components\Base\PrjTemplate\TwinCAT Project.tsproj";
        const string TUT11_BASEFOLDER = @"C:\AI_Training\TUT11";
        const string TUT11_SOLUTIONFOLDER = @"\TUT11_Solution";
        const string TUT11_NAME = "NewProject";
        const string TUT11_SLNNAME = @"\NewProject.sln";
        const string TUT11_PROGID = "VisualStudio.DTE.12.0";

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
             * Navigate to References node and cast to specific interface
             * ------------------------------------------------------ */
            ITcSmTreeItem references = _systemManager.LookupTreeItem("TIPC^NewPlcProject^NewPlcProject Project^References");
            ITcPlcLibraryManager libraryManager = (ITcPlcLibraryManager)references;

            /* ------------------------------------------------------
             * Iterate through all references of the current project
             * ------------------------------------------------------ */
            foreach (ITcPlcLibRef libraryReference in libraryManager.References)
            {
                if (libraryReference is ITcPlcLibrary)
                {
                    ITcPlcLibrary library = (ITcPlcLibrary)libraryReference;
                    Console.WriteLine("Found library: " + library.DisplayName);
                }

                if (libraryReference is ITcPlcPlaceholderRef)
                {
                    ITcPlcPlaceholderRef placeholder = (ITcPlcPlaceholderRef)libraryReference;
                    Console.WriteLine("Found placeholder: " + placeholder.PlaceholderName);
                }
            }

            /* ------------------------------------------------------
             * Add library (which is already in repository) as a reference to the PLC project
             * ------------------------------------------------------ */
            libraryManager.AddLibrary("Tc2_TcpIp", "*", "Beckhoff Automation GmbH");
            libraryManager.AddLibrary("Tc2_Database", "*", "Beckhoff Automation GmbH");
            libraryManager.AddLibrary("Tc2_Math", "*", "Beckhoff Automation GmbH");

            /* ------------------------------------------------------
             * Remove library from references
             * ------------------------------------------------------ */
            libraryManager.RemoveReference("Tc2_Math");

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
            Type t = System.Type.GetTypeFromProgID(TUT11_PROGID);
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
            DirectoryHelper.DeleteDirectory(TUT11_BASEFOLDER);
            Directory.CreateDirectory(TUT11_BASEFOLDER);
            Directory.CreateDirectory(TUT11_BASEFOLDER + TUT11_SOLUTIONFOLDER);

            /* ------------------------------------------------------
             * Create and save new solution
             * ------------------------------------------------------ */
            _solution = _dte.Solution;
            _solution.Create(TUT11_BASEFOLDER, TUT11_SOLUTIONFOLDER);
            _solution.SaveAs(TUT11_BASEFOLDER + TUT11_SOLUTIONFOLDER + TUT11_SLNNAME);
        }

        public static void createTcProject()
        {
            /* ------------------------------------------------------
             * Create new TwinCAT project, based on TwinCAT Project file (delivered with TwinCAT XAE)
             * ------------------------------------------------------ */
            _project = _solution.AddFromTemplate(TUT11_TCTEMPLATEPATH, TUT11_BASEFOLDER + TUT11_SOLUTIONFOLDER, TUT11_NAME);

            /* ------------------------------------------------------
             * Cast TwinCAT project object to ITcSysManager interface --> Automation Interface starts here
             * ------------------------------------------------------ */
            _systemManager = (ITcSysManager)_project.Object;
        }
    }
}
