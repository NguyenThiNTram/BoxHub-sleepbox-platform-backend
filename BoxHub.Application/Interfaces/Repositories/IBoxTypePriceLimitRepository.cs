using BoxHub.Application.DTOs.Responses.Pricings;
using BoxHub.Domain.Entities;
using BoxHub.Domain.Enums.Pricings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoxHub.Application.Interfaces.Repositories
{
    public interface IBoxTypePriceLimitRepository
    {
        Task<system_box_type_price_limit?> GetByIdAsync(Guid id);

        Task<system_box_type_price_limit?> GetByCapacityAndClassAsync(CapacityType capacityType, BoxClass boxClass);

        Task<bool> ExistsAsync(CapacityType capacityType, BoxClass boxClass);

        Task<List<system_box_type_price_limit>> GetAllAsync();

        Task<List<system_box_type_price_limit>> FilterAsync(BoxTypePriceLimitFilter filter);

        Task<List<system_box_type_price_limit>> GetByCapacityAsync(CapacityType capacityType);

        Task AddAsync(system_box_type_price_limit entity);

        void Update(system_box_type_price_limit entity);

        void Delete(system_box_type_price_limit entity);
    }
}
