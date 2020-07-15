# DesktopAssistant

	A transparent app made with unity, display specific objects on the desktop without blocking other parts.
	
## What does this project do?
 
	Usually the projects  we make with unity are full screen, we can’t penetrate it to see or interact other content on the desktop. But in fact we can do this kind of operation in unity now with a few simple WindowsAPI. Knowing this, we can do some wonderful things as shown in the picture below.
	
	![示例](http://blog.lidonghui.xyz:8080/Github/DesktopAssistant1.jpg)

Unity也是可以制作透明背景的程序的，使用几个简单的WindowsAPI即可。借助```System.Runtime.InteropServices```引入相关Dll并使用其中的方法即可，更多WindowsAPI相关信息可以查看 http://www.pinvoke.net/ ， 本项目主要是用了user32.dll中的相关方法。

本项目使用Live2D相关素材制作了一个萌萌哒妹子，包含一些简单的互动，这样写代码的时候就不会寂寞了。当然你可以换成任何你想要的的东西并把他们显示到你的桌面上。



后续我会添加更多的素材进去。
