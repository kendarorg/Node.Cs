@echo off >NUL 2>NUL 
if "TRUE"=="TRUE" ( 
    echo * NOT Cleaning up build directories for BasicNugetPackageFor.Test/.Net 4.0... 
) else ( 
    rd /s /q "C:\Projects\Kendar.Framework\Github\endaroza\Node.Cs.Alpha2\Node.Cs\test\BasicNugetPackageFor.Test\obj" >NUL 2>NUL 
    rd /s /q "C:\Projects\Kendar.Framework\Github\endaroza\Node.Cs.Alpha2\Node.Cs\test\BasicNugetPackageFor.Test\bin" >NUL 2>NUL 
) 
if "TRUE"=="TRUE" ( 
    echo * NOT Cleaning up build directories for BasicNugetPackageFor.Test/.Net 4.5... 
) else ( 
    rd /s /q "C:\Projects\Kendar.Framework\Github\endaroza\Node.Cs.Alpha2\Node.Cs\test\BasicNugetPackageFor.Test\obj" >NUL 2>NUL 
    rd /s /q "C:\Projects\Kendar.Framework\Github\endaroza\Node.Cs.Alpha2\Node.Cs\test\BasicNugetPackageFor.Test\bin" >NUL 2>NUL 
) 
if "TRUE"=="TRUE" ( 
    echo * NOT Cleaning up temporary nuget for BasicNugetPackageFor.Test 
) else ( 
    rd /q /s "C:\Projects\Kendar.Framework\Github\endaroza\Node.Cs.Alpha2\Node.Cs\tmp_nuget\BasicNugetPackageFor.Test" >NUL 2>NUL  
) 
