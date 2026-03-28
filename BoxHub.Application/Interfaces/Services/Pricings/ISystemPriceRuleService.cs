using BoxHub.Application.DTOs.Requests;
using BoxHub.Application.DTOs.Requests.Pricings;
using BoxHub.Application.DTOs.Responses;
using BoxHub.Application.DTOs.Responses.Pricings;
using BoxHub.Domain.Enums.Pricings;
using BoxHub.Shared.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoxHub.Application.Interfaces.Services.Pricings
{
    public interface ISystemPriceRuleService
    {
        // ===== PRICE RULE =====

        Task<ApiResponse<PagedResponse<PriceRuleSummary>>>
            GetAllAsync(PricingMode? modeFilter, bool? isActive, PagingRequest paging, CancellationToken ct);

        Task<ApiResponse<PriceRuleDetail>>
            GetDetailAsync(Guid ruleId, CancellationToken ct);

        Task<ApiResponse<PriceRuleSummary>>
            CreateAsync(CreatePriceRuleRequest req, CancellationToken ct);

        Task<ApiResponse<PriceRuleSummary>>
            UpdateAsync(Guid ruleId, UpdatePriceRuleRequest req, CancellationToken ct);

        Task<ApiResponse<bool>>
            ToggleActiveAsync(Guid ruleId, CancellationToken ct);

        Task<ApiResponse<bool>>
            DeleteAsync(Guid ruleId, CancellationToken ct);


        // ===== FACTOR =====

        Task<ApiResponse<List<PricingFactor>>>
            GetFactorsAsync(Guid ruleId, CancellationToken ct);

        Task<ApiResponse<PricingFactor>>
            AddFactorAsync(Guid ruleId, PricingFactor request, CancellationToken ct);

        Task<ApiResponse<PricingFactor>>
            UpdateFactorAsync(Guid factorId, PricingFactor request, CancellationToken ct);

        Task<ApiResponse<bool>>
            DeleteFactorAsync(Guid factorId, CancellationToken ct);


        // ===== COMBO =====

        Task<ApiResponse<List<PricingCombo>>>
            GetCombosAsync(Guid ruleId, CancellationToken ct);

        Task<ApiResponse<PricingCombo>>
            AddComboAsync(Guid ruleId, PricingCombo request, CancellationToken ct);

        Task<ApiResponse<PricingCombo>>
            UpdateComboAsync(Guid comboId, PricingCombo request, CancellationToken ct);

        Task<ApiResponse<bool>>
            DeleteComboAsync(Guid comboId, CancellationToken ct);
    }
}
