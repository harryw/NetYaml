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

NETYAMLNATIVE_DECLARE(int) parser_parse(yaml_parser_t *apParser, const unsigned char *text, size_t aSize, netyamlnative_event_handler_t *afpEventHandler);



#ifdef __cplusplus
}
#endif

#endif /* #ifndef NETYAMLNATIVE_INTEROP_H */

