using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.IO;
using System.Diagnostics;

namespace ScriptingTest
{
    /// <summary>
    /// Script Loader class
    /// </summary>
    public static class ScriptLoader
    {
        static Func<Type, bool> _scriptFilter = null;

        /// <summary>
        /// Gets or sets the script filter.
        /// </summary>
        /// <value>The script filter.</value>
        public static Func<Type,bool> ScriptFilter
        {
            get { return _scriptFilter;  }
            set { _scriptFilter = value;  }
        }

        /// <summary>
        /// Loads the script intances from Assemblies within the Executable folder.
        /// </summary>
        /// <param name="scriptTypeFilter">The script type filter (or null if not filtered).</param>
        private static void load(Func<Type,bool> scriptTypeFilter)
        {
            List<Script> scripts = new List<Script>();

            Assembly assembly = Assembly.GetExecutingAssembly();
            // Getting all Script objects from All Dlls within the Execution Folder

            IEnumerable<Script> assScripts = null;
            //assScripts = createScripts(assembly);
            //scripts.AddRange(assScripts);

            string location = assembly.Location;
            DirectoryInfo dir = new DirectoryInfo(System.IO.Path.GetDirectoryName(location));
            FileInfo[] assemblies = dir.GetFiles("*.*"); // Getting all Dlls


            IEnumerable<FileInfo> a = assemblies.Where<FileInfo>(x =>
                {
                    StringComparer cmp = StringComparer.OrdinalIgnoreCase;
                    return ((cmp.Compare(x.Extension, ".exe") == 0)) || (cmp.Compare(x.Extension, ".dll") == 0);
                }
            );

            foreach (FileInfo file in a)
            {
                //if (file.Name != "CodeGenerationDemo.exe")
                {
                    try
                    {
                        assembly = Assembly.LoadFile(file.FullName);
                        assScripts = createScripts(assembly,scriptTypeFilter);
                        scripts.AddRange(assScripts);
                    }
                    catch (Exception ex)
                    {
                        //MessageBox.Show(ex.InnerException.Message, string.Format("Load Error in Assembly '{0}'", file.FullName), MessageBoxButton.OK, MessageBoxImage.Error);
                        Debug.Fail(string.Format("Load Error in Assembly '{0}': {1}", file.FullName, ex.Message));
                    }
                }
            }
            _scripts = scripts;
        }

        static IList<Script> _scripts = null;

        /// <summary>
        /// Gets the scripts.
        /// </summary>
        /// <value>The scripts.</value>
        public static IList<Script> Scripts
        {
            get
            {
                if (_scripts == null)
                    load(_scriptFilter);
                
                return _scripts;
            }
        }

        /// <summary>
        /// Gets the script.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public static Script GetScript(string name)
        {
            if (_scripts == null)
                load(_scriptFilter);

            Script found = _scripts.FirstOrDefault<Script>(script =>
                {
                    StringComparer cmp = StringComparer.OrdinalIgnoreCase;
                    return (cmp.Compare(script.ScriptName, name) == 0);
                }
            );

            return found;
        }

        /// <summary>
        /// Instantiates the scripts from the specified assembly.
        /// </summary>
        /// <param name="assembly">The assembly.</param>
        /// <param name="filter">Type filter (or null)</param>
        /// <returns>IEnumerable&lt;Script&gt;.</returns>
        private static IEnumerable<Script> createScripts(Assembly assembly, Func<Type,bool> filter)
        {
            List<Script> scripts = new List<Script>();
            IEnumerable<Type> types = null;

            try
            {
                types = createScriptTypes(assembly); // Getting all types the are derived from Script within the assembly
            }
            catch { }

            if (types != null)
            {
                foreach (Type tp in types)
                {
                    try
                    {
                        // Check whether the Type should be added.
                        bool use = (filter == null) || filter(tp);

                        if (use)
                        {
                            // Instantiate the Script type
                            ConstructorInfo ctor = tp.GetConstructor(System.Type.EmptyTypes);

                            if (ctor != null)
                            {
                                Script instance = (Script)ctor.Invoke(new object[0]);
                                scripts.Add(instance); // Add to the result collection.
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.Fail(string.Format("Load Error in Script '{0}': {1}", tp.FullName, ex.InnerException.Message));
                    }
                }
            }
            return scripts;
        }

        /// <summary>
        /// Determines the script types from the specified assembly.
        /// </summary>
        /// <param name="assembly">The assembly.</param>
        /// <returns></returns>
        private static IEnumerable<Type> createScriptTypes(Assembly assembly)
        {
            IEnumerable<Type> scriptTypes = assembly.GetTypes().Where(type => (typeof(Script).IsAssignableFrom(type) && !type.IsAbstract));
            return scriptTypes;
        }
    }
}
