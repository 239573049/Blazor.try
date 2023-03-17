using BlazorComponent.JSInterop;
using Microsoft.JSInterop;
using System.Text;

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

    public async ValueTask SetStorage(string key, string value)
    {
        await InvokeVoidAsync("setStorage", key, value);
    }
    
    public async ValueTask<string?> GetStorage(string key)
    {
        return await InvokeAsync<string?>("getStorage", key);
    }
    
    public async ValueTask DelStorage(string key)
    {
        await InvokeVoidAsync("delStorage", key);
    }
    
    public async ValueTask ClearStorage()
    {
        await InvokeVoidAsync("clearStorage");
    }

    public async ValueTask AddCommand<T>(IJSObjectReference id, int keybinding, DotNetObjectReference<T> dotNetObjectReference, string method) where T : class
    {
        await InvokeVoidAsync("addCommand", id, keybinding, dotNetObjectReference, method);
    }
}
