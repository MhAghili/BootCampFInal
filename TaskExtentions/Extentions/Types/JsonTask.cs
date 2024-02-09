using Testpr.Enums;
using System.Text.Json.Serialization;

namespace Testpr.Types;

class JsonTask
{
    public string Title {get; set;} = "";
    public string Description { get; set; } = "";
    public DateTime DueDate { get; set; }

    public DateTime? CompletedDate { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public TaskPriority Priority { get; set; }

}
