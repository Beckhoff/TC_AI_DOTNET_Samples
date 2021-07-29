using System;
using System.IO;
using EnvDTE;
using EnvDTE100;
using EnvDTE80;
using TCatSysManagerLib;
using System.Diagnostics;
using System.Timers;
using ScriptingTest;
using System.Xml;
using System.Collections.Generic;
using System.Reflection;

namespace CodeGenerationDemo
{
    /// <summary>
    /// Demonstrates the generation + compilation of PLC and Measurement projects
    /// </summary>
    public class ConfigurationScriptD
        : CodeGenerationBaseScript
    {
        /// <summary>
        /// Handler function Executing the Script code.
        /// </summary>
        /// <param name="worker">The worker.</param>
        protected override void OnExecute(IWorker worker)
        {
            worker.Progress = 0;

            bool optScanHardware = this._context.Parameters.ContainsKey("ScanHardware");
            bool optSimulate = this._context.Parameters.ContainsKey("SimulateHardware");

            ITcSmTreeItem ncConfig = systemManager.LookupTreeItem("TINC"); // Getting NC Configuration
            ITcSmTreeItem plcConfig = systemManager.LookupTreeItem("TIPC"); // Getting PLC-Configuration
            ITcSmTreeItem devices = systemManager.LookupTreeItem("TIID"); // Getting IO-Configuration

            ITcSmTreeItem plcProject = CreatePlcProject(worker);

            SetTaskCycleTime(10000, worker);

            worker.ProgressStatus = "Activating configuration ...";
            
            systemManager.ActivateConfiguration();

            worker.ProgressStatus = "Restarting TwinCAT ...";
            
            systemManager.StartRestartTwinCAT();

            System.Threading.Thread.Sleep(5000);

            CreateScopeProject(worker);
        }

        private void SetTaskCycleTime(int cycleTime, IWorker worker)
        {
            worker.ProgressStatus = "Setting task cycle time ...";
            string xmlCycleTime = "<TreeItem><TaskDef><CycleTime>" + cycleTime + "</CycleTime></TaskDef></TreeItem>";
            ITcSmTreeItem task = systemManager.LookupTreeItem("TIRT^PlcTask");
            task.ConsumeXml(xmlCycleTime);
        }

        private void CreateScopeProject(IWorker worker)
        {
            //TODO: Parse the Orders.xml !!!!

            string scopeTemplate = @"C:\TwinCAT\Functions\TE130X-Scope-View\Templates\Projects\Scope YT Project.tcmproj";
            string scopeDestination = this.ScriptRootFolder;
            string scopeName = "FunctionGeneratorScopeSample";

            worker.ProgressStatus = "Adding TwinCAT Measurement project ...";

            Project scopeProject = dte.Solution.AddFromTemplate(scopeTemplate, scopeDestination, scopeName);

            System.Threading.Thread.Sleep(3000);

            worker.ProgressStatus = "Opening Scope control ...";

            ((IMeasurementScope)scopeProject.ProjectItems.Item(1).Object).ShowControl();

            worker.ProgressStatus = "Creating new chart ...";

            ProjectItem ChartPI;
            ((IMeasurementScope)scopeProject.ProjectItems.Item(1).Object).CreateChild(out ChartPI);
            IMeasurementScope ChartMI = (IMeasurementScope)ChartPI.Object;
            ChartMI.ChangeName("Rectangle and Sinus Chart");
            setProperty("StackedYAxis", ChartPI.Properties, true);

            worker.ProgressStatus = "Creating axis for sinus signal ...";

            ProjectItem AxisPI1;
            ChartMI.CreateChild(out AxisPI1);
            IMeasurementScope AxisMI1 = (IMeasurementScope)AxisPI1.Object;
            AxisMI1.ChangeName("Axis 1");

            worker.ProgressStatus = "Creating axis for rectangle signal ...";

            ProjectItem AxisPI2;
            ChartMI.CreateChild(out AxisPI2);
            IMeasurementScope AxisMI2 = (IMeasurementScope)AxisPI2.Object;
            AxisMI2.ChangeName("Axis 2");

            worker.ProgressStatus = "Creating channel to PLC symbol for sinus signal  ...";

            ProjectItem ChannelPI1;
            AxisMI1.CreateChild(out ChannelPI1);
            IMeasurementScope ChannelMI1 = (IMeasurementScope)ChannelPI1.Object;

            ChannelMI1.ChangeName("Sinus");
            setProperty("TargetPorts", ChannelPI1.Properties, "851");
            setProperty("Symbolbased", ChannelPI1.Properties, true);
            setProperty("SymbolDataType", ChannelPI1.Properties, TwinCAT.Scope2.Communications.Scope2DataType.REAL64);
            setProperty("SymbolName", ChannelPI1.Properties, "MAIN.aSineBuffer");
            setProperty("SampleState", ChannelPI1.Properties, 1);
            setProperty("LineWidth", ChannelPI1.Properties, 3);

            worker.ProgressStatus = "Creating channel to PLC symbol for rectangle signal  ...";

            ProjectItem ChannelPI2;
            AxisMI2.CreateChild(out ChannelPI2);
            IMeasurementScope ChannelMI2 = (IMeasurementScope)ChannelPI2.Object;

            ChannelMI2.ChangeName("Rectangle");
            setProperty("TargetPorts", ChannelPI2.Properties, "851");
            setProperty("Symbolbased", ChannelPI2.Properties, true);
            setProperty("SymbolDataType", ChannelPI2.Properties, TwinCAT.Scope2.Communications.Scope2DataType.REAL64);
            setProperty("SymbolName", ChannelPI2.Properties, "MAIN.aSquareBuffer");
            setProperty("SampleState", ChannelPI2.Properties, 1);
            setProperty("LineWidth", ChannelPI2.Properties, 3);

            worker.ProgressStatus = "Starting Scope record  ...";

            ((IMeasurementScope)scopeProject.ProjectItems.Item(1).Object).StartRecord();

            System.Threading.Thread.Sleep(5000);

            worker.ProgressStatus = "Stopping Scope record  ...";

            ((IMeasurementScope)scopeProject.ProjectItems.Item(1).Object).StopRecord();
        }

        private void setProperty(string name, EnvDTE.Properties props, object value)
        {
            foreach (Property p in props)
                if (p.Name.ToLower().Equals(name.ToLower()))
                {
                    object oldvalue = p.Value;
                    p.Value = value;
                    return;
                }
        }
    }
}
