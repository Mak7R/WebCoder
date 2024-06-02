using System.Runtime.Serialization;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace RepositoriesStorage.RepositoriesRepository;

public class FileSystemTreeNode
{
    [JsonInclude]
    [JsonPropertyName("name")]
    public string Name { get; set; }
    
    [JsonInclude]
    [JsonPropertyName("type")]
    public string Type { get; set; }
    
    [JsonInclude]
    [JsonPropertyName("children")]
    public List<FileSystemTreeNode> Children { get; set; } = [];

    public static string ToJson(FileSystemTreeNode fileSystemTreeNode)
    {
        return JsonSerializer.Serialize(fileSystemTreeNode);
    }

    public static FileSystemTreeNode FromJson(string jsonString)
    {
        return JsonSerializer.Deserialize<FileSystemTreeNode>(jsonString) ??
               throw new SerializationException("Deserialization result was null");
    }
    
    // JsonConstructor
    public FileSystemTreeNode(){}

    public FileSystemTreeNode(string name, string type)
    {
        Name = name;
        Type = type;
    }
}

public static class NodeExtensions
{
    public static string ToStringTree(this FileSystemTreeNode fileSystemTreeNode, int lvl = 0)
    {
        var sb = new StringBuilder();

        for (int i = 0; i < lvl; i++) 
            sb.Append("  ");
        
        sb.Append(fileSystemTreeNode.Name).Append('\n');
        
        foreach (var child in fileSystemTreeNode.Children)
            sb.Append(child.ToStringTree(lvl + 1));

        return sb.ToString();
    }
}