using BoxHub.Application.DTOs.Requests.Facilities;
using BoxHub.Application.DTOs.Responses;
using BoxHub.Application.DTOs.Responses.Facilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoxHub.Application.Interfaces.Services
{
    public interface IFacilityService
    {
        Task<PagedResponse<FacilitySearchItemResponse>> SearchAsync(FacilitySearchRequest request, CancellationToken ct);
    }
}
