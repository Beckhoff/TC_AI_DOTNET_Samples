using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TCatSysManagerLib;
using System.IO;
using System.Xml;
using EnvDTE80;
using EnvDTE;
using EnvDTE100;
using System.Xml.Linq;
using ScriptingTest;

namespace Scripting.CSharp
{
    /// <summary>
    /// Demonstrates how to assign tasks to CPU cores!
    /// </summary>
    public class TaskCpuSettings
        : ScriptEarlyBound
    {
        /// <summary>
        /// Project
        /// </summary>
        Project project = null;

        /// <summary>
        /// System Manager
        /// </summary>
        ITcSysManager4 systemManager = null;

        /// <summary>
        /// Handler function Initializing the Script (Configuration preparations)
        /// </summary>
        /// <param name="context"></param>
        /// <remarks>Usually used to to the open a prepared or new XAE configuration</remarks>
        protected override void OnInitialize(IContext context)
        {
            base.OnInitialize(context);
        }

        /// <summary>
        /// Handler function called after the Solution object has been created.
        /// </summary>
        protected override void OnSolutionCreated()
        {
            this.project = (Project)CreateNewProject();
            this.systemManager = (ITcSysManager4)project.Object;
            base.OnSolutionCreated();
        }


        /// <summary>
        /// Cleaning up the XAE configuration after script execution.
        /// </summary>
        /// <param name="worker">The worker.</param>
        protected override void OnCleanUp(IWorker worker)
        {
            project.Save();
            project = null;
            base.OnCleanUp(worker);
        }

        /// <summary>
        /// Handler function Executing the Script code.
        /// </summary>
        /// <param name="worker">The worker.</param>
        protected override void OnExecute(IWorker worker)
        {
            worker.Progress = 0;

            ITcSmTreeItem systemConfiguration = systemManager.LookupTreeItem("TIRC"); // System
            ITcSmTreeItem realtimeConfiguration = systemManager.LookupTreeItem("TIRS"); // Realtime-Settings
            ITcSmTreeItem ncConfiguration = systemManager.LookupTreeItem("TINC"); // NC-Configuration
            ITcSmTreeItem plcConfiguration = systemManager.LookupTreeItem("TIPC"); // Plc-Configuration

            // CPU Settings

            //<TreeItem>
	        //    <RTimeSetDef>
		    //        <MaxCPUs>3</MaxCPUs>
		    //        <Affinity>#x0000000000000007</Affinity>
		    //        <CPUs>
			//            <CPU id="0">
			//	            <LoadLimit>10</LoadLimit>
			//	            <BaseTime>10000</BaseTime>
			//	            <LatencyWarning>200</LatencyWarning>
			//            </CPU>
			//            <CPU id="1">
			//	            <LoadLimit>20</LoadLimit>
			//	            <BaseTime>5000</BaseTime>
			//	            <LatencyWarning>500</LatencyWarning>
			//            </CPU>
			//            <CPU id="2">
			//	            <LoadLimit>30</LoadLimit>
			//	            <BaseTime>3333</BaseTime>
			//	            <LatencyWarning>1000</LatencyWarning>
			//            </CPU>
		    //        </CPUs>
	        //    </RTimeSetDef>
            //</TreeItem>

            worker.Progress = 10;
            worker.ProgressStatus = "Writing Realtime Settings ...";

            string xml = null;
            MemoryStream stream = new MemoryStream();

            StringWriter stringWriter = new StringWriter();

            using (XmlWriter writer = XmlTextWriter.Create(stringWriter))
            {
                writer.WriteStartElement("TreeItem");
                writer.WriteStartElement("RTimeSetDef");
                writer.WriteElementString("MaxCPUs", "4");

                string affinityString = string.Format("#x{0}",((ulong)CpuAffinity.MaskQuad).ToString("x16"));

                writer.WriteElementString("Affinity", affinityString);
                writer.WriteStartElement("CPUs");

                writeCpuProperties(writer, 0, 10, 1000, 200);
                writeCpuProperties(writer, 1, 20, 5000, 500);
                writeCpuProperties(writer, 2, 30, 3333, 1000);

                writer.WriteEndElement(); // CPUs
                writer.WriteEndElement(); // RTimeSetDef

                writer.WriteEndElement(); // TreeItem
            }

            xml = stringWriter.ToString();
            realtimeConfiguration.ConsumeXml(xml); // Set Parameters on RuntimesSettings item

            worker.Progress = 60;
            worker.ProgressStatus = "Creating Tasks ...";
            
            ITcSmTreeItem tasks = systemManager.LookupTreeItem("TIRT");

            // Create Tasks and paremetrize.
            ITcSmTreeItem additionalTask1 = tasks.CreateChild("TaskA",1);
            setTaskAffinity(additionalTask1,CpuAffinity.CPU1);
            ITcSmTreeItem additionalTask2 = tasks.CreateChild("TaskB",1);
            setTaskAffinity(additionalTask2, CpuAffinity.CPU2);
            ITcSmTreeItem additionalTask3 = tasks.CreateChild("TaskC", 1);
            setTaskAffinity(additionalTask3, CpuAffinity.CPU3);

            worker.Progress = 70;
            worker.ProgressStatus = "Setting task priorities ...";

            // Setting Priorities (This time with XElement as another Option for XmlCreation)

            //Auto Prio mangement off

            // <TreeItem>
            //      <RTimeSetDef>
            //          <AutoPrioManagement>0</AutoPrioManagement>
            //      </RTimeSetDef>
            // </TreeItem>

            ITcSmTreeItem ncTask = ncConfiguration.CreateChild("NC Task", 3);
            setTaskPriority(ncTask, 7);

            setTaskPriority(additionalTask1, 4);
            setTaskPriority(additionalTask2, 5);
            setTaskPriority(additionalTask3, 6);

            worker.Progress = 80;

            // Adding PLC Project
            string plcAxisTemplatePath = Path.Combine(ConfigurationTemplatesFolder, "PlcAxisTemplate.tpzip");
            worker.ProgressStatus = "Adding PLC Axis project ...";

            ITcSmTreeItem plc = plcConfiguration.CreateChild("PlcAxisSample", 0, "", plcAxisTemplatePath);

            worker.Progress = 90;

            // Setting the priority of the PlcTask
            // Sleep() might be necessary in older TwinCAT 3.1 releases
            System.Threading.Thread.Sleep(3000);
            ITcSmTreeItem task1 = systemManager.LookupTreeItem("TIRT^PlcTask");
            setTaskPriority(task1, 14);


            // Adding a new System manager Task
            ITcSmTreeItem task2 = tasks.CreateChild("MyPlcTask", (int)TREEITEMTYPES.TREEITEMTYPE_TASK);

            ITcSmTreeItem plcTask = systemManager.LookupTreeItem("TIPC^PlcAxisSample^PlcAxisSample Project^PlcTask");
            ITcPlcTaskReference plcTaskRef = (ITcPlcTaskReference)plcTask;

            // Storing old task setting
            string oldLinkedTask = plcTaskRef.LinkedTask;
            // Setting a new task
            plcTaskRef.LinkedTask = task2.PathName; 

            // Checking new task linking
            string newLinkedTask = plcTaskRef.LinkedTask;

            worker.Progress = 100;
        }

        /// <summary>
        /// Sets the task CPU affinity
        /// </summary>
        /// <param name="task">The task.</param>
        /// <param name="affinityMask">The affinity mask.</param>
        private void setTaskAffinity(ITcSmTreeItem task, CpuAffinity affinityMask)
        {
            // <TreeItem>
            //      <TaskDef>
            //          <CPUAffinity>#x0000000000000001</CPUAffinity>
            //      </TaskDef>
            // </TreeItem>

            StringWriter stringWriter = new StringWriter();

            using (XmlWriter writer = new XmlTextWriter(stringWriter))
            {
                writer.WriteStartElement("TreeItem");
                writer.WriteStartElement("TaskDef");

                string affinityString = string.Format("#x{0}", ((ulong)affinityMask).ToString("x16"));
                writer.WriteElementString("CpuAffinity",affinityString);

                writer.WriteEndElement();
                writer.WriteEndElement(); //TreeItem
            }
            string xml = stringWriter.ToString();
            task.ConsumeXml(xml);
        }

        /// <summary>
        /// Sets the task priority.
        /// </summary>
        /// <param name="task">The task.</param>
        /// <param name="priority">The priority.</param>
        private void setTaskPriority(ITcSmTreeItem task, int priority)
        {
            // <TreeItem>
            //      <TaskDef>
            //          <Priority>4</Priority>
            //      </TaskDef>
            // </TreeItem>

            StringWriter stringWriter = new StringWriter();

            using (XmlWriter writer = new XmlTextWriter(stringWriter))
            {
                writer.WriteStartElement("TreeItem");
                writer.WriteStartElement("TaskDef");
                writer.WriteElementString("Priority", priority.ToString());
                writer.WriteEndElement();
                writer.WriteEndElement(); //TreeItem
            }
            string xml = stringWriter.ToString();
            task.ConsumeXml(xml);
        }

        /// <summary>
        /// Writes the CPU properties to the XmlWriter
        /// </summary>
        /// <param name="writer">XmlWriter</param>
        /// <param name="id">CPU ID</param>
        /// <param name="loadLimit">LoadLimt (0..100)</param>
        /// <param name="baseTime">Base Time in Ticks (100ns)</param>
        /// <param name="latencyWarning">Latency warning in Ticks (100ns).</param>
        private void writeCpuProperties(XmlWriter writer, int id, int loadLimit, int baseTime, int latencyWarning)
        {
            writer.WriteStartElement("CPU");
            writer.WriteAttributeString("id", id.ToString());
            writer.WriteElementString("LoadLimit", loadLimit.ToString());
            writer.WriteElementString("BaseTime", baseTime.ToString());
            writer.WriteElementString("LatencyWarning", latencyWarning.ToString());
            writer.WriteEndElement();
        }

        /// <summary>
        /// Gets the Script description
        /// </summary>
        /// <value>The description.</value>
        public override string Description
        {
            get { return "Demonstrates how create and parametrize tasks and assigning them to CPU cores!"; }
        }

        /// <summary>
        /// Gets the keywords, describing the Script features
        /// </summary>
        /// <value>The keywords.</value>
        public override string Keywords
        {
            get
            {
                return "RealtimeSettings, CPU Cores, Task, Task-Priorities ";
            }
        }

        /// <summary>
        /// Gets the Version number of TwinCAT that is necessary for script execution.
        /// </summary>
        /// <value>The TwinCAT version.</value>
        public override Version TwinCATVersion
        {
            get
            {
                return new Version(3, 1);
            }
        }

        /// <summary>
        /// Gets the build number of TwinCAT that is necessary for script execution.
        /// </summary>
        /// <value>The TwinCAT build.</value>
        public override string TwinCATBuild
        {
            get
            {
                return "4020";
            }
        }

        /// <summary>
        /// Gets the category of this script.
        /// </summary>
        /// <value>The script category.</value>
        public override string Category
        {
            get
            {
                return "Basics";
            }
        }
    }

    /// <summary>
    /// Flags and Masks for the CPU Affinity
    /// </summary>
    [Flags()]
    public enum CpuAffinity : ulong
    {
        /// <summary>
        /// CPU 1
        /// </summary>
        CPU1 = 0x0000000000000001,
        /// <summary>
        /// CPU 2
        /// </summary>
        CPU2 = 0x0000000000000002,
        /// <summary>
        /// CPU 3
        /// </summary>
        CPU3 = 0x0000000000000004,
        /// <summary>
        /// CPU 4
        /// </summary>
        CPU4 = 0x0000000000000008,
        /// <summary>
        /// CPU 5
        /// </summary>
        CPU5 = 0x0000000000000010,
        /// <summary>
        /// CPU 6
        /// </summary>
        CPU6 = 0x0000000000000020,
        /// <summary>
        /// CPU 7
        /// </summary>
        CPU7 = 0x0000000000000040,
        /// <summary>
        /// CPU 8
        /// </summary>
        CPU8 = 0x0000000000000080,

        /// <summary>
        /// None, Uninitialized
        /// </summary>
        None = 0x0000000000000000,
        /// <summary>
        /// Single Core CPU
        /// </summary>
        MaskSingle = CPU1,
        /// <summary>
        /// Dual Core CPU
        /// </summary>
        MaskDual = CPU1 | CPU2,
        /// <summary>
        /// Quad Core CPU
        /// </summary>
        MaskQuad = MaskDual | CPU3 | CPU4,
        /// <summary>
        /// Hexa Core CPU
        /// </summary>
        MaskHexa = MaskQuad | CPU5 | CPU6,
        /// <summary>
        /// Oct Core CPU
        /// </summary>
        MaskOct = MaskHexa | CPU7 | CPU8,
        /// <summary>
        /// Mask All
        /// </summary>
        MaskAll = 0xFFFFFFFFFFFFFFFF
    }
}
