using System.Collections.Generic;

using Newtonsoft.Json;


public class JsonLoader
{
    public class JsonWorld
    {
        public List<JsonNode> nodes { get; set; }
        public List<JsonEdge> links { get; set; }
    }

    public class JsonNode
    {
        public string name { get; set; }
        public int group { get; set; }
    }

    public class JsonEdge
    {
        public int source { get; set; }
        public int target { get; set; }
        public int value { get; set; }
    }

    public static JsonWorld Load(string path)
    {
        var json = System.IO.File.ReadAllText(path);
        return JsonConvert.DeserializeObject<JsonWorld>(json);
    }
}
