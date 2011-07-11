namespace NetYaml
{
	namespace Interop
	{
		internal enum YamlEventType
		{
			/** An empty event. */
			YAML_NO_EVENT,

			/** A STREAM-START event. */
			YAML_STREAM_START_EVENT,
			/** A STREAM-END event. */
			YAML_STREAM_END_EVENT,

			/** A DOCUMENT-START event. */
			YAML_DOCUMENT_START_EVENT,
			/** A DOCUMENT-END event. */
			YAML_DOCUMENT_END_EVENT,

			/** An ALIAS event. */
			YAML_ALIAS_EVENT,
			/** A SCALAR event. */
			YAML_SCALAR_EVENT,

			/** A SEQUENCE-START event. */
			YAML_SEQUENCE_START_EVENT,
			/** A SEQUENCE-END event. */
			YAML_SEQUENCE_END_EVENT,

			/** A MAPPING-START event. */
			YAML_MAPPING_START_EVENT,
			/** A MAPPING-END event. */
			YAML_MAPPING_END_EVENT
		}

		internal enum YamlEncoding
		{
			/** Let the parser choose the encoding. */
			YAML_ANY_ENCODING,
			/** The default UTF-8 encoding. */
			YAML_UTF8_ENCODING,
			/** The UTF-16-LE encoding with BOM. */
			YAML_UTF16LE_ENCODING,
			/** The UTF-16-BE encoding with BOM. */
			YAML_UTF16BE_ENCODING
		}

		internal enum YamlScalarStyle
		{
			/** Let the emitter choose the style. */
			YAML_ANY_SCALAR_STYLE,

			/** The plain scalar style. */
			YAML_PLAIN_SCALAR_STYLE,

			/** The single-quoted scalar style. */
			YAML_SINGLE_QUOTED_SCALAR_STYLE,
			/** The double-quoted scalar style. */
			YAML_DOUBLE_QUOTED_SCALAR_STYLE,

			/** The literal scalar style. */
			YAML_LITERAL_SCALAR_STYLE,
			/** The folded scalar style. */
			YAML_FOLDED_SCALAR_STYLE
		}

		internal enum YamlSequenceStyle
		{
			/** Let the emitter choose the style. */
			YAML_ANY_SEQUENCE_STYLE,

			/** The block sequence style. */
			YAML_BLOCK_SEQUENCE_STYLE,
			/** The flow sequence style. */
			YAML_FLOW_SEQUENCE_STYLE
		}

		internal enum YamlMappingStyle
		{
			/** Let the emitter choose the style. */
			YAML_ANY_MAPPING_STYLE,

			/** The block mapping style. */
			YAML_BLOCK_MAPPING_STYLE,
			/** The flow mapping style. */
			YAML_FLOW_MAPPING_STYLE
		}
	}
}