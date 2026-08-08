using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyNote.Domain.Notes
{
    public static class NoteRules
    {
        public static bool IsValidName(string? name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return false;

            if (name.IndexOf('.') > -1)
                return false;

            return true;
        }

        /// <summary>
        /// Ensure valid name
        /// </summary>
        /// <param name="name"></param>
        /// <exception cref="ArgumentException"></exception>
        public static void EnsureValidNameNote(string name)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(name);

            var invalidPathChars = Path.GetInvalidFileNameChars();

            foreach (var someChar in invalidPathChars)
            {
                if (name.IndexOf(someChar) > -1)
                    throw new ArgumentException($"The file name cannot contain the following characters {string.Join(' ', invalidPathChars)}");
            }
        }
    }
}