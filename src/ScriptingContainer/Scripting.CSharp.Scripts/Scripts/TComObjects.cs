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
using System.Runtime.InteropServices;

namespace Scripting.CSharp
{
	/// <summary>
	/// Demonstrates the generation + compilation of PLC projects
	/// </summary>
	public class TComObjects
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
		/// <summary>
		/// Handler function Executing the Script code.
		/// </summary>
		/// <param name="worker">The worker.</param>
		protected override void OnExecute(IWorker worker)
		{   
			worker.Progress = 0;

			if (worker.CancellationPending)
				throw new Exception("Cancelled");

			// Adding standard TreeItems
			ITcSmTreeItem systemConfiguration = systemManager.LookupTreeItem("TIRC"); // System
			ITcSmTreeItem realtimeConfiguration = systemManager.LookupTreeItem("TIRS"); // Realtime-Settings
			ITcSmTreeItem ncConfiguration = systemManager.LookupTreeItem("TINC"); // NC-Configuration
			ITcSmTreeItem plcConfiguration = systemManager.LookupTreeItem("TIPC"); // Plc-Configuration

			ITcSmTreeItem systemItem = systemManager.LookupTreeItem("TIRC"); // System Configuration Item
			ITcSmTreeItem tcomGroup = systemManager.LookupTreeItem("TIRC^TcCOM Objects"); // Getting the TCOM Group Item
			ITcSmTreeItem tasks = systemManager.LookupTreeItem("TIRT");

			// Create Additional Tasks.
			ITcSmTreeItem additionalTask1 = tasks.CreateChild("AdditionalTask1", 1);
			ITcSmTreeItem additionalTask2 = tasks.CreateChild("AdditionalTask2", 1);

			// Accessing Module Manager via specific interface (ITcModuleManager)
			ITcModuleManager2 moduleManager = (ITcModuleManager2)systemManager.GetModuleManager();
			Debug.Assert(moduleManager.ModuleCount == 0); // Module Count is 0

			// The following TComModules should be installed in the System. They are distributed as part of the ScriptingContainer.
			// See TComModules folder in the root.
			Dictionary<string,Guid> tcomModuleTable = new Dictionary<string,Guid>();
			tcomModuleTable.Add("TempContr",Guid.Parse("{8f5fdcff-ee4b-4ee5-80b1-25eb23bd1b45}")); // Temperature Controller
			tcomModuleTable.Add("ContrSysPT2",Guid.Parse("{acd3c8a0-d974-4eb0-91e6-a4a0eb4db128}")); // ContrSysPT2

            worker.Progress = 10;
            worker.ProgressStatus = "Creating TCOM objects ...";

			// Adding Registered TComObject (via string)
            string tempContr1Name = "TemperatureController1";
            ITcSmTreeItem tempController1Item = null;

            string tcomModuleNotAvailableMessage = @"Please ensure that the TCOM Modules

    'TempContr   ({8f5fdcff-ee4b-4ee5-80b1-25eb23bd1b45})' and
    'ContrSysPT2 ({acd3c8a0-d974-4eb0-91e6-a4a0eb4db128})'

are available!
For registering them into the System please either copy the 'TComModules' folder to 
[TwinCATInstallFolder\[Version]\CustomConfig\Modules
or extend the
[TwinCATInstallFolder\[Version]\Config\Io\TcModuleFolders.xml
with the Scripting Container 'TComModules' folder.";

            try
            {
                tempController1Item = tcomGroup.CreateChild(tempContr1Name, 0, "", tcomModuleTable["TempContr"]);
                tempController1Item.Name = tempContr1Name;
            }
            catch (COMException ex)
            {
                if (ex.ErrorCode == (int)TCSYSMANAGERHRESULTS.TSM_E_INVALIDVINFO)
                    throw new ApplicationException(tcomModuleNotAvailableMessage);
                else
                    throw ex;
            }
			Debug.Assert(moduleManager.ModuleCount == 1); // Module Count is now 1

            worker.ProgressStatus = string.Format("TCOM Object '{0}' added.",tempContr1Name);
            worker.Progress = 20;

			ITcModuleInstance2 tcomObjectInstance = (ITcModuleInstance2)tempController1Item;

			//Alternatively TCOM Objects can be added by XML TMI Data
			string tempController1Xml = tempController1Item.ProduceXml(); // Recycling TMI Xml Data from first TCOM OBject
			XmlDocument tempController2Doc = new XmlDocument();
			tempController2Doc.LoadXml(tempController1Xml);

            worker.Progress = 30;

			// Manipulating the ModuleInstance Data
			XmlNode tempController2TmiNode = tempController2Doc.SelectSingleNode("TreeItem/TcModuleInstance");
			XmlNode instanceNameNode = tempController2Doc.SelectSingleNode("TreeItem/TcModuleInstance/Module/InstanceName");

            string tempContrl2Name = "TemperatureController2";
            instanceNameNode.InnerText = tempContrl2Name; // E.g changing Instance Name

			string tempController2TmiXml = tempController2TmiNode.OuterXml; // Create the <TcModuleInstance> XML data

			// Adding Registered TComObject (via manipulated TMI Content data (xml))
            ITcSmTreeItem tempController2Item = null;


            try
            {
                tempController2Item = tcomGroup.CreateChild("Unknown", 0, "", tempController2TmiXml);
                tempController2Item.Name = tempContrl2Name;
            }
            catch (COMException ex)
            {
                if (ex.ErrorCode == (int)TCSYSMANAGERHRESULTS.TSM_E_INVALIDVINFO)
                    throw new ApplicationException(tcomModuleNotAvailableMessage);
                else
                    throw ex;
            }

			Debug.Assert(moduleManager.ModuleCount == 2);

            worker.ProgressStatus = string.Format("TCOM Object '{0}' added.", tempContrl2Name);
            worker.Progress = 40;

			ITcModuleInstance2 moduleInstance1 = (ITcModuleInstance2)tempController1Item; // Can cast ITcTreeItems to ITcModuleInstance if TComModule
			ITcModuleInstance2 moduleInstance2 = (ITcModuleInstance2)tempController2Item;

			//TODO: To be documented???
			//moduleManager.GetOnlineModulePara();
			//moduleManager.SetOnlineModulePara();
			//moduleManager.LookupModule();
			//moduleManager.TryLookupModule();

            worker.ProgressStatus = "Browsing ModuleInstances ...";

			// Demonstrate ModuleManager
			foreach (ITcModuleInstance2 moduleInstance in moduleManager) // Iterate over Module manager
			{
				// The Module Manager Should contain now 2 Modules

				string moduleType = moduleInstance.ModuleTypeName;
				string instanceName = moduleInstance.ModuleInstanceName;

				Guid classId = moduleInstance.ClassID;
				uint objId = moduleInstance.oid;
				uint parentObjId = moduleInstance.ParentOID;

				//moduleInstance.GetModulePara(pid1);
				//moduleManager.GetOnlineModulePara(objId, pid1, maxData1);

				ITcSmTreeItem treeItem = moduleInstance as ITcSmTreeItem;
				Debug.Assert(treeItem != null); // Module object is an ITcSmTreeItem also.

                worker.ProgressStatus = string.Format("\t{0}:{1} ({2})", instanceName,moduleType,classId);
			}

            worker.ProgressStatus = "Browsing ModuleInstances finished.";

            //Searching for Module Types
            //ITcEnumModuleInstances instanceIterator = moduleManager.LookupModule("TempContr");
            //// Instances should contain 2 Modules of the same Type "TempContr";

            //List<ITcModuleInstance2> temp = new List<ITcModuleInstance2>();
            //ITcModuleInstance2 act = null;

            //do
            //{
            //    uint fetched = 0;
            //    instanceIterator.Next(1, out act, out fetched);
				
            //    if (fetched > 0)
            //        temp.Add(act);

            //} while (act != null);

            //ITcEnumModuleInstances instanceIterator2;
            //bool found = moduleManager.TryLookupModule("CRalfsCyclic", out instanceIterator2);

            worker.Progress = 50;

            worker.ProgressStatus = "Setting Parameters ...";

			//Setting the Copy Symbol Flag
			setParameterCreateSymbol(tempController2Item,"CallBy",false);
			setParameterCreateSymbol(tempController2Item, "CallBy", true);

			setParameterInitValue(tempController2Item, TmiValueType.EnumText,"CallBy", "Module");
			setParameterInitValue(tempController2Item, TmiValueType.EnumText, "CallBy", "Task");

			uint pctid = getParameterId(tempController1Item,"CallBy");
			dynamic value = moduleInstance1.GetModulePara(pctid);

            worker.Progress = 60;

			DataAreaInfo[] dataAreas = getDataAreas(tempController1Item);

			setDataAreaCreateSymbols(tempController1Item, 0, "Input", true);
			setDataAreaCreateSymbols(tempController1Item, 0, "Output", true);

            worker.Progress = 70;

			setTComTask(tempController1Item, 0, additionalTask1);
			setTComTask(tempController2Item, 0, additionalTask2);

            worker.ProgressStatus = string.Format("Adding PLC Axis project ...");

			// Adding PLC Project
			string plcAxisTemplatePath = Path.Combine(ConfigurationTemplatesFolder, "PlcAxisTemplate.tpzip");

			ITcSmTreeItem plc = plcConfiguration.CreateChild("PlcAxisSample", 0, "", plcAxisTemplatePath);

			// Setting the priority of the PlcTask
			ITcSmTreeItem plcTask = systemManager.LookupTreeItem("TIRT^PlcTask");
			
			// Trigger TCOM Object by plc Task
			setTComTask(tempController2Item, 0, plcTask);

            worker.Progress = 90;
            worker.ProgressStatus = string.Format("PLC Axis project added.");

            // Linking TCOM Objects to PLC
            worker.ProgressStatus = string.Format("Linking TCOM Objects to PLC ...");
            systemManager.LinkVariables("TIRC^TcCOM Objects^TemperatureController1^Input^ExternalSetpoint", "TIPC^PlcAxisSample^PlcAxisSample Instance^PlcTask Outputs^MAIN.fbInst.iOut");
            systemManager.LinkVariables("TIRC^TcCOM Objects^TemperatureController1^Output^HeaterOn", "TIPC^PlcAxisSample^PlcAxisSample Instance^PlcTask Inputs^MAIN.bIn");
		}

        /// <summary>
        /// Sets the specified task to the TCOM object.
        /// </summary>
        /// <param name="tcomObject">The TCOM object.</param>
        /// <param name="contextId">The context id.</param>
        /// <param name="task">The task to set.</param>
        /// <exception cref="System.ArgumentNullException">tcomObject
        /// or
        /// task</exception>
        /// <exception cref="System.ArgumentException">WrongType;tcomObject</exception>
		private void setTComTask(ITcSmTreeItem tcomObject, int contextId, ITcSmTreeItem task)
		{
			if (tcomObject == null) throw new ArgumentNullException("tcomObject");
			if (task == null) throw new ArgumentNullException("task");

			if (tcomObject.ItemType != (int)TreeItemType.TComObject) throw new ArgumentException("WrongType","tcomObject");

			ITcModuleInstance2 mi = (ITcModuleInstance2)tcomObject;
			uint taskObjectId = getTaskObjectId(task);

			mi.SetModuleContext((uint)contextId, taskObjectId);
		}

		/// <summary>
		/// Gets the task ObjectId of the specified task item
		/// </summary>
		/// <param name="task">The task.</param>
		/// <returns>The task ObjectID</returns>
		/// <exception cref="System.ArgumentNullException">task</exception>
		/// <exception cref="System.ArgumentException">WrongType;task</exception>
		private uint getTaskObjectId(ITcSmTreeItem task)
		{
			if (task == null) throw new ArgumentNullException("task");
			if (task.ItemType != (int)TreeItemType.Task) throw new ArgumentException("WrongType", "task");

			string taskXml = task.ProduceXml();
			XmlDocument doc = new XmlDocument();
			doc.LoadXml(taskXml);
			XmlNode objectIdNode = doc.SelectSingleNode("TreeItem/ObjectId");
			string val = objectIdNode.InnerText;
			uint ret = TcXmlConvert.ReadHexUIntValue(val);
			return ret;
		}

		/// <summary>
		/// Sets the create symbol indicator on the specified TCOM objects Parameter
		/// </summary>
		/// <param name="tcomObject">The TCOM object.</param>
		/// <param name="parameterName">Name of the parameter to change.</param>
		/// <param name="createSymbol">if set to <c>true</c> then the symbols will be created, otherwise <c>false</c></param>
		/// <exception cref="System.ArgumentNullException">
		/// tcomObject
		/// or
		/// parameterName
		/// </exception>
		/// <exception cref="System.ArgumentException">TreeItem is not a TCOM Object;tcomObject</exception>
		private void setParameterCreateSymbol(ITcSmTreeItem tcomObject, string parameterName, bool createSymbol)
		{
			if (tcomObject == null) throw new ArgumentNullException("tcomObject");
			if (string.IsNullOrEmpty(parameterName)) throw new ArgumentNullException("parameterName");
			if (tcomObject.ItemType != (int)TreeItemType.TComObject) throw new ArgumentException("TreeItem is not a TCOM Object", "tcomObject");

			string sourceXml = tcomObject.ProduceXml();

			XmlDocument sourceDoc = new XmlDocument();
			sourceDoc.LoadXml(sourceXml);

			XmlNode sourceParametersNode = sourceDoc.SelectSingleNode("TreeItem/TcModuleInstance/Module/Parameters");

			XmlDocument targetDoc = new XmlDocument();
			XmlElement treeItemElement = targetDoc.CreateElement("TreeItem");
			XmlElement moduleInstanceElement = targetDoc.CreateElement("TcModuleInstance");
			XmlElement moduleElement = targetDoc.CreateElement("Module");
			XmlElement parametersElement = (XmlElement)targetDoc.ImportNode(sourceParametersNode, true);

			moduleElement.AppendChild(parametersElement);
			moduleInstanceElement.AppendChild(moduleElement);
			treeItemElement.AppendChild(moduleInstanceElement);
			targetDoc.AppendChild(treeItemElement);

			setParameterCreateSymbol(targetDoc, parameterName, createSymbol);

			string targetXml = targetDoc.OuterXml;
			tcomObject.ConsumeXml(targetXml);
		}

		/// <summary>
		/// Sets the create symbol flag within the document for the specified parameter
		/// </summary>
		/// <param name="doc">The XML Document</param>
		/// <param name="parameterName">Name of the Parameter</param>
		/// <param name="value">Value of the CreateSymbol flag</param>
		/// <exception cref="System.ArgumentException">parameterName</exception>
		private void setParameterCreateSymbol(XmlDocument doc, string parameterName, bool value)
		{
            if (doc == null) throw new ArgumentNullException("doc");
            if (string.IsNullOrEmpty(parameterName)) throw new ArgumentException();

			XmlNode parametersNode = doc.SelectSingleNode("TreeItem/TcModuleInstance/Module/Parameters");
			XmlNode parameterNode = parametersNode.SelectSingleNode(string.Format("Parameter[Name='{0}']",parameterName));

			if (parameterNode != null)
			{
				XmlAttribute createSymbolAttr = parameterNode.Attributes["CreateSymbol"];

				if (createSymbolAttr != null)
				{
					string oldValue = createSymbolAttr.Value;
				}
				else
				{
					createSymbolAttr = doc.CreateAttribute("CreateSymbol");
					parameterNode.Attributes.Append(createSymbolAttr);
				}
				createSymbolAttr.Value = XmlConvert.ToString(value);
			}
			else
			{
				throw new ArgumentException(string.Format("Argument {0} not found!", parameterName), "parameterName");
			}
		}

		private void setParameterInitValue(ITcSmTreeItem tcomObject, TmiValueType type, string parameterPath, string value)
		{
			if (tcomObject == null) throw new ArgumentNullException("tcomObject");
			if (string.IsNullOrEmpty(parameterPath)) throw new ArgumentNullException("parameterPath");
			if (tcomObject.ItemType != (int)TreeItemType.TComObject) throw new ArgumentException("TreeItem is not a TCOM Object", "tcomObject");


			bool dedicatedInterface = false;


			if (dedicatedInterface)
			{
				//ITcModuleInstance2 moduleInstance = (ITcModuleInstance2)tcomObject;
				//moduleInstance.SetModulePara(xxx);
			}
			else
			{
				string sourceXml = tcomObject.ProduceXml();

				XmlDocument sourceDoc = new XmlDocument();
				sourceDoc.LoadXml(sourceXml);

				List<string> symbols = getInitParameterSymbols(sourceDoc);

				if (symbols.Contains(parameterPath))
				{
					XmlElement sourceParameterValue = (XmlElement)sourceDoc.SelectSingleNode(string.Format("TreeItem/TcModuleInstance/Module/ParameterValues/Value[Name='{0}']", parameterPath));

					XmlDocument targetDoc = new XmlDocument();
					XmlElement treeItemElement = targetDoc.CreateElement("TreeItem");
					XmlElement moduleInstanceElement = targetDoc.CreateElement("TcModuleInstance");
					XmlElement moduleElement = targetDoc.CreateElement("Module");
					XmlElement parameterValues = targetDoc.CreateElement("ParameterValues");
					XmlElement valueElement = (XmlElement)targetDoc.ImportNode(sourceParameterValue, true);

					parameterValues.AppendChild(valueElement);
					moduleElement.AppendChild(parameterValues);
					moduleInstanceElement.AppendChild(moduleElement);
					treeItemElement.AppendChild(moduleInstanceElement);
					targetDoc.AppendChild(treeItemElement);

					setParameterInitValue(targetDoc, type, parameterPath, value);

					string targetXml = targetDoc.OuterXml;
					tcomObject.ConsumeXml(targetXml);
				}
				else
				{
					throw new ArgumentException(string.Format("Parameter '{0}' not found. Cannot set value!", parameterPath), "parameterName");
				}
			}
		}

        /// <summary>
        /// Sets the parameter init value within the document
        /// </summary>
        /// <param name="doc">The XML Document to set</param>
        /// <param name="type">Value Type</param>
        /// <param name="parameterName">Init parameter path (split by period '.')</param>
        /// <param name="value">The value to set.</param>
        /// <exception cref="System.ArgumentException">parameterName</exception>
		private void setParameterInitValue(XmlDocument doc, TmiValueType type,string parameterName, string value)
		{
			string[] nameSplit = parameterName.Split('.');
			int count = nameSplit.Length;

			//<TreeItem>
			//    <TcModuleInstance>
			//        <Module>
			//            <ParameterValues>
			//	            <Value>
			//		            <Name>Parameter.data1</Name>
			//		            <Value>1</Value>
			//	            </Value>
			//	            <Value>
			//		            <Name>Parameter.data2</Name>
			//		            <Value>2</Value>
			//	            </Value>
			//	            <Value>
			//		            <Name>Parameter.data3</Name>
			//		            <Value>3</Value>
			//	            </Value>
			//            </ParameterValues>
			//        </Module>
			//    </TcModuleInstance>
			//</TreeItem>

            XmlElement parameterValue = null;

            string xPath = string.Format("TreeItem/TcModuleInstance/Module/ParameterValues/Value[Name='{0}']/{1}", parameterName, type.ToString());
            parameterValue = (XmlElement)doc.SelectSingleNode(xPath);

			if (parameterValue != null)
			{
				parameterValue.InnerText = value;
			}
			else
			{
				throw new ArgumentException(string.Format("Parameter '{0}' not found. Cannot set value!", parameterName), "parameterName");
			}
		}

		private Dictionary<string,uint> getParameterIDs(ITcSmTreeItem tcomObject)
		{
			if (tcomObject == null) throw new ArgumentNullException("tcomObject");
			if (tcomObject.ItemType != (int)TreeItemType.TComObject) throw new ArgumentException("TreeItem is not a TCOM Object", "tcomObject");

			string sourceXml = tcomObject.ProduceXml();

			XmlDocument doc = new XmlDocument();
			doc.LoadXml(sourceXml);

			return getParameterIds(doc);
		}

		private static Dictionary<string, uint> getParameterIds(XmlDocument doc)
		{
			Dictionary<string, uint> ret = new Dictionary<string, uint>(StringComparer.OrdinalIgnoreCase); // Dictionary ParameterName --> PTCID (ParameterID)
			XmlNodeList availableParameters = doc.SelectNodes("TreeItem/TcModuleInstance/Module/Parameters/Parameter[not(boolean(@HideParameter)) or @HideParameter!=true]");

			foreach (XmlNode rootParameter in availableParameters)
			{
				XmlNode nameNode = rootParameter.SelectSingleNode("Name");
                Debug.Assert(nameNode != null,"No Name Tag found!");
				string name = nameNode.InnerText;

				XmlNode parameterIdNode = rootParameter.SelectSingleNode("PTCID");
                Debug.Assert(parameterIdNode != null, "No PTCID Tag found!");

                if (parameterIdNode != null)
                {
                    uint ptcid = TcXmlConvert.ReadHexUIntValue(parameterIdNode.InnerText);
                    ret.Add(name, ptcid);
                }
			}
			return ret;
		}

		private DataAreaInfo[] getDataAreas(ITcSmTreeItem tcomObject)
		{
			if (tcomObject == null) throw new ArgumentNullException("tcomObject");
			if (tcomObject.ItemType != (int)TreeItemType.TComObject) throw new ArgumentException("TreeItem is not a TCOM Object", "tcomObject");

			string sourceXml = tcomObject.ProduceXml();

			XmlDocument doc = new XmlDocument();
			doc.LoadXml(sourceXml);

			return getDataAreas(doc);
		}

		/// <summary>
		/// Gets the data areas fro the TCOM Document
		/// </summary>
		/// <param name="doc">The doc.</param>
		/// <returns></returns>
		private DataAreaInfo[] getDataAreas(XmlDocument doc)
		{
			XmlNodeList dataAreaNodes = doc.SelectNodes("TreeItem/TcModuleInstance/Module/DataAreas/DataArea");

			DataAreaInfo[] dataAreas = new DataAreaInfo[dataAreaNodes.Count];

			for(int i=0; i<dataAreaNodes.Count; i++)
			{
				dataAreas[i] = new DataAreaInfo((XmlElement)dataAreaNodes[i]); 
			}
			return dataAreas;
		}

		/// <summary>
		/// Gets a specific Data Area
		/// </summary>
		/// <param name="tcomObject">The tcom object.</param>
		/// <param name="context">The data area context.</param>
		/// <param name="dataAreaName">Name of the data area to find.</param>
		/// <returns></returns>
		/// <exception cref="System.ArgumentNullException">tcomObject</exception>
		/// <exception cref="System.ArgumentException">TreeItem is not a TCOM Object;tcomObject</exception>
		private XmlElement getDataArea(ITcSmTreeItem tcomObject, int context, string dataAreaName)
		{
			if (tcomObject == null) throw new ArgumentNullException("tcomObject");
			if (tcomObject.ItemType != (int)TreeItemType.TComObject) throw new ArgumentException("TreeItem is not a TCOM Object", "tcomObject");

			string sourceXml = tcomObject.ProduceXml();

			XmlDocument doc = new XmlDocument();
			doc.LoadXml(sourceXml);

			return getDataArea(doc, context, dataAreaName);
		}

		private XmlElement getDataArea(XmlDocument doc, int context, string dataAreaName)
		{
			XmlElement dataArea = (XmlElement)doc.SelectSingleNode(string.Format("TreeItem/TcModuleInstance/Module/DataAreas/DataArea[ContextId='{0}' and Name='{1}']", context, dataAreaName));

			if (dataArea != null)
				return dataArea;
			else
				throw new ApplicationException(string.Format("DataArea Element Name '{0}' not found in context '{1}'!", dataAreaName, context));
		}

		private void setDataAreaCreateSymbols(ITcSmTreeItem tcomObject, int context, string dataAreaName, bool createSymbols)
		{
			if (tcomObject == null) throw new ArgumentNullException("tcomObject");
			if (tcomObject.ItemType != (int)TreeItemType.TComObject) throw new ArgumentException("TreeItem is not a TCOM Object", "tcomObject");

			string sourceXml = tcomObject.ProduceXml();

			XmlDocument sourceDoc = new XmlDocument();
			sourceDoc.LoadXml(sourceXml);

			// Select the data Areas in Source Document
			XmlNode sourceDataAreas = sourceDoc.SelectSingleNode("TreeItem/TcModuleInstance/Module/DataAreas");

			// Create a Document copy of the relevant part (DataAreas) of source document
			XmlDocument targetDoc = new XmlDocument();

			XmlElement treeItem = targetDoc.CreateElement("TreeItem");
			XmlElement moduleInstance = targetDoc.CreateElement("TcModuleInstance");
			XmlElement module = targetDoc.CreateElement("Module");
			XmlElement dataAreas = (XmlElement)targetDoc.ImportNode(sourceDataAreas,true);

			targetDoc.AppendChild(treeItem);
			treeItem.AppendChild(moduleInstance);
			moduleInstance.AppendChild(module);
			module.AppendChild(dataAreas);

			// Return the requested target Element
			XmlElement dataArea = getDataArea(targetDoc, context, dataAreaName);
			
			// Set Property
			setDataAreaCreateSymbols(dataArea,createSymbols);
						
			// Save the Target Document
			string targetXml = targetDoc.OuterXml;
			tcomObject.ConsumeXml(targetXml);
		}

		private void setDataAreaCreateSymbols(XmlElement dataArea, bool createSymbols)
		{
			if (dataArea == null) throw new ArgumentNullException("dataArea");
			if (dataArea.Name != "DataArea") throw new ArgumentException("Wrong XML element","DataArea");

			XmlNode dataAreaNoNode = dataArea.SelectSingleNode("AreaNo");
			XmlAttribute createSymbolAttr = dataAreaNoNode.Attributes["CreateSymbols"];

			if (createSymbolAttr != null)
			{
				string oldValue = createSymbolAttr.Value;
			}
			else
			{
				createSymbolAttr = dataArea.OwnerDocument.CreateAttribute("CreateSymbols");
				dataAreaNoNode.Attributes.Append(createSymbolAttr);
			}
			createSymbolAttr.Value = XmlConvert.ToString(createSymbols);
		}

		private bool getDataAreaCreateSymbols(XmlElement dataArea)
		{
			if (dataArea == null) throw new ArgumentNullException("dataArea");
			if (dataArea.Name != "DataArea") throw new ArgumentException("Wrong XML element", "DataArea");

			XmlNode dataAreaNoNode = dataArea.SelectSingleNode("AreaNo");
			XmlAttribute attr = dataAreaNoNode.Attributes["CreateSymbols"];

			if (attr != null)
			{
				return XmlConvert.ToBoolean(attr.Value);
			}
			else
				return false;
		}

		private void setSymbolCreateSymbol(ITcSmTreeItem tcomObject, string areaName, string symbolName, bool createSymbol)
		{
			if (tcomObject == null) throw new ArgumentNullException("tcomObject");
			if (tcomObject.ItemType != (int)TreeItemType.TComObject) throw new ArgumentException("TreeItem is not a TCOM Object", "tcomObject");

			string sourceXml = tcomObject.ProduceXml();

			XmlDocument sourceDoc = new XmlDocument();
			sourceDoc.LoadXml(sourceXml);

			XmlNode sourceDataAreas = sourceDoc.SelectSingleNode("TreeItem/TcModuleInstance/Module/DataAreas");

			XmlDocument targetDoc = new XmlDocument();

			XmlElement treeItem = targetDoc.CreateElement("TreeItem");
			XmlElement moduleInstance = targetDoc.CreateElement("TcModuleInstance");
			XmlElement module = targetDoc.CreateElement("Module");
			XmlElement dataAreas = (XmlElement)targetDoc.ImportNode(sourceDataAreas,true); // Create a deep copy of the SourceNodes under "DataAreas"

			targetDoc.AppendChild(treeItem);
			treeItem.AppendChild(moduleInstance);
			moduleInstance.AppendChild(module);
			module.AppendChild(dataAreas);

			XmlElement symbol = selectSymbol(dataAreas, areaName, symbolName);
			setSymbolCreateSymbol(symbol, createSymbol);

			// Save the Target Document
			string targetXml = targetDoc.OuterXml;
			tcomObject.ConsumeXml(targetXml);
		}

		private XmlElement selectSymbol(XmlElement dataAreasNode, string dataAreaName, string symbolName)
		{
			XmlElement foundSymbolNode = (XmlElement)dataAreasNode.SelectSingleNode(string.Format("DataAreas/DataArea/Symbol[../Name='{0}' and Name='{1}']", dataAreaName, symbolName));

			if (foundSymbolNode != null)
				return foundSymbolNode;
			else
				throw new ApplicationException("Symbol '{0}' not found within DataArea '{1}'!");
		}

		private void setSymbolCreateSymbol(XmlElement symbol, bool createSymbol)
		{
			if (symbol == null) throw new ArgumentNullException("symbol");
			if (symbol.Name != "Symbol") throw new ArgumentException("Wrong XML element", "symbol");

			XmlAttribute createSymbolAttr = symbol.Attributes["CreateSymbol"];

			if (createSymbolAttr != null)
			{
				string oldValue = createSymbolAttr.Value;
			}
			else
			{
				createSymbolAttr = symbol.OwnerDocument.CreateAttribute("CreateSymbol");
				symbol.Attributes.Append(createSymbolAttr);
			}
			createSymbolAttr.Value = XmlConvert.ToString(createSymbol);
		}

		private bool getSymbolCreateSymbol(XmlElement symbol)
		{
			if (symbol == null) throw new ArgumentNullException("symbol");
			if (symbol.Name != "Symbol") throw new ArgumentException("Wrong XML element", "symbol");

			XmlAttribute attr = symbol.Attributes["CreateSymbol"];

			if (attr != null)
			{
				return XmlConvert.ToBoolean(attr.Value);
			}
			else
				return false;
		}

		/// <summary>
		/// Gets the ParameterID of the specified TCOM Object and parameter
		/// </summary>
		/// <param name="tcomObject">The tcom object.</param>
		/// <param name="parameterName">Name of the parameter.</param>
		/// <returns></returns>
		private uint getParameterId(ITcSmTreeItem tcomObject, string parameterName)
		{
			Dictionary<string, uint> pctidDict = getParameterIDs(tcomObject);
			return pctidDict[parameterName];
		}

		/// <summary>
		/// Gets a list of the allowed parameter paths.
		/// </summary>
		/// <param name="doc">The XML Document describing the Module instance.</param>
		/// <returns>The list of Init Parameter Symbols</returns>
		private List<string> getInitParameterSymbols(XmlDocument doc)
		{
			XmlNodeList availableParameters = doc.SelectNodes("TreeItem/TcModuleInstance/Module/Parameters/Parameter[not(boolean(@HideParameter)) or @HideParameter!=true]");
			List<string> pathList = new List<string>();

			foreach (XmlNode rootParameter in availableParameters)
			{
				XmlNode nameNode = rootParameter.SelectSingleNode("Name");
				string name = nameNode.InnerText;

				XmlNode bitSizeNode = rootParameter.SelectSingleNode("BitSize");
				
                int bitSize = -1;

                if (bitSizeNode != null)
                    bitSize = int.Parse(bitSizeNode.InnerText);

                uint ptcid = 0;
				XmlNode parameterIdNode = rootParameter.SelectSingleNode("PTCID");

                if (parameterIdNode != null)
                    ptcid = TcXmlConvert.ReadHexUIntValue(parameterIdNode.InnerText);

				XmlNodeList subItemNodes = rootParameter.SelectNodes("SubItem");


				if (subItemNodes != null && subItemNodes.Count > 0)
				{
					getParameterSubItems(subItemNodes, name, ref pathList);
				}
				else
				{
					pathList.Add(name);
				}
			}
			return pathList;
		}

		/// <summary>
		/// Gets the Parameters SubItems and adds them into the symbols list.
		/// </summary>
		/// <param name="subItems">The nodes.</param>
		/// <param name="parentPath">Parent path of the symbol</param>
		/// <param name="symbols">The combined.</param>
		/// <exception cref="System.ArgumentNullException"></exception>
		private void getParameterSubItems(XmlNodeList subItems, string parentPath, ref List<string> symbols)
		{
			if (subItems == null) throw new ArgumentNullException("nodes");
			if (string.IsNullOrEmpty(parentPath)) throw new ArgumentNullException("parentPath");
			if (symbols == null) throw new ArgumentNullException("symbols");

			foreach (XmlElement subItem in subItems)
			{
				string name = subItem.SelectSingleNode("Name").InnerText;
				int bitSize = int.Parse(subItem.SelectSingleNode("BitSize").InnerText);
				int bitOffs = int.Parse(subItem.SelectSingleNode("BitOffs").InnerText);

				XmlNodeList subItemNodes = subItem.SelectNodes("SubItem");

				string actualPath = parentPath + "." + name;

				if (subItemNodes != null && subItemNodes.Count > 0)
				{
					getParameterSubItems(subItemNodes, actualPath, ref symbols);
				}
				else
				{
					symbols.Add(actualPath);
				}
			}
		}

		/// <summary>
		/// Gets the Script description
		/// </summary>
		/// <value>The description.</value>
		public override string Description
		{
			get { return "Creation and handling of TCOM objects!"; }
		}

		/// <summary>
		/// Gets the detailed description of the <see cref="Script"/> that is shown in the Method Tips.
		/// </summary>
		/// <value>The detailed description.</value>
		public override string DetailedDescription
		{
			get
			{
				string test = "Add and Removes TCOM Objects. Export/Import TMCs/TMIs.";
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
				return "TCCOM, TMC, TMI, Custom Drivers";
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
                return "4013";
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
                return "TcCOM";
            }
        }
	}

	/// <summary>
	/// DataAreaType for TCOM Objects
	/// </summary>
	public enum DataAreaType
	{
		/// <summary>
		/// Input
		/// </summary>
		InputDst,
		/// <summary>
		/// Output
		/// </summary>
		OutputSrc,
        /// <summary>
        /// Internal
        /// </summary>
        Internal
	}

	/// <summary>
	/// Data Area Info object
	/// </summary>
	public class DataAreaInfo
	{
		/// <summary>
		/// The area type
		/// </summary>
		public readonly DataAreaType AreaType;
		/// <summary>
		/// The area id
		/// </summary>
		public readonly int AreaId;
		/// <summary>
		/// The data area name
		/// </summary>
		public readonly string Name;
		/// <summary>
		/// The data area context ID
		/// </summary>
		public readonly int ContextId;
		/// <summary>
		/// The byte size of the DataArea
		/// </summary>
		public readonly int ByteSize;

		/// <summary>
		/// The symbols of the DataAray
		/// </summary>
		public readonly SymbolInfo[] Symbols;


		/// <summary>
		/// Initializes a new instance of the <see cref="DataAreaInfo"/> class.
		/// </summary>
		/// <param name="dataAreaElement">The data area element.</param>
		public DataAreaInfo(XmlElement dataAreaElement)
		{
			XmlNode noNode = dataAreaElement.SelectSingleNode("AreaNo");
			XmlNode typeNode = dataAreaElement.SelectSingleNode("AreaNo/@AreaType");
			XmlNode nameNode = dataAreaElement.SelectSingleNode("Name");
			XmlNode contextIdNode = dataAreaElement.SelectSingleNode("ContextId");
			XmlNode byteSizeNode = dataAreaElement.SelectSingleNode("ByteSize");

			this.AreaType = (DataAreaType)Enum.Parse(typeof(DataAreaType), typeNode.InnerText);
			this.AreaId = int.Parse(noNode.InnerText);
			this.Name = nameNode.InnerText;
			this.ContextId = int.Parse(contextIdNode.InnerText);
			this.ByteSize = int.Parse(byteSizeNode.InnerText);

			XmlNodeList symbols = dataAreaElement.SelectNodes("Symbol");
			Symbols = new SymbolInfo[symbols.Count];

			for (int i = 0; i < symbols.Count; i++)
			{
				this.Symbols[i] = new SymbolInfo((XmlElement)symbols[i]);
			}
		}
	}

	/// <summary>
	/// Data Area Symbol
	/// </summary>
	public class SymbolInfo
	{
		/// <summary>
		/// Symbol name
		/// </summary>
		public readonly string Name;
		/// <summary>
		/// Symbol Bit Size
		/// </summary>
		public readonly int BitSize;
		/// <summary>
		/// Base Type of the Symbol (as GUID)
		/// </summary>
		public readonly Guid BaseTypeGuid;
		/// <summary>
		/// Base Type of the Symbol
		/// </summary>
		public readonly string BaseType;

		/// <summary>
		/// Relative bit offset of the Symbol
		/// </summary>
		public readonly int BitOffset;

		/// <summary>
		/// Initializes a new instance of the <see cref="SymbolInfo"/> class.
		/// </summary>
		/// <param name="symbolElement">The symbol element.</param>
		public SymbolInfo(XmlElement symbolElement)
		{
			XmlNode nameNode = symbolElement.SelectSingleNode("Name");
			XmlNode bitSizeNode = symbolElement.SelectSingleNode("BitSize");
			XmlNode baseTypeNode = symbolElement.SelectSingleNode("BaseType");
			XmlNode baseTypeGuidNode = symbolElement.SelectSingleNode("BaseType/@GUID");
			XmlNode bitOffsNode = symbolElement.SelectSingleNode("BitOffs");

			this.Name = nameNode.InnerText;
			this.BitSize = int.Parse(bitSizeNode.InnerText);
			this.BaseTypeGuid = Guid.Parse(baseTypeGuidNode.InnerText);
			this.BaseType = baseTypeNode.InnerText;
			this.BitOffset = int.Parse(bitOffsNode.InnerText);
		}
	}

    /// <summary>
    /// Tmi Value Type
    /// </summary>
    public enum TmiValueType
    {
        /// <summary>
        /// Value
        /// </summary>
        Value,
        /// <summary>
        /// Enum Value
        /// </summary>
        EnumText,
        /// <summary>
        /// Data Value
        /// </summary>
        Data
    }
}
