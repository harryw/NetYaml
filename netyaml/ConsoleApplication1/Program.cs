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
