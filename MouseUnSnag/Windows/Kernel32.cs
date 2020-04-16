using System;
using System.Runtime.InteropServices;

namespace MouseUnSnag.Windows
{
	/// <summary>
	/// kernel32.dll
	/// </summary>
	public static class Kernel32
	{
		[DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		public static extern IntPtr GetModuleHandle(string lpModuleName);

		//[DllImport("kernel32.dll")]
		//static extern uint GetLastError();

		//[DllImport("kernel32", SetLastError = true, CharSet = CharSet.Auto)]
		//public static extern IntPtr LoadLibrary(string fileName);

		public delegate bool ConsoleEventDelegate(int eventType);

		[DllImport("kernel32.dll", SetLastError = true)]
		public static extern bool SetConsoleCtrlHandler(ConsoleEventDelegate callback, bool add);

		//[DllImport("kernel32.dll")]
		//public static extern IntPtr GetConsoleWindow();
	}
}