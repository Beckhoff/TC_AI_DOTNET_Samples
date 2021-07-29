using System.Collections.Generic;
using System.Xml;
using TCatSysManagerLib;
using TwinCAT.SystemManager;
using ScriptingTest;
using System;

namespace Scripting.CSharp
{
    /// <summary>
    /// Helper class
    /// </summary>
    public static class Helper
    {
        /// <summary>
        /// Scans the devices on the target system and returns a list of XML Nodes representing these devices.
        /// </summary>
        /// <param name="systemManager">The system manager.</param>
        /// <returns></returns>
        public static XmlNodeList ScanDevices(ITcSysManager4 systemManager)
        {
            ITcSmTreeItem3 devices = (ITcSmTreeItem3)systemManager.LookupTreeItem("TIID");
            XmlDocument doc = new XmlDocument();
            string xml = devices.ProduceXml(false);

            doc.LoadXml(xml);
            return doc.SelectNodes("TreeItem/DeviceGrpDef/FoundDevices/Device");
        }

        /// <summary>
        /// Scans the devices and Adds an EtherCAT Automation Protocol device if an network adapter is found.
        /// </summary>
        /// <param name="systemManager">The system manager.</param>
        /// <param name="type">The type.</param>
        /// <param name="deviceName">Name of the device.</param>
        /// <param name="progress">The progress.</param>
        /// <returns></returns>
        public static ITcSmTreeItem3 CreateEthernetDevice(ITcSysManager4 systemManager, DeviceType type, string deviceName, IProgressProvider progress)
        {
            if (progress != null)
                progress.ProgressStatus = "Scanning devices ...";

            // Scans and Creates the appropriate device
            ITcSmTreeItem3 device = null;
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

            ITcSmTreeItem3 devices = (ITcSmTreeItem3)systemManager.LookupTreeItem("TIID");
            device = (ITcSmTreeItem3)devices.CreateChild(deviceName, (int)type, null, null);

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
                progress.ProgressStatus = string.Format("Device Type '{0}' added.", type.ToString());

            return device;
        }
    }

    /// <summary>
    /// Var Declaration class.
    /// </summary>
    public class VarDeclaration : ITcPlcVarDeclaration
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="VarDeclaration"/> class.
        /// </summary>
        /// <param name="scope">The scope.</param>
        /// <param name="name">The name.</param>
        /// <param name="type">The type.</param>
        /// <param name="address">The address.</param>
        /// <param name="initValue">The init value.</param>
        /// <param name="comment">The comment.</param>
        /// <param name="flags">The flags.</param>
        public VarDeclaration(PLC_VARDECL_SCOPE scope, string name, string type, string address, string initValue, string comment, PLC_VARDECL_FLAGS flags)
        {
            this._scope = scope;
            this._name = name;
            this._type = type;
            this._address = address;
            this._initValue = initValue;
            this._comment = comment;
            this._flags = flags;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="VarDeclaration"/> class.
        /// </summary>
        /// <param name="scope">The scope.</param>
        /// <param name="name">The name.</param>
        /// <param name="type">The type.</param>
        /// <param name="address">The address.</param>
        public VarDeclaration(PLC_VARDECL_SCOPE scope, string name, string type, string address)
            : this(scope, name, type, address, string.Empty, string.Empty, (PLC_VARDECL_FLAGS)0)
        {

        }

        #region ITcPlcVarDeclaration Members

        string _address;

        /// <summary>
        /// Gets the address.
        /// </summary>
        /// <value>The address.</value>
        public string Address
        {
            get { return _address; }
        }

        string _comment;
        /// <summary>
        /// Gets the comment.
        /// </summary>
        /// <value>The comment.</value>
        public string Comment
        {
            get { return _comment; }
        }

        PLC_VARDECL_FLAGS _flags;

        /// <summary>
        /// Gets the flags.
        /// </summary>
        /// <value>The flags.</value>
        public PLC_VARDECL_FLAGS Flags
        {
            get { return _flags; }
        }

        string _initValue;

        /// <summary>
        /// Gets the init value.
        /// </summary>
        /// <value>The init value.</value>
        public string InitValue
        {
            get { return _initValue; }
        }

        string _name;

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name
        {
            get { return _name; }
        }

        PLC_VARDECL_SCOPE _scope;

        /// <summary>
        /// Gets the scope.
        /// </summary>
        /// <value>The scope.</value>
        public PLC_VARDECL_SCOPE Scope
        {
            get { return _scope; }
        }

        string _type;

        /// <summary>
        /// Gets the type.
        /// </summary>
        /// <value>The type.</value>
        public string Type
        {
            get { return _type; }
        }

        #endregion
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
}
