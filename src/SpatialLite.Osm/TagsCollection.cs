using System;
using System.Collections.Generic;
using System.Linq;

namespace SpatialLite.Osm {
    /// <summary>
    /// Represents collection of Tags that are accesible by their key.
    /// </summary>
    public class TagsCollection : ICollection<Tag> {

		private List<Tag> _tags;

		/// <summary>
		/// Initializes a new instance of the TagsCollection class.
		/// </summary>
		public TagsCollection() {
		}

		/// <summary>
		/// Initializes a new instance of the TagsCollection class with specified tags.
		/// </summary>
		/// <param name="tags">Collection of tags.</param>
		public TagsCollection(IEnumerable<Tag> tags) {
			_tags = new List<Tag>(tags.Count());

			foreach (var tag in tags) {
				this.Add(tag);
			}
		}

		/// <summary>
		/// Gets the number of Tags in the TagsCollection.
		/// </summary>
		public int Count {
			get {
				if (_tags == null) {
					return 0;
				}
				else {
					return _tags.Count;
				}
			}
		}

		/// <summary>
		/// Gets a value indicating whether the TagsCollection is read-only.
		/// </summary>
		public bool IsReadOnly {
			get { return false; }
		}

		/// <summary>
		/// Gets or sets value of the Tag with given key.
		/// </summary>
		/// <param name="key">The string key of Tag.</param>
		/// <returns>The value of the Tag or null if Tag isn't found.</returns>
		public string this[string key] {
			get {
				Tag tag = this.GetTag(key);
				return tag.Value;
			}
			set {
				if (this.Contains(key)) {
					this.Remove(key);
					this.Add(new Tag(key, value));
				}
				else {
					this.Add(new Tag(key, value));
				}
			}
		}

		/// <summary>
		/// Adds Tag to the TagsCollection.
		/// </summary>
		/// <param name="tag">The Tag to add to the TagsCollection.</param>
		public void Add(Tag tag) {
			if (_tags == null) {
				_tags = new List<Tag>();
			}

			if (this.Contains(tag.Key)) {
				throw new ArgumentException(string.Format("Tag with key '{0}' is already present in the collection.", tag.Key));
			}

			_tags.Add(tag);
		}

		/// <summary>
		/// Removes all items from the TagsCollection.
		/// </summary>
		public void Clear() {
			if (_tags != null) {
				_tags.Clear();
			}
		}

		/// <summary>
		/// Determines whether the TagsCollection contains a specific value.
		/// </summary>
		/// <param name="item">The Tag to locate in the TagsCollection.</param>
		/// <returns>true if item is found in the TagsCollection; otherwise, false.</returns>
		public bool Contains(Tag item) {
			if (_tags == null) {
				return false;
			}

			return _tags.Contains(item);
		}

		/// <summary>
		/// Determines whether the TagsCollection contains a Tag with specified key.
		/// </summary>
		/// <param name="key">The key of a Tag to locate in the TagsCollection.</param>
		/// <returns>true if item is found in the TagsCollection; otherwise, false.</returns>
		public bool Contains(string key) {
			if (_tags == null) {
				return false;
			}

			for (int i = 0; i < _tags.Count; i++) {
				if (_tags[i].Key == key) {
					return true;
				}
			}

			return false;
		}

		/// <summary>
		/// opies the entire TagCollection to a compatible one-dimensional Array, starting at the specified index of the target array.
		/// </summary>
		/// <param name="array">The one-dimensional Array that is the destination of the elements copied from TagCollection.</param>
		/// <param name="arrayIndex">The zero-based index in array at which copying begins.</param>
		public void CopyTo(Tag[] array, int arrayIndex) {
			if (_tags != null) {
				_tags.CopyTo(array, arrayIndex);
			}
		}

		/// <summary>
		/// Gets Tag with the specified key from the collection.
		/// </summary>
		/// <param name="key">The key of the Tag.</param>
		/// <returns>Tag with specified Key or null, if the Tag is not found in the TagsCollection.</returns>
		public Tag GetTag(string key) {
			if (_tags == null) {
				throw new ArgumentException(string.Format("Tag with key '{0}' is not present in the collection.", key));
			}

			for (int i = 0; i < _tags.Count; i++) {
				if (_tags[i].Key == key) {
					return _tags[i];
				}
			}

			throw new ArgumentException(string.Format("Tag with key '{0}' is not present in the collection.", key));
		}

		/// <summary>
		/// Removes the specific Tag from the TagsCollection.
		/// </summary>
		/// <param name="item">The Tag to remove from the TagsCollection.</param>
		/// <returns>true if item was successfully removed from the TagsCollection, otherwise false. This method also returns false if item is not found in the original TagsCollection.</returns>
		public bool Remove(Tag item) {
			if (_tags == null) {
				return false;
			}

			return _tags.Remove(item);
		}

		/// <summary>
		/// Removes the Tag with specified key from the TagsCollection.
		/// </summary>
		/// <param name="key">The Key of the Tag to remove from the TagsCollection.</param>
		/// <returns>true if item was successfully removed from the TagsCollection, otherwise false. This method also returns false if item is not found in the original TagsCollection.</returns>
		public bool Remove(string key) {
			if (this.Contains(key)) {
				Tag tag = this.GetTag(key);
				return this.Remove(tag);
			}
			else {
				return false;
			}
		}

		/// <summary>
		/// Returns an generic enumerator that iterates through a collection.
		/// </summary>
		/// <returns>An generic IEnumerator object that can be used to iterate through the collection.</returns>
		public IEnumerator<Tag> GetEnumerator() {
			if (_tags == null) {
				return this.GetEmptyEnumerator();
			}
			else {
				return _tags.GetEnumerator();
			}
		}

		/// <summary>
		/// Returns an enumerator that iterates through a collection.
		/// </summary>
		/// <returns>An IEnumerator object that can be used to iterate through the collection.</returns>
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
			if (_tags == null) {
				return this.GetEmptyEnumerator();
			}
			else {
				return _tags.GetEnumerator();
			}
		}

		/// <summary>
		/// Returns Enumerator for empty collection.
		/// </summary>
		/// <returns>Enumerator that returns no items.</returns>
		private IEnumerator<Tag> GetEmptyEnumerator() {
			yield break;
		}
	}
}
