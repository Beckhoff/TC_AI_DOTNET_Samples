using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using ScriptingTest;
using System.Diagnostics;

/* ===============================================================================================
 * This CS-File contains multiple helper classes which are being used to represent the information
 * from Orders.xml in a object hierarchy.
 * =============================================================================================== */
namespace CodeGenerationDemo
{
    /* ===============================================================================================
     * Enum which represents a PlcObjectType from Orders.xml
     * Example: <PlcObject type="Library">
     * =============================================================================================== */
    public enum PlcObjectType
    {
        DataType,
        Library,
        Placeholder,
        POU,
        Itf,
        Gvl
    }

    /* ===============================================================================================
     * This class represents order information (all nodes below <MachineOrders>) from Orders.xml
     * and also creates the corresponding ConfigurationInfo (looks for corresponding configuration 
     * below <AvailableConfiguration> according to configuration ID)
     * =============================================================================================== */
    public class OrderInfo
    {
        /* ===============================================================================================
         * Global variables
         * =============================================================================================== */
        private ConfigurationInfo _configurationInfo;
        private string _id;
        private string _customer;
        private DateTime _delivery;
        private string _projectName;
        private string _description;
        private int _serial;

        /* ===============================================================================================
         * Constructor
         * =============================================================================================== */
        public OrderInfo(XmlElement orderNode)
        {
            if (orderNode == null) throw new ArgumentNullException("orderNode");

            XmlDocument doc = orderNode.OwnerDocument;

            _id = orderNode.Attributes["id"].Value;

            XmlNode configRefNode = orderNode.SelectSingleNode("Configuration");
            string configurationId = configRefNode.InnerText;

            // look for corresponding configuration under <AvailableConfigurations> according to its ID
            string xPath = string.Format("//Root/AvailableConfigurations/Configuration[@id='{0}']", configurationId);
            XmlNode configNode = doc.SelectSingleNode(xPath);

            XmlNode customerNode = orderNode.SelectSingleNode("Customer");
            XmlNode deliveryNode = orderNode.SelectSingleNode("Delivery");
            XmlNode serialNode = orderNode.SelectSingleNode("SerialNo");
            XmlNode projectNameNode = orderNode.SelectSingleNode("ProjectName");
            XmlNode descriptionNode = orderNode.SelectSingleNode("Description");
            
            this._configurationInfo = new ConfigurationInfo((XmlElement)configNode);
            this._customer = customerNode.InnerText;
            this._serial = int.Parse(serialNode.InnerText);
            this._projectName = projectNameNode.InnerText;
            this._description = descriptionNode.InnerText;
            this._delivery = XmlConvert.ToDateTime(deliveryNode.InnerText,XmlDateTimeSerializationMode.Local);
        }
                
        public ConfigurationInfo ConfigurationInfo
        {
            get { return _configurationInfo; }
        }
                
        public string Id
        {
            get { return _id; }
        }
                
        public string Customer
        {
            get { return _customer; }
        }
                
        public DateTime Delivery
        {
            get { return _delivery; }
        }
                
        public string ProjectName
        {
            get { return _projectName; }
        }
                
        public string Description
        {
            get { return _description; }
        }
                
        public int Serial
        {
            get { return _serial; }
        }
    }


    /* ===============================================================================================
     * This class represents configuration information from Orders.xml under <AvailableConfigurations>
     * =============================================================================================== */
    public class ConfigurationInfo
    {
        private string _id;
        private string _name;
        private string _projectTemplate;
        private string _script;
        private string _description;
        private List<OptionInfo> _options;
        private List<PlcObjectInfo> _plcObjects;
        private string _plcProjectName;
        private List<TaskInfo> _motionTasks;
        private HardwareInfo _hardware;
        private MappingsInfo _mappings;

        /* ===============================================================================================
         * Constructor
         * =============================================================================================== */
        public ConfigurationInfo(XmlElement configNode)
        {
            XmlDocument doc = configNode.OwnerDocument;

            /* ------------------------------------------------------------------
             * Read configuration attributes 'id' and 'name'
             * ------------------------------------------------------------------ */
            this._id = configNode.Attributes["id"].Value;
            this._name = configNode.Attributes["name"].Value;

            /* ------------------------------------------------------------------
             * Read configuration node 'ProjectTemplates'
             * ------------------------------------------------------------------ */
            XmlNode projectTemplateNode = configNode.SelectSingleNode("ProjectTemplate");
            if (projectTemplateNode != null)
                this._projectTemplate = projectTemplateNode.InnerText;

            /* ------------------------------------------------------------------
             * Read configuration node 'Script'
             * ------------------------------------------------------------------ */
            XmlNode scriptNode = configNode.SelectSingleNode("Script");

            this._script = scriptNode.InnerText;

            /* ------------------------------------------------------------------
             * Read configuration node 'Description'
             * ------------------------------------------------------------------ */
            XmlNode descriptionNode = configNode.SelectSingleNode("Description");

            if (descriptionNode != null)
                this._description = descriptionNode.InnerText;

            /* ------------------------------------------------------------------
             * Read configuration node 'Plc\PlcProjectName'
             * ------------------------------------------------------------------ */
            XmlNode plcNameNode = configNode.SelectSingleNode("Plc/PlcProjectName");
            if (plcNameNode != null)
                _plcProjectName = plcNameNode.InnerText;
            else
                _plcProjectName = "GeneratedProject";

            /* ------------------------------------------------------------------
             * Read configuration node 'Plc\PlcObjects\PlcObject' and all child nodes
             * ------------------------------------------------------------------ */
            _plcObjects = new List<PlcObjectInfo>();
            XmlNodeList plcObjectNodeList = configNode.SelectNodes("Plc/PlcObjects/PlcObject");

            if (plcObjectNodeList != null)
                addPlcElements(ref _plcObjects, plcObjectNodeList);

            /* ------------------------------------------------------------------
             * Read configuration node 'Motion\Tasks'
             * ------------------------------------------------------------------ */
            _motionTasks = new List<TaskInfo>();
            XmlNodeList motionTasksNodeList = configNode.SelectNodes("Motion/Task");

            if (motionTasksNodeList != null)
                addMotionTasks(ref _motionTasks, motionTasksNodeList);

            /* ------------------------------------------------------------------
             * Read configuration node 'Hardware' and all child nodes
             * ------------------------------------------------------------------ */
            XmlElement hardwareElement = (XmlElement)configNode.SelectSingleNode("Hardware");

            if (hardwareElement != null)
                _hardware = new HardwareInfo(hardwareElement);

            /* ------------------------------------------------------------------
             * Read configuration node 'Mappings' (note: mappings are specified in another XML file)
             * ------------------------------------------------------------------ */
            XmlNode mappingsNode = configNode.SelectSingleNode("Mappings");

            if (mappingsNode != null)
                _mappings = new MappingsInfo((XmlElement)mappingsNode);

            /* ------------------------------------------------------------------
             * Read configuration node 'Options\Option' and all child nodes
             * ------------------------------------------------------------------ */
            _options = new List<OptionInfo>();
            XmlNodeList optionNodeRefs = configNode.SelectNodes("Options/Option");

            if (optionNodeRefs != null)
            {
                foreach (XmlNode optionNodeRef in optionNodeRefs)
                {
                    string optionId = optionNodeRef.InnerText;
                    string xPath = string.Format("//Root/MachineOptions/Option[@id='{0}']", optionId);

                    XmlNode optionNode = doc.SelectSingleNode(xPath);

                    if (optionNode != null)
                    {
                        OptionInfo option = new OptionInfo((XmlElement)optionNode);
                        _options.Add(option);
                    }
                }
            }
        }


        /* ===============================================================================================
         * Helper method to iterate over all child nodes of Plc\PlcObjects in Orders.xml
         * =============================================================================================== */
        private void addPlcElements(ref List<PlcObjectInfo> list, XmlNodeList xmlElements)
        {
            if (xmlElements != null)
            {
                foreach (XmlElement element in xmlElements)
                {
                    PlcObjectType type = (PlcObjectType)Enum.Parse(typeof(PlcObjectType),element.Attributes["type"].Value);
                    PlcObjectInfo info = null;

                    switch (type)
                    {
                        case PlcObjectType.Library:
                            info = new LibraryInfo(element);
                            break;
                        case PlcObjectType.Placeholder:
                            info = new PlaceholderInfo(element);
                            break;
                        case PlcObjectType.DataType:
                            info = new DataTypeInfo(element);
                            break;
                        case PlcObjectType.POU:
                            info = new POUInfo(element);
                            break;
                        case PlcObjectType.Itf:
                            info = new ItfInfo(element);
                            break;
                        case PlcObjectType.Gvl:
                            info = new GvlInfo(element);
                            break;
                        default:
                            Debug.Fail("");
                            break;
                    }

                    if (info != null)
                        list.Add(info);
                }
            }
        }


        /* ===============================================================================================
         * Helper method to iterate over all child nodes of Motion\Task in Orders.xml
         * =============================================================================================== */
        private void addMotionTasks(ref List<TaskInfo> _motionTasks, XmlNodeList motionTasksNodeList)
        {
            foreach (XmlElement element in motionTasksNodeList)
            {
                TaskInfo taskInfo = new TaskInfo(element);
                _motionTasks.Add(taskInfo);
            }
        }

        #region ConfigurationInfoGetterSetter

        public string Id
        {
            get { return _id; }
        }
        
        public string Name
        {
            get { return _name; }
        }
        
        public string ProjectTemplate
        {
            get { return _projectTemplate; }
        }
        
        public string Script
        {
            get { return _script; }
        }
        
        public string Description
        {
            get { return _description; }
        }
           
        public List<OptionInfo> Options 
        {
            get { return _options; }
        }

        public List<PlcObjectInfo> PlcObjects
        {
            get { return _plcObjects; }
        }

        public string PlcProjectName
        {
            get { return _plcProjectName; }
        }

        public List<TaskInfo> MotionTasks
        {
            get { return _motionTasks; }
        }

        public HardwareInfo Hardware
        {
            get { return _hardware; }
        }

        public MappingsInfo Mappings
        {
            get { return _mappings; }
        }

        #endregion
    }

    #region HelperClassesToRepresentConfigurationFromOrdersXml

    /* ===============================================================================================
     * This abstract class represents a PlcObject from Orders.xml
     * =============================================================================================== */
    public abstract class PlcObjectInfo
    {
        protected PlcObjectInfo(XmlElement element)
        {
            this._type = (PlcObjectType)Enum.Parse(typeof(PlcObjectType),element.Attributes["type"].Value);
        }

        protected PlcObjectType _type;
	    
        public PlcObjectType Type
	    {
	      get { return _type;}
	    }
    }


    /* ===============================================================================================
     * This abstract class inherits from PlcObjectInfo
     * =============================================================================================== */
    public abstract class WorksheetInfo : PlcObjectInfo
    {
        protected WorksheetInfo(XmlElement element)
            : base(element)
        {
            this._plcPath = element.Attributes["path"].Value;
            this._templatePath = element.InnerText;
        }

        private string _plcPath;
        public string PlcPath
        {
            get { return _plcPath; }
        }

        private string _templatePath;
        public string TemplatePath
        {
            get { return _templatePath; }
        }
    }


    /* ===============================================================================================
     * This class represents a PlcObject of type "POU" in Orders.xml
     * =============================================================================================== */
    public class POUInfo : WorksheetInfo
    {
        public POUInfo(XmlElement element)
            : base(element)
        {
        }
    }


    /* ===============================================================================================
     * This class represents a PlcObject of type "Gvl" in Orders.xml
     * =============================================================================================== */
    public class GvlInfo : WorksheetInfo
    {
        public GvlInfo(XmlElement element)
            : base(element)
        {
        }

    }


    /* ===============================================================================================
     * This class represents a PlcObject of type "Itf" in Orders.xml
     * =============================================================================================== */
    public class ItfInfo : WorksheetInfo
    {
        public ItfInfo(XmlElement element)
            : base(element)
        {
        }
    }


    /* ===============================================================================================
     * This class represents a PlcObject of type "DataType" in Orders.xml
     * =============================================================================================== */
    public class DataTypeInfo : WorksheetInfo
    {
        public DataTypeInfo(XmlElement element)
            : base(element)
        {
        }

    }


    /* ===============================================================================================
     * This class represents a PlcObject of type "Library" in Orders.xml
     * =============================================================================================== */
    public class LibraryInfo : PlcObjectInfo
    {
        public LibraryInfo(XmlElement element)
            : base(element)
        {
            this._libraryName = element.InnerText;
        }

        private string _libraryName;
        public string LibraryName
        {
            get { return _libraryName; }
        }
    }


    /* ===============================================================================================
     * This class represents a PlcObject of type "Placeholder" in Orders.xml
     * =============================================================================================== */
    public class PlaceholderInfo : PlcObjectInfo
    {
        public PlaceholderInfo(XmlElement element)
            : base(element)
        {
            this._placeholderName = element.InnerText;
        }

        private string _placeholderName;
        public string PlaceholderName
        {
            get { return _placeholderName; }
        }
    }


    /* ===============================================================================================
     * This class represents <Option> in Orders.xml
     * =============================================================================================== */
    public class OptionInfo
    {
        public OptionInfo(XmlElement optionNode)
        {
            this._id = optionNode.Attributes["id"].Value;

            XmlNode descriptionNode = optionNode.SelectSingleNode("Description");
            this._description = descriptionNode.InnerText;
        }

        private string _description;
        public string Description
        {
            get { return _description; }
        }

        private string _id;
        public string Id
        {
            get { return _id; }
        }
    }


    /* ===============================================================================================
     * This class represents Motion <Task> in Orders.xml
     * =============================================================================================== */
    public class TaskInfo
    {
        public TaskInfo(XmlElement element)
        {
            _name = element.Attributes["name"].Value;

            XmlNodeList list = element.SelectNodes("Axes/Axis");
            _axes = new List<AxisInfo>();

            foreach (XmlElement node in list)
            {
                _axes.Add(new AxisInfo(node));
            }
        }

        private string _name;
        public string Name
        {
            get { return _name; }
        }

        private List<AxisInfo> _axes;
        public List<AxisInfo> Axes
        {
            get { return _axes; }
        }
        
    }


    /* ===============================================================================================
     * This class represents <Axis> in Orders.xml
     * =============================================================================================== */
    public class AxisInfo
    {
        public AxisInfo(XmlElement element)
        {
            this._name = element.Attributes["name"].Value;
            this._template = element.InnerText;
        }

        private string _name;
        public string  Name
        {
            get { return _name; }
        }

        private string _template;
        public string TemplatePath
        {
            get { return _template; }
        }
        
    }


    /* ===============================================================================================
     * This class represents <Mappings> in Orders.xml
     * =============================================================================================== */
    public class MappingsInfo
    {
        public MappingsInfo(XmlElement element)
        {
            this._template = element.InnerText;
        }

        private string _template;
        public string TemplatePath
        {
            get { return _template; }
        }
    }


    /* ===============================================================================================
     * This class represents <Hardware> in Orders.xml
     * =============================================================================================== */
    public class HardwareInfo
    {
        public HardwareInfo(XmlElement element)
        {
            XmlNodeList nodes = element.SelectNodes("Box");
            _boxes = new List<BoxInfo>();

            foreach (XmlElement boxNode in nodes)
            {
                _boxes.Add(new BoxInfo(boxNode));
            }
        }

        private List<BoxInfo> _boxes;
        public List<BoxInfo> Boxes
        {
            get { return _boxes; }
        }
        
    }


    /* ===============================================================================================
     * This class represents <Box> in Orders.xml
     * =============================================================================================== */
    public class BoxInfo
    {
        private string _name;
        private string _type;
        private List<BoxInfo> _childBoxes;

        public BoxInfo(XmlElement element)
        {
            this._name = element.Attributes["name"].Value;
            this._type = element.Attributes["type"].Value;

            XmlNodeList nodes = element.SelectNodes("Box");
            _childBoxes = new List<BoxInfo>();
            
            foreach (XmlElement boxNode in nodes)
            {
                _childBoxes.Add(new BoxInfo(boxNode));
            }
        }
                
        public string Name
        {
            get { return _name; }
        }

        public string Type
        {
            get { return _type; }
        }

        public List<BoxInfo> ChildBoxes
        {
            get { return _childBoxes; }
        }
        
    }

    public class OrderScriptContext : ScriptContext
    {
        private OrderInfo _order;

        public OrderScriptContext(IConfigurationFactory factory, OrderInfo order)
            : base(factory)
        {
            _order = order;
            projectTemplate = order.ConfigurationInfo.ProjectTemplate;

            foreach (OptionInfo o in order.ConfigurationInfo.Options)
            {
                parameters.Add(o.Id, o);
            }
        }
                
        public OrderInfo Order
        {
            get { return _order; }
        }
    }

    public class OrderCollection : List<OrderInfo>
    { }

    public class ConfigurationCollection : List<ConfigurationInfo>
    {
        public bool TryGetConfiguration(string id, out ConfigurationInfo info)
        {
            info = this.FirstOrDefault<ConfigurationInfo>(i => StringComparer.OrdinalIgnoreCase.Compare(i.Id, id) == 0);

            if (info != null)
                return true;
            else
                return false;
        }

        public bool Contains(string id)
        {
            ConfigurationInfo found = null;
            return TryGetConfiguration(id, out found);
        }
    }

    #endregion
}
