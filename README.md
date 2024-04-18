# NegativeScreen (Process Window Edition) #

## Overview

Inverts colors of specified process window or area within it. Intended for fully automated work with a specific app, controlled by config file, no GUI, no clicking, just using. The keyboard shortcuts should still work as before. To avoid making too many changes, I haven't removed much of the code that is no longer used.

Based on [NegativeScreen (Vivado Edition)](https://github.com/HeatPhoenix/NegativeScreen-Vivado-Edition-)

Huge thanks to Mlaily and HeatPhoenix for their work and sharing it on github!

Example:
<details closed><summary>Without NegativeScreen (click)</summary>
	
![606_bright_02](https://github.com/panzerdivisionbyzero/NegativeScreen-Process-Window-Edition/assets/109442319/6eb38e94-91fd-4517-9feb-cdf6c2dc843c)

</details>
<details closed><summary>With NegativeScreen (click)</summary>
	
![606_dark](https://github.com/panzerdivisionbyzero/NegativeScreen-Process-Window-Edition/assets/109442319/53379acb-e56a-4bba-a0c3-c84341c39599)

</details>

## Changes from the "Vivado Edition"

- parameterized target process name, so NegativeScreen can be used for almost any process instead of Vivado only
- added colors inversion overlay area dependence on target window controls (it's possible to limit overlay area by specific controls of target window)
- removed context menu - everthing is based on the configuration file parameters (explained further)
- replaced multiple screens overlays by single overlay adjusted to target window (optionally to its controls)
- restored Configuration and ConfigurationParser from the original repository and added some changes:
  - extended by new parameters (for more information see auto-generated config file):
    - DebugMode - shows logs console
    - ExecuteProcessFromDefinedPath - determines if NegativeScreen should run executable file given in "ProcessPath" parameter
    - FindProcessByPathInsteadName - determines if target process is defined by "ProcessPath" or "ProcessName" parameter
    - ProcessPath - absolute or relative path to target process; not used when ExecuteProcessFromDefinedPath and FindProcessByPathInsteadName are set to false.
    - ProcessPathParams - optional parameters for running executable specified in "ProcessPath"
    - ProcessName - determines which processes will be found if FindProcessByPathInsteadName = false; not used when FindProcessByPathInsteadName = true
    - MainWindowClassName - (optional) specifies which class (control) is the main window of target process
    - WindowSidesLimits - (optional) custom parameters list, limiting colors inversion overlay area
    - MainLoopPauseRefreshTime - milliseconds between cycles of waiting for target window to appear
    - MainLoopRefreshTimeIncreaseStep - used for changing "MainLoopRefreshTime" in runtime by hotkeys
    - WaitForWindowTime - maximum time in milliseconds for searching process and window for the fist time
    - WaitForWindowTimeAfterClosed - maximum time in milliseconds for searching process and window again, after it was closed / terminated
  - changed configuration file paths hierarchy and start-up behaviour:
    - configuration file is associated by NegativeScreen executable name, with ".conf" extension instead ".exe"
    - NegativeScreen tries to load configuration from working directory first, and if it fails, then tries to load it from `%AppData%\NegativeScreen` directory; if no configuration file was found, tries to create new configuration file (again: working directory first, AppData eventually) and opens it automatically in Notepad - in this case inverting overlay starts to work when Notepad is closed
  - added new data structure for ConfigurationParser: WindowSidesLimits

## How to use it

1. On the first startup:
   - NegativeScreen should create missing configuration file in working directory and open Notepad for editing
   - if creating file in working directory fails, NegativeScreen will try to create config file in `%AppData%\NegativeScreen`
   - after creating missing configuration file, NegativeScreen will open it with Notepad
   - normal operating mode will start after closing Notepad with config
2. Complete the configuration file (for more information see the configuration file contents):
   - specify target process path or name
   - specify if process should be executed by NegativeScreen ("ExecuteProcessFromDefinedPath"); some hints:
     - executing by NegativeScreen is more convenient than executing manually or by additional batch file
     - it's faster to find process started by NegativeScreen
     - if you want to find already running process (instead executing it by NegativeScreen), searching by ProcessName should be quite efficient
   - specify if process should be searched by ProcessPath or ProcessName ("FindProcessByPathInsteadName")
   - fill ProcessPath or ProcessName value
   - (optional) fill information about target window (I recommend using some other tool to look-up window controls classes, i.e. [WinInfo](https://www.softpedia.com/get/System/System-Info/WinInfo-JP.shtml)):
     - MainWindowClassName
     - WindowSidesLimits
     <br>// you can modify the configuration file anytime later with any text editor you want.
3. When your configuration file is ready:
   - save and close configuration file
   - NegativeScreen should begin normal operating mode (executing / finding process and create colors inversion overlay)
   - NegativeScreen should close when target process is terminated
4. Each subsequent use:
   - if NegativeScreen is not configured to execute process directly: start the target process / app
   - run NegativeScreen - if configuration file is correct, the colors inversion overlay should adjust automatically to specified window
   - if overaly does not show or NegativeScreen closes immediately, you can set DebugMode to "true" and try to deduce what's going wrong
   - NegativeScreen should close when target process is terminated

Note: NegativeScreen is originally designed to work as the only one instance at the same time. It's possible to remove that limitation, but I haven't changed it so far, at least for now.

---

# Base Fork Description:
## NegativeScreen (Vivado Edition) #

## Auto-Detects Vivado and darkens only Vivado! 

***

Mostly built as a personal tool to use while working in Vivado, feel free to use it. Hope it can be of use to someone who isn't me!
Many thanks to Mlaily the original dev of NegativeScreen for offering the source of the multi-monitor edition of NegativeScreen.

***

# Original Description #

NegativeScreen's main goal is to support your poor tearful eyes when enjoying the bright white interweb in a dark room.
This task is joyfully achieved by inverting the colors of your screen.

Unlike the Windows Magnifier, which is also capable of such color inversion,
NegativeScreen was specifically designed to be easy and convenient to use.

It comes with a minimal graphic interface in the form of a system tray icon with a context menu,
but don't worry, this only makes it easier to use!


## Features

Invert screen's colors.

Additionally, many color effects can be applied.
For example, different inversion modes, including "smart" modes,
swapping blacks and whites while keeping colors (about) the sames.

You can now configure the color effects manually via a configuration file.
You can also configure the hot keys for every actions, using the same configuration file.

A basic web api is part of NegativeScreen >= 2.5
It is disabled by default. When enabled, it listens by default on port 8990, localhost only.
See the configuration file to enable the api or change the listening uri...

All commands must be sent with the POST http method.
The following commands are implemented:
- TOGGLE
- ENABLE
- DISABLE
- SET "Color effect name" (without the quotes)

Any request sent with a method other than POST will not be interpreted,
and a response containing the application version will be sent.


## Requirements

NegativeScreen < 2.0 needs at least Windows Vista to run.

Versions 2.0+ need at least Windows 7.

Both run on Windows 8 or superior.

Graphic acceleration (Aero) must be enabled.

.NET 4.5 is required. (Should be available out of the box on up-to-date Windows installations)


## Default controls

-Press Win+Alt+H to Halt the program immediately
-Press Win+Alt+N to toggle color inversion (mnemonic: Night vision :))

-Press Win+Alt+F1-to-F10 to switch between inversion modes:
	* F1: standard inversion
	* F2: smart inversion1 - theoretical optimal transformation (but ugly desaturated pure colors)
	* F3: smart inversion2 - high saturation, good pure colors
	* F4: smart inversion3 - overall desaturated, yellows and blues plain bad. actually relaxing and very usable
	* F5: smart inversion4 - high saturation. yellows and blues  plain bad. actually quite readable
	* F6: smart inversion5 - not so readable. good colors. (CMY colors a bit desaturated, still more saturated than normal)
	* F7: negative sepia
	* F8: negative gray scale
	* F9: negative red
	* F10: red
	* F11: grayscale


## Configuration file

A customizable configuration file is created the first time you use "Edit Configuration" from the context menu.

The default location for this file is next to NegativeScreen.exe, and is called "negativescreen.conf"

If the default location is inaccessible,
NegativeScreen will try to create the configuration file in %AppData%/NegativeScreen/negativescreen.conf

This feature allows to deploy NegativeScreen.exe in a read-only location for unprivileged users.
Each user can then have its own configuration file.

The order of priority for trying to read a configuration file when starting NegativeScreen is as follows:
- %AppData%/NegativeScreen/negativescreen.conf
- negativescreen.conf in the directory where NegativeScreen.exe is located
- If the above fails, the embedded default configuration is used

Should something go wrong (syntax error, bad hot key...), you can simply delete the configuration file,
the internal default configuration will be used.

If the configuration file is missing, you can use the "Edit Configuration" menu to regenerate the default one.

Syntax: see in the configuration file...


***

Many thanks to Tom MacLeod who gave me the idea for the "smart" inversion mode :)


Enjoy!
