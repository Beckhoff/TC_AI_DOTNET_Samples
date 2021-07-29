using System;
using System.IO;
using ScriptingTest;
using TwinCAT.SystemManager;

namespace ScriptingTest.LateBinding
{
    /// <summary>
    /// Demonstrates the creation of an EtherCAT IO Subtree and the linking with PLC Symbols (Late Binding).
    /// </summary>
    public class NovRamDevice
        : ScriptLateBound
    {
        private dynamic systemManager = null;
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

            dynamic devices = systemManager.LookupTreeItem("TIID");
            dynamic novRam = devices.CreateChild("NovDevice1", 10, null, null);

            dynamic inputs = systemManager.LookupTreeItem(novRam.PathName + "^" + "Inputs");
            dynamic outputs = systemManager.LookupTreeItem(novRam.PathName + "^" + "Outputs");

            dynamic iVar1 = inputs.CreateChild("iVar1", 0, null, "INT");
            dynamic oVar1 = outputs.CreateChild("iVar2", 0, null, "INT");

            systemManager.LinkVariables(iVar1.PathName, oVar1.PathName, 0, 0, 0);

            string xml = novRam.ProduceXml();

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
                return "Demonstrates the creation of an NovRam Device and Sub-Symbols (Late Binding).";
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
                return "NovRam, Adding Symbols";
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
