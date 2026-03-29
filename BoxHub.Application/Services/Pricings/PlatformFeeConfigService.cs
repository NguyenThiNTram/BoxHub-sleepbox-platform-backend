using AutoMapper;
using BoxHub.Application.DTOs.Requests.Pricings;
using BoxHub.Application.DTOs.Responses.Pricings;
using BoxHub.Application.Interfaces;
using BoxHub.Application.Interfaces.Repositories;
using BoxHub.Application.Interfaces.Repositories.Pricings;
using BoxHub.Application.Interfaces.Services.Pricings;
using BoxHub.Domain.Entities;
using BoxHub.Domain.Enums.Pricings;
using BoxHub.Shared.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoxHub.Application.Services.Pricings
{
    public class PlatformFeeConfigService : IPlatformFeeConfigService
    {
        private readonly IPlatformFeeConfigRepository _repo;
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;
        private readonly IHostRegistrationRepository _hostRepo;

        public PlatformFeeConfigService(
            IPlatformFeeConfigRepository repo,
            IUnitOfWork uow,
            IMapper mapper,
            IHostRegistrationRepository hostRepo)
        {
            _repo = repo;
            _uow = uow;
            _mapper = mapper;
            _hostRepo = hostRepo;
        }

        public async Task<ApiResponse<List<PlatformFeeConfig>>>GetAllActiveAsync(CancellationToken ct)
        {
            var now = DateTime.UtcNow;

            var entities = await _repo.GetCurrentConfigsAsync(now, ct);

            var dto = _mapper.Map<List<PlatformFeeConfig>>(entities);

            return ApiResponse<List<PlatformFeeConfig>>.SuccessResponse(dto);
        }

        public async Task<ApiResponse<FeeTimeline>>GetTimelineAsync(FeeCode feeCode, Guid? hostId, CancellationToken ct)
        {
            var now = DateTime.UtcNow;

            var history = await _repo.GetHistoryByFeeCodeAsync(feeCode, hostId, ct);

            var mapped = _mapper.Map<List<PlatformFeeConfig>>(history);

            var current = mapped
                .FirstOrDefault(x =>
                    x.IsActive &&
                    x.EffectiveFrom <= now &&
                    (x.EffectiveTo == null || x.EffectiveTo >= now)
                );

            var result = new FeeTimeline
            {
                FeeCode = feeCode,
                History = mapped,
                Current = current
            };

            return ApiResponse<FeeTimeline>.SuccessResponse(result);
        }

        public async Task<ApiResponse<PlatformFeeConfig>>CreateAsync(CreatePlatformFeeConfigRequest req, CancellationToken ct)
        {
            // validation

            if (req.EffectiveFrom < DateTime.UtcNow.Date)
                return ApiResponse<PlatformFeeConfig>.Fail("EffectiveFrom must be >= today", 400);

            // XOR validation
            if ((req.PercentageValue.HasValue && req.FixedAmount.HasValue) ||
                (!req.PercentageValue.HasValue && !req.FixedAmount.HasValue))
                return ApiResponse<PlatformFeeConfig>.Fail("Either Percentage or FixedAmount required (XOR)", 400);

            if (req.CalculationMethod == CalcMethod.PERCENTAGE)
            {
                if (!req.PercentageValue.HasValue || req.PercentageValue <= 0 || req.PercentageValue > 100)
                    return ApiResponse<PlatformFeeConfig>.Fail("Invalid percentage value", 400);
            }

            if (req.CalculationMethod == CalcMethod.FIXED)
            {
                if (!req.FixedAmount.HasValue || req.FixedAmount <= 0)
                    return ApiResponse<PlatformFeeConfig>.Fail("Invalid fixed amount", 400);
            }

            if (req.EffectiveTo.HasValue && req.EffectiveTo <= req.EffectiveFrom)
                return ApiResponse<PlatformFeeConfig>.Fail("EffectiveTo must be greater than EffectiveFrom", 400);

            // Host validation
            if (req.TargetHostId.HasValue)
            {
                var host = await _hostRepo.GetByIdAsync(req.TargetHostId.Value, ct);
                if (host == null)
                    return ApiResponse<PlatformFeeConfig>.Fail("Host not found", 404);
                if (host.verified_at == null)
                    return ApiResponse<PlatformFeeConfig>.Fail("Host not verified", 400);
            }

            // Overlap check
            var hasOverlap = await _repo.HasOverlapAsync(
                req.FeeCode,
                req.EffectiveFrom,
                req.TargetHostId,
                ct);

            if (hasOverlap)
                return ApiResponse<PlatformFeeConfig>.Fail("Config already exists at this time", 409);

            // create

            var entity = _mapper.Map<platform_fee_config>(req);

            entity.config_id = Guid.NewGuid();
            entity.created_at = DateTime.UtcNow;
            entity.is_active = true;

            await _repo.AddAsync(entity, ct);
            await _uow.SaveChangesAsync(ct);

            var dto = _mapper.Map<PlatformFeeConfig>(entity);

            return ApiResponse<PlatformFeeConfig>.SuccessResponse(dto);
        }

        public async Task<ApiResponse<bool>>DeactivateAsync(Guid configId, CancellationToken ct)
        {
            var entity = await _repo.GetByIdAsync(configId, ct);

            if (entity == null)
                return ApiResponse<bool>.Fail("Not found", 404);

            var now = DateTime.UtcNow;

            //Không cho deactivate nếu đã có hiệu lực
            if (entity.effective_from <= now)
                return ApiResponse<bool>.Fail("Cannot deactivate active config", 409);

            entity.is_active = false;

            _repo.Update(entity);
            await _uow.SaveChangesAsync(ct);

            return ApiResponse<bool>.SuccessResponse(true);
        }

        public async Task<ResolvedFees>ResolveFeesForBookingAsync(Guid hostId, DateTime bookingTime, CancellationToken ct)
        {
            var configs = await _repo.ResolveAllConfigsAsync(hostId, bookingTime, ct);

            var result = new ResolvedFees();

            foreach (var config in configs)
            {
                var value = config.calculation_method == CalcMethod.PERCENTAGE
                    ? config.percentage_value ?? 0
                    : config.fixed_amount ?? 0;

                switch (config.fee_code)
                {
                    case FeeCode.COMMISSION:
                        result.Commission = value;
                        break;

                    case FeeCode.SERVICE_FEE:
                        result.ServiceFee = value;
                        break;

                    case FeeCode.VAT:
                        result.Vat = value;
                        break;
                }
            }

            return result;
        }

    }
}
