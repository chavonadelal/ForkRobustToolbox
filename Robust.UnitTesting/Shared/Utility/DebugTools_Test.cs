﻿using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using NUnit.Framework;
using Robust.Shared.Utility;

namespace Robust.UnitTesting.Shared.Utility;

[TestFixture]
[Parallelizable(ParallelScope.All)]
[TestOf(typeof(DebugTools))]
public sealed class DebugTools_Test
{
    [Test]
    [TestCase(true, 5)]
    public void TestAssertFormatNoAlloc(bool check, int val)
    {
        // FUN FACT! This test is fragile as hell!
        // The JIT is allowed to re-order allocations since it's not really a side effect it cares about.
        // I had the boxing allocation at the bottom be re-ordered to be before the GetAllocatedBytesForCurrentThread().

        var delta = AssertAaaah(check, val);

        Assert.That(delta, Is.Zero);
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static long AssertAaaah(bool check, int val)
    {
        var allocA = GC.GetAllocatedBytesForCurrentThread();

        AssertWrap(check, val);

        var allocB = GC.GetAllocatedBytesForCurrentThread();

        return allocB - allocA;
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static void AssertWrap(bool check, int val)
    {
        DebugTools.Assert(check, $"Oops: {val}");
    }
}
