//-----------------------------------------------------------------------
// <copyright file="MouseHelper.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Microsoft.Multipoint.Sdk.Samples.Common
{
    using System;
    using System.Collections.Specialized;
    using System.Windows;
    using System.Windows.Interop;
    using Sdk;

    /// <summary>
    /// A helper class that contains various functions related to Multipoint functionality
    /// </summary>
    public static class MouseHelper
    {
        /// <summary>
        /// Metric to get the width of the screen in pixels.
        /// </summary>
        private const int SmCxScreen = 0;

        /// <summary>
        /// Metric to get the height of the screen in pixels.
        /// </summary>
        private const int SmCyScreen = 1;

        /// <summary>
        /// Displays the window.
        /// </summary>
        private const int SwpNoActivate = 0x0010;

        /// <summary>
        /// Places the window at the top of the Z order.
        /// </summary>
        private const int HwndTop = 0;
        
        /// <summary>
        /// Check if there are too many mice, if so disable extras
        /// </summary>
        /// <param name="maxPlayers">The maximum number of allowed players</param>
        /// <returns>true if too many mice, otherwise false</returns>
        public static bool CheckIfTooManyMice(int maxPlayers)
        {
            // Too many mice - disable extras
            if (MultipointSdk.Instance.MouseDeviceList.Count > maxPlayers)
            {
                // Disable extra mice.  
                for (int i = maxPlayers; i < MultipointSdk.Instance.MouseDeviceList.Count - 1; i++)
                {
                    MultipointSdk.Instance.MouseDeviceList[i].DeviceVisual.Visible = false;
                }

                return true;
            }

            return false;
        }

        /// <summary>
        /// Excludes the specified mouse devices from being displayed by Multipoint.
        /// </summary>
        /// <param name="excludedDevices">A list of hardware ids obtained from device manager to excluded.</param>
        /// <remarks>
        /// It's possible that there may be devices present on the system whose drivers are incorrectly 
        /// indentifying themselves as mouse devices, or other pointing devices which are currently 
        /// disconnected (such as a Wacom tablet), that will appear to Windows as connected 
        /// mouse devices. This will cause Multipoint to draw additional and unwanted pointers. 
        /// During the creation of this sample the keyboard driver registered itself as a mouse device 
        /// with Windows and needed to be disabled.<para/>
        /// Devices can be indentified by checking <see cref="DeviceInfo.DeviceName"/>, this property
        /// contains a concatenation of the driver details, one of these being the hardware id of the device.
        /// The hardware id can be found using the device manager in Windows, by expanding the 
        /// 'Mice and other pointing devices" node and right clicking on the mouse device. This will 
        /// display the properties dialog for mouse, the hardware id can be found in the 'Details' tab
        /// </remarks>
        public static void ExcludeDevices(StringCollection excludedDevices)
        {
            if (excludedDevices == null)
            {
                throw new ArgumentNullException("excludedDevices");
            }

            foreach (DeviceInfo device in MultipointSdk.Instance.MouseDeviceList)
            {
                foreach (string excludedDevice in excludedDevices)
                {
                    if (device.DeviceName.Contains(excludedDevice))
                    {
                        device.DeviceVisual.Visible = false;
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Hides the task bar.
        /// </summary>
        /// <param name="window">The window to be re-sized.</param>       
        public static void HideTaskbar(Window window)
        {
            var cx = NativeMethods.GetSystemMetrics(SmCxScreen);
            var cy = NativeMethods.GetSystemMetrics(SmCyScreen);
            window.WindowState = WindowState.Normal;
            NativeMethods.SetWindowPos(new WindowInteropHelper(window).Handle, HwndTop, 0, 0, cx, cy, SwpNoActivate);
        }
    }
}
