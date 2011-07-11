using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace NetYaml
{
	public abstract class YamlNode
	{
		public virtual string Tag { get; set; }
		public abstract void Add(YamlNode child);
		public virtual bool AllowsChildren { get { return true; } }
	}

	public class YamlDocument : YamlNode
	{
		public YamlNode Root { get; set; }

		public override void Add(YamlNode child)
		{
			Root = child;
		}
	}

	public class YamlScalar : YamlNode
	{
		public string Value {get;set;}

		public override void Add(YamlNode child)
		{
			throw new Exception("Cannot add a child to a scalar node");
		}
		public override bool AllowsChildren { get { return false; } }
	}

	public class YamlSequence : YamlNode
	{
		public IList<YamlNode> Sequence { get; set; }

		public YamlSequence()
		{
			Sequence = new List<YamlNode>();
		}

		public override void Add(YamlNode child)
		{
			Sequence.Add(child);
		}
	}

	public class YamlMapping : YamlNode
	{
		public IDictionary<YamlScalar, YamlNode> Mapping { get; set; }

		private YamlScalar nextKey;

		public YamlMapping()
		{
			Mapping = new Dictionary<YamlScalar, YamlNode>();
			nextKey = null;
		}

		public override void Add(YamlNode child)
		{
			if (nextKey == null)
			{
				var scalar = child as YamlScalar;
				if (scalar == null)
				{
					throw new Exception("Mapping keys must be scalars");
				}
				nextKey = scalar;
			}
			else
			{
				Mapping.Add(nextKey, child);
				nextKey = null;
			}
		}
	}
}
