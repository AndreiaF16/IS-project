using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Schema;

namespace ProjectXML
{
    class HandlerXML
    {
        public HandlerXML(string xmlFile)
        {
            XmlFilePath = xmlFile;
        }

        public HandlerXML(string xmlFile, string xsdFile)
        {
            XmlFilePath = xmlFile;
            XsdFilePath = xsdFile;
        }

        public string XmlFilePath { get; set; }
        public string XsdFilePath { get; set; }

        public List<string> GetAlertsId()
        {
            List<string> alerts = new List<string>();
            XmlDocument doc = new XmlDocument();
            doc.Load(XmlFilePath);

            XmlNodeList lista = doc.SelectNodes("/alerts/alert/id");
            foreach (XmlNode item in lista)
            {
                Alert alert = GetAlertById(item.InnerText);
                string apresentacao = "ID: " + alert.ID + "   Type: " + alert.Type + "   Operator: " + alert.Operator;
                alerts.Add(apresentacao);
            }
            return alerts;
        }
        public List<Alert> GetAlerts()
        {
            List<Alert> alerts = new List<Alert>();
            XmlDocument doc = new XmlDocument();
            doc.Load(XmlFilePath);

            XmlNodeList lista = doc.SelectNodes("/alerts/alert/id");
            foreach (XmlNode item in lista)
            {
                Alert alert = GetAlertById(item.InnerText);
                alerts.Add(alert);
            }
            return alerts;
        }

        public Alert GetAlertById(string id)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(XmlFilePath);
            if (doc.SelectSingleNode("/alerts/alert[id="+id+"]/id").InnerText.Equals(null)) {
                return null;
            }

            Alert alert = null;

            if (doc.SelectSingleNode("/alerts/alert[id=" + id + "]/value2").InnerText.Equals(""))
            {
                alert = new Alert
                {
                    ID = int.Parse(doc.SelectSingleNode("/alerts/alert[id=" + id + "]/id").InnerText.ToString()),
                    Type = doc.SelectSingleNode("/alerts/alert[id=" + id + "]/type").InnerText,
                    Operator = doc.SelectSingleNode("/alerts/alert[id=" + id + "]/operator").InnerText,
                    Value = decimal.Parse(doc.SelectSingleNode("/alerts/alert[id=" + id + "]/value").InnerText.ToString(), CultureInfo.InvariantCulture),
                    Enable = bool.Parse(doc.SelectSingleNode("/alerts/alert[id=" + id + "]/enable").InnerText.ToString()),
                    Description = doc.SelectSingleNode("/alerts/alert[id=" + id + "]/description").InnerText,
                };
            }
            else
            {
                alert = new Alert
                {
                    ID = int.Parse(doc.SelectSingleNode("/alerts/alert[id=" + id + "]/id").InnerText.ToString()),
                    Type = doc.SelectSingleNode("/alerts/alert[id=" + id + "]/type").InnerText,
                    Operator = doc.SelectSingleNode("/alerts/alert[id=" + id + "]/operator").InnerText,
                    Value = decimal.Parse(doc.SelectSingleNode("/alerts/alert[id=" + id + "]/value").InnerText.ToString(), CultureInfo.InvariantCulture),
                    Value2 = decimal.Parse(doc.SelectSingleNode("/alerts/alert[id=" + id + "]/value2").InnerText.ToString(), CultureInfo.InvariantCulture),
                    Enable = bool.Parse(doc.SelectSingleNode("/alerts/alert[id=" + id + "]/enable").InnerText.ToString()),
                    Description = doc.SelectSingleNode("/alerts/alert[id=" + id + "]/description").InnerText,
                };
            }
            
            return alert;
        }

        public List<Alert> GetActiveAlerts()
        {
            List<Alert> alerts = new List<Alert>();
            XmlDocument doc = new XmlDocument();
            doc.Load(XmlFilePath);

            XmlNodeList lista = doc.SelectNodes("/alerts/alert[enable='True']/id");
            foreach (XmlNode item in lista)
            {
                Alert alert = GetAlertById(item.InnerText);
                alerts.Add(alert);
            }
            return alerts;
        }

        public void AddAlert(Alert alert)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(XmlFilePath);
            XmlNode alerts = doc.GetElementsByTagName("alerts")[0];

            XmlNode alertNode = doc.CreateElement("alert");;
            alerts.AppendChild(alertNode);

            XmlNode idNode = doc.CreateElement("id");
            idNode.AppendChild(doc.CreateTextNode(alert.ID.ToString()));
            alertNode.AppendChild(idNode);

            XmlNode typeNode = doc.CreateElement("type");
            typeNode.AppendChild(doc.CreateTextNode(alert.Type));
            alertNode.AppendChild(typeNode);

            XmlNode operatorNode = doc.CreateElement("operator");
            operatorNode.AppendChild(doc.CreateTextNode(alert.Operator));
            alertNode.AppendChild(operatorNode);

            XmlNode valueNode = doc.CreateElement("value");
            valueNode.AppendChild(doc.CreateTextNode(alert.Value.ToString()));
            alertNode.AppendChild(valueNode);

            XmlNode value2Node = doc.CreateElement("value2");
            value2Node.AppendChild(doc.CreateTextNode(alert.Value2.ToString()));
            alertNode.AppendChild(value2Node);

            XmlNode enableNode = doc.CreateElement("enable");
            enableNode.AppendChild(doc.CreateTextNode(alert.Enable.ToString()));
            alertNode.AppendChild(enableNode);

            XmlNode descriptionNode = doc.CreateElement("description");
            descriptionNode.AppendChild(doc.CreateTextNode(alert.Description));
            alertNode.AppendChild(descriptionNode);
            doc.Save(XmlFilePath);
        }

        public void UpdateAlertEnableById(int id, bool enable)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(XmlFilePath);
            XmlNode elementEnable = doc.SelectSingleNode("/alerts/alert[id=" + id + "]/enable");
            if (elementEnable != null)
            {
                elementEnable.InnerText = enable.ToString();
                doc.Save(XmlFilePath);
            }
        }

        public void DeleteAlertById(int id)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(XmlFilePath);
            XmlNode element = doc.SelectSingleNode("/alerts/alert[id=" + id + "]");
            if (element != null)
            {
                doc.DocumentElement.RemoveChild(element);
                doc.Save(XmlFilePath);
            }
        }
    }
}
