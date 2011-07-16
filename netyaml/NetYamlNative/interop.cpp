
#include "interop.h"

#ifndef FALSE
#	define FALSE 0
#endif
#ifndef TRUE
#	define TRUE 1
#endif

/* returns 1 on success, 0 on error */
NETYAMLNATIVE_DECLARE(int) parser_create(yaml_parser_t **appParser)
{
	int lSuccess = FALSE;
	*appParser = (yaml_parser_t*)malloc(sizeof(yaml_parser_t));
	if (NULL != *appParser)
	{
		lSuccess = yaml_parser_initialize(*appParser);
	}
	return lSuccess;
}

NETYAMLNATIVE_DECLARE(void) parser_destroy(yaml_parser_t *apParser)
{
	yaml_parser_delete(apParser);

	if (NULL != apParser)
	{
		free(apParser);
	}
}

NETYAMLNATIVE_DECLARE(int) parser_parse(
	yaml_parser_t *apParser, 
	const unsigned char *aText, 
	size_t aSize, 
	netyamlnative_event_handler_t afpEventHandler)
{
	yaml_parser_set_input_string(apParser, aText, aSize);

	int lError = FALSE;
	int lFinished = FALSE;
	yaml_event_t lEvent;
	while (!lFinished && !lError)
	{
		if (!yaml_parser_parse(apParser, &lEvent))
		{
			lError = -1;
			break;
		}
		lError = afpEventHandler(apParser, &lEvent);
		lFinished = (lEvent.type == YAML_STREAM_END_EVENT);
		yaml_event_delete(&lEvent);
	}
	return lError;
}

NETYAMLNATIVE_DECLARE(int) event_create_stream_start(
	yaml_event_t *apEvent, 
	yaml_encoding_t aEncoding)
{
	return yaml_stream_start_event_initialize(
		apEvent, 
		aEncoding);
}

NETYAMLNATIVE_DECLARE(int) event_create_stream_end(yaml_event_t *apEvent)
{
	return yaml_stream_end_event_initialize(apEvent);
}

NETYAMLNATIVE_DECLARE(int) event_create_document_start(
	yaml_event_t *apEvent, 
	yaml_version_directive_t *apVersionDirective,
	yaml_tag_directive_t *apTagDirectivesStart,
	yaml_tag_directive_t *apTagDirectivesEnd,
	int aImplicit)
{
	return yaml_document_start_event_initialize(
		apEvent, 
		apVersionDirective, 
		apTagDirectivesStart, 
		apTagDirectivesEnd, 
		aImplicit);
}

NETYAMLNATIVE_DECLARE(int) event_create_document_end(
	yaml_event_t *apEvent, 
	int aImplicit)
{
	return yaml_document_end_event_initialize(
		apEvent,
		aImplicit);
}

NETYAMLNATIVE_DECLARE(int) event_create_scalar(
	yaml_event_t *apEvent, 
	yaml_char_t *apAnchor, 
	yaml_char_t *apTag,
	yaml_char_t *apValue, 
	int aLength,
	int aPlainImplicit, 
	int aQuotedImplicit,
	yaml_scalar_style_t aStyle)
{
	return yaml_scalar_event_initialize(
		apEvent, 
		apAnchor, 
		apTag,
		apValue, 
		aLength,
		aPlainImplicit, 
		aQuotedImplicit,
		aStyle);
}


NETYAMLNATIVE_DECLARE(int) event_create_sequence_start(
	yaml_event_t *apEvent,
	yaml_char_t *apAnchor, 
	yaml_char_t *apTag, 
	int aImplicit,
	yaml_sequence_style_t aStyle)
{
	return yaml_sequence_start_event_initialize(
		apEvent,
		apAnchor, 
		apTag, 
		aImplicit,
		aStyle);
}

NETYAMLNATIVE_DECLARE(int) event_create_sequence_end(yaml_event_t *apEvent)
{
	return yaml_sequence_end_event_initialize(apEvent);
}

NETYAMLNATIVE_DECLARE(int) event_create_mapping_start(
	yaml_event_t *apEvent, 
	yaml_char_t *apAnchor, 
	yaml_char_t *apTag, 
	int aImplicit,
	yaml_mapping_style_t aStyle)
{
	return yaml_mapping_start_event_initialize(
		apEvent, 
		apAnchor, 
		apTag, 
		aImplicit,
		aStyle);
}

NETYAMLNATIVE_DECLARE(int) event_create_mapping_end(yaml_event_t *apEvent)
{
	return yaml_mapping_end_event_initialize(apEvent);
}

NETYAMLNATIVE_DECLARE(int) event_create_alias(
	yaml_event_t *apEvent, 
	yaml_char_t *aAnchor)
{
	return yaml_alias_event_initialize(
		apEvent, 
		aAnchor);
}

NETYAMLNATIVE_DECLARE(void) event_destroy(yaml_event_t *apEvent)
{
//	yaml_event_delete(apEvent);
}

NETYAMLNATIVE_DECLARE(int) emitter_create(
	yaml_emitter_t **appEmitter,
	yaml_write_handler_t afpWriteHandler)
{
	int lSuccess = FALSE;
	*appEmitter = (yaml_emitter_t*)malloc(sizeof(yaml_emitter_t));
	if (NULL != *appEmitter)
	{
		lSuccess = yaml_emitter_initialize(*appEmitter);
	}
	if (lSuccess)
	{
		yaml_emitter_set_output(*appEmitter, afpWriteHandler, *appEmitter);
	}
	return lSuccess;
}

NETYAMLNATIVE_DECLARE(void) emitter_destroy(yaml_emitter_t *apEmitter)
{
	yaml_emitter_flush(apEmitter);
	yaml_emitter_delete(apEmitter);

	if (NULL != apEmitter)
	{
		free(apEmitter);
	}
}

NETYAMLNATIVE_DECLARE(int) emitter_emit(
	yaml_emitter_t *apEmitter, 
	yaml_event_t *apEvent)
{
	return yaml_emitter_emit(apEmitter, apEvent);
}
