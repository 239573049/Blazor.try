using BlazorComponent.JSInterop;
using Microsoft.JSInterop;

namespace Blazor.Try.Shared;

public class TryJsInterop : JSModule
{
    public TryJsInterop(IJSRuntime js) : base(js, "./_content/Blazor.Try.Shared/tryJsInterop.js")
    {
    }
    
    public async ValueTask Init()
    {
        await InvokeVoidAsync("init");
    }

    public async ValueTask AddCommand<T>(IJSObjectReference id, int keybinding, DotNetObjectReference<T> dotNetObjectReference, string method) where T : class
    {
        await InvokeVoidAsync("addCommand", id, keybinding, dotNetObjectReference, method);
    }
}
