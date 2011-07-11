using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace NetYaml.Interop
{
	[StructLayout(LayoutKind.Sequential)]
	internal struct YamlString
	{
		IntPtr value;

		public static implicit operator string(YamlString yamlString)
		{
			return Marshal.PtrToStringAnsi(yamlString.value);
		}
	}
}
