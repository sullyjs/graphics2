using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Template
{/*
    public class Node<T>
    {
        public T Value { get; private set; }
        public IList<Node<T>> Children { get; private set; }

        public Node(T value, IEnumerable<Node<T>> children)
        {
            this.Value = value;
            this.Children = new List<Node<T>>(children);
        }

        public static IEnumerable<Node<T>> Hierarchize<T, TKey, TOrderKey>(
        this IEnumerable<T> elements,
        TKey topMostKey,
        Func<T, TKey> keySelector,
        Func<T, TKey> parentKeySelector,
        Func<T, TOrderKey> orderingKeySelector)
        {
            var families = elements.ToLookup(parentKeySelector);
            var childrenFetcher = default(Func<TKey, IEnumerable<Node<T>>>);
            childrenFetcher = parentId => families[parentId]
                .OrderBy(orderingKeySelector)
                .Select(x => new Node<T>(x, childrenFetcher(keySelector(x))));

            return childrenFetcher(topMostKey);
        }
    }*/
}