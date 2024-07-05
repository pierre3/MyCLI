using System.Collections;

interface ICommandCompletionProvider
{
    void Add(ICommandCompletionItem item);
    IEnumerable<string> Complete(string wordToComplete, string input, int cursorPosition);
}
