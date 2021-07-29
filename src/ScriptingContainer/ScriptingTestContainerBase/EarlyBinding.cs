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
        /// <summary>
        /// Initializes a new instance of the <see cref="EarlyBoundFactory"/> class.
        /// </summary>
        public EarlyBoundFactory(IVsFactory factory, bool useComRegistration)
            : base(factory, useComRegistration)
        {
        }
    }
}
