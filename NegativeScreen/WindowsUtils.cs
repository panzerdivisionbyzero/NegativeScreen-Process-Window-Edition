// Copyright 2024 Pawel Witkowski
// https://github.com/panzerdivisionbyzero

// This file is part of NegativeScreen.
// https://github.com/panzerdivisionbyzero/NegativeScreen-Process-Window-Edition

// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.

// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.

// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

namespace NegativeScreen
{
	public static class WindowsUtils
	{
		public static bool IsWindowMinimized(IntPtr windowHandle)
		{
			// https://stackoverflow.com/questions/1003073/how-to-check-whether-another-app-is-minimized-or-not
			WINDOWPLACEMENT placement = new WINDOWPLACEMENT();
			NativeMethods.GetWindowPlacement(windowHandle, ref placement);
			return placement.showCmd == (int)ShowWindowStyles.SW_SHOWMINIMIZED;
		}

		public static string GetClassName(IntPtr hWnd)
		{
			StringBuilder className = new StringBuilder(256);
			NativeMethods.GetClassName(hWnd, className, className.Capacity);
			return className.ToString();
		}

		public static IntPtr FindWindowOfClass(string className, List<IntPtr> windows, int resultsToSkip = 0)
		{
			var skips = 0;
			var index = 0;
			foreach (var w in windows)
			{
				if (GetClassName(w) == className)
				{
					if (skips < resultsToSkip)
					{
						skips++;
						continue;
					}

					Console.WriteLine("className = " + className + "; index = " + index);
					return w;
				}

				index++;
			}

			return IntPtr.Zero;
		}

		public static List<IntPtr> GetAllChildHandles(IntPtr mainWindowHandle)
		{
			List<IntPtr> childHandles = new List<IntPtr>();

			GCHandle gcChildhandlesList = GCHandle.Alloc(childHandles);
			IntPtr pointerChildHandlesList = GCHandle.ToIntPtr(gcChildhandlesList);

			try
			{
				NativeMethods.EnumWindowProc childProc = new NativeMethods.EnumWindowProc(EnumWindow);
				NativeMethods.EnumChildWindows(mainWindowHandle, childProc, pointerChildHandlesList);
			}
			finally
			{
				gcChildhandlesList.Free();
			}

			return childHandles;
		}

		public static bool EnumWindow(IntPtr hWnd, IntPtr lParam)
		{
			GCHandle gcChildhandlesList = GCHandle.FromIntPtr(lParam);

			if (gcChildhandlesList == null || gcChildhandlesList.Target == null)
			{
				return false;
			}

			List<IntPtr> childHandles = gcChildhandlesList.Target as List<IntPtr>;
			childHandles.Add(hWnd);

			return true;
		}

		public static List<IntPtr> EnumerateProcessWindowHandles(Process proc)
		{
			var handles = new List<IntPtr>();

			foreach (ProcessThread thread in proc.Threads)
			{
				NativeMethods.EnumThreadWindows(thread.Id,
					(hWnd, lParam) =>
					{
						handles.Add(hWnd);
						return true;
					}, IntPtr.Zero);
			}

			return handles;
		}

		public static bool ProcessHasAnyWindow(Process proc)
		{
			var result = false;
			foreach (ProcessThread thread in proc.Threads)
			{
				NativeMethods.EnumThreadWindows(thread.Id,
					(hWnd, lParam) =>
					{
						result = true;
						return true;
					}, IntPtr.Zero);
			}

			return result;
		}

		public static IntPtr GetWindowHandle(string title)
		{
			return NativeMethods.FindWindow(null, title);
		}

		public static string GetWindowText(IntPtr hWnd)
		{
			var length = NativeMethods.GetWindowTextLength(hWnd) + 1;
			var title = new StringBuilder(length);
			NativeMethods.GetWindowText(hWnd, title, length);
			return title.ToString();
		}

		public static Process[] GetProcessesByPath(string processPath, string machineName = ".")
		{
			if (processPath == null)
				processPath = string.Empty;
			Process[] processes = Process.GetProcesses(machineName);
			ArrayList arrayList = new ArrayList();
			for (int index = 0; index < processes.Length; ++index)
			{
				var proc = processes[index];

				if (!ProcessHasAnyWindow(proc) || proc.MainWindowHandle == IntPtr.Zero)
				{
					continue;
				}

				var mainModuleAccessible = false;
				try
				{
					mainModuleAccessible = proc.MainModule != null;
				}
				catch
				{
					Console.WriteLine("Cannot access modules of process PID = " + proc.Id);
				}

				if (mainModuleAccessible &&
				    string.Equals(processPath, proc.MainModule.FileName, StringComparison.OrdinalIgnoreCase))
					arrayList.Add(proc);
				else
					processes[index].Dispose();
			}

			Process[] processesByName = new Process[arrayList.Count];
			arrayList.CopyTo(processesByName, 0);
			return processesByName;
		}
	}
}