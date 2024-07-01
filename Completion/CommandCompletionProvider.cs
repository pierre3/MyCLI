using System.Collections;

/// <summary>
/// Provides command completion functionality.
/// </summary>
class CommandCompletionProvider : IEnumerable<ICommandCompletionItem>
{
    /// <summary>
    /// Gets the list of command completion items.
    /// </summary>
    public IList<ICommandCompletionItem> Items { get; } = [];

    /// <summary>
    /// Adds a command completion item to the list.
    /// </summary>
    public void Add(ICommandCompletionItem item) => Items.Add(item);

    /// <summary>
    /// Returns an enumerator that iterates through the command completion items.
    /// </summary>
    public IEnumerator<ICommandCompletionItem> GetEnumerator()
    {
        return Items.GetEnumerator();
    }

    /// <summary>
    /// Returns an enumerator that iterates through the command completion items.
    /// </summary>
    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    /// <summary>
    /// Asynchronously gets the completion items for a given word to complete.
    /// </summary>
    public async Task<IEnumerable<string>> GetCompletionItemsAsync(string wordToComplete, string input, int cursorPosition)
    {
        var tokens = Parse(input, cursorPosition).Skip(1).ToArray();
        return await GetCompletionItemsAsync(tokens, wordToComplete.Trim('"'));
    }

    /// <summary>
    /// Asynchronously gets the completion items for a given word to complete.
    /// </summary>
    private async Task<IEnumerable<string>> GetCompletionItemsAsync(string[] tokens, string wordToComplete)
    {
        // No command input
        if (tokens.Length == 0)
        {
            return GetCommandNames("");
        }
        // The first element is the command name
        var cmd = Items.FirstOrDefault(x => tokens[0] == x.CommandName);
        if (cmd == null)
        {
            // If no command is entered or is being entered, display a list of command names
            return GetCommandNames(wordToComplete);
        }
        // Get a list of options from the command being entered
        var options = cmd.GetOptions().ToArray();
        if (tokens.Length == 1) // Only the command is specified
        {
            return GetOptionNames(wordToComplete, options);
        }

        var op1 = options.FirstOrDefault(o => o == tokens[^1]);
        // The last input value matches the option
        if (op1 != null)
        {
            // Get input candidates for the option
            return await GetCompletionItemByOptionName(cmd, op1, tokens, options, wordToComplete);
        }

        var op2 = options.FirstOrDefault(o => o == tokens[^2]);
        // If the second from the last is an option
        if (op2 != null && wordToComplete != "")
        {
            // Get input candidates for the option
            return await GetCompletionItemByOptionName(cmd, op2, tokens, options, wordToComplete);
        }

        return GetOptionNames(wordToComplete, options.Where(o => !tokens.Contains(o)).ToArray());
    }

    /// <summary>
    /// Asynchronously gets the completion items by option name.
    /// </summary>
    private static async Task<IEnumerable<string>> GetCompletionItemByOptionName(
        ICommandCompletionItem cmd, string optionName, string[] tokens, string[] options, string wordToComplete)
    {
        // Get input candidates for the option
        var item = await cmd.GetCompletionItemsAsync(optionName, wordToComplete);
        if (!item.Any())
        {
            // If there are no value candidates, suggest the option as a candidate
            return GetOptionNames(wordToComplete, options.Where(o => !tokens.Contains(o)).ToArray());
        }
        // Filter by the string being entered
        return item.Where(x => x.Contains(wordToComplete, StringComparison.InvariantCultureIgnoreCase));
    }

    /// <summary>
    /// Gets the option names.
    /// </summary>
    private static IEnumerable<string> GetOptionNames(string wordToComplete, string[] options)
    {
        if (wordToComplete == "")
        {
            return options;
        }
        return options.Where(o => o.Contains(wordToComplete));
    }

    /// <summary>
    /// Gets the command names.
    /// </summary>
    private IEnumerable<string> GetCommandNames(string wordToComplete)
    {
        if (wordToComplete == "")
        {
            return Items.Select(x => x.CommandName);
        }
        return Items.Where(x => x.CommandName.Contains(wordToComplete)).Select(x => x.CommandName);
    }

    /// <summary>
    /// Parses the input into tokens.
    /// </summary>
    private static IEnumerable<string> Parse(string input, int position)
    {
        var source = input[0..Math.Min(input.Length, position)];
        var token = "";
        var inQuote = false;
        var isEscaped = false;
        for (int i = 0; i < source.Length; i++)
        {
            var c = source[i];
            if (isEscaped)
            {
                token += c;
                isEscaped = false;
                continue;
            }
            switch (c)
            {
                case '"':
                    inQuote = !inQuote;
                    break;
                case '\\':
                    isEscaped = true;
                    break;
                case ' ':
                    if (inQuote)
                    {
                        token += c;
                    }
                    else if (!string.IsNullOrEmpty(token))
                    {
                        yield return token;
                        token = "";
                    }
                    break;
                default:
                    token += c;
                    break;
            }
        }
        if (!string.IsNullOrEmpty(token))
        {
            yield return token;
        }
    }

}
