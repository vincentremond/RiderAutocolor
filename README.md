# RiderAutocolor

## About

This program modifies the JetBrains Rider solution configuration file (workspace.xml) to display each project of a different color.

Inspiration from Visual Studio's [Custom Document Well](https://marketplace.visualstudio.com/items?itemName=VisualStudioPlatformTeam.CustomDocumentWell) extension.

## Screenshot

![Screenshot of the result](screenshot.png)

## Usage

Normal usage :

    dotnet run -- --sln "PATH_TO_SLN_FILE"

Whole directory usage :

    dotnet run -- --directory "PATH_TO_DIRECTORY"

Show help :

    dotnet run -- --help

## Examples

    dotnet run -- --sln "D:\GIT\Main.Library\Library.sln"
    dotnet run -- --directory "D:\GIT\"
