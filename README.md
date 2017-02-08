# Zapp
[![Build status](https://ci.appveyor.com/api/projects/status/sv453plywnnulnf9?svg=true)](https://ci.appveyor.com/project/OmniaRetail/zapp) [![NuGet](https://buildstats.info/nuget/Zapp)](https://www.nuget.org/packages/Zapp/)

Zapp is a distribution/orchestration service for standalone NuGet packages.  

## Usecase

If you have tightly coupled packages which are automatically deployed and needs to be fused into single applications. 

## Features

- Merge (zip, nupkg) files into standalone apps.
- Orchestration (draining) of apps.

*All features are maintained on-deploy*

## Prerequisites

- [.NET 4.5.1](https://www.microsoft.com/nl-nl/download/details.aspx?id=40773)
- Redis Server ([Windows](https://github.com/MSOpenTech/redis/releases) or [Linux](https://redis.io/download)) *(replaceable)*

## Installation

Install the NuGet package using the command below:

```
Install-Package Zapp
```

...or search for `Zapp` in the NuGet index.

## Getting started

We currently only support `Ninject` to bootstrap:

- [Ninject](Zapp.Example/Bootstrap.cs) example 

## Swagger

Zapp uses [Swagger](http://swagger.io/) to document all the rest-api methods.

`http://localhost:6464/swagger`

## Services

Zapp is split into different services:

| Service | Description |
| --- | --- | 
| [`IRestService`](Zapp/Rest/IRestService.cs) | Service which provides http-method(s) for announcing new deployments. |
| [`IConfigStore`](Zapp/Config/IConfigStore.cs) | Store used for providing configuration to other services. |
| [`ISyncService`](Zapp/Sync/ISyncService.cs) | Service used to verify and synchronize deploy versions across nodes. |
| [`IPackService`](Zapp/Pack/IPackService.cs) | Service used for loading the developer's packaged code. |
| [`IFusionService`](Zapp/Fuse/IFusionService.cs) | Service used fusing the developer's packages into standalone apps. |
| [`IScheduleService`](Zapp/Schedule/IScheduleService.cs) | Service used for orchestrating the standalone apps. | 
| [`IDeployService`](Zapp/Deploy/IDeployService.cs) | Service used for orchestrating the incoming deploymens. | 

### Limitations

The package only uses file-based packing and orchestrating.