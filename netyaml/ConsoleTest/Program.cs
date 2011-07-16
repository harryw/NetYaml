using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NetYaml;

namespace NetYaml.ConsoleTest
{
	class Program
	{
		static void Main(string[] args)
		{
			//var doc = ParseTest();
			//EmitTest(doc);
			//EmitTest(null);
			AliasTest();
			Console.ReadLine();
		}

		private static void AliasTest()
		{
			var repeatNode = new YScalar("dup");
			var doc = new YDocument(new YSequence(repeatNode, repeatNode));
			string yaml = Yaml.Dump(doc);
			Console.WriteLine(yaml);
		}

		static void EmitTest(YDocument doc)
		{
			doc = doc ?? new YDocument(
				new YMapping(new Dictionary<YScalar, YNode> {
						{"x", new YSequence(new YScalar("a"), new YScalar("b"))},
						{"y", new YMapping(new Dictionary<YScalar, YNode> {
							{"f", new YSequence(new YScalar("g"), new YScalar("h"))},
							{"i", new YScalar("jk")}
						})},
					})
				);
			string yaml = Yaml.Dump(doc);
			Console.WriteLine(yaml);
		}

		static YDocument ParseTest()
		{
			string yaml =
@"---
x: 
- a
- b
y:
  f: [g,h]
  i: jk";
			var docs = Yaml.Parse(yaml);
			var doc = docs.First();
			Console.WriteLine("Key 1: {0}", doc["x"][0]);
			Console.WriteLine("Key 2: {0}", doc["y"]["f"][0]);
			Console.WriteLine(docs);
			return doc;
		}
	}
}
