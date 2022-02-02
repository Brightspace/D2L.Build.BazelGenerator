# D2L.Build.BazelGenerator

Extracted helper for generating base [Brightspace/rules_csharp][rules_csharp]
rules for .NET Framework reference assemblies.

```powershell
nuget install `
  Microsoft.NETFramework.ReferenceAssemblies.net48 `
  -OutputDirectory 'C:\reference-assemblies' `
  -ExcludeVersion

dotnet run `
  --project D2L.Build.BazelGenerator/D2L.Build.BazelGenerator.csproj `
  net48 `
  'C:\reference-assemblies\Microsoft.NETFramework.ReferenceAssemblies.net48' `
  'build\.NETFramework\v4.8'
```

[rules_csharp]: https://github.com/Brightspace/rules_csharp
