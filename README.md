# Azure App Service SDK for Unity3D

The LightBuzz Azure App Service SDK for Unity3D is a framework that allows you to consume remote Azure services and even store data locally. It's secured using HTTPS. Supports Android, iOS, Windows Standalone, MacOS, and UWP.

_It's an SDK you can actually use in your apps and games without compatibility problems._

![lightbuzz-azure-unity](https://user-images.githubusercontent.com/562680/39691509-70b6b07e-51e6-11e8-8111-eaa171308999.png)

## Download

To use the SDK, you can either clone the current repository or download the .unitypackage file from the [Releases](https://github.com/LightBuzz/Azure-Unity/releases/) section.

> :inbox_tray: [Download](https://github.com/LightBuzz/Azure-Unity/releases/latest) the latest stable version.

## Features

### Azure App Services

The **LightBuzz Azure SDK for Unity** consumes [Azure App Service APIs](http://azure.microsoft.com/en-us/documentation/articles/app-service-api-apps-why-best-platform/) and supports local database storage.

### HTTPS

The LightBuzz SDK is built with security in mind. The native Microsoft ```HttpClient``` modules do not support HTTPS in Unity. Our team has built the ```HttpClient``` from scratch to support HTTPS, providing your data with a secure connection.

### Local Database

Unlike most of the available SDKs, the LightBuzz Azure SDK for Unity fully supports local database storage. This means you can use the Azure App Services to store data into a local SQLite database. You can sync (pull/push) your local data with the remote server.

As a result, your customers can use your app/game without an active Internet connection.

## Supported Platforms

The LightBuzz Azure SDK for Unity supports the following platforms:

* Unity Editor
* Android
* iOS
* Windows Desktop (Standalone)
* MacOS (Standalone)
* Universal Windows Platform (UWP)

## Requirements

### Unity version

To use the SDK, you need **[Unity 2017.1 or later](https://store.unity.com/)**. 

The SDK is built with the latest C# features, so you need to use the **.NET 4.6 Scripting Runtime version**.

![unity-2017-scripting-runtime](https://user-images.githubusercontent.com/562680/40273209-dd7c0578-5bc4-11e8-8c1b-adf2c0d0e69a.png)

_If using Unity 2018, the scripting runtime is set to 4.6 by default._

![unity-2018-scripting-runtime](https://user-images.githubusercontent.com/562680/40273190-81339a74-5bc4-11e8-965e-1e5bfb0d8de0.png)

### Scripting Backend

The SDK works with the following Scripting Backend options:

| Platform | Scripting Backend |
| ------- | ------- |
| Standalone | Mono |
| Android | Mono |
| iOS | IL2CPP |
| UWP | .NET |

## How to use

In the included samples, we have created a simple demo that implements Microsoft's [ToDo List example](https://azure.microsoft.com/en-us/resources/samples/app-service-api-dotnet-todo-list/).

We have implemented a generic **Data Access Object** for you to use, called ```MobileAppsTableDAO```. The ```MobileAppsTableDAO``` supports all of the common operations out-of-the-box. All you need to do is call the proper C# methods.

Using the code is fairly simple:

### Initialization

```
private MobileServiceClient azureClient;
private MobileAppsTableDAO<TodoItem> todoTableDAO;

private async Task Init()
{
    azureClient = new MobileServiceClient(mobileAppUri, new LightBuzzHttpsHandler());
    todoTableDAO = new MobileAppsTableDAO<TodoItem>(azureClient);

    if (todoTableDAO.SupportsLocalStore)
    {
        await LocalStore.Init(azureClient);
        await LocalStore.Sync();
    }
}
```

### Get

```
private async Task Get()
{
    List<TodoItem> list = await todoTableDAO.FindAll();

    foreach (TodoItem item in list)
    {
        Debug.Log("Text: " + item.Text);
    }
}
```

### Insert

```
private async Task Insert()
{
    TodoItem item = new TodoItem
    {
        Text = "Hello World!"
    };

    await todoTableDAO.Insert(item);
}
```

### Delete

```
private async Task Delete()
{
    List<TodoItem> list = await todoTableDAO.FindAll();

    TodoItem item = list.LastOrDefault();

    if (item != null)
    {
        await todoTableDAO.Delete(item);
    }
}
```

### Sync local and remote data

```
private async Task Sync()
{
    if (todoTableDAO.SupportsLocalStore)
    {
        await LocalStore.Sync();
    }
}
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
| **App Services** | ![check](https://user-images.githubusercontent.com/562680/39688758-f10e1352-51dc-11e8-9327-e428bc0eeb02.png) | ![check](https://user-images.githubusercontent.com/562680/39688758-f10e1352-51dc-11e8-9327-e428bc0eeb02.png) | ![check](https://user-images.githubusercontent.com/562680/39688758-f10e1352-51dc-11e8-9327-e428bc0eeb02.png) |
| **HTTPS** | ![check](https://user-images.githubusercontent.com/562680/39688758-f10e1352-51dc-11e8-9327-e428bc0eeb02.png) | ![check](https://user-images.githubusercontent.com/562680/39688758-f10e1352-51dc-11e8-9327-e428bc0eeb02.png) | ![close](https://user-images.githubusercontent.com/562680/39688759-f14063ac-51dc-11e8-929c-c6625252f285.png) |
| **Local database** | ![check](https://user-images.githubusercontent.com/562680/39688758-f10e1352-51dc-11e8-9327-e428bc0eeb02.png) | ![close](https://user-images.githubusercontent.com/562680/39688759-f14063ac-51dc-11e8-929c-c6625252f285.png) | ![close](https://user-images.githubusercontent.com/562680/39688759-f14063ac-51dc-11e8-929c-c6625252f285.png) |
| **Unity Editor** | ![check](https://user-images.githubusercontent.com/562680/39688758-f10e1352-51dc-11e8-9327-e428bc0eeb02.png) | ![check](https://user-images.githubusercontent.com/562680/39688758-f10e1352-51dc-11e8-9327-e428bc0eeb02.png) | ![check](https://user-images.githubusercontent.com/562680/39688758-f10e1352-51dc-11e8-9327-e428bc0eeb02.png) |
| **Windows Desktop** | ![check](https://user-images.githubusercontent.com/562680/39688758-f10e1352-51dc-11e8-9327-e428bc0eeb02.png) | ![check](https://user-images.githubusercontent.com/562680/39688758-f10e1352-51dc-11e8-9327-e428bc0eeb02.png) | ![check](https://user-images.githubusercontent.com/562680/39688758-f10e1352-51dc-11e8-9327-e428bc0eeb02.png) |
| **Mac OS X** | ![check](https://user-images.githubusercontent.com/562680/39688758-f10e1352-51dc-11e8-9327-e428bc0eeb02.png) | ![close](https://user-images.githubusercontent.com/562680/39688759-f14063ac-51dc-11e8-929c-c6625252f285.png) | ![close](https://user-images.githubusercontent.com/562680/39688759-f14063ac-51dc-11e8-929c-c6625252f285.png) |
| **Android** | ![check](https://user-images.githubusercontent.com/562680/39688758-f10e1352-51dc-11e8-9327-e428bc0eeb02.png) | ![check](https://user-images.githubusercontent.com/562680/39688758-f10e1352-51dc-11e8-9327-e428bc0eeb02.png) | ![check](https://user-images.githubusercontent.com/562680/39688758-f10e1352-51dc-11e8-9327-e428bc0eeb02.png) |
| **iOS** | ![check](https://user-images.githubusercontent.com/562680/39688758-f10e1352-51dc-11e8-9327-e428bc0eeb02.png) | ![check](https://user-images.githubusercontent.com/562680/39688758-f10e1352-51dc-11e8-9327-e428bc0eeb02.png) | ![check](https://user-images.githubusercontent.com/562680/39688758-f10e1352-51dc-11e8-9327-e428bc0eeb02.png) |
| **UWP** | ![check](https://user-images.githubusercontent.com/562680/39688758-f10e1352-51dc-11e8-9327-e428bc0eeb02.png) | ![close](https://user-images.githubusercontent.com/562680/39688759-f14063ac-51dc-11e8-929c-c6625252f285.png) | ![check](https://user-images.githubusercontent.com/562680/39688758-f10e1352-51dc-11e8-9327-e428bc0eeb02.png) |

## Resources

* [Getting started with Unity3D](https://unity3d.com/learn/)
* [Getting started with App Services](http://azure.microsoft.com/documentation/articles/app-service-api-dotnet-get-started/)
* [Azure Portal](https://portal.azure.com/)
