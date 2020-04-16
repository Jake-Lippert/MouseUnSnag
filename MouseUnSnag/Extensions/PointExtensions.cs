using System;
using System.Drawing;

namespace MouseUnSnag.Extensions
{
	public static class PointExtensions
	{
		/// <summary>
		/// Geometric helpers. These all deal with Rectangles, Points, and X and Y values.
		/// </summary>
		/// <returns>
		/// Return the signs of X and Y. This essentially gives us the "component direction" of
		/// the point (N.B. the vector length is not "normalized" to a length 1 "unit vector" if
		/// both the X and Y components are non-zero).
		/// </returns>
		public static Point Sign(this Point point) => new Point(Math.Sign(point.X), Math.Sign(point.Y));

		/// <summary>
		/// "Direction" vector from P1 to P2. X/Y of returned point will have values
		/// of -1, 0, or 1 only (vector is not normalized to length 1).
		/// </summary>
		public static Point Direction(this Point point1, Point point2) => Sign(point2 - (Size)point1);

		public static double DistanceTo(this Point point1, Point point2) => Math.Sqrt(Math.Pow(point1.X - point2.X, 2) + Math.Pow(point1.Y - point2.Y, 2));

		public static bool SetCursorPos(this Point point) => Windows.User32.SetCursorPos(point.X, point.Y);
	}
}