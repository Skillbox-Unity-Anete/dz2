using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Homework_Theme_01
{
    public static class Extensions
    {
        public static void ForEach<T>(this IEnumerable<T> @enum, Action<T,int> action)
        {
            var i = 0; //i потому что Count может быть затратным
            foreach (var elem in @enum)
            {
                action(elem,i);
                i++;
            }
        }
    }
}
