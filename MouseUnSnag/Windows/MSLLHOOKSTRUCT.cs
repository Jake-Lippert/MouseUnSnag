using System;
using System.Drawing;
using System.Runtime.InteropServices;

namespace MouseUnSnag.Windows
{
	[StructLayout(LayoutKind.Sequential)]
	public struct MSLLHOOKSTRUCT
	{
		public Point pt;
		public uint mouseData;
		public uint flags;
		public uint time;
		public IntPtr dwExtraInfo;
	}
}