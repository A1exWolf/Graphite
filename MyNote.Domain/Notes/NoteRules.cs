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

        public static void IsValidNameExeption(string name)
        {
            if (name.IndexOf('.') > -1)
                throw new ArgumentException("Имя не может содержать точку");
        }
    }
}