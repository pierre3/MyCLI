using System.Collections;

class CommandCompletionItem(string commandName) : ICommandCompletionItem, IEnumerable<KeyValuePair<string, IEnumerable<string>>>
{
    private readonly Dictionary<string, IEnumerable<string>> items = new();

    public string CommandName { get; } = commandName;

    public IEnumerable<string> GetCompletionItems(string optionName, string _) => items[optionName];

    public IEnumerable<string> GetOptions() => items.Keys;

    public void Add(string key, IEnumerable<string> value) => items.Add(key, value);

    public IEnumerator<KeyValuePair<string, IEnumerable<string>>> GetEnumerator()
    {
        return ((IEnumerable<KeyValuePair<string, IEnumerable<string>>>)items).GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}