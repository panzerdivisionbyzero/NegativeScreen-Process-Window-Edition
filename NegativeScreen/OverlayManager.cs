//Copyright 2011-2012 Melvyn Laily
//http://arcanesanctum.net

//This file is part of NegativeScreen.

//This program is free software: you can redistribute it and/or modify
//it under the terms of the GNU General Public License as published by
//the Free Software Foundation, either version 3 of the License, or
//(at your option) any later version.

//This program is distributed in the hope that it will be useful,
//but WITHOUT ANY WARRANTY; without even the implied warranty of
//MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//GNU General Public License for more details.

//You should have received a copy of the GNU General Public License
//along with this program.  If not, see <http://www.gnu.org/licenses/>.

//Edited by Zacharia Rudge

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace NegativeScreen
{
	/// <summary>
	/// inherits from Form so that hot keys can be bound to this "window"...
	/// </summary>
	class OverlayManager : Form
	{
		public const int HALT_HOTKEY_ID = 42;//random id =°
		public const int TOGGLE_HOTKEY_ID = 43;
		public const int RESET_TIMER_HOTKEY_ID = 44;
		public const int INCREASE_TIMER_HOTKEY_ID = 45;
		public const int DECREASE_TIMER_HOTKEY_ID = 46;

		//TODO: maybe I should think about loops and config file...
		public const int MODE1_HOTKEY_ID = 51;
		public const int MODE2_HOTKEY_ID = 52;
		public const int MODE3_HOTKEY_ID = 53;
		public const int MODE4_HOTKEY_ID = 54;
		public const int MODE5_HOTKEY_ID = 55;
		public const int MODE6_HOTKEY_ID = 56;
		public const int MODE7_HOTKEY_ID = 57;
		public const int MODE8_HOTKEY_ID = 58;
		public const int MODE9_HOTKEY_ID = 59;
		public const int MODE10_HOTKEY_ID = 60;

		/// <summary>
		/// control whether the main loop is paused or not.
		/// </summary>
		private bool _mainLoopPaused = false;

		private int _waitForWindowTimeMs = Configuration.Current.WaitForWindowTime;
		private int _waitFormWindowTimeAfterClosedMs = Configuration.Current.WaitForWindowTimeAfterClosed;
		private int _mainLoopRefreshTime = Configuration.Current.MainLoopRefreshTime;
		private int _mainLoopPauseRefreshTime = Configuration.Current.MainLoopPauseRefreshTime;
		private NegativeOverlay _overlay;
		private WindowRectLimiter _windowRectLimiter;

		private DateTime _firstFailedWindowCheck = DateTime.MaxValue;
		private bool _resolutionHasChanged = false;

		public OverlayManager()
		{
			if (!NativeMethods.RegisterHotKey(this.Handle, HALT_HOTKEY_ID, KeyModifiers.MOD_WIN | KeyModifiers.MOD_ALT, Keys.H))
			{
				throw new Exception("RegisterHotKey(win+alt+H)", Marshal.GetExceptionForHR(Marshal.GetHRForLastWin32Error()));
			}
			if (!NativeMethods.RegisterHotKey(this.Handle, TOGGLE_HOTKEY_ID, KeyModifiers.MOD_WIN | KeyModifiers.MOD_ALT, Keys.N))
			{
				throw new Exception("RegisterHotKey(win+alt+N)", Marshal.GetExceptionForHR(Marshal.GetHRForLastWin32Error()));
			}
			if (!NativeMethods.RegisterHotKey(this.Handle, RESET_TIMER_HOTKEY_ID, KeyModifiers.MOD_WIN | KeyModifiers.MOD_ALT, Keys.Multiply))
			{
				throw new Exception("RegisterHotKey(win+alt+Multiply)", Marshal.GetExceptionForHR(Marshal.GetHRForLastWin32Error()));
			}
			if (!NativeMethods.RegisterHotKey(this.Handle, INCREASE_TIMER_HOTKEY_ID, KeyModifiers.MOD_WIN | KeyModifiers.MOD_ALT, Keys.Add))
			{
				throw new Exception("RegisterHotKey(win+alt+Add)", Marshal.GetExceptionForHR(Marshal.GetHRForLastWin32Error()));
			}
			if (!NativeMethods.RegisterHotKey(this.Handle, DECREASE_TIMER_HOTKEY_ID, KeyModifiers.MOD_WIN | KeyModifiers.MOD_ALT, Keys.Subtract))
			{
				throw new Exception("RegisterHotKey(win+alt+Substract)", Marshal.GetExceptionForHR(Marshal.GetHRForLastWin32Error()));
			}

			if (!NativeMethods.RegisterHotKey(this.Handle, MODE1_HOTKEY_ID, KeyModifiers.MOD_WIN | KeyModifiers.MOD_ALT, Keys.F1))
			{
				throw new Exception("RegisterHotKey(win+alt+F1)", Marshal.GetExceptionForHR(Marshal.GetHRForLastWin32Error()));
			}
			if (!NativeMethods.RegisterHotKey(this.Handle, MODE2_HOTKEY_ID, KeyModifiers.MOD_WIN | KeyModifiers.MOD_ALT, Keys.F2))
			{
				throw new Exception("RegisterHotKey(win+alt+F2)", Marshal.GetExceptionForHR(Marshal.GetHRForLastWin32Error()));
			}
			if (!NativeMethods.RegisterHotKey(this.Handle, MODE3_HOTKEY_ID, KeyModifiers.MOD_WIN | KeyModifiers.MOD_ALT, Keys.F3))
			{
				throw new Exception("RegisterHotKey(win+alt+F3)", Marshal.GetExceptionForHR(Marshal.GetHRForLastWin32Error()));
			}
			if (!NativeMethods.RegisterHotKey(this.Handle, MODE4_HOTKEY_ID, KeyModifiers.MOD_WIN | KeyModifiers.MOD_ALT, Keys.F4))
			{
				throw new Exception("RegisterHotKey(win+alt+F4)", Marshal.GetExceptionForHR(Marshal.GetHRForLastWin32Error()));
			}
			if (!NativeMethods.RegisterHotKey(this.Handle, MODE5_HOTKEY_ID, KeyModifiers.MOD_WIN | KeyModifiers.MOD_ALT, Keys.F5))
			{
				throw new Exception("RegisterHotKey(win+alt+F5)", Marshal.GetExceptionForHR(Marshal.GetHRForLastWin32Error()));
			}
			if (!NativeMethods.RegisterHotKey(this.Handle, MODE6_HOTKEY_ID, KeyModifiers.MOD_WIN | KeyModifiers.MOD_ALT, Keys.F6))
			{
				throw new Exception("RegisterHotKey(win+alt+F6)", Marshal.GetExceptionForHR(Marshal.GetHRForLastWin32Error()));
			}
			if (!NativeMethods.RegisterHotKey(this.Handle, MODE7_HOTKEY_ID, KeyModifiers.MOD_WIN | KeyModifiers.MOD_ALT, Keys.F7))
			{
				throw new Exception("RegisterHotKey(win+alt+F7)", Marshal.GetExceptionForHR(Marshal.GetHRForLastWin32Error()));
			}
			if (!NativeMethods.RegisterHotKey(this.Handle, MODE8_HOTKEY_ID, KeyModifiers.MOD_WIN | KeyModifiers.MOD_ALT, Keys.F8))
			{
				throw new Exception("RegisterHotKey(win+alt+F8)", Marshal.GetExceptionForHR(Marshal.GetHRForLastWin32Error()));
			}
			if (!NativeMethods.RegisterHotKey(this.Handle, MODE9_HOTKEY_ID, KeyModifiers.MOD_WIN | KeyModifiers.MOD_ALT, Keys.F9))
			{
				throw new Exception("RegisterHotKey(win+alt+F9)", Marshal.GetExceptionForHR(Marshal.GetHRForLastWin32Error()));
			}
			if (!NativeMethods.RegisterHotKey(this.Handle, MODE10_HOTKEY_ID, KeyModifiers.MOD_WIN | KeyModifiers.MOD_ALT, Keys.F10))
			{
				throw new Exception("RegisterHotKey(win+alt+F10)", Marshal.GetExceptionForHR(Marshal.GetHRForLastWin32Error()));
			}

			if (!NativeMethods.MagInitialize())
			{
				throw new Exception("MagInitialize()", Marshal.GetExceptionForHR(Marshal.GetHRForLastWin32Error()));
			}

			Microsoft.Win32.SystemEvents.DisplaySettingsChanged += new EventHandler(SystemEvents_DisplaySettingsChanged);

			Initialization();
		}

		void SystemEvents_DisplaySettingsChanged(object sender, EventArgs e)
		{
			Console.WriteLine(DateTime.Now.ToString());
			//we can't start the loop here, in the event handler, because it seems to block the next events
			_resolutionHasChanged = true;
		}

		private void Initialization()
		{
			_windowRectLimiter = new WindowRectLimiter(Configuration.Current.WindowSidesLimits);
			_overlay = new NegativeOverlay();

			var startedProcHandle = Configuration.Current.ExecuteProcessFromDefinedPath
				? Process.Start(Configuration.Current.QualifiedProcessPath, Configuration.Current.ProcessPathParams)
				: null;

			RefreshLoop(_overlay, startedProcHandle);
		}

		private void RefreshLoop(NegativeOverlay overlay, Process startedProcHandle)
		{
			bool noError = true;
			var currentWaitForWindowTimeMs = _waitForWindowTimeMs;
			while (noError)
			{
				var proc = FindProc(startedProcHandle, Configuration.Current.ProcessName,
					Configuration.Current.QualifiedProcessPath, Configuration.Current.MainWindowClassName,
					out var targetWindowRect, out var childHandles);

				if (proc == null)
				{
					if (_firstFailedWindowCheck == DateTime.MaxValue)
					{
						Console.WriteLine("waiting for window - begin");
						_firstFailedWindowCheck = DateTime.Now;
					}
					if ((int)(DateTime.Now - _firstFailedWindowCheck).TotalMilliseconds > currentWaitForWindowTimeMs)
					{
						return;
					}
					if (this._mainLoopRefreshTime > 0)
					{
						Console.WriteLine("waiting for window...");
						System.Threading.Thread.Sleep(this._mainLoopRefreshTime);
					}

					continue;
				}

				_firstFailedWindowCheck = DateTime.MaxValue;
				currentWaitForWindowTimeMs = _waitFormWindowTimeAfterClosedMs;

				_mainLoopPaused = WindowsUtils.IsWindowMinimized(proc.MainWindowHandle);

				var overlayRect = _windowRectLimiter.LimitRect(targetWindowRect, childHandles);
				Console.WriteLine("overlayRect = " + overlayRect.left + ", " + overlayRect.top + ", " +
				                  overlayRect.right + ", " + overlayRect.bottom);

				overlay.ChangeRect(overlayRect);

				if (_resolutionHasChanged)
				{
					_resolutionHasChanged = false;
					//if the screen configuration change, we try to reinitialize all the overlays.
					//we break the loop. the initialization method is called...
					break;
				}

				noError = RefreshOverlay(overlay);
				if (!noError)
				{
					//application is exiting
					break;
				}

				//Process Window messages
				Application.DoEvents();

				if (this._mainLoopRefreshTime > 0)
				{
					System.Threading.Thread.Sleep(this._mainLoopRefreshTime);
				}

				//pause
				while (_mainLoopPaused)
				{
					_mainLoopPaused = WindowsUtils.IsWindowMinimized(proc.MainWindowHandle);
					Console.WriteLine("proc main window is visible (waiting loop) = " + !_mainLoopPaused);

					if (!_mainLoopPaused)
					{
						overlay.Visible = true;
						break;
					}

					overlay.Visible = false;

					System.Threading.Thread.Sleep(_mainLoopPauseRefreshTime);
					Application.DoEvents();
				}
			}
			if (noError)
			{
				//the loop broke because of a screen resolution change
				Initialization();
			}
		}

		private Process FindProc(Process startedProcHandle, string procNameForSearch, string procPathForSearch,
			string mainWindowClassName, out NativeMethods.windowRECT targetWindowRect, out List<IntPtr> childHandles)
		{
			Process[] processes;
			if (Configuration.Current.FindProcessByPathInsteadName)
			{
				// no need to search process if it's executed by Negative Screen (stored in startedProcHandle)
				processes = Configuration.Current.ExecuteProcessFromDefinedPath
					? startedProcHandle == null || startedProcHandle.HasExited
						? new Process[0]
						: new Process[1] { startedProcHandle }
					: WindowsUtils.GetProcessesByPath(procPathForSearch);
			}
			else
			{
				processes = Process.GetProcessesByName(procNameForSearch);
			}

			Console.WriteLine("foundProcesses.Length = " + processes.Length);

			targetWindowRect = new NativeMethods.windowRECT();

			// find process with visible window:
			// (useful for multi-process apps)
			foreach (var proc in processes)
			{
				Console.WriteLine("procId = " + proc.Id + "; searching for visible window...");
				childHandles = WindowsUtils.EnumerateProcessWindowHandles(proc);
				var targetWindowHandle = mainWindowClassName.Length == 0
					? proc.MainWindowHandle
					: WindowsUtils.FindWindowOfClass(mainWindowClassName, childHandles);

				if (NativeMethods.GetWindowRect(targetWindowHandle, ref targetWindowRect) &&
				    targetWindowRect.right - targetWindowRect.left > 0 &&
				    targetWindowRect.bottom - targetWindowRect.top > 0)
				{
					Console.WriteLine("procId = " + proc.Id + "; procMainWindowHandle = " + proc.MainWindowHandle +
					                  "; targetWindowHandle = " + targetWindowHandle);
					childHandles = WindowsUtils.GetAllChildHandles(targetWindowHandle);
					Console.WriteLine("targetWindowRect = " + targetWindowRect.left + ", " + targetWindowRect.top +
					                  ", " + targetWindowRect.right + ", " + targetWindowRect.bottom);
					return proc;
				}
			}

			childHandles = null;
			return null;
		}

		/// <summary>
		/// return true on success, false on failure.
		/// </summary>
		/// <returns></returns>
		private bool RefreshOverlay(NegativeOverlay overlay)
		{
			try
			{
				// Reclaim topmost status.
				if (!NativeMethods.SetWindowPos(overlay.Handle, NativeMethods.HWND_TOPMOST, 0, 0, 0, 0,
			   (int)SetWindowPosFlags.SWP_NOACTIVATE | (int)SetWindowPosFlags.SWP_NOMOVE | (int)SetWindowPosFlags.SWP_NOSIZE))
				{
					throw new Exception("SetWindowPos()", Marshal.GetExceptionForHR(Marshal.GetHRForLastWin32Error()));
				}
				// Force redraw.
				if (!NativeMethods.InvalidateRect(overlay.HwndMag, IntPtr.Zero, true))
				{
					throw new Exception("InvalidateRect()", Marshal.GetExceptionForHR(Marshal.GetHRForLastWin32Error()));
				}
				return true;
			}
			catch (ObjectDisposedException)
			{
				//application is exiting
				return false;
			}
		}

		private void UnregisterHotKeys()
		{
			NativeMethods.UnregisterHotKey(this.Handle, HALT_HOTKEY_ID);
			NativeMethods.UnregisterHotKey(this.Handle, TOGGLE_HOTKEY_ID);
			NativeMethods.UnregisterHotKey(this.Handle, RESET_TIMER_HOTKEY_ID);
			NativeMethods.UnregisterHotKey(this.Handle, INCREASE_TIMER_HOTKEY_ID);
			NativeMethods.UnregisterHotKey(this.Handle, DECREASE_TIMER_HOTKEY_ID);

			NativeMethods.UnregisterHotKey(this.Handle, MODE1_HOTKEY_ID);
			NativeMethods.UnregisterHotKey(this.Handle, MODE2_HOTKEY_ID);
			NativeMethods.UnregisterHotKey(this.Handle, MODE3_HOTKEY_ID);
			NativeMethods.UnregisterHotKey(this.Handle, MODE4_HOTKEY_ID);
			NativeMethods.UnregisterHotKey(this.Handle, MODE5_HOTKEY_ID);
			NativeMethods.UnregisterHotKey(this.Handle, MODE6_HOTKEY_ID);
			NativeMethods.UnregisterHotKey(this.Handle, MODE7_HOTKEY_ID);
			NativeMethods.UnregisterHotKey(this.Handle, MODE8_HOTKEY_ID);
			NativeMethods.UnregisterHotKey(this.Handle, MODE9_HOTKEY_ID);
			NativeMethods.UnregisterHotKey(this.Handle, MODE10_HOTKEY_ID);
		}

		protected override void WndProc(ref Message m)
		{
			// Listen for operating system messages.
			switch (m.Msg)
			{
				case (int)WindowMessage.WM_DWMCOMPOSITIONCHANGED:
					//aero has been enabled/disabled. It causes the magnified control to stop working
					if (Environment.OSVersion.Version.Major == 6 && Environment.OSVersion.Version.Minor == 0)
					{
						//running Vista.
						//The creation of the magnification Window on this OS seems to change desktop composition,
						//leading to infinite loop
					}
					else
					{
						Initialization();
					}
					break;
				case (int)WindowMessage.WM_HOTKEY:
					switch ((int)m.WParam)
					{
						case HALT_HOTKEY_ID:
							//otherwise, if paused, the application never stops
							_mainLoopPaused = false;
							this.Dispose();
							Application.Exit();
							break;
						case TOGGLE_HOTKEY_ID:
							this._mainLoopPaused = !_mainLoopPaused;
							break;
						case RESET_TIMER_HOTKEY_ID:
							this._mainLoopRefreshTime = Configuration.Current.MainLoopRefreshTime;
							break;
						case INCREASE_TIMER_HOTKEY_ID:
							this._mainLoopRefreshTime += Configuration.Current.MainLoopRefreshTimeIncreaseStep;
							break;
						case DECREASE_TIMER_HOTKEY_ID:
							this._mainLoopRefreshTime -= Configuration.Current.MainLoopRefreshTimeIncreaseStep;
							if (this._mainLoopRefreshTime < 0)
							{
								this._mainLoopRefreshTime = 0;
							}
							break;
						case MODE1_HOTKEY_ID:
							BuiltinMatrices.ChangeColorEffect(_overlay, BuiltinMatrices.Negative);
							break;
						case MODE2_HOTKEY_ID:
							BuiltinMatrices.ChangeColorEffect(_overlay, BuiltinMatrices.NegativeHueShift180);
							break;
						case MODE3_HOTKEY_ID:
							BuiltinMatrices.ChangeColorEffect(_overlay, BuiltinMatrices.NegativeHueShift180Variation1);
							break;
						case MODE4_HOTKEY_ID:
							BuiltinMatrices.ChangeColorEffect(_overlay, BuiltinMatrices.NegativeHueShift180Variation2);
							break;
						case MODE5_HOTKEY_ID:
							BuiltinMatrices.ChangeColorEffect(_overlay, BuiltinMatrices.NegativeHueShift180Variation3);
							break;
						case MODE6_HOTKEY_ID:
							BuiltinMatrices.ChangeColorEffect(_overlay, BuiltinMatrices.NegativeHueShift180Variation4);
							break;
						case MODE7_HOTKEY_ID:
							BuiltinMatrices.ChangeColorEffect(_overlay, BuiltinMatrices.NegativeSepia);
							break;
						case MODE8_HOTKEY_ID:
							BuiltinMatrices.ChangeColorEffect(_overlay, BuiltinMatrices.NegativeGrayScale);
							break;
						case MODE9_HOTKEY_ID:
							BuiltinMatrices.ChangeColorEffect(_overlay, BuiltinMatrices.NegativeRed);
							break;
						case MODE10_HOTKEY_ID:
							BuiltinMatrices.ChangeColorEffect(_overlay, BuiltinMatrices.Red);
							break;
						default:
							break;
					}
					break;
			}
			base.WndProc(ref m);
		}

		protected override void Dispose(bool disposing)
		{
			UnregisterHotKeys();
			NativeMethods.MagUninitialize();

			base.Dispose(disposing);
		}
	}
}
