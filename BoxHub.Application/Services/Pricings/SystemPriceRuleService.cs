using AutoMapper;
using BoxHub.Application.DTOs.Requests;
using BoxHub.Application.DTOs.Requests.Pricings;
using BoxHub.Application.DTOs.Responses;
using BoxHub.Application.DTOs.Responses.Pricings;
using BoxHub.Application.Interfaces;
using BoxHub.Application.Interfaces.Repositories.Pricings;
using BoxHub.Application.Interfaces.Services.Pricings;
using BoxHub.Application.Validators.Pricings;
using BoxHub.Domain.Entities;
using BoxHub.Domain.Enums.Pricings;
using BoxHub.Shared.Results;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoxHub.Application.Services.Pricings
{
    public class SystemPriceRuleService : ISystemPriceRuleService
    {
        private readonly ISystemPriceRuleRepository _repo;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _uow;

        public SystemPriceRuleService(ISystemPriceRuleRepository repo, IMapper mapper, IUnitOfWork uow)
        {
            _repo = repo;
            _mapper = mapper;
            _uow = uow;
        }

        public async Task<ApiResponse<PagedResponse<PriceRuleSummary>>> GetAllAsync(PricingMode? modeFilter, bool? isActive, PagingRequest paging, CancellationToken ct)
        {
            var query = _repo.GetQueryable();

            if (modeFilter.HasValue)
                query = query.Where(x => x.pricing_mode == modeFilter.Value);

            if (isActive.HasValue)
                query = query.Where(x => x.is_active == isActive.Value);

            var totalCount = await query.CountAsync(ct);

            var items = await query
                .OrderByDescending(x => x.priority)
                .Skip((paging.PageNumber - 1) * paging.PageSize)
                .Take(paging.PageSize)
                .ToListAsync(ct);

            var mapped = _mapper.Map<List<PriceRuleSummary>>(items);

            // list ruleIds
            var ruleIds = mapped.Select(x => x.RuleId).ToList();

            // Batch query
            var factorCounts = await _repo.GetFactorCountsByRuleIdsAsync(ruleIds, ct);
            var comboCounts = await _repo.GetComboCountsByRuleIdsAsync(ruleIds, ct);

            // Map DTO
            foreach (var item in mapped)
            {
                item.FactorCount = factorCounts.TryGetValue(item.RuleId, out var fCount) ? fCount : 0;
                item.ComboCount = comboCounts.TryGetValue(item.RuleId, out var cCount) ? cCount : 0;
            }

            var result = new PagedResponse<PriceRuleSummary>(
                mapped, totalCount, paging.PageNumber, paging.PageSize);

            return ApiResponse<PagedResponse<PriceRuleSummary>>.SuccessResponse(result);
        }

        public async Task<ApiResponse<PriceRuleDetail>>GetDetailAsync(Guid ruleId, CancellationToken ct)
        {
            var entity = await _repo.GetWithDetailsAsync(ruleId, ct);

            if (entity == null)
                return ApiResponse<PriceRuleDetail>.Fail("Not found", 404);

            var dto = _mapper.Map<PriceRuleDetail>(entity);

            return ApiResponse<PriceRuleDetail>.SuccessResponse(dto);
        }

        public async Task<ApiResponse<PriceRuleSummary>>CreateAsync(CreatePriceRuleRequest req, CancellationToken ct)
        {
            var error = PricingValidationHelper.ValidateCreate(req);

            if (error != null)
                return ApiResponse<PriceRuleSummary>.Fail(error, 400);

            var entity = _mapper.Map<system_price_rule>(req);

            await _repo.AddAsync(entity, ct);
            await _uow.SaveChangesAsync(ct);

            var dto = _mapper.Map<PriceRuleSummary>(entity);

            return ApiResponse<PriceRuleSummary>.SuccessResponse(dto);
        }

        public async Task<ApiResponse<PriceRuleSummary>>UpdateAsync(Guid ruleId, UpdatePriceRuleRequest req, CancellationToken ct)
        {
            var entity = await _repo.GetByIdAsync(ruleId, ct);

            if (entity == null)
                return ApiResponse<PriceRuleSummary>.Fail("Not found", 404);

            var error = PricingValidationHelper.ValidateUpdate(entity, req);

            if (error != null)
                return ApiResponse<PriceRuleSummary>.Fail(error, 400);

            // block change mode
            // if have booking, block change mode (TODO: implement later)

            if (entity.pricing_mode == PricingMode.HOURLY)
            {
                if (req.MinHours.HasValue && req.MaxHours.HasValue &&
                    req.MaxHours <= req.MinHours)
                    return ApiResponse<PriceRuleSummary>.Fail("Invalid hours range", 400);
            }

            _mapper.Map(req, entity);

            _repo.Update(entity);
            await _uow.SaveChangesAsync(ct);

            var dto = _mapper.Map<PriceRuleSummary>(entity);

            return ApiResponse<PriceRuleSummary>.SuccessResponse(dto);
        }

        public async Task<ApiResponse<bool>>ToggleActiveAsync(Guid ruleId, CancellationToken ct)
        {
            var entity = await _repo.GetByIdAsync(ruleId, ct);

            if (entity == null)
                return ApiResponse<bool>.Fail("Not found", 404);

            if (entity.is_active == true)
            {
                var count = await _repo.CountActiveByModeAsync(entity.pricing_mode, ct);

                if (count <= 1)
                    return ApiResponse<bool>.Fail("Must have at least 1 active rule per mode", 409);
            }

            entity.is_active = !entity.is_active;

            _repo.Update(entity);
            await _uow.SaveChangesAsync(ct);

            return ApiResponse<bool>.SuccessResponse(true);
        }

        public async Task<ApiResponse<bool>>DeleteAsync(Guid ruleId, CancellationToken ct)
        {
            var entity = await _repo.GetByIdAsync(ruleId, ct);

            if (entity == null)
                return ApiResponse<bool>.Fail("Not found", 404);

            // check host_base_prices
            // check bookings

            entity.is_active = false;

            _repo.Update(entity);
            await _uow.SaveChangesAsync(ct);

            return ApiResponse<bool>.SuccessResponse(true);
        }

        public async Task<ApiResponse<List<PricingFactor>>>GetFactorsAsync(Guid ruleId, CancellationToken ct)
        {
            var factors = await _repo.GetFactorsByRuleIdAsync(ruleId, ct);

            var dto = _mapper.Map<List<PricingFactor>>(factors);

            return ApiResponse<List<PricingFactor>>.SuccessResponse(dto);
        }

        public async Task<ApiResponse<PricingFactor>>AddFactorAsync(Guid ruleId, PricingFactor req, CancellationToken ct)
        {
            var entity = _mapper.Map<pricing_factor>(req);
            entity.rule_id = ruleId;

            var error = PricingValidationHelper.ValidateFactor(entity);

            if (error != null)
                return ApiResponse<PricingFactor>.Fail(error, 400);

            await _repo.AddFactorAsync(entity, ct);
            await _uow.SaveChangesAsync(ct);

            return ApiResponse<PricingFactor>.SuccessResponse(_mapper.Map<PricingFactor>(entity));
        }

        public async Task<ApiResponse<PricingFactor>>UpdateFactorAsync(Guid factorId, PricingFactor req, CancellationToken ct)
        {
            var entity = await _repo.GetFactorByIdAsync(factorId, ct);

            if (entity == null)
                return ApiResponse<PricingFactor>.Fail("Not found", 404);

            _mapper.Map(req, entity);

            _repo.UpdateFactor(entity);
            await _uow.SaveChangesAsync(ct);

            return ApiResponse<PricingFactor>.SuccessResponse(_mapper.Map<PricingFactor>(entity));
        }

        public async Task<ApiResponse<bool>>DeleteFactorAsync(Guid factorId, CancellationToken ct)
        {
            var entity = await _repo.GetFactorByIdAsync(factorId, ct);

            if (entity == null)
                return ApiResponse<bool>.Fail("Not found", 404);

            _repo.DeleteFactor(entity);
            await _uow.SaveChangesAsync(ct);

            return ApiResponse<bool>.SuccessResponse(true);
        }

        public async Task<ApiResponse<List<PricingCombo>>>GetCombosAsync(Guid ruleId, CancellationToken ct)
        {
            var combos = await _repo.GetCombosByRuleIdAsync(ruleId, ct);

            var dto = _mapper.Map<List<PricingCombo>>(combos);

            return ApiResponse<List<PricingCombo>>.SuccessResponse(dto);
        }

        public async Task<ApiResponse<PricingCombo>>AddComboAsync(Guid ruleId, PricingCombo req, CancellationToken ct)
        {
            var existing = await _repo.GetCombosByRuleIdAsync(ruleId, ct);

            var error = PricingValidationHelper.ValidateCombo(existing, req.Hours);

            if (error != null)
                return ApiResponse<PricingCombo>.Fail(error, 400);

            var entity = _mapper.Map<pricing_combo>(req);
            entity.rule_id = ruleId;

            await _repo.AddComboAsync(entity, ct);
            await _uow.SaveChangesAsync(ct);

            return ApiResponse<PricingCombo>.SuccessResponse(_mapper.Map<PricingCombo>(entity));
        }

        public async Task<ApiResponse<PricingCombo>>UpdateComboAsync(Guid comboId, PricingCombo req, CancellationToken ct)
        {
            var entity = await _repo.GetComboByIdAsync(comboId, ct);

            if (entity == null)
                return ApiResponse<PricingCombo>.Fail("Not found", 404);

            _mapper.Map(req, entity);

            _repo.UpdateCombo(entity);
            await _uow.SaveChangesAsync(ct);

            return ApiResponse<PricingCombo>.SuccessResponse(_mapper.Map<PricingCombo>(entity));
        }

        public async Task<ApiResponse<bool>>DeleteComboAsync(Guid comboId, CancellationToken ct)
        {
            var entity = await _repo.GetComboByIdAsync(comboId, ct);

            if (entity == null)
                return ApiResponse<bool>.Fail("Not found", 404);

            _repo.DeleteCombo(entity);
            await _uow.SaveChangesAsync(ct);

            return ApiResponse<bool>.SuccessResponse(true);
        }
    }
}
