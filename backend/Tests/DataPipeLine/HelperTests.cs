using DataPipeline.Helpers;

namespace Tests.DataPipeline
{

    public class PlayerMatchingTests
    {
        [Theory]
        [InlineData("kennethwalkeriii", "kennethwalker")]
        [InlineData("derrickkellyii", "derrickkelly")]
        [InlineData("larryallenjr", "larryallen")]
        public void RemoveSuffix_PlayersSuffixIsRemoved(string name1, string name2)
        {
            var actual = PlayerMatcher.RemoveSuffixEquality(name1, name2);
            Assert.True(actual);
        }

        [Theory]
        [InlineData("zonovanknight", "bamknight")]
        [InlineData("marquisebrown", "hollywoodbrown")]
        public void CheckAltNameEquality_AlternateNamesMapAndEqual(string name1, string name2)
        {
            var actual = PlayerMatcher.CheckAltNameEquality(name1, name2);
            Assert.True(actual);
        }
    }

    public class HelpersUnitTests
    {
        [Theory]
        [InlineData("Ja&#x27;Marr Chase", "jamarrchase")]
        [InlineData("A.J. Brown", "ajbrown")]
        [InlineData("De&#x27;Von Achane", "devonachane")]
        public void Name_IsNormalized(string input, string expected)
        {
            var actual = NormalizeField.Name(input);
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData("RB5", "RB")]
        [InlineData("TE13", "TE")]
        public void Position_IsNormalized(string input, string expected)
        {
            var actual = NormalizeField.Position(input);
            Assert.Equal(expected, actual);
        }
    }

}