using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TCatSysManagerLib;

namespace Tutorial_05_AddingIO
{
    class Program
    {
        const string TUT05_TCTEMPLATEPATH = @"C:\TwinCAT\3.1\Components\Base\PrjTemplate\TwinCAT Project.tsproj";
        const string TUT05_BASEFOLDER = @"C:\AI_Training\TUT05";
        const string TUT05_SOLUTIONFOLDER = @"\TUT05_Solution";
        const string TUT05_NAME = "NewProject";
        const string TUT05_SLNNAME = @"\NewProject.sln";
        const string TUT05_PROGID = "VisualStudio.DTE.12.0";

        static EnvDTE.DTE _dte;
        static EnvDTE.Solution _solution;
        static EnvDTE.Project _project;

        static ITcSysManager _systemManager;

        [STAThread]
        static int Main(string[] args)
        {
            try
            {
                MessageFilter.Register();

                createVsInstance(false, true, true);
                createVsSolution();
                createTcProject();

                /* ======================================================
                 * Place tutorial code below
                 * ====================================================== */

                /* ------------------------------------------------------
                 * Navigate to I/O node and create new EtherCAT Master, which is identified by its SubType 111
                 * ------------------------------------------------------ */

                ITcSmTreeItem io = _systemManager.LookupTreeItem("TIID");
                ITcSmTreeItem newEtherCatMaster = io.CreateChild("EtherCAT Master", 111, null, null);

                /* ------------------------------------------------------
                 * Below the EtherCAT Master, create an EK1100 and several terminals
                 * ------------------------------------------------------ */

                ITcSmTreeItem newEk1100_1 = newEtherCatMaster.CreateChild("EK1100-1", 999, "", "EK1100-0000-0001");
                ITcSmTreeItem newTerm1 = newEk1100_1.CreateChild("EL2004-1", 9099, "", "EL2004-0000-0000");
                ITcSmTreeItem newTerm2 = newEk1100_1.CreateChild("EL2004-2", 999, "", "EL2004-0000-0000");
                ITcSmTreeItem newTerm3 = newEk1100_1.CreateChild("EL2004-3", 999, "", "EL2004-0000-0000");
                ITcSmTreeItem newTerm4 = newEk1100_1.CreateChild("EL2004-4", 999, "", "EL2004-0000-0000");
                ITcSmTreeItem newTerm5 = newEk1100_1.CreateChild("EL2004-5", 999, "", "EL2004-0000-0000");
                ITcSmTreeItem newTerm6 = newEk1100_1.CreateChild("EL2004-6", 999, "", "EL2004-0000-0000");
                ITcSmTreeItem newTerm7 = newEk1100_1.CreateChild("EL1004-7", 999, "", "EL1004-0000-0000");
                ITcSmTreeItem newTerm8 = newEk1100_1.CreateChild("EL1004-8", 999, "", "EL1004-0000-0000");
                ITcSmTreeItem newTerm9 = newEk1100_1.CreateChild("EL1004-9", 999, "", "EL1004-0000-0000");

                /* ------------------------------------------------------
                 * Below the EtherCAT Master, create another EK1100 and several terminals
                 * ------------------------------------------------------ */

                ITcSmTreeItem newEk1100_2 = newEtherCatMaster.CreateChild("EK1100-2", 999, "", "EK1100-0000-0001");
                ITcSmTreeItem newTerm10 = newEk1100_2.CreateChild("EL2004-10", 999, "", "EL2004-0000-0000");
                ITcSmTreeItem newTerm11 = newEk1100_2.CreateChild("EL2004-11", 999, "", "EL2004-0000-0000");
                ITcSmTreeItem newTerm12 = newEk1100_2.CreateChild("EL2004-12", 999, "", "EL2004-0000-0000");
                ITcSmTreeItem newTerm13 = newEk1100_2.CreateChild("EL2004-13", 999, "", "EL2004-0000-0000");
                ITcSmTreeItem newTerm14 = newEk1100_2.CreateChild("EL2004-14", 999, "", "EL2004-0000-0000");
                ITcSmTreeItem newTerm15 = newEk1100_2.CreateChild("EL2004-15", 999, "", "EL2004-0000-0000");
                ITcSmTreeItem newTerm16 = newEk1100_2.CreateChild("EL1004-16", 999, "", "EL1004-0000-0000");
                ITcSmTreeItem newTerm17 = newEk1100_2.CreateChild("EL1004-17", 999, "", "EL1004-0000-0000");
                ITcSmTreeItem newTerm18 = newEk1100_2.CreateChild("EL1004-18", 999, "", "EL1004-0000-0000");

                /* ------------------------------------------------------
                 * Below the EtherCAT Master, create a BK1120 and several terminals (K-Bus)
                 * ------------------------------------------------------ */
                ITcSmTreeItem newBk1120 = newEtherCatMaster.CreateChild("BK1120", 999, "", "BK1120-0000-9995");
                ITcSmTreeItem newTerm19 = newBk1120.CreateChild("KL1104", 1104, "End Term (KL9010)");
                ITcSmTreeItem newTerm20 = newBk1120.CreateChild("KL1104", 1104, "End Term (KL9010)");
                ITcSmTreeItem newTerm21 = newBk1120.CreateChild("KL1408", 1408, "End Term (KL9010)");
                ITcSmTreeItem newTerm22 = newBk1120.CreateChild("KL1408", 1408, "End Term (KL9010)");

                /* ======================================================
                 * Place tutorial code above
                 * ====================================================== */

                MessageFilter.Revoke();
            }
            catch(Exception ex)
            {
                Console.Error.WriteLine("Error: " + ex.Message);
                return 42;
            }
            return 0;
        }

        public static void createVsInstance(bool suppressUI, bool userControl, bool visible)
        {
            /* ------------------------------------------------------
             * Create Visual Studio instance and make VS window visible
             * ------------------------------------------------------ */
            Type t = System.Type.GetTypeFromProgID(TUT05_PROGID);
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
            DirectoryHelper.DeleteDirectory(TUT05_BASEFOLDER);
            Directory.CreateDirectory(TUT05_BASEFOLDER);
            Directory.CreateDirectory(TUT05_BASEFOLDER + TUT05_SOLUTIONFOLDER);

            /* ------------------------------------------------------
             * Create and save new solution
             * ------------------------------------------------------ */
            _solution = _dte.Solution;
            _solution.Create(TUT05_BASEFOLDER, TUT05_SOLUTIONFOLDER);
            _solution.SaveAs(TUT05_BASEFOLDER + TUT05_SOLUTIONFOLDER + TUT05_SLNNAME);
        }

        public static void createTcProject()
        {
            /* ------------------------------------------------------
             * Create new TwinCAT project, based on TwinCAT Project file (delivered with TwinCAT XAE)
             * ------------------------------------------------------ */
            _project = _solution.AddFromTemplate(TUT05_TCTEMPLATEPATH, TUT05_BASEFOLDER + TUT05_SOLUTIONFOLDER, TUT05_NAME);

            /* ------------------------------------------------------
             * Cast TwinCAT project object to ITcSysManager interface --> Automation Interface starts here
             * ------------------------------------------------------ */
            _systemManager = (ITcSysManager)_project.Object;
        }
    }
}
