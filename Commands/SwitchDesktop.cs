using ConsoleAppFramework;
using Cysharp.Diagnostics;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Xml.Linq;
using Zx;


partial class MyCommands
{
    [Command("swdt")]
    public async Task SwitchDesktop(string name)
    {
        Env.shell = "pwsh -c";
        var desktop = await Env.runl($"Get-DesktopList | ? -Property Name -EQ '{name}'");
        if (desktop?.Length > 0)
        {
            await $"Switch-Desktop '{name}'";
        }
        else
        {
            await $"New-Desktop | Set-DesktopName -Name '{name}' -PassThru | Switch-Desktop";
        }
    }


    class SwitchDesktopCompletionItem : ICommandCompletionItem
    {
        private static readonly IEnumerable<string> options = ["--name"];

        public string CommandName => "swdt";

        public async Task<IEnumerable<string>> GetCompletionItemsAsync(string optionName, string wordToComplete)
        {
            if (optionName != "--name") { return []; }
            Env.shell = "pwsh -c";
            Env.verbose = false;
            var desktopNames = await Env.runl($"Get-DesktopList | % {{ $_.Name }}");
            return desktopNames.Select(name => $"\"{name}\"");
        }

        public IEnumerable<string> GetAllOptions() => options;

        public IEnumerable<string> GetOptions(string wordToComplete)
            => options.Where(o => o.Contains(wordToComplete, StringComparison.InvariantCultureIgnoreCase));
    }
}
