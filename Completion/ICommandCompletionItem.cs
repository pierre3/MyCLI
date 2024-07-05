interface ICommandCompletionItem
{
    string CommandName { get; }
    IEnumerable<string> GetAllOptions();
    IEnumerable<string> GetOptions(string wordToComplete);
    Task<IEnumerable<string>> GetCompletionItemsAsync(string optionName, string wordToComplete);
}
