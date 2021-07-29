using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TCatSysManagerLib;

namespace Tutorial_02_ExistingVsInstance
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

            #region FromTutorial01_StartsNewVsInstance

            string TUT01_TCTEMPLATEPATH = @"C:\TwinCAT\3.1\Components\Base\PrjTemplate\TwinCAT Project.tsproj";
            string TUT01_BASEFOLDER = @"C:\AI_Training\TUT02";
            string TUT01_SOLUTIONFOLDER = @"\TUT02_Solution";

            EnvDTE.DTE dte;
            EnvDTE.Solution solution;
            EnvDTE.Project project;

            /* ------------------------------------------------------
             * Create Visual Studio instance and make VS window visible
             * ------------------------------------------------------ */
            Type t = System.Type.GetTypeFromProgID("VisualStudio.DTE.12.0");
            dte = (EnvDTE.DTE)System.Activator.CreateInstance(t);
            dte.SuppressUI = false;
            dte.UserControl = true;
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

            #endregion

            EnvDTE.DTE dte2 = attachToExistingDte(TUT01_BASEFOLDER + TUT01_SOLUTIONFOLDER + @"\NewProject.sln", "VisualStudio.DTE.12.0");

            /* ======================================================
             * Place tutorial code above
             * ====================================================== */

            MessageFilter.Revoke();
        }

        public static EnvDTE.DTE attachToExistingDte(string solutionPath, string progId)
        {
            EnvDTE.DTE dte = null;
            try
            {
                Hashtable dteInstances = Helper.GetIDEInstances(false, progId);
                IDictionaryEnumerator hashtableEnumerator = dteInstances.GetEnumerator();

                while (hashtableEnumerator.MoveNext())
                {
                    EnvDTE.DTE dteTemp = (EnvDTE.DTE)hashtableEnumerator.Value;
                    if (dteTemp.Solution.FullName == solutionPath)
                    {
                        Console.WriteLine("Found solution in list of all open DTE objects. " + dteTemp.Name);
                        dte = dteTemp;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            return dte;
        }
    }
}
