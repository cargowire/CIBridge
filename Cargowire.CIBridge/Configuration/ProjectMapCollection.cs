using System.Configuration;

namespace Cargowire.CIBridge.Configuration
{
    [ConfigurationCollection(typeof(ProjectMapElement), AddItemName ="project", CollectionType = ConfigurationElementCollectionType.BasicMap)]
    public class ProjectMapCollection : ConfigurationElementCollection<string, ProjectMapElement>
    {
        protected override string ElementName { get { return "project"; } }

        public override ConfigurationElementCollectionType CollectionType
        {
            get { return ConfigurationElementCollectionType.BasicMap; }
        }

		protected override object GetElementKey(ConfigurationElement element) {
            return ((ProjectMapElement)element).ProjectName;
        }
    }
}