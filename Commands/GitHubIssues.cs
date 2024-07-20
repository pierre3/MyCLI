using ConsoleAppFramework;
using System.Net.Http.Headers;
using System.Text.Json;
using Zx;

partial class MyCommands
{
    [Command("gh-issues")]
    [ConsoleAppFilter<BitwardenSessionFilter>]
    public async Task GitHubIssues()
    {
        var token = await $"bw get notes gh-issue-token";

        var request = new HttpRequestMessage();
        request.RequestUri = new Uri("https://api.github.com/repos/pierre3/PlantUmlClassDiagramGenerator/issues");
        request.Method = HttpMethod.Get;
        //request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        request.Headers.Add("X-GitHub-Api-Version", "2022-11-28");
        request.Headers.UserAgent.Add(new ProductInfoHeaderValue(new ProductHeaderValue("mycli")));
        request.Headers.Accept.Clear();
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/vnd.github+json"));
        var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();

        var responseJson = await response.Content.ReadAsStringAsync();

        static IEnumerable<T>? DeserializeAnonimousType<T>(string value, T _) => JsonSerializer.Deserialize<IEnumerable<T>>(value);
        var items = DeserializeAnonimousType(responseJson, new { number = 1, state = "", title = "", url = "" });

        if (items == null)
        {
            Console.WriteLine("No Opened Issue.");
            return;
        }
        foreach (var item in items)
        {
            Console.WriteLine("------------------------------------------------------");
            Console.WriteLine($"#{item.number}: {item.title}");
            Console.WriteLine(item.url);
        }

    }
}
