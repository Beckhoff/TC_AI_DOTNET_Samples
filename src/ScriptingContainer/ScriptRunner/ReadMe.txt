Activating the Powershell Script
================================

To use the Powershell module, it must first be registered at the current Powershell session. So from the opened Powerhsell console type

>> cd [ModuleLocation]
>> import-module .\Powershell.ScriptRunner.dll -Verbose


VERBOSE: Loading module from path 'C:\tmp\Powershell.ScriptRunner.dll'.
VERBOSE: Importing cmdlet 'Start-TcScripts'.

This will import the Start-TcScripts cmdlet.
Help information for this cmdlet can be produced via:

>> get-help Start-TcScripts

NAME
    Start-TcScripts

SYNTAX
    Start-TcScripts [-Name] <string[]> [-UserMode] [-Visible] [-SuppressUI]
    [-DTE <string>]  [<CommonParameters>]

EXAMPLE 

>> Start-TcScripts * -visible

	
Debugging the Script Runner Powershell Script within Visual Studio
==================================================================
Add call to Powershell on Project Debugging Settings:

Start Action: Start External Program

C:\Windows\System32\WindowsPowerShell\v1.0\powershell.exe 

(for using 32-Bit Process on 32-Bit System, 64-Bit Process on 64-Bit Systems)

Running 32-Bit Powershell process on 64-Bit Systems only
C:\Windows\SysWow64\WindowsPowerShell\v1.0\powershell.exe

Command Line Arguments  for automatic loading the Powershell modulc during debugging

-noexit -command "import-module .\Powershell.ScriptRunner.dll -Verbose; update-formatData .\Powershell.ScriptRunner.format.ps1xml"