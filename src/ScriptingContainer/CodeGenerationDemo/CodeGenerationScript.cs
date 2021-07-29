using System;
using System.IO;
using EnvDTE;
using EnvDTE100;
using EnvDTE80;
using TCatSysManagerLib;
using TwinCAT.SystemManager;
using System.Diagnostics;
using System.Timers;
using ScriptingTest;
using System.Xml;
using System.Collections.Generic;
using System.Reflection;
using Scripting.CSharp;

namespace CodeGenerationDemo
{
    /// <summary>
    /// Demonstrates the generation + compilation of PLC projects
    /// </summary>
    public abstract class CodeGenerationBaseScript
        : ScriptEarlyBound
    {
        protected ITcSysManager4 systemManager = null;
        protected Project project = null;

        /// <summary>
        /// Handler function Initializing the Script (Configuration preparations)
        /// </summary>
        /// <param name="context"></param>
        /// <remarks>Usually used to to the open a prepared or new XAE configuration</remarks>
        protected override void OnInitialize(IContext context)
        {
            base.OnInitialize(context);
        }

        protected override void OnSolutionCreated()
        {
            string projectTemplate = null;

            if (string.IsNullOrEmpty(_context.ProjectTemplate))
                projectTemplate = VsXaeTemplatePath;
            else
                projectTemplate = Path.Combine(ApplicationDirectory, _context.ProjectTemplate);
            
            bool exists = File.Exists(projectTemplate);

            this.project = (Project)CreateNewProject(this.ScriptName, projectTemplate);
            this.systemManager = (ITcSysManager4)project.Object;
            //base.OnSolutionCreated();
        }

        /// <summary>
        /// Cleaning up the XAE configuration after script execution.
        /// </summary>
        /// <param name="worker">The worker.</param>
        protected override void OnCleanUp(IWorker worker)
        {
            base.OnCleanUp(worker);
        }

        /// <summary>
        /// Insertion Mode for creating PLC projects.
        /// </summary>
        public enum CreatePlcMode
        {
            /// <summary>
            /// Copies a PLC Project
            /// </summary>
            Copy = 0,
            /// <summary>
            /// Moves a PLC Project
            /// </summary>
            Move = 1,
            /// <summary>
            /// References a PLC Project
            /// </summary>
            Reference = 2
        }
        
        public override string Description
        {
            get { return this.ScriptName; }
        }

        protected void CreateMotion(IWorker worker)
        {
            if (worker.CancellationPending)
                throw new Exception("Execution cancelled!");

            OrderScriptContext context = (OrderScriptContext)_context;
            ConfigurationInfo configurationInfo = context.Order.ConfigurationInfo;

            ITcSmTreeItem ncConfig = systemManager.LookupTreeItem("TINC");

            if (configurationInfo.MotionTasks != null)
            {
                foreach (TaskInfo taskInfo in configurationInfo.MotionTasks)
                {
                    ITcSmTreeItem task = null;

                    worker.ProgressStatus = string.Format("Creating Motion Task '{0}'", taskInfo.Name);

                    if (!TryLookupChild(ncConfig, taskInfo.Name, out task))
                    {
                        task = ncConfig.CreateChild(taskInfo.Name, 1);
                    }
                    ITcSmTreeItem axes = null;
                    TryLookupChild(task, "Axes", out axes);

                    if (axes == null)
                        TryLookupChild(task, "Achsen", out axes);

                    foreach (AxisInfo axisInfo in taskInfo.Axes)
                    {
                        if (worker.CancellationPending)
                            throw new Exception("Execution cancelled!");

                        ITcSmTreeItem axis = null;
                        worker.ProgressStatus = string.Format("Creating Axis '{0}'", axisInfo.Name);

                        if (!TryLookupChild(axes, axisInfo.Name, out axis))
                        {
                            axis = axes.CreateChild(axisInfo.Name, 1);
                            ConsumeTemplate(axis, axisInfo.TemplatePath);
                        }
                    }
                }
            }
        }

        protected void CreateMappings(IWorker worker)
        {
            OrderScriptContext context = (OrderScriptContext)_context;
            ConfigurationInfo configurationInfo = context.Order.ConfigurationInfo;

            MappingsInfo info = configurationInfo.Mappings;

            if (info != null)
            {
                if (worker.CancellationPending)
                    throw new Exception("Execution cancelled!");

                worker.ProgressStatus = "Generating mappings ...";
                ConsumeMappings(info.TemplatePath);
            }
        }

        protected ITcSmTreeItem CreatePlcProject(IWorker worker)
        {
            if (worker.CancellationPending)
                throw new Exception("Execution cancelled!");

            ITcSmTreeItem plcProject = null;
            OrderScriptContext context = (OrderScriptContext)_context;
            ConfigurationInfo configurationInfo = context.Order.ConfigurationInfo;

            if (context.Order.ConfigurationInfo.PlcObjects != null)
            {
                string plcProjectName = configurationInfo.PlcProjectName;
                ITcSmTreeItem plcConfig = systemManager.LookupTreeItem("TIPC");

                worker.ProgressStatus = string.Format("Creating empty PLC Project '{0}' ...", plcProjectName);
                ITcSmTreeItem plcProjectRoot = plcConfig.CreateChild(plcProjectName, 0, "", vsXaePlcEmptyTemplateName);

                ITcPlcProject plcProjectRootIec = (ITcPlcProject)plcProjectRoot;
                plcProjectRootIec.BootProjectAutostart = true;
                plcProjectRootIec.GenerateBootProject(true);

                plcProject = plcProjectRoot.LookupChild(plcProjectName + " Project");

                foreach (PlcObjectInfo plcObjectInfo in context.Order.ConfigurationInfo.PlcObjects)
                {
                    if (worker.CancellationPending)
                        throw new Exception("Execution cancelled!");

                    switch (plcObjectInfo.Type)
                    {
                        case PlcObjectType.DataType:
                            createWorksheet((WorksheetInfo)plcObjectInfo, plcProject, worker);
                            break;
                        case PlcObjectType.Library:
                            createLibrary((LibraryInfo)plcObjectInfo, plcProject, worker);
                            break;
                        case PlcObjectType.Placeholder:
                            createPlaceholder((PlaceholderInfo)plcObjectInfo, plcProject, worker);
                            break;
                        case PlcObjectType.POU:
                            createWorksheet((WorksheetInfo)plcObjectInfo, plcProject, worker);
                            break;
                        case PlcObjectType.Itf:
                            createWorksheet((WorksheetInfo)plcObjectInfo, plcProject, worker);
                            break;
                        case PlcObjectType.Gvl:
                            createWorksheet((WorksheetInfo)plcObjectInfo, plcProject, worker);
                            break;
                        default:
                            Debug.Fail("");
                            break;
                    }
                }

                ITcSmTreeItem realtimeTasks = systemManager.LookupTreeItem("TIRT");
                ITcSmTreeItem rtTask = realtimeTasks.CreateChild("PlcTask", TreeItemType.Task.AsInt32());

                ITcSmTreeItem taskRef = null;
                worker.ProgressStatus = "Linking PLC instance with task 'PlcTask' ...";

                if (!TryLookupChild(plcProject, "PlcTask", out taskRef))
                {
                    if (worker.CancellationPending)
                        throw new Exception("Execution cancelled!");

                    taskRef = plcProject.CreateChild("PlcTask", TreeItemType.PlcTask.AsInt32(), "", "MAIN");
                }
            }
            return plcProject;
        }

        private ITcSmTreeItem createWorksheet(WorksheetInfo info, ITcSmTreeItem plcProject, IWorker worker)
        {
            string[] plcSide = info.PlcPath.Split('/','\\','^');

            ITcSmTreeItem parent = plcProject;
            ITcSmTreeItem ret = null;

            for (int index = 0; index < plcSide.Length; index++)
            {
                // Create Folder if not exist
                ITcSmTreeItem child = null;

                if (!TryLookupChild(parent, plcSide[index], out child))
                {
                    child = createPlcFolder(parent, plcSide[index], null, worker);
                }
                parent = child;
            }

            ret = createWorksheet2(info, parent, worker);
            return ret;
        }

        private void createLibrary(LibraryInfo info, ITcSmTreeItem plcProject, IWorker worker)
        {
            worker.ProgressStatus = string.Format("Adding Library '{0}' ...", info.LibraryName);

            ITcSmTreeItem referencesItem = plcProject.LookupChild("References");
            ITcPlcLibraryManager libraryManager = (ITcPlcLibraryManager)referencesItem;
            libraryManager.AddLibrary(info.LibraryName);
        }

        private void createPlaceholder(PlaceholderInfo info, ITcSmTreeItem plcProject, IWorker worker)
        {
            worker.ProgressStatus = string.Format("Adding Placeholder '{0}' ...", info.PlaceholderName);

            ITcSmTreeItem referencesItem = plcProject.LookupChild("References");
            ITcPlcLibraryManager libraryManager = (ITcPlcLibraryManager)referencesItem;
            libraryManager.AddPlaceholder(info.PlaceholderName);
        }


        protected ITcSmTreeItem CreateHardware(IWorker worker, bool scanHardware, bool simulation)
        {
            OrderScriptContext context = (OrderScriptContext)_context;
            ConfigurationInfo configurationInfo = context.Order.ConfigurationInfo;
            HardwareInfo hardware = configurationInfo.Hardware;
            ITcSmTreeItem device = null;

            if (hardware != null)
            {

                ITcSmTreeItem devices = systemManager.LookupTreeItem("TIID");                       // Getting IO-Configuration

                if (worker.CancellationPending)
                    throw new Exception("Execution cancelled!");

                // Scans the Fieldbus interfaces and adds an EtherCAT Device.
                string deviceName = "EtherCAT Master";
                worker.ProgressStatus = string.Format("Creating device '{0}'", deviceName);
                device = Helper.CreateEthernetDevice(this.systemManager, DeviceType.EtherCAT_DirectMode, deviceName, worker);

                ITcSmTreeItem parent = device;

                foreach (BoxInfo boxInfo in hardware.Boxes)
                {
                    if (worker.CancellationPending)
                        throw new Exception("Execution cancelled!");

                    ITcSmTreeItem box = CreateBox(parent, boxInfo, worker);
                }
            }
            return device;
        }

        private ITcSmTreeItem CreateBox(ITcSmTreeItem parent, BoxInfo boxInfo, IWorker worker)
        {
            worker.ProgressStatus = string.Format("Creating Box '{0}' ({1})...", boxInfo.Name,boxInfo.Type);
            ITcSmTreeItem ret = parent.CreateChild(boxInfo.Name, (int)BoxType.EtherCAT_EXXXXX, "", boxInfo.Type);

            if (boxInfo.ChildBoxes != null)
            {
                foreach (BoxInfo child in boxInfo.ChildBoxes)
                {
                    ITcSmTreeItem tcChild = CreateBox(ret, child, worker);
                }

            }

            return ret;
        }

        #region Helpers
        private ITcSmTreeItem createPlcFolder(ITcSmTreeItem parent, string folderName, string before, IWorker worker)
        {
            worker.ProgressStatus = string.Format("Creating Folder '{0}' ...", folderName);
            ITcSmTreeItem item = parent.CreateChild(folderName, TreeItemType.PlcFolder.AsInt32(), before, null);
            return item;
        }

        private ITcSmTreeItem createWorksheet2(WorksheetInfo info, ITcSmTreeItem parent, IWorker worker)
        {
            string template = Path.Combine(ApplicationDirectory, info.TemplatePath);

            XmlDocument doc = new XmlDocument();
            doc.Load(template);

            ITcSmTreeItem ret = null;

            switch (info.Type)
            {
                case PlcObjectType.DataType:
                    ret = createDut((DataTypeInfo)info,parent, worker, doc);
                    break;
                case PlcObjectType.POU:
                    ret = createPou((POUInfo)info, parent, worker, doc);
                    break;
                case PlcObjectType.Itf:
                    ret = createItf((ItfInfo)info,parent, worker, doc);
                    break;
                case PlcObjectType.Gvl:
                    ret = createGvl((GvlInfo)info,parent,worker,doc);
                    break;
                default:
                    Debug.Fail("");
                    break;
            }

            return ret;
        }

        private ITcSmTreeItem createPou(POUInfo pouInfo, ITcSmTreeItem parent, IWorker worker, XmlDocument doc)
        {
            XmlNode pouNode = doc.SelectSingleNode("TcPlcObject/POU");
            string pouName = pouNode.Attributes["Name"].Value;

            worker.ProgressStatus = string.Format("Creating POU '{0}' ...", pouName);

            ITcSmTreeItem item = null;

            if (!TryLookupChild(parent, pouName, out item))
            {
                item = parent.CreateChild(pouName, TreeItemType.PlcPouFunctionBlock.AsInt32(),"",null);
            }
            ITcPlcPou pou = (ITcPlcPou)item;

            pou.DocumentXml = doc.OuterXml;
            return item;
        }

        private ITcSmTreeItem createItf(ItfInfo itfInfo, ITcSmTreeItem parent, IWorker worker, XmlDocument doc)
        {
            XmlNode itfNode = doc.SelectSingleNode("TcPlcObject/Itf");
            string itfName = itfNode.Attributes["Name"].Value;

            worker.ProgressStatus = string.Format("Creating Interface '{0}' ...", itfName);

            ITcSmTreeItem item = null;
            XmlElement node = (XmlElement)doc.SelectSingleNode("TcPlcObject/Itf/Declaration");

            string declString = node.InnerText;

            if (!TryLookupChild(parent, itfName, out item))
            {
                item = parent.CreateChild(itfName, TreeItemType.PlcInterface.AsInt32(),"",declString);
            }

            //Debug.Fail("");
            ITcXmlDocument xmlDoc = (ITcXmlDocument)item;
            xmlDoc.DocumentXml = doc.OuterXml;
            ITcPlcDeclaration decl = (ITcPlcDeclaration)item;
            //decl.DeclarationText = node.InnerText;
            return item;
        }

        private ITcSmTreeItem createGvl(GvlInfo gvlInfo, ITcSmTreeItem parent, IWorker worker, XmlDocument doc)
        {
            XmlNode gvlNode = doc.SelectSingleNode("TcPlcObject/GVL");
            string gvlName = gvlNode.Attributes["Name"].Value;

            worker.ProgressStatus = string.Format("Creating GlobalVariable Sheet '{0}' ...", gvlName);

            ITcSmTreeItem item = null;

            if (!TryLookupChild(parent, gvlName, out item))
            {
                item = parent.CreateChild(gvlName, TreeItemType.PlcGvl.AsInt32());
            }

            ITcXmlDocument xmlDoc = (ITcXmlDocument)item;
            ITcPlcDeclaration decl = (ITcPlcDeclaration)item;

            XmlElement node = (XmlElement)doc.SelectSingleNode("TcPlcObject/GVL/Declaration");
            decl.DeclarationText = node.InnerText;
            return item;
        }

        private ITcSmTreeItem createDut(DataTypeInfo info, ITcSmTreeItem parent, IWorker worker, XmlDocument doc)
        {
            XmlNode dutNode = doc.SelectSingleNode("TcPlcObject/DUT");
            string typeName = dutNode.Attributes["Name"].Value;
           
            worker.ProgressStatus = string.Format("Creating Type '{0}' ...", typeName);

            XmlNode declNode = dutNode.SelectSingleNode("Declaration");
            string declaration = string.Empty;
            declaration = declNode.InnerText;

            ITcSmTreeItem dataTypeItem = parent.CreateChild(typeName, TreeItemType.PlcDutStruct.AsInt32(), "", declaration);
            
            ITcPlcDeclaration decl = (ITcPlcDeclaration)dataTypeItem;
            ITcXmlDocument tcDoc = (ITcXmlDocument)dataTypeItem;

            string xml = tcDoc.DocumentXml;

            return dataTypeItem;
        }

        private bool TryLookupChild(ITcSmTreeItem parent, string childName, out ITcSmTreeItem child)
        {
            foreach (ITcSmTreeItem c in parent)
            {
                if (c.Name == childName)
                {
                    child = c;
                    return true;
                }
            }
            child = null;
            return false;
        }

        private bool TryLookupChild2(ITcSmTreeItem parent, string childName, out ITcSmTreeItem child)
        {
            try
            {
                child = parent.LookupChild(childName);
                return true;
            }
            catch
            {
                child = null;
                return false;
            }
        }

        private bool TryLookupChild3(ITcSmTreeItem parent, string childName, out ITcSmTreeItem child)
        {
            string parentPath = parent.PathName;
            string path = parentPath + "^" + childName;

            return TryLookupTreeItem(path, out child);

        }


        private bool TryLookupTreeItem(string itemPath, out ITcSmTreeItem treeItem)
        {
            try
            {
                treeItem = this.systemManager.LookupTreeItem(itemPath);
                return true;
            }
            catch
            {
                treeItem = null;
                return false;
            }
        }

        #endregion


        private void ConsumeTemplate(ITcSmTreeItem item, string templatePath)
        {
            string templ = Path.Combine(ApplicationDirectory,templatePath);

            XmlTextReader reader = new XmlTextReader(templ);
            reader.MoveToContent();
            item.ConsumeXml(reader.ReadOuterXml());
        }

        private void ConsumeMappings(string templatePath)
        {
            string templ = Path.Combine(ApplicationDirectory,templatePath);
            XmlTextReader reader = new XmlTextReader(templ);
            reader.MoveToContent();
            systemManager.ConsumeMappingInfo(reader.ReadOuterXml());
        }

        private bool CompileProject(IWorker worker, out ErrorItems errors)
        {
            bool buildSucceeded = false;

            if (worker.CancellationPending)
                throw new Exception("Execution cancelled!");

            worker.ProgressStatus = "Compiling project ...";

            dte.Solution.SolutionBuild.Build(true);
            buildSucceeded = waitForBuildAndCheckErrors(worker, out errors);

            if (!buildSucceeded)
            {
                int overallMessages = errors.Count;

                int errorCount = 0;
                int warningCount = 0;
                int messageCount = 0;

                for (int i = 1; i <= overallMessages; i++) // List is starting from 1!!!
                {
                    ErrorItem item = errors.Item(i);

                    if (item.ErrorLevel == vsBuildErrorLevel.vsBuildErrorLevelHigh)
                    {
                        errorCount++;
                        worker.ProgressStatus = "Compiler error: " + item.Description;
                    }
                    else if (item.ErrorLevel == vsBuildErrorLevel.vsBuildErrorLevelMedium)
                    {
                        warningCount++;
                        worker.ProgressStatus = "Compiler warning: " + item.Description;
                    }
                    else if (item.ErrorLevel == vsBuildErrorLevel.vsBuildErrorLevelLow)
                    {
                        messageCount++;
                        worker.ProgressStatus = "Compiler message: " + item.Description;
                    }

                }
            }
            else
            {
                worker.ProgressStatus = "Build Succeeded!";
            }

            if (worker.CancellationPending)
                throw new Exception("Execution cancelled!");

            return buildSucceeded;
        }

        private bool waitForBuildAndCheckErrors(IWorker worker, out ErrorItems errorItems)
        {
            bool buildSucceeded = false;
            vsBuildState state = dte.Solution.SolutionBuild.BuildState;

            while (state == vsBuildState.vsBuildStateInProgress)
            {
                System.Threading.Thread.Sleep(500);

                if (worker.CancellationPending)
                {
                    dte.ExecuteCommand("Build.Cancel");
                    
                }
                state = dte.Solution.SolutionBuild.BuildState;
            }

            buildSucceeded = (dte.Solution.SolutionBuild.LastBuildInfo == 0 && state == vsBuildState.vsBuildStateDone);

            // The ErrorList is not significant for the succeeded Build!!!
            // Because the PLC Project can contain errors within not used types.
            // Relevant is only the "LastBuildInfo" object that contains the numer of failed project compilations!

            ErrorList errorList = dte.ToolWindows.ErrorList;
            errorItems = errorList.ErrorItems;

            
            return buildSucceeded;
        }
    }
}
