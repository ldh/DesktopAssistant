using System;
using System.Runtime.InteropServices;

public static class WindowsAPI {

    /// <summary>
    /// 可以调出一些消息窗口，暂时没有用到呢
    /// </summary>
    /// <param name="hWnd"></param>
    /// <param name="text"></param>
    /// <param name="caption"></param>
    /// <param name="type"></param>
    /// <returns></returns>
    [DllImport("user32.dll")]
    public static extern int MessageBox(IntPtr hWnd, string text, string caption, uint type);

    /// <summary>
    /// 获取当前的窗口的Handle（我实在不想把它翻译成句柄，后面都用Handle了）
    /// </summary>
    /// <returns></returns>
    [DllImport("user32.dll")]
    private static extern IntPtr GetActiveWindow();
    
    /// <summary>
    /// 改变窗口的样式，一些参数的具体介绍后面会加上
    /// </summary>
    /// <param name="hWnd">Handle</param>
    /// <param name="nIndex"></param>
    /// <param name="dwNewLong"></param>
    /// <returns></returns>
    [DllImport("user32.dll")]
    private static extern int SetWindowLong(IntPtr hWnd, int nIndex, uint dwNewLong);

    /// <summary>
    /// 这里用来让我们的窗口永远保持在桌面最上层， 这样就不会被其他程序遮挡了
    /// </summary>
    /// <param name="hWnd"></param>
    /// <param name="hWndInsertAfter"></param>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="cx"></param>
    /// <param name="cy"></param>
    /// <param name="uFlags"></param>
    /// <returns></returns>
    [DllImport("user32.dll", SetLastError = true)]
    private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int x, int y, int cx, int cy, uint uFlags);
    
    [StructLayout(LayoutKind.Sequential)]
    public struct MARGINS {
        public int cxLeftWidth;
        public int cxRightWidth;
        public int cyTopHeight;
        public int cyBottomHeight;
    }

    /// <summary>
    /// 让你的窗口变透明（显示上的）
    /// </summary>
    /// <param name="hWnd"></param>
    /// <param name="margins"></param>
    /// <returns></returns>
    [DllImport("Dwmapi.dll")]
    private static extern uint DwmExtendFrameIntoClientArea(IntPtr hWnd, ref MARGINS margins);

    // [DllImport("user32.dll")]
    // private static extern int SetLayeredWindowAttributes(IntPtr hWnd, uint crKey, byte bAlpha, uint dwFlags);
    

    /// <summary>
    /// 扩展样式
    /// </summary>
    private const int GWL_EXSTYLE = -20;
    
    private const uint WS_EX_LAYERED = 0x00080000;
    private const uint WS_EX_TRANSPARENT = 0x00000020;

    private static readonly IntPtr HWND_TOPMOST = new IntPtr(-1);

    /// <summary>
    /// 保存当前窗口的Handle
    /// </summary>
    private static IntPtr hWnd;
    
    /// <summary>
    /// 不要在编辑器模式下运行此方法，当然你不信的话你可以试试 ^.^
    /// </summary>
    public static void InitWindow()
    {
        hWnd = GetActiveWindow();

        MARGINS margins = new MARGINS { cxLeftWidth = -1 };
        
        DwmExtendFrameIntoClientArea(hWnd, ref margins);

        SetWindowLong(hWnd, GWL_EXSTYLE, WS_EX_LAYERED | WS_EX_TRANSPARENT);

        SetWindowPos(hWnd, HWND_TOPMOST, 0, 0, 0, 0, 0);
    }

    /// <summary>
    /// 控制你的程序是可穿透状态，目前使用Collider2D进行判断
    /// </summary>
    /// <param name="isHitACollider"></param>
    public static void SetClickThrough(bool isHitACollider) {
        if (isHitACollider)
        {
            SetWindowLong(hWnd, GWL_EXSTYLE, WS_EX_LAYERED);
        }
        else 
        {
            SetWindowLong(hWnd, GWL_EXSTYLE, WS_EX_LAYERED | WS_EX_TRANSPARENT);
        }
    }
    
    
    
}
