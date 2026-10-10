namespace App.Api.Services;

/// <summary>
/// Thread-safe, in-memory item list. Data is lost when the API restarts.
/// </summary>
public class ItemStore
{
    public sealed record Item(int Id, string Title, bool Done, DateTime CreatedAt);

    private readonly object _lock = new();
    private readonly List<Item> _items;
    private int _nextId;

    public ItemStore()
    {
        var now = DateTime.UtcNow;
        _items =
        [
            new(1, "Read the README", true, now.AddDays(-2)),
            new(2, "Try the Items page", false, now.AddDays(-1)),
            new(3, "Log in as admin / admin123", false, now),
        ];
        _nextId = _items.Count + 1;
    }

    public IReadOnlyList<Item> All()
    {
        lock (_lock)
        {
            return _items.OrderByDescending(i => i.CreatedAt).ToList();
        }
    }

    public Item Add(string title)
    {
        lock (_lock)
        {
            var item = new Item(_nextId++, title, false, DateTime.UtcNow);
            _items.Add(item);
            return item;
        }
    }

    public Item? SetDone(int id, bool done)
    {
        lock (_lock)
        {
            var index = _items.FindIndex(i => i.Id == id);
            if (index < 0)
            {
                return null;
            }

            var updated = _items[index] with { Done = done };
            _items[index] = updated;
            return updated;
        }
    }

    public bool Remove(int id)
    {
        lock (_lock)
        {
            return _items.RemoveAll(i => i.Id == id) > 0;
        }
    }
}
