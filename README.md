# Zapp
Zapp is a Windows process orchestrator for packed apps.

| | |
| --- | --- |
| **Build** | [![Build status](https://ci.appveyor.com/api/projects/status/sv453plywnnulnf9?svg=true)](https://ci.appveyor.com/project/OmniaRetail/zapp) |
| **NuGet** | [![NuGet](https://buildstats.info/nuget/Zapp)](https://www.nuget.org/packages/Zapp/) |
| **Gitter** | not yet |

## Installation

Install the NuGet package using the command below,

```
Install-Package Zapp
```

. . . or search for `Zapp` in the NuGet index.

## Getting started
The code below is an example how to use the library.

```
using Zapp;
using Zapp.Rest;
using Zapp.Pack;

var server = new ZappServer(
    new OwinRestService("http://localhost:6464/"),	// Instance of: IRestService
	new ZipPackageService(),						// Instance of: IPackageService
);

server.Start();
```