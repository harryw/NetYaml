using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using NetYaml.Interop;

namespace NetYaml
{
	public static class Yaml
	{
		public static IList<YamlDocument> Parse(string yaml)
		{
			IList<YamlDocument> docs;
			NativeParser.Parse(yaml, out docs);
			return docs;
		}

		public static string Dump(IList<YamlDocument> docs)
		{
			return NativeEmitter.Dump(docs);
		}

		public static string Dump(YamlDocument doc)
		{
			return Dump(new List<YamlDocument> { doc });
		}
	}
}
