interface ICommandCompletionItem
{
    string CommandName { get; }
    
    IEnumerable<string> GetOptions();
    IEnumerable<string> GetCompletionItems(string optionName, string wordToComplete);
}
