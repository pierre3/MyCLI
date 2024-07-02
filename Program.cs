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
            .GetCompletionItemsAsync(wordToComplete, input, cursorPosition);
        foreach (var item in items)
        {
            Console.WriteLine(item);
        }
    }
}