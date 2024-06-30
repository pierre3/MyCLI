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
        static Task<IEnumerable<string>> TaskResult(IEnumerable<string> result)
            => Task.FromResult(result);

        CompletionProvider =
            [
                new GoogleCompletionItem(httpClient),
                new CommandCompletionItem("search")
                {
                    {"--category", ()=> TaskResult(["books","movies","music"])},
                    {"--sort", ()=> TaskResult(["relevance","date","popularity"])},
                    {"--filter", ()=> TaskResult(["free","paid","all"])}
                },
                new CommandCompletionItem("share")
                {
                    {"--platform", ()=> TaskResult(["facebook","twitter","linkedin"])},
                    {"--visibility", ()=> TaskResult(["public","private","friends"])},
                    {"--tag", ()=> TaskResult(["fun","education","promotion"])}
                },
                new CommandCompletionItem("edit")
                {
                    {"--file", ()=> TaskResult(["document1","document2","document3"])},
                    {"--mode", ()=> TaskResult(["read","write","append"])},
                    {"--backup", ()=> TaskResult([])}
                },
                new CommandCompletionItem("view")
                {
                    {"--layout", ()=> TaskResult(["grid","list","detail"])},
                    {"--sort", ()=> TaskResult(["name","date","size"])},
                    {"--filter", ()=> TaskResult(["all","folders","files"])}
                },
            ];


    }
    public async Task Complete(string wordToComplete, string input, int cursorPosition)
    {
        var items = await CompletionProvider
            .GetCompletionItemsAsync(wordToComplete, input, cursorPosition);
        foreach (var item in items)
        {
            Console.WriteLine(item);
        }
    }
}