using System;
using System.Drawing;
using System.Runtime.InteropServices;

namespace MouseUnSnag.Windows
{
	/// <summary>
	/// user32.dll
	/// </summary>
	public static class User32
	{
		public const int SW_HIDE = 0;
		public const int SW_SHOW = 5;
		public const int WH_MOUSE_LL = 14; // Win32 low-level mouse event hook ID.
		public const uint WM_MOUSEMOVE = 0x0200;

		public delegate IntPtr HookProc(int nCode, uint wParam, IntPtr lParam);

		[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		public static extern IntPtr SetWindowsHookEx(int idHook, HookProc lpfn, IntPtr hMod, uint dwThreadId);

		[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool UnhookWindowsHookEx(IntPtr hhk);

		[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		public static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, uint wParam, IntPtr lParam);

		[DllImport("user32.dll")]
		public static extern bool SetCursorPos(int X, int Y);

		[DllImport("user32.dll")]
		public static extern bool GetCursorPos(out Point lpPoint);

		//[DllImport("user32.dll")]
		//public static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

		//https://msdn.microsoft.com/en-us/library/windows/desktop/dd145062(v=vs.85).aspx
		[DllImport("User32.dll")]
		public static extern IntPtr MonitorFromPoint([In] Point pt, [In] uint dwFlags);
	}
}