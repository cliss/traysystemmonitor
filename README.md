# Tray System Monitor
Small system monitor that lives in the Windows tray

# Usage
Tray System Monitor is a small monitor that shows two graphs in the Windows tray. You can select which monitors you'd like to see. Out of the box, the following are provided:

* Battery remaining
* CPU usage
* Disk usage
* Memory usage
* Network downstream
* Network upstream
* Network up & downstream
* Volume
* Wireless strength

It's very easy for developers to add new monitors; more on that below.

To change the monitors shown, right-click on the tray icon. To start Task Manager, double-click on the icon.

# Extending
To create your own monitor – a `StatusProvider` – is very simple.

1. Create a new C# class library
2. Include a reference to `Casey.TraySystemMonitor.Ext.dll`
3. Create a class that implements `Casey.TraySystemMonitor.Ext.IStatusProvider`
4. Build your DLL
5. Drop it in the same folder as `Casey.TraySystemMonitor.Gui.exe`
6. Restart Tray System Monitor

You should find that the new `StatusProvider` in your DLL is auto-discovered and added as a possible item.

# Disclaimers
I wrote this in 2007 as a fresh-faced C# developer with only around a year experience. I've grown a lot since then, but I made very few changes to this code when I dug it back out. Your mileage may vary, and please don't judge me by what you see here.
