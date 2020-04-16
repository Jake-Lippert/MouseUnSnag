using MouseUnSnag.Enums;
using MouseUnSnag.Windows;

namespace MouseUnSnag.Extensions
{
	public static class ScreenExtensions
	{
		public static uint GetDpi(this System.Windows.Forms.Screen screen, DpiType dpiType)
		{
			try
			{
				var monitor = User32.MonitorFromPoint(screen.Bounds.Location, User32.MONITOR_DEFAULTTONEAREST);
				SHCore.GetDpiForMonitor(monitor, dpiType, out var dpiX, out var _);
				return dpiX;
			}
			catch (System.DllNotFoundException)
			{
				return 96; // On Windows <8, just assume scaling 100%.
			}
		}

		public static int GetScreenMidpointY(this Contracts.IScreen screen) => (screen.Bounds.Top + screen.Bounds.Bottom) / 2;
	}
}