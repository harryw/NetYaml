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
		internal abstract void Add(YamlNode child);
		internal virtual bool AllowsChildren { get { return true; } }
		internal abstract IEnumerable<YamlNode> SubNodes { get; }

		protected YamlNode()
		{
		}
		protected YamlNode(string tag)
		{
			Tag = tag;
		}

		public virtual string Scalar 
		{
			get { throw new Exception("This YAML node is not a scalar"); }
			set { throw new Exception("This YAML node is not a scalar"); }
		}
		public virtual IList<YamlNode> Sequence { get { throw new Exception("This YAML node is not a sequence"); } }
		public virtual IDictionary<YamlScalar, YamlNode> Mapping { get { throw new Exception("This YAML node is not a mapping"); } }

		public YamlNode this[int index]
		{
			get { return Sequence[index]; }
			set { Sequence[index] = value; }
		}

		public YamlNode this[string key]
		{
			get { return Mapping[key]; }
			set { Mapping[key] = value; }
		}

		public static implicit operator string (YamlNode node)
		{
			return node.Scalar;
		}
	}

	public sealed class YamlDocument : YamlNode
	{
		public YamlNode Root { get; set; }

		internal override void Add(YamlNode child)
		{
			Root = child;
		}
		internal override IEnumerable<YamlNode> SubNodes
		{
			get { yield return Root; }
		}

		public override string Scalar
		{
			get { return Root.Scalar; }
			set { Root.Scalar = value; }
		}
		public override IList<YamlNode> Sequence { get { return Root.Sequence; } }
		public override IDictionary<YamlScalar, YamlNode> Mapping { get { return Root.Mapping; } }
	}

	public sealed class YamlScalar : YamlNode
	{
		public override string Scalar { get; set; }

		public YamlScalar(string value) : this(value, null)
		{
		}
		public YamlScalar(string value, string tag) : base(tag)
		{
			Scalar = value;
		}

		internal override void Add(YamlNode child)
		{
			throw new Exception("Cannot add a child to a scalar node");
		}
		internal override bool AllowsChildren { get { return false; } }
		internal override IEnumerable<YamlNode> SubNodes
		{
			get { return Enumerable.Empty<YamlNode>(); }
		}

		public static implicit operator YamlScalar(string value)
		{
			return new YamlScalar(value);
		}

		public override bool Equals(object obj)
		{
			var other = obj as YamlScalar;
			return other != null && string.Equals(Scalar, other.Scalar);
		}

		public override int GetHashCode()
		{
			return Scalar.GetHashCode();
		}

		public override string ToString()
		{
			return Scalar;
		}
	}

	public sealed class YamlSequence : YamlNode
	{
		private IList<YamlNode> sequence;
		public override IList<YamlNode> Sequence { get { return sequence; } }

		public YamlSequence() : this(null)
		{
		}
		public YamlSequence(string tag) : base(tag)
		{
			sequence = new List<YamlNode>();
		}

		internal override void Add(YamlNode child)
		{
			Sequence.Add(child);
		}
		internal override IEnumerable<YamlNode> SubNodes
		{
			get { return Sequence; }
		}
	}

	public sealed class YamlMapping : YamlNode
	{
		private IDictionary<YamlScalar, YamlNode> mapping;
		public override IDictionary<YamlScalar, YamlNode> Mapping { get { return mapping; } }

		private YamlScalar nextKey;


		public YamlMapping() : this(null)
		{
		}
		public YamlMapping(string tag) : base(tag)
		{
			mapping = new Dictionary<YamlScalar, YamlNode>();
			nextKey = null;
		}

		internal override void Add(YamlNode child)
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
		internal override IEnumerable<YamlNode> SubNodes
		{
			get { return Mapping.SelectMany(x => new List<YamlNode>{x.Key, x.Value}); }
		}
	}
}
