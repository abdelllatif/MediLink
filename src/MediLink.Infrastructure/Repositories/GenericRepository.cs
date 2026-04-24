namespace MediLink.Infrastructure.Repositories;

using MediLink.Domain.Entities;
using MediLink.Infrastructure.Data;

/// <summary>
/// Generic repository implementation for entities that inherit from BaseEntity.
/// </summary>
/// <typeparam name="T">Entity type.</typeparam>
public class GenericRepository<T> : BaseRepository<T> where T : BaseEntity
{
    public GenericRepository(AppDbContext context) : base(context)
    {
    }
}
