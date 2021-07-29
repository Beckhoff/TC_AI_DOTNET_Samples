using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TCatSysManagerLib;
using TwinCAT.Measurement.AutomationInterface;
using TwinCAT.Scope2.Communications;

namespace Tutorial_16_AddChartsAxesChannels
{
    class Program
    {
        const string TUT16_TCTEMPLATEPATH = @"C:\TwinCAT\3.1\Components\Base\PrjTemplate\TwinCAT Project.tsproj";
        const string TUT16_BASEFOLDER = @"C:\AI_Training\TUT16";
        const string TUT16_SOLUTIONFOLDER = @"\TUT16_Solution";
        const string TUT16_NAME = "NewProject";
        const string TUT16_SLNNAME = @"\NewProject.sln";
        const string TUT16_PROGID = "VisualStudio.DTE.12.0";

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
            string template_measurementScopeProject = @"C:\TwinCAT\Functions\TE130X-Scope-View\Templates\Projects\Measurement Scope Project.tcmproj";

            /* ======================================================
             * Place tutorial code below
             * ====================================================== */

            /* ------------------------------------------------------
             * First prepare PLC Project (includes a "Function Generator")
             * Will be later used to link scope channels
             * ------------------------------------------------------ */
            ITcSmTreeItem plc = _systemManager.LookupTreeItem("TIPC");
            ITcSmTreeItem plcProject = plc.CreateChild("FunctionGenerator", 0, "", templateDir + "TUT16_PlcProject.tpzip");

            /* ------------------------------------------------------
             * Set PLC project as AutoStart
             * ------------------------------------------------------ */
            ITcPlcProject plcProjectIec = (ITcPlcProject)plcProject;
            plcProjectIec.BootProjectAutostart = true;
            plcProjectIec.GenerateBootProject(true);

            /* ------------------------------------------------------
             * Activate configuration on local system and restart TwinCAT
             * Side note: PLC Project is configured to be automatically started
             * ------------------------------------------------------ */
            _systemManager.ActivateConfiguration();
            _systemManager.StartRestartTwinCAT();

            /* ------------------------------------------------------
             * Due to asynchronous StartRestartTwinCAT() call, insert a Thread.Sleep()
             * ------------------------------------------------------ */
            System.Threading.Thread.Sleep(2000);
            
            /* ------------------------------------------------------
             * Add new Measurement Projects, according to template file
             * ------------------------------------------------------ */
            EnvDTE.Project scopeProject = _dte.Solution.AddFromTemplate(template_measurementScopeProject, TUT16_BASEFOLDER + TUT16_SOLUTIONFOLDER, "NewMeasurementProject");
            ((IMeasurementScope)scopeProject.ProjectItems.Item(1).Object).ShowControl();

            /* ------------------------------------------------------
             * Add a new chart
             * Side note: As you can see, usage of CreateChild() is a little bit different in Scope AI
             * ------------------------------------------------------ */
            EnvDTE.ProjectItem newChart;
            ((IMeasurementScope)scopeProject.ProjectItems.Item(1).Object).CreateChild(out newChart);

            /* ------------------------------------------------------
             * Change name of chart
             * ------------------------------------------------------ */
            IMeasurementScope newChartObj = (IMeasurementScope)newChart.Object;
            newChartObj.ChangeName("FunctionGenerator Chart");

            /* ------------------------------------------------------
             * Set chart property "StackedYAxis"
             * ------------------------------------------------------ */
            foreach (EnvDTE.Property property in newChart.Properties)
                if (property.Name == "StackedYAxis")
                    property.Value = true;

            /* ------------------------------------------------------
             * Adding a new axis
             * ------------------------------------------------------ */
            EnvDTE.ProjectItem newAxis;
            newChartObj.CreateChild(out newAxis);
            IMeasurementScope newAxisObj = (IMeasurementScope)newAxis.Object;

            /* ------------------------------------------------------
             * Change name of axis
             * ------------------------------------------------------ */
            newAxisObj.ChangeName("Axis 1");

            /* ------------------------------------------------------
             * Adding a new channel
             * ------------------------------------------------------ */
            EnvDTE.ProjectItem newChannel;
            newAxisObj.CreateChild(out newChannel);
            IMeasurementScope newChannelObj = (IMeasurementScope)newChannel.Object;

            /* ------------------------------------------------------
             * Change name of channel
             * ------------------------------------------------------ */
            newChannelObj.ChangeName("Signals.Triangular");

            /* ------------------------------------------------------
             * Change channel properties (link to PLC variable!)
             * ------------------------------------------------------ */
            foreach (EnvDTE.Property property in newChannel.Properties)
            {
                switch (property.Name)
                {
                    case "TargetPorts":
                        property.Value = "851";
                        break;

                    case "Symbolbased":
                        property.Value = true;
                        break;

                    case "SymbolDataType":
                        property.Value = Scope2DataType.REAL64;
                        break;

                    case "SymbolName":
                        property.Value = "MAIN.aSquareBuffer[1]";
                        break;

                    case "SampleState":
                        property.Value = 0;
                        break;

                    case "LineWidth":
                        property.Value = 3;
                        break;
                }
            }

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
            Type t = System.Type.GetTypeFromProgID(TUT16_PROGID);
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
            DirectoryHelper.DeleteDirectory(TUT16_BASEFOLDER);
            Directory.CreateDirectory(TUT16_BASEFOLDER);
            Directory.CreateDirectory(TUT16_BASEFOLDER + TUT16_SOLUTIONFOLDER);

            /* ------------------------------------------------------
             * Create and save new solution
             * ------------------------------------------------------ */
            _solution = _dte.Solution;
            _solution.Create(TUT16_BASEFOLDER, TUT16_SOLUTIONFOLDER);
            _solution.SaveAs(TUT16_BASEFOLDER + TUT16_SOLUTIONFOLDER + TUT16_SLNNAME);
        }

        public static void createTcProject()
        {
            /* ------------------------------------------------------
             * Create new TwinCAT project, based on TwinCAT Project file (delivered with TwinCAT XAE)
             * ------------------------------------------------------ */
            _project = _solution.AddFromTemplate(TUT16_TCTEMPLATEPATH, TUT16_BASEFOLDER + TUT16_SOLUTIONFOLDER, TUT16_NAME);

            /* ------------------------------------------------------
             * Cast TwinCAT project object to ITcSysManager interface --> Automation Interface starts here
             * ------------------------------------------------------ */
            _systemManager = (ITcSysManager)_project.Object;
        }
    }
}
