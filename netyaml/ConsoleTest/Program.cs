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
			var doc = new YamlDocument();
			var xyMapping = doc.Root = new YamlMapping();
			var abSequence = xyMapping["x"] = new YamlSequence();
			abSequence.Sequence.Add(new YamlScalar("a"));
			abSequence.Sequence.Add(new YamlScalar("b"));
			var fiMapping = xyMapping["y"] = new YamlMapping();
			var ghSequence = fiMapping["f"] = new YamlSequence();
			ghSequence.Sequence.Add(new YamlScalar("g"));
			ghSequence.Sequence.Add(new YamlScalar("h"));
			fiMapping["i"] = new YamlScalar("jk");
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
