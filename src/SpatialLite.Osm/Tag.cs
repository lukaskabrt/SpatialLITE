using System;

namespace SpatialLite.Osm {
    /// <summary>
    /// Represents OSM tag and it's value.
    /// </summary>
    public class Tag {

		private string _key;
		private string _value;

		/// <summary>
		/// Initializes a new instance of the Tag class with specified key and value.
		/// </summary>
		/// <param name="key">The key of the Tag.</param>
		/// <param name="value">The value of the Tag.</param>
		public Tag(string key, string value) {
			if (key == null) {
				throw new ArgumentNullException("Parameter 'key' can't be null.");
			}

			if (key == string.Empty) {
				throw new ArgumentException("Parameter 'key' can't be empty string.");
			}

			if (value == null) {
				throw new ArgumentNullException("Parameter 'value' can't be null.");
			}

			_key = key;
			_value = value;
		}

		/// <summary>
		/// Gets the key of the tag.
		/// </summary>
		public string Key {
			get {
				return _key;
			}
		}

		/// <summary>
		/// Gets the value of the tag.
		/// </summary>
		public string Value {
			get {
				return _value;
			}
		}

		/// <summary>
		/// Compares the current Tag object with the specified object for equivalence.
		/// </summary>
		/// <param name="obj">The object to test for equivalence with the current Tag object.</param>
		/// <returns>true if the objects are equal, otherwise returns false.</returns>
		public override bool Equals(object obj) {
			if (obj == null) {
				return false;
			}

			Tag other = obj as Tag;
			if (other != null) {
				return Equals(other);
			}
			else {
				return false;
			}
		}

		/// <summary>
		/// Compares the current Tag object with the specified Tag.
		/// </summary>
		/// <param name="other">The Tag to test for equivalence with the current Tag object.</param>
		/// <returns>true if the objects are equal, otherwise returns false.</returns>
		public bool Equals(Tag other) {
			if (other == null) {
				return false;
			}

			return _key.Equals(other._key) && _value.Equals(other._value);
		}

		/// <summary>
		/// Returns the hash code for the current object.
		/// </summary>
		/// <returns>An integer hash code.</returns>
		public override int GetHashCode() {
			return unchecked(_key.GetHashCode() * 83 + _value.GetHashCode());
		}
	}
}
