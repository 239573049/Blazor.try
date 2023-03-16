using Microsoft.Extensions.DependencyInjection;

namespace Blazor.Try.Shared;

public static class BlazorTryExtension
{
    /// <summary>
    /// 当前是否为WebAssembly
    /// </summary>
    public static bool WebAssembly { get; private set; }

    /// <summary>
    /// 添加 Blazor Try 
    /// </summary>
    /// <param name="services"></param>
    /// <param name="webAssembly">是否WebAssembly启动</param>
    /// <returns></returns>
    public static IServiceCollection AddBlazorTry(this IServiceCollection services, bool webAssembly = false)
    {
        WebAssembly = webAssembly;
        services.AddScoped<TryJsInterop>();
        services.AddMasaBlazor();
        return services;
    }
}