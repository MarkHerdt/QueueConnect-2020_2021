using System.Collections.Generic;
using System.Linq;

namespace QueueConnect.ExtensionMethods
{
    public static class ListExtensions
    {
        /// <summary>
        /// Randomly shuffles a List <br/>
        /// Fisher–Yates shuffle
        /// </summary>
        /// <typeparam name="T">Type of the List</typeparam>
        /// <param name="_List">List to shuffle</param>
        public static List<T> ShuffleList<T>(this IList<T> _List)
        {
            var _random = new System.Random();

            var _n = _List.Count;

            while (_n > 1)
            {
                _n--;
                var _k = _random.Next(_n + 1);
                var _value = _List[_k];
                _List[_k] = _List[_n];
                _List[_n] = _value;
            }

            return _List.ToList();
        }
        
        /// <summary>
        /// Moves an item inside a List
        /// </summary>
        /// <param name="_List">The List to move the item in</param>
        /// <param name="_OldIndex">Current index of the item</param>
        /// <param name="_NewIndex">Index where the item should be moved to</param>
        /// <typeparam name="T"></typeparam>
        public static void Move<T>(this List<T> _List, int _OldIndex, int _NewIndex)
        {
            var _item = _List[_OldIndex];
            
            _List.RemoveAt(_OldIndex);
            _List.Insert(_NewIndex, _item);
        }
        
        /// <summary>
        /// Moves an item inside a List
        /// </summary>
        /// <param name="_List">The List to move the item in</param>
        /// <param name="_Item">The item to move</param>
        /// <param name="_NewIndex">Index where the item should be moved to</param>
        /// <typeparam name="T"></typeparam>
        public static void Move<T>(this List<T> _List, T _Item, int _NewIndex)
        {
            _List.Remove(_Item);
            _List.Insert(_NewIndex, _Item);
        }
    }
}