using ConsoleAppFramework;
using System.Text.RegularExpressions;
using Zx;

internal partial class BitwardenSessionFilter(ConsoleAppFilter next) : ConsoleAppFilter(next)
{
    public override async Task InvokeAsync(ConsoleAppContext context, CancellationToken cancellationToken)
    {
        Env.verbose = false;

        T? DeserializeAnonimousType<T>(string value, T _) => System.Text.Json.JsonSerializer.Deserialize<T>(value);

        CopyEnvBwSession();
        Console.WriteLine("Checking Bitwarden status...");
        var ret = await "bw status";
        var status = DeserializeAnonimousType(ret, new { status = "" })?.status;
        switch (status)
        {
            case "unauthenticated":
                await BwLogin();
                break;
            case "locked":
                await BwUnlock();
                break;
        };
        Console.WriteLine("Ready! The Bitwarden session is active.");
        await Next.InvokeAsync(context, cancellationToken);
    }

    private static void CopyEnvBwSession()
    {
        var session = Environment.GetEnvironmentVariable("BW_SESSION", EnvironmentVariableTarget.User);
        if (!string.IsNullOrEmpty(session))
        {
            Environment.SetEnvironmentVariable("BW_SESSION", session, EnvironmentVariableTarget.Process);
        }
    }

    private static void SetEnvBwSession(string session)
    {
        Environment.SetEnvironmentVariable("BW_SESSION", session, EnvironmentVariableTarget.Process);
        Environment.SetEnvironmentVariable("BW_SESSION", session, EnvironmentVariableTarget.User);
    }

    private static async Task BwLogin()
    {
        Console.WriteLine("bw logion");
        Console.Write("? Email Address: "); var email = Console.ReadLine();
        Console.Write("? Master Password: "); var password = ReadPassword();
        Console.Write("? Auth Code: "); var code = Console.ReadLine();

        var ret = await $"bw login --code {code} {email} {password}";
        var session = GetBwSession(ret);
        SetEnvBwSession(session);
    }

    private static async Task BwUnlock()
    {
        Console.WriteLine("bw unlock");
        Console.Write("? Master Password: "); var password = ReadPassword();
        var ret = await $"bw unlock {password}";
        var session = GetBwSession(ret);
        SetEnvBwSession(session);
    }

    [GeneratedRegex("BW_SESSION=\"(.*?)\"")]
    private static partial Regex BwSessionRegiex();

    private static string GetBwSession(string ret)
    {
        var match = BwSessionRegiex().Match(ret);
        if (match.Success)
        {
            return match.Groups[1].Value;
        }
        return "";
    }

    private static string ReadPassword()
    {
        string password = "";
        ConsoleKeyInfo keyInfo;
        do
        {
            keyInfo = Console.ReadKey(intercept: true);
            if (keyInfo.Key == ConsoleKey.Backspace && password.Length > 0)
            {
                Console.Write("\b \b");
                password = password[0..^1];
            }
            else if (!char.IsControl(keyInfo.KeyChar))
            {
                Console.Write("*");
                password += keyInfo.KeyChar;
            }
        } while (keyInfo.Key != ConsoleKey.Enter);
        Console.WriteLine("");
        return password;
    }

}
