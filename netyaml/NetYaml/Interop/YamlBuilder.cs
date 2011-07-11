using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetYaml.Interop
{
	public class YamlBuilder
	{
		public IList<YamlDocument> Documents { get; private set; }

		private Dictionary<string, YamlNode> anchors;
		private Stack<YamlNode> nodeStack;

		public YamlBuilder()
		{
			Documents = new List<YamlDocument>();
			nodeStack = new Stack<YamlNode>();
		}

		private void SetAnchor(string anchor, YamlNode node)
		{
			if (!string.IsNullOrEmpty(anchor))
				anchors[anchor] = node;
		}

		private YamlNode CurrentNode
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
			if (CurrentNode != null && !(CurrentNode is YamlDocument))
				throw new Exception("Stream ended before document ended");
		}

		public void DocumentStart()
		{
			var doc = new YamlDocument();
			Documents.Add(doc);
			nodeStack.Push(doc);
			anchors = new Dictionary<string, YamlNode>();
		}

		public void DocumentEnd()
		{
			if (!(CurrentNode is YamlDocument))
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
			var node = new YamlScalar();
			node.Tag = tag;
			node.Value = value;
			CurrentNode.Add(node);
			SetAnchor(anchor, node);
		}

		public void SequenceStart(string anchor, string tag)
		{
			var node = new YamlSequence();
			node.Tag = tag;
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
			var node = new YamlMapping();
			node.Tag = tag;
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
