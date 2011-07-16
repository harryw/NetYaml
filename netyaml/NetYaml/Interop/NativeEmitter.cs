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

			internal static unsafe string Dump(IList<YDocument> documents)
			{
				IntPtr pNativeEmitter;
				var writer = new YamlWriter();
				if (1 != CreateEmitter(&pNativeEmitter, WriteYamlContent))
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

			private static void GenerateEvents(IntPtr pNativeEmitter, IList<YDocument> documents)
			{
				GenerateEvent(pNativeEmitter, x => CreateEventStreamStart((YamlEvent*)x, YamlEncoding.YAML_ANY_ENCODING));
				foreach (var document in documents)
				{
					YamlVersionDirective *NO_VERSION_DIRECTIVE = null;
					YamlTagDirective *NO_TAG_DIRECTIVE = null;
					const int DOCUMENT_START_EXPLICIT = 0;
					GenerateEvent(pNativeEmitter, x => CreateEventDocumentStart((YamlEvent*)x, NO_VERSION_DIRECTIVE, NO_TAG_DIRECTIVE, NO_TAG_DIRECTIVE, DOCUMENT_START_EXPLICIT));
					var visited = new HashSet<YNode>();
					var duplicates= new HashSet<YNode>();
					FindDuplicateNodes(document, visited, duplicates);
					var aliases = GenerateAliases(duplicates);
					visited.Clear();
					GenerateNodeEvents(pNativeEmitter, document.Root, aliases, visited);
					const int DOCUMENT_END_IMPLICIT = 1;
					GenerateEvent(pNativeEmitter, x => CreateEventDocumentEnd((YamlEvent*)x, DOCUMENT_END_IMPLICIT));
				}
				GenerateEvent(pNativeEmitter, x => CreateEventStreamEnd((YamlEvent*)x));
			}

			private static IDictionary<YNode, string> GenerateAliases(HashSet<YNode> duplicateNodes)
			{
				var aliases = new Dictionary<YNode, string>();
				int count = 0;
				foreach (var node in duplicateNodes)
				{
					aliases.Add(node, string.Format("id{0}", ++count));
				}
				return aliases;
			}

			private static void FindDuplicateNodes(YNode node, HashSet<YNode> visited, HashSet<YNode> duplicates)
			{
				if (visited.Contains(node))
				{
					duplicates.Add(node);
				}
				else
				{
					visited.Add(node);
					foreach (var subNode in node.SubNodes)
					{
						FindDuplicateNodes(subNode, visited, duplicates);
					}
				}
			}

			private static void GenerateNodeEvents(IntPtr pNativeEmitter, YNode node, IDictionary<YNode, string> aliases, HashSet<YNode> visited)
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
					GenerateEvent(pNativeEmitter, x => CreateEventAlias((YamlEvent*)x, alias));
				}
				else
				{
					string anchor = isAnchor ? alias : null;
					var scalarNode = node as YScalar;
					var sequenceNode = node as YSequence;
					var mappingNode = node as YMapping;
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

			private static void GenerateSubTypeEvents(IntPtr pNativeEmitter, YScalar node, IDictionary<YNode, string> aliases, string anchor)
			{
				const int PLAIN_IMPLICIT = 1;
				const int QUOTED_IMPLICIT = 1;
				GenerateEvent(pNativeEmitter, x => CreateEventScalar((YamlEvent*)x, anchor, node.Tag.Value, node.Scalar, node.Scalar.Length, PLAIN_IMPLICIT, QUOTED_IMPLICIT, YamlScalarStyle.YAML_ANY_SCALAR_STYLE));
			}

			private static void GenerateSubTypeEvents(IntPtr pNativeEmitter, YSequence node, IDictionary<YNode, string> aliases, string anchor, HashSet<YNode> visited)
			{
				const int IMPLICIT = 1;
				GenerateEvent(pNativeEmitter, x => CreateEventSequenceStart((YamlEvent*)x, anchor, node.Tag.Value, IMPLICIT, YamlSequenceStyle.YAML_ANY_SEQUENCE_STYLE));
				foreach (var item in node.Sequence)
				{
					GenerateNodeEvents(pNativeEmitter, item, aliases, visited);
				}
				GenerateEvent(pNativeEmitter, x => CreateEventSequenceEnd((YamlEvent*)x));
			}

			private static void GenerateSubTypeEvents(IntPtr pNativeEmitter, YMapping node, IDictionary<YNode, string> aliases, string anchor, HashSet<YNode> visited)
			{
				const int IMPLICIT = 1;
				GenerateEvent(pNativeEmitter, x => CreateEventMappingStart((YamlEvent*)x, anchor, node.Tag.Value, IMPLICIT, YamlMappingStyle.YAML_ANY_MAPPING_STYLE));
				foreach (var pair in node.Mapping)
				{
					GenerateNodeEvents(pNativeEmitter, pair.Key, aliases, visited);
					GenerateNodeEvents(pNativeEmitter, pair.Value, aliases, visited);
				}
				GenerateEvent(pNativeEmitter, x => CreateEventMappingEnd((YamlEvent*)x));
			}

			private static unsafe void GenerateEvent(IntPtr pNativeEmitter, Func<IntPtr, int> eventInit)
			{
				var @event = new YamlEvent();
				// This method is unsafe, so the @event struct is already fixed on the stack.
				// However, you can't pass unsafe types into lambdas so we have to cast to an IntPtr.
				var pEvent = &@event;
				if (0 == eventInit((IntPtr)pEvent))
				{
					throw new Exception("Failed to initialize YAML serialization event");
				}
				try
				{
					Emit(pNativeEmitter, pEvent);
				}
				finally
				{
					DestroyEvent(pEvent);
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
			private static extern int CreateEmitter(
				IntPtr* pNativeEmitter,
				YamlWriteHandler writeHandler);

			[DllImport("NetYamlNative", EntryPoint = "emitter_destroy", CallingConvention = CallingConvention.Cdecl)]
			private static extern void DestroyEmitter(IntPtr pNativeEmitter);

			[DllImport("NetYamlNative", EntryPoint = "emitter_emit", CallingConvention = CallingConvention.Cdecl)]
			private static extern int Emit(
				IntPtr pNativeEmitter, 
				YamlEvent *pEvent);

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
