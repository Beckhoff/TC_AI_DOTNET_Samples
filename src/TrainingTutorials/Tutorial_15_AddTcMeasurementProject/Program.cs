using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TCatSysManagerLib;

namespace Tutorial_15_AddTcMeasurementProject
{
    class Program
    {
        const string TUT15_TCTEMPLATEPATH = @"C:\TwinCAT\3.1\Components\Base\PrjTemplate\TwinCAT Project.tsproj";
        const string TUT15_BASEFOLDER = @"C:\AI_Training\TUT15";
        const string TUT15_SOLUTIONFOLDER = @"\TUT15_Solution";
        const string TUT15_NAME = "NewProject";
        const string TUT15_SLNNAME = @"\NewProject.sln";
        const string TUT15_PROGID = "VisualStudio.DTE.12.0";

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
             * Specify path to TwinCAT Measurement Project Templates
             * ------------------------------------------------------ */
            string template_emptyMeasurementProject = @"C:\TwinCAT\Functions\TE130X-Scope-View\Templates\Projects\Empty Measurement Project.tcmproj";
            string template_measurementScopeProject = @"C:\TwinCAT\Functions\TE130X-Scope-View\Templates\Projects\Measurement Scope Project.tcmproj";
            
            /* ------------------------------------------------------
             * Add new Measurement Projects, according to template files
             * ------------------------------------------------------ */
            EnvDTE.Project scopeProject1 = _dte.Solution.AddFromTemplate(template_emptyMeasurementProject, TUT15_BASEFOLDER + TUT15_SOLUTIONFOLDER, "NewEmptyMeasurementProject");
            EnvDTE.Project scopeProject2 = _dte.Solution.AddFromTemplate(template_measurementScopeProject, TUT15_BASEFOLDER + TUT15_SOLUTIONFOLDER, "NewMeasurementProject");

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
            Type t = System.Type.GetTypeFromProgID(TUT15_PROGID);
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
            DirectoryHelper.DeleteDirectory(TUT15_BASEFOLDER);
            Directory.CreateDirectory(TUT15_BASEFOLDER);
            Directory.CreateDirectory(TUT15_BASEFOLDER + TUT15_SOLUTIONFOLDER);

            /* ------------------------------------------------------
             * Create and save new solution
             * ------------------------------------------------------ */
            _solution = _dte.Solution;
            _solution.Create(TUT15_BASEFOLDER, TUT15_SOLUTIONFOLDER);
            _solution.SaveAs(TUT15_BASEFOLDER + TUT15_SOLUTIONFOLDER + TUT15_SLNNAME);
        }

        public static void createTcProject()
        {
            /* ------------------------------------------------------
             * Create new TwinCAT project, based on TwinCAT Project file (delivered with TwinCAT XAE)
             * ------------------------------------------------------ */
            _project = _solution.AddFromTemplate(TUT15_TCTEMPLATEPATH, TUT15_BASEFOLDER + TUT15_SOLUTIONFOLDER, TUT15_NAME);

            /* ------------------------------------------------------
             * Cast TwinCAT project object to ITcSysManager interface --> Automation Interface starts here
             * ------------------------------------------------------ */
            _systemManager = (ITcSysManager)_project.Object;
        }
    }
}
