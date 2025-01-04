using System;
using System.Linq;

namespace Nwpie.Foundation.Abstractions.Utilities
{
    public static class IdentifierUtils
    {
        public static string NewId() => NUlid.Ulid.NewUlid().ToString().ToLower();

        public static string RandomString(int length, bool isNumIncluded = true)
        {
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ" + (isNumIncluded ? "0123456789" : "");

            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[Randomizer.Next(s.Length)])
              .ToArray());
        }

        public static string RandomNumber(int length)
        {
            const string chars = "0123456789";

            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[Randomizer.Next(s.Length)])
              .ToArray());
        }

        public static readonly Random Randomizer = new Random();
    }
}
