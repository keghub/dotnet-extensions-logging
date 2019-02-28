[![Build status](https://ci.appveyor.com/api/projects/status/rifbfduro49d6iyi/branch/master?svg=true)](https://ci.appveyor.com/project/EMG/dotnet-extensions-logging/branch/master) ![EMG Logging Extensions](https://img.shields.io/nuget/v/emg.extensions.logging.loggly.svg)


# EMG Logging Extensions
The **EMG Logging Extensions** are a set of libraries that extend the [Microsoft logging extensions](https://github.com/aspnet/Extensions) to provide additional functionalities.

## EMG Logging Extensions for Loggly
The [EMG Logging Extensions for Loggly](./docs/loggly.md) is an opinionated library that helps delivering structured logs to [Loggly](https://www.loggly.com/).

## Versioning
The projects in this repository follows [Semantic Versioning 2.0.0](http://semver.org/spec/v2.0.0.html) for the public releases (published to the [nuget.org](https://www.nuget.org/)).

## Building
This project uses [CAKE](https://cakebuild.net/) as a build engine.
* [Here you can see the build history](https://ci.appveyor.com/project/EMG/dotnet-extensions-logging/history)

If you would like to build Nybus locally, just execute the `build.cake` script.

You can do it by using the .NET tool created by CAKE authors and use it to execute the build script.
```powershell
dotnet tool install -g Cake.Tool
dotnet cake
```

Many thanks to [AppVeyor](http://www.appveyor.com/) for their support to the .NET Open Source community.