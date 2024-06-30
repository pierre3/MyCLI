using ConsoleAppFramework;
using System.Diagnostics;
using System.Xml.Linq;

partial class MyCommands
{
    /// <summary>
    /// 指定したクエリでgoogle検索します
    /// </summary>
    /// <param name="query">-q,検索クエリ</param>
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
    /// 入力した文字列から、検索キーワードの候補を生成するCompletionItem
    /// </summary>
    class GoogleCompletionItem : ICommandCompletionItem
    {
        private HttpClient _httpClient;
        public GoogleCompletionItem(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public string CommandName => "google";

        public async Task<IEnumerable<string>> GetCompletionItemsAsync(string optionName, string wordToComplete)
        {
            if (optionName != "-q" || wordToComplete=="") { return []; }

            var response = await _httpClient.GetAsync($"https://google.com/complete/search?q={System.Web.HttpUtility.UrlEncode(wordToComplete)}&output=toolbar");
            using var stream = await response.Content.ReadAsStreamAsync();
            var xml = XDocument.Load(stream);
            return xml.Descendants("suggestion")
                .Select(element => $"\"{(element.Attribute("data")?.Value ?? "")}\"");
        }

        public IEnumerable<string> GetOptions() => ["-q"];
    }
}