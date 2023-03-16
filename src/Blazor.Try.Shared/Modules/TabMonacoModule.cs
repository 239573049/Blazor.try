using System.Text.Json.Serialization;
using Masa.Blazor;

namespace Blazor.Try.Shared.Modules;

public class TabMonacoModule
{
    [JsonIgnore]
    public MMonacoEditor MonacoEditor { get; set; }

    public string Name { get; set; }

    /// <summary>
    /// Monaco配置
    /// </summary>
    public object Options { get; set; }
}