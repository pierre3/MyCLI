# MyCLI

This is a sample that demonstrates how to add an auto-completion feature, which operates with PowerShell, to a CLI tool created with a .NET console application.

![completion2](/img/completion2.gif)

## Sample Usage

### Prerequisites

This sample has been implemented and operationally verified in the following environment:

- .NET 8.0
- PowerShell v7.4.3

### Installation

1. Clone this repository and navigate to the repository folder.
2. Package it using the `dotnet pack` command.
3. Install it as a global tool using the `dotnet tool install` command.

```powershell
PS > dotnet pack
PS > dotnet tool install -g --add-source .\nupkg mycli
```

### Enable Tab Completion
Open your PowerShell profile and paste the following script to enable tab completion.

```powershell
PS > notepad $PROFILE
```

```powershell
# PowerShell parameter completion shim for the mycli
Register-ArgumentCompleter -Native -CommandName mycli -ScriptBlock {
    param($wordToComplete, $commandAst, $cursorPosition)
    mycli complete --word-to-complete $wordToComplete --input "$commandAst" --cursor-position $cursorPosition | ForEach-Object {
        [System.Management.Automation.CompletionResult]::new($_, $_, 'ParameterValue', $_)
    }
}

```

## Implementation Overview
The sample application is a .NET console application that uses the ConsoleAppFramework. In addition to the commands provided as a CLI tool, it implements a complete method for tab completion.

```cs
using ConsoleAppFramework;
using Microsoft.Extensions.DependencyInjection;

var services = new ServiceCollection()
    .AddHttpClient();
using var serviceProvider = services.BuildServiceProvider();
ConsoleApp.ServiceProvider = serviceProvider;

var app = ConsoleApp.Create();
app.Add<MyCommands>();
await app.RunAsync(args);

partial class MyCommands
{
    private readonly CommandCompletionProvider CompletionProvider;

    public MyCommands(HttpClient httpClient)
    {
        CompletionProvider =
            [
                new GoogleCompletionItem(httpClient),
                new CommandCompletionItem("search")
                {
                    {"--category", ["books","movies","music"]},
                    {"--sort", ["relevance","date","popularity"]},
                    {"--filter", ["free","paid","all"]}
                },
                new CommandCompletionItem("share")
                {
                    {"--platform", ["facebook","twitter","linkedin"]},
                    {"--visibility", ["public","private","friends"]},
                    {"--tag", ["fun","education","promotion"]}
                },
                new CommandCompletionItem("edit")
                {
                    {"--file", ["document1","document2","document3"]},
                    {"--mode", ["read","write","append"]},
                    {"--backup", []}
                },
                new CommandCompletionItem("view")
                {
                    {"--layout", ["grid","list","detail"]},
                    {"--sort", ["name","date","size"]},
                    {"--filter", ["all","folders","files"]}
                },
            ];
    }

    public async Task Complete(string wordToComplete, string input, int cursorPosition)
    {
        var items = await CompletionProvider
            .CompleteAsync(wordToComplete, input, cursorPosition);
        foreach (var item in items)
        {
            Console.WriteLine(item);
        }
    }
}
```

### CommandCompletionItem

CommandCompletionItem class is used to provide command completion functionality in the MyCLI application. It takes a list of commands and their possible options, and provides these as suggestions when the user is typing in the console.

![completion](/img/completion.gif)

```cs
class CommandCompletionItem(string commandName) : ICommandCompletionItem, IEnumerable
{
    private readonly Dictionary<string, IEnumerable<string>> items = new();

    public string CommandName { get; } = commandName;

    public IEnumerable<string> GetAllOptions() => items.Keys;

    public IEnumerable<string> GetOptions(string wordToComplete)
        => items.Keys.Where(o => o.Contains(wordToComplete, StringComparison.InvariantCultureIgnoreCase));

    public Task<IEnumerable<string>> GetCompletionItemsAsync(string optionName, string wordToComplete)
    {
        var result = items[optionName].Where(o => o.Contains(wordToComplete, StringComparison.InvariantCultureIgnoreCase));
        return Task.FromResult(result);
    }
    
    public void Add(string key, IEnumerable<string> value) => items.Add(key, value);

    IEnumerator IEnumerable.GetEnumerator() => items.GetEnumerator();
}
```


### GoogleCompletionItem

GoogleCompletionItem class is an example of dynamically generating completion candidates. It uses the Google Suggest API to generate search keyword candidates.

![google-command](/img/google-command.gif)

```cs
using ConsoleAppFramework;
using System.Diagnostics;
using System.Xml.Linq;

partial class MyCommands
{
    [Command("google")]
    public void GoogleSearch(string query)
    {
        var process = new Process()
        {
            StartInfo = new ProcessStartInfo()
            {
                FileName = "pwsh",
                Arguments = $"-Command start https://www.google.com/search?q={System.Web.HttpUtility.UrlEncode(query)}"
            }
        };
        process.Start();
    }

    class GoogleCompletionItem : ICommandCompletionItem
    {
        private HttpClient _httpClient;

        private static readonly IEnumerable<string> options = ["--query"];

        public GoogleCompletionItem(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public string CommandName => "google";

        public async Task<IEnumerable<string>> GetCompletionItemsAsync(string optionName, string wordToComplete)
        {
            if (optionName != "--query" || wordToComplete=="") { return []; }

            var response = await _httpClient.GetAsync($"https://google.com/complete/search?hl=en&q={System.Web.HttpUtility.UrlEncode(wordToComplete)}&output=toolbar");
            using var stream = await response.Content.ReadAsStreamAsync();
            var xml = XDocument.Load(stream);
            return xml.Descendants("suggestion")
                .Select(element => $"\"{(element.Attribute("data")?.Value ?? "")}\"");
        }

        public IEnumerable<string> GetAllOptions() => options;

        public IEnumerable<string> GetOptions(string wordToComplete) 
            => options.Where(o => o.Contains(wordToComplete, StringComparison.InvariantCultureIgnoreCase));
    }
}
```


