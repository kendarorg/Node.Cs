<!--settings(
title=Components
description=Components
)-->

<!--include(shared/breadcrumb.php)-->

## {Title}

Several components are available as dependecies.

### INodeConsole

The console for Node.cs

* Write
* WriteLine
* ReadLine

### INodeExecutionContext

It contains:

* Command line arguments and environment variables
* The directory in which Node.Cs had been called
* Node.Cs executable path (from now on will be nodePath)
* Binaries path (nodePath/bin)
* Temporary files path (nodePath/tmp)

### IWindsorContainer

The Castle Windsor IoC.

### INodeCsEntryPoint

The main entry point for Node.Cs

### IUiCommandsHandler

Needed to register/unregister commands

* RegisterCommand to register commands
* UnregisterCommand
* ContainsCommand
* Run To run a command string as in .ncs files

### INodeModule

The base class for Node.Cs modules