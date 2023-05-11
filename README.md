# Pandority

Roslyn source generators for Unity projects:

- [Enum HasFlag](Documentation~/EnumHasFlagGenerator.md)
- [Accessors](Documentation~/AccessorsGenerator.md)

## Requirements

**To use**

- Unity 2021.3 or newer

**To develop**

- JetBrains Rider or Visual Studio
- [NET 6.0 SDK](https://dotnet.microsoft.com/en-us/download)

## Setup

Install the package to your Unity project.

The package comes with a Unity AssetPostprocessor which automatically configures the `Pandority.dll` importer.
As a "bonus" feature, it will also apply the correct settings to any DLL whose name ends with `SourceGenerator.dll`.

## Troubleshooting

If you encounter issue during setup,
consult the [Unity documentation](https://docs.unity3d.com/2021.3/Documentation/Manual/roslyn-analyzers.html)

If the source generator fails to produce output, check for a crash log in these locations:

| Operating System | Log File Location          |
|-----------------:|:---------------------------|
|            macOS | `~/Library/Logs/Pandority` |
|          Windows | `%LOCALAPPDATA%\Pandority` |
|            Linux | `~/.config/Pandority`      |

### Script Utilities

The following log helper can be run as a shell script in a bash interpreter:

`sh ./open-log.sh` opens the log file directory.

`sh ./open-log.sh <file>` opens the log with the file name.

If a file is not found, the log directory will be opened instead.
In cases where the log directory does not exist, the script will print its path.

To enable debug logs, refer to the _Development_ section for instructions.

## Development

Open the _SourceGeneratorProject_ directory in your IDE to modify the source generator.
Review the contents of the .csproj file to see the configuration and build process details.

Run `nuget restore` to install the required NuGet packages.

Run `dotnet build --configuration Release`
to produce an optimized DLL that will be automatically copied to the Unity package directory.

Run `dotnet build` to build a debug version with logging enabled.

A new release of the generator should always be built with the release configuration for performance reasons.

### Debug Logging

To enable more detailed logging beyond the always-enabled crash log,
set the solution configuration to `Debug` and rebuild the project.

### Running Unit Tests

Run `dotnet test` to execute the unit tests. The test project uses xUnit and NET 6.0.