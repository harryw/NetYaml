using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace NetYaml.Interop
{
	[StructLayout(LayoutKind.Sequential)]
	internal struct YamlVersionDirective
	{
		internal YamlVersionDirective(int majorVersion, int minorVersion)
		{
			major = majorVersion;
			minor = minorVersion;
		}

		/** The major version number. */
		internal int major;
		/** The minor version number. */
		internal int minor;
	}
}
