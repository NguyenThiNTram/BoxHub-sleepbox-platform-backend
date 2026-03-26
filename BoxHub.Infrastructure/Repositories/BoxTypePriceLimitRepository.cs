using BoxHub.Application.DTOs.Responses.Pricings;
using BoxHub.Application.Interfaces.Repositories;
using BoxHub.Domain.Entities;
using BoxHub.Domain.Enums.Pricings;
using BoxHub.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoxHub.Infrastructure.Repositories
{
    public class BoxTypePriceLimitRepository : IBoxTypePriceLimitRepository
    {
        private readonly BoxHubDbContext _db;
        public BoxTypePriceLimitRepository(BoxHubDbContext db)
        {
            _db = db;
        }

        public async Task<system_box_type_price_limit?> GetByIdAsync(Guid id)
        {
            return await _db.system_box_type_price_limits
                .FirstOrDefaultAsync(x => x.limit_id == id);
        }

        public async Task<system_box_type_price_limit?> GetByCapacityAndClassAsync(CapacityType capacityType, BoxClass boxClass)
        {
            return await _db.system_box_type_price_limits
                .FirstOrDefaultAsync(x =>
                    x.capacity_type == capacityType &&
                    x.box_class == boxClass);
        }

        public async Task<bool> ExistsAsync(
            CapacityType capacityType,
            BoxClass boxClass)
        {
            return await _db.system_box_type_price_limits
                .AnyAsync(x =>
                    x.capacity_type == capacityType &&
                    x.box_class == boxClass);
        }

        public async Task<List<system_box_type_price_limit>> GetAllAsync()
        {
            return await _db.system_box_type_price_limits
                .OrderBy(x => x.capacity_type)
                .ThenBy(x => x.box_class)
                .ToListAsync();
        }

        public async Task<List<system_box_type_price_limit>> FilterAsync(BoxTypePriceLimitFilter filter)
        {
            var query = _db.system_box_type_price_limits.AsQueryable();

            if (filter.CapacityType.HasValue)
            {
                query = query.Where(x => x.capacity_type == filter.CapacityType.Value);
            }

            if (filter.BoxClass.HasValue)
            {
                query = query.Where(x => x.box_class == filter.BoxClass.Value);
            }

            if (filter.IsActive.HasValue)
            {
                query = query.Where(x => x.is_active == filter.IsActive.Value);
            }

            return await query
                .OrderBy(x => x.capacity_type)
                .ThenBy(x => x.box_class)
                .ToListAsync();
        }

        public async Task<List<system_box_type_price_limit>> GetByCapacityAsync(CapacityType capacityType)
        {
            return await _db.system_box_type_price_limits
                .Where(x => x.capacity_type == capacityType)
                .ToListAsync();
        }

        public async Task AddAsync(system_box_type_price_limit entity)
        {
            await _db.system_box_type_price_limits.AddAsync(entity);
        }

        public void Update(system_box_type_price_limit entity)
        {
            _db.system_box_type_price_limits.Update(entity);
        }

        public void Delete( system_box_type_price_limit entity)
        {
            _db.system_box_type_price_limits.Remove(entity);
        }

    }
}
