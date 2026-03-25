using BoxHub.Application.DTOs.Requests.Facilities;
using BoxHub.Application.DTOs.Responses;
using BoxHub.Application.DTOs.Responses.Facilities;
using BoxHub.Application.Interfaces.Repositories;
using BoxHub.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoxHub.Infrastructure.Repositories
{
    public class FacilityRepository : IFacilityRepository
    {
        private readonly BoxHubDbContext _db;
        public FacilityRepository(BoxHubDbContext db)
        {
            _db = db;
        }

        public async Task<PagedResponse<FacilitySearchItemResponse>> SearchAsync(
            FacilitySearchRequest request, CancellationToken ct)
        {
            var query = _db.facilities
                .AsNoTracking()
                .AsQueryable();

            // search (tên or địa chỉ)
            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                var keyword = request.Search.Trim();

                query = query.Where(f =>
                    EF.Functions.ILike(f.facility_name, $"%{keyword}%") ||
                    EF.Functions.ILike(f.address_street, $"%{keyword}%") ||
                    EF.Functions.ILike(f.address_district, $"%{keyword}%") ||
                    EF.Functions.ILike(f.address_city, $"%{keyword}%"));
            }

            // khu vuc: quan 1,...
            if (request.Areas?.Any() == true)
            {
                query = query.Where(f =>
                    request.Areas.Contains(f.address_district));
            }

            // box type
            if (!string.IsNullOrWhiteSpace(request.BoxType))
            {
                query = query.Where(f =>
                    _db.sleepboxes.Any(sb =>
                        sb.area_id == sb.area_id &&
                        sb.box_type == request.BoxType));
            }

            // tien ich (ANY logic)
            if (request.AmenityIds?.Any() == true)
            {
                query = query.Where(f =>
                    _db.facility_amenities.Any(fa =>
                        fa.facility_id == f.facility_id &&
                        request.AmenityIds.Contains(fa.amenity_id)));
            }

            // price
            if (request.PriceMin.HasValue || request.PriceMax.HasValue)
            {
                query = query.Where(f =>
                    _db.host_base_prices.Any(p =>
                        p.facility_id == f.facility_id &&
                        (!request.PriceMin.HasValue || p.base_hour_price >= request.PriceMin) &&
                        (!request.PriceMax.HasValue || p.base_hour_price <= request.PriceMax)
                    ));
            }

            // raitng
            if (request.MinRating.HasValue)
            {
                query = query.Where(f =>
                    _db.reviews
                        .Where(r => r.facility_id == f.facility_id)
                        .Average(r => (double?)r.rating_score) >= request.MinRating);
            }

            // AVAILABILITY
            if (request.CheckIn.HasValue && request.CheckOut.HasValue)
            {
                var checkIn = request.CheckIn.Value;
                var checkOut = request.CheckOut.Value;

                query = query.Where(f =>
                    _db.sleepboxes.Any(sb =>
                        _db.facility_areas.Any(a =>
                            a.area_id == sb.area_id &&
                            a.facility_id == f.facility_id
                        ) &&
                        !_db.box_availabilities.Any(ba =>
                            ba.box_id == sb.box_id &&
                            ba.start_time < checkOut &&
                            ba.end_time > checkIn
                        )
                    ));
            }

            var total = await query.CountAsync(ct);

            var facilities = await query
                .OrderByDescending(f => f.created_at)
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync(ct);

            // map
            var items = new List<FacilitySearchItemResponse>();

            foreach (var f in facilities)
            {
                // rating
                var reviews = await _db.reviews
                    .Where(r => r.facility_id == f.facility_id)
                    .ToListAsync(ct);

                var avgRating = reviews.Any()
                    ? reviews.Average(r => r.rating_score)
                    : 0;

                // price
                var prices = await _db.host_base_prices
                    .Where(p => p.facility_id == f.facility_id)
                    .Select(p => p.base_hour_price)
                    .ToListAsync(ct);

                var minPrice = prices.Any() ? prices.Min() : (decimal?)null;

                // matched amenities
                List<string>? matchedAmenities = null;

                if (request.AmenityIds?.Any() == true)
                {
                    matchedAmenities = await _db.facility_amenities
                        .Where(fa =>
                            fa.facility_id == f.facility_id &&
                            request.AmenityIds.Contains(fa.amenity_id))
                        .Join(_db.amenities,
                            fa => fa.amenity_id,
                            a => a.amenity_id,
                            (fa, a) => a.amenity_name)
                        .ToListAsync(ct);
                }

                items.Add(new FacilitySearchItemResponse
                {
                    FacilityId = f.facility_id,
                    FacilityName = f.facility_name,
                    District = f.address_district,
                    MinPrice = minPrice,
                    AvgRating = avgRating,
                    ReviewCount = reviews.Count,
                    MatchedAmenities = matchedAmenities
                });
            }

            return new PagedResponse<FacilitySearchItemResponse>(
                items,
                total,
                request.PageNumber,
                request.PageSize
            );
        }
    
    }
}
