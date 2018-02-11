using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DotNetCoreCustomAuthentication.Generators
{
    public static class RandomName
    {
        private readonly static Random R = new Random();

        private readonly static string[] FirstNames = { "Boris", "Charles", "Gary", "David", "Tommy", "Mike" };
        private readonly static string[] LastNames = { "Jones", "Smith", "Lewis", "Lloyd", "Davies", "Markowicz" };

        public static string GetFirstName()
        {
            return FirstNames[R.Next(FirstNames.Length)];
        }

        public static string GetLastName()
        {
            return LastNames[R.Next(LastNames.Length)];
        }

    }
}
