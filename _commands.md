### Add new class library to the solution
```
$LibName = "Mi5hmasH."
dotnet new classlib -o $LibName
dotnet sln add $LibName

```

### Pack NuGets
```
dotnet build -c Release
dotnet pack -c Release --output .\.NuGets\nupkgs
```

### NuGet Cache Folder
```
%UserProfile%\.nuget\packages
```