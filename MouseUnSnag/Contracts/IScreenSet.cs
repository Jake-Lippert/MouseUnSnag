using System.Collections.Generic;
using System.Drawing;

namespace MouseUnSnag.Contracts
{
	public interface IScreenSet
	{
		IScreen[] AllScreens { get; }
		List<IScreen> LeftMost { get; }
		List<IScreen> RightMost { get; }
		List<IScreen> TopMost { get; }
		List<IScreen> BottomMost { get; }

		/// <summary>
		/// Find which screen the point is on. If it is not on one, return null.
		/// </summary>
		IScreen WhichScreen(Point point);
		/// <summary>
		/// CheckJumpCursor() returns TRUE, ONLY if the cursor is "stuck". By "stuck" we
		/// specifically mean that the user is trying to move the mouse beyond the boundaries of
		/// the screen currently containing the cursor. This is determined when the *current*
		/// cursor position does not equal the *previous* mouse position. If there is another
		/// adjacent screen (or a "wrap around" screen), then we can consider moving the mouse
		/// onto that screen.
		///
		/// Note that this is ENTIRELY a *GEOMETRIC* method. Screens are "rectangles", and the
		/// cursor and mouse are "points." The mouse/cursor hardware interaction (obtaining
		/// current mouse and cursor information) is handled in routines further below, and any
		/// Screen changes are handled by the DisplaySettingsChanged event. There are no
		/// hardware or OS/Win32 references or interactions here.
		/// </summary>
		bool CheckJumpCursor(Point cursor, Point mouse, out Point newCursor);
		Point GetAdjustedPoint(Point cursor, Point mouse);
		/// <summary>
		/// Find the closest monitor that is in the direction of the mouse.
		/// </summary>
		IScreen ScreenInDirection(IScreen cursorScreen, Point mouse);
	}
}