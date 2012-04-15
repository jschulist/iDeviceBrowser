using System;
using System.Collections.Generic;
using System.Text;

// FROM SYSTEM.CORE IN .NET 3.5, SLIGHTLY MODIFIED TO REDUCE DEPENDENCIES
namespace System.Linq
{
    internal struct Buffer<TElement>
    {
        internal TElement[] items;
        internal int count;
        internal Buffer(IEnumerable<TElement> source)
        {
            TElement[] array = null;
            int num = 0;
            ICollection<TElement> collection = source as ICollection<TElement>;
            if (collection != null)
            {
                num = collection.Count;
                if (num > 0)
                {
                    array = new TElement[num];
                    collection.CopyTo(array, 0);
                }
            }
            else
            {
                foreach (TElement current in source)
                {
                    if (array == null)
                    {
                        array = new TElement[4];
                    }
                    else
                    {
                        if (array.Length == num)
                        {
                            TElement[] array2 = new TElement[checked(num * 2)];
                            Array.Copy(array, 0, array2, 0, num);
                            array = array2;
                        }
                    }
                    array[num] = current;
                    num++;
                }
            }
            this.items = array;
            this.count = num;
        }

        internal TElement[] ToArray()
        {
            if (this.count == 0)
            {
                return new TElement[0];
            }
            if (this.items.Length == this.count)
            {
                return this.items;
            }
            TElement[] array = new TElement[this.count];
            Array.Copy(this.items, 0, array, 0, this.count);
            return array;
        }
    }

    public class Enumerable
    {
        public static TSource[] ToArray<TSource>(IEnumerable<TSource> source)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            return new Buffer<TSource>(source).ToArray();
        }

        public static int Count<TSource>(IEnumerable<TSource> source)
        {
            checked
            {
                if (source == null)
                {
                    throw new ArgumentNullException("source");
                }
                ICollection<TSource> collection = source as ICollection<TSource>;
                if (collection != null)
                {
                    return collection.Count;
                }
                int num = 0;
                using (IEnumerator<TSource> enumerator = source.GetEnumerator())
                {
                    while (enumerator.MoveNext())
                    {
                        num++;
                    }
                }
                return num;
            }
        }

        private static IEnumerable<TSource> SkipIterator<TSource>(IEnumerable<TSource> source, int count)
        {
            using (IEnumerator<TSource> enumerator = source.GetEnumerator())
            {
                while (count > 0 && enumerator.MoveNext())
                {
                    count--;
                }
                if (count <= 0)
                {
                    while (enumerator.MoveNext())
                    {
                        yield return enumerator.Current;
                    }
                }
            }
            yield break;
        }

        public static IEnumerable<TSource> Skip<TSource>(IEnumerable<TSource> source, int count)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            return Enumerable.SkipIterator<TSource>(source, count);
        }
    }
}
