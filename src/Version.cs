

namespace UnityFreeFlight {

	/// <summary>
	/// Free Flight version is based on Semantic versioning.
	/// versions are constructed as MAJOR.MINOR.PATCH[-PRERELEASE]
	/// See semver.org for more information.
	/// </summary>
	/// <returns>The free flight version.</returns>
	public static class Version {
	
		/// <summary>
		/// Returns the Free Flight version using semantic versioning.
		/// </summary>
		public static string version() {
			System.Version ver = typeof(FreeFlight).Assembly.GetName().Version;
			string type = ver.Revision != 0 ? "-alpha" + ver.Revision : "";
			return string.Format ("v{0}.{1}.{2}{3}", ver.Major, ver.Minor, ver.Build, type);
		}

	}

}