using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TCatSysManagerLib;

namespace Tutorial_01_NewVsInstance
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            MessageFilter.Register();

            /* ======================================================
             * Place tutorial code below
             * ====================================================== */


            string TUT01_TCTEMPLATEPATH = @"C:\TwinCAT\3.1\Components\Base\PrjTemplate\TwinCAT Project.tsproj";
            string TUT01_BASEFOLDER = @"C:\AI_Training\TUT01";
            string TUT01_SOLUTIONFOLDER = @"\TUT01_Solution";

            EnvDTE.DTE dte;
            EnvDTE.Solution solution;
            EnvDTE.Project project;
            ITcSysManager systemManager;

            /* ------------------------------------------------------
             * Create Visual Studio instance and make VS window visible
             * ------------------------------------------------------ */
            Type t = System.Type.GetTypeFromProgID("VisualStudio.DTE.12.0");
            dte = (EnvDTE.DTE)System.Activator.CreateInstance(t);
            dte.SuppressUI = false;
            dte.MainWindow.Visible = true;

            /* ------------------------------------------------------
             * Create directories for new Visual Studio solution
             * ------------------------------------------------------ */
            DirectoryHelper.DeleteDirectory(TUT01_BASEFOLDER);
            Directory.CreateDirectory(TUT01_BASEFOLDER);
            Directory.CreateDirectory(TUT01_BASEFOLDER + TUT01_SOLUTIONFOLDER);

            /* ------------------------------------------------------
             * Create and save new solution
             * ------------------------------------------------------ */
            solution = dte.Solution;
            solution.Create(TUT01_BASEFOLDER, TUT01_SOLUTIONFOLDER);
            solution.SaveAs(TUT01_BASEFOLDER + TUT01_SOLUTIONFOLDER + @"\NewProject.sln");

            /* ------------------------------------------------------
             * Create new TwinCAT project, based on TwinCAT Project file (delivered with TwinCAT XAE)
             * ------------------------------------------------------ */
            project = solution.AddFromTemplate(TUT01_TCTEMPLATEPATH, TUT01_BASEFOLDER + TUT01_SOLUTIONFOLDER, "NewProject");

            /* ------------------------------------------------------
             * Cast TwinCAT project object to ITcSysManager interface --> Automation Interface starts here
             * ------------------------------------------------------ */
            systemManager = (ITcSysManager)project.Object;


            /* ======================================================
             * Place tutorial code above
             * ====================================================== */

            MessageFilter.Revoke();
        }
    }
}
