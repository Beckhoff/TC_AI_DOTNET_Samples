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

namespace Scripting.CSharp
{
    /// <summary>
    /// Demonstrates the generation + compilation of PLC projects. 
    /// The method Execute() will be called by ScriptingContainer and will execute the actual script code.
    /// </summary>
    public class GeneratePlcProject
        : ScriptEarlyBound
    {
        ITcSysManager4 systemManager = null;
        Project project = null;

        /// <summary>
        /// Handler function Initializing the Script (Configuration preparations)
        /// </summary>
        /// <param name="context"></param>
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
            this.project = (Project)CreateNewProject();
            this.systemManager = (ITcSysManager4)project.Object;
            base.OnSolutionCreated();
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

        private void browse(ITcSmTreeItem root, IWorker worker)
        {
            string name = root.Name;
            string pathName = root.PathName;
            TreeItemType itemType = (TreeItemType)root.ItemType;
            string subTypeName = root.ItemSubTypeName;

            worker.ProgressStatus = string.Format("Browsing node '{0}'", pathName);

            string xml = root.ProduceXml();

            // Iteration over each node and produce output for demo purposes
            foreach (ITcSmTreeItem3 child in root)
            {
                browse(child, worker);
            }
        }

        /// <summary>
        /// Handler function Executing the Script code.
        /// </summary>
        /// <param name="worker">The worker.</param>
        protected override void OnExecute(IWorker worker)
        {   
            worker.Progress = 0;
            if (worker.CancellationPending)
                throw new Exception("Cancelled");

            /* =================================================================================================
             * Get references to PLC and IO nodes
             * ================================================================================================= */
            ITcSmTreeItem plcConfig = systemManager.LookupTreeItem("TIPC"); // Getting PLC Configuration item
            ITcSmTreeItem devices = systemManager.LookupTreeItem("TIID"); // Getting IO-Configuration item

            /* =================================================================================================
             * Attach an empty PLC Project using the "TwinCAT Project" template
             * ================================================================================================= */
            worker.ProgressStatus = "Creating empty PLC Project ...";
            ITcSmTreeItem plcGenerated = plcConfig.CreateChild("PlcGenerated", 0, "", vsXaePlcStandardTemplateName);
           
            worker.ProgressStatus = "PLC Project created ...";
            worker.Progress = 10;


            /* =================================================================================================
             * Get references to attached PLC Project and some of its childs
             * ================================================================================================= */
            ITcSmTreeItem plcProjectRootItem = systemManager.LookupTreeItem("TIPC^PlcGenerated"); // Plc Project Root (XAE Base side)
            ITcSmTreeItem plcProjectItem = systemManager.LookupTreeItem("TIPC^PlcGenerated^PlcGenerated Project"); // PlcProject (PlcControl side) determined via LookupTreeItem
            ITcSmTreeItem plcInstancesItem = systemManager.LookupTreeItem("TIPC^PlcGenerated^PlcGenerated Instance"); // Instances
            ITcSmTreeItem plcPousItem = systemManager.LookupTreeItem("TIPC^PlcGenerated^PlcGenerated Project^POUs");
            ITcSmTreeItem plcDutsItem = systemManager.LookupTreeItem("TIPC^PlcGenerated^PlcGenerated Project^DUTs");
            ITcSmTreeItem plcGlvsItem = systemManager.LookupTreeItem("TIPC^PlcGenerated^PlcGenerated Project^GVLs");


            /* =================================================================================================
             * Alternative to get Nested Projects
             * ================================================================================================= */
            ITcProjectRoot plcProjectRootItem2 = (ITcProjectRoot)plcGenerated;
            ITcSmTreeItem plcProjectItem2 = plcProjectRootItem2.NestedProject;

            /* =================================================================================================
             * Iterate through subnotes (via _NewEnum)
             * ================================================================================================= */
            foreach (ITcSmTreeItem item in plcProjectItem)
            {
                string name = item.Name;
            }

            /* =================================================================================================
             * Creating datatypes in sub-folder \DUTs\MyDuts\
             * ================================================================================================= */
            ITcSmTreeItem dutFolder = CreatePlcFolder(plcDutsItem, "MyDuts", "", worker);

            string declAlias = @"TYPE AliasType : BOOL; END_TYPE";
            ITcSmTreeItem plcDutAlias = CreateDut(dutFolder, TreeItemType.PlcDutAlias, "AliasType", "INT", declAlias, worker);

            string declEnum = @"TYPE EnumType :
{
    red := 0,
    yellow := 1,
    green := 2
};
END_TYPE";
            ITcSmTreeItem plcDutEnum = CreateDut(dutFolder, TreeItemType.PlcDutEnum, "EnumType", null, declEnum , worker);

            string declStruct = @"TYPE StructType :
STRUCT
    INT i1,
    INT i2,
    INT i3
END_STRUCT
END_TYPE";
            ITcSmTreeItem plcDutStruct = CreateDut(dutFolder, TreeItemType.PlcDutStruct, "StructType", null, declStruct, worker);

            string declUnion = @"TYPE UnionType :
UNION
END_UNION
END_TYPE";            
            ITcSmTreeItem plcDutUnion = CreateDut(dutFolder, TreeItemType.PlcDutUnion, "UnionType", null, declUnion, worker);

            /* =================================================================================================
             * Creating Global Variables in GVL folder
             * ================================================================================================= */
            string glvDecl = @"VAR_GLOBAL
    b1 AT %I*: BOOL;
    b2 AT %I*: BOOL;
    b3 AT %Q*: BOOL;
    b4 AT %Q*: BOOL;
END_VAR";
            ITcSmTreeItem plcGlv = CreateGlv(plcGlvsItem, "MyGlv", glvDecl, worker);

            /* =================================================================================================
             * Creating ParameterList in GVL folder
             * ================================================================================================= */
            // ITcSmTreeItem plcParameterList = CreateParameterList(plcGlvsItem, "MyParameterList", null, worker);

            /* =================================================================================================
             * Creating POUs under folder "MyPous"
             * ================================================================================================= */
            ITcSmTreeItem myPousItem = CreatePlcFolder(plcPousItem, "MyPous", "", worker);

            CreatePOUsIL(myPousItem, worker);
            worker.Progress = 10;
            CreatePOUsST(myPousItem, worker);
            worker.Progress = 20;
            CreatePOUsSFC(myPousItem, worker);
            worker.Progress = 30;
            CreatePOUsCFC(myPousItem, worker);
            worker.Progress = 40;
            CreatePOUsFBD(myPousItem, worker);
            worker.Progress = 50;
            CreatePOUsLD(myPousItem, worker);
            worker.Progress = 60;

            /* =================================================================================================
             * Creating Interfaces under folder "MyPous"
             * ================================================================================================= */
            ITcSmTreeItem itfItem = myPousItem.CreateChild("IMyInterface", TreeItemType.PlcInterface.AsInt32(), "", (object)"ITcUnknown");
            ITcSmTreeItem itfMethodItem = itfItem.CreateChild("ItfMeth", TreeItemType.PlcItfMethod.AsInt32(), "", (object)"BOOL");
            ITcSmTreeItem itfPropItem = itfItem.CreateChild("ItfProp", TreeItemType.PlcItfProperty.AsInt32(), "", (object)"BOOL");
            ITcSmTreeItem itfPropSet = itfPropItem.CreateChild("", TreeItemType.PlcItfPropSet.AsInt32(), "", null);
            ITcSmTreeItem itfPropGet = itfPropItem.CreateChild("", TreeItemType.PlcItfPropGet.AsInt32(), "", null);

            /* =================================================================================================
             * Adjust Visual Studio Solution configuration
             * ================================================================================================= */
            SolutionBuild solutionBuild = solution.SolutionBuild;
            SolutionConfiguration activeConfig = solutionBuild.ActiveConfiguration;
            SolutionContexts contexts = activeConfig.SolutionContexts;

            // ATTENTION: VisualStudio has a bug so that the SolutionContexts Collections cannot be used. This
            // Bug is fixed by Microsoft in VS2012 and later!

            // Iterate over the active Configuration Solution Contexts and activate all Projects within the Solution
            // (Root and Nested projects)
            foreach (SolutionContext context in activeConfig.SolutionContexts) 
            {
                string projectName = context.ProjectName;
                string configName = context.ConfigurationName;
                string platform = context.PlatformName;

                bool shouldBuild = context.ShouldBuild;

                if (shouldBuild == false) // If not set
                {
                    context.ShouldBuild = true; // ShouldBuild can be set.
                }
            }

            /* =================================================================================================
             * Check all objects of the Project
             * ================================================================================================= */
            ITcPlcIECProject2 nestedProject = (ITcPlcIECProject2)plcProjectItem;
            bool projectOk = nestedProject.CheckAllObjects();

            /* =================================================================================================
             * Compiling project using Automation Interface methods
             * ================================================================================================= */
            ITcPlcProject iecProjectRoot = (ITcPlcProject)plcProjectRootItem;
            worker.ProgressStatus = "Compiling project (1) ...";
            
            iecProjectRoot.CompileProject();

            ErrorItems errors = dte.ToolWindows.ErrorList.ErrorItems;
            worker.Progress = 75;
            iecProjectRoot.BootProjectAutostart = true;

            
            /* =================================================================================================
             * Compiling project using Visual Studio DTE methods 
             * ================================================================================================= */
            errors = null;
            worker.ProgressStatus = "Compiling project (3) ...";
            dte.Solution.SolutionBuild.Build(true);
            waitForBuildAndCheckErrors(worker, out errors);

            /* =================================================================================================
             * Generating Boot Project
             * ================================================================================================= */
            worker.ProgressStatus = "Generating boot project ...";
            iecProjectRoot.GenerateBootProject(true);

            /* =================================================================================================
             * Optional: Saving the PLC Project as a PLC library and PLC Compiled Library. For more information
             * please see the sample "ManagePlcLibraries.cs"
             * ================================================================================================= */
           // ITcPlcIECProject nestedProject = (ITcPlcIECProject)plcProjectItem;
            if (!Directory.Exists(this.ScriptTempFolder))
                Directory.CreateDirectory(this.ScriptTempFolder);

            string libPath = Path.Combine(this.ScriptTempFolder, this.ScriptName + ".library");
            
            string xml = @" <TreeItem>
                                <IECProjectDef>
                                    <ProjectInfo>
                                <Company>Beckhoff Automation GmbH</Company>
                                <Title>TestLibrary</Title>
                                <Version>1.2.3.4</Version>
                                <DefaultNamespace>Beckhoff.TestLibrary</DefaultNamespace>
                                <Author>Beckhoff Employee</Author>
                                <Description>Test Library Description</Description>
                                <Released>true</Released>
                                    </ProjectInfo>
                                </IECProjectDef>
                            </TreeItem>";
            plcProjectItem.ConsumeXml(xml);
            nestedProject.SaveAsLibrary(libPath, false); // Save as Library but don't install
            xml = @"<TreeItem>
                              <IECProjectDef>
                                <ProjectInfo>
                                <Title>TestLibrary Compiled</Title>
                                </ProjectInfo>
                              </IECProjectDef>
                            </TreeItem>";
            plcProjectItem.ConsumeXml(xml);
            string compiledLibPath = Path.Combine(this.ScriptTempFolder, this.ScriptName + ".compiled-library");
            nestedProject.SaveAsLibrary(compiledLibPath, false);


            /* =================================================================================================
             * Prepare XML for PLC Login
             * ================================================================================================= */
            string xmlLogin = @"<TreeItem>
                                    <IECProjectDef>
                                        <OnlineSettings>
                                                <Commands>
                                                        <LoginCmd>true</LoginCmd>
                                                        <LogoutCmd>false</LogoutCmd>
                                                        <StartCmd>false</StartCmd>
                                                        <StopCmd>false</StopCmd>
                                                        <ResetColdCmd>false</ResetColdCmd>
                                                        <ResetOriginCmd>false</ResetOriginCmd>
                                                </Commands>
                                        </OnlineSettings>
                                    </IECProjectDef>
                                </TreeItem>";

            /* =================================================================================================
             * Prepare XML for PLC Start
             * ================================================================================================= */
            string xmlStart = @"<TreeItem>
                                    <IECProjectDef>
                                        <OnlineSettings>
                                                <Commands>
                                                        <LoginCmd>false</LoginCmd>
                                                        <LogoutCmd>false</LogoutCmd>
                                                        <StartCmd>true</StartCmd>
                                                        <StopCmd>false</StopCmd>
                                                        <ResetColdCmd>false</ResetColdCmd>
                                                        <ResetOriginCmd>false</ResetOriginCmd>
                                                </Commands>
                                        </OnlineSettings>
                                    </IECProjectDef>
                                </TreeItem>";

            /* =================================================================================================
             * Prepare XML for PLC Stop
             * ================================================================================================= */
            string xmlStop = @"<TreeItem>
                                    <IECProjectDef>
                                        <OnlineSettings>
                                                <Commands>
                                                        <LoginCmd>false</LoginCmd>
                                                        <LogoutCmd>false</LogoutCmd>
                                                        <StartCmd>false</StartCmd>
                                                        <StopCmd>true</StopCmd>
                                                        <ResetColdCmd>false</ResetColdCmd>
                                                        <ResetOriginCmd>false</ResetOriginCmd>
                                                </Commands>
                                        </OnlineSettings>
                                    </IECProjectDef>
                                </TreeItem>";

            /* =================================================================================================
             * Prepare XML for PLC Logout
             * ================================================================================================= */
            string xmlLogout = @"<TreeItem>
                                    <IECProjectDef>
                                        <OnlineSettings>
                                                <Commands>
                                                        <LoginCmd>false</LoginCmd>
                                                        <LogoutCmd>true</LogoutCmd>
                                                        <StartCmd>false</StartCmd>
                                                        <StopCmd>false</StopCmd>
                                                        <ResetColdCmd>false</ResetColdCmd>
                                                        <ResetOriginCmd>false</ResetOriginCmd>
                                                </Commands>
                                        </OnlineSettings>
                                    </IECProjectDef>
                                </TreeItem>";

            /* =================================================================================================
             * Activate configuration and restart TwinCAT
             * ================================================================================================= */
            worker.ProgressStatus = "Activating configuration ...";
            systemManager.ActivateConfiguration();

            worker.ProgressStatus = "Restarting TwinCAT ...";
            systemManager.StartRestartTwinCAT();

            /* =================================================================================================
             * Execute PLC Login, Start, Logout, ...
             * ================================================================================================= */
            System.Threading.Thread.Sleep(5000);
            worker.ProgressStatus = "Logging in to PLC runtime ...";
            System.Threading.Thread.Sleep(5000);
            plcProjectItem.ConsumeXml(xmlLogin);
            worker.ProgressStatus = "Stopping PLC runtime ...";
            System.Threading.Thread.Sleep(5000);
            plcProjectItem.ConsumeXml(xmlStop);
            worker.ProgressStatus = "Starting PLC runtime ...";
            System.Threading.Thread.Sleep(5000);
            plcProjectItem.ConsumeXml(xmlStart);
            worker.ProgressStatus = "Logging out of PLC runtime ...";
            System.Threading.Thread.Sleep(5000);
            plcProjectItem.ConsumeXml(xmlLogout);
            
            worker.Progress = 100;
        }


        /// <summary>
        /// Waits for build command to finish and checks for any errors in the ErrorList window
        /// </summary>
        /// <param name="worker">The worker.</param>
        /// <param name="errorItems">Returns found errors in ErrorItems list.</param>
        private bool waitForBuildAndCheckErrors(IWorker worker, out ErrorItems errorItems)
        {
            bool buildSucceeded = false;
            vsBuildState state = dte.Solution.SolutionBuild.BuildState;

            /* =================================================================================================
             * Wait for build process to finish
             * ================================================================================================= */
            while (state == vsBuildState.vsBuildStateInProgress) 
            {
                System.Threading.Thread.Sleep(500);
                state = dte.Solution.SolutionBuild.BuildState;
            } 
            buildSucceeded = (dte.Solution.SolutionBuild.LastBuildInfo == 0 && state == vsBuildState.vsBuildStateDone);

            /* =================================================================================================
             * Check for any errors. Please note that the ErrorList is not significant for a successful build!
             * Because the PLC Project can contain errors within not used types.
             * Relevant is only the "LastBuildInfo" object that contains the numer of failed project compilations!
             * ================================================================================================= */
            ErrorList errorList = dte.ToolWindows.ErrorList;
            errorItems = errorList.ErrorItems;

            int overallMessages = errorItems.Count;

            int errors = 0;
            int warnings = 0;
            int messages = 0;

            for (int i = 1; i <= overallMessages; i++) // Please note: List is starting from 1 !
            {
                ErrorItem item = errorItems.Item(i);

                if (item.ErrorLevel == vsBuildErrorLevel.vsBuildErrorLevelHigh)
                {
                    errors++;
                    worker.ProgressStatus = "Compiler error: " + item.Description;
                }
                else if (item.ErrorLevel == vsBuildErrorLevel.vsBuildErrorLevelMedium)
                {
                    warnings++;
                    worker.ProgressStatus = "Compiler warning: " + item.Description;
                }
                else if (item.ErrorLevel == vsBuildErrorLevel.vsBuildErrorLevelLow)
                {
                    messages++;
                    worker.ProgressStatus = "Compiler message: " + item.Description;
                }

            }
            return buildSucceeded;
        }


        /// <summary>
        /// Creates a PLC folder
        /// </summary>
        /// <param name="parent">The parent item.</param>
        /// <param name="folderName">Name of the folder to be created.</param>
        /// <param name="before">Before item.</param>
        /// <param name="worker">The worker.</param>
        private ITcSmTreeItem CreatePlcFolder(ITcSmTreeItem parent, string folderName, string before, IWorker worker)
        {
            /* =================================================================================================
             * Creating the PLC folder using CreateChild()
             * ================================================================================================= */
            worker.ProgressStatus = string.Format("Creating Folder '{0}' ...", folderName);
            ITcSmTreeItem item = parent.CreateChild(folderName,TreeItemType.PlcFolder.AsInt32(),before,null);
            return item;
        }


        /// <summary>
        /// Creates a DUT
        /// </summary>
        /// <param name="parent">The parent item.</param>
        /// <param name="treeItemType">Tree item type.</param>
        /// <param name="typeName">Name of the DUT.</param>
        /// <param name="baseType">Base type.</param>
        /// <param name="declarationCode">Declaration code of the DUT.</param>
        /// <param name="worker">The worker.</param>
        private ITcSmTreeItem CreateDut(ITcSmTreeItem parent, TreeItemType treeItemType, string typeName, string baseType, string declarationCode, IWorker worker)
        {
            /* =================================================================================================
             * Creating the DUT using CreateChild()
             * ================================================================================================= */
            worker.ProgressStatus = string.Format("Creating Type '{0}' ...", typeName);
            string declaration = GetDUTEmptyDeclaration(treeItemType, typeName, baseType);
            ITcSmTreeItem item = parent.CreateChild(typeName, treeItemType.AsInt32(), "", declaration);
            return item;
        }


        /// <summary>
        /// Creates Global Variable List
        /// </summary>
        /// <param name="parent">The parent item.</param>
        /// <param name="name">Name of the item.</param>
        /// <param name="declarationCode">Fill the GVL with declaration code.</param>
        /// <param name="worker">The worker.</param>
        private ITcSmTreeItem CreateGlv(ITcSmTreeItem parent, string name, string declarationCode, IWorker worker)
        {
            /* =================================================================================================
             * Creating the GVL using CreateChild()
             * ================================================================================================= */
            worker.ProgressStatus = string.Format("Creating Global Variable worksheet '{0}' ...", name);
            ITcSmTreeItem item = parent.CreateChild(name, TreeItemType.PlcGvl.AsInt32(), "", declarationCode);

            /* =================================================================================================
             * Accessing the declaration code of the newly created GVL
             * ================================================================================================= */
            ITcPlcDeclaration declaration = (ITcPlcDeclaration)item;
            string oldDeclaration1 = declaration.DeclarationText;
            declaration.DeclarationText = declarationCode;
            string oldDeclaration2 = declaration.DeclarationText;
            string newDecl = declaration.DeclarationText;
            return item;
        }

        /// <summary>
        /// Creates Global Variable Parameter List
        /// </summary>
        /// <param name="parent">The parent item.</param>
        /// <param name="name">Name of the Parameter List.</param>
        /// <param name="declarationCode">Fill the Parameter List with declaration code.</param>
        /// <param name="worker">The worker.</param>
        private ITcSmTreeItem CreateParameterList(ITcSmTreeItem parent, string name, string declarationCode, IWorker worker)
        {
            /* =================================================================================================
             * Creating the ParameterList using CreateChild()
             * ================================================================================================= */
            worker.ProgressStatus = string.Format("Creating ParameterList worksheet '{0}' ...", name);
            ITcSmTreeItem item = parent.CreateChild(name, TreeItemType.PlcGvlParameters.AsInt32(), "", declarationCode);

            //* =================================================================================================
            // * Accessing the declaration code of the newly created ParaemeterList
            // * ================================================================================================= */
            //ITcPlcDeclaration declaration = (ITcPlcDeclaration)item;
            //string oldDeclaration1 = declaration.DeclarationText;
            //declaration.DeclarationText = declarationCode;
            //string oldDeclaration2 = declaration.DeclarationText;
            //string newDecl = declaration.DeclarationText;
            return item;
        }


        /// <summary>
        /// Creates SFC POUs
        /// </summary>
        /// <param name="plcPousItem">The PLC pous item.</param>
        /// <param name="worker">The worker.</param>
        private void CreatePOUsSFC(ITcSmTreeItem plcPousItem, IWorker worker)
        {
            /* =================================================================================================
             * Creating SFC POUs using CreateChild()
             * ================================================================================================= */
            IECLanguageType language = IECLanguageType.SFC;
            worker.ProgressStatus = "Generating SFC POUs ...";
            ITcSmTreeItem pouProgramItem = AddPOUProgram("SFCProgram", plcPousItem, language, worker);
            ITcSmTreeItem fbPouItem = AddPOUFB("SFCFunctionBlock", plcPousItem, language, "ADSREAD",new string[] {"ITcPersist","ITcTask"},worker); // Nonsense to use this deriavation here just as sample
            ITcSmTreeItem functionPouItem = AddPouFunction("SFCFunction", "DINT", plcPousItem, language, worker);
        }

        /// <summary>
        /// Creates IL POUs
        /// </summary>
        /// <param name="parent">The parent.</param>
        /// <param name="worker">The worker.</param>
        private void CreatePOUsIL(ITcSmTreeItem parent, IWorker worker)
        {
            /* =================================================================================================
             * Creating IL POUs using CreateChild()
             * ================================================================================================= */
            IECLanguageType language = IECLanguageType.IL;
            worker.ProgressStatus = "Generating IL POUs ...";
            ITcSmTreeItem functionBlock = AddPOUFB("ILFunctionBlock", parent, language, null, null, worker);
            ITcSmTreeItem program = AddPOUProgram("ILProgram", parent, language, worker);
            ITcSmTreeItem function = AddPouFunction("ILFunction", "DINT", parent, language, worker);
        }

        /// <summary>
        /// Creates ST POUs
        /// </summary>
        /// <param name="parent">The parent.</param>
        /// <param name="worker">The worker.</param>
        private void CreatePOUsST(ITcSmTreeItem parent, IWorker worker)
        {
            /* =================================================================================================
             * Creating ST POUs using CreateChild()
             * ================================================================================================= */
            IECLanguageType language = IECLanguageType.ST;
            worker.ProgressStatus = "Generating ST POUs ...";
            ITcSmTreeItem functionBlock = AddPOUFB("STFunctionBlock", parent, language, "ADSRDSTATE", new string[] {"ITcADI"}, worker);
            ITcSmTreeItem program = AddPOUProgram("STProgram", parent, language, worker);
            ITcSmTreeItem function = AddPouFunction("STFunction", "DINT", parent, language, worker);
        }


        /// <summary>
        /// Creates CFC Pous
        /// </summary>
        /// <param name="plcPousItem">The PLC pous item.</param>
        /// <param name="worker">The worker.</param>
        private void CreatePOUsCFC(ITcSmTreeItem plcPousItem, IWorker worker)
        {
            /* =================================================================================================
             * Creating CFC POUs using CreateChild()
             * ================================================================================================= */
            IECLanguageType language = IECLanguageType.CFC;
            worker.ProgressStatus = "Generating CFC POUs ...";
            ITcSmTreeItem pouProgramItem = AddPOUProgram("CFCProgram", plcPousItem, language, worker);
            ITcSmTreeItem fbPouItem = AddPOUFB("CFCFunctionBlock", plcPousItem, language, null,null, worker);
            ITcSmTreeItem functionPouItem = AddPouFunction("CFCFunction", "DINT", plcPousItem, language, worker);
        }


        /// <summary>
        /// Creates FBD POUs
        /// </summary>
        /// <param name="plcPousItem">The PLC pous item.</param>
        /// <param name="worker">The worker.</param>
        private void CreatePOUsFBD(ITcSmTreeItem plcPousItem, IWorker worker)
        {
            /* =================================================================================================
             * Creating SFC FBD using CreateChild()
             * ================================================================================================= */
            IECLanguageType language = IECLanguageType.FBD;
            worker.ProgressStatus = "Generating FBD POUs ...";
            ITcSmTreeItem pouProgramItem = AddPOUProgram("FBDProgram", plcPousItem, language, worker);
            ITcSmTreeItem fbPouItem = AddPOUFB("FBDFunctionBlock", plcPousItem, language, null,null, worker);
            ITcSmTreeItem functionPouItem = AddPouFunction("FBDFunction", "DINT", plcPousItem, language, worker);
        }


        /// <summary>
        /// Creates LD POUs
        /// </summary>
        /// <param name="plcPousItem">The PLC pous item.</param>
        /// <param name="worker">The worker.</param>
        private void CreatePOUsLD(ITcSmTreeItem plcPousItem, IWorker worker)
        {
            /* =================================================================================================
             * Creating LD POUs using CreateChild()
             * ================================================================================================= */
            IECLanguageType language = IECLanguageType.LD;
            worker.ProgressStatus = "Generating LD POUs ...";
            ITcSmTreeItem pouProgramItem = AddPOUProgram("LDProgram", plcPousItem, language, worker);
            ITcSmTreeItem fbPouItem = AddPOUFB("LDFunctionBlock", plcPousItem, language, null,null, worker);
            ITcSmTreeItem functionPouItem = AddPouFunction("LDFunction", "DINT", plcPousItem, language, worker);
        }


        /// <summary>
        /// Adds a function POU to the parent item
        /// </summary>
        /// <param name="pouName">Name of the pou.</param>
        /// <param name="returnType">Type of the return.</param>
        /// <param name="parent">The parent.</param>
        /// <param name="language">The language.</param>
        /// <param name="worker">The worker.</param>
        /// <returns></returns>
        private ITcSmTreeItem AddPouFunction(string pouName, string returnType, ITcSmTreeItem parent, IECLanguageType language, IWorker worker)
        {
            /* =================================================================================================
             * Prepare vInfo parameter with IEC language and return data type for the new Function
             * ================================================================================================= */
            string[] data = new string[2];  // two vInfo parameters are needed
            data[0] = language.AsString();  // IEC language
            data[1] = returnType;           // Return data type

            /* =================================================================================================
             * Create Function using CreateChild()
             * ================================================================================================= */
            ITcSmTreeItem function = parent.CreateChild(pouName, TreeItemType.PlcPouFunction.AsInt32(), "", data);
            worker.ProgressStatus = string.Format("POU Function '{0}' added ...",pouName);

            /* =================================================================================================
             * Retrieve XML representation of new Function
             * ================================================================================================= */
            ITcPlcPou pou = (ITcPlcPou)function;
            string documentXml = pou.DocumentXml;

            /* =================================================================================================
             * Access declaration (text-based) and implementation area (text- and XML-based)
             * ================================================================================================= */
            ITcPlcDeclaration decl = (ITcPlcDeclaration)function;
            ITcPlcImplementation impl = (ITcPlcImplementation)function;
            string declText = decl.DeclarationText;
            string implText = impl.ImplementationText;
            string implXml = impl.ImplementationXml;

            return function;
        }


        /// <summary>
        /// Adds a function block POU to the parent
        /// </summary>
        /// <param name="pouName">Name of the pou.</param>
        /// <param name="parent">The parent.</param>
        /// <param name="language">The language.</param>
        /// <param name="extensionType">Type of the extension.</param>
        /// <param name="implementationInterfaces">The implementation interfaces.</param>
        /// <param name="worker">The worker.</param>
        /// <returns></returns>
        private ITcSmTreeItem AddPOUFB(string pouName, ITcSmTreeItem parent, IECLanguageType language, string extensionType, string[] implementationInterfaces, IWorker worker)
        {
            /* =================================================================================================
             * Check if the new POU should use Extends/Implements modifier
             * ================================================================================================= */
            bool extends = !string.IsNullOrEmpty(extensionType);
            bool implements = (implementationInterfaces != null && implementationInterfaces.Length > 0 && !string.IsNullOrEmpty(implementationInterfaces[0]));
            
            /* =================================================================================================
             * Prepare vInfo parameter, depending on used modifier
             * ================================================================================================= */
            List<string> parameters = new List<string>();
            parameters.Add(language.AsString());
            if (extends)
            {
                parameters.Add("Extends");
                parameters.Add(extensionType);
            }
            if (implements)
            {
                parameters.Add("Implements");
                parameters.Add(string.Join(",", implementationInterfaces));
            }
            else if (implements && extends)
            {
                parameters.Add("Extends");
                parameters.Add(extensionType);
                parameters.Add("Implements");
                parameters.Add(string.Join(",", implementationInterfaces));
            }

            /* =================================================================================================
             * Create Function Block using CreateChild()
             * ================================================================================================= */
            ITcSmTreeItem functionBlock = parent.CreateChild(pouName, TreeItemType.PlcPouFunctionBlock.AsInt32(), "",parameters.ToArray());
            worker.ProgressStatus = string.Format("POU FunctionBlock '{0}' added ...",pouName);

            /* =================================================================================================
             * Adding Action to Function Block
             * ================================================================================================= */
            ITcSmTreeItem action = AddAction(functionBlock, language);

            /* =================================================================================================
             * Adding Property to Function Block
             * ================================================================================================= */
            ITcSmTreeItem prop = AddProperty(functionBlock, language);

            /* =================================================================================================
             * Adding Transition to Function Block
             * ================================================================================================= */
            ITcSmTreeItem transition = AddTransition(functionBlock, language);

            /* =================================================================================================
             * Adding Method to Function Block
             * ================================================================================================= */
            ITcSmTreeItem method1 = AddMethod(functionBlock, "Method1", language,"BOOL",PLCACCESS.PLCACCESS_PUBLIC);
            ITcSmTreeItem method2 = AddMethod(functionBlock, "Method2", language);

            /* =================================================================================================
             * Access XML representation of Function Block
             * ================================================================================================= */
            ITcPlcPou functionBlockPOU = (ITcPlcPou)functionBlock;
            string documentXml = functionBlockPOU.DocumentXml;

            /* =================================================================================================
             * Access declaration area (text-based) and implementation area (text- and XML-based)
             * ================================================================================================= */
            ITcPlcDeclaration decl = (ITcPlcDeclaration)functionBlock;
            ITcPlcImplementation impl = (ITcPlcImplementation)functionBlock;
            string declText = decl.DeclarationText;
            string implText = impl.ImplementationText;
            string implXml = impl.ImplementationXml;

            return functionBlock;
        }


        /// <summary>
        /// Adds a Program pou to the Parent
        /// </summary>
        /// <param name="pouName">Name of the pou.</param>
        /// <param name="parent">The parent.</param>
        /// <param name="language">The language.</param>
        /// <param name="worker">The worker.</param>
        /// <returns></returns>
        private ITcSmTreeItem AddPOUProgram(string pouName, ITcSmTreeItem parent, IECLanguageType language, IWorker worker)
        {
            /* =================================================================================================
             * Create Program POU using CreateChild()
             * ================================================================================================= */
            ITcSmTreeItem programPou = parent.CreateChild(pouName, TreeItemType.PlcPouFunctionBlock.AsInt32(), "", language.AsString());
            worker.ProgressStatus = string.Format("POU Program '{0}' added ...",pouName);

            /* =================================================================================================
             * Accessing declaration and implementation area 
             * ================================================================================================= */
            ITcPlcPou pou = programPou as ITcPlcPou;
            ITcPlcDeclaration pouDecl = programPou as ITcPlcDeclaration;
            ITcPlcImplementation pouImpl = programPou as ITcPlcImplementation;
            ITcXmlDocument pouDoc = programPou as ITcXmlDocument;
            string decl1 = pouDecl.DeclarationText;
            string impl1 = pouImpl.ImplementationText;

            pouDecl.DeclarationText += (Environment.NewLine + "(* Added by Script *)");

            string oldDocumentXml = pouDoc.DocumentXml; // Accessing POUs Code Document

            /* =================================================================================================
             * Modifying implementation area, based on IEC language.
             * Please note: Direct access to implementation text is only provided for ST !
             * ================================================================================================= */
            if (language == IECLanguageType.ST)
            {
                pouImpl.ImplementationText = "i := i + 1; (* This code is added by script *)";
                pouImpl.ImplementationText += (Environment.NewLine + "(* Added by Script *)");

            }
            else if (language == IECLanguageType.SFC)
            {
                loadTemplate(pou,language);
            }

            else if (language == IECLanguageType.FBD)
            {
                loadTemplate(pou, language);
            }
            else if (language == IECLanguageType.LD)
            {
                loadTemplate(pou, language);
            }
            else if (language == IECLanguageType.CFC)
            {
                loadTemplate(pou, language);
            }
            return programPou;
        }


        /// <summary>
        /// Loads template XML file for a POU
        /// </summary>
        /// <param name="pou">POU object.</param>
        /// <param name="language">The IEC language.</param>
        /// <returns></returns>
        private void loadTemplate(ITcPlcPou pou, IECLanguageType language)
        {
            string templateName = null;
            switch (language)
            {
                case IECLanguageType.CFC:
                    templateName = "PouProgramCFC.xml";
                    break;
                case IECLanguageType.FBD:
                    templateName = "PouProgramFBD.xml";
                    break;
                case IECLanguageType.SFC:
                    templateName = "PouProgramSFC.xml";
                    break;
                case IECLanguageType.ST:
                    templateName = "PouProgramST.xml";
                    break;
                case IECLanguageType.LD:
                    templateName = "PouProgramLD.xml";
                    break;
                case IECLanguageType.IL:
                case IECLanguageType.None:
                    throw new NotSupportedException();
            }
            
            string template = Path.Combine(base.ConfigurationTemplatesFolder, templateName);

            // Loading into XmlDocument to check if data is well formed
            XmlDocument doc = new XmlDocument();
            doc.Load(template);
             
            ITcXmlDocument pouDoc = (ITcXmlDocument)pou;
            string oldDoc = pou.DocumentXml;
            pouDoc.DocumentXml = doc.OuterXml; // Write the new Document
        }


        /// <summary>
        /// Adds a Method to a POU
        /// </summary>
        /// <param name="pou">POU object.</param>
        /// <param name="name">Name of the Method</param>
        /// <param name="language">The IEC language.</param>
        /// <param name="returnType">Return type</param>
        /// <param name="accessor">The accessor.</param>
        /// <returns></returns>
        private ITcSmTreeItem AddMethod(ITcSmTreeItem pou, string name, IECLanguageType language, string returnType, PLCACCESS accessor)
        {
            /* =================================================================================================
             * Prepare vInfo parameter
             * ================================================================================================= */
            dynamic vInfo = new string[4];
            vInfo[0] = language.AsString();
            vInfo[1] = returnType;
            vInfo[2] = PlcAccessConverter.ToString(accessor);
            vInfo[3] = @"<ST><![CDATA[(* ST Method *)]]></ST>";

            /* =================================================================================================
             * Create method using CreateChild()
             * ================================================================================================= */
            ITcSmTreeItem method = pou.CreateChild(name, TreeItemType.PlcMethod.AsInt32(), "", (object)vInfo);

            /* =================================================================================================
             * Accessing declaration and implementation area
             * ================================================================================================= */
            ITcPlcDeclaration decl = (ITcPlcDeclaration)method;
            string declText = decl.DeclarationText;
            ITcPlcImplementation impl = (ITcPlcImplementation)method;
            string implText = impl.ImplementationText;
            string implXml = impl.ImplementationXml;

            return method;
        }

        /// <summary>
        /// Adds a Method to a POU
        /// </summary>
        /// <param name="pou">POU object.</param>
        /// <param name="name">Name of the Method</param>
        /// <param name="language">The IEC language.</param>
        /// <param name="returnType">Return type.</param>
        /// <returns></returns>
        private ITcSmTreeItem AddMethod(ITcSmTreeItem pou, string name, IECLanguageType language, string returnType = "")
        {
            /* =================================================================================================
             * Prepare vInfo parameter
             * ================================================================================================= */
            dynamic vInfo = new string[4];
            vInfo[0] = language.AsString();
            vInfo[1] = returnType;
            vInfo[2] = string.Empty;
            vInfo[3] = @"<ST><![CDATA[(* ST Method *)]]></ST>";

            /* =================================================================================================
             * Create method using CreateChild()
             * ================================================================================================= */
            ITcSmTreeItem method = pou.CreateChild(name, TreeItemType.PlcMethod.AsInt32(), "", (object)vInfo);

            /* =================================================================================================
             * Accessing declaration and implementation area
             * ================================================================================================= */
            ITcPlcDeclaration decl = (ITcPlcDeclaration)method;
            string declText = decl.DeclarationText;
            ITcPlcImplementation impl = (ITcPlcImplementation)method;
            string implText = impl.ImplementationText;
            string implXml = impl.ImplementationXml;

            return method;
        }

        /// <summary>
        /// Adds a Transition to a POU
        /// </summary>
        /// <param name="pou">POU object.</param>
        /// <param name="language">The IEC language.</param>
        /// <returns></returns>
        private ITcSmTreeItem AddTransition(ITcSmTreeItem pou, IECLanguageType language)
        {
            /* =================================================================================================
             * Preparing vInfo parameter
             * ================================================================================================= */
            dynamic vInfo = new string[2];
            vInfo[0] = language.AsString();
            if (language == IECLanguageType.ST)
            {
                vInfo[1] = @"<ST><![CDATA[(* ST Transition *)]]></ST>";
            }

            /* =================================================================================================
             * Creating Transition using CreateChild()
             * ================================================================================================= */
            ITcSmTreeItem transition =  pou.CreateChild("STTransition", TreeItemType.PlcTransition.AsInt32(), "", (object)vInfo);

            /* =================================================================================================
             * Accessing declaration and implementation area
             * ================================================================================================= */
            ITcPlcImplementation impl = (ITcPlcImplementation)transition;
            string implText = impl.ImplementationText;
            string implXml = impl.ImplementationXml;
            if (language == IECLanguageType.ST)
            {
                impl.ImplementationText = "//Test\n//Test";

                string xml1 = impl.ImplementationXml;
                string text1 = impl.ImplementationText;

                impl.ImplementationText = "//Test\\n//Test";
                string xml2 = impl.ImplementationXml;
                string text2 = impl.ImplementationText;
            }
            
            return transition;
        }


        /// <summary>
        /// Adds a Set to a Property
        /// </summary>
        /// <param name="prop">Property object.</param>
        /// <param name="language">The IEC language.</param>
        /// <returns></returns>
        private ITcSmTreeItem AddPropertySet(ITcSmTreeItem prop, IECLanguageType language)
        {
            /* =================================================================================================
             * Preparing vInfo parameter
             * ================================================================================================= */
            dynamic vInfo = new string[3];
            vInfo[0] = language.AsString();
            vInfo[1] = "PUBLIC";
            if (language == IECLanguageType.ST)
            {
                vInfo[2] = @"<ST><![CDATA[(* ST PropSet *)]]></ST>";
            }

            /* =================================================================================================
             * Creating Property using CreateChild()
             * ================================================================================================= */
            ITcSmTreeItem propput = prop.CreateChild("", TreeItemType.PlcPropertySet.AsInt32(), "", (object)vInfo);

            /* =================================================================================================
             * Accessing declaration and implementation area
             * ================================================================================================= */
            ITcPlcDeclaration decl = (ITcPlcDeclaration)propput;
            ITcPlcImplementation impl = (ITcPlcImplementation)propput;
            string declText = decl.DeclarationText;
            string implText = impl.ImplementationText;
            string implXml = impl.ImplementationXml;

            return propput;
        }


        /// <summary>
        /// Adds a Get to a Property
        /// </summary>
        /// <param name="prop">Property object.</param>
        /// <param name="language">The IEC language.</param>
        /// <returns></returns>
        private ITcSmTreeItem AddPropertyGet(ITcSmTreeItem prop, IECLanguageType language)
        {
            /* =================================================================================================
             * Preparing vInfo parameter
             * ================================================================================================= */
            dynamic vInfo = new string[3];
            vInfo[0] = language.AsString();
            vInfo[1] = "PUBLIC";
            if (language == IECLanguageType.ST)
            {
                vInfo[2] = @"<ST><![CDATA[(* ST PropGet *)]]></ST>";
            }

            /* =================================================================================================
             * Creating Property using CreateChild()
             * ================================================================================================= */
            ITcSmTreeItem propget = prop.CreateChild("", TreeItemType.PlcPropertyGet.AsInt32(), "", (object)vInfo);

            /* =================================================================================================
             * Accessing declaration and implementation area
             * ================================================================================================= */
            ITcPlcDeclaration decl = (ITcPlcDeclaration)propget;
            ITcPlcImplementation impl = (ITcPlcImplementation)propget;
            string declText = decl.DeclarationText;
            string implText = impl.ImplementationText;
            string implXml = impl.ImplementationXml;
            
            return propget;
        }


        /// <summary>
        /// Adds a Property to a POU
        /// </summary>
        /// <param name="pou">POU object.</param>
        /// <param name="language">The IEC language.</param>
        /// <returns></returns>
        private ITcSmTreeItem AddProperty(ITcSmTreeItem pou, IECLanguageType language)
        {
            /* =================================================================================================
             * Preparing vInfo parameter
             * ================================================================================================= */
            dynamic vInfo = new string[3];
            vInfo[0] = language.AsString();
            vInfo[1] = "BOOL"; // Property Return Type
            vInfo[2] = "PUBLIC"; // Property Accessor

            /* =================================================================================================
             * Creating Property using CreateChild()
             * ================================================================================================= */
            ITcSmTreeItem prop = pou.CreateChild("STProperty", TreeItemType.PlcProperty.AsInt32(), "", (object)vInfo);

            /* =================================================================================================
             * Adding Getter and Setter
             * ================================================================================================= */
            ITcSmTreeItem propGet = AddPropertySet(prop, language);
            ITcSmTreeItem propSet = AddPropertyGet(prop, language);

            /* =================================================================================================
             * Accessing declaration area (Properties do not have an implementation area)
             * ================================================================================================= */
            ITcPlcDeclaration propDeclaration = (ITcPlcDeclaration)prop;
            string declText = propDeclaration.DeclarationText;

            return prop;
        }


        /// <summary>
        /// Adds an Action to a POU
        /// </summary>
        /// <param name="pou">POU object.</param>
        /// <param name="language">The IEC language.</param>
        /// <returns></returns>
        private ITcSmTreeItem AddAction(ITcSmTreeItem pou, IECLanguageType language)
        {
            /* =================================================================================================
             * Preparing vInfo parameter
             * ================================================================================================= */
            dynamic vInfo = new string[2];
            vInfo[0] = language.AsString();
            if (language == IECLanguageType.ST)
            {
                vInfo[1] = @"<ST><![CDATA[(* ST Action *)]]></ST>";
            }

            /* =================================================================================================
             * Creating Property using CreateChild()
             * ================================================================================================= */            
            ITcSmTreeItem action = pou.CreateChild("STAction", TreeItemType.PlcAction.AsInt32(), "", vInfo);

            /* =================================================================================================
             * Accessing implementation area (Actions do not have a declaration area)
             * ================================================================================================= */
            ITcPlcImplementation impl = (ITcPlcImplementation)action;
            string implText = impl.ImplementationText;
            string implXml = impl.ImplementationXml;

            return action;
        }


        /// <summary>
        /// Writes all POUs into worker window
        /// </summary>
        /// <param name="root">Root object.</param>
        /// <param name="worker">The worker.</param>
        /// <returns></returns>
        private void DumpPous(ITcSmTreeItem root, IWorker worker)
        {
            foreach (ITcSmTreeItem item in root)
            {
                ITcPlcPou pou = item as ITcPlcPou;

                if (pou != null)
                {
                    string pouName = item.Name;
                    worker.ProgressStatus = string.Format("POU '{0}' found!", pouName);
                }
                else
                {
                    DumpPous(item, worker);
                }
            }

        }


        /// <summary>
        /// Gets the Script description
        /// </summary>
        /// <value>The description.</value>
        public override string Description
        {
            get { return "Demonstrates the creation of PLC projects, POUs etc., Setting of Solution Configurations."; }
        }


        /// <summary>
        /// Gets the detailed description of the <see cref="Script"/> that is shown in the Method Tips.
        /// </summary>
        /// <value>The detailed description.</value>
        public override string DetailedDescription
        {
            get
            {
                string test = @"Creation of a new PLC Project from Scratch. Adding Libraries, POUs of the different types and languages and referencing tasks.
Compilation of the project and Generating a Boot project.  Access and adjustment of Solution Configurations
";
                return test;
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
                return "Create PlcProject, POU Management, Interface / Code access, PlcImport/Export, Build Configuration";
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
                return new Version(3, 1);
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
                return "4012";
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
                return "PLC";
            }
        }

        private string GetDUTEmptyDeclaration(TreeItemType dutType, string childName, string baseClass)
        {
            if (childName == null) throw new ArgumentNullException("childName");

            switch (dutType)
            {
                case TreeItemType.PlcDutAlias:
                    if (baseClass == null || baseClass == string.Empty) throw new ArgumentException("Base class not specified!", "baseClass");
                    return string.Format("TYPE {0}: {1}; END_TYPE", childName, baseClass);
                
                case TreeItemType.PlcDutEnum:
                    return string.Format("TYPE {0} :\n(\n\tenum_member := 0\n);\nEND_TYPE\n", childName);
                
                case TreeItemType.PlcDutStruct:
                    if (baseClass == null || baseClass == string.Empty)
                        return string.Format("TYPE {0}:\n\tSTRUCT\n\tEND_STRUCT\nEND_TYPE", childName);
                    else
                        return string.Format("TYPE {0} EXTENDS {1} :\n\tSTRUCT\n\tEND_STRUCT\nEND_TYPE", childName);
                
                case TreeItemType.PlcDutUnion:
                    return string.Format("TYPE {0}:\n\tUNION\n\tEND_UNION\nEND_TYPE", childName);
                
                default:
                    throw new ArgumentException("dutType");
            }
        }
    }
}
