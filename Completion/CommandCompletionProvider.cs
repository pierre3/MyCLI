using System.Collections;

/// <summary>
/// Provides command completion functionality.
/// </summary>
class CommandCompletionProvider : ICommandCompletionProvider, IEnumerable
{
    private readonly IList<ICommandCompletionItem> _items = [];

    /// <summary>
    /// Adds a command completion item to the list.
    /// </summary>
    /// <param name="item">The command completion item to add.</param>
    public void Add(ICommandCompletionItem item) => _items.Add(item);


    /// <summary>
    /// Completes the command based on the input and cursor position.
    /// </summary>
    /// <param name="wordToComplete">The word to complete.</param>
    /// <param name="input">The input string.</param>
    /// <param name="cursorPosition">The cursor position in the input string.</param>
    /// <returns>A list of possible completions for the command.</returns>
    public IEnumerable<string> Complete(string wordToComplete, string input, int cursorPosition)
    {
        var tokens = Parse(input, cursorPosition).Skip(1).ToArray();
        return GetCompletionItems(tokens, wordToComplete);
    }

    /// <summary>
    /// Gets the completion items for the command.
    /// </summary>
    /// <param name="tokens">The tokens from the input string.</param>
    /// <param name="wordToComplete">The word to complete.</param>
    /// <returns>A list of possible completions for the command.</returns>
    private IEnumerable<string> GetCompletionItems(string[] tokens, string wordToComplete)
    {
        // No command input
        if (tokens.Length == 0)
        {
            return GetCommandNames("");
        }
        // The first element is the command name
        var cmd = _items.FirstOrDefault(x => tokens[0] == x.CommandName);
        if (cmd == null)
        {
            // If the command is not entered or is being entered, display a list of command names
            return GetCommandNames(wordToComplete);
        }
        // Get a list of options from the command being entered
        var allOptions = cmd.GetAllOptions().ToArray();
        if (tokens.Length == 1) // Only the command is specified
        {
            return cmd.GetOptions(wordToComplete);
        }

        var op1 = allOptions.FirstOrDefault(o => o == tokens[^1]);
        // The last input value matches the option
        if (op1 != null)
        {
            // Get input candidates for the option
            return GetCompletionItemByOptionName(cmd, op1, tokens, allOptions, wordToComplete);
        }

        var op2 = allOptions.FirstOrDefault(o => o == tokens[^2]);
        // If the second from the end is an option
        if (op2 != null && wordToComplete != "")
        {
            // Get input candidates for the option
            return GetCompletionItemByOptionName(cmd, op2, tokens, allOptions, wordToComplete);
        }

        return cmd.GetOptions(wordToComplete).Where(o => !tokens.Contains(o));
    }

    /// <summary>
    /// Gets the completion items for a specific option of a command.
    /// </summary>
    /// <param name="cmd">The command.</param>
    /// <param name="optionName">The name of the option.</param>
    /// <param name="tokens">The tokens from the input string.</param>
    /// <param name="options">The options for the command.</param>
    /// <param name="wordToComplete">The word to complete.</param>
    /// <returns>A list of possible completions for the option.</returns>
    private static IEnumerable<string> GetCompletionItemByOptionName(
        ICommandCompletionItem cmd, string optionName, string[] tokens, string[] options, string wordToComplete)
    {
        // Get input candidates for the option
        var item = cmd.GetCompletionItems(optionName, wordToComplete);
        if (!item.Any())
        {
            // If there are no value candidates, use the option as a candidate
            return cmd.GetOptions(wordToComplete).Where(o => !tokens.Contains(o));
        }
        return item;
    }

    /// <summary>
    /// Gets the names of all commands.
    /// </summary>
    /// <param name="wordToComplete">The word to complete.</param>
    /// <returns>A list of all command names.</returns>
    private IEnumerable<string> GetCommandNames(string wordToComplete)
    {
        if (wordToComplete == "")
        {
            return _items.Select(x => x.CommandName);
        }
        return _items.Where(x => x.CommandName.Contains(wordToComplete)).Select(x => x.CommandName);
    }

    /// <summary>
    /// Parses the input string into tokens.
    /// </summary>
    /// <param name="input">The input string.</param>
    /// <param name="position">The cursor position in the input string.</param>
    /// <returns>A list of tokens from the input string.</returns>
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

    /// <summary>
    /// Returns an enumerator that iterates through the command completion items.
    /// </summary>
    /// <returns>An enumerator for the command completion items.</returns>
    IEnumerator IEnumerable.GetEnumerator() => _items.GetEnumerator();

}
