## NetYaml

NetYaml is a .Net library providing managed bindings for the LibYAML native YAML library.

## Usage

The API is intended to be unobtrusive and loosely-typed.  

Example Yaml.Parse usage:

```c#
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

Example Yaml.Dump usage:

```c#
var doc = new YDocument(
  new YMapping(new Dictionary<YScalar, YNode> {
    {"x", new YSequence(new YScalar("a"), new YScalar("b"))},
    {"y", new YMapping(new Dictionary<YScalar, YNode> {
      {"f", new YSequence(new YScalar("g"), new YScalar("h"))},
      {"i", new YScalar("jk")}
    })},
  })
);
string yaml = Yaml.Dump(doc);
Console.WriteLine(yaml); // prints the YAML from the above parse example
```

## Copyright

Copyright (c) 2011 Harry Wilkinson. See LICENSE for details.

LibYAML is Copyright (c) 2006 Kirill Simonov.