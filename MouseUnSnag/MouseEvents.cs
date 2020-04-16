using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Microsoft.Win32;
using MouseUnSnag.Contracts;
using MouseUnSnag.Enums;
using MouseUnSnag.Extensions;
using MouseUnSnag.Windows;

namespace MouseUnSnag
{
	/// <summary>
	/// The MouseEvents class deals with the low-level mouse events.
	/// </summary>
	public class MouseEvents
	{
		private IScreenSet _screenSet;
		private bool _updatingDisplaySettings;
		private Kernel32.ConsoleEventDelegate _ctrlCHandler;
		private User32.HookProc _mouseHookDelegate;
		private IntPtr _llMouseHookhand = IntPtr.Zero;
		private IntPtr _thisModHandle = IntPtr.Zero;

		private IntPtr SetHook(int hookNum, User32.HookProc proc)
		{
			using var curProcess = Process.GetCurrentProcess();
			using var curModule = curProcess.MainModule;
			if (_thisModHandle == IntPtr.Zero)
			{
				_thisModHandle = Kernel32.GetModuleHandle(curModule.ModuleName);
			}

			return User32.SetWindowsHookEx(hookNum, proc, _thisModHandle, 0);
		}

		private void UnsetHook(ref IntPtr hookHand)
		{
			if (hookHand != IntPtr.Zero)
			{
				User32.UnhookWindowsHookEx(hookHand);
				hookHand = IntPtr.Zero;
			}
		}

		/// <summary>
		/// Called whenever the mouse moves. This routine leans entirely on the
		/// CheckJumpCursor() routine to see if there is any need to "mess with" the cursor
		/// position, to make it jump from one monitor to another.
		/// </summary>
		private IntPtr LLMouseHookCallback(int nCode, uint wParam, IntPtr lParam)
		{
			if (!(nCode < 0 || wParam != User32.WM_MOUSEMOVE || _updatingDisplaySettings))
			{
				var mouse = ((MSLLHOOKSTRUCT)Marshal.PtrToStructure(lParam, typeof(MSLLHOOKSTRUCT))).pt;

				// If we jump the cursor, then we return 1 here to tell the OS that we
				// have handled the message, so it doesn't call SetCursorPos() right
				// after we do, and "undo" our call to SetCursorPos().
				if (User32.GetCursorPos(out var cursor) && _screenSet.CheckJumpCursor(cursor, mouse, out var newCursor))
				{
					newCursor.SetCursorPos();
					return (IntPtr)1;
				}

				// Default is to let the OS handle the mouse events, when "return" does not happen in
				// if() clause above.
			}
			return User32.CallNextHookEx(_llMouseHookhand, nCode, wParam, lParam);
		}

		private void Event_DisplaySettingsChanged(object sender, EventArgs e)
		{
			_updatingDisplaySettings = true;
			Debug.WriteLine("\nDisplay Settings Changed...");
			Debug.WriteLine(_screenSet = new ScreenSet());
			_updatingDisplaySettings = false;
		}

		private bool ConsoleEventCallback(int eventType)
		{
			Debug.Write("\nIn ConsoleEventCallback, Unhooking mouse events...");
			UnsetHook(ref _llMouseHookhand);
			SystemEvents.DisplaySettingsChanged -= Event_DisplaySettingsChanged;
			Debug.WriteLine("  Done.");
			return false;
		}

		public void Run()
		{
			// DPI Awareness API is not available on older OS's, but they work in
			// physical pixels anyway, so we just ignore if the call fails.
			try
			{
				SHCore.SetProcessDpiAwareness(PROCESS_DPI_AWARENESS.Process_Per_Monitor_DPI_Aware);
			}
			catch (DllNotFoundException)
			{
				Debug.WriteLine("No SHCore.DLL. No problem.");
			}

			// Make sure we catch CTRL-C hard-exit of program.
			_ctrlCHandler = new Kernel32.ConsoleEventDelegate(ConsoleEventCallback);
			Kernel32.SetConsoleCtrlHandler(_ctrlCHandler, true);

			Debug.WriteLine(_screenSet = new ScreenSet());

			// Get notified of any screen configuration changes.
			SystemEvents.DisplaySettingsChanged += Event_DisplaySettingsChanged;

			//User32.ShowWindow(Kernel32.GetConsoleWindow(), User32.SW_HIDE);

			//--Keep a reference to the delegate, so it does not get garbage collected.
			_mouseHookDelegate = LLMouseHookCallback;
			_llMouseHookhand = SetHook(User32.WH_MOUSE_LL, _mouseHookDelegate);

			Debug.WriteLine("");

			// This is the one that runs "forever" while the application is alive, and handles
			// events, etc. This application is ABSOLUTELY ENTIRELY driven by the LLMouseHook
			// and DisplaySettingsChanged events.
			System.Windows.Forms.Application.Run();

			Debug.WriteLine("Exiting!!!");
			UnsetHook(ref _llMouseHookhand);
		}
	}
}