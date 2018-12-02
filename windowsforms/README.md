# .NET Core 3.0 Windows Forms Samples
Beginning with [.NET Core 3.0](https://github.com/dotnet/core-sdk#installers-and-binaries), you can build Windows Forms applications. 

## Why build WinForms applications on top of .NET Core?
If you're new to .NET Core, here are a few resources to help you understand the advantages of .NET Core for building Windows applications:
* [Blog: .NET Core 3 and Support for Windows Desktop Applications](https://blogs.msdn.microsoft.com/dotnet/2018/05/07/net-core-3-and-support-for-windows-desktop-applications/)
* [Video: Modernizing Desktop Apps on Windows 10 with .NET Core 3.0 and much more](https://channel9.msdn.com/events/Build/2018/BRK3501?term=scott%20hunter&pubDate=year&lang-en=true)

## Quality disclaimer
.NET Core 3 support for desktop development is not yet in preview. There are early daily builds available supporting WinForms and WPF. You will likely encounter missing tools, bugs, and unexpected behavior. We do not recommend using this SDK and tools for building applications for production scenarios. We do recommend using this SDK and tools to evaluate your how easy it will be to migrate your existing applications, or if you're just interested in trying out the latest upcoming Windows development technology.

## Samples in this repo
| Sample Name | Description |
| ----------- | ----------- |
| [Hello World - shared source](helloworld-sharedsource) | This sample shows you how to share source between a .NET Framework WinForms application and a .NET Core WinForms application. Use this to get the full .NET Framework tooling experience while still building for .NET Core. |
| [Matching Game](matching-game) | This sample demonstrates simple event handling and timers in a .NET Core 3 WinForms application |
| [DataGridView Sample](datagridview) | This sample demonstrates DataGridView usage in .NET Core 3 |
| [Graphics Sample](graphics) | This sample demonstrates using GDI+ APIs via the Graphics type in .NET Core 3 |

## Getting Started

### Prerequisites and getting the tools

Install Visual Studio 2017 Update 15.8 or higher from [https://visualstudio.microsoft.com/downloads/](https://visualstudio.microsoft.com/downloads/), selecting the **.NET desktop development** workload with the options: **.NET Framwork 4.7.2 development tools** and **.NET Core 2.1 development tools**. 

Install the latest [.NET Core 3.0 SDK daily build](https://aka.ms/netcore3sdk) available in the [dotnet/code-sdk repo](https://github.com/dotnet/core-sdk).


### Analyzing your application's for .NET Core 3.0 readiness
If you want to first understand your existing applications readiness for targeting .NET Core 3.0, you can run the .NET Portability Analyzer using the download link and instructions [here](https://blogs.msdn.microsoft.com/dotnet/2018/08/08/are-your-windows-forms-and-wpf-applications-ready-for-net-core-3-0/). This will produce a report that shows you API compatibility for each assembly that your application depends on.

### Creating new .NET Core 3.0 WinForms applications
To create a new application you can use the `dotnet new` command, using the new templates for WinForms.

In your favorite console run:
```cmd
dotnet new winforms -o MyWinFormsApp
cd MyWinFormsApp
dotnet build
dotnet run
```

## Porting existing applications

>We recommend running the [APIPort tool](https://github.com/Microsoft/dotnet-apiport-ui/releases) first to determine if there are any APIs your application depends on that are missing with .NET Core. 

There is no tooling available to help with project migration. In order to migrate your WinForms application, you will create a new project and manually port all of the elements defined in your original project. You will notice the new project is based on the simplified project format, and not everything is migrated. 

### Migrate the head project
Ideally you should migrate all projects in your solution to target .NET Core 3.0 and/or .NET Standard 2.0. The first step to migrate will be to retarget the application's entry point (i.e. 'head' project) and mantain your existing references.

1. Start from a working Solution. You must be able to open the solution in Visual Studio and double check that you can build and run without any issues.
2. If your solution also has server-side projects, such as ASP.NET, we recommend splitting your solution into different server and client solutions. For this effort, work with the client solution only. 
3. Add a new .NET Core 3.0 Windows Forms project to the solution. Adding this project to a sibling folder to your existing 'head' project will make it easier to port references later (using relative paths to other projects or assemblies in the solution)
4.  If your 'head' project uses NuGet packages, you must add the same NuGet packages to the new project. The new SDK-Style projects only support the PackageReference format for adding NuGet package references. If your existing project uses `packages.config`, you must migrate to the new format. You can use the Migrator Tool described [here](https://docs.microsoft.com/nuget/reference/migrate-packages-config-to-package-reference) to automate this process.
6. Copy the `PackageReference` elements generated in the previous step from the original project into the new project's .csproj file.
7. Copy the `ProjectReference` elements from the original project. Note: The new project format does not use the `Name` and `ProjectGuid` elements, so you can safely delete those.
8. At this point it's a good idea to try and restore/build to make sure all dependencies are properly configured.
9. [Link the files](#link-files-from-the-old-project) from your existing .NET Framework WinForms project to the .NET Core 3.0 WinForms project.
10. **Optional** If you have difficulties with compiler linking, you can copy the project files from the .NET Framework WinForms project to the new .NET Core 3.0 WinForms project. 
    * C# files (files with the `.cs.` extension) are included by default in the .csproj.
    * Other project elements like `EmbeddedResources` can also use globbing.

### Migration tips

#### Configure assembly file generation

Most existing projects include an `AssemblyInfo.cs` file in the Properties folder. The new project style uses a different approach and generates the same assembly attributes as part of the build process. To disable that behavior you can add the property:
```xml
<GenerateAssemblyInfo>false</GenerateAssemblyInfo>
```
#### Include the Windows.Compatibility Pack
Not every framework assembly is available in the .NET Core base class library. Windows applications like WinForms and WPF could have dependencies that are not available in .NET Core or .NET Standard. Adding a reference to the [Windows Compatibilty Pack](https://docs.microsoft.com/dotnet/core/porting/windows-compat-pack) will help reduce missing assembly dependencies as it includes several types that might be needed by your application.

```cmd
dotnet add package Microsoft.Windows.Compatibility
```
#### Link Files from the old project

Visual Studio does not yet support designers and custom tools for .NET Core desktop development. You can keep your files in the original project and link the generated files to the new project by using the link attribute in the project elements, e.g. `<Compile Link="" />`. See the [sample](helloworld-sharedsource) in this repo for an example of this.

#### Migrating WCF clients

.NET Core has its own implementation of `System.ServiceModel` with some differences:
* It's available as NuGet packages (also included in the Windows Compatiblity Pack).
* There are [unsupported features](https://github.com/dotnet/wcf/blob/master/release-notes/SupportedFeatures-v2.1.0.md) that you should review.
* The binding and endpoint address must be specified in the service client constructor. Otherwise, if you reuse the ServiceReference created by Visual Studio, you may get the following error: `System.PlatformNotSupportedException: 'Configuration files are not supported.'`
 
## Filing issues and getting help
You can file WinForms and WPF related issues in the [dotnet/core repo](https://github.com/dotnet/core/issues). If you are trying out WPF or WinForms development on top of .NET Core 3.0 and get stuck or have questions, please reach out to netcore3modernize@microsoft.com. 

### Known issues
Take a look at the issues filed with the [WinForms area tag](https://github.com/dotnet/core/labels/area-winforms).
