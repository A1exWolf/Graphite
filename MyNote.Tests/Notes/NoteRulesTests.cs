using MyNote.Domain.Notes;

namespace MyNote.Tests.Notes
{
    public sealed class NoteRulesTests
    {
        [Theory]
        [InlineData(null, false)]
        [InlineData("", false)]
        [InlineData("   ", false)]
        [InlineData("My Note", true)]
        public void IsValidName_ReturnsExpectedResult(string? name, bool expected)
        {
            var result = NoteRules.IsValidName(name);

            Assert.Equal(expected, result);
        }

        [Fact]
        public void IsValidName_WhenNameIsEmpty_ReturnsFalse()
        {
            var name = string.Empty;

            var result = NoteRules.IsValidName(name);

            Assert.False(result);
        }

        [Fact]
        public void IsValidName_WhenNameIsValid_ReturnsTrue()
        {
            var name = "My Note";

            var result = NoteRules.IsValidName(name);

            Assert.True(result);
        }
    }
}