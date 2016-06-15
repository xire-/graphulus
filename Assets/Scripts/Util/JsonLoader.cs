using Newtonsoft.Json;
using System.Collections.Generic;

public class JsonLoader
{
    public static JsonRoot Deserialize(string path)
    {
        var json = System.IO.File.ReadAllText(path);
        return JsonConvert.DeserializeObject<JsonRoot>(json);
    }

    public struct JsonEdge
    {
        public int source, target;
        public float value;
    }

    public struct JsonNode
    {
        public int group;
        public string name;
    }

    public struct JsonRoot
    {
        public List<JsonEdge> links;
        public List<JsonNode> nodes;
    }
}