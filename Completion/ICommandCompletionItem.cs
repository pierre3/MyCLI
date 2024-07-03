interface ICommandCompletionItem
{
    string CommandName { get; }
    IEnumerable<string> GetAllOptions();
    IEnumerable<string> GetOptions(string wordToComplete);
    IEnumerable<string> GetCompletionItems(string optionName, string wordToComplete);
}
