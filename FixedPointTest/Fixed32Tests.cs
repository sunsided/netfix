using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentAssertions;
using NUnit.Framework;

namespace ConsoleApplication1
{
    [TestFixture]
    public class Fixed32Tests
    {
        [Test]
        public void AddingYieldsCorrectResult([Random(Int32.MinValue, Int32.MaxValue, 1)] int a, [Random(Int32.MinValue, Int32.MaxValue, 1)]int b)
        {
            var fa = Fixed32.FromInteger(a);
            var fb = Fixed32.FromInteger(b);
            var fc = fa + fb;
            fc.ToInt32.Should().Be(a + b);
        }

        [Test]
        public void MultiplicationOfTwoValuesYieldsCorrectResult([Values(-1, 0, 1)] int a, [Values(-42, 0, 42)]  int b)
        {
            var fa = Fixed32.FromInteger(a);
            var fb = Fixed32.FromInteger(b);
            var fc = fa * fb;
            fc.ToInt32.Should().Be(a * b);
        }


    }
}
