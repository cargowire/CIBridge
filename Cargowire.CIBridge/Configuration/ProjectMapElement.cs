using System.Configuration;

namespace Cargowire.CIBridge.Configuration
{
    public class ProjectMapElement : ConfigurationElement
    {
        [ConfigurationProperty("projectname", IsRequired=true, IsKey=true)]
        public string ProjectName {
			get { return base["projectname"] as string; }
			set { base["projectname"] = value; }
        }

        [ConfigurationProperty("buildname", IsRequired=true)]
        public string BuildName {
			get { return base["buildname"] as string; }
			set { base["buildname"] = value; }
        }
    }
}