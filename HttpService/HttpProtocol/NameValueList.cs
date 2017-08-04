using System;
using System.Collections.Generic;
using System.Text;

namespace Doms.HttpService.HttpProtocol
{
    /// <summary>
    /// Name-value pair collection
    /// </summary>
    public class NameValueList
    {
        private string[] _itemNames;
        private string[] _itemValues;

        private const int _defaultCapacity = 6;
        private int _size;

        public NameValueList()
        {
            _itemNames = new string[0];
            _itemValues = new string[0];
        }

        #region properties
        public string[] Names
        {
            get { return _itemNames; }
        }

        public string[] Values
        {
            get { return _itemValues; }
        }

        public int Count
        {
            get { return _size; }
        }
        #endregion

        #region methods

        /// <summary>
        /// Append new item
        /// </summary>
        public void Add(string name, string val)
        {
            if (_size == _itemNames.Length)
            {
                ensureCapacity(_size + 1);
            }
            _itemNames[_size] = name;
            _itemValues[_size] = val;
            _size++;
        }

        /// <summary>
        /// Get all match items
        /// </summary>
        public string[] GetValues(string name)
        {
            List<string> result = new List<string>();
            for (int idx = 0; idx < _size; idx++)
            {
                if (String.Compare(_itemNames[idx], name, true) == 0)
                {
                    result.Add(_itemValues[idx]);
                }
            }

            if (result.Count > 0)
            {
                return result.ToArray();
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Get the first match item
        /// </summary>
        public string Get(string name)
        {
            int position = this.IndexOf(name, 0);
            if (position >= 0)
            {
                return _itemValues[position];
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Check the specify name item
        /// </summary>
        public bool Exist(string name)
        {
            return (this.IndexOf(name, 0) >= 0);
        }

        /// <summary>
        /// Update the first match item or add new item that does not exist
        /// </summary>
        public void Set(string name, string val)
        {
            int position = this.IndexOf(name, 0);
            if (position >= 0)
            {
                _itemValues[position] = val;
            }
            else
            {
                this.Add(name, val);
            }
        }

        /// <summary>
        /// Find the first match item with ignore case
        /// </summary>
        public int IndexOf(string name, int index)
        {
            int idx = index;
            for (; idx < _size; idx++)
            {
                if (String.Compare(_itemNames[idx], name, true) == 0)
                {
                    break;
                }
            }

            if (idx < _size)
            {
                return idx;
            }
            else
            {
                return -1;
            }
        }

        /// <summary>
        /// Remove all match items
        /// </summary>
        public void Remove(string name)
        {
            for (int idx = _size - 1; idx >= 0; idx--)
            {
                if (String.Compare(_itemNames[idx], name, true) == 0)
                {
                    this.RemoveAt(idx);
                }
            }
        }

        /// <summary>
        /// Remove item by index
        /// </summary>
        public void RemoveAt(int index)
        {
            if (index >= _size)
            {
                throw new ArgumentOutOfRangeException("index");
            }

            _size--;

            if (index < _size)
            {
                Array.Copy(_itemNames, index + 1, _itemNames, index, _size - index);
                Array.Copy(_itemValues, index + 1, _itemValues, index, _size - index);
            }
            _itemNames[_size] = null;
            _itemValues[_size] = null;
        }

        /// <summary>
        /// Clear all items
        /// </summary>
        public void Clear()
        {
            if (_size > 0)
            {
                Array.Clear(_itemNames, 0, _size);
                Array.Clear(_itemValues, 0, _size);
            }
            _size = 0;
        }

        #endregion

        #region private functions
        private void ensureCapacity(int min)
        {
            if (_itemNames.Length < min)
            {
                int num = (_itemNames.Length == 0) ? _defaultCapacity : (_itemNames.Length * 2);
                if (num < min)
                {
                    num = min;
                }

                string[] destinationArray1 = new string[num];
                string[] destinationArray2 = new string[num];
                if (_size > 0)
                {
                    Array.Copy(_itemNames, 0, destinationArray1, 0, _size);
                    Array.Copy(_itemValues, 0, destinationArray2, 0, _size);
                }
                _itemNames = destinationArray1;
                _itemValues = destinationArray2;
            }
        }
        #endregion

    }//end class
}
