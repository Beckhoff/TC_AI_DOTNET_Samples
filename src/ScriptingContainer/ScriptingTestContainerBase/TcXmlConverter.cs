    using System;
    using System.Xml;
    using System.IO;
    using System.Collections;

    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Globalization;

namespace ScriptingTest
{
    /// <summary>
    /// Xml Converter class for TwinCAT specifics
    /// </summary>
    public class TcXmlConvert
    {
        /// <summary>
        /// Reads the element content as bin hex.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <returns></returns>
        public static byte[] ReadElementContentAsBinHex(XmlReader reader)
        {
            byte[] buffer = new byte[1024];
            List<byte> ret = new List<byte>();

            int allBytes = 0;
            int readBytes = 0;
            while ((readBytes = reader.ReadElementContentAsBinHex(buffer, 0, 1024)) > 0)
            {
                allBytes += readBytes;

                for (int i = 0; i < readBytes; i++)
                {
                    ret.Add(buffer[i]);
                }
            }

            Debug.Assert(allBytes == ret.Count);

            byte[] r = new byte[allBytes];
            ret.CopyTo(r);

            reader.MoveToContent();
            return r;
        }

        /// <summary>
        /// Reads an hexadecimal integer value from XML string
        /// </summary>
        /// <param name="str">The string</param>
        /// <returns>The value</returns>
        public static int ReadHexIntValue(string str)
        {
            if (str.StartsWith("#x"))
            {
                string val = str.Substring(2);
                return int.Parse(val, NumberStyles.HexNumber);
            }
            else
            {
                return int.Parse(str);
            }
        }

        /// <summary>
        /// Reads an hexadecimal value from XML string
        /// </summary>
        /// <param name="str">The string</param>
        /// <returns>The value</returns>
        public static uint ReadHexUIntValue(string str)
        {
            if (str.StartsWith("#x"))
            {
                string val = str.Substring(2);
                return uint.Parse(val, NumberStyles.HexNumber);
            }
            else
            {
                return uint.Parse(str);
            }
        }

        /// <summary>
        /// Converts an hexadecimal value to XML string
        /// </summary>
        /// <param name="val">The value</param>
        /// <returns>The string representation</returns>
        public static string WriteHexUIntValue(uint val)
        {
            return string.Format("#x{0}", val.ToString("X8"));
        }

        /// <summary>
        /// Converts an hexadecimal value to XML string
        /// </summary>
        /// <param name="val">The value</param>
        /// <returns>The string representatino</returns>
        public static string WriteHexValue(int val)
        {
            return string.Format("#x{0}", val.ToString("X8"));
        }

        /// <summary>
        /// Reads an hexadecimal integer value from XML string
        /// </summary>
        /// <param name="str">The string</param>
        /// <returns>The value</returns>
        public static ulong ReadHexULongValue(string str)
        {
            if (str.StartsWith("#x"))
            {
                string val = str.Substring(2);
                return ulong.Parse(val, NumberStyles.HexNumber);
            }
            else
            {
                return ulong.Parse(str);
            }
        }

        /// <summary>
        /// Converts an hexadecimal value to XML string
        /// </summary>
        /// <param name="val">The value</param>
        /// <returns>The string representation</returns>
        public static string WriteHexULongValue(ulong val)
        {
            return string.Format("#x{0}", val.ToString("X16"));
        }

        /// <summary>
        /// Reads a specified booleaan (from "True", "False", "1" and "0");
        /// </summary>
        /// <param name="str">The STR.</param>
        /// <returns></returns>
        public static bool ReadBoolean(string str)
        {
            bool ret = false;

            if (bool.TryParse(str, out ret))
                return ret;
            else
            {
                int i = 0;

                i = int.Parse(str);

                if (i != 0)
                {
                    ret = true;
                }
                else
                {
                    ret = false;
                }
                return ret;
            }
        }
    }
}

