interface ICommandCompletionItem
{
    string CommandName { get; }
    
    IEnumerable<string> GetOptions();
    Task<IEnumerable<string>> GetCompletionItemsAsync(string optionName, string wordToComplete);
}
