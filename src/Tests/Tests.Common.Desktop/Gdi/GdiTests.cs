﻿// ------------------------
//    WInterop Framework
// ------------------------

// Copyright (c) Jeremy W. Kuhne. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using FluentAssertions;
using System.Linq;
using WInterop.Gdi;
using WInterop.Gdi.DataTypes;
using WInterop.Windows.DataTypes;
using Xunit;

namespace DesktopTests.Gdi
{
    public class GdiTests
    {
        [Fact]
        public void EnumerateDisplayDevices()
        {
            var devices = GdiDesktopMethods.EnumerateDisplayDevices(null).ToArray();
            devices.Should().Contain(d => (d.StateFlags & (DeviceState.DISPLAY_DEVICE_ACTIVE | DeviceState.DISPLAY_DEVICE_PRIMARY_DEVICE)) ==
                (DeviceState.DISPLAY_DEVICE_ACTIVE | DeviceState.DISPLAY_DEVICE_PRIMARY_DEVICE));
        }

        [Fact]
        public void EnumerateDisplayDevices_Monitors()
        {
            var device = GdiDesktopMethods.EnumerateDisplayDevices(null).First();
            var monitor = GdiDesktopMethods.EnumerateDisplayDevices(device.DeviceName).First();

            // Something like \\.\DISPLAY1 and \\.\DISPLAY1\Monitor0
            monitor.DeviceName.Should().StartWith(device.DeviceName);
        }

        [Fact]
        public void EnumerateDisplaySettings_Null()
        {
            var settings = GdiDesktopMethods.EnumerateDisplaySettings(null).ToArray();
            settings.Should().NotBeEmpty();
        }

        [Fact]
        public void EnumerateDisplaySettings_FirstDevice()
        {
            var device = GdiDesktopMethods.EnumerateDisplayDevices(null).First();
            var settings = GdiDesktopMethods.EnumerateDisplaySettings(device.DeviceName);
            settings.Should().NotBeEmpty();
        }

        [Fact]
        public void EnumerateDisplaySettings_FirstDevice_CurrentMode()
        {
            var device = GdiDesktopMethods.EnumerateDisplayDevices(null).First();
            var settings = GdiDesktopMethods.EnumerateDisplaySettings(device.DeviceName, GdiDesktopMethods.Defines.ENUM_CURRENT_SETTINGS).ToArray();
            settings.Length.Should().Be(1);
        }

        [Fact]
        public void GetDeviceContext_NullWindow()
        {
            // Null here should be the entire screen
            DeviceContext context = GdiDesktopMethods.GetDeviceContext(WindowHandle.NullWindowHandle);
            context.IsInvalid.Should().BeFalse();
            int pixelWidth = GdiDesktopMethods.GetDeviceCapability(context, DeviceCapability.HORZRES);
            int pixelHeight = GdiDesktopMethods.GetDeviceCapability(context, DeviceCapability.VERTRES);
        }

        [Fact]
        public void GetWindowDeviceContext_NullWindow()
        {
            // Null here should be the entire screen
            DeviceContext context = GdiDesktopMethods.GetWindowDeviceContext(WindowHandle.NullWindowHandle);
            context.IsInvalid.Should().BeFalse();
            int pixelWidth = GdiDesktopMethods.GetDeviceCapability(context, DeviceCapability.HORZRES);
            int pixelHeight = GdiDesktopMethods.GetDeviceCapability(context, DeviceCapability.VERTRES);
        }
    }
}
