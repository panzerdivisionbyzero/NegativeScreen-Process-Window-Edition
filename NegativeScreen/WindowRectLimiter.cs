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
using System.Collections.Generic;

namespace NegativeScreen
{
	public class WindowRectLimiter
	{
		private List<WindowSideLimit> _configs;

		public WindowRectLimiter(List<WindowSideLimit> configs)
		{
			_configs = configs;
		}

		private struct ControlData
		{
			public IntPtr Handle;
			public NativeMethods.windowRECT Rect;
		}

		public NativeMethods.windowRECT LimitRect(NativeMethods.windowRECT mainWindowRect, List<IntPtr> childHandles)
		{
			var result = mainWindowRect;
			var controlsData = new Dictionary<string, ControlData>();

			foreach (var config in _configs)
			{
				if (!controlsData.TryGetValue(config.WindowClassName, out var currentControlData))
				{
					currentControlData = new ControlData
					{
						Handle = WindowsUtils.FindWindowOfClass(config.WindowClassName, childHandles,
							config.WindowClassFoundIndex)
					};

					if (currentControlData.Handle == IntPtr.Zero ||
					    !NativeMethods.GetWindowRect(currentControlData.Handle, ref currentControlData.Rect))
					{
						continue;
					}

					controlsData.Add(config.WindowClassName, currentControlData);
				}

				var controlSideValue = GetControlSideValue(config.WindowClassSide, currentControlData.Rect);

				switch (config.RectSide)
				{
					case RectSides.top:
						result.top = controlSideValue + config.ConstMargin;
						break;
					case RectSides.right:
						result.right = controlSideValue - config.ConstMargin;
						break;
					case RectSides.bottom:
						result.bottom = controlSideValue - config.ConstMargin;
						break;
					default:
						result.left = controlSideValue + config.ConstMargin;
						break;
				}
			}

			return result;
		}

		private int GetControlSideValue(RectSides controlSide, NativeMethods.windowRECT controlRect)
		{
			switch (controlSide)
			{
				case RectSides.top: return controlRect.top;
				case RectSides.right: return controlRect.right;
				case RectSides.bottom: return controlRect.bottom;
				default: return controlRect.left;
			}
		}
	}
}