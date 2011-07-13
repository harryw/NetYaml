#ifndef NETYAMLNATIVE_INTEROP_H
#define NETYAMLNATIVE_INTEROP_H

#include <yaml.h>
typedef int ( *netyamlnative_event_handler_t)(yaml_parser_t *apParser, yaml_event_t *apEvent);

#ifdef __cplusplus
extern "C" {
#endif


#ifdef _WIN32
#   define  NETYAMLNATIVE_DECLARE(type)  __declspec(dllexport) type
#else
#   define  NETYAMLNATIVE_DECLARE(type)  type
#endif


	NETYAMLNATIVE_DECLARE(int) parser_create(yaml_parser_t **appParser);

	NETYAMLNATIVE_DECLARE(void) parser_destroy(yaml_parser_t *apParser);

	NETYAMLNATIVE_DECLARE(int) parser_parse(
		yaml_parser_t *apParser, 
		const unsigned char *text, 
		size_t aSize, 
		netyamlnative_event_handler_t *afpEventHandler);

	NETYAMLNATIVE_DECLARE(int) event_create_stream_start(
		yaml_event_t *apEvent, 
		yaml_encoding_t aEncoding);

	NETYAMLNATIVE_DECLARE(int) event_create_stream_end(yaml_event_t *apEvent);

	NETYAMLNATIVE_DECLARE(int) event_create_document_start(
		yaml_event_t *apEvent, 
		yaml_version_directive_t *apVersionDirective,
		yaml_tag_directive_t *apTagDirectivesStart,
		yaml_tag_directive_t *apTagDirectivesEnd,
		int aImplicit);

	NETYAMLNATIVE_DECLARE(int) event_create_document_end(
		yaml_event_t *apEvent, 
		int aImplicit);

	NETYAMLNATIVE_DECLARE(int) event_create_scalar(
		yaml_event_t *apEvent, 
		yaml_char_t *apAnchor, 
		yaml_char_t *apTag,
		yaml_char_t *apValue, 
		int aLength,
		int aPlainImplicit, 
		int aQuotedImplicit,
		yaml_scalar_style_t aStyle);

	NETYAMLNATIVE_DECLARE(int) event_create_sequence_start(
		yaml_event_t *apEvent,
		yaml_char_t *apAnchor, 
		yaml_char_t *apTag, 
		int aImplicit,
		yaml_sequence_style_t aStyle);

	NETYAMLNATIVE_DECLARE(int) event_create_sequence_end(yaml_event_t *apEvent);

	NETYAMLNATIVE_DECLARE(int) event_create_mapping_start(
		yaml_event_t *apEvent, 
		yaml_char_t *apAnchor, 
		yaml_char_t *apTag, 
		int aImplicit,
		yaml_mapping_style_t aStyle);

	NETYAMLNATIVE_DECLARE(int) event_create_mapping_end(yaml_event_t *apEvent);

	NETYAMLNATIVE_DECLARE(int) event_create_alias(
		yaml_event_t *apEvent, 
		yaml_char_t *aAnchor);

	NETYAMLNATIVE_DECLARE(void) event_destroy(yaml_event_t *apEvent);

	NETYAMLNATIVE_DECLARE(int) emitter_create(yaml_emitter_t **appEmitter);

	NETYAMLNATIVE_DECLARE(void) emitter_destroy(yaml_emitter_t *apEmitter);

	NETYAMLNATIVE_DECLARE(int) emitter_emit(
		yaml_emitter_t *apEmitter, 
		yaml_event_t *apEvent, 
		yaml_write_handler_t afpWriteHandler);


#ifdef __cplusplus
}
#endif

#endif /* #ifndef NETYAMLNATIVE_INTEROP_H */

