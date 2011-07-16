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
			var doc = new YDocument(
				new YMapping(new Dictionary<YScalar, YNode> {
						{"x", new YSequence(new YScalar("a"), new YScalar("b"))},
						{"y", new YMapping(new Dictionary<YScalar, YNode> {
							{"f", new YSequence(new YScalar("g"), new YScalar("h"))},
							{"i", new YScalar("jk")}
						})}
					})
				);
			//var xyMapping = doc.Root = new YMapping();
			//var abSequence = xyMapping["x"] = new YSequence();
			//abSequence.Sequence.Add(new YScalar("a"));
			//abSequence.Sequence.Add(new YScalar("b"));
			//var fiMapping = xyMapping["y"] = new YMapping();
			//var ghSequence = fiMapping["f"] = new YSequence();
			//ghSequence.Sequence.Add(new YScalar("g"));
			//ghSequence.Sequence.Add(new YScalar("h"));
			//fiMapping["i"] = new YScalar("jk");
			string yaml = Yaml.Dump(doc);
			Console.WriteLine(yaml);
			Console.ReadLine();
		}

		static void EmitTest()
		{
		}

		static void ParseTest()
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
			Console.ReadLine();
		}
	}
}
