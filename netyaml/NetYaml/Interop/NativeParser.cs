using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace NetYaml
{
	namespace Interop
	{
		internal unsafe static class NativeParser
		{
			private static IDictionary<IntPtr, YamlBuilder> parsers;

			static NativeParser()
			{
				parsers = new Dictionary<IntPtr, YamlBuilder>();
			}

			internal static void Parse(string text, out IList<YamlDocument> documents)
			{
				IntPtr pNativeParser;
				var builder = new YamlBuilder();
				if (1 != CreateParser(&pNativeParser))
				{
					throw new Exception("Failed to create a native parser");
				}
				try
				{
					parsers.Add(pNativeParser, builder);
					if (0 != ParseEvents(pNativeParser, text, text.Length, ParseYamlEvent))
					{
						throw new Exception("Error parsing YAML");
					}
					documents = parsers[pNativeParser].Documents;
					parsers.Remove(pNativeParser);
				}
				finally
				{
					DestroyParser(pNativeParser);
				}
			}

			private static int ParseYamlEvent(IntPtr pNativeParser, YamlEvent* pEvent)
			{
				int returnCode = 0;
				try
				{
					var builder = parsers[pNativeParser];
					switch (pEvent->type)
					{
						case YamlEventType.YAML_STREAM_START_EVENT:
							builder.StreamStart();
							break;
						case YamlEventType.YAML_STREAM_END_EVENT:
							builder.StreamEnd();
							break;
						case YamlEventType.YAML_DOCUMENT_START_EVENT:
							builder.DocumentStart();
							break;
						case YamlEventType.YAML_DOCUMENT_END_EVENT:
							builder.DocumentEnd();
							break;
						case YamlEventType.YAML_ALIAS_EVENT:
							builder.Alias(pEvent->data.alias.anchor);
							break;
						case YamlEventType.YAML_SCALAR_EVENT:
							builder.Scalar(pEvent->data.scalar.anchor, pEvent->data.scalar.tag, pEvent->data.scalar.value);
							break;
						case YamlEventType.YAML_SEQUENCE_START_EVENT:
							builder.SequenceStart(pEvent->data.scalar.anchor, pEvent->data.scalar.tag);
							break;
						case YamlEventType.YAML_SEQUENCE_END_EVENT:
							builder.SequenceEnd();
							break;
						case YamlEventType.YAML_MAPPING_START_EVENT:
							builder.MappingStart(pEvent->data.scalar.anchor, pEvent->data.scalar.tag);
							break;
						case YamlEventType.YAML_MAPPING_END_EVENT:
							builder.MappingEnd();
							break;
						default:
							break;
					}
				}
				catch
				{
					// Don't allow exceptions to get back to unmanaged caller.
					// Non-zero indicates an error.
					returnCode = 1;
				}
				return returnCode;
			}

			[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
			private unsafe delegate int YamlEventHandler(IntPtr pParser, YamlEvent * pEvent);

			[DllImport("NetYamlNative", EntryPoint = "parser_create", CallingConvention = CallingConvention.Cdecl)]
			private static extern int CreateParser(IntPtr* pParser);

			[DllImport("NetYamlNative", EntryPoint = "parser_destroy", CallingConvention = CallingConvention.Cdecl)]
			private static extern int DestroyParser(IntPtr pParser);

			[DllImport("NetYamlNative", EntryPoint = "?parser_parse@@YAHPAUyaml_parser_s@@PBEIP6AH0PAUyaml_event_s@@@Z@Z", CallingConvention = CallingConvention.Cdecl)]
			private static extern int ParseEvents(IntPtr pParser, string text, int textLength, YamlEventHandler handler);
		}
	}
}
