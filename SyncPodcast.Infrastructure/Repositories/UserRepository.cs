using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.EntityFrameworkCore;
using SyncPodcast.Domain.Entities;
using SyncPodcast.Domain.Interfaces;
using SyncPodcast.Infrastructure.Persistence;

namespace SyncPodcast.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly SyncPodcastDbContext _db;
    public UserRepository(SyncPodcastDbContext db) => _db = db;

    public async Task<User?> GetByUsernameAsync(string username, CancellationToken ct)
    {
        return await _db.Users
            .FirstOrDefaultAsync(u => u.Username == username, ct);
    }

    public async Task<User?> GetByIdAsync(Guid id, CancellationToken ct)
    {
        return await _db.Users
            .FirstOrDefaultAsync(u => u.ID == id, ct);
    }

    public async Task<User?> GetByEmailAsync(string email, CancellationToken ct)
    {
        return await _db.Users
            .FirstOrDefaultAsync(u => u.Email == email, ct);
    }

    public async Task AddAsync(User user, CancellationToken ct)
    {
        // Add user to db
        _db.Users.Add(user);

        // write change
        await _db.SaveChangesAsync(ct);
    }

    public async Task<User?> UpdateUserAsync(User user, CancellationToken ct)
    {
        _db.Users.Update(user);

        await _db.SaveChangesAsync(ct);

        return user;

    }

    public async Task DeleteAsync(Guid id, CancellationToken ct)
    {
        User? user = await _db.Users.FirstOrDefaultAsync(u => u.ID == id, ct);
        if ( user is not null)
        {
            _db.Users.Remove(user);
            await _db.SaveChangesAsync(ct);
        }
    }
}


