﻿// Copyright (c) Jeremy W. Kuhne. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace WInterop.Console
{
    /// <summary>
    ///  Console input handle modes.
    /// </summary>
    /// <msdn><see cref="https://docs.microsoft.com/en-us/windows/console/getconsolemode"/></msdn>
    [Flags]
    public enum ConsoleInputMode : uint
    {
        /// <summary>
        ///  CTRL+C is processed by the system and is not placed in the input buffer.
        ///  [ENABLE_PROCESSED_INPUT]
        /// </summary>
        EnableProcessedInput = 0x0001,

        /// <summary>
        ///  The ReadFile or ReadConsole function returns only when a carriage return character is
        ///  read. [ENABLE_LINE_INPUT]
        /// </summary>
        EnableLineInput = 0x0002,

        /// <summary>
        ///  Characters read by the ReadFile or ReadConsole function are written to the active screen
        ///  buffer as they are read. Requires <see cref="EnableLineInput"/>. [ENABLE_ECHO_INPUT]
        /// </summary>
        EnableEchoInput = 0x0004,

        /// <summary>
        ///  User interactions that change the size of the console screen buffer are reported in
        ///  the console's input buffer. [ENABLE_WINDOW_INPUT]
        /// </summary>
        EnableWindowInput = 0x0008,

        /// <summary>
        ///  If the mouse pointer is within the borders of the console window and the window has the
        ///  keyboard focus, mouse events generated by mouse movement and button presses are placed
        ///  in the input buffer. [ENABLE_MOUSE_INPUT]
        /// </summary>
        EnableMouseInput = 0x0010,

        /// <summary>
        ///  When enabled, text entered in a console window will be inserted at the current cursor
        ///  location. [ENABLE_INSERT_MODE]
        /// </summary>
        EnableInsertMode = 0x0020,

        /// <summary>
        ///  This flag enables the user to use the mouse to select and edit text. Requires use of
        /// <see cref="EnableExtendedFlags"/>. [ENABLE_QUICK_EDIT_MODE]
        /// </summary>
        EnableQuickEditMode = 0x0040,

        /// <summary>
        ///  Needed for <see cref="EnableQuickEditMode"/> [ENABLE_EXTENDED_FLAGS]
        /// </summary>
        EnableExtendedFlags = 0x0080,

        /// <summary>
        ///  In the Windows headers, but not documented. [ENABLE_AUTO_POSITION]
        /// </summary>
        EnableAutoPosition = 0x0100,

        /// <summary>
        ///  Setting this flag directs the Virtual Terminal processing engine to convert user input
        ///  received by the console window into Console Virtual Terminal Sequences.
        ///  [ENABLE_VIRTUAL_TERMINAL_INPUT]
        /// </summary>
        EnableVirtualTerminalInput = 0x0200
    }
}
