using System;
using System.IO;
using System.Net;
using System.Xml;
using System.Xml.Linq;
using EnvDTE;
using EnvDTE100;
using EnvDTE80;
using TCatSysManagerLib;
using TwinCAT;
using TwinCAT.SystemManager;
using ScriptingTest;

namespace Scripting.CSharp
{
    /// <summary>
    /// Demonstration of implementing EtherCAT Automation Protocol with Network Variables
    /// </summary>
    /// <remarks>
    /// Creation of to TwinCAT XAE Projects within one solution (Publisher and Subscriber).
    /// Creation of EAP (EtherCAT Network protocol) Device
    /// Creation of Publisher Subscriber Network Variables with underlying Realtime Ethernet and UDP protocol
    /// Demonstration of different configurations Broadcast, Multicast, Unicast for each protocol
    /// </remarks>
    public class EtherCATAutomationProtocol
        : ScriptEarlyBound
    {
        private ITcSysManager4 sysManPublisher = null;
        private ITcSysManager4 sysManSubscriber = null;

        private Project publisherProject = null;
        private Project subscriberProject = null;

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
            this.publisherProject = (Project)CreateNewProject("Publisher");
            this.sysManPublisher = (ITcSysManager4)publisherProject.Object;
            this.subscriberProject = (Project)CreateNewProject("Subscriber");
            this.sysManSubscriber = (ITcSysManager4)subscriberProject.Object;

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
        /// Handler function Executing the Script code.
        /// </summary>
        /// <param name="worker">The worker.</param>
        protected override void OnExecute(IWorker worker)
        {
            worker.ProgressStatus = "Starting EAP Creation ...";
            worker.Progress = 0;

            /* =================================================================================================
             * Prepare AMS NetIDs for Publisher and Subscriber. Please replace with your NetIDs
             * ================================================================================================= */
            AmsNetId netIdPublisher = AmsNetId.Parse("1.2.3.4.1.1");
            AmsNetId netIdSubscriber = AmsNetId.Parse("2.2.3.4.1.1");

            worker.ProgressStatus = "Setting TargetNetIds ...";

            /* =================================================================================================
             * Connect to both ADS devices via their AMS NetID and the method SetTargetNetID
             * ================================================================================================= */
            sysManPublisher.SetTargetNetId(netIdPublisher.ToString());
            sysManSubscriber.SetTargetNetId(netIdSubscriber.ToString());

            /* =================================================================================================
             * Prepare path for PLC project
             * ================================================================================================= */
            string plcAxisTemplatePath = Path.Combine(ConfigurationTemplatesFolder, "PlcAxisTemplate.tpzip");

            worker.ProgressStatus = "Creating PLC Project ...";
            worker.Progress = 10;

            /* =================================================================================================
             * Adding PLC project to Publisher
             * ================================================================================================= */
            ITcSmTreeItem pubPlcConfig = sysManPublisher.LookupTreeItem("TIPC"); // Getting PLC-Configuration
            ITcSmTreeItem pubPlc = pubPlcConfig.CreateChild("Publisher", 0, "", plcAxisTemplatePath);

            worker.Progress = 20;

            /* =================================================================================================
             * Creating Publisher IOs
             * Scan the fieldbus interfaces and add an EtherCAT Device
             * ================================================================================================= */
            ITcSmTreeItem3 pubDevice = Helper.CreateEthernetDevice(this.sysManPublisher, DeviceType.EtherCAT_AutomationProtocol, "RTEthernet",worker);
            
            worker.ProgressStatus = "Creating Publishers ...";
            worker.Progress = 30;

            /* =================================================================================================
             * Adding Publisher RT variables (Broadcast, Multicast, Unicast)
             * ================================================================================================= */
            ITcSmTreeItem3 publisherRTBroadcast = (ITcSmTreeItem3)pubDevice.CreateChild("PubRTBroadcast", (int)BoxType.Ethernet_Publisher, null, null);
            SetPublisherBroadCastRT(publisherRTBroadcast);
            ITcSmTreeItem3 publisherRTMulticast = (ITcSmTreeItem3)pubDevice.CreateChild("PubRTMulticast", (int)BoxType.Ethernet_Publisher, null, null);
            SetPublisherMulticastRT(publisherRTMulticast, 1);
            ITcSmTreeItem3 publisherRTUnicast = (ITcSmTreeItem3)pubDevice.CreateChild("PubRTUnicast", (int)BoxType.Ethernet_Publisher, null, null);
            SetPublisherUnicastRT(publisherRTUnicast, netIdPublisher);

            /* =================================================================================================
             * Adding Publisher UDP variables (Broadcast, Multicast, Unicast)
             * ================================================================================================= */
            ITcSmTreeItem3 publisherUDPBroadcast = (ITcSmTreeItem3)pubDevice.CreateChild("PubUDPBroadcast", (int)BoxType.Ethernet_Publisher, null, null);
            SetPublisherBroadCastUdp(publisherUDPBroadcast);
            ITcSmTreeItem3 publisherUDPMulticast = (ITcSmTreeItem3)pubDevice.CreateChild("PubUDPMulticast", (int)BoxType.Ethernet_Publisher, null, null);
            SetPublisherMulticastUdp(publisherUDPMulticast, IPAddress.Parse("224.0.0.0"), IPAddress.None);
            ITcSmTreeItem3 publisherUDPUnicast = (ITcSmTreeItem3)pubDevice.CreateChild("PubUDPUnicast", (int)BoxType.Ethernet_Publisher, null, null);
            SetPublisherUnicastUdp(publisherUDPUnicast, IPAddress.Parse("1.2.3.4"), IPAddress.None);
            
            worker.ProgressStatus = "Creating Subscribers ...";
            worker.Progress = 40;

            /* =================================================================================================
             * Adding PLC project to Subscriber
             * ================================================================================================= */
            ITcSmTreeItem subPlcConfig = sysManSubscriber.LookupTreeItem("TIPC"); // Getting PLC-Configuration
            ITcSmTreeItem subPlc = subPlcConfig.CreateChild("Subscriber", 0, "", plcAxisTemplatePath);

            worker.Progress = 50;

            worker.ProgressStatus = "Creating Subscriber Side IO ...";
            worker.Progress = 60;

            /* =================================================================================================
             * Creating Subscriber IOs
             * Scan the fieldbus interfaces and add an EtherCAT Device
             * ================================================================================================= */
            ITcSmTreeItem3 subDevice = Helper.CreateEthernetDevice(this.sysManSubscriber, DeviceType.EtherCAT_AutomationProtocol, "RTEthernet",worker);

            worker.Progress = 70;

            /* =================================================================================================
             * Adding Subscriber RT variables (Broadcast, Multicast, Unicast)
             * ================================================================================================= */
            ITcSmTreeItem3 subscriberRTBroadcast = (ITcSmTreeItem3)subDevice.CreateChild("SubRTBroadcast", (int)BoxType.Ethernet_Subscriber, null, null);
            SetSubscriberBroadcastRT(subscriberRTBroadcast);
            ITcSmTreeItem3 subscriberRTMulticast = (ITcSmTreeItem3)subDevice.CreateChild("SubRTMulticast", (int)BoxType.Ethernet_Subscriber, null, null);
            SetSubscriberMulticastRT(subscriberRTMulticast, 1);
            ITcSmTreeItem3 subscriberRTUnicast = (ITcSmTreeItem3)subDevice.CreateChild("SubRTUnicast", (int)BoxType.Ethernet_Subscriber, null, null);
            SetSubscriberUnicastRT(subscriberRTUnicast, netIdPublisher);

            /* =================================================================================================
             * Adding Subscriber UDP variables (Broadcast, Multicast, Unicast)
             * ================================================================================================= */
            ITcSmTreeItem3 subscriberUDPBroadcast = (ITcSmTreeItem3)subDevice.CreateChild("SubUDPBroadcast", (int)BoxType.Ethernet_Subscriber, null, null);
            SetSubscriberBroadcastUdp(subscriberUDPBroadcast);
            ITcSmTreeItem3 subscriberUDPMulticast = (ITcSmTreeItem3)subDevice.CreateChild("SubUDPMulticast", (int)BoxType.Ethernet_Subscriber, null, null);
            SetSubscriberMulticastUdp(subscriberUDPMulticast, AmsNetId.Empty, IPAddress.Parse("224.0.0.0"));
            ITcSmTreeItem3 subscriberUDPUnicast = (ITcSmTreeItem3)subDevice.CreateChild("SubUDPUnicast", (int)BoxType.Ethernet_Subscriber, null, null);
            SetSubscriberUnicastUdp(subscriberUDPUnicast,netIdPublisher);

            /* =================================================================================================
             * Linking the Publisher and Subscriber objects and mapping of PLC to IO
             * ================================================================================================= */
            string subPlcVar = "TIPC^Subscriber^Subscriber Instance^PlcTask Inputs^MAIN.iIn";
            string pubPlcVar = "TIPC^Publisher^Publisher Instance^PlcTask Outputs^MAIN.iOut";

            worker.Progress = 80;
            worker.ProgressStatus = "Linking variables ...";
            connectViaNetworkVariables(sysManPublisher, pubPlcVar, pubDevice, publisherRTBroadcast, sysManSubscriber, subPlcVar, subDevice, subscriberRTBroadcast);
            connectViaNetworkVariables(sysManPublisher, pubPlcVar, pubDevice, publisherRTMulticast, sysManSubscriber, subPlcVar, subDevice, subscriberRTMulticast);
            connectViaNetworkVariables(sysManPublisher, pubPlcVar, pubDevice, publisherRTUnicast, sysManSubscriber, subPlcVar, subDevice, subscriberRTUnicast);
            connectViaNetworkVariables(sysManPublisher, pubPlcVar, pubDevice, publisherUDPBroadcast, sysManSubscriber, subPlcVar, subDevice, subscriberUDPBroadcast);
            connectViaNetworkVariables(sysManPublisher, pubPlcVar, pubDevice, publisherUDPMulticast, sysManSubscriber, subPlcVar, subDevice, subscriberUDPMulticast);
            connectViaNetworkVariables(sysManPublisher, pubPlcVar, pubDevice, publisherUDPUnicast, sysManSubscriber, subPlcVar, subDevice, subscriberUDPUnicast);

            worker.Progress = 100;
            worker.ProgressStatus = "Script succeeded!";
        }

        /// <summary>
        /// Gets the Script description
        /// </summary>
        /// <value>The description.</value>
        public override string Description
        {
            get
            {
                return "Demonstration of implementing EtherCAT Automation Protocol\n with Network Variables";
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
                return "EtherCAT Automation Protocol (EAP), RTEthernet,\n NetworkVariables, Publisher, Subscriber,\bScanDevices, Mapping";
            }
        }

        /// <summary>
        /// Connects PlcSource -> Publisher -> Subscriber - > PlcTarget
        /// </summary>
        /// <param name="pubSysMan">Publisher side SystemManager</param>
        /// <param name="pubSymbolPath">Publisher side symbol path</param>
        /// <param name="pubDevice">Publisher device.</param>
        /// <param name="publisher">Publisher object</param>
        /// <param name="subSysMan">Subscriber System Manager</param>
        /// <param name="subSymbolPath">Subscriber Symbol Path</param>
        /// <param name="subDevice">Subscriber Device</param>
        /// <param name="subscriber">Subscriber object.</param>
        private void connectViaNetworkVariables(ITcSysManager4 pubSysMan, string pubSymbolPath, ITcSmTreeItem3 pubDevice, ITcSmTreeItem3 publisher, ITcSysManager4 subSysMan, string subSymbolPath, ITcSmTreeItem3 subDevice, ITcSmTreeItem3 subscriber)
        {
            if (pubDevice.ItemType != (int)TreeItemType.Device) throw new ArgumentException();
            if (subDevice.ItemType != (int)TreeItemType.Device) throw new ArgumentException();
            if (pubDevice.ItemSubType != (int)DeviceType.EtherCAT_AutomationProtocol) throw new ArgumentException();
            if (subDevice.ItemSubType != (int)DeviceType.EtherCAT_AutomationProtocol) throw new ArgumentException();

            if (publisher.ItemType != (int)TreeItemType.Box) throw new ArgumentException();
            if (subscriber.ItemType != (int)TreeItemType.Box) throw new ArgumentException();
            if (publisher.ItemSubType != (int)BoxType.Ethernet_Publisher) throw new ArgumentException();
            if (subscriber.ItemSubType != (int)BoxType.Ethernet_Subscriber) throw new ArgumentException();

            ITcSmTreeItem3 pubSymbol = (ITcSmTreeItem3)pubSysMan.LookupTreeItem(pubSymbolPath);
            ITcSmTreeItem3 subSymbol = (ITcSmTreeItem3)subSysMan.LookupTreeItem(subSymbolPath);

            int index1 = pubSymbolPath.LastIndexOf('^');
            int index2 = subSymbolPath.LastIndexOf('^');

            string pubName = pubSymbolPath.Substring(index1 + 1);
            string subName = subSymbolPath.Substring(index2 + 1);

            string pubDataType = GetVariableDataType(pubSymbol);
            int pubBitSize = GetVariableBitSize(pubSymbol);

            string subDataType = GetVariableDataType(subSymbol);
            int subBitSize = GetVariableBitSize(subSymbol);

            if (pubBitSize != subBitSize) throw new ArgumentException("DataTypes inconsistent!");


            int publisherId = -1;
            ITcSmTreeItem3 publisherVar = CreatePublisherVariable(publisher, pubName, pubDataType, ref publisherId);
            ITcSmTreeItem3 subscriberVar = CreateSubscriberVariable(subscriber, pubName, pubDataType, publisherId);

            LinkVariables(sysManPublisher, pubSymbol, publisherVar);
            LinkVariables(sysManSubscriber, subscriberVar, subSymbol);
        }

        /// <summary>
        /// Gets the VarData symbol of a Network Variable
        /// </summary>
        /// <param name="networkVariable">The Network Varialbe (NVPublisherVar or NVSubscriberVar)</param>
        /// <returns></returns>
        private ITcSmTreeItem3 GetVarData(ITcSmTreeItem3 networkVariable)
        {
            if (networkVariable.ItemType != (int)TreeItemType.NVPublisherVar && networkVariable.ItemType != (int)TreeItemType.NVSubscriberVar)
                throw new ArgumentException();

            foreach (ITcSmTreeItem3 child in networkVariable)
            {
                if (child.ItemType == (int)TreeItemType.VariableGroup)
                {
                    if (networkVariable.ItemType == (int)TreeItemType.NVPublisherVar)
                    {
                        if (child.ItemSubType == (int)VarGroupType.Output)
                        {
                            foreach (ITcSmTreeItem3 outputItem in child)
                            {
                                if (string.Compare("VarData", outputItem.Name, true) == 0)
                                    return outputItem;
                            }
                        }
                    }

                    else if (networkVariable.ItemType == (int)TreeItemType.NVSubscriberVar)
                    {
                        if (child.ItemSubType == (int)VarGroupType.Input)
                        {
                            foreach (ITcSmTreeItem3 outputItem in child)
                            {
                                if (string.Compare("VarData", outputItem.Name, true) == 0)
                                    return outputItem;
                            }
                        }

                    }
                }
            }
            throw new ApplicationException("Unexpected!");
        }

        /// <summary>
        /// Links (Maps) the specified symbols ( TreeItemType.Variable) within one XAE project
        /// </summary>
        /// <param name="systemManager">The system manager.</param>
        /// <param name="source">The source.</param>
        /// <param name="target">The target.</param>
        /// <param name="offsetSource">The offset source.</param>
        /// <param name="offsetTarget">The offset target.</param>
        /// <param name="overlap">The overlap.</param>
        private void LinkVariables(ITcSysManager4 systemManager, ITcSmTreeItem3 source, ITcSmTreeItem3 target, int offsetSource, int offsetTarget, int overlap)
        {
            systemManager.LinkVariables(source.PathName, target.PathName, offsetSource, offsetTarget, overlap);
        }

        /// <summary>
        /// Links (Maps) the specified symbols ( TreeItemType.Variable) within one XAE project
        /// </summary>
        /// <param name="systemManager">The system manager.</param>
        /// <param name="source">The source variable</param>
        /// <param name="target">The target variable</param>
        private void LinkVariables(ITcSysManager4 systemManager, ITcSmTreeItem3 source, ITcSmTreeItem3 target)
        {
            LinkVariables(systemManager, source, target, 0, 0, 0);
        }

        /// <summary>
        /// Produces the XML and loads it into an XmlDocument.
        /// </summary>
        /// <param name="treeItem">The tree item.</param>
        /// <returns></returns>
        private XmlDocument ProduceXml(ITcSmTreeItem3 treeItem)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(treeItem.ProduceXml(false));
            return doc;
        }

        /// <summary>
        /// Gets the type variable / symbol.
        /// </summary>
        /// <param name="variable">The variable.</param>
        /// <returns></returns>
        private string GetVariableDataType(ITcSmTreeItem3 variable)
        {
            if (variable.ItemType != (int)TreeItemType.Variable) throw new ArgumentException();

            XmlDocument doc = ProduceXml(variable);
            XmlNode node = doc.SelectSingleNode("TreeItem/VarDef/VarType");
            return node.InnerText;
        }

        /// <summary>
        /// Gets the variable DataType id.
        /// </summary>
        /// <param name="variable">The variable.</param>
        /// <returns></returns>
        private int GetVariableDataTypeId(ITcSmTreeItem3 variable)
        {
            if (variable.ItemType != (int)TreeItemType.Variable) throw new ArgumentException();
            return variable.ItemSubType;
        }

        /// <summary>
        /// Gets the bitsize of the Variable / its represented data type.
        /// </summary>
        /// <param name="variable">The variable.</param>
        /// <returns></returns>
        private int GetVariableBitSize(ITcSmTreeItem3 variable)
        {
            if (variable.ItemType != (int)TreeItemType.Variable) throw new ArgumentException();

            XmlDocument doc = ProduceXml(variable);
            XmlNode node = doc.SelectSingleNode("TreeItem/VarDef/VarBitSize");
            return int.Parse(node.InnerText);
        }


        /// <summary>
        /// Scans the devices on the target system and returns a list of XML Nodes representing these devices.
        /// </summary>
        /// <param name="systemManager">The system manager.</param>
        /// <returns></returns>
        private XmlNodeList ScanDevices(ITcSysManager4 systemManager)
        {
            ITcSmTreeItem3 devices = (ITcSmTreeItem3)systemManager.LookupTreeItem("TIID");

            XmlDocument doc = this.ProduceXml(devices);
            return doc.SelectNodes("TreeItem/DeviceGrpDef/FoundDevices/Device");
        }

        /// <summary>
        /// Creates the spefied publisher variable
        /// </summary>
        /// <param name="publisher">The publisher.</param>
        /// <param name="varName">Name of the var.</param>
        /// <param name="dataType">Type of the data.</param>
        /// <param name="publisherId">The publisher id.</param>
        /// <returns></returns>
        private ITcSmTreeItem3 CreatePublisherVariable(ITcSmTreeItem3 publisher, string varName, string dataType, ref int publisherId)
        {
            if (publisher.ItemType != (int)TreeItemType.Box || publisher.ItemSubType != (int)BoxType.Ethernet_Publisher) throw new ArgumentException();

            ITcSmTreeItem3 publisherVar = (ITcSmTreeItem3)publisher.CreateChild(varName, (int)TreeItemType.NVPublisherVar, null, dataType);

            if (publisherId > 0)
            {
                SetPublisherId(publisherVar, publisherId);
            }
            publisherId = GetPublisherId(publisherVar);

            ITcSmTreeItem3 dataVariable = GetVarData(publisherVar);
            return dataVariable;
        }

        /// <summary>
        /// Creates the specified Subscriber variable
        /// </summary>
        /// <param name="subscriber">The subscriber.</param>
        /// <param name="varName">Name of the var.</param>
        /// <param name="dataType">Type of the data.</param>
        /// <param name="publisherId">The publisher id.</param>
        /// <returns></returns>
        private ITcSmTreeItem3 CreateSubscriberVariable(ITcSmTreeItem3 subscriber, string varName, string dataType, int publisherId)
        {
            if (subscriber.ItemType != (int)TreeItemType.Box || subscriber.ItemSubType != (int)BoxType.Ethernet_Subscriber) throw new ArgumentException();

            ITcSmTreeItem3 subscriberVar = (ITcSmTreeItem3)subscriber.CreateChild(varName, (int)TreeItemType.NVPublisherVar, null, dataType);

            SetPublisherId(subscriberVar, publisherId);

            ITcSmTreeItem3 dataVariable = GetVarData(subscriberVar);
            return dataVariable;
        }

        /// <summary>
        /// Sets the publisher id.
        /// </summary>
        /// <param name="nvVar">The Network variable.</param>
        /// <param name="publisherId">The publisher id.</param>
        private void SetPublisherId(ITcSmTreeItem3 nvVar, int publisherId)
        {
            string xml;

            if (nvVar.ItemType == (int)TreeItemType.NVSubscriberVar)
            {
                xml = string.Format("<TreeItem><NvSubVarDef><NvId>{0}</NvId></NvSubVarDef></TreeItem>", publisherId);
                nvVar.ConsumeXml(xml);
            }
            else if (nvVar.ItemType == (int)TreeItemType.NVPublisherVar)
            {
                xml = string.Format("<TreeItem><NvPubVarDef><NvId>{0}</NvId></NvPubVarDef></TreeItem>", publisherId);
                nvVar.ConsumeXml(xml);
            }
            else throw new ArgumentException();
        }

        /// <summary>
        /// Gets the publisher id.
        /// </summary>
        /// <param name="nvVar">The Network variable</param>
        /// <returns></returns>
        private int GetPublisherId(ITcSmTreeItem3 nvVar)
        {
            if (nvVar.ItemType == (int)TreeItemType.NVSubscriberVar)
            {
                XmlDocument doc = ProduceXml(nvVar);
                XmlNode node = doc.SelectSingleNode("TreeItem/NvSubVarDef/NvId");
                return int.Parse(node.InnerText);

            }
            else if (nvVar.ItemType == (int)TreeItemType.NVPublisherVar)
            {
                XmlDocument doc = ProduceXml(nvVar);
                XmlNode node = doc.SelectSingleNode("TreeItem/NvPubVarDef/NvId");
                return int.Parse(node.InnerText);
            }
            else
                throw new ArgumentException();
        }

        /// <summary>
        /// Sets the Properties for a Realtime Ethernet Broadcast publisher
        /// </summary>
        public void SetPublisherBroadCastRT(ITcSmTreeItem3 publisher)
        {
            SetPublisherAddressRT(publisher, DefaultMacIds.Broadcast, AmsNetId.Empty);
        }

        /// <summary>
        /// Sets the Properties for a Realtime Ethernet Broadcast publisher
        /// </summary>
        public void SetPublisherBroadCastUdp(ITcSmTreeItem3 publisher)
        {
            SetPublisherAddressUdp(publisher, IPAddress.Broadcast, IPAddress.None);
        }

        /// <summary>
        /// Sets the Properties for a Realtime Ethernet Multicast publisher
        /// </summary>
        /// <param name="publisher">The publisher.</param>
        /// <param name="id">Multicast ID</param>
        public void SetPublisherMulticastRT(ITcSmTreeItem3 publisher, System.UInt16 id)
        {
            byte[] b = BitConverter.GetBytes(id);
            //byte[] macId = new byte[] { 0x01, 0x01, 0x05, 0x04, b[1], b[0] };

            byte[] macId = DefaultMacIds.NvMulticast;
            macId[4] = b[1];
            macId[5] = b[0];
            SetPublisherAddressRT(publisher, macId, AmsNetId.Empty);
        }

        /// <summary>
        /// Sets the Properties for a Realtime Ethernet Multicast publisher
        /// </summary>
        /// <param name="publisher">The publisher.</param>
        /// <param name="multicastAddress">The multicast address.</param>
        /// <param name="gateway">The gateway address</param>
        public void SetPublisherMulticastUdp(ITcSmTreeItem3 publisher, IPAddress multicastAddress, IPAddress gateway)
        {
            SetPublisherAddressUdp(publisher, multicastAddress, gateway);
        }

        /// <summary>
        /// Sets the Properties for a Realtime Ethernet Unicast publisher (via AmsNetId)
        /// </summary>
        /// <param name="publisher">The publisher.</param>
        /// <param name="publisherNetId">The publisher net id.</param>
        public void SetPublisherUnicastRT(ITcSmTreeItem3 publisher,AmsNetId publisherNetId)
        {
            if (publisherNetId == null) throw new ArgumentNullException("publisherNetId");

            SetPublisherAddressRT(publisher, DefaultMacIds.Empty, publisherNetId);
        }

        /// <summary>
        /// Sets the Properties for a Realtime Ethernet Unicast publisher (via MacId)
        /// </summary>
        /// <param name="publisher">The publisher.</param>
        /// <param name="multicastMacId">The multicast mac id.</param>
        public void SetPublisherUnicastRT(ITcSmTreeItem3 publisher, byte[] multicastMacId)
        {
            if (multicastMacId == null) throw new ArgumentNullException("multicastMacId");

            SetPublisherAddressRT(publisher, multicastMacId, AmsNetId.Empty);
        }

        /// <summary>
        /// Sets the Properties for a Realtime Ethernet Unicast publisher (via AmsNetId)
        /// </summary>
        /// <param name="publisher">The publisher.</param>
        /// <param name="ip">The ip.</param>
        /// <param name="gateway">The gateway address.</param>
        public void SetPublisherUnicastUdp(ITcSmTreeItem3 publisher, IPAddress ip, IPAddress gateway)
        {

            byte[] bytes = ip.GetAddressBytes();
            if (bytes.Length > 4) throw new ArgumentException();

            int i = BitConverter.ToInt32(bytes, 0);
            int host = IPAddress.NetworkToHostOrder(i);

            byte[] bytes2 = BitConverter.GetBytes(host);

            byte[] publisherNetId = new byte[6];
            Array.Copy(bytes2, publisherNetId, 4);
            publisherNetId[4] = 0;
            publisherNetId[5] = 0;

            SetPublisherAddressUdp(publisher, ip, gateway);
        }

        /// <summary>
        /// Sets the RT parameters for a publisher
        /// </summary>
        /// <param name="publisher">The publisher.</param>
        /// <param name="macId">The mac id.</param>
        /// <param name="netId">The net id.</param>
        void SetPublisherAddressRT(ITcSmTreeItem3 publisher, byte[] macId, AmsNetId netId)
        {
            if (publisher.ItemType != (int)TreeItemType.Box || publisher.ItemSubType != (int)BoxType.Ethernet_Publisher)
                throw new ArgumentException();

            if (macId.Length != 6)
                throw new ArgumentException();

            string macIdStr = string.Format("{0:x2} {1:x2} {2:x2} {3:x2} {4:x2} {5:x2}", macId[0], macId[1], macId[2], macId[3], macId[4], macId[5]);
            string netIdStr = netId.ToString();

            XDocument x = new XDocument();
            x.Add(new XElement("TreeItem",
                new XElement("BoxDef",
                    new XElement("NvPubDef",
                        new XElement("Udp",new XAttribute("Enabled",0)),
                        new XElement("PublisherNetId",netIdStr),
                        new XElement("MacAddress",macIdStr),
                        new XElement("IoDiv",
                            new XElement("Divider",1),
                            new XElement("Modulo",0)),
                        new XElement("VLAN",
                            new XElement("Enable",0),
                            new XElement("Id",0),
                            new XElement("Prio",0)),
                        new XElement("ArpInterval",1000),
                        new XElement("DisableSubscriberMonitoring",0),
                        new XElement("TargetChangeable",0)))));
            
            string xml = x.ToString(SaveOptions.None);
            publisher.ConsumeXml(xml);
        }

        /// <summary>
        /// Sets the parameters for a UDP publisher
        /// </summary>
        /// <param name="publisher">The publisher.</param>
        /// <param name="address">The address.</param>
        /// <param name="gateway">The gateway.</param>
        void SetPublisherAddressUdp(ITcSmTreeItem3 publisher, IPAddress address, IPAddress gateway)
        {
            if (publisher.ItemType != (int)TreeItemType.Box || publisher.ItemSubType != (int)BoxType.Ethernet_Publisher)
                throw new ArgumentException();

            XDocument x = new XDocument();
            x.Add(new XElement("TreeItem",
                new XElement("BoxDef",
                    new XElement("NvPubDef",
                        new XElement("Udp", new XAttribute("Enabled", 1),
                            new XElement("Address", address),
                            new XElement("Gateway", gateway)),
                            new XElement("IoDiv",
                                new XElement("Divider", 1),
                                new XElement("Modulo", 0)),
                            new XElement("VLAN",
                                new XElement("Enable", 0),
                                new XElement("Id", 0),
                                new XElement("Prio", 0)),
                            new XElement("ArpInterval", 1000),
                            new XElement("DisableSubscriberMonitoring", 0),
                            new XElement("TargetChangeable", 0)))));

            string xml = x.ToString(SaveOptions.None);
            publisher.ConsumeXml(xml);
        }

        /// <summary>
        /// Sets the parameters for a RT subscriber
        /// </summary>
        /// <param name="subscriber">The subscriber.</param>
        /// <param name="multicastMacId">The multicast mac id.</param>
        /// <param name="publisherNetId">The publisher net id.</param>
        void SetSubscriberAddressRT(ITcSmTreeItem3 subscriber, byte[] multicastMacId, AmsNetId publisherNetId)
        {
            if (subscriber.ItemType != (int)TreeItemType.Box || subscriber.ItemSubType != (int)BoxType.Ethernet_Subscriber)
                throw new ArgumentException();

            if (multicastMacId.Length != 6)
                throw new ArgumentException();

            string macIdStr = string.Format("{0:x2} {1:x2} {2:x2} {3:x2} {4:x2} {5:x2}", multicastMacId[0], multicastMacId[1], multicastMacId[2], multicastMacId[3], multicastMacId[4], multicastMacId[5]);

            XDocument x = new XDocument();
            x.Add(new XElement("TreeItem",
                new XElement("BoxDef",
                    new XElement("NvSubDef",
                            new XElement("PublisherNetId", publisherNetId.ToString()),
                            new XElement("MulticastMacAddr", macIdStr)))));

            string xml = x.ToString(SaveOptions.None);
            subscriber.ConsumeXml(xml);
        }

        /// <summary>
        /// Sets the parameters for a UDP Subscriber
        /// </summary>
        /// <param name="publisher">The publisher.</param>
        /// <param name="multicastIpAddr">The multicast ip addr.</param>
        /// <param name="publisherNetId">The publisher net id.</param>
        void SetSubscriberAddressUdp(ITcSmTreeItem3 publisher, IPAddress multicastIpAddr, AmsNetId publisherNetId)
        {
            if (publisher.ItemType != (int)TreeItemType.Box || publisher.ItemSubType != (int)BoxType.Ethernet_Subscriber)
                throw new ArgumentException();

				XDocument x = new XDocument();

				IPAddress test1 = IPAddress.Broadcast;
				IPAddress test2 = IPAddress.Any;
				IPAddress test3 = IPAddress.None;

			    // Check whether Multicast or not
	
            	if (multicastIpAddr != IPAddress.None)
				{
					x.Add(new XElement("TreeItem",
						 new XElement("BoxDef",
							  new XElement("NvSubDef",
										 new XElement("PublisherNetId", publisherNetId),
										 new XElement("MulticastIpAddr", multicastIpAddr)))));
				}
				else
				{
					x.Add(new XElement("TreeItem",
						 new XElement("BoxDef",
							  new XElement("NvSubDef",
										 new XElement("PublisherNetId", publisherNetId)))));
				}

            string xml = x.ToString(SaveOptions.None);
            publisher.ConsumeXml(xml);
        }

        /// <summary>
        /// Sets the parameters for a RT Broadcast subscriber.
        /// </summary>
        /// <param name="subscriber">The subscriber.</param>
        void SetSubscriberBroadcastRT(ITcSmTreeItem3 subscriber)
        {
            SetSubscriberAddressRT(subscriber, DefaultMacIds.Empty, AmsNetId.Empty);
        }

        /// <summary>
        /// Sets the parameters for a RT Multicast subscriber
        /// </summary>
        /// <param name="subscriber">The subscriber.</param>
        /// <param name="id">The id.</param>
        void SetSubscriberMulticastRT(ITcSmTreeItem3 subscriber, UInt16 id)
        {
            byte[] b = BitConverter.GetBytes(id);
            byte[] macId = new byte[] { 0x01, 0x01, 0x05, 0x04, b[1], b[0] };

            SetSubscriberAddressRT(subscriber, macId, AmsNetId.Empty);
        }

        /// <summary>
        /// Sets parameters for a RT Unicast subscriber
        /// </summary>
        /// <param name="subscriber">The subscriber.</param>
        /// <param name="netId">The net id.</param>
        void SetSubscriberUnicastRT(ITcSmTreeItem3 subscriber, AmsNetId netId)
        {
            SetSubscriberAddressRT(subscriber, DefaultMacIds.Empty, netId);
        }

        /// <summary>
        /// Sets parameters for a UDP Broadcast subscriber
        /// </summary>
        /// <param name="subscriber">The subscriber.</param>
        void SetSubscriberBroadcastUdp(ITcSmTreeItem3 subscriber)
        {
            SetSubscriberAddressUdp(subscriber, IPAddress.None, AmsNetId.Empty);
        }

        /// <summary>
        /// Sets parameters for a UDP Multicast subscriber
        /// </summary>
        /// <param name="subscriber">The subscriber.</param>
        /// <param name="netId">The net id.</param>
        /// <param name="multicastAddress">The multicast address.</param>
        void SetSubscriberMulticastUdp(ITcSmTreeItem3 subscriber, AmsNetId netId, IPAddress multicastAddress)
        {
            SetSubscriberAddressUdp(subscriber, multicastAddress,netId);
        }

        /// <summary>
        /// Sets the parameters for a UDP Unicast subscriber
        /// </summary>
        /// <param name="subscriber">The subscriber.</param>
        /// <param name="publisherNetId">The publisher NetId</param>
        void SetSubscriberUnicastUdp(ITcSmTreeItem3 subscriber, AmsNetId publisherNetId)
        {
            // An Unicast subscriber doesn't need any settings, because the Unicast behaviour is set on the
            // Publisher side (sinding only to a single address).
            SetSubscriberAddressUdp(subscriber, IPAddress.None, publisherNetId);
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

