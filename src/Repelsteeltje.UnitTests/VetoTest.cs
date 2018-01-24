using NUnit.Framework;

namespace Repelsteeltje.UnitTests
{
    [TestFixture]
    public class VetoTest
    {
        [Test]
        public void Regex_Matches()
        {
            Veto veto = "/^C/";
            var act = veto.Pattern.IsMatch("Corniel");
            var exp = true;

            Assert.AreEqual(exp, act);
        }
    }
}
