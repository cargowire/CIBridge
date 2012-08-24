using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net.Configuration;
using System.Reflection;
using System.Xml;

namespace Cargowire.CIBridge.Configuration
{
    public static class ConfigurationSettings
    {
        private static Dictionary<string, object> configSettings = new Dictionary<string, object>();

        private static string GetConfigFilePath(Type typeInSourceAssembly)
        {
            return string.Format("{0}.config", Assembly.GetAssembly(typeInSourceAssembly).Location);
        }

        private static XmlDocument LoadConfigDocument(Type typeInSourceAssembly)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(GetConfigFilePath(typeInSourceAssembly));
            return doc;
        }

        public static ConnectionStringSettings GetConnectionStringSettings(string connectionStringName)
        {
            return ConfigurationManager.ConnectionStrings[connectionStringName];
        }

        public static SmtpSection GetMailSettings()
        {
        	return ConfigurationManager.GetSection("system.net/mailSettings/smtp") as SmtpSection;
        }

        public static T GetSetting<T>(string settingName) { return GetSetting<T>(settingName, true); }
        public static T GetSetting<T>(string settingName, bool isRequired) { return GetSetting<T>(null, settingName, isRequired); }
        public static T GetSetting<T>(string settingName, bool isRequired, T defaultValue) { return GetSetting<T>(null, settingName, isRequired, default(T)); }
        public static T GetSetting<T>(Type typeInSourceAssembly, string settingName, bool isRequired) { return GetSetting<T>(typeInSourceAssembly, settingName, isRequired, default(T)); }
        public static T GetSetting<T>(Type typeInSourceAssembly, string settingName, bool isRequired, T defaultValue)
        {
            T settingValue = default(T);

            if (configSettings.ContainsKey(settingName))
                settingValue = (T)configSettings[settingName];
            else
            {
                string settingStringValue;

                try
                {
                    settingStringValue = GetStringValueFromSetting(settingName, typeInSourceAssembly);
                }
                catch (ConfigurationErrorsException ex)
                {
                    throw new ConfigurationSettingsException(string.Format("Unable to read the setting '{0}' from the configuration file", settingName), ex);
                }

                if (settingStringValue == null && isRequired)
                    throw new ConfigurationSettingsException(string.Format("The setting '{0}' is missing from the configuration file", settingName));
                else if (settingStringValue != null)
                {
                    try
                    {
                        if (typeof(T).IsSubclassOf(typeof(Enum)))
                            settingValue = (T)Enum.Parse(typeof(T), settingStringValue);
                        else
                            settingValue = (T)Convert.ChangeType(settingStringValue, typeof(T));
                    }
                    catch (FormatException ex)
                    {
                        throw new ConfigurationSettingsException(string.Format("The configuration file setting '{0}' is in an invalid format", settingName), ex);
                    }
                    catch (InvalidCastException ex)
                    {
                        throw new ConfigurationSettingsException(string.Format("The configuration file setting '{0}' is in an invalid format", settingName), ex);
                    }

                    // There's a slight chance of a race condition here. Catch and ignore the resulting exception
                    try
                    {
                        configSettings.Add(settingName, settingValue);
                    }
                    catch (ArgumentException) { }
                }
            }
            return settingValue;
        }

        /// <summary></summary>
        /// <param name="settingName"></param>
        /// <param name="typeInSourceAssembly"></param>
        /// <returns></returns>
        private static string GetStringValueFromSetting(string settingName, Type typeInSourceAssembly)
        {
            if (typeInSourceAssembly == null)
                return ConfigurationManager.AppSettings[settingName];
            else
            {
                // Load config document for current assembly
                XmlDocument doc = LoadConfigDocument(typeInSourceAssembly);

                // Retrieve appSettings node
                XmlNode node = doc.SelectSingleNode("//appSettings");

                if (node == null)
                    throw new InvalidOperationException("appSettings section not found in config file.");

                // Select the 'add' element that contains the key
                XmlElement elem = (XmlElement)node.SelectSingleNode(string.Format("//add[@key='{0}']", settingName));

                if (elem != null) return elem.GetAttribute("value");
            }
            // Couldn't find the setting
            return null;
        }

        public static void WriteSetting<T>(Type typeInSourceAssembly, string settingName, T value)
        {
            // Load config document for current assembly
            XmlDocument doc = LoadConfigDocument(typeInSourceAssembly);

            // Retrieve appSettings node
            XmlNode node = doc.SelectSingleNode("//appSettings");

            if (node == null) throw new InvalidOperationException("appSettings section not found in config file.");

            // Select the 'add' element that contains the key
            XmlElement elem = (XmlElement)node.SelectSingleNode(string.Format("//add[@key='{0}']", settingName));

            if (elem != null)
            {
                // Add value for key
                elem.SetAttribute("value", value == null ? string.Empty : value.ToString());
            }
            else
            {
                // Key was not found so create the 'add' element and set it's key/value attributes 
                elem = doc.CreateElement("add");
                elem.SetAttribute("key", settingName);
                elem.SetAttribute("value", value == null ? string.Empty : value.ToString());
                node.AppendChild(elem);
            }
            doc.Save(GetConfigFilePath(typeInSourceAssembly));
        }

        public static void RemoveSetting(Type typeInSourceAssembly, string settingName)
        {
            // Load config document for current assembly and retrieve appSettings node
            XmlDocument doc = LoadConfigDocument(typeInSourceAssembly);
            XmlNode node = doc.SelectSingleNode("//appSettings");

            try
            {
                if (node == null)
                    throw new InvalidOperationException("appSettings section not found in config file.");
                else
                {
                    // Remove 'add' element with corresponding key
                    node.RemoveChild(node.SelectSingleNode(string.Format("//add[@key='{0}']", settingName)));
                    doc.Save(GetConfigFilePath(typeInSourceAssembly));
                }
            }
            catch (NullReferenceException e)
            {
                throw new Exception(string.Format("The key {0} does not exist.", settingName), e);
            }
        }
	}
}