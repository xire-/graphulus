using Newtonsoft.Json;
using System.Collections.Generic;

public class JsonLoader
{
    public static JsonRoot Deserialize(string path) {
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

    public struct JsonParams
    {
        public float convergence;
        public float damping;
        public float repulsion;
        public float stiffness;
    }

    public struct JsonRoot
    {
        public List<JsonEdge> edges;
        public List<JsonNode> nodes;
        public JsonParams parameters;
    }
}
