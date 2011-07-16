using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace NetYaml
{
	public abstract class YNode
	{
		public YTag Tag { get; set; }
		internal abstract void Add(YNode child);
		internal virtual bool AllowsChildren { get { return true; } }
		internal abstract IEnumerable<YNode> SubNodes { get; }

		protected YNode()
			: this(null)
		{
		}
		protected YNode(YTag tag)
		{
			Tag = tag ?? new YTag(null);
		}

		public virtual string Scalar 
		{
			get { throw new Exception("This YAML node is not a scalar"); }
			set { throw new Exception("This YAML node is not a scalar"); }
		}
		public virtual IList<YNode> Sequence { get { throw new Exception("This YAML node is not a sequence"); } }
		public virtual IDictionary<YScalar, YNode> Mapping { get { throw new Exception("This YAML node is not a mapping"); } }

		public YNode this[int index]
		{
			get { return Sequence[index]; }
			set { Sequence[index] = value; }
		}

		public YNode this[string key]
		{
			get { return Mapping[key]; }
			set { Mapping[key] = value; }
		}

		public static implicit operator string (YNode node)
		{
			return node.Scalar;
		}
	}

	public sealed class YDocument : YNode
	{
		public YNode Root { get; set; }

		public YDocument()
		{
		}
		public YDocument(YNode child)
		{
			Root = child;
		}

		internal override void Add(YNode child)
		{
			Root = child;
		}
		internal override IEnumerable<YNode> SubNodes
		{
			get { yield return Root; }
		}

		public override string Scalar
		{
			get { return Root.Scalar; }
			set { Root.Scalar = value; }
		}
		public override IList<YNode> Sequence { get { return Root.Sequence; } }
		public override IDictionary<YScalar, YNode> Mapping { get { return Root.Mapping; } }
	}

	public sealed class YScalar : YNode
	{
		public override string Scalar { get; set; }

		public YScalar()
			: this(null, null)
		{
		}
		public YScalar(YTag tag)
			: this(tag, null)
		{
		}
		public YScalar(string value)
			: this(null, value)
		{
		}
		public YScalar(YTag tag, string value) 
			: base(tag)
		{
			Scalar = value;
		}

		internal override void Add(YNode child)
		{
			throw new Exception("Cannot add a child to a scalar node");
		}
		internal override bool AllowsChildren { get { return false; } }
		internal override IEnumerable<YNode> SubNodes
		{
			get { return Enumerable.Empty<YNode>(); }
		}

		public static implicit operator YScalar(string value)
		{
			return new YScalar(value);
		}

		public override bool Equals(object obj)
		{
			var other = obj as YScalar;
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

	public sealed class YSequence : YNode
	{
		private IList<YNode> sequence;
		public override IList<YNode> Sequence { get { return sequence; } }

		public YSequence(params YNode[] children)
			: this(null, children)
		{
		}
		public YSequence(YTag tag, params YNode[] children)
			: base(tag)
		{
			sequence = children.ToList();
		}

		internal override void Add(YNode child)
		{
			Sequence.Add(child);
		}
		internal override IEnumerable<YNode> SubNodes
		{
			get { return Sequence; }
		}
	}

	public sealed class YMapping : YNode
	{
		private IDictionary<YScalar, YNode> mapping;
		public override IDictionary<YScalar, YNode> Mapping { get { return mapping; } }

		private YScalar nextKey;

		public YMapping()
			: this(null, null)
		{
		}
		public YMapping(YTag tag)
			: this(tag, null)
		{
		}
		public YMapping(IDictionary<YScalar, YNode> dictionary)
			: this(null, dictionary)
		{
			mapping = dictionary;
		}
		public YMapping(YTag tag, IDictionary<YScalar, YNode> dictionary)
			: base(tag)
		{
			mapping = dictionary ?? new Dictionary<YScalar, YNode>();
		}

		internal override void Add(YNode child)
		{
			if (nextKey == null)
			{
				var scalar = child as YScalar;
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
		internal override IEnumerable<YNode> SubNodes
		{
			get { return Mapping.SelectMany(x => new List<YNode>{x.Key, x.Value}); }
		}
	}
}
