using Newtonsoft.Json;

/// <summary>
/// A simple todo item.
/// </summary>
public class TodoItem
{
    /// <summary>
    /// The ID of the item.
    /// </summary>
    [JsonProperty(PropertyName = "id")]
    public string Id { get; set; }

    /// <summary>
    /// The text of the item.
    /// </summary>
    [JsonProperty(PropertyName = "text")]
    public string Text { get; set; }

    /// <summary>
    /// Specifies whether the item is completed or not.
    /// </summary>
    [JsonProperty(PropertyName = "complete")]
    public bool Complete { get; set; }
}
