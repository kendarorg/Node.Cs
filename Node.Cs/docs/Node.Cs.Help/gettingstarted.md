<!--settings(
title=Getting started
description=Getting started
)-->

<!--include(shared/breadcrumb.php)-->

## {Title}

### Introduction

First download the source from github [https://github.com/endaroza](https://github.com/endaroza)

And build all.

The project to start will be "src\Node.cs"

The Node.cs command line will show then up

### Node.Cs Prompt

Several command are available (case insensitive)

help: show the list of all available commands with its short descriptions
help [cmd]: show the help for the [cmd] command
run [script]: run a Node.cs script (.cs or .ncs). Will run the 'void Execute()' method
run [script] [function]: run a Node.cs script (.cs or .ncs). Will run the 'void [function]' method
exit: exit from the prompt with errorcode 0
exit [number]: exit with the error code [number]


### Node.Cs Scripts

Two kind of scripts are available:

* .cs: C# source files
* .ncs: File containing Node.Cs Prompt commands
