using System;
using System.Runtime.InteropServices;

public static class WindowsAPI {

    [DllImport("user32.dll")]
    public static extern int MessageBox(IntPtr hWnd, string text, string caption, uint type);

    [DllImport("user32.dll")]
    private static extern IntPtr GetActiveWindow();
    
    /// <summary>
    /// Change the window style
    /// </summary>
    /// <param name="hWnd">Handle</param>
    /// <param name="nIndex"></param>
    /// <param name="dwNewLong"></param>
    /// <returns></returns>
    [DllImport("user32.dll")]
    private static extern int SetWindowLong(IntPtr hWnd, int nIndex, uint dwNewLong);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

    [DllImport("user32.dll")]
    private static extern int SetLayeredWindowAttributes(IntPtr hWnd, uint crKey, byte bAlpha, uint dwFlags);

    public struct MARGINS {
        public int cxLeftWidth;
        public int cxRightWidth;
        public int cyTopHeight;
        public int cyBottomHeight;
    }

    [DllImport("Dwmapi.dll")]
    public static extern uint DwmExtendFrameIntoClientArea(IntPtr hWnd, ref MARGINS margins);

    /// <summary>
    /// 扩展样式
    /// </summary>
    private const int GWL_EXSTYLE = -20;

    
    private const uint WS_EX_LAYERED = 0x00080000;
    private const uint WS_EX_TRANSPARENT = 0x00000020;

    private static readonly IntPtr HWND_TOPMOST = new IntPtr(-1);

    private const uint LWA_COLORKEY = 0x00000001;

    /// <summary>
    /// A handle to our unity window
    /// </summary>
    private static IntPtr hWnd;
    

    /// <summary>
    /// You really don't want to call it in the editor.
    /// </summary>
    public static void InitWindow()
    {
        hWnd = GetActiveWindow();

        MARGINS margins = new MARGINS { cxLeftWidth = -1 };
        DwmExtendFrameIntoClientArea(hWnd, ref margins);

        SetWindowLong(hWnd, GWL_EXSTYLE, WS_EX_LAYERED | WS_EX_TRANSPARENT);
        // SetLayeredWindowAttributes(hWnd, 0, 0, LWA_COLORKEY);

        SetWindowPos(hWnd, HWND_TOPMOST, 0, 0, 0, 0, 0);
    }

    /// <summary>
    /// Control whether your mouse can through your project window.
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
