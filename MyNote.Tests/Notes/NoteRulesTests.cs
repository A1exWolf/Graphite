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
        [InlineData("My note.md", false)]
        public void IsValidName_ReturnsExpectedResult(string? name, bool expected)
        {
            var result = NoteRules.IsValidName(name);

            Assert.Equal(expected, result);
        }

        // [Theory]
        // [InlineData("My note.md")]
        // [InlineData("My notemd")]
        // public void EnsureValidName_ReturnsArgumentIfError(string name)
        // {
        //     Assert.Throws<ArgumentException>(() => NoteRules.EnsureValidName(name));
        // }

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