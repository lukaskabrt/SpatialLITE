using System;
using System.Collections;
using System.Collections.Generic;

namespace SpatialLite.Graphs.Algorithms.AStar {
    class BinaryHeap<T> : ICollection<T>, IEnumerable<T> {
		private List<T> _storage;

		private Comparer<T> _comparer;

		public int Count => _storage.Count;

		public bool IsReadOnly => false;

		public BinaryHeap() {
			_storage = new List<T>(65535);
			_comparer = Comparer<T>.Default;
		}

		public T Peek() {
			if (_storage.Count == 0) {
				throw new InvalidOperationException("The heap is empty");
			}
			return _storage[0];
		}

		public T RemoveTop() {
			if (_storage.Count == 0) {
				throw new InvalidOperationException("The heap is empty.");
			}
			T result = _storage[0];
			T item = _storage[_storage.Count - 1];
			_storage.RemoveAt(_storage.Count - 1);
			if (_storage.Count > 0) {
				BubbleDown(item, 0);
			}
			return result;
		}

		protected void BubbleDown(T item, int index) {
			int num = index;
			while (num < _storage.Count / 2) {
				int num2 = 2 * num + 1;
				if (num2 < _storage.Count - 1 && _comparer.Compare(_storage[num2], _storage[num2 + 1]) > 0) {
					num2++;
				}
				if (_comparer.Compare(_storage[num2], item) >= 0) {
					break;
				}
				_storage[num] = _storage[num2];
				num = num2;
			}
			_storage[num] = item;
		}

		protected void BubleUp(T item, int index) {
			while (index > 0 && _comparer.Compare(_storage[(index - 1) / 2], item) > 0) {
				_storage[index] = _storage[(index - 1) / 2];
				index = (index - 1) / 2;
			}
			_storage[index] = item;
		}

		public void Add(T item) {
			int count = _storage.Count;
			_storage.Add(item);
			BubleUp(item, count);
		}

		public void Clear() {
			_storage.Clear();
		}

		public bool Contains(T item) {
			return _storage.Contains(item);
		}

		public void CopyTo(T[] array, int arrayIndex) {
			_storage.CopyTo(array, arrayIndex);
		}

		public bool Remove(T item) {
			int num = _storage.IndexOf(item);
			if (num > -1) {
				T val = _storage[_storage.Count - 1];
				_storage.RemoveAt(_storage.Count - 1);
				if (_storage.Count > 0) {
					int num2 = (num - 1) / 2;
					if (num2 >= 0 && _comparer.Compare(_storage[num2], val) > 0) {
						BubleUp(val, num);
					} else if (num < _storage.Count) {
						BubbleDown(val, num);
					}
				}
				return true;
			}
			return false;
		}

		public IEnumerator<T> GetEnumerator() {
			return _storage.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator() {
			return _storage.GetEnumerator();
		}
	}
}
