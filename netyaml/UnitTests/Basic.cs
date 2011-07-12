using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NetYaml.UnitTests
{
	[TestClass]
	public class Basic
	{
		[TestMethod]
		public void TestMethod1()
		{
			// arrange
			string yaml =
@"---
- sequence item 1
- sequence item 2";

			// act
			var docs = Yaml.Parse(yaml);

			// assert
			Assert.AreEqual("", docs);
		}
	}
}
