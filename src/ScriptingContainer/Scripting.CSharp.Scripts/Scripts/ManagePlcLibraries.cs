using System;
using System.IO;
using EnvDTE;
using EnvDTE100;
using EnvDTE80;
using TCatSysManagerLib;
using TwinCAT.SystemManager;
using System.Xml;
using System.Collections.Generic;
using System.Diagnostics;
using ScriptingTest;
using System.Reflection;

namespace Scripting.CSharp
{
    /// <summary>
    /// Demonstrates the creation of PLC projects
    /// </summary>
    public class ManagePlcLibraries
        : ScriptEarlyBound
    {
        ITcSysManager4 systemManager = null;
        Project project = null;

        ITcSmTreeItem plcConfig = null;
        ITcSmTreeItem plcProjectRoot = null;
        ITcSmTreeItem plcProject = null;
        ITcSmTreeItem plcInstances = null;

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

            plcConfig = systemManager.LookupTreeItem("TIPC"); // Getting NC Configuration

            // Generate a PLC Project
            _context.Worker.ProgressStatus = "Creating empty PLC Project ...";
            plcProjectRoot = plcConfig.CreateChild("PlcGenerated", 0, "", vsXaePlcStandardTemplateName);
            plcProject = systemManager.LookupTreeItem("TIPC^PlcGenerated^PlcGenerated Project"); // PlcProject (PlcControl side)
            plcInstances = systemManager.LookupTreeItem("TIPC^PlcGenerated^PlcGenerated Instance"); // Instances

            _context.Worker.ProgressStatus = "PLC Project created ...";
            
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
        /// Handler function Executing the Script code.
        /// </summary>
        /// <param name="worker">The worker.</param>
        protected override void OnExecute(IWorker worker)
        {   
            worker.Progress = 0;
            if (worker.CancellationPending)
                throw new Exception("Cancel");

            worker.Progress = 25;

            //###############################################################################################################################################################
            //string basePath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + @"\Templates\";
            //string sampleLibrary = basePath + @"Tc2_AiSampleLib.library";
            string sampleLibrary = Path.Combine(Script.ApplicationDirectory, "Templates", "Tc2_AiSampleLib.library");

            //###############################################################################################################################################################
            // Determination of the Library Manager Interface
            //###############################################################################################################################################################

            ITcSmTreeItem project = systemManager.LookupTreeItem("TIPC^PlcGenerated^PlcGenerated Project");
            ITcSmTreeItem referencesItem = systemManager.LookupTreeItem("TIPC^PlcGenerated^PlcGenerated Project^References");

            ITcPlcLibraryManager libraryManager = (ITcPlcLibraryManager)referencesItem;

            // Alternatively:
            //ITcPlcLibraryManager libraryManager = (ITcPlcLibraryManager)project;

            //###############################################################################################################################################################
            // Iteration over Repositories and Installed libraries
            //###############################################################################################################################################################

            worker.ProgressStatus = "Iterating through Repositories:";
            foreach (ITcPlcLibRepository repo in libraryManager.Repositories)
            {
                worker.ProgressStatus = string.Format("\tRepository: {0} Folder: {1}", repo.Name, repo.Folder);
            }

            worker.ProgressStatus = "Iterating through Libraries:";
            foreach (ITcPlcLibRef libRef in libraryManager.References)
            {
                if (libRef is ITcPlcLibrary)
                {
                    ITcPlcLibrary lib = (ITcPlcLibrary)libRef;

                    string displayName = lib.DisplayName;
                    worker.ProgressStatus = string.Format("\tLibrary: {0}", displayName);

                }
                else if (libRef is ITcPlcPlaceholderRef)
                {
                    ITcPlcPlaceholderRef placeholder = (ITcPlcPlaceholderRef)libRef;
                    string placeholderName = placeholder.PlaceholderName;

                    string effectiveResolution = string.Empty;
                    
                    if (placeholder.EffectiveResolution != null)
                        effectiveResolution = placeholder.EffectiveResolution.DisplayName;

                    worker.ProgressStatus = string.Format("\tPlaceholder: {0}", placeholderName);
                    worker.ProgressStatus = string.Format("\t\t Effective: {0}", effectiveResolution);
                }
            }

            //###############################################################################################################################################################
            // Repository Handling (Insert, Move, Add + Install Library). Uninstall and repository deletion will be shown at the end of this script
            //###############################################################################################################################################################
            if (!Directory.Exists(@"c:\tmp"))
                Directory.CreateDirectory(@"c:\tmp");

            // Ensuring that the System is in a clean state
            try
            {
                libraryManager.UninstallLibrary("TestRepo", "Tc2_AiSampleLib", "1.0.0.0", "Beckhoff Automation");
            } catch {}
            try
            {
                libraryManager.RemoveRepository("TestRepo");
            } catch {}


            libraryManager.InsertRepository("TestRepo", @"c:\tmp",0);
            libraryManager.InstallLibrary("TestRepo", sampleLibrary, false);

            try
            {
                libraryManager.MoveRepository("TestRepo", 1);

                worker.ProgressStatus = "Adding / Removing Libraries";

                //###############################################################################################################################################################
                //  Demonstrating different alternatives how to add/remove libraries
                //###############################################################################################################################################################

                // Adding and Removing libraries
                libraryManager.AddLibrary("Tc2_AiSampleLib", "*", "Beckhoff Automation"); // Adds the newest found Library
                libraryManager.RemoveReference("Tc2_AiSampleLib");

                // Adding and Removing libraries
                libraryManager.AddLibrary("Tc2_AiSampleLib", "1.*", "Beckhoff Automation"); // Adds the newest major version 1
                libraryManager.RemoveReference("Tc2_AiSampleLib");

                // Adding and Removing libraries (via their Display Text)
                libraryManager.AddLibrary("Tc2_AiSampleLib, * (Beckhoff Automation)"); // Adds the newest found Library
                libraryManager.RemoveReference("Tc2_AiSampleLib");

                // Adding and Removing Libraries
                libraryManager.AddLibrary("Tc2_AiSampleLib, 1.* (Beckhoff Automation)");
                libraryManager.RemoveReference("Tc2_AiSampleLib");


                // Adding libraries (via XML)
                string xmlAdd = @"
                                <TreeItem>
                                    <IECProjectDef>
                                        <AddReferences>
                                            <Library>
                                                <DisplayName>Tc2_AiSampleLib, * (Beckhoff Automation)</DisplayName>
                                            </Library>
                                        </AddReferences>
                                    </IECProjectDef>
                                </TreeItem>
                            ";

                project.ConsumeXml(xmlAdd);

                // Removing libraries (via XML)
                string xmlRemove = @"
                                <TreeItem>
                                    <IECProjectDef>
                                        <RemoveReferences>
                                            <Library>
                                                <DisplayName>Tc2_AiSampleLib, 1.0.0.0 (Beckhoff Automation)</DisplayName>
                                            </Library>
                                        </RemoveReferences>
                                    </IECProjectDef>
                                </TreeItem>
                            ";

                project.ConsumeXml(xmlRemove);


                //###############################################################################################################################################################
                //  Demonstrates how to scan for libraries
                //###############################################################################################################################################################

                // Scan for libraries (via XML)
                string xmlScanLibraries = @"
                                        <TreeItem>
                                            <IECProjectDef>
                                                <ScanLibraries>
                                                    <Active>true</Active>
                                                </ScanLibraries>
                                            </IECProjectDef>
                                        </TreeItem>
                                      ";

                project.ConsumeXml(xmlScanLibraries);
                string xmlScannedLibraries = project.ProduceXml(false);

                XmlDocument doc = new XmlDocument();
                doc.LoadXml(xmlScannedLibraries);

                XmlNodeList scannedLibrariesNodeList = doc.SelectNodes("TreeItem/IECProjectDef/ScannedLibraries/Library");
                worker.ProgressStatus = string.Format("{0} Xml coded libraries found!", scannedLibrariesNodeList.Count);

                foreach (XmlNode node in scannedLibrariesNodeList)
                {
                    string name = node.SelectSingleNode("LibraryName").InnerText;
                    worker.ProgressStatus = string.Format("\tLibrary: {0}", name);
                }

                worker.ProgressStatus = "==================";

                ITcPlcReferences scannedLibraries = libraryManager.ScanLibraries();

                worker.ProgressStatus = string.Format("{0} libraries found!", scannedLibraries.Count);
                worker.ProgressStatus = "Iterating Scanned Libraries:";
                foreach (ITcPlcLibrary scanned in scannedLibraries)
                {
                    worker.ProgressStatus = string.Format("\tLibrary: {0}", scanned.DisplayName);
                }

                //###############################################################################################################################################################
                //  Adding / Removing Placeholders
                //###############################################################################################################################################################

                libraryManager.AddPlaceholder("Tc2_MDP"); // Adding preexisting placeholder
                libraryManager.RemoveReference("Tc2_MDP"); // Removing placeholder Ref

                libraryManager.AddPlaceholder("Placeholder_AiSampleLib", "Tc2_AiSampleLib", "*", "Beckhoff Automation");

                libraryManager.SetEffectiveResolution("Placeholder_AiSampleLib", "Tc2_AiSampleLib", "1.0.0.0", "Beckhoff Automation");
                libraryManager.RemoveReference("Placeholder_AiSampleLib");


                string xmlAddPlaceholder = @"
                                <TreeItem>
                                    <IECProjectDef>
                                        <AddReferences>
                                            <PlaceholderReference>
                                                <Name>Placeholder_AiSampleLib</Name>
                                                <DefaultResolution>Tc2_AiSampleLib, * (Beckhoff Automation)</DefaultResolution>
                                            </PlaceholderReference>
                                        </AddReferences>
                                    </IECProjectDef>
                                </TreeItem>
                            ";

                string xmlRemovePlaceholder = @"
                                <TreeItem>
                                    <IECProjectDef>
                                        <RemoveReferences>
                                            <PlaceholderReference>
                                                <Name>Placeholder_AiSampleLib</Name>
                                            </PlaceholderReference>
                                        </RemoveReferences>
                                    </IECProjectDef>
                                </TreeItem>
                            ";

                project.ConsumeXml(xmlAddPlaceholder);


                //###############################################################################################################################################################
                //  Setting the CurrentLibrary / Effective Resolution
                //###############################################################################################################################################################

                //Set Effective Resolution
                string setEffResolution = @"
                    <TreeItem>
                        <IECProjectDef>
                            <References>
                                <PlaceholderReference>
				                    <Name>Placeholder_AiSampleLib</Name>
				                    <!-- <LibItemName>#Tc3_Module</LibItemName> -->
				                    <CurrentLibrary>
					                    <Name>Tc2_AiSampleLib</Name>
					                    <Version>1.0.0.0</Version>
					                    <Distributor>Beckhoff Automation</Distributor>
    			                    </CurrentLibrary>
                                    <!-- <CurrentLibrary>
                                        <DisplayName>Tc2_AiSampleLib,1.0.0.0 (Beckhoff Automation)</DisplayName>
                                    </CurrentLibrary> -->
			                    </PlaceholderReference>
                            </References>
                        </IECProjectDef>
                    </TreeItem>
            ";

                project.ConsumeXml(setEffResolution);

                //###############################################################################################################################################################
                //  Freeze Placeholders
                //###############################################################################################################################################################
                libraryManager.FreezePlaceholder();
                worker.Progress = 100;

                //###############################################################################################################################################################
                //  Remove placeholder, uninstall library from repository and then delete repository
                //###############################################################################################################################################################
                project.ConsumeXml(xmlRemovePlaceholder);
            }
            finally
            {
                libraryManager.UninstallLibrary("TestRepo", "Tc2_AiSampleLib", "1.0.0.0", "Beckhoff Automation");
                libraryManager.RemoveRepository("TestRepo");
            }
        }

        /// <summary>
        /// Gets the Script description
        /// </summary>
        /// <value>The description.</value>
        public override string Description
        {
            get { return "Demonstrates how to manage PLC Libs, Placeholders and Repositories. Shows how to (Version-)freeze Library references."; }
        }

        /// <summary>
        /// Gets the keywords, describing the Script features
        /// </summary>
        /// <value>The keywords.</value>
        public override string Keywords
        {
            get
            {
                return "References, Libraries, Placeholders, Repositories, Freeze Libraries";
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
    }
}
