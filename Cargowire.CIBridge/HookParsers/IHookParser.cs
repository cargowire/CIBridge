using System.Collections.Generic;
using System.Collections.Specialized;

namespace Cargowire.CIBridge
{
	public interface IHookParser
	{
		/// <summary>Translates an http get/post from the Source control solution into a generic format
		/// for use with a build engine</summary>
		IEnumerable<HookInfo> Parse(NameValueCollection requestData);
	}
}