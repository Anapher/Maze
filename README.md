# Maze
Maze is a Remote Administration Tool for Windows aiming to give administrators access to the Windows machines they manage. Maze is solely intended for legal and ethical usage and I will do anything to fight against malicious usage.

## Building the project
It is very important that you build the project once before you open it in Visual Studio, as the build process will also download required files.

### Clone the repository
Clone recursivly using
`git clone --recurse-submodules https://github.com/Anapher/Maze.git`

### Build the project
Build using [Fake](https://fake.build): `dotnet fake build` (execute this in the root of the repository). Please note that you need to have Visual Studio 2019 installed and you need the packages
- ASP.Net and web development
- .NET desktop development
- Desktop Development with C++
- Visual Studio extension development
- Platform independent .Net Core development

When the build completed, you can find all artifacts in `/artifacts`

### Setup Liget and upload artifacts
Download the Liget binaries from [here](https://github.com/MazeAdmin/liget/releases/tag/Maze-1.0) (or of course you can also clone the source code and build them by yourself) and execute the Liget server using `dotnet Liget.dll`. Liget will provide all required modules in form of Nuget packages. Now, we must upload all artifacts generated from the build in the previous step. You must upload all nuget packages from `/artifacts/modules/` and from `/artifacts/nupkgs/`. To upload all files of a directory, execute `dotnet nuget push *.* --source http://localhost:LigetPort/`. You can find the exact url of your Liget server in the console output.

### Debugging Maze
Open `Maze.sln` using Visual Studio. It's important that you build the whole solution once in Debug mode.
Now, navigate to **Maze.Server/appsettings.json** and change the current entry in `Modules/PrimarySources` to the url of your Liget server (but please keep the relative url, so if your Liget server started on `http://localhost:9011`, you have to input `http://localhost:9011/api/v3/index.json` here).
Finally, to successfully debug Maze, make a right click on `Solution 'Maze'` in your Solution Explorer (the top most entry), select `Set Startup Projects...`, choose `Multiple startup projects` and set the action of `Maze`, `Maze.Server` and `Maze.Administration` to `Start`. After saving that, start the solution and find out the Maze server url. You can do so by reading the Output of `Maze.Server - ASP.NET Core Web Server` (there should be an entry like `Now listening on: http://127.0.0.1:9873`) or by doing a right click on `IIS Express` in your Windows Tray.
Go to **Maze/mazesettings.json** and change the url of `Connection/ServerUris/MainServer` to the url of your Maze server. Now you can restart the solution and you are set. In the administration, you can log in to the server using the url you just configured in the client and the credentials admin:admin.

### Installing the modules
To install modules, go to `Modules` in administration, install the modules you like, restart the server and wait some time for the components to load. Enjoy.

### Debugging modules
Debugging modules is crucial for successful development. To easily accomplish that, I wrote a small utility `ModuleLocalUpdater` that replaces the compiled modules loaded from Liget with the debug versions locally so you have full IDE support.

## Features
### Extensibility
Maze is just a framework that only provides fundamental features to hold connections to clients. It is highly extensibly by `Modules`. The module system is based on *NuGet*, with the three Maze platoforms (Client, Server, Administration) as framework. Modules can have dependencies on regular NuGet packages aswell as on other Modules.
![Modules](https://i.imgur.com/fwLzxpC.png)

### Tasks Infrastructure
The module `Tasks.Infrastructure` provides a framework for tasks which are a collection of commands that can be scheduled to execute on a given audience.
![Create Task](https://i.imgur.com/CP6DoQP.png)

Task command results are grouped in executions which itself are grouped in sessions to create a nice overview. Commands may report their current progress aswell as return a HTTP response as result. View providers can create suited views for the responses, in the screenshot it's a simple log.
![Task Overview](https://i.imgur.com/wn33hf1.png)

### File Explorer
The file explorer mocks the style of the Windows Explorer.
![File Explorer](https://i.imgur.com/u0Ym3Z1.png)

### Task Manager
The task manager was designed with the [Process Explorer](https://docs.microsoft.com/en-us/sysinternals/downloads/process-explorer) in mind.
![Task Manager](https://i.imgur.com/32sHISe.png)

### Registry Explorer
Simple but fully featured, the Registry Editor allows to create, update and delete registry values and sub keys.
![Registry Explorer](https://i.imgur.com/QS75WFM.png)

### Client Panel
Even if all tools are easily reachable by the context menu, a TeamViewer like overview that shows the current screen aswell as provide access to the important features can be an advantage. Also, the Client Panel is a nice demonstration about how the modules can depend on each other.
![Client Panel](https://i.imgur.com/75bmzKy.png)
