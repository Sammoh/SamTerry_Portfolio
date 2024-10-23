using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Generic object pooler for reusing objects.
/// </summary>
/// <typeparam name="T">Type of the object to pool.</typeparam>
namespace Sammoh.Two
{
    public class ObjectPooler<T> where T : Component
    {
        private readonly Func<T> _createFunc;
        private readonly Queue<T> _objects = new Queue<T>();

        public ObjectPooler(Func<T> createFunc, int initialSize)
        {
            _createFunc = createFunc;

            for (int i = 0; i < initialSize; i++)
            {
                T obj = _createFunc();
                obj.gameObject.SetActive(false);
                _objects.Enqueue(obj);
            }
        }

        /// <summary>
        /// Gets an object from the pool.
        /// </summary>
        /// <returns>An instance of the pooled object.</returns>
        public T Get()
        {
            if (_objects.Count == 0)
            {
                AddObjects(1);
            }

            T obj = _objects.Dequeue();
            obj.gameObject.SetActive(true);
            return obj;
        }
        
        /// <summary>
        /// Gets all objects from the pool.
        /// </summary>
        /// <returns>An instance of the pooled object.</returns>
        public T[] GetAll()
        {
            T[] objects = _objects.ToArray();
            foreach (T obj in objects)
            {
                obj.gameObject.SetActive(true);
            }
            _objects.Clear();
            return objects;
        }

        /// <summary>
        /// Returns an object to the pool.
        /// </summary>
        /// <param name="obj">The object to return to the pool.</param>
        public void ReturnToPool(T obj)
        {
            if (obj == null) return; // Add this line to guard against null

            obj.gameObject.SetActive(false);
            _objects.Enqueue(obj);
        }

        /// <summary>
        /// Adds more objects to the pool.
        /// </summary>
        /// <param name="count">Number of objects to add.</param>
        private void AddObjects(int count)
        {
            for (int i = 0; i < count; i++)
            {
                T obj = _createFunc();
                obj.gameObject.SetActive(false);
                _objects.Enqueue(obj);
            }
        }

        /// <summary>
        /// Removes all objects from the pool.
        /// </summary>
        /// <param name="destroy">Whether to destroy the objects.</param>
        public void Clear(bool destroy = false)
        {
            if (destroy)
            {
                while (_objects.Count > 0)
                {
                    UnityEngine.Object.Destroy(_objects.Dequeue().gameObject);
                }
            }
            else
            {
                _objects.Clear();
            }
        }
        
        /// <summary>
        /// Organizes the objects in the pool.
        /// </summary>
        /// <param name="reorderFunc">The function to reorder the objects.</param>
        public void Reorder(Func<T, T, int> reorderFunc)
        {
            T[] objects = _objects.ToArray();
            Array.Sort(objects, (a, b) => reorderFunc(a, b));
            _objects.Clear();
            foreach (T obj in objects)
            {
                _objects.Enqueue(obj);
            }
        }
    }
}