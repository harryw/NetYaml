using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetYaml.Interop
{
	public class YamlBuilder
	{
		public IList<YDocument> Documents { get; private set; }

		private Dictionary<string, YNode> anchors;
		private Stack<YNode> nodeStack;

		public YamlBuilder()
		{
			Documents = new List<YDocument>();
			nodeStack = new Stack<YNode>();
		}

		private void SetAnchor(string anchor, YNode node)
		{
			if (!string.IsNullOrEmpty(anchor))
				anchors[anchor] = node;
		}

		private YNode CurrentNode
		{
			get
			{
				return !nodeStack.Any() ? null : nodeStack.Peek();
			}
		}

		public void StreamStart()
		{
			//do nothing
		}

		public void StreamEnd()
		{
			if (CurrentNode != null && !(CurrentNode is YDocument))
				throw new Exception("Stream ended before document ended");
		}

		public void DocumentStart()
		{
			var doc = new YDocument();
			Documents.Add(doc);
			nodeStack.Push(doc);
			anchors = new Dictionary<string, YNode>();
		}

		public void DocumentEnd()
		{
			if (!(CurrentNode is YDocument))
				throw new Exception("Document ended unexpectedly");
			nodeStack.Pop();
		}

		public void Alias(string alias)
		{
			var node = anchors[alias];
			CurrentNode.Add(node);
		}

		public void Scalar(string anchor, string tag, string value)
		{
			var node = new YScalar(new YTag(tag), value);
			CurrentNode.Add(node);
			SetAnchor(anchor, node);
		}

		public void SequenceStart(string anchor, string tag)
		{
			var node = new YSequence(new YTag(tag));
			CurrentNode.Add(node);
			nodeStack.Push(node);
			SetAnchor(anchor, node);
		}

		public void SequenceEnd()
		{
			nodeStack.Pop();
		}

		public void MappingStart(string anchor, string tag)
		{
			var node = new YMapping(new YTag(tag));
			CurrentNode.Add(node);
			nodeStack.Push(node);
			SetAnchor(anchor, node);
		}

		public void MappingEnd()
		{
			nodeStack.Pop();
		}
	}
}
