//-----------------------------------------------------------------------
// <copyright file="NativeMethods.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Microsoft.Multipoint.Sdk.Samples.Common
{
    using System;
    using System.Runtime.InteropServices;

    /// <summary>
    /// Defination of Win32 API calls
    /// </summary>
    internal static class NativeMethods
    {
        /// <summary>
        /// Retrieves the specified system metric or system configuration setting.
        /// </summary>
        /// <param name="which">The system metric or configuration setting to be retrieved. </param>
        /// <returns>If the function succeeds, the return value is the requested system metric or configuration setting.
        /// If the function fails, the return value is 0.</returns>
        [DllImport("user32.dll", EntryPoint = "GetSystemMetrics")]
        internal static extern int GetSystemMetrics(int which);

        /// <summary>
        /// Changes the size, position, and Z order of a child, pop-up, or top-level window. These windows are ordered according to their appearance on the screen. The topmost window receives the highest rank and is the first window in the Z order.
        /// </summary>
        /// <param name="hwnd">A handle to the window.</param>
        /// <param name="hwndInsertAfter">A handle to the window to precede the positioned window in the Z order.</param>
        /// <param name="x">Specifies the new position of the left side of the window, in client coordinates.</param>
        /// <param name="y">Specifies the new position of the top of the window, in client coordinates.</param>
        /// <param name="width">Specifies the new width of the window, in pixels. </param>
        /// <param name="height">Specifies the new height of the window, in pixels. </param>
        /// <param name="flags">Specifies the window sizing and positioning flags. </param>
        [DllImport("user32.dll")]
        internal static extern void SetWindowPos(IntPtr hwnd, int hwndInsertAfter, int x, int y, int width, int height, uint flags);
    }
}
