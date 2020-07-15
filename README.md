## DesktopAssistant
  A transparent app made with unity
 
## Homepage

	An open source MMOG server engine. 
	Just use Python scripting to be able to complete any game logic simply and efficiently (supports hotfixing).
	Various KBEngine plugins can be quickly combined with (Unity3D, OGRE, Cocos2d-x, HTML5, etc.) technology to 
	form a complete game client.

	The engine is written in C++, and saves developers from having to re-implement common server-side 
	technology, allowing them to concentrate on game logic development, to quickly create a variety of games.

	(Because it is often asked what the upper limit of the load is that KBEngine can handle, the underlying 
	architecture has been designed as a multi-process distributed dynamic load balancing solution. In theory, 
	by continuously expanding the hardware, the upper limit of the load can also be continuously increased. 
	The upper limit of the capacity of a single machine depends on the complexity of the game logic itself.)

Unity也是可以制作透明背景的程序的，使用几个简单的WindowsAPI即可。借助```System.Runtime.InteropServices```引入相关Dll并使用其中的方法即可，更多WindowsAPI相关信息可以查看 http://www.pinvoke.net/ ， 本项目主要是用了user32.dll中的相关方法。

本项目使用Live2D相关素材制作了一个萌萌哒妹子，包含一些简单的互动，这样写代码的时候就不会寂寞了。当然你可以换成任何你想要的的东西并把他们显示到你的桌面上。

![示例](http://blog.lidonghui.xyz:8080/Github/DesktopAssistant1.jpg)

后续我会添加更多的素材进去。
