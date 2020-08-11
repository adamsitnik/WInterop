﻿// Copyright (c) Jeremy W. Kuhne. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Drawing;
using WInterop.Errors;
using WInterop.Gdi;
using WInterop.Modules;
using WInterop.Windows.Native;

namespace WInterop.Windows
{
    public static partial class Windows
    {
        public static unsafe CursorHandle LoadCursor(CursorId id)
        {
            HCURSOR handle = Imports.LoadCursorW(ModuleInstance.Null, (char*)(uint)id);
            if (handle.IsInvalid)
                Error.ThrowLastError(id.ToString());

            return new CursorHandle(handle, ownsHandle: false);
        }

        public static CursorHandle LoadCursorFromFile(string path)
        {
            HCURSOR handle = Imports.LoadCursorFromFileW(path);
            if (handle.IsInvalid)
                Error.ThrowLastError(path);

            return new CursorHandle(handle, ownsHandle: false);
        }

        public static CursorHandle SetCursor(CursorHandle cursor)
        {
            return new CursorHandle(Imports.SetCursor(cursor), ownsHandle: false);
        }

        /// <summary>
        ///  Replaces the specified system cursor with the given cursor. The cursor will
        ///  be destroyed and as such must not be loaded from a resource. Use CopyCursor
        ///  on cursors loaded from resources before calling this method.
        /// </summary>
        public static void SetSystemCursor(CursorHandle cursor, SystemCursor id)
            => Error.ThrowLastErrorIfFalse(Imports.SetSystemCursor(cursor, id));

        public static int ShowCursor(bool show)
        {
            return Imports.ShowCursor(show);
        }

        public static CursorHandle CopyCursor(CursorHandle cursor)
        {
            CursorHandle copy = Imports.CopyCursor(cursor);
            if (copy.IsInvalid)
                Error.ThrowLastError();

            return copy;
        }

        public static Point GetCursorPosition()
        {
            Error.ThrowLastErrorIfFalse(
                Imports.GetCursorPos(out Point point));

            return point;
        }

        public static Point GetPhysicalCursorPosition()
        {
            Error.ThrowLastErrorIfFalse(
                Imports.GetPhysicalCursorPos(out Point point));

            return point;
        }

        public static void SetCursorPosition(Point point)
            => Error.ThrowLastErrorIfFalse(Imports.SetCursorPos(point.X, point.Y));

        public static void SetPhysicalCursorPosition(Point point)
            => Error.ThrowLastErrorIfFalse(Imports.SetPhysicalCursorPos(point.X, point.Y));

        public static void CreateCaret(this in WindowHandle window, BitmapHandle bitmap, Size size)
            => Error.ThrowLastErrorIfFalse(Imports.CreateCaret(window, bitmap, size.Width, size.Height));

        public static void DestroyCaret()
            => Error.ThrowLastErrorIfFalse(Imports.DestroyCaret());

        public static void SetCaretPosition(Point point)
            => Error.ThrowLastErrorIfFalse(Imports.SetCaretPos(point.X, point.Y));

        public static void ShowCaret(this in WindowHandle window)
            => Error.ThrowLastErrorIfFalse(Imports.ShowCaret(window));

        public static void HideCaret(this in WindowHandle window)
            => Error.ThrowLastErrorIfFalse(Imports.HideCaret(window));

        public static Rectangle GetClipCursor()
        {
            Error.ThrowLastErrorIfFalse(Imports.GetClipCursor(out Rect rect));
            return rect;
        }
    }
}
