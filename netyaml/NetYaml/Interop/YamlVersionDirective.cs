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
		/** The major version number. */
		int major;
		/** The minor version number. */
		int minor;
	}
}
