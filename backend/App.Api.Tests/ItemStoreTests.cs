using App.Api.Services;

namespace App.Api.Tests;

public class ItemStoreTests
{
    [Fact]
    public void NewStore_IsSeededWithItems()
    {
        var store = new ItemStore();

        Assert.NotEmpty(store.All());
    }

    [Fact]
    public void All_ReturnsItemsNewestFirst()
    {
        var store = new ItemStore();

        var items = store.All().ToList();

        Assert.True(items.SequenceEqual(items.OrderByDescending(i => i.CreatedAt)));
    }

    [Fact]
    public void Add_CreatesItemWithUniqueIncrementingId()
    {
        var store = new ItemStore();

        var first = store.Add("First");
        var second = store.Add("Second");

        Assert.Equal("First", first.Title);
        Assert.False(first.Done);
        Assert.NotEqual(first.Id, second.Id);
        Assert.True(second.Id > first.Id);

        var items = store.All();
        Assert.Contains(items, i => i.Id == first.Id && i.Title == "First");
        Assert.Contains(items, i => i.Id == second.Id && i.Title == "Second");
    }

    [Fact]
    public void SetDone_UpdatesExistingItem()
    {
        var store = new ItemStore();
        var item = store.Add("Toggle me");

        var updated = store.SetDone(item.Id, true);

        Assert.NotNull(updated);
        Assert.True(updated!.Done);
        Assert.Equal(item.Title, updated.Title);
    }

    [Fact]
    public void SetDone_ReturnsNull_ForUnknownId()
    {
        var store = new ItemStore();

        Assert.Null(store.SetDone(9999, true));
    }

    [Fact]
    public void Remove_DeletesItemAndReportsResult()
    {
        var store = new ItemStore();
        var item = store.Add("Temp");

        Assert.True(store.Remove(item.Id));
        Assert.False(store.Remove(item.Id));
        Assert.DoesNotContain(store.All(), i => i.Id == item.Id);
    }
}
