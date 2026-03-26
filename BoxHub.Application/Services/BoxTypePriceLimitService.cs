using AutoMapper;
using BoxHub.Application.DTOs.Requests.Pricings;
using BoxHub.Application.DTOs.Responses.Pricings;
using BoxHub.Application.Interfaces;
using BoxHub.Application.Interfaces.Services;
using BoxHub.Domain.Entities;
using BoxHub.Domain.Enums.Pricings;
using BoxHub.Shared.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoxHub.Application.Services
{
    public class BoxTypePriceLimitService : IBoxTypePriceLimitService
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;

        public BoxTypePriceLimitService(IUnitOfWork uow, IMapper mapper)
        {
            _uow = uow;
            _mapper = mapper;
        }

        public async Task<ApiResponse<IEnumerable<BoxTypePriceLimitResponse>>> GetAllAsync()
        {
            var data = await _uow.BoxTypePriceLimits.GetAllAsync();
            var result = _mapper.Map<IEnumerable<BoxTypePriceLimitResponse>>(data);

            return ApiResponse<IEnumerable<BoxTypePriceLimitResponse>>
            .SuccessResponse(result);
        }

        public async Task<ApiResponse<BoxTypePriceLimitResponse>> GetByIdAsync(Guid id)
        {
            var entity = await _uow.BoxTypePriceLimits.GetByIdAsync(id);

            if (entity == null)
                return ApiResponse<BoxTypePriceLimitResponse>.Fail("Not found", 404);

            return ApiResponse<BoxTypePriceLimitResponse>
                .SuccessResponse(_mapper.Map<BoxTypePriceLimitResponse>(entity));
        }

        public async Task<ApiResponse<IEnumerable<BoxTypePriceLimitResponse>>> GetByCapacityAndClassAsync(
        CapacityType capacityType,
        BoxClass? boxClass)
        {
                // CASE 1: có boxClass → trả 1 phần tử (nhưng vẫn là list)
                if (boxClass.HasValue)
                {
                    var entity = await _uow.BoxTypePriceLimits
                        .GetByCapacityAndClassAsync(capacityType, boxClass.Value);

                    if (entity == null)
                        return ApiResponse<IEnumerable<BoxTypePriceLimitResponse>>
                            .Fail("Not found", 404);

                    var result = new List<BoxTypePriceLimitResponse>
            {
                _mapper.Map<BoxTypePriceLimitResponse>(entity)
            };

                    return ApiResponse<IEnumerable<BoxTypePriceLimitResponse>>
                        .SuccessResponse(result);
                }

                // CASE 2: chỉ có capacity → trả nhiều
                var list = await _uow.BoxTypePriceLimits
                    .GetByCapacityAsync(capacityType);

                var mapped = _mapper.Map<IEnumerable<BoxTypePriceLimitResponse>>(list);

                return ApiResponse<IEnumerable<BoxTypePriceLimitResponse>>
                    .SuccessResponse(mapped);
            }

        public async Task<ApiResponse<BoxTypePriceLimitResponse>> CreateAsync(CreateBoxTypePriceLimitRequest req, CancellationToken ct)
        {
            // Validate range
            if (req.MaxPrice <= req.MinPrice)
                return ApiResponse<BoxTypePriceLimitResponse>.Fail("MaxPrice must be greater than MinPrice", 400);

            // Unique check
            var exists = await _uow.BoxTypePriceLimits
                .ExistsAsync(req.CapacityType, req.BoxClass);

            if (exists)
                return ApiResponse<BoxTypePriceLimitResponse>.Fail("Price limit already exists", 409);

            var entity = _mapper.Map<system_box_type_price_limit>(req);
            entity.is_active = true;

            await _uow.BoxTypePriceLimits.AddAsync(entity);
            await _uow.SaveChangesAsync(ct);

            return ApiResponse<BoxTypePriceLimitResponse>
            .SuccessResponse(_mapper.Map<BoxTypePriceLimitResponse>(entity));
        }

        public async Task<ApiResponse<BoxTypePriceLimitResponse>> UpdateAsync(
            Guid id,
            UpdateBoxTypePriceLimitRequest req, CancellationToken ct)
        {
            var entity = await _uow.BoxTypePriceLimits.GetByIdAsync(id);

            if (entity == null)
                return ApiResponse<BoxTypePriceLimitResponse>.Fail("Not found", 404);

            // Merge (partial update)
            var newMin = req.MinPrice ?? entity.min_price;
            var newMax = req.MaxPrice ?? entity.max_price;

            // Validate
            if (newMax <= newMin)
                return ApiResponse<BoxTypePriceLimitResponse>.Fail("Invalid price range", 400);
                
            // Apply
            entity.min_price = newMin;
            entity.max_price = newMax;
            entity.is_active = req.IsActive;

            _uow.BoxTypePriceLimits.Update(entity);
            await _uow.SaveChangesAsync(ct);

            return ApiResponse<BoxTypePriceLimitResponse>
            .SuccessResponse(_mapper.Map<BoxTypePriceLimitResponse>(entity));
        }

        public async Task<ApiResponse<bool>> ToggleActiveAsync(Guid id, CancellationToken ct)
        {
            var entity = await _uow.BoxTypePriceLimits.GetByIdAsync(id);

            if (entity == null)
                return ApiResponse<bool>.Fail("Not found", 404);

            entity.is_active = !entity.is_active;

            List<Guid> affectedFacilities = new();

            _uow.BoxTypePriceLimits.Update(entity);
            await _uow.SaveChangesAsync(ct);

            return ApiResponse<bool>.SuccessResponse(true);
        }

        public async Task<ApiResponse<bool>> DeleteAsync(Guid id, CancellationToken ct)
        {
            var entity = await _uow.BoxTypePriceLimits.GetByIdAsync(id);

            if (entity == null)
                return ApiResponse<bool>.Fail("Not found", 404);

            _uow.BoxTypePriceLimits.Delete(entity);
            await _uow.SaveChangesAsync(ct);

            return ApiResponse<bool>.SuccessResponse(true);
        }
    }
}
