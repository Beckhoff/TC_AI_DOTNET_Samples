using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using TCatSysManagerLib;

namespace Tutorial_14_ChangeSettingsTcComObjects
{
    /* ======================================================
     * PLEASE NOTE: This tutorial requires that the ZIP file
     * from the Templates folder is being extracted to 
     * "C:\TwinCAT\3.1\Config\Modules\" BEFORE script execution.
     * ====================================================== */
    class Program
    {
        const string TUT14_TCTEMPLATEPATH = @"C:\TwinCAT\3.1\Components\Base\PrjTemplate\TwinCAT Project.tsproj";
        const string TUT14_BASEFOLDER = @"C:\AI_Training\TUT14";
        const string TUT14_SOLUTIONFOLDER = @"\TUT14_Solution";
        const string TUT14_NAME = "NewProject";
        const string TUT14_SLNNAME = @"\NewProject.sln";
        const string TUT14_PROGID = "VisualStudio.DTE.12.0";

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
             * Navigate to TcCOM Objects folder
             * ------------------------------------------------------ */
            ITcSmTreeItem tcom = _systemManager.LookupTreeItem("TIRC^TcCOM Objects");

            /* ------------------------------------------------------
             * GUIDs of TcCOM objects - this is how a TcCOM Object is being uniquely identified
             * ------------------------------------------------------ */
            string guidTempController = "{8f5fdcff-ee4b-4ee5-80b1-25eb23bd1b45}";

            /* ------------------------------------------------------
             * Add TcCOM modules
             * ------------------------------------------------------ */
            ITcSmTreeItem newTcCom1 = tcom.CreateChild("TemperatureControllerModule", 0, "", guidTempController);

            /* ------------------------------------------------------
             * Change settings of TcCOM Object via XML description
             * First load XML description into XmlDocument(), which will be used in all examples.
             * ------------------------------------------------------ */
            string tcComXml = newTcCom1.ProduceXml();
            XmlDocument tcComXmlDoc = new XmlDocument();
            tcComXmlDoc.LoadXml(tcComXml);

            /* ------------------------------------------------------
             * Example: Change module instance name via XML description
             * ------------------------------------------------------ */
            XmlNode instanceNameNode = tcComXmlDoc.SelectSingleNode("TreeItem/TcModuleInstance/Module/InstanceName");
            string newInstanceName = "TemperatureController2";
            instanceNameNode.InnerText = newInstanceName;
            newTcCom1.ConsumeXml(tcComXmlDoc.OuterXml);

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
            Type t = System.Type.GetTypeFromProgID(TUT14_PROGID);
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
            DirectoryHelper.DeleteDirectory(TUT14_BASEFOLDER);
            Directory.CreateDirectory(TUT14_BASEFOLDER);
            Directory.CreateDirectory(TUT14_BASEFOLDER + TUT14_SOLUTIONFOLDER);

            /* ------------------------------------------------------
             * Create and save new solution
             * ------------------------------------------------------ */
            _solution = _dte.Solution;
            _solution.Create(TUT14_BASEFOLDER, TUT14_SOLUTIONFOLDER);
            _solution.SaveAs(TUT14_BASEFOLDER + TUT14_SOLUTIONFOLDER + TUT14_SLNNAME);
        }

        public static void createTcProject()
        {
            /* ------------------------------------------------------
             * Create new TwinCAT project, based on TwinCAT Project file (delivered with TwinCAT XAE)
             * ------------------------------------------------------ */
            _project = _solution.AddFromTemplate(TUT14_TCTEMPLATEPATH, TUT14_BASEFOLDER + TUT14_SOLUTIONFOLDER, TUT14_NAME);

            /* ------------------------------------------------------
             * Cast TwinCAT project object to ITcSysManager interface --> Automation Interface starts here
             * ------------------------------------------------------ */
            _systemManager = (ITcSysManager)_project.Object;
        }
    }
}
