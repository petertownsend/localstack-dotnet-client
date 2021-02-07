# LocalStack .Net Core and .Net Framework Client

![LocalStack](https://github.com/localstack-dotnet/localstack-dotnet-client/blob/master/assets/localstack-dotnet.png?raw=true)

This is an easy-to-use .NET client for [LocalStack](https://github.com/localstack/localstack).
The client library provides a thin wrapper around [aws-sdk-net](https://github.com/aws/aws-sdk-net) which
automatically configures the target endpoints to use LocalStack for your local cloud
application development.

## Continuous integration

| Build server    	| Platform 	| Build status                                                                                                                                                                                                                                                                         	|
|-----------------	|----------	|--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------	|
| Azure Pipelines 	| Ubuntu   	| [![Build Status](https://denizirgindev.visualstudio.com/localstack-dotnet-client/_apis/build/status/Ubuntu?branchName=master)](https://denizirgindev.visualstudio.com/localstack-dotnet-client/_build/latest?definitionId=8&branchName=master) 	|
| Azure Pipelines 	| macOs   	| [![Build Status](https://denizirgindev.visualstudio.com/localstack-dotnet-client/_apis/build/status/macOS?branchName=master)](https://denizirgindev.visualstudio.com/localstack-dotnet-client/_build/latest?definitionId=10&branchName=master) 	|
| Azure Pipelines 	| Windows   	| [![Build Status](https://denizirgindev.visualstudio.com/localstack-dotnet-client/_apis/build/status/Windows?branchName=master)](https://denizirgindev.visualstudio.com/localstack-dotnet-client/_build/latest?definitionId=9&branchName=master)	|

## Table of Contents

1. [Supported Platforms](#supported-platforms)
2. [Prerequisites](#prerequisites)
3. [Installation](#installation)
4. [Usage](#usage)
    - [LocalStack.Client.Extensions (Recommended)](#localstack-client-extensions)
        - [Installation](#extensions-installation)
        - [Usage](#extensions-usage)
        - [About AddAwsService](#extensions-usage-about-addawsservice)
    - [Standalone Initialization](#standalone-initialization)
    - [Microsoft.Extensions.DependencyInjection Initialization](#di)
5. [Developing](#developing)
6. [License](#license)

## <a name="supported-platforms"></a> Supported Platforms

* [.NET 5.0](https://dotnet.microsoft.com/download/dotnet/5.0)
* [.NET Standard 2.0](https://docs.microsoft.com/en-us/dotnet/standard/net-standard)
* [.NET 4.6.1 and Above](https://dotnet.microsoft.com/download/dotnet-framework)

## <a name="prerequisites"></a> Prerequisites

To make use of this library, you need to have [LocalStack](https://github.com/localstack/localstack)
installed on your local machine. In particular, the `localstack` command needs to be available.

## <a name="installation"></a>  Installation

The easiest way to install *LocalStack .NET Client* is via `nuget`:

```
Install-Package LocalStack.Client
```

Or use `dotnet cli`

```
dotnet add package LocalStack.Client
```

| Stable                                                                                                              | Nightly                                                                                                                                                                        |
|---------------------------------------------------------------------------------------------------------------------|--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| [![NuGet](https://img.shields.io/nuget/v/LocalStack.Client.svg)](https://www.nuget.org/packages/LocalStack.Client/) | [![MyGet](https://img.shields.io/myget/localstack-dotnet-client/v/LocalStack.Client.svg?label=myget)](https://www.myget.org/feed/localstack-dotnet-client/package/nuget/LocalStack.Client) |

## <a name="usage"></a> Usage

This library provides a thin wrapper around [aws-sdk-net](https://github.com/aws/aws-sdk-net). 
Therefore the usage of this library is same as using `AWS SDK for .NET`.

See [Getting Started with the AWS SDK for .NET](https://docs.aws.amazon.com/sdk-for-net/v3/developer-guide/net-dg-setup.html)

This library can be used with any DI library, [AWSSDK.Extensions.NETCore.Setup](https://docs.aws.amazon.com/sdk-for-net/latest/developer-guide/net-dg-config-netcore.html) or it can be used as standalone.

### <a name="localstack-client-extensions"></a>  LocalStack.Client.Extensions (Recommended)

[LocalStack.Client.Extensions](https://www.nuget.org/packages/LocalStack.Client/) is extensions for the LocalStack.NET Client to integrate with .NET Core configuration and dependency injection frameworks. The extensions also provides wrapper around [AWSSDK.Extensions.NETCore.Setup](https://docs.aws.amazon.com/sdk-for-net/latest/developer-guide/net-dg-config-netcore.html) to use both LocalStack and AWS side-by-side.

This approach is recommended since `AWSSDK.Extensions.NETCore.Setup` is very popular and also it is best practice for using [AWSSDK.NET](https://aws.amazon.com/sdk-for-net/) with .NET Core or .NET 5

#### <a name="extensions-installation"></a>  Installation

The easiest way to install *LocalStack .NET Client Extensions* is via `nuget`:

```
Install-Package LocalStack.Client.Extensions
```

Or use `dotnet cli`

```
dotnet add package LocalStack.Client.Extensions  
```

#### <a name="extensions-usage"></a> Usage

The usage is very similar to `AWSSDK.Extensions.NETCore.Setup` with some differences.

```csharp
public void ConfigureServices(IServiceCollection services)
{
    // Add framework services.
    services.AddMvc();

    services.AddLocalStack(Configuration)
    services.AddDefaultAWSOptions(Configuration.GetAWSOptions());
    services.AddAwsService<IAmazonS3>();
    services.AddAwsService<IAmazonDynamoDB>();
}
```

The most important difference is that `AddAwsService` extensions method is used instead of `AddAWSService` used in `AWSSDK.Extensions.NETCore.Setup`. The reason for this will be explained later in this section. 
In addition, the `AddLocalStack` extension method is also used.

`AddLocalStack` extension method is responsible for both configurations and adding of `LocalStack.Client` dependencies to service collection.

You can configure `LocalStack.Client` by using entries in the `appsettings.json` files, as shown in the following example.

```json
"LocalStack": {
    "UseLocalStack": true,
    "Session": {
        "AwsAccessKeyId": "my-AwsAccessKeyId",
        "AwsAccessKey": "my-AwsAccessKey",
        "AwsSessionToken": "my-AwsSessionToken",
        "RegionName": "eu-central-1"
    },
    "Config": {
        "LocalStackHost": "localhost",
        "UseSsl": false,
        "UseLegacyPorts": false,
        "EdgePort": 4566
    }
}
```

All the entries above are has shown with default values (except `UseLocalStack`, it's `false` by default). 
So the above entries do not need to be specified. 

What is entered for the aws credential values ​​in the `Session` section does not matter for LocalStack. `RegionName` is important since LocalStack creates resources by spesified region.

`Config` section contains important entries for local development. Starting with LocalStack releases after `v0.11.5`, all services are now exposed via the edge service (port 4566) only! If you are using a version of LocalStack lower than v0.11.5, you should set `UseLegacyPorts` to `true`. Edge port can be set to any available port ([see LocalStack configuration section](https://github.com/localstack/localstack#configurations)). If you have made such a change in LocalStack's configuration, be sure to set the same port value to `EdgePort` in the `Config` section. For `LocalStackHost` and `UseSsl` entries, ​​corresponding to the [LocalStack configuration](https://github.com/localstack/localstack#configurations) should be used.

The following sample setting files can be used to use both `LocalStack.Client` and` AWSSDK.Extensions.NETCore.Setup` in different environments.

`appsettings.Development.json`
```json
"LocalStack": {
    "UseLocalStack": true,
    "Session": {
        ...
    },
    "Config": {
        ...
    }
}
```

`appsettings.Production.json`
```json
"LocalStack": {
    "UseLocalStack": false
},
"AWS": {
    "Profile": "<your aws profile>",
    "Region": "eu-central-1"
}
```

See project [LocalStack.Client.Sandbox.WithGenericHost](https://github.com/localstack-dotnet/localstack-dotnet-client/tree/master/tests/sandboxes/LocalStack.Client.Sandbox.WithGenericHost) for a use case.

#### <a name="extensions-usage-about-addawsservice"></a> About AddAwsService

`AddAwsService` is equivalent of `AddAWSService` used in `AWSSDK.Extensions.NETCore.Setup`. It decides which factory to use when resolving any AWS Service. To decide this, it checks the `UseLocalStack` entry. 
If the `UseLocalStack` entry is `true`, it uses the [Session](https://github.com/localstack-dotnet/localstack-dotnet-client/blob/master/src/LocalStack.Client/Session.cs) class of `LocalStack.Client` to create AWS Service. If the `UseLocalStack` entry is `false`, it uses the [ClientFactory](https://github.com/aws/aws-sdk-net/blob/master/extensions/src/AWSSDK.Extensions.NETCore.Setup/ClientFactory.cs) class of `AWSSDK.Extensions.NETCore.Setup` which is also used by original `AddAWSService`.

It is named as `AddAwsService` to avoid name conflict with `AddAWSService`.


### <a name="standalone-initialization"></a> Standalone Initialization

If you do not want to use any DI library, you have to instantiate `SessionStandalone` as follows. 

```csharp
/*
* ==== Default Values ====
* AwsAccessKeyId: accessKey (It doesn't matter to LocalStack)
* AwsAccessKey: secretKey (It doesn't matter to LocalStack)
* AwsSessionToken: token (It doesn't matter to LocalStack)
* RegionName: us-east-1
* ==== Custom Values ====
* var sessionOptions = new SessionOptions("someAwsAccessKeyId", "someAwsAccessKey", "someAwsSessionToken", "eu-central-");
*/
var sessionOptions = new SessionOptions();

/*
* ==== Default Values ====
* LocalStackHost: localhost
* UseSsl: false
* UseLegacyPorts: false (Set true if your LocalStack version is 0.11.5 or above)
* EdgePort: 4566 (It doesn't matter if use legacy ports)
* ==== Custom Values ====
* var configOptions = new ConfigOptions("mylocalhost", false, false, 4566);
*/
var configOptions = new ConfigOptions();

ISession session = SessionStandalone.Init()
                                .WithSessionOptions(sessionOptions)
                                .WithConfigurationOptions(configOptions).Create();

var amazonS3Client = session.CreateClientByImplementation<AmazonS3Client>();
```
`CreateClientByInterface<TSerice>` method can also be used to create AWS service, as follows

```csharp
var amazonS3Client = session.CreateClientByInterface<IAmazonS3>();
```

### <a name="di"></a>  Microsoft.Extensions.DependencyInjection Initialization

First, you need to install `Microsoft.Extensions.DependencyInjection` nuget package as follows

```
dotnet add package Microsoft.Extensions.DependencyInjection
```

Register necessary dependencies to `ServiceCollection` as follows

```csharp
var collection = new ServiceCollection();

/*
* ==== Default Values ====
* AwsAccessKeyId: accessKey (It doesn't matter to LocalStack)
* AwsAccessKey: secretKey (It doesn't matter to LocalStack)
* AwsSessionToken: token (It doesn't matter to LocalStack)
* RegionName: us-east-1
* ==== Custom Values ====
* var sessionOptions = new SessionOptions("someAwsAccessKeyId", "someAwsAccessKey", "someAwsSessionToken", "eu-central-");
*/
var sessionOptions = new SessionOptions();

/*
* ==== Default Values ====
* LocalStackHost: localhost
* UseSsl: false
* UseLegacyPorts: false (Set true if your LocalStack version is 0.11.5 or above)
* EdgePort: 4566 (It doesn't matter if use legacy ports)
* ==== Custom Values ====
* var configOptions = new ConfigOptions("mylocalhost", false, false, 4566);
*/
var configOptions = new ConfigOptions();

collection
    .AddScoped<ISessionOptions, SessionOptions>(provider => sessionOptions)
    .AddScoped<IConfigOptions, ConfigOptions>(provider => configOptions))
    .AddScoped<IConfig, Config>()
    .AddSingleton<ISessionReflection, SessionReflection>()
    .AddSingleton<ISession, Session>()
    .AddTransient<IAmazonS3>(provider =>
    {
        var session = provider.GetRequiredService<ISession>();

        return (IAmazonS3) session.CreateClientByInterface<IAmazonS3>();
    });

ServiceProvider serviceProvider = collection.BuildServiceProvider();

var amazonS3Client = serviceProvider.GetRequiredService<IAmazonS3>();
```

If you want to use it with `ConfigurationBuilder`, you can also choose a usage as below.

```csharp
var collection = new ServiceCollection();
var builder = new ConfigurationBuilder();

builder.SetBasePath(Directory.GetCurrentDirectory());
builder.AddJsonFile("appsettings.json", true);
builder.AddJsonFile("appsettings.Development.json", true);
builder.AddEnvironmentVariables();
builder.AddCommandLine(args);

IConfiguration configuration = builder.Build();

collection.Configure<LocalStackOptions>(options => configuration.GetSection("LocalStack").Bind(options, c => c.BindNonPublicProperties = true));
/*
* ==== Default Values ====
* AwsAccessKeyId: accessKey (It doesn't matter to LocalStack)
* AwsAccessKey: secretKey (It doesn't matter to LocalStack)
* AwsSessionToken: token (It doesn't matter to LocalStack)
* RegionName: us-east-1
    */
collection.Configure<SessionOptions>(options => configuration.GetSection("LocalStack")
                                                                .GetSection(nameof(LocalStackOptions.Session))
                                                                .Bind(options, c => c.BindNonPublicProperties = true));
/*
    * ==== Default Values ====
    * LocalStackHost: localhost
    * UseSsl: false
    * UseLegacyPorts: false (Set true if your LocalStack version is 0.11.5 or above)
    * EdgePort: 4566 (It doesn't matter if use legacy ports)
    */
collection.Configure<ConfigOptions>(options => configuration.GetSection("LocalStack")
                                                            .GetSection(nameof(LocalStackOptions.Config))
                                                            .Bind(options, c => c.BindNonPublicProperties = true));


collection.AddTransient<IConfig, Config>(provider =>
{
    ConfigOptions options = provider.GetRequiredService<IOptions<ConfigOptions>>().Value;

    return new Config(options);
})
.AddSingleton<ISessionReflection, SessionReflection>()
.AddSingleton<ISession, Session>(provider =>
{
    SessionOptions sessionOptions = provider.GetRequiredService<IOptions<SessionOptions>>().Value;
    var config = provider.GetRequiredService<IConfig>();
    var sessionReflection = provider.GetRequiredService<ISessionReflection>();

    return new Session(sessionOptions, config, sessionReflection);
})
.AddTransient<IAmazonS3>(provider =>
{
    var session = provider.GetRequiredService<ISession>();

    return (IAmazonS3) session.CreateClientByInterface<IAmazonS3>();
});

ServiceProvider serviceProvider = collection.BuildServiceProvider();

var amazonS3Client = serviceProvider.GetRequiredService<IAmazonS3>();
```

## <a name="developing"></a> Developing

We welcome feedback, bug reports, and pull requests!

Use these commands to get you started and test your code:

Windows
```
build.ps1
```

Linux
```
./build.sh
```

<!-- ## Changelog

* v0.8: Add more service endpoint mappings that will be implemented in the near future -->

## <a name="license"></a> License
Licensed under MIT, see [LICENSE](LICENSE) for the full text.
