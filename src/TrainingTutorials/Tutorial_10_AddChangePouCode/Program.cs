using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TCatSysManagerLib;

namespace Tutorial_10_AddChangePouCode
{
    class Program
    {
        const string TUT10_TCTEMPLATEPATH = @"C:\TwinCAT\3.1\Components\Base\PrjTemplate\TwinCAT Project.tsproj";
        const string TUT10_BASEFOLDER = @"C:\AI_Training\TUT10";
        const string TUT10_SOLUTIONFOLDER = @"\TUT10_Solution";
        const string TUT10_NAME = "NewProject";
        const string TUT10_SLNNAME = @"\NewProject.sln";
        const string TUT10_PROGID = "VisualStudio.DTE.12.0";

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
             * Navigate to MAIN POU
             * ------------------------------------------------------ */
            ITcSmTreeItem main = _systemManager.LookupTreeItem("TIPC^NewPlcProject^NewPlcProject Project^POUs^MAIN");

            /* ------------------------------------------------------
             * Cast to specific interface for declaration and implementation area
             * ------------------------------------------------------ */
            ITcPlcDeclaration mainDecl = (ITcPlcDeclaration)main;
            ITcPlcImplementation mainImpl = (ITcPlcImplementation)main;

            /* ------------------------------------------------------
             * Get current declaration and implementation area content
             * ------------------------------------------------------ */
            string strMainDecl = mainDecl.DeclarationText;
            string strMainImpl = mainImpl.ImplementationText;

            /* ------------------------------------------------------
             * Define and set new declaration and implementation area content
             * ------------------------------------------------------ */
            string strMainDeclNew = @"PROGRAM MAIN
VAR
	fbTest1	: FB_TEST1;
	fbTest2 : FB_TEST2;
    bBool   : BOOL;
END_VAR";

            string strMainImplNew = "bBool := NOT bBool;";

            mainDecl.DeclarationText = strMainDeclNew;
            mainImpl.ImplementationText = strMainImplNew;


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
            Type t = System.Type.GetTypeFromProgID(TUT10_PROGID);
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
            DirectoryHelper.DeleteDirectory(TUT10_BASEFOLDER);
            Directory.CreateDirectory(TUT10_BASEFOLDER);
            Directory.CreateDirectory(TUT10_BASEFOLDER + TUT10_SOLUTIONFOLDER);

            /* ------------------------------------------------------
             * Create and save new solution
             * ------------------------------------------------------ */
            _solution = _dte.Solution;
            _solution.Create(TUT10_BASEFOLDER, TUT10_SOLUTIONFOLDER);
            _solution.SaveAs(TUT10_BASEFOLDER + TUT10_SOLUTIONFOLDER + TUT10_SLNNAME);
        }

        public static void createTcProject()
        {
            /* ------------------------------------------------------
             * Create new TwinCAT project, based on TwinCAT Project file (delivered with TwinCAT XAE)
             * ------------------------------------------------------ */
            _project = _solution.AddFromTemplate(TUT10_TCTEMPLATEPATH, TUT10_BASEFOLDER + TUT10_SOLUTIONFOLDER, TUT10_NAME);

            /* ------------------------------------------------------
             * Cast TwinCAT project object to ITcSysManager interface --> Automation Interface starts here
             * ------------------------------------------------------ */
            _systemManager = (ITcSysManager)_project.Object;
        }
    }
}
