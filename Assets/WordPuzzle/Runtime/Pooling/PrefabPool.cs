using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Codeabuse.Pooling
{
    public class PrefabPool<T> where T: Object
    {
        public delegate T CreateObject(T prefab, Transform parent);
        
        public PrefabPool(T prefab, Transform storageRoot, int spawnAmount, CreateObject factory = null) 
                : this(prefab, storageRoot, factory)
        {
            SpawnAmount = spawnAmount;
        }
        
        public PrefabPool(T prefab, Transform storageRoot, CreateObject factory = null)
        {
            if (!(typeof(T).IsSubclassOf(typeof(Component)) ||
                  (_isGameObject = typeof(T) == typeof(GameObject))))
            {
                Debug.LogException(new ArgumentException($"{prefab.name} must be Component or GameObject!"));
                return;
            }
            _prefab = prefab;
            _storageRoot = storageRoot;
            _factory = factory ?? Object.Instantiate;
        }

        private readonly bool _isGameObject;

        private int _spawnAmount = 5;
        
        private readonly Stack<T> _instances = new();
        private readonly Transform _storageRoot;
        private readonly T _prefab;

        private readonly CreateObject _factory;

        public bool ActivateOnGet
        {
            get;
            set;
        } = true;

        /// <summary>
        /// How many instances are created when the pool is depleted.
        /// </summary>
        public int SpawnAmount
        {
            get => _spawnAmount;
            set => _spawnAmount = value;
        }

        public int InstancesCount => _instances.Count;

        public Action<T> OnCreate;

        public T Get()
        {
            if (_instances.Count == 0)
                PopulatePool(SpawnAmount);
            var instance = _instances.Pop();
            if (ActivateOnGet)
            {
                ActivateGameObject(instance, true);
            }
            if (instance is IPooledBehavior pooledBehavior)
                pooledBehavior.OnGet();
            return instance;
        }

        public void PopulatePool(int count = 5)
        {
            for (var i = 0; i < count; i++)
            {
                var instance = _factory(_prefab, _storageRoot);
                ActivateGameObject(instance, false);
                _instances.Push(instance);
                OnCreate?.Invoke(instance);
            }
        }

        public void Release(T instance)
        {
            _instances.Push(instance);
            
            if (instance is IPooledBehavior pooledBehavior)
                pooledBehavior.OnRelease();
            ActivateGameObject(instance, false);
            GetGameObject(instance).transform.SetParent(_storageRoot);
        }

        public void Clear()
        {
            foreach (var instance in _instances)
            {
                Object.Destroy(GetGameObject(instance));
            }
        }

        private void ActivateGameObject(T instance, bool value)
        {
            GetGameObject(instance).SetActive(value);
        }

        private GameObject GetGameObject(T instance)
        {
            return _isGameObject ? (instance as GameObject) : (instance as Component)?.gameObject;
        }
    }
}