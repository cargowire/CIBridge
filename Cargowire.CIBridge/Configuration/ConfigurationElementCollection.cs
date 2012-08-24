using System.Configuration;

namespace Cargowire.CIBridge.Configuration
{
    public abstract class ConfigurationElementCollection<K, V> : ConfigurationElementCollection where V : ConfigurationElement, new()
    {
        public V this[K index] { get { return BaseGet(index) as V; } }
        public V this[int index] { get { return BaseGet(index) as V; } }

        public override abstract ConfigurationElementCollectionType CollectionType{get;}
        protected override abstract string ElementName { get; }

        protected override System.Configuration.ConfigurationElement CreateNewElement(){
            return new V();
        }

        public void Add(V path){
            BaseAdd(path);
        }

        public void Remove(K key){
            BaseRemove(key);
        }

        public void Clear(){
            BaseClear();
        }
    }
}
