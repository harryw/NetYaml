using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetYaml.Interop
{
	public class YamlWriter
	{
		public StringBuilder Builder { get; private set; }

		public YamlWriter()
		{
			Builder = new StringBuilder();
		}

		public void Write(string text)
		{
			Builder.Append(text);
		}
	}
}
