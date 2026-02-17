using UnityEngine;
using System.Collections.Generic;

public sealed class ObjectPool<T> : IPool<T> where T : Component
    {
        private readonly T _prefab;
        private readonly Transform _parent;
        private readonly Stack<T> _items;

        public ObjectPool(T prefab, int prewarm, Transform parent)
        {
            _prefab = prefab;
            _parent = parent;
            _items = new Stack<T>(Mathf.Max(0, prewarm));

            for (int i = 0; i < prewarm; i++)
            {
                var it = CreateNew();
                Return(it);
            }
        }

        public T Get()
        {
            var it = _items.Count > 0 ? _items.Pop() : CreateNew();
            it.gameObject.SetActive(true);

            if (it.TryGetComponent<IPoolable>(out var p))
                p.OnGetFromPool();

            return it;
        }

        public void Release(T item)
        {
            if (item == null) return;

            if (item.TryGetComponent<IPoolable>(out var p))
                p.OnReturnToPool();

            Return(item);
        }

        private T CreateNew()
        {
            var it = Object.Instantiate(_prefab, _parent);
            it.gameObject.SetActive(false);
            return it;
        }

        private void Return(T item)
        {
            item.transform.SetParent(_parent, false);
            item.gameObject.SetActive(false);
            _items.Push(item);
        }
    }

