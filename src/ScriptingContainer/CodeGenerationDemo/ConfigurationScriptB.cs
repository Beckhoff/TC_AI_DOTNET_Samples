using System;
using System.IO;
using EnvDTE;
using EnvDTE100;
using EnvDTE80;
using TCatSysManagerLib;
using TwinCAT.SystemManager;
using System.Diagnostics;
using System.Timers;
using ScriptingTest;
using System.Xml;
using System.Collections.Generic;
using System.Reflection;

namespace CodeGenerationDemo
{
    /// <summary>
    /// Demonstrates the generation + compilation of PLC projects
    /// </summary>
    public class ConfigurationScriptB
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

            ITcSmTreeItem device = CreateHardware(worker, optScanHardware, optSimulate);

            CreatePlcProject(worker);
            CreateMotion(worker);
            CreateMappings(worker);
        }
    }
}
