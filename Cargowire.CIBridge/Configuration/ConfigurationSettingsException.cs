using System;
using System.Runtime.Serialization;

namespace Cargowire.CIBridge.Configuration
{
    [Serializable()]
    public class ConfigurationSettingsException : Exception
    {
        public ConfigurationSettingsException() { }
        public ConfigurationSettingsException(string message) : base(message) { }
        public ConfigurationSettingsException(string message, Exception ex) : base(message, ex) { }
        protected ConfigurationSettingsException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}