using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace NetYaml.Interop
{
	[StructLayout(LayoutKind.Sequential)]
	internal unsafe struct YamlEvent
	{
		/** The event type. */
		internal YamlEventType type;

		[StructLayout(LayoutKind.Explicit)]
		internal struct YamlEventDataUnion
		{
			/** The stream parameters (for @c YAML_STREAM_START_EVENT). */
			[StructLayout(LayoutKind.Sequential)]
			internal struct StreamStart
			{
				/** The document encoding. */
				internal YamlEncoding encoding;
			}
			[FieldOffset(0)]
			internal StreamStart stream_start;

			/** The document parameters (for @c YAML_DOCUMENT_START_EVENT). */
			[StructLayout(LayoutKind.Sequential)]
			internal struct DocumentStart
			{
				/** The version directive. */
				internal YamlVersionDirective* version_directive;

				/** The list of tag directives. */
				internal YamlTagDirectives tag_directives;

				/** Is the document indicator implicit? */
				internal int is_implicit;
			}
			[FieldOffset(0)]
			internal DocumentStart document_start;

			/** The document end parameters (for @c YAML_DOCUMENT_END_EVENT). */
			[StructLayout(LayoutKind.Sequential)]
			internal struct DocumentEnd
			{
				/** Is the document end indicator implicit? */
				internal int is_implicit;
			}
			[FieldOffset(0)]
			internal DocumentEnd document_end;

			/** The alias parameters (for @c YAML_ALIAS_EVENT). */
			[StructLayout(LayoutKind.Sequential)]
			internal struct Alias
			{
				/** The anchor. */
				internal YamlString anchor;
			}
			[FieldOffset(0)]
			internal Alias alias;

			/** The scalar parameters (for @c YAML_SCALAR_EVENT). */
			[StructLayout(LayoutKind.Sequential)]
			internal struct Scalar
			{
				/** The anchor. */
				internal YamlString anchor;
				/** The tag. */
				internal YamlString tag;
				/** The scalar value. */
				internal YamlString value;
				/** The length of the scalar value. */
				internal Size length;
				/** Is the tag optional for the plain style? */
				internal int plain_implicit;
				/** Is the tag optional for any non-plain style? */
				internal int quoted_implicit;
				/** The scalar style. */
				internal YamlScalarStyle style;
			}
			[FieldOffset(0)]
			internal Scalar scalar;

			/** The sequence parameters (for @c YAML_SEQUENCE_START_EVENT). */
			[StructLayout(LayoutKind.Sequential)]
			internal struct SequenceStart
			{
				/** The anchor. */
				internal YamlString anchor;
				/** The tag. */
				internal YamlString tag;
				/** Is the tag optional? */
				internal int is_implicit;
				/** The sequence style. */
				internal YamlSequenceStyle style;
			}
			[FieldOffset(0)]
			internal SequenceStart sequence_start;

			/** The mapping parameters (for @c YAML_MAPPING_START_EVENT). */
			[StructLayout(LayoutKind.Sequential)]
			internal struct MappingStart
			{
				/** The anchor. */
				internal YamlString anchor;
				/** The tag. */
				internal YamlString tag;
				/** Is the tag optional? */
				internal int is_implicit;
				/** The mapping style. */
				internal YamlMappingStyle style;
			}
			[FieldOffset(0)]
			internal MappingStart mapping_start;
		}
		internal YamlEventDataUnion data;

		[StructLayout(LayoutKind.Sequential)]
		internal struct YamlMark
		{
			/** The position index. */
			internal int index;

			/** The position line. */
			internal int line;

			/** The position column. */
			internal int column;
		}

		/** The beginning of the event. */
		internal YamlMark start_mark;
		/** The end of the event. */
		internal YamlMark end_mark;
	}
}
