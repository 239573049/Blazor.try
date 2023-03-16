using BlazorComponent.JSInterop;
using Microsoft.JSInterop;

namespace Blazor.Try.Shared;

public class TryJsInterop : JSModule
{
    public TryJsInterop(IJSRuntime js) : base(js, "./_content/Blazor.Try.Shared/tryJsInterop.js")
    {
    }
    
}
