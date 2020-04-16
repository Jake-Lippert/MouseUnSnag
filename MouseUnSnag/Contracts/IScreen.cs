using System.Drawing;

namespace MouseUnSnag.Contracts
{
	public interface IScreen
	{
		IScreenSet ScreenSet { get; }
		/// <summary>
		/// Index into Screen.AllScreens[] for this SnagScreen object.
		/// </summary>
		int Index { get; }

		Rectangle Bounds { get; }
		uint Dpi { get; }
	}
}