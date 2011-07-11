
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
	if (NULL != apParser)
	{
		free(apParser);
	}
}

NETYAMLNATIVE_DECLARE(int) parser_parse(yaml_parser_t *apParser, const unsigned char *aText, size_t aSize, netyamlnative_event_handler_t afpEventHandler)
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

