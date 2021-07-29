using System.IO;
using System.Xml;
using EnvDTE;
using EnvDTE100;
using EnvDTE80;
using TCatSysManagerLib;
using TwinCAT.SystemManager;
using System;
using ScriptingTest;

namespace Scripting.CSharp
{
    /// <summary>
    /// Demonstrates the creation of an EtherCAT IO Subtree and the linking with PLC Symbols (Early Binding), Alternative Box adding via VInfo Structe
    /// </summary>
    public class EtherCATLinking2
        : ScriptEarlyBound
    {

        /// <summary>
        /// System Manager object
        /// </summary>
        private ITcSysManager4 systemManager = null;

        /// <summary>
        /// TwinCAT XAE Project ojbect
        /// </summary>
        private Project project = null;

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
            base.OnCleanUp(worker);
        }

        /// <summary>
        /// Name of the PLC Template
        /// </summary>
        private string plcTemplateName = "PlcFile.tpy";

        /// <summary>
        /// Name of the Project Template
        /// </summary>
        private string xaeProjectTemplateName = "DemoProject.tsp";

        /// <summary>
        /// Gets the Path to the PLC Template
        /// </summary>
        /// <value>The PLC file.</value>
        private string PlcTemplatePath
        {
            get { return Path.Combine(ApplicationDirectory, plcTemplateName); }
        }

        /// <summary>
        /// Gets the Path to the Project Template
        /// </summary>
        /// <value>The TSM file.</value>
        private string XAEProjectTemplatePath
        {
            get { return Path.Combine(ApplicationDirectory, xaeProjectTemplateName); }
        }


        /// <summary>
        /// Handler function Executing the Script code.
        /// </summary>
        /// <param name="worker">The worker.</param>
        protected override void OnExecute(IWorker worker)
        {
            //TODO: Integrate sample how to select specific ports

            /*set ek1100 = tsm.LookupTreeItem("TIID^Device 1 (EtherCAT)^Term 1 (EK1100)")
            call ek1100.CreateChild("Term1(El2004)", BOXTYPE_EXXXXX, "", "EL2004")

            set ek1122 = ek1100.CreateChild("Term2(EK1122)", BOXTYPE_EXXXXX, "", "EK1122")

            set ek1100_1 = ek1122.CreateChild("Term3(EK1100)", BOXTYPE_EXXXXX, "@D", "EK1100")
            call ek1100_1.CreateChild("Term4(El2008)", BOXTYPE_EXXXXX, "", "EL2008")
            call ek1100_1.CreateChild("Term5(El9011)", BOXTYPE_EXXXXX, "", "EL9011")

            set ek1100_2 = ek1122.CreateChild("Term6(EK1100)", BOXTYPE_EXXXXX, "@B", "EK1100")
            call ek1100_2.CreateChild("Term7(El1008)", BOXTYPE_EXXXXX, "", "EL1008")
            call ek1100_2.CreateChild("Term8(El9011)", BOXTYPE_EXXXXX, "", "EL9011")

            call ek1100.CreateChild("Term9(El2004)", BOXTYPE_EXXXXX, "", "EL2004")
            call ek1100.CreateChild("Term10(El9011)", BOXTYPE_EXXXXX, "", "EL9011") */

            worker.Progress = 0;

            string XmlStr;
            int El6731DevId;
            int El6751DevId;

            int[] vInfo = new int[4];
            vInfo[0] = 2; // vendorId Beckhoff
            vInfo[1] = 0; // productCode
            vInfo[2] = 0; // revision, only used for EL6731
            vInfo[3] = 0; // serial number

            //systemManager.OpenConfiguration(this.XAEProjectTemplatePath);

            //NC configuration
            //search for NC configuration
            ITcSmTreeItem ncConfig = systemManager.LookupTreeItem("TINC");
            //create NC Task
            ITcSmTreeItem task1 = ncConfig.CreateChild("NC-Task 1", 1);
            //search for NC Axes Group
            ITcSmTreeItem axes = systemManager.LookupTreeItem("TINC^NC-Task 1 SAF^Axes");
            //create NC Axis
            ITcSmTreeItem axis1 = axes.CreateChild("Axis 1", 1);

            //IO configuration
            //search for IO Devices
            ITcSmTreeItem devices = systemManager.LookupTreeItem("TIID");
            
            
            //create EtherCAT-Master
            // Scans the Fieldbus interfaces and adds an EtherCAT Device.
            ITcSmTreeItem device = Helper.CreateEthernetDevice(this.systemManager, DeviceType.EtherCAT_DirectMode, "EtherCAT Master", worker);

            //ITcSmTreeItem device = devices.CreateChild("EtherCAT Master", 94);
            //search for EtherCAT-Master

            //device.ConsumeXml("<TreeItem><DeviceDef><AddressInfo><Pnp><DeviceDesc>Local Area Connection 2</DeviceDesc></Pnp></AddressInfo></DeviceDef></TreeItem>");

            //// Alternativly
            //XElement xTreeItem = new XElement("TreeItem");
            //XElement xDeviceDev = new XElement("DeviceDev");
            //XElement xAddressInfo = new XElement("AddressInfo");
            //XElement xPnp = new XElement("Pnp");
            //XElement xDeviceDescr = new XElement("DeviceDescr",new XText("LocalAreaConnection 2");

            //xPnp.Add(xDeviceDescr);
            //xAddressInfo.Add(xPnp);
            //xDeviceDev.Add(xAddressInfo);
            //xTreeItem.Add(xDeviceDev);
            //device.ConsumeXml(xTreeItem.ToString(SaveOptions.None));

            // TreeItem.DeviceDef.AddressInfo.Pnp.DeviceDesc = "Local Area Connection 2"; // Dieser Syntax???

            //create EK1100 A2P
            worker.Progress = 10;
            worker.ProgressStatus = "Creating A2P (EK1100)";

            vInfo[1] = 72100946; //productCode EK1100
            vInfo[2] = 65536;    //revision 0000-0001
            ITcSmTreeItem a2p = device.CreateChild("A2P (EK1100)", (int)BoxType.EtherCAT_EXXXXX, "", (object)vInfo);

            //create Terminals
            vInfo[1] = 66465874; //productCode EL1014
            vInfo[2] = 0;        //revision 0000-0000
            ITcSmTreeItem terminal = device.CreateChild("100 (EL1014)", (int)BoxType.EtherCAT_EXXXXX, "", (object)vInfo);

            vInfo[1] = 616050768; //productCode EL9400
            vInfo[2] = 0;        //revision 0000-0000
            terminal = device.CreateChild("101 (EL9400)", (int)BoxType.EtherCAT_EXXXXX, "", (object)vInfo);

            vInfo[1] = 131346514; //productCode EL2004
            vInfo[2] = 0;        //revision 0000-0000
            terminal = device.CreateChild("102 (EL2004)", (int)BoxType.EtherCAT_EXXXXX, "", (object)vInfo);

            vInfo[1] = 596389968; //productCode EL9100
            vInfo[2] = 0;        //revision 0000-0000
            terminal = device.CreateChild("103 (EL9100)", (int)BoxType.EtherCAT_EXXXXX, "", (object)vInfo);

            vInfo[1] = 131346514; //productCode EL2004
            vInfo[2] = 0;        //revision 0000-0000
            terminal = device.CreateChild("104 (EL2004)", (int)BoxType.EtherCAT_EXXXXX, "", (object)vInfo);

            vInfo[1] = 131346514; //productCode EL2004
            vInfo[2] = 0;        //revision 0000-0000
            terminal = device.CreateChild("105 (EL2004)", (int)BoxType.EtherCAT_EXXXXX, "", (object)vInfo);

            vInfo[1] = 131346514; //productCode EL2004
            vInfo[2] = 0;        //revision 0000-0000
            terminal = device.CreateChild("106 (EL2004)", (int)BoxType.EtherCAT_EXXXXX, "", (object)vInfo);

            vInfo[1] = 131346514; //productCode EL2004
            vInfo[2] = 0;        //revision 0000-0000
            terminal = device.CreateChild("107 (EL2004)", (int)BoxType.EtherCAT_EXXXXX, "", (object)vInfo);

            vInfo[1] = 131346514; //productCode EL2004
            vInfo[2] = 0;        //revision 0000-0000
            terminal = device.CreateChild("108 (EL2004)", (int)BoxType.EtherCAT_EXXXXX, "", (object)vInfo);

            vInfo[1] = 65810514; //productCode EL1004
            vInfo[2] = 0;        //revision 0000-0000
            terminal = device.CreateChild("109 (EL1004)", (int)BoxType.EtherCAT_EXXXXX, "", (object)vInfo);

            vInfo[1] = 66465874; //productCode EL1014
            vInfo[2] = 0;        //revision 0000-0000
            terminal = device.CreateChild("110 (EL1014)", (int)BoxType.EtherCAT_EXXXXX, "", (object)vInfo);

            vInfo[1] = 72756306; //productCode EL1110
            vInfo[2] = 0;        //revision 0000-0000
            terminal = device.CreateChild("111 (EK1110)", (int)BoxType.EtherCAT_EXXXXX, "", (object)vInfo);


            //create EK1100 A3P
            worker.Progress = 20; 
            worker.ProgressStatus = "Creating A3P (EK1100)";

            //search for EtherCAT Master
            vInfo[1] = 72100946; //productCode EK1100
            vInfo[2] = 0;        //revision 0000-0000
            terminal = device.CreateChild("A3P (EK1100)", (int)BoxType.EtherCAT_EXXXXX, "", (object)vInfo);

            //create Terminals
            vInfo[1] = 442445906; //productCode EL6751
            vInfo[2] = 0;         //revision 0000-0000
            terminal = device.CreateChild("204 (EL6751)", (int)BoxType.EtherCAT_EXXXXX, "", (object)vInfo);

            vInfo[1] = 441135186; //productCode EL6731
            //vInfo[2] = 655032320 //revision 0000-9995
            vInfo[2] = 0;        //revision 0000-0000
            terminal = device.CreateChild("205 (EL6731)", (int)BoxType.EtherCAT_EXXXXX, "", (object)vInfo);

            vInfo[1] = 590491728; //productCode EL9010
            vInfo[2] = 0;         //revision 0000-0000
            terminal = device.CreateChild("206 (EL9010)", (int)BoxType.EtherCAT_EXXXXX, "", (object)vInfo);

            //create BK1120 A4P
            vInfo[1] = 73411618;  //productCode BK1120
            vInfo[2] = 655032320; //revision 0000-9995
            //vInfo[2] = 0        //revision 0000-0000
            terminal = device.CreateChild("A4P (BK1120)", (int)BoxType.EtherCAT_EXXXXX, "", (object)vInfo);

            //create terminals
            ITcSmTreeItem a4p = systemManager.LookupTreeItem("TIID^EtherCAT Master^A4P (BK1120)");
            terminal = a4p.CreateChild("Term3 (KL2114)", 2114, "End Term (KL9010)");
            terminal = a4p.CreateChild("Term4 (KL1104)", 1104, "End Term (KL9010)");
            terminal = a4p.CreateChild("Term5 (KL1104)", 1104, "End Term (KL9010)");
            terminal = a4p.CreateChild("Term6 (KL1408)", 1408, "End Term (KL9010)");
            terminal = a4p.CreateChild("Term7 (KL1408)", 1408, "End Term (KL9010)");

            //create CANopen Master Device
            //create Profibus Master
            worker.ProgressStatus = "Creating CANopen Master (EL6751)";
            ITcSmTreeItem canMaster = devices.CreateChild("CANopen Master (EL6751)", 87);

            //search for the Device ID of the EL6751
            ITcSmTreeItem item = systemManager.LookupTreeItem("TIID^EtherCAT Master^A3P (EK1100)^204 (EL6751)");
            XmlStr = item.ProduceXml(false);

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(XmlStr);
            El6751DevId = 0;

            //search for the Device ID of the EL6751
            foreach (XmlNode childNode in xmlDoc.ChildNodes)
            {
                XmlNode currNode = childNode;
                if (currNode.Name == "ItemId")
                {
                    El6751DevId = int.Parse(currNode.Value);
                    break;
                }
            }

            //set the EL6751 Device ID as address in CANopen Master Device
            worker.Progress = 40; 
            worker.ProgressStatus = "Creating CANopen Master (EL6751)";

            item = systemManager.LookupTreeItem("TIID^CANopen Master (EL6751)");
            item.ConsumeXml("<TreeItem><DeviceDef><AddressInfo><Ecat><EtherCATDeviceId>" + El6751DevId.ToString() + "</EtherCATDeviceId></Ecat></AddressInfo></DeviceDef></TreeItem>");

            //create CANopen Slaves
            //search for CANopen Master
            item = systemManager.LookupTreeItem("TIID^CANopen Master (EL6751)");

            //create BK5150
            item.CreateChild("Box11 (BK5150)", 5008);
            //set node address
            item = systemManager.LookupTreeItem("TIID^CANopen Master (EL6751)^Box11 (BK5150)");
            item.ConsumeXml("<TreeItem><BoxDef><FieldbusAddress>11</FieldbusAddress></BoxDef></TreeItem>");
            //create terminals
            item.CreateChild("Term2 (KL1002)", 1002, "End Term (KL9010)");
            item.CreateChild("Term3 (KL2114)", 2114, "End Term (KL9010)");


            //create Profibus Master Device
            //search for IO Devices
            item = systemManager.LookupTreeItem("TIID");

            //create Profibus Master
            worker.ProgressStatus = "Creating Profibus Master (EL6731)";
            worker.Progress = 50; 

            item.CreateChild("Profibus Master (EL6731)", 86);

            //search button functionality of Profibus Master device
            //search for the Device ID of the EL6731
            item = systemManager.LookupTreeItem("TIID^EtherCAT Master^A3P (EK1100)^205 (EL6731)");
            XmlStr = item.ProduceXml(false);
            xmlDoc.LoadXml(XmlStr);
            El6731DevId = 0;

            //search for the Device ID of the EL6731
            foreach (XmlNode childNode in xmlDoc.ChildNodes)
            {
                XmlNode currNode = childNode;
                if (currNode.Name == "ItemId")
                {
                    El6731DevId = int.Parse(currNode.Value);
                    break;
                }
            }

            //set the EL6731 Device ID as address in Profibus Master Device
            item = systemManager.LookupTreeItem("TIID^Profibus Master (EL6731)");
            item.ConsumeXml("<TreeItem><DeviceDef><AddressInfo><Ecat><EtherCATDeviceId>" + El6731DevId.ToString() + "</EtherCATDeviceId></Ecat></AddressInfo></DeviceDef></TreeItem>");

            //create Profibus Slaves
            //search for Profibus Master
            item = systemManager.LookupTreeItem("TIID^Profibus Master (EL6731)");
            //create Drive
            item.CreateChild("Screw A (AXIS_Screw_A_FC310x)", 1008);
            //        //create Rod
            //        nErr = item.CreateChild("MTS Rod 1: Injection A and Carriage A (MTS2)", 1003, "", "C:\TwinCAT\Io\Profibus\MTSE04C3.GSD")
            //        //create TcPlug
            //        nErr = item.CreateChild("Injection A Heats (TCPLUG)", 1003, "", "C:\TwinCAT\Io\Profibus\TcPlug.gsd")

            //create BK3120
            item.CreateChild("Box22 (BK3120)", 1010);
            //set Profibus DP-Address
            item = systemManager.LookupTreeItem("TIID^Profibus Master (EL6731)^Box22 (BK3120)");
            item.ConsumeXml("<TreeItem><BoxDef><FieldbusAddress>22</FieldbusAddress></BoxDef></TreeItem>");
            //create terminals
            item.CreateChild("Term5 (KL2114)", 2114, "End Term (KL9010)");

            //set profibus DP-Address + disable
            item = systemManager.LookupTreeItem("TIID^Profibus Master (EL6731)^Screw A (AXIS_Screw_A_FC310x)");
            item.ConsumeXml("<TreeItem><BoxDef><FieldbusAddress>2</FieldbusAddress></BoxDef></TreeItem>");
            item.ConsumeXml("<TreeItem><Disabled>1</Disabled></TreeItem>");


            //create Virtual USB Master Device
            //search for IO Devices
            item = systemManager.LookupTreeItem("TIID");
            
            worker.Progress = 60; 
            worker.ProgressStatus = "Creating Profibus Master (EL6731)";

            item.CreateChild("Profibus Master (EL6731)", 86);

            //create Profibus Master
            item.CreateChild("Virtual USB Interface (USB)", 57);
            //set virtual device name
            item = systemManager.LookupTreeItem("TIID^Virtual USB Interface (USB)");
            item.ConsumeXml("<TreeItem><USB><VirtualDeviceNames>1</VirtualDeviceNames></USB></TreeItem>");

            //create USB Box
            item.CreateChild("Box 0 (CPX8XX)", 9591);

            ////PLC configuration
            ////search for PLC project
            //item = systemManager.LookupTreeItem("TIPC^PlcFile");
            ////rescan plc project
            //item.ConsumeXml("<TreeItem><PlcDef><ProjectPath>" + this.PlcTemplatePath + "</ProjectPath><ReScan>1</ReScan></PlcDef></TreeItem>");

            string plcAxisTemplatePath = Path.Combine(ConfigurationTemplatesFolder, "PlcAxisTemplate.tpzip");

            worker.Progress = 70; 
            worker.ProgressStatus = "Adding PLC Axis project ...";

            ITcSmTreeItem plcConfig = systemManager.LookupTreeItem("TIPC");
            ITcSmTreeItem plc = plcConfig.CreateChild("PlcProject", 0, "", plcAxisTemplatePath);

            ITcSmTreeItem plcProject = systemManager.LookupTreeItem("TIPC^PlcProject");
            ITcSmTreeItem plcInstances = systemManager.LookupTreeItem("TIPC^PlcProject^PlcProject Project");

            //create links
            //link nc axis to drive
            //search for nc axis
            item = systemManager.LookupTreeItem("TINC^NC-Task 1 SAF^Axes^Axis 1");
            //link to drive
            item.ConsumeXml("<TreeItem><NcAxisDef><AxisType>1</AxisType><AxisIoType>ProfibusMC</AxisIoType><IoItem><PathName>TIID^Profibus Master (EL6731)^Screw A (AXIS_Screw_A_FC310x)</PathName></IoItem></NcAxisDef></TreeItem>");

            worker.Progress = 90; 
            worker.ProgressStatus = "Linking variables ...";

            //link terminal channel to plc variable
            systemManager.LinkVariables("TIPC^PlcProject^PlcProject Instance^PlcTask Inputs^MAIN.bIn", "TIID^EtherCAT Master^A2P (EK1100)^100 (EL1014)^Channel 1^Input");
            //link terminal channel to plc variable
            systemManager.LinkVariables("TIPC^PlcProject^PlcProject Instance^PlcTask Outputs^MAIN.bOut", "TIID^EtherCAT Master^A2P (EK1100)^102 (EL2004)^Channel 1^Output");
            //link terminal channel to plc variable
            systemManager.LinkVariables("TIPC^PlcProject^PlcProject Instance^PlcTask Outputs^MAIN.bOut", "TIID^EtherCAT Master^A4P (BK1120)^Term3 (KL2114)^Channel 1^Output");
            //link terminal channel to plc variable
            systemManager.LinkVariables("TIPC^PlcProject^PlcProject Instance^PlcTask Outputs^MAIN.bOut", "TIID^CANopen Master (EL6751)^Box11 (BK5150)^Term3 (KL2114)^Channel 1^Output");
            //link terminal channel to plc variable
            systemManager.LinkVariables("TIPC^PlcProject^PlcProject Instance^PlcTask Outputs^MAIN.bOut", "TIID^Profibus Master (EL6731)^Box22 (BK3120)^Term5 (KL2114)^Channel 1^Output");
            //link Control Panel LED to plc variable
            systemManager.LinkVariables("TIPC^PlcProject^PlcProject Instance^PlcTask Outputs^MAIN.bOut", "TIID^Virtual USB Interface (USB)^Box 0 (CPX8XX)^Outputs^LED 1");
        }

        /// <summary>
        /// Gets the Script description
        /// </summary>
        /// <value>The description.</value>
        public override string Description
        {
            get
            {
                return "Demonstrates an alternative way of the creation of an\nEtherCAT IO Subtree with the more specific vInfo Structure\n(see also Script 'EtherCATLinking')";
            }
        }

        /// <summary>
        /// Gets the keywords, describing the Script features
        /// </summary>
        /// <value>The keywords.</value>
        public override string Keywords
        {
            get
            {
                return "EtherCAT, BoxCreation via VInfo, IO-PLC Mapping,\nScanning Devices, PLC rescan";
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
                return "3100";
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
