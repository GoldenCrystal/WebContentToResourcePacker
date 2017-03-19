# WebContentToResourcePacker
This project provide a means for ASP.NET Core applications to bundle all their
web content (typically in wwwroot) into a single embedded resource.
The files can then be read using the `ResourceManager` class.

Currently, the package can only be used to build using the full .NET Framework,
as the classes needed to write resources are yet unavaiblable in .NET Standard.

# How to use
Add the nuget package `WebContentToResourcePacker`, and put the following
fragment in your .csproj:
```xml
<ItemGroup>
  <WebContent Include="wwwroot\**\*.*">
    <LogicalName>%(RecursiveDir)%(FileName)%(Extension)</LogicalName>
  </WebContent>
  <Content Remove="wwwroot\**\*.*" />
</ItemGroup>
```
This will tell the tooling that all files under `wwwroot` are `WebContent`
items and remove them from the `Content` item group.
The MSBuild target embedded in this NuGet package will then pick the files up
and wrap them in an embedded resource during the build.