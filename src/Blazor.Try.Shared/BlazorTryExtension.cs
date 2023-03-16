using Microsoft.Extensions.DependencyInjection;

namespace Blazor.Try.Shared;

public static class BlazorTryExtension
{
    /// <summary>
    /// 添加 Blazor Try 
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection AddBlazorTry(this IServiceCollection services)
    {

        services.AddMasaBlazor();
        return services;
    }
}