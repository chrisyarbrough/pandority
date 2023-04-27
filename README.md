# Pandority

A Roslyn source generator for Unity projects, which adds the extension method `HasFlagNonAlloc` to user-defined enums.
This custom method provides improved performance compared to `System.Enum.HasFlag` by avoiding boxing allocations.

![img](Documentation~/ProfilerSample.png)

## Requirements

**To use**

- Unity 2021.3 or newer

**To develop**

- JetBrains Rider or Visual Studio
- Consult the [Unity documentation](https://docs.unity3d.com/2021.3/Documentation/Manual/roslyn-analyzers.html)

## Usage

Install the package to your Unity project. The source generator will be automatically configured during import.

To generate the extension code for **enums**:

- Declare the `PandorityTarget` attribute on every assembly that should be visible to the generator
- Apply the `System.Flags` attribute to the desired enum types

### Example

```csharp
[assembly: PandorityTarget]

[System.Flags]
public enum Element
{
    Fire = 1,
    Water = 2,
    Earth = 4,
    Air = 8,
}

public class GettingStarted
{
    public GettingStarted()
    {
        Element element = Element.Fire | Element.Earth;
        bool hasFire = element.HasFlagNonAlloc(Element.Fire);
        bool hasWater = element.HasFlagNonAlloc(Element.Water);
        bool hasEarth = element.HasFlagNonAlloc(Element.Earth);
        bool hasAir = element.HasFlagNonAlloc(Element.Air);
    }
}
```

## Limitations

The generated extension method is named `HasFlagNonAlloc` instead of `HasFlag` to avoid being hidden
by the builtin `System.Enum.HasFlag` instance method. In C#, instance methods take precedence over extension methods.
However, consider that the new name is also clearer about the fact that it's a performance optimization.

## Troubleshooting

If the source generator fails to run, check if a crash log was generated in these locations:

| Operating System | Log File Location          |
|-----------------:|:---------------------------|
|            macOS | `~/Library/Logs/Pandority` |
|          Windows | `%LOCALAPPDATA%\Pandority` |
|            Linux | `~/.config/Pandority`      |

Run `sh ./open-log.sh` (macOS) to open the log file with a default application (e.g. Console.app).

## Implementation Notes

The package comes with a Unity AssetPostprocessor which automatically configures the `Pandority.dll` importer.
As a "bonus" feature, it will also apply to any DLL whose name ends with `SourceGenerator.dll`.

## Development

Open the _SourceGeneratorProject_ directory in your IDE to modify the source generator.
Review the contents of the .csproj file to see the configuration and build process details.

Run `nuget restore` to install the required NuGet packages.

Run `dotnet build` to produce a new DLL that will be automatically copied to the Unity package directory.
