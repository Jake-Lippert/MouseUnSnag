using System.Drawing;
using MouseUnSnag.Enums;
using MouseUnSnag.Extensions;

namespace MouseUnSnag
{
	public class Screen : Contracts.IScreen
	{
		private readonly System.Windows.Forms.Screen _systemScreen;

		public Screen(Contracts.IScreenSet screenSet, int index)
		{
			ScreenSet = screenSet;
			_systemScreen = System.Windows.Forms.Screen.AllScreens[Index = index];
		}


		public Contracts.IScreenSet ScreenSet { get; }
		/// <summary>
		/// Index into Screen.AllScreens[] for this SnagScreen object.
		/// </summary>
		public int Index { get; }
		public Rectangle Bounds => _systemScreen.Bounds;
		public uint Dpi => _systemScreen.GetDpi(DpiType.Raw);

		public override string ToString() => $"{Index}: ({Bounds.Left},{Bounds.Top}) + ({Bounds.Width},{Bounds.Height}) => ({Bounds.Right},{Bounds.Bottom}): [{Dpi} DPI]";
	}
}