# MyCLI

This is a sample that demonstrates how to add an auto-completion feature, which operates with PowerShell, to a CLI tool created with a .NET console application.

## Prerequisites

This sample has been implemented and operationally verified in the following environment:

- .NET 8.0
- PowerShell v7.4.3

## Installation

1. Clone this repository and navigate to the repository folder.
2. Package it using the `dotnet pack` command.
3. Install it as a global tool using the `dotnet tool install` command.

```powershell
PS > dotnet pack
PS > dotnet tool install -g --add-source .\nupkg mycli
```

## Enable Tab Completion
Open your PowerShell profile and paste the following script to enable tab completion.

```powershell
PS > notepad $PROFILE
```

```powershell
# PowerShell parameter completion shim for the mycli
Register-ArgumentCompleter -Native -CommandName mycli -ScriptBlock {
    param($wordToComplete, $commandAst, $cursorPosition)
    mycli complete --word-to-complete $wordToComplete --input "$commandAst" --cursor-position $cursorPosition | ForEach-Object {
        [System.Management.Automation.CompletionResult]::new($_, $_, 'ParameterValue', $_)
    }
}

```

