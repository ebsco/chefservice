using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace Test
{
    [TestFixture]
    public class Class1
    {
        [Test]
        public void Test()
        {
            ChefService.Arguments sa = new ChefService.Arguments(new[] { "a", "b" });
            //throw new Exception();
        }
    }
}
