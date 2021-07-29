using System;
using System.IO;
using ScriptingTest;
using TwinCAT.SystemManager;

namespace ScriptingTest.LateBinding
{
    /// <summary>
    /// Demonstrates the creation of an EtherCAT IO Subtree and the linking with PLC Symbols (Late Binding).
    /// </summary>
    public class EtherCATLinking
        : ScriptLateBound
    {
        /// <summary>
        /// The system manager
        /// </summary>
        private dynamic systemManager = null;
        /// <summary>
        /// The current Visual Studio project.
        /// </summary>
        private dynamic project = null;

        /// <summary>
        /// Handler function Initializing the Script (Configuration preparations)
        /// </summary>
        /// <param name="context">The Script Context</param>
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
            this.project = (dynamic)CreateNewProject();
            this.systemManager = project.Object;
            base.OnSolutionCreated();
        }

        /// <summary>
        /// Cleaning up the XAE configuration after script execution.
        /// </summary>
        /// <param name="worker">The worker.</param>
        protected override void OnCleanUp(IWorker worker)
        {
        }

        /// <summary>
        /// Handler function Executing the Script code.
        /// </summary>
        /// <param name="worker">The worker.</param>
        protected override void OnExecute(IWorker worker)
        {
            worker.Progress = 0;
            string netId = systemManager.GetTargetNetId();

            dynamic ncConfig = systemManager.LookupTreeItem("TINC"); // Getting NC Configuration
            dynamic ncTask1 = ncConfig.CreateChild("NC-Task 1", 1); // Creating NC TAsk
            dynamic ncAxes = systemManager.LookupTreeItem("TINC^NC-Task 1 SAF^Axes"); // Getting Axes Folder
            dynamic ncAxis1 = ncAxes.CreateChild("Axis 1", 1); // Create Axis
            dynamic plcConfig = systemManager.LookupTreeItem("TIPC"); // Getting PLC-Configuration
            dynamic devices = systemManager.LookupTreeItem("TIID"); // Getting IO-Configuration

            ////create EtherCAT-Master
            //dynamic device = devices.CreateChild("EtherCAT Master", 94);

            ////Device Parametrization for "Local Area Connection 2"
            //device.ConsumeXml("<TreeItem><DeviceDef><AddressInfo><Pnp><DeviceDesc>Local Area Connection 2</DeviceDesc></Pnp></AddressInfo></DeviceDef></TreeItem>");

            dynamic device = Helper.CreateEthernetDevice(systemManager, DeviceType.EtherCAT_DirectMode, "EtherCAT Master", worker);

            worker.Progress = 10;
            worker.ProgressStatus = "Creating A2P (EK1100) ...";

            //create EK1100 A2P
            dynamic a2p = device.CreateChild("A2P (EK1100)", (int)BoxType.EtherCAT_EXXXXX, "", "EK1100-0000-0001");
            //create Terminals
            device.CreateChild("100 (EL1014)", (int)BoxType.EtherCAT_EXXXXX, "", "EL1014-0000-0000");
            device.CreateChild("101 (EL9400)", (int)BoxType.EtherCAT_EXXXXX, "", "EL9400");
            device.CreateChild("102 (EL2004)", (int)BoxType.EtherCAT_EXXXXX, "", "EL2004-0000-0000");
            device.CreateChild("103 (EL9100)", (int)BoxType.EtherCAT_EXXXXX, "", "EL9100");
            device.CreateChild("104 (EL2004)", (int)BoxType.EtherCAT_EXXXXX, "", "EL2004-0000-0000");
            device.CreateChild("105 (EL2004)", (int)BoxType.EtherCAT_EXXXXX, "", "EL2004-0000-0000");
            device.CreateChild("106 (EL2004)", (int)BoxType.EtherCAT_EXXXXX, "", "EL2004-0000-0000");
            device.CreateChild("107 (EL2004)", (int)BoxType.EtherCAT_EXXXXX, "", "EL2004-0000-0000");
            device.CreateChild("108 (EL2004)", (int)BoxType.EtherCAT_EXXXXX, "", "EL2004-0000-0000");
            device.CreateChild("109 (EL1004)", (int)BoxType.EtherCAT_EXXXXX, "", "EL1004-0000-0000");
            device.CreateChild("110 (EL1014)", (int)BoxType.EtherCAT_EXXXXX, "", "EL1014-0000-0000");
            device.CreateChild("111 (EK1110)", (int)BoxType.EtherCAT_EXXXXX, "", "EK1110-0000-0000");

            worker.Progress = 25;
            worker.ProgressStatus = "Creating A3P (EK1100) ...";

            //create EK1100 A3P
            dynamic a3p = device.CreateChild("A3P (EK1100)", (int)BoxType.EtherCAT_EXXXXX, "", "EK1100-0000-0000");
            //create Terminals
            a3p.CreateChild("204 (EL6751)", (int)BoxType.EtherCAT_EXXXXX, "", "EL6751-0000-0000");
            a3p.CreateChild("205 (EL6731)", (int)BoxType.EtherCAT_EXXXXX, "", "EL6731-0000-0000");
            a3p.CreateChild("206 (EL9010)", (int)BoxType.EtherCAT_EXXXXX, "", "EL9011");

            //create BK1120 A4P
            dynamic a4p = device.CreateChild("A4P (BK1120)", (int)BoxType.EtherCAT_EXXXXX, "", "BK1120-0000-9995");
            //create terminals
            a4p.CreateChild("Term3 (KL2114)", 2114, "End Term (KL9010)");
            a4p.CreateChild("Term4 (KL1104)", 1104, "End Term (KL9010)");
            a4p.CreateChild("Term5 (KL1104)", 1104, "End Term (KL9010)");
            a4p.CreateChild("Term6 (KL1408)", 1408, "End Term (KL9010)");
            a4p.CreateChild("Term7 (KL1408)", 1408, "End Term (KL9010)");

            //create CANopen Master Device
            //create Profibus Master
            dynamic canOpenMaster = devices.CreateChild("CANopen Master (EL6751)", 87);

            //search button functionality of Profibus Master device
            // set the EtherCATDeviceName to the path of the EL6751-Terminal (known from above, separated by //^//)
            //  I/O Configuration      = "TIID"
            //  + EtherCAT-Master name = "EtherCAT Master"
            //  + EK1100 coupler name  = "A2P (EK1100)"
            //  + EL6751 name          = "204 (EL6751)"
            //  EL6751 path = "TIID^EtherCAT Master^A3P (EK1100)^204 (EL6751)" is the combination from
            canOpenMaster.ConsumeXml("<TreeItem><DeviceDef><AddressInfo><Ecat><EtherCATDeviceName>TIID^EtherCAT Master^A3P (EK1100)^204 (EL6751)</EtherCATDeviceName></Ecat></AddressInfo></DeviceDef></TreeItem>");

            //create CANopen Slaves

            //create BK5150
            dynamic box11 = canOpenMaster.CreateChild("Box11 (BK5150)", 5008);
            ////set node address
            box11.ConsumeXml("<TreeItem><BoxDef><FieldbusAddress>11</FieldbusAddress></BoxDef></TreeItem>");
            //create terminals
            box11.CreateChild("Term2 (KL1002)", 1002, "End Term (KL9010)");
            box11.CreateChild("Term3 (KL2114)", 2114, "End Term (KL9010)");

            worker.Progress = 50;
            worker.ProgressStatus = "Creating Profibus Master (EL6731) ...";


            //create Profibus Master Device
            //create Profibus Master
            dynamic profibusMaster = devices.CreateChild("Profibus Master (EL6731)", 86);

            ////assign Profibus Master device to EL6731 terminal
            //// set the EtherCATDeviceName to the path of the EL6731-Terminal (known from above, separated by //^//)
            ////  I/O Configuration      = "TIID"
            ////  + EtherCAT-Master name = "EtherCAT Master"
            ////  + EK1100 coupler name  = "A2P (EK1100)"
            ////  + EL6731 name          = "205 (EL6731)"
            ////  EL6731 path = "TIID^EtherCAT Master^A3P (EK1100)^205 (EL6731)" is the combination from
            profibusMaster.ConsumeXml("<TreeItem><DeviceDef><AddressInfo><Ecat><EtherCATDeviceName>TIID^EtherCAT Master^A3P (EK1100)^205 (EL6731)</EtherCATDeviceName></Ecat></AddressInfo></DeviceDef></TreeItem>");

            //create Profibus Slaves

            //create Drive
            profibusMaster.CreateChild("Screw A (AXIS_Screw_A_FC310x)", 1008);
            //create Rod
            //       nErr = item.CreateChild("MTS Rod 1: Injection A and Carriage A (MTS2)", 1003, "", "C:\TwinCAT\Io\Profibus\MTSE04C3.GSD")
            //       //create TcPlug
            //       nErr = item.CreateChild("Injection A Heats (TCPLUG)", 1003, "", "C:\TwinCAT\Io\Profibus\TcPlug.gsd")

            //create BK3120
            dynamic box22 = profibusMaster.CreateChild("Box22 (BK3120)", 1010);
            //set DP-Address
            box22.ConsumeXml("<TreeItem><BoxDef><FieldbusAddress>22</FieldbusAddress></BoxDef></TreeItem>");
            //create terminals
            box22.CreateChild("Term5 (KL2114)", 2114, "End Term (KL9010)");

            //set DP-Address + Diasbled
            dynamic screwA = systemManager.LookupTreeItem("TIID^Profibus Master (EL6731)^Screw A (AXIS_Screw_A_FC310x)");
            screwA.ConsumeXml("<TreeItem><BoxDef><FieldbusAddress>2</FieldbusAddress></BoxDef></TreeItem>");
            screwA.ConsumeXml("<TreeItem><Disabled>1</Disabled></TreeItem>");

            //create Virtual USB Master Device
            //create Profibus Master
            worker.Progress = 75;
            worker.ProgressStatus = "Creating Virtual USB Interface (USB) ...";

            dynamic usbDevice = devices.CreateChild("Virtual USB Interface (USB)", 57);
            //set virtual device name
            usbDevice.ConsumeXml("<TreeItem><USB><VirtualDeviceNames>1</VirtualDeviceNames></USB></TreeItem>");

            //create USB Box
            usbDevice.CreateChild("Box 0 (CPX8XX)", 9591);

            //PLC configuration
            //search for PLC project

            worker.Progress = 60;
            worker.ProgressStatus = "Adding PLC Project ...";

            string plcAxisTemplatePath = Path.Combine(ConfigurationTemplatesFolder, "PlcAxisTemplate.tpzip");

            dynamic plc = plcConfig.CreateChild("PlcAxisSample", 0, "", plcAxisTemplatePath);

            dynamic plcProject = systemManager.LookupTreeItem("TIPC^PlcAxisSample");
            dynamic plcInstances = systemManager.LookupTreeItem("TIPC^PlcAxisSample^PlcAxisSample Project");

            // Or alternatively via IRootProject
            plcInstances = plcProject.NestedProject;

            //rescan plc project
            //plc.ConsumeXml("<TreeItem><PlcDef><ProjectPath>" + PlcFile + "</ProjectPath><ReScan>1</ReScan></PlcDef></TreeItem>");

            //create links
            //link nc axis to drive
            //search for nc axis
            dynamic axis1 = systemManager.LookupTreeItem("TINC^NC-Task 1 SAF^Axes^Axis 1");
            //link to drive
            axis1.ConsumeXml("<TreeItem><NcAxisDef><AxisType>1</AxisType><AxisIoType>ProfibusMC</AxisIoType><IoItem><PathName>TIID^Profibus Master (EL6731)^Screw A (AXIS_Screw_A_FC310x)</PathName></IoItem></NcAxisDef></TreeItem>");

            worker.Progress = 90;
            worker.ProgressStatus = "Linking variables ...";

            //link terminal channel to plc variable
            systemManager.LinkVariables("TIPC^PlcAxisSample^PlcAxisSample Instance^PlcTask Inputs^MAIN.bIn", "TIID^EtherCAT Master^A2P (EK1100)^100 (EL1014)^Channel 1^Input");
            //link terminal channel to plc variable

            systemManager.LinkVariables("TIPC^PlcAxisSample^PlcAxisSample Instance^PlcTask Outputs^MAIN.bOut", "TIID^EtherCAT Master^A2P (EK1100)^102 (EL2004)^Channel 1^Output");
            //link terminal channel to plc variable

            systemManager.LinkVariables("TIPC^PlcAxisSample^PlcAxisSample Instance^PlcTask Outputs^MAIN.bOut", "TIID^EtherCAT Master^A4P (BK1120)^Term3 (KL2114)^Channel 1^Output");
            //link terminal channel to plc variable

            systemManager.LinkVariables("TIPC^PlcAxisSample^PlcAxisSample Instance^PlcTask Outputs^MAIN.bOut", "TIID^CANopen Master (EL6751)^Box11 (BK5150)^Term3 (KL2114)^Channel 1^Output");
            //link terminal channel to plc variable

            systemManager.LinkVariables("TIPC^PlcAxisSample^PlcAxisSample Instance^PlcTask Outputs^MAIN.bOut", "TIID^Profibus Master (EL6731)^Box22 (BK3120)^Term5 (KL2114)^Channel 1^Output");
            //link Control Panel LED to plc variable

            systemManager.LinkVariables("TIPC^PlcAxisSample^PlcAxisSample Instance^PlcTask Outputs^MAIN.bOut", "TIID^Virtual USB Interface (USB)^Box 0 (CPX8XX)^Outputs^LED 1");

            worker.Progress = 100;
        }

        /// <summary>
        /// Gets the Script description
        /// </summary>
        /// <value>The description.</value>
        public override string Description
        {
            get
            {
                return "Demonstrates the creation of an EtherCAT IO Subtree\nand the linking with PLC Symbols (Late Binding).";
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
                return "EtherCAT, IO-PLC Mapping, Scanning Devices";
            }
        }

        /// <summary>
        /// Gets the binding type of the script
        /// </summary>
        /// <value>The programming language.</value>
        public override string Binding
        {
            get
            {
                return "Dynamic";
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
                return "4012.0";
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
