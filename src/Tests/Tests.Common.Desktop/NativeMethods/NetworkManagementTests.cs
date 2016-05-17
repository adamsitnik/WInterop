﻿// ------------------------
//    WInterop Framework
// ------------------------

// Copyright (c) Jeremy W. Kuhne. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Xunit;
using FluentAssertions;
using System.Linq;

namespace WInterop.DesktopTests.NativeMethodTests
{
    public partial class NetworkManagementTests
    {
        [Fact]
        public void BasicGetLocalGroupNames()
        {
            string[] knownLocalGroups = { "Administrators", "Guests", "Users" };
            var localGroups = NetworkManagement.NetworkDesktopMethods.EnumerateLocalGroups();
            localGroups.Should().Contain(knownLocalGroups);
            knownLocalGroups.Should().BeSubsetOf(localGroups);
        }

        [Fact]
        public void BasicGetLocalGroupMembers()
        {
            string[] knownMembers = { "Authenticated Users", "INTERACTIVE" };
            var members = NetworkManagement.NetworkDesktopMethods.EnumerateGroupUsers("Users");
            members.Select(m => m.Name).Should().Contain(knownMembers);
            knownMembers.Should().BeSubsetOf(members.Select(m => m.Name));
        }

        [Fact(Skip = "Need to conditionalize this on admin rights.")]
        public void AddLocalGroup()
        {
            NetworkManagement.NetworkDesktopMethods.AddLocalGroup("TestGroup", "This group is for testing");
        }
    }
}
