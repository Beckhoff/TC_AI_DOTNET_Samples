using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using EnvDTE;

namespace ScriptingTest
{
    /// <summary>
    /// Running Object Table Access
    /// </summary>
    public class ROTAccess
    {
        [DllImport("ole32.dll")]
        private static extern int GetRunningObjectTable(int reserved, out IRunningObjectTable rot);
        [DllImport("ole32.dll")]
        private static extern int CreateBindCtx(int reserved, out IBindCtx bindContext);

        /// <summary>
        /// Get a snapshot of the running object table (ROT).
        /// </summary>
        /// <returns>A dictionary mapping the name of the object in the ROT to the corresponding object (COM Object)</returns>
        public static Dictionary<string, List<object>> GetRunningObjectTable()
        {
            Dictionary<string, List<object>> result = new Dictionary<string, List<object>>();

            IntPtr pFetched = new IntPtr();
            IRunningObjectTable runningObjectTable;
            IEnumMoniker monikerEnumerator;
            IMoniker[] monikers = new IMoniker[1];

            GetRunningObjectTable(0, out runningObjectTable);
            runningObjectTable.EnumRunning(out monikerEnumerator);
            monikerEnumerator.Reset();

            while (monikerEnumerator.Next(1, monikers, pFetched) == 0)
            {
                IBindCtx ctx;
                CreateBindCtx(0, out ctx);

                string runningObjectDisplayName;
                monikers[0].GetDisplayName(ctx, null, out runningObjectDisplayName);

                object runningObjectVal;
                runningObjectTable.GetObject(monikers[0], out runningObjectVal);

                List<object> x = null;
                if (!result.TryGetValue(runningObjectDisplayName, out x))
                {
                    x = new List<object>();
                    result.Add(runningObjectDisplayName, x);
                }
                x.Add(runningObjectVal);
            }
            return result;
        }


        /// <summary>
        /// Get a table of the currently running instances of the Visual Studio .NET IDE.
        /// </summary>
        /// <returns>A dictionary mapping common information about the DTE object to the EnvDte.DTE object itself</returns>
        public static Dictionary<ROTDteInfo, DTE> GetRunningDTETable()
        {
            Dictionary<string, DTE> runningIDEInstances = new Dictionary<string, DTE>();
            Dictionary<string, List<object>> runningObjects = GetRunningObjectTable();

            IEnumerable<KeyValuePair<string, List<object>>> objTable = runningObjects.Where<KeyValuePair<string, List<object>>>((entry) => { ROTDteInfo info = null; return ROTDteInfo.TryParse(entry.Key, out info); });

            Dictionary<ROTDteInfo, DTE> ret = new Dictionary<ROTDteInfo, DTE>();

            foreach (KeyValuePair<string, List<object>> entry in objTable)
            {
                string displayName = entry.Key;
                Debug.Assert(entry.Value.Count == 1);

                if (entry.Value.Count > 0)
                {
                    DTE dte = (DTE)entry.Value[0];
                    string solutionPath = dte.Solution.FullName;

                    ROTDteInfo info = ROTDteInfo.Parse(entry.Key);
                    info.SolutionPath = solutionPath;
                    ret.Add(info, dte);
                }
            }
            return ret;
        }

        /// <summary>
        /// Gets the DTE Object which has opened a solution with the specified name.
        /// </summary>
        /// <param name="solutionName">Name of the solution (without path, without extension)</param>
        /// <returns>The DTE Object</returns>
        public static DTE GetActiveDTE(string solutionName)
        {
            KeyValuePair<ROTDteInfo, DTE> foundEntry = GetRunningDTETable().First<KeyValuePair<ROTDteInfo, DTE>>((entry) => { return (StringComparer.OrdinalIgnoreCase.Compare(entry.Key.SolutionName, solutionName) == 0); });
            return foundEntry.Value;
        }
    }

    /// <summary>
    /// DTE Information Object
    /// </summary>
    public class ROTDteInfo
    {
        /// <summary>
        /// The version of the DTE
        /// </summary>
        public readonly Version Version;
        /// <summary>
        /// The process identifier of the devenv process
        /// </summary>
        public readonly int ProcessId;

        /// <summary>
        /// Gets the solution path of the Info object
        /// </summary>
        /// <value>
        /// The solution path.
        /// </value>
        public string SolutionPath { get; internal set; }

        /// <summary>
        /// Gets a value indicating whether this instance represents an TcXaeShell instance
        /// </summary>
        /// <value><c>true</c> if this instance is shell; otherwise, <c>false</c>.</value>
        public bool IsShell { get; internal set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ROTDteInfo" /> class.
        /// </summary>
        /// <param name="version">The version.</param>
        /// <param name="solutionPath">The solution path.</param>
        /// <param name="processId">The process identifier.</param>
        /// <param name="isShell">if set to <c>true</c> [is shell].</param>
        public ROTDteInfo(Version version, string solutionPath, int processId, bool isShell)
        {
            this.Version = version;
            this.SolutionPath = solutionPath;
            this.ProcessId = processId;
            this.IsShell = isShell;
        }

        /// <summary>
        /// Gets the name of the solution.
        /// </summary>
        /// <value>
        /// The name of the solution.
        /// </value>
        public string SolutionName
        {
            get
            {
                if (!string.IsNullOrEmpty(this.SolutionPath))
                    return Path.GetFileNameWithoutExtension(this.SolutionPath);
                else
                    return string.Empty;
            }
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            if (IsShell)
            {
                return string.Format("!TcXaeShell.DTE.{0}:{1}", this.Version, this.ProcessId);
            }
            else
            {
                return string.Format("!VisualStudio.DTE.{0}:{1}", this.Version, this.ProcessId);
            }
        }

        /// <summary>
        /// Parses the specified string.
        /// </summary>
        /// <param name="str">The string.</param>
        /// <returns></returns>
        /// <exception cref="System.FormatException"></exception>
        public static ROTDteInfo Parse(string str)
        {
            ROTDteInfo info = null;
            if (!TryParse(str, out info))
            {
                throw new FormatException();
            }
            return info;
        }

        /// <summary>
        /// Tries to parse the specified string to an <see cref="ROTDteInfo"/> instance.
        /// </summary>
        /// <param name="str">The string.</param>
        /// <param name="obj">The object.</param>
        /// <returns>true if succeeded, false if not.</returns>
        public static bool TryParse(string str, out ROTDteInfo obj)
        {
            string[] searchStrings = new string[] { "!VisualStudio.DTE.", "!TcXaeShell.DTE." };
            bool found = false;
            obj = null;

            for (int i = 0; i < searchStrings.Length; i++)
            {
                string searchStr = searchStrings[i];
                bool isShell = (i == 1);

                int index = searchStr.Length;
                int index2 = -1;

                if (str.StartsWith(searchStr))
                {
                    index2 = str.LastIndexOf(':');
                    int processId = -1;

                    if (index2 < 0)
                        index2 = str.Length;
                    else
                    {
                        processId = int.Parse(str.Substring(index2 + 1));
                    }

                    string versionStr = str.Substring(index, index2 - index);
                    Version version = Version.Parse(versionStr);
                    obj = new ROTDteInfo(version, string.Empty, processId, isShell);
                    found = true;
                    break;
                }
                else
                {
                    found = false;
                }
            }
            return found;
        }
    }
}
