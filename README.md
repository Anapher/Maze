# Maze
Maze is a Remote Administration Tool for Windows aiming to give Administrators access to the Windows machines they must manage. Maze is solely intended for legal and ethical usage and I will do anything to fight against malicious usage.

## Building the project
It is very important that you build the project once before you open it in Visual Studio, as the build process will also download required files.
1. Clone the repository recursively
`git clone --recurse-submodules -j8 https://github.com/MazeAdmin/Maze.git`

2. Build using Fake: `fake build` (execute this in the root of the repository).

If you don't have Fake installed, you can find instructions [here](https://fake.build/fake-gettingstarted.html#Install-FAKE).

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

# No License
I have no yet decided how I will publish this project. Until then, this project is under [no license](https://choosealicense.com/no-permission/).
You have no permission from the creators of the software to use, modify, or share the software. Although a code host such as GitHub may allow you to view and fork the code, this does not imply that you are permitted to use, modify, or share the software for any purpose.
