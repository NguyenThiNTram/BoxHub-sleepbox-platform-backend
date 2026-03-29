using AutoMapper;
using BoxHub.Application.DTOs.Requests.Pricings;
using BoxHub.Application.DTOs.Responses.Pricings;
using BoxHub.Application.Interfaces;
using BoxHub.Application.Interfaces.Repositories.Pricings;
using BoxHub.Application.Interfaces.Services.Pricings;
using BoxHub.Domain.Entities;
using BoxHub.Shared.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoxHub.Application.Services.Pricings
{
    public class AddonServiceService : IAddonServiceService
    {
        private readonly IAddonServiceRepository _repo;
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;

        public AddonServiceService(
            IAddonServiceRepository repo,
            IUnitOfWork uow,
            IMapper mapper)
        {
            _repo = repo;
            _uow = uow;
            _mapper = mapper;
        }

        public async Task<ApiResponse<IEnumerable<AddonService>>> GetAllAsync(bool? isActive, CancellationToken ct)
        {
            IEnumerable<addon_service> entities;

            if (isActive.HasValue && isActive.Value)
                entities = await _repo.GetActiveAsync(ct);
            else
                entities = await _repo.GetAllAsync(ct);

            var result = _mapper.Map<IEnumerable<AddonService>>(entities);

            return ApiResponse<IEnumerable<AddonService>>.SuccessResponse(result);
        }

        public async Task<ApiResponse<AddonService>> CreateAsync(CreateAddonServiceRequest req, CancellationToken ct)
        {
            var name = req.ServiceName?.Trim();

            if (string.IsNullOrWhiteSpace(name))
                return ApiResponse<AddonService>.Fail("Service name is required", 400);

            // Duplicate check (case-insensitive)
            var exists = await _repo.GetByNameAsync(name, ct);
            if (exists != null)
                return ApiResponse<AddonService>.Fail("Service name already exists", 409);

            var entity = new addon_service
            {
                service_id = Guid.NewGuid(),
                service_name = name,
                unit = req.Unit?.Trim(),
                description = req.Description?.Trim(),
                is_active = true,
                created_at = DateTime.UtcNow
            };

            await _repo.AddAsync(entity, ct);
            await _uow.SaveChangesAsync(ct);

            var dto = _mapper.Map<AddonService>(entity);

            return ApiResponse<AddonService>.SuccessResponse(dto);
        }

        public async Task<ApiResponse<AddonService>> UpdateAsync(Guid serviceId, UpdateAddonServiceRequest req, CancellationToken ct)
        {
            var entity = await _repo.GetByIdAsync(serviceId, ct);
            if (entity == null)
                return ApiResponse<AddonService>.Fail("Service not found", 404);

            // Update name
            if (!string.IsNullOrWhiteSpace(req.ServiceName))
            {
                var newName = req.ServiceName.Trim();

                // Check duplicate (exclude current)
                var isDuplicate = await _repo.ExistsAsync(
                    x => x.service_name.ToLower() == newName.ToLower()
                         && x.service_id != serviceId,
                    ct);

                if (isDuplicate)
                    return ApiResponse<AddonService>.Fail("Service name already exists", 409);

                entity.service_name = newName;
            }

            // Update unit
            if (req.Unit != null)
                entity.unit = req.Unit.Trim();

            // Update description
            if (req.Description != null)
                entity.description = req.Description.Trim();

            _repo.Update(entity);
            await _uow.SaveChangesAsync(ct);

            var dto = _mapper.Map<AddonService>(entity);

            return ApiResponse<AddonService>.SuccessResponse(dto);
        }

        public async Task<ApiResponse<bool>> ToggleActiveAsync(Guid serviceId, CancellationToken ct)
        {
            var entity = await _repo.GetByIdAsync(serviceId, ct);
            if (entity == null)
                return ApiResponse<bool>.Fail("Service not found", 404);

            entity.is_active = !entity.is_active;

            _repo.Update(entity);
            await _uow.SaveChangesAsync(ct);

            return ApiResponse<bool>.SuccessResponse(true);
        }

        public async Task<ApiResponse<bool>> DeleteAsync(Guid serviceId, CancellationToken ct)
        {
            var entity = await _repo.GetByIdAsync(serviceId, ct);
            if (entity == null)
                return ApiResponse<bool>.Fail("Service not found", 404);

            // Guard: đang được host sử dụng
            var isUsed = await _repo.IsUsedByHostAsync(serviceId, ct);
            if (isUsed)
                return ApiResponse<bool>.Fail("Service is being used by hosts", 409);

            // Soft delete
            entity.is_active = false;

            _repo.Update(entity);
            await _uow.SaveChangesAsync(ct);

            return ApiResponse<bool>.SuccessResponse(true);
        }
    }
}
