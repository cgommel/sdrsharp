using System;
using System.Collections.Generic;

namespace SDRSharp.FrequencyManager
{

    [Serializable]
    public class SerializableDictionary<TKey, TValue>
    {
        private List<TKey> _keys = new List<TKey>();
        private List<TValue> _values = new List<TValue>();

        public bool ContainsKey(TKey key){
            return _keys.IndexOf(key) != -1;
        }

        public TValue this[TKey index]{
            get{
                var idx = _keys.IndexOf(index);
                if (idx != -1)
                    return _values[idx];
                throw new KeyNotFoundException();
            }

            set{
                var idx = _keys.IndexOf(index);
                if (idx == -1)
                {
                    _keys.Add(index);
                    _values.Add(value);
                }
                else
                {
                    _values[idx] = value;
                }
            }
        }

        public void Clear()
        {
            _keys.Clear();
            _values.Clear();
        }

        public List<TKey> Keys
        {
            get { return _keys; }
            set { _keys = value; }
        }

        public List<TValue> Values
        {
            get { return _values; }
            set { _values = value; }
        }
    }
}
