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
using TwinCAT.SystemManager;
using System.Xml.Linq;
using ScriptingTest;

namespace Scripting.CSharp
{
    /// <summary>
    /// Demonstrates how to assign tasks to CPU cores!
    /// </summary>
    public class Ax5000Drive
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

            /* =================================================================================================
             * Navigate to IO node and create new EtherCAT Master device
             * ================================================================================================= */
            ITcSmTreeItem deviceSettings = systemManager.LookupTreeItem("TIID");
            ITcSmTreeItem device = Helper.CreateEthernetDevice(systemManager, DeviceType.EtherCAT_DirectMode, "EtherCAT Master", worker);

            /* =================================================================================================
            * Example for binding Encoders and Drives
            * ================================================================================================= */
            ITcSmTreeItem ek1100 = device.CreateChild("EK1100", 0, "", "EK1100");
            ITcSmTreeItem el7332 = ek1100.CreateChild("EL7332", 0, "", "EL7332");
            ITcSmTreeItem el5101_1 = ek1100.CreateChild("EL5101-1", 0, "", "EL5101");
            ITcSmTreeItem el5101_2 =ek1100.CreateChild("EL5101-2", 0, "", "EL5101");
            ITcSmTreeItem el7342 = ek1100.CreateChild("EL7342", 0, "", "EL7342");

            ITcSmTreeItem motion = systemManager.LookupTreeItem("TINC");

            ITcSmTreeItem ncTask = motion.CreateChild("NC-Task SAF");
            ITcSmTreeItem axes = ncTask.LookupChild("Axes");

            ITcSmTreeItem axis1 = axes.CreateChild("Axis1",1);
            ITcSmTreeItem axis2 = axes.CreateChild("Axis2", 1, "", null);
            ITcSmTreeItem axis3 = axes.CreateChild("Axis3", 1, "", null);
            ITcSmTreeItem axis4 = axes.CreateChild("Axis4", 1, "", null);


            /* =================================================================================================
             * Create new AX5000 and read default parameters via ProduceXml
             * ================================================================================================= */
            ITcSmTreeItem ax5000Box = device.CreateChild("Ax5000",(int)BoxType.EtherCAT_EXXXXX, string.Empty, "AX5201-0000-0011");
            string sourceXml = ax5000Box.ProduceXml(false);

            /* =================================================================================================
             * Store default parameters in XmlDocument object for easier handling
             * ================================================================================================= */
            XmlDocument sourceDoc = new XmlDocument();
            sourceDoc.LoadXml(sourceXml);

            /* =================================================================================================
             * We only have to restore the InitCmds that are not "fixed". The Fixed ones will be overwritten and recreated automatically and are "ReadOnly".
             * For simplicity reasons we could consume the "Fixed" ones also - they don't have any effect, but for correctness we define the XPath here exactly.
             * ================================================================================================= */
            XmlNodeList sourceCommands = sourceDoc.SelectNodes("/TreeItem/EtherCAT/Slave/Mailbox/SoE/InitCmds/InitCmd[not(@Fixed) or not(@Fixed='true' or @Fixed>=1)]");

            XmlDocument targetDoc = new XmlDocument();
            targetDoc.LoadXml("<TreeItem><EtherCAT><Slave><Mailbox DataLinkLayer=\"true\"><SoE><InitCmds></InitCmds></SoE></Mailbox></Slave></EtherCAT></TreeItem>");

            XmlNode targetCommands = targetDoc.SelectSingleNode("/TreeItem/EtherCAT/Slave/Mailbox/SoE/InitCmds");
            XmlNode targetNode = null;

            foreach (XmlNode sourceNode in sourceCommands)
            {
                targetNode = targetDoc.ImportNode(sourceNode, true);
                targetCommands.AppendChild(targetNode);
            }

            targetNode = targetDoc.CreateElement("InitCmd");
            targetNode.InnerXml = "<Transition>PS</Transition><Comment><![CDATA[Motor brake]]></Comment><Timeout>0</Timeout><OpCode>3</OpCode><DriveNo>1</DriveNo><IDN>32828</IDN><Elements>64</Elements><Attribute>0</Attribute><Data>1100</Data>";
            targetCommands.AppendChild(targetNode);

            string targetXml = targetDoc.OuterXml;
            ax5000Box.ConsumeXml(targetXml);

            string syncUnitName = "SyncUnit1";

            // Setting SyncUnit (Via XmlWriter)

            //<TreeItem>
            //    <EtherCAT>
            //        <Slave>
            //            <SyncUnits>
            //                <SyncUnit>SyncUnit1</SyncUnit>
            //            </SyncUnits>
            //        </Slave>
            //    </EtherCAT>
            //</TreeItem>
            using (StringWriter writer = new StringWriter())
            {
                using (XmlTextWriter xmlWriter = new XmlTextWriter(writer))
                {
                    xmlWriter.WriteStartElement("TreeItem");
                    xmlWriter.WriteStartElement("EtherCAT");
                    xmlWriter.WriteStartElement("Slave");
                    xmlWriter.WriteStartElement("SyncUnits");

                    xmlWriter.WriteElementString("SyncUnit", syncUnitName);

                    xmlWriter.WriteEndElement();
                    xmlWriter.WriteEndElement();
                    xmlWriter.WriteEndElement();
                    xmlWriter.WriteEndElement();

                    ax5000Box.ConsumeXml(writer.ToString());
                }
            }

            

        }

        private void setSyncUnit(ITcSmTreeItem etherCATSlave, string syncUnitName)
        {
           
        }

        /// <summary>
        /// Gets the Script description
        /// </summary>
        /// <value>The description.</value>
        public override string Description
        {
            get { return "Creation of an EtherCAT AX5000 Device and setting of Mailbox Parameters"; }
        }

        /// <summary>
        /// Gets the keywords, describing the Script features
        /// </summary>
        /// <value>The keywords.</value>
        public override string Keywords
        {
            get
            {
                return "AX5000, Mailbox Init Commands, SyncUnits";
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
                return new Version(3, 0);
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
                return "I/O";
            }
        }
    }
}
