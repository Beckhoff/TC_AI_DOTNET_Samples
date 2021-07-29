using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.ComponentModel;

namespace CodeGenerationDemo
{
    /* ===============================================================================================
     * This class reads machine configurations from Orders.xml and creates a new object representation of
     * this configuration via classes ConfigurationInfo and OrderInfo from ScriptInfo.cs.
     * =============================================================================================== */
    public class DataModel : INotifyPropertyChanged
    {
        public DataModel() {}

        public void Load()
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(@".\Orders.xml");

            XmlNodeList configurationNodes = doc.SelectNodes("Root/AvailableConfigurations/Configuration");

            foreach (XmlElement configurationNode in configurationNodes)
            {
                _configurations.Add(new ConfigurationInfo(configurationNode));
            }

            XmlNodeList orderNodes = doc.SelectNodes("Root/MachineOrders/Order");

            foreach (XmlElement orderNode in orderNodes)
            {
                _orders.Add(new OrderInfo(orderNode));
            }

            if (PropertyChanged != null)
            {
                OnPropertyChanged(new PropertyChangedEventArgs("Orders"));
            }
        }

        OrderCollection _orders = new OrderCollection();

        public OrderCollection Orders
        {
            get { return _orders; }
        }
        
        ConfigurationCollection _configurations = new ConfigurationCollection();

        public ConfigurationCollection Configurations
        {
            get { return _configurations; }
        }


        #region INotifyPropertyChanged Members

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion


        private void OnPropertyChanged(PropertyChangedEventArgs args)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, args);
            }
        }
    }
}
