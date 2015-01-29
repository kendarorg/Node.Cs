<!--settings(
title=Getting started
description=Getting started
)-->

<!--include(shared/breadcrumb.php)-->

## {Title}

### Introduction

First download the source from github [https://github.com/kendarorg](https://github.com/kendarorg)

And build all.

The project to start will be "src\Node.cs"

The Node.cs command line will show then up

### Node.Cs Directories

The main directories are:

* CurrentDirectory: The directory from which Node had been started
* Tmp: The Node temporary directory
* Packages: The storage for nuget packages
* Bin: Extra binary files

### Node.Cs Prompt

Several command are available (case insensitive). 
Note that parameters containing spaces must be surrounded by double or single quotes.

* help: show the list of all available commands with its short descriptions
* help [cmd]: show the help for the [cmd] command.
* echo [message]: show the given message.
* run [script]: run a Node.cs script (.cs or .ncs). Will run the 'void Execute()' method. If no file 
extension is present, .cs is assumed.
* run [script] [function]: run a Node.cs script (.cs or .ncs). Will run the 'void [function]' method If no file 
extension is present, .cs is assumed.
* exit: exit from the prompt with errorcode 0.
* exit [number]: exit with the error code [number].
* dll load [dllPath]: load a dll in memory. If it has not an absolute path it's searched on
	* CurrentDirectory
	* Bin directory
	* Temp directory
	* Node.Cs.exe directory
    * Packages directory
* nuget load [nugetPackage] (version): download the package from nuget.org and install it into the packages 
folder. If the version is specified the EXACT version is taken. Else the LATEST STABLE version is taken.


### Node.Cs Scripts

Two kind of scripts are available:

* .cs: C# source files
* .ncs: File containing Node.Cs Prompt commands

