using ConsoleAppFramework;
using System.Diagnostics;
using System.Text;
using System.Xml.Linq;
using Zx;

partial class MyCommands
{
    [ConsoleAppFilter<BitwardenSessionFilter>]
    public async Task BwMessage()
    {
        Console.Write("メッセージ: ");
        var message = await $"bw get notes secure-memo";
        Console.WriteLine(message);
    }
}
