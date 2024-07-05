using System.Collections;

interface ICommandCompletionProvider
{
    void Add(ICommandCompletionItem item);
    Task<IEnumerable<string>> CompleteAsync(string wordToComplete, string input, int cursorPosition);
}
