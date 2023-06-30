using NUnit.Framework;

namespace Erudite1603;

public class SimpleTests
{
    [TestFixture]
    public class Erudite1603Tests
    {
        [Test]
        public void BasicTests()
        {
        
            var lettersArray = new String[,]
            {
                {"a","b","r","a"},
                {"a","d","a","c"},
                {"b","a","b","r"},
                {"a","r","c","a"}
            
            };
            var words = new string[]
            {
                "abracadabra",
                "ababaab",
                "ababaaba",
                "rbaadara",
                "arbcrad",
                "adacc"
            };
            var wordsResult =  new string[]
            {
                "abracadabra: YES",
                "ababaab: YES",
                "ababaaba: NO",
                "rbaadara: YES",
                "arbcrad: YES",
                "adacc: NO"
            };
            Assert.AreEqual( wordsResult, Program.MainProgram(words, lettersArray));
        }
    }
}