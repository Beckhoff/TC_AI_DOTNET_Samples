using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Runtime.InteropServices;
using System.IO;
using System.Reflection;

namespace ScriptingTest
{
    /// <summary>
    /// Late bound configuration factory
    /// </summary>
    /// <remarks>Configuration items (and all script types) will be stored in late bound types (e.g 'dynamic' in C#)</remarks>
    public class LateBoundFactory : ConfigurationFactory
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LateBoundFactory"/> class.
        /// </summary>
        public LateBoundFactory(IVsFactory factory, bool useComRegistration)
            : base(factory,useComRegistration)
        {
        }
    }
}
