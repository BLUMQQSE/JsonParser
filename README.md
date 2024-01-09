# JsonParser
Class for converting to and from a jsonstring and a data structure to contain the json data. Allows for storing all json info into one data structure.

Example:

```
public void Main(String[] args)
{
  string dataStr = @"{"name": "Billy", "age":42, "hobbies":["Golfing", "Swimming", "Hiking"]}";
  JsonValue value = new JsonValue();
  value.Parse(dataStr);
  value["employed"].Set(true);
  foreach (var item in value["hobbies"].Array)
  {
    Console.WriteLine(item.AsString());
  }
  value["hobbies"][2].Set(12.3);
  Console.WriteLine(value.ToFormattedString());
}
```
Will output:
```
Golfing
Swimming
Hiking
{
  "name": "Billy",
  "age": 42,
  "employed": true,
  "hobbies":
  [
    "Golfing",
    "Swimming",
    12.3
  ]
}
```
