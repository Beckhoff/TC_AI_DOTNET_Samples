using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using EnvDTE;
using EnvDTE80;
using EnvDTE90;
using EnvDTE90a;
using EnvDTE100;
using System.Runtime.InteropServices;
using System.IO;
using TCatSysManagerLib;
using System.Xml;
using System.Diagnostics;
using System.Reflection;

namespace ScriptingTest
{
    /// <summary>
    /// Configuration Factory for Early Binding
    /// </summary>
    /// <remarks>DTE and Solution objects are stored in static types.</remarks>
    public class EarlyBoundFactory : ConfigurationFactory
    {
        ///// <summary>
        ///// DTE object
        ///// </summary>
        //DTE2 _dte = null;

        ///// <summary>
        ///// Solution object
        ///// </summary>
        //Solution4 _sln = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="EarlyBoundFactory"/> class.
        /// </summary>
        public EarlyBoundFactory(IVsFactory factory)
            : base(factory)
        {
        }

        ///// <summary>
        ///// Handler function to create the script configuration.
        ///// </summary>
        //protected override void OnCreateConfiguration(/*string projectTemplate*/)
        //{
        //    _dte = base.DTE;
        //    _sln = base.Solution;
        //}

        ///// <summary>
        ///// Handler function for Configuration cleanup.
        ///// </summary>
        //protected override void OnClosingConfiguration()
        //{
        //    // Removing the local typed objects.
        //    this._dte = null;
        //    this._sln = null;

        //    base.OnClosingConfiguration();
        //}
    }
}
