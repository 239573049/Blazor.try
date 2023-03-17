using System.IO;
using System.Runtime.Loader;
using System.Text.Json;
using Blazor.Try.Shared.Modules;
using BlazorComponent;
using Masa.Blazor.Extensions.Languages.Razor;
using Masa.Blazor.Presets;
using Microsoft.AspNetCore.Razor.Language;
using Microsoft.CodeAnalysis;
using Microsoft.JSInterop;

namespace Blazor.Try.Shared.Pages;

public partial class Home : IAsyncDisposable
{
    private Type? componentType;

    private bool load;

    private string GitHub = "https://github.com/239573049/Blazor.try";

    private static string Code = """
<MCard
    Class="overflow-hidden mx-auto"
    Height="200"
    MaxWidth="500">
    <MBottomNavigation
        Absolute
        HideOnScroll
        Horizontal
        ScrollTarget="#hide-on-scroll-example">
        <MButton
            Color="deep-purple accent-4"
            Text>
            <span>Recents</span>

            <MIcon>mdi-history</MIcon>
        </MButton>

        <MButton
            Color="deep-purple accent-4"
            Text>
            <span>Favorites</span>

            <MIcon>mdi-heart</MIcon>
        </MButton>

        <MButton
            Color="deep-purple accent-4"
            Text>
            <span>Nearby</span>

            <MIcon>mdi-map-marker</MIcon>
        </MButton>
    </MBottomNavigation>

    <MResponsive
        Id="hide-on-scroll-example"
        Class="overflow-y-auto"
        MaxHeight="600">
        <MResponsive Height="1500"></MResponsive>
    </MResponsive>
</MCard>
""";

    /// <summary>
    /// 编译器需要使用的程序集
    /// </summary>
    private static List<string> Assemblys = new()
    {
        "BlazorComponent",
        "Masa.Blazor",
        "OneOf",
        "FluentValidation",
        "netstandard",
        "FluentValidation.DependencyInjectionExtensions",
        "System",
        "Microsoft.AspNetCore.Components",
        "System.Linq.Expressions",
        "System.Net.Http.Json",
        "System.Private.CoreLib",
        "Microsoft.AspNetCore.Components.Web",
        "System.Collections",
        "System.Linq",
        "System.Runtime"
    };

    private static List<PortableExecutableReference>? PortableExecutableReferences;

    private static List<RazorExtension>? RazorExtensions;

    public List<TabMonacoModule> TabMonacoList { get; set; } = new();

    public StringNumber TabStringNumber { get; set; }

    private PEnqueuedSnackbars? _enqueuedSnackbars;

    public bool createPModal { get; set; }

    public TabMonacoModule CreateMona { get; set; } = new();

    private bool initialize = false;

    private bool drawer = true;

    private DotNetObjectReference<Home> _objRef;

    [JSInvokable("RunCode")]
    public async void RunCode()
    {
        if (initialize == false)
        {
            return;
        }

        load = true;
        StateHasChanged();
        await Task.Delay(10);

        // 得到当前编辑器的代码
        var code = await TabMonacoList[(int)TabStringNumber].MonacoEditor.GetValue();

        // 更新当前编辑器的代码
        TabMonacoList[(int)TabStringNumber].Options = new
        {
            value = code,
            language = "razor",
            theme = "vs-dark",
            automaticLayout = true,
        };

        var options = new CompileRazorOptions()
        {
            Code = code,
            ConcurrentBuild = true
        };

        await TryJsInterop.SetStorage("code", JsonSerializer.Serialize(TabMonacoList));

        componentType = RazorCompile.CompileToType(options);
        load = false;
        StateHasChanged();
    }

    private void Close(TabMonacoModule tabMonacoModule)
    {
        if (TabMonacoList.Count == 1)
        {
            _enqueuedSnackbars?.EnqueueSnackbar(new SnackbarOptions()
            {
                Content = "Keep at least one",
                Type = AlertTypes.Error,
            });
            return;
        }

        tabMonacoModule.MonacoEditor.Dispose();
        TabMonacoList.Remove(tabMonacoModule);
        TabStringNumber = 0;
    }

    private async Task CreateFile()
    {
        createPModal = false;
        if (TabMonacoList.Any(x => x.Name == CreateMona.Name))
        {
            CreateMona = new TabMonacoModule();
            _enqueuedSnackbars?.EnqueueSnackbar(new SnackbarOptions()
            {
                Content = "Duplicate file name",
                Type = AlertTypes.Error,
            });
            return;
        }

        TabMonacoList.Add(new TabMonacoModule()
        {
            Name = CreateMona.Name,
            Options = new
            {
                value = Code,
                language = "razor",
                theme = "vs-dark",
                automaticLayout = true,
            }
        });

        await TryJsInterop.SetStorage("code", JsonSerializer.Serialize(TabMonacoList));

        CreateMona = new TabMonacoModule();
    }

    protected override async Task OnInitializedAsync()
    {
        _objRef = DotNetObjectReference.Create(this);

        await base.OnInitializedAsync();
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            load = true;
            RazorCompile.Initialized(await GetReference(), GetRazorExtension());
            load = false;
            var value = JsonSerializer.Deserialize<List<TabMonacoModule>>(await TryJsInterop.GetStorage("code") ??"[]");
            if (value == null || value.Count == 0)
            {
                TabMonacoList.Add(new TabMonacoModule()
                {
                    Name = "masa.razor",
                    Options = new
                    {
                        value = Code,
                        language = "razor",
                        theme = "vs-dark",
                        automaticLayout = true,
                    }
                });

                await TryJsInterop.SetStorage("code", JsonSerializer.Serialize(TabMonacoList));
            }
            else
            {
                TabMonacoList = value;
            }

            await TryJsInterop.Init();

            StateHasChanged();

        }

        await base.OnAfterRenderAsync(firstRender);
    }

    private async Task InitMonaco(TabMonacoModule tabMonacoModule)
    {
        // 监听CTRL+S
        await TryJsInterop.AddCommand(tabMonacoModule.MonacoEditor.Monaco, 2097, _objRef, nameof(RunCode));
    }

    private async Task<List<PortableExecutableReference>?> GetReference()
    {

        if (PortableExecutableReferences == null)
        {
            PortableExecutableReferences = new List<PortableExecutableReference>();
            if (BlazorTryExtension.WebAssembly)
            {
                foreach (var asm in Assemblys)
                {
                    try
                    {
                        await using var stream = await HttpClient!.GetStreamAsync($"_framework/{asm}.dll");

                        if (stream.Length > 0)
                        {
                            PortableExecutableReferences?.Add(MetadataReference.CreateFromStream(stream));
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }
                }
            }
            else
            {
                foreach (var asm in AssemblyLoadContext.Default.Assemblies)
                {
                    try
                    {
                        PortableExecutableReferences?.Add(MetadataReference.CreateFromFile(asm.Location));
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }
                }
            }
        }

        initialize = true;

        return PortableExecutableReferences;
    }

    private List<RazorExtension> GetRazorExtension()
    {
        if (RazorExtensions == null)
        {
            RazorExtensions = new List<RazorExtension>();

            foreach (var asm in typeof(Index).Assembly.GetReferencedAssemblies())
            {
                RazorExtensions.Add(new AssemblyExtension(asm.FullName, AppDomain.CurrentDomain.Load(asm.FullName)));
            }
        }

        return RazorExtensions;
    }

    public async ValueTask DisposeAsync()
    {
        _objRef.Dispose();
    }
}