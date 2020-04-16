/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Dale Roberts. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

using System;
using System.Drawing;
using System.Windows.Forms;
using MouseUnSnag.Enums;
using MouseUnSnag.Windows;

namespace MouseUnSnag
{
	public static class StaticStuff
{
    public static bool SetCursorPos (Point p) { return User32.SetCursorPos(p.X, p.Y); }

    public static uint GetDpi (Screen screen, DpiType dpiType)
    {
        try
        {
            var mon = User32.MonitorFromPoint (screen.Bounds.Location, 2 /*MONITOR_DEFAULTTONEAREST*/ );
            SHCore.GetDpiForMonitor (mon, dpiType, out uint dpiX, out uint dpiY);
            return dpiX;
        }
        catch (System.DllNotFoundException)
        {
            return 96; // On Windows <8, just assume scaling 100%.
        }
    }

    // ============================================================================================
    // ============================================================================================
    // Geometric helpers. These all deal with Rectangles, Points, and X and Y values.
    //

    // Return the signs of X and Y. This essentially gives us the "component direction" of
    // the point (N.B. the vector length is not "normalized" to a length 1 "unit vector" if
    // both the X and Y components are non-zero).
    public static Point Sign (Point p) => new Point (Math.Sign (p.X), Math.Sign (p.Y));

    // 3-way Max()
    public static int Max (int x, int y, int z) => Math.Max (x, Math.Max (y, z));

    // "Direction" vector from P1 to P2. X/Y of returned point will have values
    // of -1, 0, or 1 only (vector is not normalized to length 1).
    public static Point Direction (Point P1, Point P2) => Sign (P2 - (Size) P1);

    // If P is anywhere inside R, then OutsideDistance() returns (0,0).
    // Otherwise, it returns the (x,y) delta (sign is preserved) from P to the
    // nearest edge/corner of R. For Right and Bottom we must correct by 1,
    // since the Rectangle Right and Bottom are one larger than the largest
    // valid pixel.
    public static int OutsideXDistance(Rectangle R, Point P)
        => Math.Max (Math.Min (0, P.X - R.Left), P.X - R.Right + 1);
        
    public static int OutsideYDistance(Rectangle R, Point P)
        => Math.Max (Math.Min (0, P.Y - R.Top), P.Y - R.Bottom + 1);
        
    public static Point OutsideDistance (Rectangle R, Point P)
        => new Point (OutsideXDistance(R,P), OutsideYDistance(R,P));

    // This is sort-of the "opposite" of above. In a sense it "captures" the point to the
    // boundary/inside of the rectangle, rather than "excluding" it to the exterior of the rectangle.
    //
    // If the point is outside the rectangle, then it returns the closest location on the
    // rectangle boundary to the Point. If Point is inside Rectangle, then it just returns
    // the point.
    public static Point ClosestBoundaryPoint (this Rectangle R, Point P)
        => new Point (
            Math.Max (Math.Min (P.X, R.Right - 1), R.Left),
            Math.Max (Math.Min (P.Y, R.Bottom - 1), R.Top));

    // In which direction(s) is(are) the point outside of the rectangle? If P is
    // inside R, then this returns (0,0). Else X and/or Y can be either -1 or
    // +1, depending on which direction P is outside R.
    public static Point OutsideDirection (Rectangle R, Point P) => Sign (OutsideDistance (R, P));

    // Return TRUE if the Y value of P is within the Rectangle's Y bounds.
    public static bool ContainsY (Rectangle R, Point P) => (P.Y >= R.Top) && (P.Y < R.Bottom);

    public static bool ContainsX (Rectangle R, Point P) => (P.X >= R.Left) && (P.X < R.Right);
}
}