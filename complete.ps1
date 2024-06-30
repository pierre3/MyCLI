# PowerShell parameter completion shim for the mycli
Register-ArgumentCompleter -Native -CommandName mycli -ScriptBlock {
    param($wordToComplete, $commandAst, $cursorPosition)
    mycli complete --word-to-complete $wordToComplete --input "$commandAst" --cursor-position $cursorPosition | ForEach-Object {
        [System.Management.Automation.CompletionResult]::new($_, $_, 'ParameterValue', $_)
    }
}
