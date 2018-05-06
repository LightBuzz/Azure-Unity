# Azure App Service SDK for Unity3D

The LightBuzz Azure App Service SDK for Unity3D is a framework that allows you to consume remote Azure services and even store data locally. It's secured using HTTPS. Supports Android, iOS, Windows Standalone, MacOS, and UWP.

![lightbuzz-azure-unity](https://user-images.githubusercontent.com/562680/39676374-63a5e20c-5172-11e8-964a-aaa0246a3464.png)

## Download

To use the SDK, you can either clone the current repository or download the .unitypackage file from the [Releases](https://github.com/LightBuzz/Azure-Unity/releases/) section.

> :inbox_tray: [Download](https://github.com/LightBuzz/Azure-Unity/releases/download/v1.0.0/lightbuzz-azure-unity-1.0.0.unitypackage) the latest stable version.

> _For older versions, check the [Releases](https://github.com/LightBuzz/Azure-Unity/releases/) page._

## Features

### Azure App Services

The **LightBuzz Azure SDK for Unity** consumes [Azure App Service APIs](http://azure.microsoft.com/en-us/documentation/articles/app-service-api-apps-why-best-platform/) and supports local database storage.

### HTTPS

The LightBuzz SDK is built with security in mind. The native Microsoft ```HttpClient``` modules do not support HTTPS in Unity. Our team has built the ```HttpClient``` from scratch to support HTTPS, providing your data with a secure connection.

### Local Store

Unlike most of the available SDKs, the LightBuzz Azure SDK for Unity fully supports local database storage. This means you can use the Azure App Services to store data into a local SQLite database. You can sync (pull/push) your local data with the remote server.

As a result, your customers can use your app/game without an active Internet connection.

## Supported Platforms

The LightBuzz Azure SDK for Unity supports the following platforms:

* Android
* iOS
* Windows Standalone
* MacOS
* UWP (coming soon)

## How to use

In the included samples, we have created a simple demo that implements Microsoft's [ToDo List example](https://azure.microsoft.com/en-us/resources/samples/app-service-api-dotnet-todo-list/).

Using the code is fairly simple:

```
```

## Contributors

The SDK is brought to you by [LightBuzz Inc.](https://lightbuzz.com), a New York based company.

* [Georgia Makoudi](https://lightbuzz.com/author/georgia/), Azure Specialist
* [Vangos Pterneas](https://lightbuzz.com/author/vangos/), Microsoft MVP
* [George Karakatsiotis](https://lightbuzz.com/about/), AI Scientist

## Comparison with other SDKs

Similar to our SDK, there are a couple other SDKs available. The most popular ones are the following:

* DeadlyFingers' SDK [Unity3D Azure AppServices](https://github.com/Unity3dAzure/AppServices)
* Brian Peek's SDK [Azure SDK for Unity3D](https://github.com/BrianPeek/AzureSDKs-Unity)

|         | LightBuzz SDK | DeadlyFingers SDK | Brian Peek SDK |
| ------- | ------- | ------- | ------- |
| **App Services** | :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: |
| **HTTPS** | :heavy_check_mark: | :heavy_check_mark: | :x: |
| **Local database** | :heavy_check_mark: | :x: | :x: |
| **GET/POST/PUT/DELETE** | :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: |
| **PATCH** | :heavy_check_mark: | :x: | :x: |

## Resources

* [Getting started with Unity3D](https://unity3d.com/learn/)
* [Getting started with App Services](http://azure.microsoft.com/documentation/articles/app-service-api-dotnet-get-started/)
* [Azure Portal](https://portal.azure.com/)
