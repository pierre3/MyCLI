using ConsoleAppFramework;
using System.Diagnostics;
using Zx;

partial class MyCommands
{

    [Command("pin")]
    public async Task PinWindow()
    {
        Env.shell = "pwsh -c";
        var terminal = Process.GetProcessesByName("WindowsTerminal").FirstOrDefault();
        if (terminal != null)
        {
            await $"Pin-Window {terminal.MainWindowHandle}";
            Console.WriteLine($"Pinned \"{terminal.MainWindowTitle}\".");
        }
        var slack = Process.GetProcessesByName("slack").FirstOrDefault();
        if (slack != null)
        {
            await $"Pin-Window {slack.MainWindowHandle}";
            Console.WriteLine($"Pinned \"{slack.MainWindowTitle}\".");
        }
        var outlook = Process.GetProcessesByName("olk").FirstOrDefault();
        if (outlook != null)
        {
            await $"Pin-Window {outlook.MainWindowHandle}";
            Console.WriteLine($"Pinned \"{outlook.MainWindowTitle}\".");
        }
    }
}
