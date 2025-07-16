# Wireless Button Reprogrammer

## Table of Contents

* [Overview](#overview)
* [Warning](#warning)
* [Supported Devices](#supported-devices)
* [Request Support for Your Device](#request-support-for-your-device)
* [Setup](#setup)
* [Finding Vendor & Product ID](#finding-vendor--product-id)
* [Changing Keycodes](#changing-keycodes)
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

[Want your device supported?](#request-support-for-your-device)

---

## Request Support for Your Device

1. Download and run [`Debug.exe`](https://github.com/TizianGuth/Wireless-Button-Reprogrammer/releases/tag/Debug)
2. Follow the prompts
3. Open a GitHub issue and share your readings (`debug.json`)

---

## Setup

1. Download the `.exe` from [Releases](https://github.com/TizianGuth/Wireless-Button-Reprogrammer/releases)
2. Run the program
3. Input your VID & PID (see below)
4. Click **Apply**, then **Start**

---

## Finding Vendor & Product ID

**Easiest Method (New):**
Download [`debug.zip`](https://github.com/Tiziam/Wireless-Button-Reprogrammer/releases/tag/Debug) and follow instructions to step 3.

**Alternative (Old method - Busdog):**

1. Install [Busdog](https://github.com/djpnewton/busdog)
2. Enable "Automatically trace new Devices"
3. Unplug & replug your USB dongle
4. Look for `VID_XXXX` and `PID_XXXX`

   * ![Trace](https://github.com/GuthiYT/hyperxrebutton/blob/main/doc/img/busdog_trace_new.png)
   * ![Device](https://github.com/GuthiYT/hyperxrebutton/blob/main/doc/img/busdog_device.png)

---

## Changing Keycodes

Use this [MSDN list](https://learn.microsoft.com/en-us/windows/win32/inputdev/virtual-key-codes) to find desired keycodes.
Enter them in the program, then click **Apply** and **Start**.

---

## Technologies

* WPF (.NET 8.0, Visual Studio)
* [AudioManager](https://gist.github.com/sverrirs/d099b34b7f72bb4fb386)
* [HIDLibrary](https://github.com/mikeobrien/HidLibrary)

---
