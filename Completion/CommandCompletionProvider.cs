﻿using System.Collections;

class CommandCompletionProvider : IEnumerable<ICommandCompletionItem>
{
    public IList<ICommandCompletionItem> Items { get; } = [];
    public void Add(ICommandCompletionItem item) => Items.Add(item);

    public IEnumerator<ICommandCompletionItem> GetEnumerator()
    {
        return Items.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public IEnumerable<string> GetCompletionItems(string wordToComplete, string input, int cursorPosition)
    {
        var tokens = Parse(input, cursorPosition).Skip(1).ToArray();
        return GetCompletionItems(tokens, wordToComplete);
    }

    private IEnumerable<string> GetCompletionItems(string[] tokens, string wordToComplete)
    {
        //コマンド入力なし
        if (tokens.Length == 0)
        {
            return GetCommandNames("");
        }
        //最初の要素がコマンド名
        var cmd = Items.FirstOrDefault(x => tokens[0] == x.CommandName);
        if (cmd == null)
        {
            //コマンド未入力 or 入力途中の場合、コマンド名一覧を表示
            return GetCommandNames(wordToComplete);
        }
        //入力中のコマンドからオプション一覧を取得
        var options = cmd.GetOptions().ToArray();
        if(tokens.Length == 1) //コマンドのみ指定済み
        {
            return GetOptionNames(wordToComplete, options);
        }
        
        var op1 = options.FirstOrDefault(o => o == tokens[^1]);
        //最後の入力値がオプションと一致
        if (op1 != null)
        {
            //オプションに対する入力候補を取得
            return GetCompletionItemByOptionName(cmd, op1, tokens, options, wordToComplete);
        }

        var op2 = options.FirstOrDefault(o => o == tokens[^2]);
        //後ろから２つ目がオプションの場合
        if (op2 != null && wordToComplete != "")
        {
            //オプションに対する入力候補を取得
            return GetCompletionItemByOptionName(cmd, op2, tokens, options, wordToComplete);
        }
        
        return GetOptionNames(wordToComplete, options.Where(o=>!tokens.Contains(o)).ToArray());
    }

    private static IEnumerable<string> GetCompletionItemByOptionName(
        ICommandCompletionItem cmd, string optionName, string[] tokens, string[] options, string wordToComplete)
    {
        //オプションに対する入力候補を取得
        var item = cmd.GetCompletionItems(optionName, wordToComplete);
        if (!item.Any())
        {
            //値の候補なしの場合オプションを候補として出す
            return GetOptionNames(wordToComplete, options.Where(o => !tokens.Contains(o)).ToArray());
        }
        //入力中の文字列でフィルタ
        return item.Where(x => x.Contains(wordToComplete,StringComparison.InvariantCultureIgnoreCase));
    }

    private static IEnumerable<string> GetOptionNames(string wordToComplete, string[] options)
    {
        if (wordToComplete == "")
        {
            return options;
        }
        return options.Where(o => o.Contains(wordToComplete));
    }

    private IEnumerable<string> GetCommandNames(string wordToComplete)
    {
        if (wordToComplete == "")
        {
            return Items.Select(x => x.CommandName);
        }
        return Items.Where(x => x.CommandName.Contains(wordToComplete)).Select(x => x.CommandName);
    }

    private static IEnumerable<string> Parse(string input, int position)
    {
        var source = input[0..Math.Min(input.Length, position)];
        var token = "";
        var inQuote = false;
        var isEscaped = false;
        for (int i = 0; i < source.Length; i++)
        {
            var c = source[i];
            if (isEscaped)
            {
                token += c;
                isEscaped = false;
                continue;
            }
            switch (c)
            {
                case '"':
                    inQuote = !inQuote;
                    break;
                case '\\':
                    isEscaped = true;
                    break;
                case ' ':
                    if (inQuote)
                    {
                        token += c;
                    }
                    else if (!string.IsNullOrEmpty(token))
                    {
                        yield return token;
                        token = "";
                    }
                    break;
                default:
                    token += c;
                    break;
            }
        }
        if (!string.IsNullOrEmpty(token))
        {
            yield return token;
        }
    }

}