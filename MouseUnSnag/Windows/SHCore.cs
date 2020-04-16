using System;
using System.Runtime.InteropServices;
using MouseUnSnag.Enums;

namespace MouseUnSnag.Windows
{
	/// <summary>
	/// SHCore.dll
	/// </summary>
	public static class SHCore
	{

		[DllImport("SHCore.dll", SetLastError = true)]
		public static extern bool SetProcessDpiAwareness(PROCESS_DPI_AWARENESS awareness);

		//https://msdn.microsoft.com/en-us/library/windows/desktop/dn280510(v=vs.85).aspx
		[DllImport("Shcore.dll")]
		public static extern IntPtr GetDpiForMonitor([In] IntPtr hmonitor, [In] DpiType dpiType, [Out] out uint dpiX, [Out] out uint dpiY);
	}
}