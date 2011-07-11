using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace NetYaml.Interop
{
	[StructLayout(LayoutKind.Sequential)]
	internal unsafe struct YamlTagDirectives
	{
		/** The beginning of the tag directives list. */
		YamlTagDirective * start;
		/** The end of the tag directives list. */
		YamlTagDirective * end;
	}
}
