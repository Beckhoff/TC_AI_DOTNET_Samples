using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TCatSysManagerLib;

namespace Tutorial_03_NewTcConfiguration
{
    class Program
    {
        const string TUT03_TCTEMPLATEPATH = @"C:\TwinCAT\3.1\Components\Base\PrjTemplate\TwinCAT Project.tsproj";
        const string TUT03_BASEFOLDER = @"C:\AI_Training\TUT03";
        const string TUT03_SOLUTIONFOLDER = @"\TUT03_Solution";
        const string TUT03_NAME = "NewProject";
        const string TUT03_SLNNAME = @"\NewProject.sln";
        const string TUT03_PROGID = "VisualStudio.DTE.12.0";

        static EnvDTE.DTE _dte;
        static EnvDTE.Solution _solution;
        static EnvDTE.Project _project;

        [STAThread]
        static void Main(string[] args)
        {
            MessageFilter.Register();

            /* ======================================================
             * Place tutorial code below
             * ====================================================== */

            createVsInstance(false, true, true);

            /* ------------------------------------------------------
             * Create directories for new Visual Studio solution
             * ------------------------------------------------------ */
            DirectoryHelper.DeleteDirectory(TUT03_BASEFOLDER);
            Directory.CreateDirectory(TUT03_BASEFOLDER);
            Directory.CreateDirectory(TUT03_BASEFOLDER + TUT03_SOLUTIONFOLDER);

            /* ------------------------------------------------------
             * Create and save new solution
             * ------------------------------------------------------ */
            _solution = _dte.Solution;
            _solution.Create(TUT03_BASEFOLDER, TUT03_SOLUTIONFOLDER);
            _solution.SaveAs(TUT03_BASEFOLDER + TUT03_SOLUTIONFOLDER + TUT03_SLNNAME);

            /* ------------------------------------------------------
             * Create new TwinCAT project, based on TwinCAT Project file (delivered with TwinCAT XAE)
             * ------------------------------------------------------ */
            _project = _solution.AddFromTemplate(TUT03_TCTEMPLATEPATH, TUT03_BASEFOLDER + TUT03_SOLUTIONFOLDER, TUT03_NAME);

            /* ------------------------------------------------------
             * Cast TwinCAT project object to ITcSysManager interface --> Automation Interface starts here
             * ------------------------------------------------------ */
            ITcSysManager systemManager = (ITcSysManager)_project.Object;

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
            Type t = System.Type.GetTypeFromProgID(TUT03_PROGID);
            _dte = (EnvDTE.DTE)System.Activator.CreateInstance(t);
            _dte.SuppressUI = suppressUI;
            _dte.UserControl = userControl; // true = leaves VS window open after code execution
            _dte.MainWindow.Visible = visible;
        }
    }
}
