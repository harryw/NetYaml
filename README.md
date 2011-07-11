## NetYaml

NetYaml is a .Net library providing managed bindings for the LibYAML native YAML library.

## Usage

The API is intended to be unobtrusive and loosely-typed.  For example:

```C#
string yaml =
@"---
x: 
- a
- b
y:
  f: [g,h]
  i: jk";

var doc = Yaml.Parse(yaml).First();
Console.WriteLine(doc["y"]["f"][0]); // prints 'g'
```


## Copyright

Copyright (c) 2011 Harry Wilkinson. See LICENSE for details.

LibYAML is Copyright (c) 2006 Kirill Simonov.