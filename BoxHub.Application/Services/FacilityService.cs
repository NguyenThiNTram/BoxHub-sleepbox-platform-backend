using BoxHub.Application.DTOs.Requests.Facilities;
using BoxHub.Application.DTOs.Responses;
using BoxHub.Application.DTOs.Responses.Facilities;
using BoxHub.Application.Interfaces.Repositories;
using BoxHub.Application.Interfaces.Services;
using BoxHub.Shared.Errors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoxHub.Application.Services
{
    public class FacilityService : IFacilityService
    {
        private readonly IFacilityRepository _facilityRepository;

        public FacilityService(IFacilityRepository facilityRepository)
        {
            _facilityRepository = facilityRepository;
        }

        public async Task<PagedResponse<FacilitySearchItemResponse>> SearchAsync(
            FacilitySearchRequest request, CancellationToken ct)
        {
            // Validate basic
            if (request.PageNumber <= 0)
            {
                throw new ApiException(ErrorCodes.ValidationFailed, "PageNumber must be >= 1", 400);
            }

            if (request.PageSize <= 0)
            {
                throw new ApiException(ErrorCodes.ValidationFailed, "PageSize must be >= 1", 400);
            }

            // Validate date
            if (request.CheckIn.HasValue && request.CheckOut.HasValue)
            {
                if (request.CheckIn >= request.CheckOut)
                {
                    throw new ApiException(ErrorCodes.ValidationFailed, "CheckOut must be greater than CheckIn", 400);
                }
            }

            // Validate price
            if (request.PriceMin.HasValue && request.PriceMax.HasValue)
            {
                if (request.PriceMin > request.PriceMax)
                {
                    throw new ApiException(ErrorCodes.ValidationFailed, "PriceMin cannot be greater than PriceMax", 400);
                }
            }

            var result = await _facilityRepository.SearchAsync(request, ct);

            return result;
        }
    }
}
