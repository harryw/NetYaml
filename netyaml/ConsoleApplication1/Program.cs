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
x: 1
y: 2";
			var docs = Yaml.Parse(yaml);
			Console.WriteLine(docs);
			Console.ReadLine();
		}
	}
}
