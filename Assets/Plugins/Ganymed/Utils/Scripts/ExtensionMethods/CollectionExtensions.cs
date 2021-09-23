using System.Collections.Generic;
using UnityEngine;

namespace Ganymed.Utils.ExtensionMethods
{
    public static class CollectionExtensions
    {
        /// <summary>
        /// Returns Count - 1
        /// </summary>
        /// <param name="inspected"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static int Indices<T>(this IList<T> inspected)
            => inspected.Count - 1;

        /// <summary>
        /// Add the value to the list if the list does not already contain it.
        /// </summary>
        /// <param name="target"></param>
        /// <param name="value"></param>
        /// <typeparam name="T"></typeparam>
        public static void AddIfNotInList<T>(this IList<T> target, T value)
        {
            if(target.IsNull() || IsValueNull(value)) return;
            if (target.Contains(value)) return;
            target.Add(value);
        }

        /// <summary>
        /// Remove the value from the list if it does exist.
        /// </summary>
        /// <param name="target"></param>
        /// <param name="value"></param>
        /// <typeparam name="T"></typeparam>
        public static void DeleteIfExists<T>(this IList<T> target, T value)
        {
            if(target.IsNull() || value.IsValueNull()) return;
            if (!target.Contains(value)) return;
            target.Remove(value);
        }

        /// <summary>
        /// Returns true if the list is null, false if the list is not null.
        /// </summary>
        /// <param name="target"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static bool IsNull<T>(this IList<T> target)
        {
            return target == null;
        }
        
        /// <summary>
        /// Returns false if the value is null, false if the value is not null.
        /// </summary>
        /// <param name="value"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static bool IsValueNull<T>(this T value)
        {
            return value == null;
        }


        public static T GetRandomArrayElement<T>(this T[] array)
        {
            return array[Random.Range(0, array.Length)];
        }
    }
}
