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
		public static IList<YDocument> Parse(string yaml)
		{
			IList<YDocument> docs;
			NativeParser.Parse(yaml, out docs);
			return docs;
		}

		public static string Dump(IList<YDocument> docs)
		{
			return NativeEmitter.Dump(docs);
		}

		public static string Dump(YDocument doc)
		{
			return Dump(new List<YDocument> { doc });
		}
	}
}
