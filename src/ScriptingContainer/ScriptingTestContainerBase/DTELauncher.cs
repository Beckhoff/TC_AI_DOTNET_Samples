using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Setup.Configuration;
using EnvDTE;
using ScriptingTest;
using Process = System.Diagnostics.Process;
using Thread = System.Threading.Thread;

namespace ScriptingTestContainerBase
{
    public static class DTELauncher
    {
        // https://docs.microsoft.com/en-Us/visualstudio/extensibility/launch-visual-studio-dte?view=vs-2022

        internal static DTE LaunchVsDte(VSInfo vs, bool launchByProgId)
        {
            DTE dte = null;
            if (launchByProgId)
            {
                Type tp = Type.GetTypeFromProgID(vs.ProgId);
                if (tp == null)
                     throw new ApplicationException("AppID '{0}' not found!");

                dte = (DTE)System.Activator.CreateInstance(tp, true);
            }
            else
            {
                string installationPath = vs.InstallationPath;
                string executablePath = Path.Combine(installationPath, @"Common7\IDE\devenv.exe");

                // Only for internal testing purposes
                string tcHive = System.Environment.GetEnvironmentVariable("TcHive");
                Process vsProcess;
                if (tcHive != null)
                {
                    vsProcess = Process.Start(executablePath,$"/rootSuffix {tcHive} /NoSplash");
                }
                else
                {
                    vsProcess = Process.Start(executablePath, "/NoSplash");
                }

                //string runningObjectDisplayName = $"VisualStudio.DTE.16.0:{vsProcess.Id}";
                string runningObjectDisplayName = $"{vs.ProgId}:{vsProcess.Id}";

                string[] searchStrings = new string[] {"!VisualStudio.DTE.", "!TcXaeShell.DTE."};
                dte = WaitForVS(runningObjectDisplayName);
            }
            return dte;
        }

        private static DTE WaitForVS(string runningObjectDisplayName)
        {
            IEnumerable<string> runningObjectDisplayNames = null;
            object runningObject;
            for (int i = 0; i < 60; i++)
            {
                try
                {
                    runningObject = GetRunningObject(runningObjectDisplayName, out runningObjectDisplayNames);
                }
                catch
                {
                    runningObject = null;
                }

                if (runningObject != null)
                {
                    return (EnvDTE.DTE) runningObject;
                }

                Thread.Sleep(millisecondsTimeout: 1000);
            }

            throw new TimeoutException($"Failed to retrieve DTE object. Current running objects: {string.Join(";", runningObjectDisplayNames)}");
        }


        //internal static EnvDTE.DTE LaunchVsDte(bool isPreRelease)
        //{
        //    ISetupInstance setupInstance = GetSetupInstance(isPreRelease);
        //    string installationPath = setupInstance.GetInstallationPath();
        //    string executablePath = Path.Combine(installationPath, @"Common7\IDE\devenv.exe");
        //    Process vsProcess = Process.Start(executablePath);
        //    string runningObjectDisplayName = $"VisualStudio.DTE.16.0:{vsProcess.Id}";

        //    return WaitForVS(runningObjectDisplayName);
        //}

        private static object GetRunningObject(string displayName, out IEnumerable<string> runningObjectDisplayNames)
        {
            IBindCtx bindContext = null;
            NativeMethods.CreateBindCtx(0, out bindContext);

            IRunningObjectTable runningObjectTable = null;
            bindContext.GetRunningObjectTable(out runningObjectTable);

            IEnumMoniker monikerEnumerator = null;
            runningObjectTable.EnumRunning(out monikerEnumerator);

            object runningObject = null;
            List<string> runningObjectDisplayNameList = new List<string>();
            IMoniker[] monikers = new IMoniker[1];
            IntPtr numberFetched = IntPtr.Zero;
            while (monikerEnumerator.Next(1, monikers, numberFetched) == 0)
            {
                IMoniker moniker = monikers[0];

                string objectDisplayName = null;
                try
                {
                    moniker.GetDisplayName(bindContext, null, out objectDisplayName);
                }
                catch (UnauthorizedAccessException)
                {
                    // Some ROT objects require elevated permissions.
                }

                if (!string.IsNullOrWhiteSpace(objectDisplayName))
                {
                    runningObjectDisplayNameList.Add(objectDisplayName);
                    if (objectDisplayName.EndsWith(displayName, StringComparison.Ordinal))
                    {
                        runningObjectTable.GetObject(moniker, out runningObject);
                        if (runningObject == null)
                        {
                            throw new InvalidOperationException($"Failed to get running object with display name {displayName}");
                        }
                    }
                }
            }

            runningObjectDisplayNames = runningObjectDisplayNameList;
            return runningObject;
        }

        private static ISetupInstance GetSetupInstance(bool isPreRelease)
        {
            return GetSetupInstances().First(i => IsPreRelease(i) == isPreRelease);
        }

        internal static IEnumerable<ISetupInstance> GetSetupInstances()
        {
            ISetupConfiguration setupConfiguration = new SetupConfiguration();
            IEnumSetupInstances enumerator = setupConfiguration.EnumInstances();

            int count;
            do
            {
                ISetupInstance[] setupInstances = new ISetupInstance[1];
                enumerator.Next(1, setupInstances, out count);
                if (count == 1 && setupInstances[0] != null)
                {
                    yield return setupInstances[0];
                }
            } while (count == 1);
        }

        private static bool IsPreRelease(ISetupInstance setupInstance)
        {
            ISetupInstanceCatalog setupInstanceCatalog = (ISetupInstanceCatalog) setupInstance;
            return setupInstanceCatalog.IsPrerelease();
        }

        private static class NativeMethods
        {
            [DllImport("ole32.dll")]
            public static extern int CreateBindCtx(uint reserved, out IBindCtx ppbc);
        }
    }
}
