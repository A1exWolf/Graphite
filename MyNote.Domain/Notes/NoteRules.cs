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
            return !string.IsNullOrWhiteSpace(name);
        }
    }
}