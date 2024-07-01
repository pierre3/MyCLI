using System.Collections;

class CommandCompletionItem( string commandName) : ICommandCompletionItem, IEnumerable<KeyValuePair<string, Func<IEnumerable<string>>>>
{
    private readonly Dictionary<string, Func<IEnumerable<string>>> items = new();    

    public string CommandName { get; } = commandName;

    public IEnumerable<string> GetCompletionItems(string optionName, string _) => items[optionName]();

    public IEnumerable<string> GetOptions() => items.Keys;

    public void Add(string key, Func<IEnumerable<string>> value) => items.Add(key, value);

    public IEnumerator<KeyValuePair<string, Func<IEnumerable<string>>>> GetEnumerator()
    {
        return ((IEnumerable<KeyValuePair<string, Func<IEnumerable<string>>>>)items).GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}