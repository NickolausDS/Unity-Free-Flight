using UnityEngine;
using System.Collections;
	
	public static class Build {

		public static void writeVersion() {

			#if !UNITY_WEBPLAYER

			if (Application.isEditor) {
				string arg = getSingleArgument ();
				if (arg != null)
					System.IO.File.WriteAllText (arg, "v" + typeof(FreeFlight).Assembly.GetName ().Version.ToString ());
				else 
					throw new UnityException ("Missing path");
			}
			#endif
		}


		/// <summary>
		/// Get the first argument in the set of command line arguments after GetCommandLineArgs
		/// Useful because ALL command line arguments are passed, not just the ones for -executeMethod
		/// 
		/// This script is a brutish way to grab the first argument after the -executeMethod and <Script.Methodname>
		/// arguments
		/// </summary>
		/// <returns>The single argument.</returns>
		public static string getSingleArgument() {
			string retval = null;

			#if !UNITY_WEBPLAYER
			string[] args = System.Environment.GetCommandLineArgs ();
			bool executeMethodArg = false;
			bool methodArg = false;

			foreach (string arg in args) {
				if (arg == "-executemethod") {
					executeMethodArg = true;
					continue;
				}

				if (executeMethodArg && !methodArg) {
					methodArg = true;
					continue;
				}

				if (executeMethodArg && methodArg) {
					retval = arg;
					break;
				}

			}
			#endif
			return retval;
		}
	}

