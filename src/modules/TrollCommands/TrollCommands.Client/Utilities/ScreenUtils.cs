using System;
using System.Runtime.InteropServices;

namespace TrollCommands.Client.Utilities
{
    public static class ScreenUtils
    {
        [Flags]
        public enum RedrawWindowFlags : uint
        {
            /// <summary>
            ///     Invalidates the rectangle or region that you specify in lprcUpdate or hrgnUpdate.
            ///     You can set only one of these parameters to a non-NULL value. If both are NULL, RDW_INVALIDATE invalidates the
            ///     entire window.
            /// </summary>
            Invalidate = 0x1,

            /// <summary>
            ///     Causes the OS to post a WM_PAINT message to the window regardless of whether a portion of the window is
            ///     invalid.
            /// </summary>
            InternalPaint = 0x2,

            /// <summary>
            ///     Causes the window to receive a WM_ERASEBKGND message when the window is repainted.
            ///     Specify this value in combination with the RDW_INVALIDATE value; otherwise, RDW_ERASE has no effect.
            /// </summary>
            Erase = 0x4,

            /// <summary>
            ///     Validates the rectangle or region that you specify in lprcUpdate or hrgnUpdate.
            ///     You can set only one of these parameters to a non-NULL value. If both are NULL, RDW_VALIDATE validates the entire
            ///     window.
            ///     This value does not affect internal WM_PAINT messages.
            /// </summary>
            Validate = 0x8,

            NoInternalPaint = 0x10,

            /// <summary>Suppresses any pending WM_ERASEBKGND messages.</summary>
            NoErase = 0x20,

            /// <summary>Excludes child windows, if any, from the repainting operation.</summary>
            NoChildren = 0x40,

            /// <summary>Includes child windows, if any, in the repainting operation.</summary>
            AllChildren = 0x80,

            /// <summary>
            ///     Causes the affected windows, which you specify by setting the RDW_ALLCHILDREN and RDW_NOCHILDREN values, to
            ///     receive WM_ERASEBKGND and WM_PAINT messages before the RedrawWindow returns, if necessary.
            /// </summary>
            UpdateNow = 0x100,

            /// <summary>
            ///     Causes the affected windows, which you specify by setting the RDW_ALLCHILDREN and RDW_NOCHILDREN values, to receive
            ///     WM_ERASEBKGND messages before RedrawWindow returns, if necessary.
            ///     The affected windows receive WM_PAINT messages at the ordinary time.
            /// </summary>
            EraseNow = 0x200,

            Frame = 0x400,

            NoFrame = 0x800
        }

        public enum TernaryRasterOperations : uint
        {
            SRCCOPY = 0x00CC0020,
            SRCPAINT = 0x00EE0086,
            SRCAND = 0x008800C6,
            SRCINVERT = 0x00660046,
            SRCERASE = 0x00440328,
            NOTSRCCOPY = 0x00330008,
            NOTSRCERASE = 0x001100A6,
            MERGECOPY = 0x00C000CA,
            MERGEPAINT = 0x00BB0226,
            PATCOPY = 0x00F00021,
            PATPAINT = 0x00FB0A09,
            PATINVERT = 0x005A0049,
            DSTINVERT = 0x00550009,
            BLACKNESS = 0x00000042,
            WHITENESS = 0x00FF0062,
            CAPTUREBLT = 0x40000000 //only if WinVer >= 5.0.0 (see wingdi.h)
        }

        /// <summary>
        ///     Creates a bitmap compatible with the device that is associated with the specified device context.
        /// </summary>
        /// <param name="hdc">A handle to a device context.</param>
        /// <param name="nWidth">The bitmap width, in pixels.</param>
        /// <param name="nHeight">The bitmap height, in pixels.</param>
        /// <returns>
        ///     If the function succeeds, the return value is a handle to the compatible bitmap (DDB). If the function fails,
        ///     the return value is <see cref="IntPtr.Zero" />.
        /// </returns>
        [DllImport("gdi32.dll", EntryPoint = "CreateCompatibleBitmap")]
        public static extern IntPtr CreateCompatibleBitmap([In] IntPtr hdc, int nWidth, int nHeight);

        [DllImport("gdi32.dll", EntryPoint = "CreateCompatibleDC", SetLastError = true)]
        public static extern IntPtr CreateCompatibleDC([In] IntPtr hdc);

        [DllImport("gdi32.dll", ExactSpelling = true, SetLastError = true)]
        public static extern IntPtr SelectObject(IntPtr hdc, IntPtr hgdiobj);

        [DllImport("gdi32.dll", ExactSpelling = true, SetLastError = true)]
        public static extern bool DeleteObject(IntPtr hObject);

        [DllImport("gdi32.dll", EntryPoint = "BitBlt", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool BitBlt([In] IntPtr hdc, int nXDest, int nYDest, int nWidth, int nHeight, [In] IntPtr hdcSrc, int nXSrc, int nYSrc,
            TernaryRasterOperations dwRop);

        [DllImport("user32.dll", SetLastError = false)]
        public static extern IntPtr GetDesktopWindow();

        [DllImport("user32.dll")]
        public static extern IntPtr GetWindowDC(IntPtr hWnd);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool GetWindowRect(IntPtr hwnd, out RECT lpRect);

        [DllImport("gdi32.dll", ExactSpelling = true, SetLastError = true)]
        public static extern bool DeleteDC(IntPtr hdc);

        [DllImport("user32.dll")]
        public static extern bool ReleaseDC(IntPtr hWnd, IntPtr hDC);

        [DllImport("gdi32.dll")]
        public static extern bool StretchBlt(IntPtr hdcDest, int nXOriginDest, int nYOriginDest, int nWidthDest, int nHeightDest, IntPtr hdcSrc,
            int nXOriginSrc, int nYOriginSrc, int nWidthSrc, int nHeightSrc, TernaryRasterOperations dwRop);

        [DllImport("user32.dll")]
        public static extern bool RedrawWindow(IntPtr hWnd, IntPtr lprcUpdate, IntPtr hrgnUpdate, RedrawWindowFlags flags);


        public static void Initialize(out IntPtr hwnd, out IntPtr hdc, out RECT rect)
        {
            hwnd = GetDesktopWindow();
            hdc = GetWindowDC(hwnd);
            GetWindowRect(hwnd, out rect);
        }

        public static void Destroy(IntPtr hwnd, IntPtr hdc)
        {
            ReleaseDC(hwnd, hdc);
        }

        public static void ClearScreen()
        {
            RedrawWindow(IntPtr.Zero, IntPtr.Zero, IntPtr.Zero,
                RedrawWindowFlags.Invalidate | RedrawWindowFlags.Erase | RedrawWindowFlags.AllChildren);
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int Left; // x position of upper-left corner
            public int Top; // y position of upper-left corner
            public int Right; // x position of lower-right corner
            public int Bottom; // y position of lower-right corner

            public int Width => Right - Left;
            public int Height => Bottom - Top;
        }
    }
}