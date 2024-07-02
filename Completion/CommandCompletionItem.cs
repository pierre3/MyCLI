using System.Collections;

class CommandCompletionItem(string commandName) : ICommandCompletionItem, IEnumerable
{
    private readonly Dictionary<string, IEnumerable<string>> items = new();

    public string CommandName { get; } = commandName;

    public IEnumerable<string> GetCompletionItems(string optionName, string _) => items[optionName];

    public IEnumerable<string> GetOptions() => items.Keys;

    public void Add(string key, IEnumerable<string> value) => items.Add(key, value);

    IEnumerator IEnumerable.GetEnumerator() => items.GetEnumerator();
}