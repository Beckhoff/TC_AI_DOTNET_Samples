using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ScriptingTest
{
    /// <summary>
    /// TwinCAT Configuration Factory interface
    /// </summary>
    public interface IConfigurationFactory
    {
        /// <summary>
        /// Initializes the factory (Creates the configuration)
        /// </summary>
        void Initialize(IContext scriptContext);

        /// <summary>
        /// Executes the script.
        /// </summary>
        /// <param name="script">The script to execute</param>
        void ExecuteScript(Script script /* IDictionary<string,dynamic> parameters, IWorker worker*/);

        /// <summary>
        /// Closes the configuration.
        /// </summary>
        void Cleanup();

        /// <summary>
        /// Gets or sets the Application ID for the TwinCAT XAE Shell
        /// </summary>
        /// <value>The app ID.</value>
        string AppID
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the IDE will be shown during Script processing
        /// </summary>
        /// <value><c>true</c> if [show application]; otherwise, <c>false</c>.</value>
        bool IsIdeVisible { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the IDE instance will left to UserControl after script execution.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is IDE user control; otherwise, <c>false</c>.
        /// </value>
        bool IsIdeUserControl { get; set; }

        /// <summary>
        /// Gets a value indicating whether an TwinCAT XAE Configuration is opened by this factory class.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is configuration open; otherwise, <c>false</c>.
        /// </value>
        bool IsConfigurationOpen { get; }
        /// <summary>
        /// Gets a value indicating whether this instance is generating.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is generating; otherwise, <c>false</c>.
        /// </value>
        bool IsExecuting { get; }

        /// <summary>
        /// Gets the created DTE object.
        /// </summary>
        /// <value>The DTE.</value>
        dynamic DTE { get; }

        /// <summary>
        /// Gets created solution object.
        /// </summary>
        /// <value>The solution.</value>
        dynamic Solution { get; }

        /// <summary>
        /// Gets the Visual Studio Factory
        /// </summary>
        /// <value>
        /// The Visual Studio Factory
        /// </value>
        IVsFactory VsFactory { get; }

        /// <summary>
        /// Gets the Visual Studio Process ID.
        /// </summary>
        /// <value>The Visual Studio process identifier.</value>
        int VsProcessId { get;  }
    }
}
