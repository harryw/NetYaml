using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace NetYaml.Interop
{
	[StructLayout(LayoutKind.Sequential)]
	internal unsafe struct YamlTagDirective
	{
		/** The tag handle. */
		YamlString* handle;
		/** The tag prefix. */
		YamlString* prefix;
	}
}
