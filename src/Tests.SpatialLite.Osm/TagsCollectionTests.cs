using SpatialLite.Osm;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Tests.SpatialLite.Osm;

public class TagsCollectionTests
{
    private readonly Tag[] _tags = new Tag[] {
        new("test-key-1", "test-value-1"),
        new("test-key-2", "test-value-2"),
        new("test-key-3", "test-value-3")
    };

    private readonly Tag[] _tagsDuplicitKeys = new Tag[] {
        new("test-key-1", "test-value-1"),
        new("test-key-1", "test-value-2")
    };

    [Fact]
    public void Constructor_CreatesEmptyTagsCollection()
    {
        TagsCollection target = new();

        Assert.Empty(target);
    }

    [Fact]
    public void Constructor_IEnumerable_CreatesCollectionWithGivetTags()
    {
        TagsCollection target = new(_tags);

        Assert.Equal(_tags.Count(), target.Count());
        Assert.Contains(_tags[0], target);
        Assert.Contains(_tags[1], target);
    }

    [Fact]
    public void Constructor_IEnumerable_ThrowsExceptionWithDuplicateKeys()
    {
        Assert.Throws<ArgumentException>(() => new TagsCollection(_tagsDuplicitKeys));
    }

    [Fact]
    public void Add_AddsTag()
    {
        TagsCollection target = new();
        target.Add(_tags[0]);

        Assert.Contains(_tags[0], target);
    }

    [Fact]
    public void Add_ThrowsExceptionIfTagAlreadyPresent()
    {
        TagsCollection target = new();
        target.Add(_tagsDuplicitKeys[0]);

        Assert.Throws<ArgumentException>(delegate { target.Add(_tagsDuplicitKeys[1]); });
    }

    [Fact]
    public void Clear_DoesNothingOnEmptyCollection()
    {
        TagsCollection target = new();

        target.Clear();
    }

    [Fact]
    public void Clear_RemovesAllItems()
    {
        TagsCollection target = new(_tags);
        target.Clear();

        Assert.Empty(target);
    }

    [Fact]
    public void Contains_Tag_ReturnsFalseForEmptyCollection()
    {
        TagsCollection target = new();

        Assert.DoesNotContain(new Tag("key", "value"), target);
    }

    [Fact]
    public void Contains_Tag_ReturnsFalseIfCollectionDoesNotContainTag()
    {
        TagsCollection target = new(_tags);

        Tag testTag = new("test-key-1", "other-value");
        Assert.DoesNotContain(testTag, target);
    }

    [Fact]
    public void Contains_Tag_ReturnsTrueIfCollectionContainsTag()
    {
        TagsCollection target = new(_tags);

        Assert.Contains(_tags[0], target);
    }

    [Fact]
    public void Contains_string_ReturnsFalseForEmptyCollection()
    {
        TagsCollection target = new();

        Assert.False(target.Contains("key"));
    }

    [Fact]
    public void Contains_string_ReturnsFalseIfCollectionDoesNotContainTag()
    {
        TagsCollection target = new(_tags);

        Assert.False(target.Contains("non-existing-key"));
    }

    [Fact]
    public void Contains_string_ReturnsTrueIfCollectionContainsTag()
    {
        TagsCollection target = new(_tags);

        Assert.True(target.Contains(_tags[0].Key));
    }

    [Fact]
    public void CopyTo_ThrowsArgumentNullExceptionIfArrayIsNull()
    {
        TagsCollection target = new(_tags);

        Assert.Throws<ArgumentNullException>(() => target.CopyTo(null, 0));
    }

    [Fact]
    public void CopyTo_ThrowsArgumentOutOfRangeExceptionIfIndexIsLessThenZero()
    {
        Tag[] array = new Tag[5];
        TagsCollection target = new(_tags);

        Assert.Throws<ArgumentOutOfRangeException>(() => target.CopyTo(array, -4));
    }

    [Fact]
    public void CopyTo_ThrowsArgumentExceptionIfSpaceDesignedForCollectionInArrayIsShort()
    {
        Tag[] array = new Tag[5];
        TagsCollection target = new(_tags);

        Assert.Throws<ArgumentException>(() => target.CopyTo(array, 4));
    }

    [Fact]
    public void CopyTo_CopiesElementsToArray()
    {
        Tag[] array = new Tag[5];
        TagsCollection target = new(_tags);

        target.CopyTo(array, 1);

        Assert.Null(array[0]);
        Assert.Same(_tags[0], array[1]);
        Assert.Same(_tags[1], array[2]);
        Assert.Same(_tags[2], array[3]);
        Assert.Null(array[4]);
    }

    [Fact]
    public void GetTag_ReturnsNullForEmptyCollection()
    {
        TagsCollection target = new();

        Assert.Throws<ArgumentException>(() => target.GetTag("other-key"));
    }

    [Fact]
    public void GetTag_ReturnsNullIfCollectionDoesNotContainTag()
    {
        TagsCollection target = new(_tags);

        Assert.Throws<ArgumentException>(() => target.GetTag("other-key"));
    }

    [Fact]
    public void GetTag_ReturnsTagIfCollectionContainsTag()
    {
        TagsCollection target = new(_tags);
        Tag returned = target.GetTag(_tags[0].Key);

        Assert.Same(_tags[0], returned);
    }

    [Fact]
    public void Remove_Tag_ReturnsFalseForEmptyCollection()
    {
        TagsCollection target = new();
        bool removed = target.Remove(new Tag("key", "value"));

        Assert.False(removed);
    }

    [Fact]
    public void Remove_Tag_DoNothingAndReturnsFalseIfCollectionDoesNotContainTag()
    {
        TagsCollection target = new(_tags);

        Tag testTag = new("other-key", "other-value");
        Assert.False(target.Remove(testTag));
        CompareCollections(_tags, target);
    }

    [Fact]
    public void Remove_Tag_RemovesItemAndReturnsTrueIfCollectionContainsTag()
    {
        TagsCollection target = new(_tags);

        Assert.True(target.Remove(_tags[0]));
        CompareCollections(_tags.Skip(1), target);
    }

    [Fact]
    public void Remove_string_ReturnsFalseForEmptyCollection()
    {
        TagsCollection target = new();
        bool removed = target.Remove("key");

        Assert.False(removed);
    }

    [Fact]
    public void Remove_string_DoNothingAndReturnsFalseIfCollectionDoesNotContainTag()
    {
        Tag[] tags = new Tag[] { new("test-key-1", "test-value"), new("test-key-2", "test-value") };

        TagsCollection target = new(tags);

        Assert.False(target.Remove("non-existing-tag"));
    }

    [Fact]
    public void Remove_string_RemovesItemAndReturnsTrueIfCollectionContainsTag()
    {
        Tag[] tags = new Tag[] { new("test-key-1", "test-value"), new("test-key-2", "test-value") };

        TagsCollection target = new(tags);

        Assert.True(target.Remove(tags[1].Key));
        Assert.Single(target);
        Assert.Contains(tags[0], target);
    }

    [Fact]
    public void Count_ReturnsZeroForEmptyCollection()
    {
        TagsCollection target = new();

        Assert.Empty(target);
    }

    [Fact]
    public void Count_ReturnsTagsCount()
    {
        TagsCollection target = new(_tags);

        Assert.Equal(_tags.Length, target.Count);
    }

    [Fact]
    public void IsReadOnly_ReturnsFalse()
    {
        TagsCollection target = new();

        Assert.False(target.IsReadOnly);
    }

    [Fact]
    public void Item_Get_ThrowsExceptionForEmptyCollection()
    {
        TagsCollection target = new();

        Assert.Throws<ArgumentException>(() => target["other-key"]);
    }

    [Fact]
    public void Item_Get_ThrowsExceptionIfCollectionDoesNotContainTag()
    {
        TagsCollection target = new(_tags);

        Assert.Throws<ArgumentException>(() => target["other-key"]);
    }

    [Fact]
    public void Item_Get_ReturnsTagValueIfCollectionContainsTag()
    {
        TagsCollection target = new(_tags);
        string returnedValue = target[_tags[0].Key];

        Assert.Equal(_tags[0].Value, returnedValue);
    }

    [Fact]
    public void Item_Set_AddsItemToEmptyCollection()
    {
        TagsCollection target = new();

        target["test-key"] = "test-value";

        Assert.Single(target);
        Assert.Equal("test-value", target["test-key"]);
    }

    [Fact]
    public void Item_Set_AddsItemToCollection()
    {
        TagsCollection target = new(_tags);

        target["test-key-101"] = "test-value-101";

        Assert.Equal(_tags.Length + 1, target.Count);
        Assert.Equal("test-value-101", target["test-key-101"]);
    }

    [Fact]
    public void Item_Set_SetsTagValue()
    {
        TagsCollection target = new(_tags);
        target[_tags[0].Key] = "new-value";

        Assert.Equal("new-value", target[_tags[0].Key]);
    }

    private void CompareCollections(IEnumerable<Tag> expected, IEnumerable<Tag> actual)
    {
        Assert.Equal(expected.Count(), actual.Count());
        Assert.True(expected.All(tag => actual.Contains(tag)));
    }
}
