using UserManagementAPI.Models;

namespace UserManagementAPI.Services;

public class InMemoryUserStore : IUserStore
{
    private readonly List<User> _users = new();
    private int _nextId = 1;

    public IEnumerable<User> GetAll() => _users;

    public User? GetById(int id) => _users.FirstOrDefault(u => u.Id == id);

    public User Add(User user)
    {
        user.Id = _nextId++;
        user.CreatedAt = DateTime.UtcNow;
        _users.Add(user);
        return user;
    }

    public bool Update(int id, User updated)
    {
        var existing = _users.FirstOrDefault(u => u.Id == id);
        if (existing is null) return false;

        existing.FirstName = updated.FirstName;
        existing.LastName = updated.LastName;
        existing.Email = updated.Email;
        existing.Department = updated.Department;
        return true;
    }

    public bool Delete(int id)
    {
        var existing = _users.FirstOrDefault(u => u.Id == id);
        if (existing is null) return false;
        _users.Remove(existing);
        return true;
    }
}