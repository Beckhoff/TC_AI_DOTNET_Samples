using System;
using System.Collections.Generic;
using System.Xml;
using TwinCAT.SystemManager;

namespace ScriptingTest.LateBinding
{
    internal static class Helper
    {
        /// <summary>
        /// Scans the devices on the target system and returns a list of XML Nodes representing these devices.
        /// </summary>
        /// <param name="systemManager">The system manager.</param>
        /// <returns></returns>
        internal static XmlNodeList ScanDevices(dynamic systemManager)
        {
            dynamic devices = systemManager.LookupTreeItem("TIID");
            XmlDocument doc = new XmlDocument();
            string xml = devices.ProduceXml(false);

            doc.LoadXml(xml);
            return doc.SelectNodes("TreeItem/DeviceGrpDef/FoundDevices/Device");
        }

        /// <summary>
        /// Scans the devices and Adds an EtherCAT Automation Protocol device if an network adapter is found.
        /// </summary>
        /// <param name="systemManager">The system manager.</param>
        /// <param name="type">Device type</param>
        /// <param name="deviceName">Name of the device.</param>
        /// <param name="progress">Progress interface</param>
        /// <returns></returns>
        internal static dynamic CreateEthernetDevice(dynamic systemManager, DeviceType type, string deviceName, IProgressProvider progress)
        {
            if (progress != null)
                progress.ProgressStatus = "Scanning devices ...";

            // Scans and Creates the appropriate device
            dynamic device = null;
            XmlNodeList nodes = ScanDevices(systemManager);
            List<XmlNode> ethernetNodes = new List<XmlNode>();

            foreach (XmlNode node in nodes)
            {
                XmlNode typeNode = node.SelectSingleNode("ItemSubType");

                int subType = int.Parse(typeNode.InnerText);

#pragma warning disable 0618
                if (subType == (int)DeviceType.EtherCAT_AutomationProtocol || subType == (int)DeviceType.Ethernet_RTEthernet || subType == (int)DeviceType.EtherCAT_DirectMode || subType == (int)DeviceType.EtherCAT_DirectModeV210)
#pragma warning restore 0618
                {
                    ethernetNodes.Add(node);
                }
            }

            if (progress != null)
                progress.ProgressStatus = string.Format("Scan found '{0}' Ethernet devices.", ethernetNodes.Count);

            dynamic devices = systemManager.LookupTreeItem("TIID");
            device = devices.CreateChild(deviceName, (int)type, null, null);

            if (ethernetNodes.Count > 0)
            {
                // Limitation: Taking only the first found Ethernet Adapter here!!!
                XmlNode ethernetNode = ethernetNodes[0];
                XmlNode addressInfoNode = ethernetNode.SelectSingleNode("AddressInfo");

                // Set the Address Info
                string xml = string.Format("<TreeItem><DeviceDef>{0}</DeviceDef></TreeItem>", addressInfoNode.OuterXml);
                device.ConsumeXml(xml);
            }

            if (progress != null)
                progress.ProgressStatus = string.Format("Devíce Type '{0}' added.", type.ToString());

            return device;
        }
    }

    /// <summary>
    /// Converter class converting instances of the <see cref="PLCACCESS"/> enumeration
    /// </summary>
    public class PlcAccessConverter
    {
        /// <summary>
        /// Converts the <see cref="PLCACCESS"/> enumeration to string.
        /// </summary>
        /// <param name="access"></param>
        /// <returns></returns>
        public static string ToString(PLCACCESS access)
        {
            switch (access)
            {
                case PLCACCESS.PLCACCESS_INTERNAL:
                    return "INTERNAL";
                case PLCACCESS.PLCACCESS_PRIVATE:
                    return "PRIVATE";
                case PLCACCESS.PLCACCESS_PROTECTED:
                    return "PROTECTED";
                case PLCACCESS.PLCACCESS_PUBLIC:
                    return "PUBLIC";
                default:
                    throw new NotImplementedException();
            }
        }
    }

    /// <summary>
    /// Enum redefinition (because Interop is not available in late bound code)
    /// </summary>
    public enum vsBuildState
    {
        /// <summary>
        /// Build not started
        /// </summary>
        vsBuildStateNotStarted = 1,
        /// <summary>
        /// Build in progress
        /// </summary>
        vsBuildStateInProgress = 2,
        /// <summary>
        /// Build done
        /// </summary>
        vsBuildStateDone = 3,
    }

    /// <summary>
    /// Enum redefinition (because Interop is not available in late bound code)
    /// </summary>
    public enum PLCACCESS
    {
        /// <summary>
        /// Public accessor
        /// </summary>
        PLCACCESS_PUBLIC = 0,
        /// <summary>
        /// Private accessor
        /// </summary>
        PLCACCESS_PRIVATE = 1,
        /// <summary>
        /// Protected accessor
        /// </summary>
        PLCACCESS_PROTECTED = 2,
        /// <summary>
        /// Internal accessor
        /// </summary>
        PLCACCESS_INTERNAL = 3,
    }
}
