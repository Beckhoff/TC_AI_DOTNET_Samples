using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TCatSysManagerLib;
using System.IO;
using System.Xml;
using EnvDTE80;
using EnvDTE;
using EnvDTE100;
using System.Xml.Linq;
using ScriptingTest;
using System.Xml.XPath;

namespace Scripting.CSharp
{
   

    /// <summary>
    /// Demonstrates how to assign tasks to CPU cores!
    /// </summary>
    public class RouteManagement
        : ScriptEarlyBound
    {
        /// <summary>
        /// Project
        /// </summary>
        Project project = null;

        /// <summary>
        /// System Manager
        /// </summary>
        ITcSysManager4 systemManager = null;

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
            project.Save();
            project = null;
            base.OnCleanUp(worker);
        }

        /// <summary>
        /// Handler function Executing the Script code.
        /// </summary>
        /// <param name="worker">The worker.</param>
        protected override void OnExecute(IWorker worker)
        {
            worker.Progress = 0;

            ITcSmTreeItem systemConfiguration = systemManager.LookupTreeItem("TIRC"); // System
            ITcSmTreeItem realtimeConfiguration = systemManager.LookupTreeItem("TIRS"); // Realtime-Settings
            ITcSmTreeItem routeConfiguration = systemManager.LookupTreeItem("TIRR"); // Route Settings

            // The following XML string triggers a Broadcast Search if consumed on TIRR node
            string xml = @"<TreeItem>
                                <RoutePrj>
                                    <TargetList>
                                        <BroadcastSearch>true</BroadcastSearch>
                                    </TargetList>
                                </RoutePrj>
                            </TreeItem>";

            worker.ProgressStatus = "Broadcast search ...";
            routeConfiguration.ConsumeXml(xml); // Trigger Broadcast Search
            string producedXml = routeConfiguration.ProduceXml(); // Get the result

            XmlDocument doc = new XmlDocument();
            XPathNavigator nav = doc.CreateNavigator();
            doc.LoadXml(producedXml);

            XPathNodeIterator iter = nav.Select("/TreeItem/RoutePrj/TargetList/Target/Name");

            foreach (XPathNavigator node in iter)
            {
                string routeName = node.Value;
                worker.ProgressStatus = string.Format("Found target '{0}'",routeName);
            }
            worker.Progress = 50;
            worker.ProgressStatus = "Creating static routes ...";

            addRoute(routeConfiguration, "CX-0F7EF4", "5.15.126.244.1.1", "172.17.36.203", true, true);
            addRoute(routeConfiguration, "CX-16BA1F", "5.22.186.31.1.1", "172.17.36.163", false, true);

            worker.Progress = 70;
            worker.ProgressStatus = "Creating project routes ...";

            addRoute(routeConfiguration, "CP_127E94", "172.17.36.221.1.1", "172.17.36.210", true, false);
            addRoute(routeConfiguration, "CX_02444F", "5.2.68.79.1.1", "172.17.36.166", false, false);

            worker.Progress = 100;
        }

        /// <summary>
        /// Adds a static or project route to a remote target
        /// </summary>
        /// <param name="routesNode">Routes node in TwinCAT XAE.</param>
        /// <param name="hostname">Hostname of the remote target.</param>
        /// <param name="amsNetId">AmsNetId of the remote target.</param>
        /// <param name="ipAddress">IP Address of the remote target.</param>
        /// <param name="useHostname">Selects if the hostname or IP address of the remote target should be used. true = hostname, false = IP address</param>
        /// <param name="staticRoute">Selects if a static or project route should be added. true = static, false = project</param>
        private void addRoute(ITcSmTreeItem routesNode, string hostname, string amsNetId, string ipAddress, bool useHostname, bool staticRoute)
        {
            string addStaticRouteHostname = @"<TreeItem>
                                                <RoutePrj>
                                                  <AddRoute>
                                                    <RemoteName>" + hostname + @"</RemoteName>
                                                    <RemoteNetId>" + amsNetId + @"</RemoteNetId>
                                                    <RemoteHostName>" + hostname + @"</RemoteHostName>
                                                    <UserName>Administrator</UserName>
                                                    <Password>1</Password>
                                                    <NoEncryption></NoEncryption>
                                                  </AddRoute>
                                                </RoutePrj>
                                              </TreeItem>";
            string addStaticRouteIp = @"<TreeItem>
                                                <RoutePrj>
                                                  <AddRoute>
                                                    <RemoteName>" + ipAddress + @"</RemoteName>
                                                    <RemoteNetId>" + amsNetId + @"</RemoteNetId>
                                                    <RemoteIpAddr>" + ipAddress + @"</RemoteIpAddr>
                                                    <UserName>Administrator</UserName>
                                                    <Password>1</Password>
                                                    <NoEncryption></NoEncryption>
                                                  </AddRoute>
                                                </RoutePrj>
                                              </TreeItem>";
            string addProjectRouteHostname = @"<TreeItem>
                                                 <RoutePrj>
                                                   <AddProjectRoute>
                                                     <Name>" + hostname + @"</Name>
                                                     <NetId>" + amsNetId + @"</NetId>
                                                     <HostName>" + hostname + @"</HostName>
                                                   </AddProjectRoute>
                                                 </RoutePrj>
                                              </TreeItem>";
            string addProjectRouteIp = @"<TreeItem>
                                           <RoutePrj>
                                             <AddProjectRoute>
                                               <Name>" + ipAddress + @"</Name>
                                               <NetId>" + amsNetId + @"</NetId>
                                               <IpAddr>" + ipAddress + @"</IpAddr>
                                             </AddProjectRoute>
                                           </RoutePrj>
                                         </TreeItem>";
            
            if (useHostname)
            {
                if (staticRoute)
                {
                    routesNode.ConsumeXml(addStaticRouteHostname);
                }
                else
                {
                    routesNode.ConsumeXml(addProjectRouteHostname);
                }
            }
            else
            {
                if (staticRoute)
                {
                    routesNode.ConsumeXml(addStaticRouteIp);
                }
                else
                {
                    routesNode.ConsumeXml(addProjectRouteIp);
                }
            }
        }

        /// <summary>
        /// Gets the Script description
        /// </summary>
        /// <value>The description.</value>
        public override string Description
        {
            get { return "Applies a Broadcast search and adds actual / project routes!"; }
        }

        /// <summary>
        /// Gets the keywords, describing the Script features
        /// </summary>
        /// <value>The keywords.</value>
        public override string Keywords
        {
            get
            {
                return "Broadcast Search, Routes";
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
                return "Basics";
            }
        }
    }
}
