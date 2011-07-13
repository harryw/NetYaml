using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace NetYaml
{
	namespace Interop
	{
		internal unsafe static class NativeEmitter
		{
			private static IDictionary<IntPtr, YamlWriter> emitters;

			static NativeEmitter()
			{
				emitters = new Dictionary<IntPtr, YamlWriter>();
			}

			internal static string Dump(IList<YamlDocument> documents)
			{
				IntPtr pNativeEmitter;
				var writer = new YamlWriter();
				if (1 != CreateEmitter(&pNativeEmitter))
				{
					throw new Exception("Failed to create a native emitter");
				}
				try
				{
					emitters.Add(pNativeEmitter, writer);
					GenerateEvents(pNativeEmitter, documents); 
					emitters.Remove(pNativeEmitter);
				}
				finally
				{
					DestroyEmitter(pNativeEmitter);
				}
				return writer.Builder.ToString();
			}

			private static void GenerateEvents(IntPtr pNativeEmitter, IList<YamlDocument> documents)
			{
				GenerateEvent(pNativeEmitter, x => CreateEventStreamStart(&x, YamlEncoding.YAML_ANY_ENCODING));
				foreach (var document in documents)
				{
					YamlVersionDirective *NO_VERSION_DIRECTIVE = null;
					YamlTagDirective *NO_TAG_DIRECTIVE = null;
					const int DOCUMENT_START_EXPLICIT = 0;
					GenerateEvent(pNativeEmitter, x => CreateEventDocumentStart(&x, NO_VERSION_DIRECTIVE, NO_TAG_DIRECTIVE, NO_TAG_DIRECTIVE, DOCUMENT_START_EXPLICIT));
					var visited = new HashSet<YamlNode>();
					var duplicates= new HashSet<YamlNode>();
					FindDuplicateNodes(document, visited, duplicates);
					var aliases = GenerateAliases(duplicates);
					visited.Clear();
					GenerateNodeEvents(pNativeEmitter, document.Root, aliases, visited);
					const int DOCUMENT_END_IMPLICIT = 1;
					GenerateEvent(pNativeEmitter, x => CreateEventDocumentEnd(&x, DOCUMENT_END_IMPLICIT));
				}
				GenerateEvent(pNativeEmitter, x => CreateEventStreamEnd(&x));
			}

			private static IDictionary<YamlNode, string> GenerateAliases(HashSet<YamlNode> duplicateNodes)
			{
				var aliases = new Dictionary<YamlNode, string>();
				int count = 0;
				foreach (var node in duplicateNodes)
				{
					aliases.Add(node, string.Format("id{0}", ++count));
				}
				return aliases;
			}

			private static void FindDuplicateNodes(YamlNode node, HashSet<YamlNode> visited, HashSet<YamlNode> duplicates)
			{
				if (visited.Contains(node))
				{
					duplicates.Add(node);
				}
				else
				{
					visited.Add(node);
				}
				foreach (var subNode in node.SubNodes)
				{
					FindDuplicateNodes(subNode, visited, duplicates);
				}
			}

			private static void GenerateNodeEvents(IntPtr pNativeEmitter, YamlNode node, IDictionary<YamlNode, string> aliases, HashSet<YamlNode> visited)
			{
				string alias;
				bool hasAlias = aliases.TryGetValue(node, out alias);
				bool isAnchor = hasAlias && !visited.Contains(node);
				bool isAlias = hasAlias && !isAnchor;
				if (isAnchor)
				{
					visited.Add(node);
				}

				if (isAlias)
				{
					if (!visited.Contains(node))
					{
						GenerateEvent(pNativeEmitter, x => CreateEventAlias(&x, alias));
					}
				}
				else
				{
					string anchor = isAnchor ? alias : null;
					var scalarNode = node as YamlScalar;
					var sequenceNode = node as YamlSequence;
					var mappingNode = node as YamlMapping;
					if (scalarNode != null)
					{
						GenerateSubTypeEvents(pNativeEmitter, scalarNode, aliases, anchor);
					}
					else if (sequenceNode != null)
					{
						GenerateSubTypeEvents(pNativeEmitter, sequenceNode, aliases, anchor, visited);
					}
					else if (mappingNode != null)
					{
						GenerateSubTypeEvents(pNativeEmitter, mappingNode, aliases, anchor, visited);
					}
				}
			}

			private static void GenerateSubTypeEvents(IntPtr pNativeEmitter, YamlScalar node, IDictionary<YamlNode, string> aliases, string anchor)
			{
				const int PLAIN_IMPLICIT = 1;
				const int QUOTED_IMPLICIT = 1;
				GenerateEvent(pNativeEmitter, x => CreateEventScalar(&x, anchor, node.Tag, node.Scalar, node.Scalar.Length, PLAIN_IMPLICIT, QUOTED_IMPLICIT, YamlScalarStyle.YAML_ANY_SCALAR_STYLE));
			}

			private static void GenerateSubTypeEvents(IntPtr pNativeEmitter, YamlSequence node, IDictionary<YamlNode, string> aliases, string anchor, HashSet<YamlNode> visited)
			{
				const int IMPLICIT = 1;
				GenerateEvent(pNativeEmitter, x => CreateEventSequenceStart(&x, anchor, node.Tag, IMPLICIT, YamlSequenceStyle.YAML_ANY_SEQUENCE_STYLE));
				foreach (var item in node.Sequence)
				{
					GenerateNodeEvents(pNativeEmitter, item, aliases, visited);
				}
				GenerateEvent(pNativeEmitter, x => CreateEventSequenceEnd(&x));
			}

			private static void GenerateSubTypeEvents(IntPtr pNativeEmitter, YamlMapping node, IDictionary<YamlNode, string> aliases, string anchor, HashSet<YamlNode> visited)
			{
				const int IMPLICIT = 1;
				GenerateEvent(pNativeEmitter, x => CreateEventMappingStart(&x, anchor, node.Tag, IMPLICIT, YamlMappingStyle.YAML_ANY_MAPPING_STYLE));
				foreach (var pair in node.Mapping)
				{
					GenerateNodeEvents(pNativeEmitter, pair.Key, aliases, visited);
					GenerateNodeEvents(pNativeEmitter, pair.Value, aliases, visited);
				}
				GenerateEvent(pNativeEmitter, x => CreateEventMappingEnd(&x));
			}

			private static unsafe void GenerateEvent(IntPtr pNativeEmitter, Func<YamlEvent, int> eventInit)
			{
				var @event = new YamlEvent();
				// this method is unsafe, so the @event struct is already fixed on the stack
				if (0 == eventInit(@event))
				{
					throw new Exception("Failed to initialize YAML serialization event");
				}
				try
				{
					Emit(pNativeEmitter, &@event, WriteYamlContent);
				}
				finally
				{
					DestroyEvent(&@event);
				}
			}

			private static int WriteYamlContent(
				IntPtr pNativeEmitter,
				YamlString buffer,
				int size)
			{
				int returnCode = 1;
				try
				{
					string text = ((string)buffer).Substring(0, size);
					var writer = emitters[pNativeEmitter];
					writer.Write(text);
				}
				catch
				{
					// Don't allow exceptions to get back to unmanaged caller.
					// Zero indicates an error.
					returnCode = 0;
				}
				return returnCode;
			}

			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			private unsafe delegate int YamlWriteHandler(
				IntPtr pNativeEmitter, 
				YamlString buffer, 
				int size);

			[DllImport("NetYamlNative", EntryPoint = "emitter_create", CallingConvention = CallingConvention.Cdecl)]
			private static extern int CreateEmitter(IntPtr* pNativeEmitter);

			[DllImport("NetYamlNative", EntryPoint = "emitter_destroy", CallingConvention = CallingConvention.Cdecl)]
			private static extern void DestroyEmitter(IntPtr pNativeEmitter);

			[DllImport("NetYamlNative", EntryPoint = "emitter_emit", CallingConvention = CallingConvention.Cdecl)]
			private static extern int Emit(
				IntPtr pNativeEmitter, 
				YamlEvent *pEvent, 
				YamlWriteHandler writeHandler);

			[DllImport("NetYamlNative", EntryPoint = "event_create_stream_start", CallingConvention = CallingConvention.Cdecl)]
			private static extern int CreateEventStreamStart(
				YamlEvent *pEvent,
				YamlEncoding aEncoding);

			[DllImport("NetYamlNative", EntryPoint = "event_create_stream_end", CallingConvention = CallingConvention.Cdecl)]
			private static extern int CreateEventStreamEnd(YamlEvent* pEvent);

			[DllImport("NetYamlNative", EntryPoint = "event_create_document_start", CallingConvention = CallingConvention.Cdecl)]
			private static extern int CreateEventDocumentStart(
				YamlEvent *pEvent, 
				YamlVersionDirective *pVersionDirective,
				YamlTagDirective *pTagDirectivesStart,
				YamlTagDirective *pTagDirectivesEnd,
				int aImplicit);

			[DllImport("NetYamlNative", EntryPoint = "event_create_document_end", CallingConvention = CallingConvention.Cdecl)]
			private static extern int CreateEventDocumentEnd(
				YamlEvent *pEvent, 
				int aImplicit);

			[DllImport("NetYamlNative", EntryPoint = "event_create_scalar", CallingConvention = CallingConvention.Cdecl)]
			private static extern int CreateEventScalar(
				YamlEvent *pEvent, 
				YamlString anchor, 
				YamlString tag,
				YamlString value, 
				int aLength,
				int aPlainImplicit, 
				int aQuotedImplicit,
				YamlScalarStyle aStyle);

			[DllImport("NetYamlNative", EntryPoint = "event_create_sequence_start", CallingConvention = CallingConvention.Cdecl)]
			private static extern int CreateEventSequenceStart(
				YamlEvent *pEvent,
				YamlString anchor, 
				YamlString tag, 
				int aImplicit,
				YamlSequenceStyle aStyle);

			[DllImport("NetYamlNative", EntryPoint = "event_create_sequence_end", CallingConvention = CallingConvention.Cdecl)]
			private static extern int CreateEventSequenceEnd(YamlEvent* pEvent);

			[DllImport("NetYamlNative", EntryPoint = "event_create_mapping_start", CallingConvention = CallingConvention.Cdecl)]
			private static extern int CreateEventMappingStart(
				YamlEvent *pEvent, 
				YamlString anchor, 
				YamlString tag, 
				int aImplicit,
				YamlMappingStyle aStyle);

			[DllImport("NetYamlNative", EntryPoint = "event_create_mapping_end", CallingConvention = CallingConvention.Cdecl)]
			private static extern int CreateEventMappingEnd(YamlEvent* pEvent);

			[DllImport("NetYamlNative", EntryPoint = "event_create_alias", CallingConvention = CallingConvention.Cdecl)]
			private static extern int CreateEventAlias(YamlEvent* pEvent, YamlString alias);

			[DllImport("NetYamlNative", EntryPoint = "event_destroy", CallingConvention = CallingConvention.Cdecl)]
			private static extern void DestroyEvent(YamlEvent *pEvent);
		}
	}
}
