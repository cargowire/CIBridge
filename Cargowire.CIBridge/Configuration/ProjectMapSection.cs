using System.Configuration;

namespace Cargowire.CIBridge.Configuration
{
    public sealed class ProjectMapSection : ConfigurationSection
    {
		private ProjectMapSection() { }

		private static ProjectMapSection _instance = null;
        /// <summary>Implemented as singleton</summary>
		public static ProjectMapSection Instance
        {
            get {
                if (_instance == null)
					_instance = ConfigurationManager.GetSection("ProjectMapping") as ProjectMapSection;

                return _instance;
            }
        }

        [ConfigurationProperty("projects", IsDefaultCollection=true)]
        public ProjectMapCollection Profiles {
			get { return base["projects"] as ProjectMapCollection; }
			set { base["projects"] = value; }
        }

        public string GetBuildName(string key)
        {
        	string buildName = key;
            ProjectMapElement cpe = Profiles[key];
			if (cpe != null)
				buildName = cpe.BuildName;

            return buildName;
        }

    }
}