using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TCatSysManagerLib;

namespace Tutorial_04_ExistingTcConfiguration
{
    class Program
    {
        const string TUT04_TCTEMPLATEPATH = @"C:\TwinCAT\3.1\Components\Base\PrjTemplate\TwinCAT Project.tsproj";
        const string TUT04_BASEFOLDER = @"C:\AI_Training\TUT04";
        const string TUT04_SOLUTIONFOLDER = @"\TUT04_Solution";
        const string TUT04_NAME = "NewProject";
        const string TUT04_SLNNAME = @"\NewProject.sln";
        const string TUT04_PROGID = "VisualStudio.DTE.12.0";

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

            #region FromTutorial03_CreatesAndClosesTwinCatConfiguration

            createVsInstance(false, false, false);
            createVsSolution();

            /* ------------------------------------------------------
             * Create new TwinCAT project, based on TwinCAT Project file (delivered with TwinCAT XAE)
             * ------------------------------------------------------ */
            _project = _solution.AddFromTemplate(TUT04_TCTEMPLATEPATH, TUT04_BASEFOLDER + TUT04_SOLUTIONFOLDER, TUT04_NAME);
            _project.Save();
            _solution.SaveAs(TUT04_BASEFOLDER + TUT04_SOLUTIONFOLDER + TUT04_SLNNAME);
            _solution.Close();

            #endregion


            /* ------------------------------------------------------
             * Create Visual Studio instance and make VS window visible
             * ------------------------------------------------------ */
            createVsInstance(false, true, true);

            /* ------------------------------------------------------
             * Open existing solution
             * ------------------------------------------------------ */
            _solution = _dte.Solution;
            _solution.Open(TUT04_BASEFOLDER + TUT04_SOLUTIONFOLDER + TUT04_SLNNAME);


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
            Type t = System.Type.GetTypeFromProgID(TUT04_PROGID);
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
            DirectoryHelper.DeleteDirectory(TUT04_BASEFOLDER);
            Directory.CreateDirectory(TUT04_BASEFOLDER);
            Directory.CreateDirectory(TUT04_BASEFOLDER + TUT04_SOLUTIONFOLDER);

            /* ------------------------------------------------------
             * Create and save new solution
             * ------------------------------------------------------ */
            _solution = _dte.Solution;
            _solution.Create(TUT04_BASEFOLDER, TUT04_SOLUTIONFOLDER);
            _solution.SaveAs(TUT04_BASEFOLDER + TUT04_SOLUTIONFOLDER + TUT04_SLNNAME);
        }
    }
}
