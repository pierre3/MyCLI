using System.Collections;

class CommandCompletionItem(string commandName) : ICommandCompletionItem, IEnumerable
{
    private readonly Dictionary<string, IEnumerable<string>> items = new();

    public string CommandName { get; } = commandName;

    public IEnumerable<string> GetAllOptions() => items.Keys;

    public IEnumerable<string> GetOptions(string wordToComplete)
        => items.Keys.Where(o => o.Contains(wordToComplete, StringComparison.InvariantCultureIgnoreCase));

    public IEnumerable<string> GetCompletionItems(string optionName, string wordToComplete)
        => items[optionName].Where(o => o.Contains(wordToComplete, StringComparison.InvariantCultureIgnoreCase));

    
    public void Add(string key, IEnumerable<string> value) => items.Add(key, value);

    IEnumerator IEnumerable.GetEnumerator() => items.GetEnumerator();
}