using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TCatSysManagerLib;

namespace Tutorial_06_AddingIOtemplates
{
    class Program
    {
        const string TUT06_TCTEMPLATEPATH = @"C:\TwinCAT\3.1\Components\Base\PrjTemplate\TwinCAT Project.tsproj";
        const string TUT06_BASEFOLDER = @"C:\AI_Training\TUT06";
        const string TUT06_SOLUTIONFOLDER = @"\TUT06_Solution";
        const string TUT06_NAME = "NewProject";
        const string TUT06_SLNNAME = @"\NewProject.sln";
        const string TUT06_PROGID = "VisualStudio.DTE.12.0";

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
             * Navigate to I/O node and create EtherCAT Master, EK1100 and several terminals via ImportChild()
             * ------------------------------------------------------ */
            ITcSmTreeItem io = _systemManager.LookupTreeItem("TIID");
            ITcSmTreeItem etherCatMaster = io.ImportChild(templateDir + "TUT06_EthercatMaster.xti", "", true, "EtherCAT Master");
            ITcSmTreeItem ek1100 = etherCatMaster.ImportChild(templateDir + "TUT06_EK1100.xti", "", true, "EK1100");
            ITcSmTreeItem term1 = ek1100.ImportChild(templateDir + "TUT06_EL1008.xti", "", true, "EL1008-1");
            ITcSmTreeItem term2 = ek1100.ImportChild(templateDir + "TUT06_EL1008.xti", "", true, "EL1008-2");
            ITcSmTreeItem term3 = ek1100.ImportChild(templateDir + "TUT06_EL2008.xti", "", true, "EL2008-3");
            ITcSmTreeItem term4 = ek1100.ImportChild(templateDir + "TUT06_EL2008.xti", "", true, "EL2008-4");

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
            Type t = System.Type.GetTypeFromProgID(TUT06_PROGID);
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
            DirectoryHelper.DeleteDirectory(TUT06_BASEFOLDER);
            Directory.CreateDirectory(TUT06_BASEFOLDER);
            Directory.CreateDirectory(TUT06_BASEFOLDER + TUT06_SOLUTIONFOLDER);

            /* ------------------------------------------------------
             * Create and save new solution
             * ------------------------------------------------------ */
            _solution = _dte.Solution;
            _solution.Create(TUT06_BASEFOLDER, TUT06_SOLUTIONFOLDER);
            _solution.SaveAs(TUT06_BASEFOLDER + TUT06_SOLUTIONFOLDER + TUT06_SLNNAME);
        }

        public static void createTcProject()
        {
            /* ------------------------------------------------------
             * Create new TwinCAT project, based on TwinCAT Project file (delivered with TwinCAT XAE)
             * ------------------------------------------------------ */
            _project = _solution.AddFromTemplate(TUT06_TCTEMPLATEPATH, TUT06_BASEFOLDER + TUT06_SOLUTIONFOLDER, TUT06_NAME);

            /* ------------------------------------------------------
             * Cast TwinCAT project object to ITcSysManager interface --> Automation Interface starts here
             * ------------------------------------------------------ */
            _systemManager = (ITcSysManager)_project.Object;
        }
    }
}
