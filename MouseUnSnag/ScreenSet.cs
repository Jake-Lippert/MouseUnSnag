using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using MouseUnSnag.Contracts;
using MouseUnSnag.Extensions;

namespace MouseUnSnag
{
	/// <summary>
	/// Keeps track of the physical screens/monitors attached to the system
	/// </summary>
	public class ScreenSet : IScreenSet
	{
		private int _nJumps = 0;
		private Point _lastMouse = Point.Empty;

		/// <summary>
		/// Loop through System.Windows.Forms.Screen.AllScreens[] array to initialize ourselves
		/// </summary>
		public ScreenSet() => AllScreens = System.Windows.Forms.Screen.AllScreens.Select((s, i) => new Screen(this, i)).ToArray();
		public ScreenSet(params IScreen[] screens) => AllScreens = screens.ToArray();

		public IScreen[] AllScreens { get; }
		public List<IScreen> LeftMost { get; } = new List<IScreen>();
		public List<IScreen> RightMost { get; } = new List<IScreen>();
		public List<IScreen> TopMost { get; } = new List<IScreen>();
		public List<IScreen> BottomMost { get; } = new List<IScreen>();
		public bool EnableHorizontalWrap { get; set; }
		/// <summary>
		/// Rectangle that contains all screens
		/// </summary>
		public Rectangle BoundingBox => AllScreens.Aggregate(Rectangle.Empty, (agg, s) => Rectangle.Union(agg, s.Bounds));

		public override string ToString() => AllScreens.Aggregate(new StringBuilder($"There are {AllScreens.Length} screen(s) bound by: {BoundingBox}"), (agg, screen) => agg.AppendLine().Append(screen.ToString())).ToString();

		/// <summary>
		/// Find which screen the point is on. If it is not on one, return null.
		/// </summary>
		public IScreen WhichScreen(Point point) => AllScreens.FirstOrDefault(s => s.Bounds.Contains(point));

		/// <summary>
		/// Find the best point to "wrap" around the cursor, either horizontally or
		/// vertically. We consider only the "OuterMost" screens. For instance, if
		/// the mouse is moving to the left, we consider only the screens in the
		/// RightMost[] array.
		/// </summary>
		public Point WrapPoint(Point cursor, Point direction)
		{
			if (direction.X != 0)
			{
				var closestDistance = int.MaxValue;
				IScreen wrapScreen = null; // Our "wrap screen".

				// Find closest Left- or Right-most screen, in Y direction.
				foreach (var screen in direction.X == 1 ? LeftMost : RightMost)
				{
					var distance = Math.Abs(screen.Bounds.OutsideYDistance(cursor));
					if (distance < closestDistance)
					{
						closestDistance = distance;
						wrapScreen = screen;
					}
				}
				return wrapScreen.Bounds.ClosestBoundaryPoint(new Point(direction.X == 1 ? wrapScreen.Bounds.Left : wrapScreen.Bounds.Right, cursor.Y));
			}

			// We should never get here, but if we do, just return the current
			// Cursor location.
			return cursor;
		}


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
		public bool CheckJumpCursor(Point cursor, Point mouse, out Point newCursor)
		{
			//--Gather pertinent information about cursor, mouse, screens.
			var (cursorScreen, mouseScreen) = (WhichScreen(cursor), WhichScreen(mouse));
			var previousMouse = _lastMouse;
			var isStuck = cursor != previousMouse && mouseScreen != cursorScreen;

			_lastMouse = mouse;

			//--Let caller know we did NOT jump the cursor.
			if (!isStuck && cursorScreen == mouseScreen || ScreenInDirection(cursorScreen, mouse) is null)
			{
				newCursor = cursor;
				return false;
			}
			newCursor = GetAdjustedPoint(cursor, mouse);

			System.Diagnostics.Debug.WriteLine(@$"UnSnag #{++_nJumps}
	StuckDirection: {cursorScreen.Bounds.OutsideDirection(mouse)}
	Distance: {cursorScreen.Bounds.OutsideDistance(mouse)}
	mouse: {mouse}
	previousMouse: {previousMouse} ==? cursor: {cursor} (On Monitor#{cursorScreen}/{mouseScreen})
	Stuck: {isStuck}");

			return true;
		}

		public Point GetAdjustedPoint(Point cursor, Point mouse)
		{
			var cursorScreen = WhichScreen(cursor);

			if (WhichScreen(mouse) is { } mouseScreen)
			{
				//--If the mouse "location" (which can take on a value beyond the current cursor screen) has a value,
				//	then it is "within" another valid screen bounds, so just jump to it!
				return ComputeAdjustment(mouseScreen);
			}
			else if (ScreenInDirection(cursorScreen, mouse) is { } jumpScreen)
			{
				return jumpScreen.Bounds.ClosestBoundaryPoint(ComputeAdjustment(jumpScreen));
			}
			else if (EnableHorizontalWrap && cursorScreen.Bounds.OutsideDirection(mouse) is { } stuckDirection && stuckDirection.X != 0)
			{
				return WrapPoint(cursor, stuckDirection);
			}
			else
			{
				return cursor;
			}

			Point ComputeAdjustment(IScreen targetScreen)
			{
				//var midpointOffsetPixels = cursorScreen.ScreenMidpointY - mouseScreen.ScreenMidpointY;
				//Debug.WriteLine($"CursorMidpoint: {cursorScreen.ScreenMidpointY}, MouseMidpoint: {mouseScreen.ScreenMidpointY}, {midpointOffsetPixels}|{1.0 * midpointOffsetPixels / cursorScreen.Dpi:n2}|{1.0 * midpointOffsetPixels / mouseScreen.Dpi:n2}");

				var cursorInchesOffMidpoint = 1.0 * (cursor.Y - cursorScreen.GetScreenMidpointY()) / cursorScreen.Dpi;
				var mousePixelsOffMidpoint = (int)Math.Round(cursorInchesOffMidpoint * targetScreen.Dpi + targetScreen.GetScreenMidpointY());
				//Debug.WriteLine($"Cursor: {cursorInchesOffMidpoint:n2}, Mouse: {mousePixelsOffMidpoint}");

				return new Point(mouse.X, mousePixelsOffMidpoint);
			}
		}

		/// <summary>
		/// Find the closest monitor that is in the direction of the mouse.
		/// </summary>
		public IScreen ScreenInDirection(IScreen cursorScreen, Point mouse) => AllScreens
			.Where(screen => screen != cursorScreen && cursorScreen.Bounds.OutsideDirection(mouse) switch
			{
				{ X: 1 } => cursorScreen.Bounds.Right == screen.Bounds.Left,
				{ X: -1 } => cursorScreen.Bounds.Left == screen.Bounds.Right,
				{ Y: 1 } => cursorScreen.Bounds.Bottom == screen.Bounds.Top,
				{ Y: -1 } => cursorScreen.Bounds.Top == screen.Bounds.Bottom,
				_ => false
			})
			.OrderBy(s => mouse.DistanceTo(s.Bounds.ClosestBoundaryPoint(mouse)))
			.FirstOrDefault();
	}
}