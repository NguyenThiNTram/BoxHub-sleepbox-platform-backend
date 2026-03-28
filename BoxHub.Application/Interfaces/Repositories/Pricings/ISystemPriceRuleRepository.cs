using BoxHub.Application.DTOs.Responses.Pricings;
using BoxHub.Domain.Entities;
using BoxHub.Domain.Enums.Pricings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoxHub.Application.Interfaces.Repositories.Pricings
{
    public interface ISystemPriceRuleRepository
    {
        // rule price

        Task<List<system_price_rule>> GetAllAsync(CancellationToken ct);

        Task<system_price_rule?> GetByIdAsync(Guid ruleId, CancellationToken ct);

        Task<system_price_rule?> GetWithDetailsAsync(Guid ruleId, CancellationToken ct);

        Task<List<system_price_rule>> GetByModeAsync(PricingMode mode, CancellationToken ct);

        Task<int> CountActiveByModeAsync(PricingMode mode, CancellationToken ct);

        Task AddAsync(system_price_rule entity, CancellationToken ct);

        void Update(system_price_rule entity);

        void Delete(system_price_rule entity);


        // factor

        Task<List<pricing_factor>> GetFactorsByRuleIdAsync(Guid ruleId, CancellationToken ct);

        Task<pricing_factor?> GetFactorByIdAsync(Guid factorId, CancellationToken ct);

        Task<Dictionary<Guid, int>> GetFactorCountsByRuleIdsAsync(List<Guid> ruleIds, CancellationToken ct);

        Task AddFactorAsync(pricing_factor entity, CancellationToken ct);

        void UpdateFactor(pricing_factor entity);

        void DeleteFactor(pricing_factor entity);


        // combo

        Task<List<pricing_combo>> GetCombosByRuleIdAsync(Guid ruleId, CancellationToken ct);

        Task<pricing_combo?> GetComboByIdAsync(Guid comboId, CancellationToken ct);

        Task<Dictionary<Guid, int>> GetComboCountsByRuleIdsAsync(List<Guid> ruleIds, CancellationToken ct);

        Task AddComboAsync(pricing_combo entity, CancellationToken ct);

        void UpdateCombo(pricing_combo entity);

        void DeleteCombo(pricing_combo entity);


        // support

        Task<int> CountFactorsAsync(Guid ruleId, CancellationToken ct);

        Task<int> CountCombosAsync(Guid ruleId, CancellationToken ct);

        Task<bool> ExistsAsync(Guid ruleId, CancellationToken ct);

        IQueryable<system_price_rule> GetQueryable();
    }
}
