using BoxHub.Application.DTOs.Requests.Facilities;
using BoxHub.Application.DTOs.Responses;
using BoxHub.Application.DTOs.Responses.Facilities;
using BoxHub.Application.Interfaces;
using BoxHub.Application.Interfaces.Repositories;
using BoxHub.Application.Interfaces.Services;
using BoxHub.Domain.Entities;
using BoxHub.Domain.Enums;
using BoxHub.Shared.Errors;
using BoxHub.Shared.Results;
using System.Text.Json;

namespace BoxHub.Application.Services;

public class FacilityService : IFacilityService
{
    private readonly IFacilityRepository _facilityRepository;
    private readonly ICloudinaryService _cloudinary;
    private readonly IUnitOfWork _uow;

    public FacilityService(
        IFacilityRepository facilityRepository,
        ICloudinaryService cloudinary,
        IUnitOfWork uow)
    {
        _facilityRepository = facilityRepository;
        _cloudinary = cloudinary;
        _uow = uow;
    }

    public async Task<Result<CreateFacilityResponse>> CreateForHostAsync(
        Guid hostUserId,
        CreateFacilityRequest request,
        CancellationToken ct)
    {
        var name = (request.FacilityName ?? "").Trim();
        if (string.IsNullOrWhiteSpace(name))
        {
            return Result<CreateFacilityResponse>.Failure(
                ErrorCodes.ValidationFailed,
                "Tên cơ sở không được để trống.",
                400);
        }

        if (request.BusinessLicense is not { Length: > 0 })
        {
            return Result<CreateFacilityResponse>.Failure(
                ErrorCodes.ValidationFailed,
                "Cần upload Giấy chứng nhận đăng ký cơ sở kinh doanh (BUSINESS_LICENSE).",
                400);
        }

        if (request.PcccDocument is not { Length: > 0 })
        {
            return Result<CreateFacilityResponse>.Failure(
                ErrorCodes.ValidationFailed,
                "Cần upload Giấy chứng nhận / Biên bản PCCC.",
                400);
        }

        var brandId = await _facilityRepository.GetBrandIdForHostUserAsync(hostUserId, ct);
        if (brandId is null)
        {
            return Result<CreateFacilityResponse>.Failure(
                ErrorCodes.Forbidden,
                "Không tìm thấy thương hiệu cho tài khoản Host. Vui lòng hoàn tất hồ sơ brand trước.",
                403);
        }

        var licenseUrl = await _cloudinary.UploadFileAsync(request.BusinessLicense);
        var pcccUrl = await _cloudinary.UploadFileAsync(request.PcccDocument);

        if (string.IsNullOrWhiteSpace(licenseUrl) || string.IsNullOrWhiteSpace(pcccUrl))
        {
            return Result<CreateFacilityResponse>.Failure(
                ErrorCodes.ServerError,
                "Upload giấy tờ không trả về URL.",
                500);
        }

        var now = DateTime.UtcNow;
        var facilityId = Guid.NewGuid();

        var facility = new facility
        {
            facility_id = facilityId,
            brand_id = brandId.Value,
            facility_name = name,
            description = string.IsNullOrWhiteSpace(request.Description) ? null : request.Description.Trim(),
            address_street = string.IsNullOrWhiteSpace(request.AddressStreet) ? null : request.AddressStreet.Trim(),
            address_ward = string.IsNullOrWhiteSpace(request.AddressWard) ? null : request.AddressWard.Trim(),
            address_district = string.IsNullOrWhiteSpace(request.AddressDistrict) ? null : request.AddressDistrict.Trim(),
            address_city = string.IsNullOrWhiteSpace(request.AddressCity) ? null : request.AddressCity.Trim(),
            latitude = request.Latitude,
            longitude = request.Longitude,
            house_rules = string.IsNullOrWhiteSpace(request.HouseRules) ? null : request.HouseRules.Trim(),
            cleaning_buffer_minutes = request.CleaningBufferMinutes,
            facility_status = "PENDING",
            created_at = now,
            updated_at = now,
            facility_amenities = new List<facility_amenity>()
        };

        var licenseJson = JsonSerializer.Serialize(new List<string> { licenseUrl });
        var pcccJson = JsonSerializer.Serialize(new List<string> { pcccUrl });

        var documents = new List<facility_document>
        {
            new()
            {
                document_id = Guid.NewGuid(),
                facility_id = facilityId,
                document_type = FacilityDocumentType.BusinessLicense,
                version = 1,
                attachments = licenseJson,
                document_status = "PENDING",
                created_at = now,
                updated_at = now
            },
            new()
            {
                document_id = Guid.NewGuid(),
                facility_id = facilityId,
                document_type = FacilityDocumentType.PCCC,
                version = 1,
                attachments = pcccJson,
                document_status = "PENDING",
                created_at = now,
                updated_at = now
            }
        };

        await _facilityRepository.AddFacilityAsync(facility, ct);
        await _facilityRepository.AddFacilityDocumentsAsync(documents, ct);
        await _uow.SaveChangesAsync(ct);

        return Result<CreateFacilityResponse>.Success(new CreateFacilityResponse
        {
            FacilityId = facilityId,
            BrandId = brandId.Value,
            FacilityName = name,
            FacilityStatus = "PENDING"
        });
    }

    public async Task<PagedResponse<FacilitySearchItemResponse>> SearchAsync(
        FacilitySearchRequest request,
        CancellationToken ct)
    {
        if (request.PageNumber <= 0)
            throw new ApiException(ErrorCodes.ValidationFailed, "PageNumber must be >= 1", 400);

        if (request.PageSize <= 0)
            throw new ApiException(ErrorCodes.ValidationFailed, "PageSize must be >= 1", 400);

        if (request.CheckIn.HasValue && request.CheckOut.HasValue && request.CheckIn >= request.CheckOut)
            throw new ApiException(ErrorCodes.ValidationFailed, "CheckOut must be greater than CheckIn", 400);

        if (request.PriceMin.HasValue && request.PriceMax.HasValue && request.PriceMin > request.PriceMax)
            throw new ApiException(ErrorCodes.ValidationFailed, "PriceMin cannot be greater than PriceMax", 400);

        return await _facilityRepository.SearchAsync(request, ct);
    }
}
