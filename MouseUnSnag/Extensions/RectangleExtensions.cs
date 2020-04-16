using System;
using System.Drawing;

namespace MouseUnSnag.Extensions
{
	public static class RectangleExtensions
	{
		/// <summary>
		/// If point is anywhere inside rectangle, then OutsideDistance() returns (0,0).
		/// Otherwise, it returns the (x,y) delta (sign is preserved) from point to the
		/// nearest edge/corner of rectangle. For Right and Bottom we must correct by 1,
		/// since the Rectangle Right and Bottom are one larger than the largest
		/// valid pixel.
		/// </summary>
		public static int OutsideXDistance(this Rectangle rectangle, Point point) => Math.Max(Math.Min(0, point.X - rectangle.Left), point.X - rectangle.Right + 1);

		public static int OutsideYDistance(this Rectangle rectangle, Point point) => Math.Max(Math.Min(0, point.Y - rectangle.Top), point.Y - rectangle.Bottom + 1);

		public static Point OutsideDistance(this Rectangle rectangle, Point point) => new Point(rectangle.OutsideXDistance(point), rectangle.OutsideYDistance(point));

		/// <summary>
		/// This is sort-of the "opposite" of above. In a sense it "captures" the point to the
		/// boundary/inside of the rectangle, rather than "excluding" it to the exterior of the rectangle.
		/// 
		/// If the point is outside the rectangle, then it returns the closest location on the
		/// rectangle boundary to the Point. If Point is inside Rectangle, then it just returns
		/// the point.
		/// </summary>
		public static Point ClosestBoundaryPoint(this Rectangle rectangle, Point point) => new Point(
			Math.Max(Math.Min(point.X, rectangle.Right - 1), rectangle.Left),
			Math.Max(Math.Min(point.Y, rectangle.Bottom - 1), rectangle.Top));

		/// <summary>
		/// In which direction(s) is(are) the point outside of the rectangle? If point is
		/// inside rectangle, then this returns (0,0). Else X and/or Y can be either -1 or
		/// +1, depending on which direction point is outside rectangle.
		/// </summary>
		public static Point OutsideDirection(this Rectangle rectangle, Point point) => rectangle.OutsideDistance(point).Sign();
	}
}