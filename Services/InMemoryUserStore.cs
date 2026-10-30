using UserManagementAPI.Models;

namespace UserManagementAPI.Services;

public class InMemoryUserStore : IUserStore
{
    private readonly List<User> _users = new();
    private readonly object _lock = new();
    private int _nextId = 1;

    public IEnumerable<User> GetAll()
    {
        lock (_lock)
        {
            // Return a snapshot to avoid exposing internal list
            return _users.ToList();
        }
    }

    public User? GetById(int id)
    {
        lock (_lock)
        {
            return _users.FirstOrDefault(u => u.Id == id);
        }
    }

    public User Add(User user)
    {
        lock (_lock)
        {
            var email = user.Email.Trim().ToLowerInvariant();
            if (_users.Any(u => u.Email.Equals(email, StringComparison.OrdinalIgnoreCase)))
                throw new InvalidOperationException("A user with this email already exists.");

            user.Id = _nextId++;
            user.Email = email;
            user.CreatedAt = DateTime.UtcNow;
            _users.Add(user);
            return user;
        }
    }

    public bool Update(int id, User updated)
    {
        lock (_lock)
        {
            var existing = _users.FirstOrDefault(u => u.Id == id);
            if (existing is null) return false;

            var email = updated.Email.Trim().ToLowerInvariant();
            if (_users.Any(u => u.Id != id &&
                                u.Email.Equals(email, StringComparison.OrdinalIgnoreCase)))
                throw new InvalidOperationException("A user with this email already exists.");

            existing.FirstName = updated.FirstName.Trim();
            existing.LastName = updated.LastName.Trim();
            existing.Email = email;
            existing.Department = updated.Department?.Trim() ?? string.Empty;
            return true;
        }
    }

    public bool Delete(int id)
    {
        lock (_lock)
        {
            var existing = _users.FirstOrDefault(u => u.Id == id);
            if (existing is null) return false;
            _users.Remove(existing);
            return true;
        }
    }
}