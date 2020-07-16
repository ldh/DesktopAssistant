# DesktopAssistant

A transparent app made with unity, display specific objects on the desktop without blocking other parts.
	
## What does this project do?
 
Usually the projects  we made with unity are full screen and we can’t penetrate it to see or interact other content on the desktop. But in fact we can do this kind of operation in unity now with a few simple WindowsAPI. Knowing this, we can do some wonderful things as shown in the picture below.
	
![示例](http://blog.lidonghui.xyz:8080/Github/DesktopAssistant1.jpg)

Add something cute to your desktop so you won’t be alone. This project uses Live2D  to create a cute girl, including some simple interactions.

## Docs

In order to achieve these effects, we only need to use  namespace ```System.Runtime.InteropServices``` in C# to import related dll and use the method in it.

View http://www.pinvoke.net/ to learn more about this. This project mainly used user32.dll , using DllImport attribute to define the method in it.

Here are a few examples below, more documentation is comming...

```csharp
    [DllImport("user32.dll")]
    private static extern IntPtr GetActiveWindow();
```

```csharp
    [DllImport("user32.dll")]
    private static extern int SetWindowLong(IntPtr hWnd, int nIndex, uint dwNewLong);
```

```csharp
    [DllImport("user32.dll", SetLastError = true)]
    private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);
```

Make sure about this when you build a transparent project:
 * The camera's background color is (0,0,0,0).
 * View the settings in **Resolution and Presentation** of this project.
 
 
Existing functions:
 * Right click on the girl to move her position.
 * Left click on the girl to interact with her.
 * The girl will aim at your mouse position.



If you want to know more, download the project and try it!

## Write at the end

I will gradually add more features and materials. Just give me a star if you find it useful :smile:.
