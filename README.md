# Wireless Button Reprogrammer
#### `Personal note: Thank you for having a look at this project, a star would be greatly appreciated!`
## Table of Contents

* [Overview](#overview)
* [Warning](#warning)
* [Supported Devices](#supported-devices)
* [Setup](#setup)
* [Get Device Supported](#get-device-supported)
* [How To Change Keycodes](#changing-keycodes)
* [Preview](#preview)
* [Technologies](#technologies)

---

## Overview

Reprogram any button on (most) wireless headsets to perform up to **3 custom actions** (default: Pause, Skip, Replay).

[See Supported Devices](#supported-devices)

---

## Warning

Original button functions (e.g., mute) still trigger because they're handled onboard.
Only use this tool if you’re okay with losing the original behavior.

---

## Supported Devices

*(Windows only)*

* HyperX Cloud II Wireless (DTS version)
* HyperX Cloud III Wireless
* Corsair Virtuoso XT

[Want your device supported?](#get-device-supported)

---

## Setup

1. Download and run [`WBR.exe`](https://github.com/TizianGuth/Wireless-Button-Reprogrammer/releases/latest)
2. Press `Debug`
3. Reconnect your device
4. Select your device
5. Finished!
---

## Get Device Supported
### Help others and share your findings!
To make other peoples lives easier, please be so kind and share your `presets.json` (located at `%appdata%` -> `WBR`) by submitting an [Issue](https://github.com/TizianGth/Wireless-Button-Reprogrammer/issues)!

1. Download and run [`WBR.exe`](https://github.com/TizianGuth/Wireless-Button-Reprogrammer/releases/latest)
2. Press `Debug`
3. Reconnect your device
4. Select your device
5. Wait 5-10s after replugging
6. Press the desired button on your headset multiple times (as a precaution, to account for occasionally changing "click signatures")
7. Make sure the output looks reasonable, then press `Save` and enter a custom preset name
8. Select the preset from the drop-down menue.
9. Finished!

---

## Changing Keycodes

Use this [MSDN list](https://learn.microsoft.com/en-us/windows/win32/inputdev/virtual-key-codes) to find desired keycodes.
Enter them in the program, then click **Apply** and **Start**.

---

## Preview
### Main Application
---
![Main](https://raw.githubusercontent.com/TizianGth/Wireless-Button-Reprogrammer/refs/heads/main/doc/img/Application.png)
### Debug Application
---
![Debug](https://raw.githubusercontent.com/TizianGth/Wireless-Button-Reprogrammer/refs/heads/main/doc/img/Debug.png)

---

## Technologies
* C#
* WPF (.NET 8.0, Visual Studio)
* [HIDLibrary](https://github.com/mikeobrien/HidLibrary)

---
