using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EnvDTE80;
using EnvDTE100;
using System.Reflection;
using System.IO;
using System.ComponentModel;
using System.Threading;
using System.Diagnostics;
using System.Globalization;

namespace ScriptingTest
{
    /// <summary>
    /// Progress provider interface
    /// </summary>
    public interface IProgressProvider
    {
        /// <summary>
        /// Sets the progress.
        /// </summary>
        /// <value>The progress.</value>
        int Progress { set; }
        /// <summary>
        /// Gets or sets the status string.
        /// </summary>
        /// <value>The status.</value>
        string ProgressStatus { get; set; }

        /// <summary>
        /// Occurs when the progress has been changed.
        /// </summary>
        event ProgressChangedEventHandler ProgressChanged;

        /// <summary>
        /// Occurs when the status has been changed.
        /// </summary>
        event EventHandler<ProgressStatusChangedArgs> ProgressStatusChanged;
    }

    /// <summary>
    /// Event arguments indicating, that the Progress status has been changed.
    /// </summary>
    public class ProgressStatusChangedArgs
        : EventArgs
    {
        /// <summary>
        /// Contains the current Status as string
        /// </summary>
        public readonly string Status;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProgressStatusChangedArgs"/> class.
        /// </summary>
        /// <param name="status">The status.</param>
        public ProgressStatusChangedArgs(string status)
        {
            this.Status = status;
        }
    }
}

namespace TwinCAT
{
    /// <summary>
    /// Default MacIds 
    /// </summary>
    public class DefaultMacIds
    {
        /// <summary>
        /// Broadcast Address
        /// </summary>
        public static byte[] Broadcast = new byte[] { 0xff, 0xff, 0xff, 0xff, 0xff, 0xff };
        /// <summary>
        /// First Multicast Address
        /// </summary>
        public static byte[] FirstMulticast = new byte[] { 0x01, 0xff, 0x5e, 0xff, 0xff, 0xff };
        /// <summary>
        /// Empty Address
        /// </summary>
        public static byte[] Empty = new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
        /// <summary>
        /// EtherCAT Multicast Address
        /// </summary>
        public static byte[] EcatMulticast = new byte[] { 0x01, 0x01, 0x05, 0x01, 0x00, 0x00 };
        /// <summary>
        /// Network variables Multicast Address
        /// </summary>
        public static byte[] NvMulticast = new byte[] { 0x01, 0x01, 0x05, 0x04, 0x00, 0x00 };
        /// <summary>
        /// Profinet DCP Multicast Address
        /// </summary>
        public static byte[] ProfinetDcpMulticast = new byte[] { 0x01, 0x0E, 0xCF, 0x00, 0x00, 0x00 };
    }

    /// <summary>
    /// AMS/ADS Net ID
    /// </summary>
    public class AmsNetId
    {
        byte[] netId = new byte[6];

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="netId"></param>
        public AmsNetId(byte[] netId)
        {
            if (netId.Length != 6)
                throw new ArgumentException("Not a valid NetId", "netId");
            this.netId = netId;
        }

        /// <summary>
        /// Converts the netId to string
        /// </summary>
        public override string ToString()
        {
            return string.Format("{0}.{1}.{2}.{3}.{4}.{5}", netId[0], netId[1], netId[2], netId[3], netId[4], netId[5]);
        }

        /// <summary>
        /// Converts the NetId object to byte array
        /// </summary>
        /// <returns></returns>
        public byte[] ToBytes()
        {
            byte[] n = new byte[6];
            Array.Copy(netId, 0, n, 0, 6);
            return n;
        }

        /// <summary>
        /// Creates an empty NetId ("0.0.0.0.0.0")
        /// </summary>
        public static AmsNetId Empty
        {
            get { return new AmsNetId(new byte[] { 0, 0, 0, 0, 0, 0 }); }
        }

        /// <summary>
        /// Creates the local NetId ("127.0.0.1.1.1")
        /// </summary>
        public static AmsNetId LocalHost
        {
            get { return new AmsNetId(new byte[] { 127, 0, 0, 1, 1, 1 }); }
        }

        
        /// <summary>
        /// Converts the string representation of the address to <see cref="AmsNetId"/>.
        /// </summary>
        /// <param name="str"></param>
        /// <param name="netId"></param>
        /// <returns></returns>
        public static bool TryParse(string str, out AmsNetId netId)
        {
            bool ret = false;
            netId = null;

            string[] strings = str.Split(new char[] { '.' });

            if (strings.Length == 6)
            {
                byte[] bytes = new byte[6];
                for (int i = 0; i < strings.Length; i++)
                {
                    if (!byte.TryParse(strings[i], out bytes[i]))
                        return false;
                }
                netId = new AmsNetId(bytes);
                ret = true;
            }
            return ret;
        }

        /// <summary>
        /// Converts the string representation of the address to <see cref="AmsNetId"/>.
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static AmsNetId Parse(string str)
        {
            AmsNetId ret = null;
            if (!TryParse(str, out ret))
                throw new FormatException("Format of AmsNetId is not valid!");
            else
                return ret;
        }
    }
}
