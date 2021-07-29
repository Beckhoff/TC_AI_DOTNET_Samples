using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Collections;
using System.Runtime.InteropServices;

namespace Tutorial_02_ExistingVsInstance
{
    public class Helper
    {
        [DllImport("ole32.dll")]
        public static extern int GetRunningObjectTable(int reserved, out UCOMIRunningObjectTable prot);

        [DllImport("ole32.dll")]
        public static extern int CreateBindCtx(int reserved, out UCOMIBindCtx ppbc);

        public static string getFreeTcDirectory(string basePath)
        {
            int max = 1;
            List<string> directories = new List<string>(Directory.EnumerateDirectories(basePath));
            foreach(var directory in directories)
            {
                string[] dirNames = directory.Split('\\');
                if (dirNames[dirNames.Length - 1].Contains("TwinCAT Project"))
                {
                    string number = dirNames[dirNames.Length - 1].Substring(15, dirNames[dirNames.Length - 1].Length - 15);
                    if (Convert.ToInt32(number) > max)
                        max = Convert.ToInt32(number);
                }
            }
            max++;

            return "TwinCAT Project" + max.ToString();
        }

        /// <summary>
        /// Get a snapshot of the running object table (ROT).
        /// </summary>
        /// <returns>A hashtable mapping the name of the object
        //     in the ROT to the corresponding object</returns>
        public static Hashtable GetRunningObjectTable()
        {
            Hashtable result = new Hashtable();

            int numFetched;
            UCOMIRunningObjectTable runningObjectTable;
            UCOMIEnumMoniker monikerEnumerator;
            UCOMIMoniker[] monikers = new UCOMIMoniker[1];

            GetRunningObjectTable(0, out runningObjectTable);
            runningObjectTable.EnumRunning(out monikerEnumerator);
            monikerEnumerator.Reset();

            while (monikerEnumerator.Next(1, monikers, out numFetched) == 0)
            {
                UCOMIBindCtx ctx;
                CreateBindCtx(0, out ctx);

                string runningObjectName;
                monikers[0].GetDisplayName(ctx, null, out runningObjectName);

                object runningObjectVal;
                runningObjectTable.GetObject(monikers[0], out runningObjectVal);

                result[runningObjectName] = runningObjectVal;
            }

            return result;
        }


        /// <summary>
        /// Get a table of the currently running instances of the Visual Studio .NET IDE.
        /// </summary>
        /// <param name="openSolutionsOnly">Only return instances
        ///                   that have opened a solution</param>
        /// <returns>A hashtable mapping the name of the IDE
        ///       in the running object table to the corresponding
        ///                                  DTE object</returns>
        public static Hashtable GetIDEInstances(bool openSolutionsOnly, string progId)
        {
            Hashtable runningIDEInstances = new Hashtable();
            Hashtable runningObjects = GetRunningObjectTable();

            IDictionaryEnumerator rotEnumerator = runningObjects.GetEnumerator();
            while (rotEnumerator.MoveNext())
            {
                string candidateName = (string)rotEnumerator.Key;
                if (!candidateName.StartsWith("!" + progId))
                    continue;

                EnvDTE.DTE ide = rotEnumerator.Value as EnvDTE.DTE;
                if (ide == null)
                    continue;

                if (openSolutionsOnly)
                {
                    try
                    {
                        string solutionFile = ide.Solution.FullName;
                        if (solutionFile != String.Empty)
                        {
                            runningIDEInstances[candidateName] = ide;
                        }
                    }
                    catch { }
                }
                else
                {
                    runningIDEInstances[candidateName] = ide;
                }
            }
            return runningIDEInstances;
        }
    }
}
