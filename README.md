# Visual Studio menu plugin for DevPrompt

For debugging a DevPrompt plugin:
* Get DevPrompt.exe from [https://github.com/spadapet/DevPrompt](https://github.com/spadapet/DevPrompt) by either cloning the repo and building DevPrompt.sln, or install the MSI.
* In Visual Studio
    * Open VisualStudioMenu.Plugin.sln
    * Right-click the VisualStudioMenu.Plugin project
    * Choose Properties
* In the __Debug__ tab, set:
    * Start external program = (path to DevPrompt.exe)
    * Command line arguments = /plugin "(path to VisualStudioMenu.Plugin.dll)"
* For example:
    * Start external program = c:\git\DevPrompt\out\Debug.x64\bin\DevPrompt.exe
    * Command line arguments = /plugin "C:\git\DevPrompt.VisualStudioMenu.Plugin\VisualStudioMenu.Plugin\bin\Debug\VisualStudioMenu.Plugin.dll"
* Make sure that you don't install the public copy of the plugin from the __Settings__ dialog. If so, then two copies of the plugin could be loaded.

Then you can run and debug the plugin from Visual Studio.
