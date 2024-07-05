using ConsoleAppFramework;
using System.Diagnostics;
using System.Xml.Linq;

partial class MyCommands
{
    /// <summary>
    /// Performs a Google search with the specified query.
    /// </summary>
    /// <param name="query">The search query.</param>
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

    /// <summary>
    /// CompletionItem that generates search keyword candidates from the entered string.
    /// </summary>
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
            if (optionName != "-query" || wordToComplete=="") { return []; }

            var response = await _httpClient.GetAsync($"https://google.com/complete/search?q={System.Web.HttpUtility.UrlEncode(wordToComplete)}&output=toolbar");
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