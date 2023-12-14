using Common.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Common.Collections
{
    /// <summary>
    /// 順序付き辞書クラス
    /// </summary>
    /// <typeparam name="T">任意の型</typeparam>
    public class OrderedDictionary<T> : IDictionary<string, T>, IReadOnlyDictionary<string, T>, IList<T>, IReadOnlyList<T>
    {
        /// <summary>
        /// デフォルトのキープレフィックス
        /// </summary>
        private readonly string DefaultKeyPrefix = "Name";

        private readonly object _LockObj = new object();

        private readonly Dictionary<string, string> _dict = new Dictionary<string, string>();
        private readonly List<string> _ordered = new List<string>();
        private readonly Dictionary<string, T> _storage = new Dictionary<string, T>();
        private int _defaultKeyIndex = 0;

        /// <summary>
        /// Get the value stored in this dictionary from the array index.
        /// </summary>
        /// <param name="index">array index</param>
        /// <returns>Returns the value corresponding to a key.</returns>
        public T this[int index]
        {
            get => _storage[_ordered[index]];
            set
            {
                UpdateItem(value, index);
            }
        }

        /// <summary>
        /// Get the value stored in this dictionary from the key.
        /// </summary>
        /// <param name="key">dictionary key value</param>
        /// <returns>Returns the value corresponding to a key.</returns>
        public T this[string key]
        {
            get => _storage[key];
            set
            {
                UpdateItem(value, key);
            }
        }

        /// <summary>
        /// Get the count for this dictionary.
        /// </summary>
        public int Count => _storage.Count;

        /// <summary>
        /// this dictionary is readonly 
        /// </summary>
        public bool IsReadOnly => ((ICollection<T>)_dict).IsReadOnly;

        /// <summary>
        /// キーコレクションを順序で返します
        /// </summary>
        public ICollection<string> Keys
        {
            get
            {
                return _ordered;
            }
        }

        /// <summary>
        /// 値コレクションを順序で返します
        /// </summary>
        public ICollection<T> Values
        {
            get
            {
                return _ordered.Select(e => _storage[e]).ToList();
            }
        }

        IEnumerable<string> IReadOnlyDictionary<string, T>.Keys
        {
            get
            {
                foreach (var key in _ordered)
                {
                    yield return key;
                }
            }
        }

        IEnumerable<T> IReadOnlyDictionary<string, T>.Values
        {
            get
            {
                foreach (var key in _ordered)
                {
                    yield return  _storage[_dict[key]];
                }
            }
        }

        public OrderedDictionary()
        {
        }

        public OrderedDictionary(string defaultName)
        {
            DefaultKeyPrefix = defaultName;
        }

        private void AddItem(T item, string key)
        {
            string guid = new GuidStore().NewGuid().ToString();
            _dict[key] = guid;
            _ordered.Add(guid);
            _storage[guid] = item;
        }

        private void AddItem(T item)
        {
            string name = GenerateDefaultDictKey();
            AddItem(item, name);
        }

        private int InsertItem(T item, int index, string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                return -1;
            }
            if (index < 0 || index >= _ordered.Count)
            {
                return -2;
            }
            string guid = new GuidStore().NewGuid().ToString();
            _ordered.Insert(index, guid);
            _dict[key] = guid;
            _storage[guid] = item;
            return 0;
        }

        private int InsertItem(T item, int index)
        {
            string name = GenerateDefaultDictKey();
            return InsertItem(item, index, name);
        }

        private bool UpdateItem(T item, int index)
        {
            if (index < 0 || index > _ordered.Count)
            {
                return false;
            }

            _storage[_ordered[index]] = item;
            return true;
        }

        private bool UpdateItem(T item, string key)
        {
            if (!_dict.ContainsKey(key))
            {
                return false;
            }

            _storage[_dict[key]] = item;
            return true;
        }

        private bool RemoveData(int index)
        {
            if (index < 0 || index >= _ordered.Count)
            {
                return false;
            }
            string guid = _ordered[index];
            string key = _dict.First(e => e.Value == guid).Key;
            _dict.Remove(key);
            _ordered.RemoveAt(index);
            _storage.Remove(guid);
            return true;
        }

        private bool RemoveData(string key)
        {
            int index = _ordered.IndexOf(key);
            string guid = _dict[key];
            _dict.Remove(key);
            _ordered.RemoveAt(index);
            _storage.Remove(guid);
            return true;
        }

        private bool RemoveData(T item)
        {
            int index = IndexOf(item);
            if (index == -1)
            {
                return false;
            }
            return RemoveData(index);
        }

        private string GenerateDefaultDictKey()
        {
            string name = DefaultKeyPrefix + _defaultKeyIndex++;
            if (_dict.ContainsKey(name))
            {
                while (true)
                {
                    name = DefaultKeyPrefix + _defaultKeyIndex++;
                    if (!_dict.ContainsKey(name))
                    {
                        break;
                    }
                }
            }
            return name;
        }

        #region interface...

        public void Add(T item)
        {
            AddItem(item);
        }

        public void Add(string key, T item)
        {
            lock(_LockObj)
            {
                AddItem(item, key);
            }
        }

        public void Add(KeyValuePair<string, T> item)
        {
            lock(_LockObj)
            {
                AddItem(item.Value, item.Key);
            }
        }

        public void Append(T item, string key = "")
        {
            lock (_LockObj)
            {
                AddItem(item, key);
            }
        }

        public void Insert(int index, T item)
        {
            lock (_LockObj)
            {
                InsertItem(item, index);
            }
        }

        public void Insert(int index, T item, string name)
        {
            lock (_LockObj)
            {
                InsertItem(item, index, name);
            }
        }

        public bool Contains(T item)
        {
            return _ordered.Equals(item);
        }

        public bool Contains(KeyValuePair<string, T> item)
        {
            if (!_dict.ContainsKey(item.Key))
            {
                return false;
            }
            T data = _storage[_dict[item.Key]];
            return data.Equals(item.Value);
        }

        public bool ContainsKey(string key)
        {
            return _dict.ContainsKey(key);
        }

        public int IndexOf(T item)
        {
            int findIndex = -1;
            for (var i = 0; i < _ordered.Count; i++)
            {
                if (item.Equals(_storage[_ordered[i]]))
                {
                    findIndex = i;
                    break;
                }
            }
            return findIndex;
        }

        public bool TryGetValue(string key, out T value)
        {
            if (!_dict.ContainsKey(key))
            {
                value = default;
                return false;
            }

            value = _storage[_dict[key]];
            return true;
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            if (array == null)
            {
                throw new ArgumentNullException("array");
            }
            if (arrayIndex < 0)
            {
                throw new ArgumentOutOfRangeException("arrayIndex", $"{arrayIndex} is out of range");
            }
            int length = array.Length - arrayIndex;
            if (length > _ordered.Count)
            {
                throw new ArgumentException($"out of range: array_max - arrayIndex = {length}");
            }

            for (var i = 0; i < length; i++)
            {
                array[i + arrayIndex] = _storage[_dict[_ordered[i]]];
            }
        }

        public void CopyTo(KeyValuePair<string, T>[] array, int arrayIndex)
        {
            if (array == null)
            {
                throw new ArgumentNullException("array");
            }
            if (arrayIndex < 0)
            {
                throw new ArgumentOutOfRangeException("arrayIndex", $"{arrayIndex} is out of range");
            }
            int length = array.Length - arrayIndex;
            if (length > _ordered.Count)
            {
                throw new ArgumentException($"out of range: array_max - arrayIndex = {length}");
            }

            for (var i = 0; i < length; i++)
            {
                string key = _ordered[i];
                array[i + arrayIndex] = new KeyValuePair<string, T>(key, _storage[_dict[key]]);
            }
        }

        public bool Remove(T item)
        {
            lock(_LockObj)
            {
                return RemoveData(item);
            }
        }

        public bool Remove(string key)
        {
            lock(_LockObj)
            {
                if (!_dict.ContainsKey(key))
                {
                    return false;
                }

                return RemoveData(key);
            }
        }

        public bool Remove(KeyValuePair<string, T> item)
        {
            lock (_LockObj)
            {
                if (!_dict.ContainsKey(item.Key))
                {
                    return false;
                }

                T data = _storage[_dict[item.Key]];
                if (!data.Equals(item.Value))
                {
                    return false;
                }
                return RemoveData(item.Key);
            }
        }

        public void RemoveAt(int index)
        {
            lock (_LockObj)
            {
                if (index < 0 || index >= _ordered.Count)
                {
                    return;
                }

                RemoveData(index);
            }
        }

        public void Clear()
        {
            lock(_LockObj)
            {
                _dict.Clear();
                _ordered.Clear();
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            foreach (var k in _ordered)
            {
                yield return _storage[_dict[k]];
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        IEnumerator<KeyValuePair<string, T>> IEnumerable<KeyValuePair<string, T>>.GetEnumerator()
        {
            foreach (var k in _ordered)
            {
                yield return new KeyValuePair<string, T>(k, _storage[_dict[k]]);
            }
        }

        public IEnumerable<(string key, int index, T item)> Entries()
        {
            for (var i = 0; i < _ordered.Count; i++)
            {
                string key = _ordered[i];
                yield return (key, i, _storage[_dict[key]]);
            }
        }

        #endregion interface
    }

    ///// <summary>
    ///// Ordered dictionary list class
    ///// </summary>
    ///// <typeparam name="T">Dictionary Value Type</typeparam>
    //public class OrderedDictionary<T> : IDictionary<string, T>, IReadOnlyDictionary<string, T>, IList<T>, IReadOnlyList<T>
    //{
    //    /// <summary>
    //    /// Default dictionary key value.
    //    /// </summary>
    //    private readonly string DefaultKeyPrefix = "Name";

    //    private readonly Dictionary<string, (int X, T Y)> _dict = new Dictionary<string, (int, T)>();
    //    private readonly List<string> _ordered = new List<string>();
    //    private int _defaultKeyIndex = 0;

    //    /// <summary>
    //    /// Get the value stored in this dictionary from the array index.
    //    /// </summary>
    //    /// <param name="index">array index</param>
    //    /// <returns>Returns the value corresponding to a key.</returns>
    //    public T this[int index]
    //    {
    //        get => _dict[_ordered[index]].Y;
    //        set
    //        {
    //            UpdateItem(value, index);
    //        }
    //    }

    //    /// <summary>
    //    /// Get the value stored in this dictionary from the key.
    //    /// </summary>
    //    /// <param name="key">dictionary key value</param>
    //    /// <returns>Returns the value corresponding to a key.</returns>
    //    public T this[string key]
    //    {
    //        get => _dict[key].Y;
    //        set
    //        {
    //            UpdateItem(value, key);
    //        }
    //    }

    //    /// <summary>
    //    /// Get the count for this dictionary.
    //    /// </summary>
    //    public int Count => _dict.Count;

    //    /// <summary>
    //    /// this dictionary is readonly 
    //    /// </summary>
    //    public bool IsReadOnly => ((ICollection<T>)_dict).IsReadOnly;

    //    public ICollection<string> Keys
    //    {
    //        get
    //        {
    //            return _ordered;
    //        }
    //    }

    //    public ICollection<T> Values
    //    {
    //        get
    //        {
    //            return _ordered.ToDictionary(e => e, e => _dict[e].Y).Values;
    //        }
    //    }

    //    IEnumerable<string> IReadOnlyDictionary<string, T>.Keys
    //    {
    //        get
    //        {
    //            foreach (var key in _ordered)
    //            {
    //                yield return key;
    //            }
    //        }
    //    }

    //    IEnumerable<T> IReadOnlyDictionary<string, T>.Values
    //    {
    //        get
    //        {
    //            foreach (var key in _ordered)
    //            {
    //                yield return _dict[key].Y;
    //            }
    //        }
    //    }

    //    public OrderedDictionary()
    //    {
    //    }

    //    public OrderedDictionary(string defaultName)
    //    {
    //        DefaultKeyPrefix = defaultName;
    //    }

    //    private void AddItem(T item, string key)
    //    {
    //        int index;
    //        lock (_ordered)
    //        {
    //            index = _ordered.Count;
    //            _dict[key] = (index, item);
    //            _ordered.Add(key);
    //        }
    //    }

    //    private void AddItem(T item)
    //    {
    //        string name = GenerateDictionaryKey();
    //        AddItem(item, name);
    //    }

    //    private void InsertItem(T item, int index, string key)
    //    {
    //        if (string.IsNullOrEmpty(key))
    //        {
    //            throw new ArgumentNullException("name");
    //        }
    //        if (index < 0 || index >= _ordered.Count)
    //        {
    //            throw new ArgumentOutOfRangeException("index", $"{index} is out of range");
    //        }
    //        _ordered.Insert(index, key);
    //        _dict[key] = (index, item);
    //    }

    //    private void InsertItem(T item, int index)
    //    {
    //        string name = GenerateDictionaryKey();
    //        InsertItem(item, index, name);
    //    }

    //    private bool UpdateItem(T item, int index)
    //    {
    //        if (index < 0 || index > _ordered.Count) return false;

    //        string key = _ordered[index];
    //        _dict[key] = (index, item);
    //        return true;
    //    }

    //    private bool UpdateItem(T item, string key)
    //    {
    //        if (!_dict.ContainsKey(key)) return false;

    //        var index = _dict[key].X;
    //        _dict[key] = (index, item);
    //        return true;
    //    }

    //    private bool RemoveData(int index)
    //    {
    //        if (index < 0 || index >= _ordered.Count)
    //        {
    //            return false;
    //        }
    //        string key = _ordered[index];

    //        lock (_ordered)
    //        {
    //            T backup = _dict[key].Y;
    //            if (!_dict.Remove(key))
    //            {
    //                // rollback
    //                _dict[key] = (index, backup);
    //                return false;
    //            }
    //            _ordered.RemoveAt(index);
    //        }
    //        return true;
    //    }

    //    private bool RemoveData(T item)
    //    {
    //        int index = IndexOf(item);
    //        if (index == -1)
    //        {
    //            return false;
    //        }
    //        return RemoveData(index);
    //    }

    //    private string GenerateDictionaryKey()
    //    {
    //        string name = DefaultKeyPrefix + _defaultKeyIndex++;
    //        if (_dict.ContainsKey(name))
    //        {
    //            while (true)
    //            {
    //                name = DefaultKeyPrefix + _defaultKeyIndex++;
    //                if (!_dict.ContainsKey(name))
    //                {
    //                    break;
    //                }
    //            }
    //        }
    //        return name;
    //    }

    //    public void Add(T item)
    //    {
    //        AddItem(item);
    //    }

    //    public void Add(string key, T item)
    //    {
    //        AddItem(item, key);
    //    }

    //    public void Add(KeyValuePair<string, T> item)
    //    {
    //        AddItem(item.Value, item.Key);
    //    }

    //    public void Append(T item, string key = "")
    //    {
    //        AddItem(item, key);
    //    }

    //    public void Insert(int index, T item)
    //    {
    //        InsertItem(item, index);
    //    }

    //    public void Insert(int index, T item, string name)
    //    {
    //        InsertItem(item, index, name);
    //    }

    //    public bool Contains(T item)
    //    {
    //        return _ordered.Equals(item);
    //    }

    //    public bool Contains(KeyValuePair<string, T> item)
    //    {
    //        if (!_dict.ContainsKey(item.Key))
    //        {
    //            return false;
    //        }
    //        T data = _dict[item.Key].Y;
    //        return data.Equals(item.Value);
    //    }

    //    public bool ContainsKey(string key)
    //    {
    //        return _dict.ContainsKey(key);
    //    }

    //    public int IndexOf(T item)
    //    {
    //        int findIndex = -1;
    //        for (var i = 0; i < _ordered.Count; i++)
    //        {
    //            if (_dict[_ordered[i]].Y.Equals(item))
    //            {
    //                findIndex = i;
    //                break;
    //            }
    //        }
    //        return findIndex;
    //    }

    //    public bool TryGetValue(string key, out T value)
    //    {
    //        if (!_dict.ContainsKey(key))
    //        {
    //            value = default;
    //            return false;
    //        }

    //        value = _dict[key].Y;
    //        return true;
    //    }

    //    public void CopyTo(T[] array, int arrayIndex)
    //    {
    //        if (array == null)
    //        {
    //            throw new ArgumentNullException("array");
    //        }
    //        if (arrayIndex < 0)
    //        {
    //            throw new ArgumentOutOfRangeException("arrayIndex", $"{arrayIndex} is out of range");
    //        }
    //        int length = array.Length - arrayIndex;
    //        if (length > _ordered.Count)
    //        {
    //            throw new ArgumentException($"out of range: array_max - arrayIndex = {length}");
    //        }

    //        for (var i = 0; i < length; i++)
    //        {
    //            array[i + arrayIndex] = _dict[_ordered[i]].Y;
    //        }
    //    }

    //    public void CopyTo(KeyValuePair<string, T>[] array, int arrayIndex)
    //    {
    //        if (array == null)
    //        {
    //            throw new ArgumentNullException("array");
    //        }
    //        if (arrayIndex < 0)
    //        {
    //            throw new ArgumentOutOfRangeException("arrayIndex", $"{arrayIndex} is out of range");
    //        }
    //        int length = array.Length - arrayIndex;
    //        if (length > _ordered.Count)
    //        {
    //            throw new ArgumentException($"out of range: array_max - arrayIndex = {length}");
    //        }

    //        for (var i = 0; i < length; i++)
    //        {
    //            string key = _ordered[i];
    //            array[i + arrayIndex] = new KeyValuePair<string, T>(key, _dict[key].Y);
    //        }
    //    }

    //    public bool Remove(T item)
    //    {
    //        return RemoveData(item);
    //    }

    //    public bool Remove(string key)
    //    {
    //        if (!_dict.ContainsKey(key))
    //        {
    //            return false;
    //        }

    //        int index = _dict[key].X;
    //        return RemoveData(index);
    //    }

    //    public bool Remove(KeyValuePair<string, T> item)
    //    {
    //        if (!_dict.ContainsKey(item.Key))
    //        {
    //            return false;
    //        }

    //        (int index, T data) = _dict[item.Key].ToTuple();
    //        if (!data.Equals(item.Value))
    //        {
    //            return false;
    //        }
    //        return RemoveData(index);
    //    }

    //    public void RemoveAt(int index)
    //    {
    //        if (index < 0 || index >= _ordered.Count)
    //        {
    //            return;
    //        }

    //        RemoveData(index);
    //    }

    //    public void Clear()
    //    {
    //        _dict.Clear();
    //        _ordered.Clear();
    //    }

    //    public IEnumerator<T> GetEnumerator()
    //    {
    //        foreach (var k in _ordered)
    //        {
    //            yield return _dict[k].Y;
    //        }
    //    }

    //    IEnumerator IEnumerable.GetEnumerator()
    //    {
    //        return GetEnumerator();
    //    }

    //    IEnumerator<KeyValuePair<string, T>> IEnumerable<KeyValuePair<string, T>>.GetEnumerator()
    //    {
    //        foreach (var k in _ordered)
    //        {
    //            yield return new KeyValuePair<string, T>(k, _dict[k].Y);
    //        }
    //    }

    //    public IEnumerable<(string key, int index, T item)> Entries()
    //    {
    //        for (var i = 0; i < _ordered.Count; i++)
    //        {
    //            string key = _ordered[i];
    //            yield return (key, i, _dict[key].Y);
    //        }
    //    }
    //}
}
