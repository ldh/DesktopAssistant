# DesktopAssistant

A transparent app made with unity, display specific objects on the desktop without blocking other parts.
	
## What does this project do?
 
Usually the projects  we made with unity are full screen and we can’t penetrate it to see or interact other content on the desktop. But in fact we can do this kind of operation in unity now with a few simple WindowsAPI. Knowing this, we can do some wonderful things as shown in the picture below.
	
![示例](http://blog.lidonghui.xyz:8080/Github/DesktopAssistant1.jpg)

Add something cute to your desktop so you won’t be alone. This project uses Live2D  to create a cute girl, including some simple interactions.

## Docs

In order to achieve these effects, we only need to use  namespace ```System.Runtime.InteropServices``` in C# to import related dll and use the method in it.

View http://www.pinvoke.net/ to learn more about this. This project mainly used method in user32.dll.


后续我会添加更多的素材进去。
