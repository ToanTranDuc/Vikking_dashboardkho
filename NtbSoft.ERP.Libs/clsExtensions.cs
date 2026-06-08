using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NtbSoft.ERP.Libs
{
    public static class clsExtensions
    {
        public static TCollection SkipFirst<T, TCollection>(this ICollection<T> input)
       where TCollection : ICollection<T>, new()
        {
            var processedItems = input.Skip(1);

            var result = new TCollection();
            foreach (T item in processedItems)
            {
                result.Add(item);
            }

            return result;
        }

        /// <summary>
        /// Example: T=clsLineInfo, TCollection=clsLineCollection
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TCollection"></typeparam>
        /// <param name="input"></param>
        /// <returns></returns>
        public static TCollection CastTo<T, TCollection>(this ICollection<T> input)
            where TCollection : ICollection<T>, new()
        {
            var processedItems = input;
            var result = new TCollection();
            foreach (T item in processedItems)
                result.Add(item);

            return result;
        }
    }
}
