using BoxHub.Application.DTOs.Responses.Pricings;
using BoxHub.Application.Interfaces.Repositories.Pricings;
using BoxHub.Domain.Entities;
using BoxHub.Domain.Enums.Pricings;
using BoxHub.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoxHub.Infrastructure.Repositories.Pricings
{
    public class SystemPriceRuleRepository : ISystemPriceRuleRepository
    {
        private readonly BoxHubDbContext _db;
        public SystemPriceRuleRepository(BoxHubDbContext db)
        {
            _db = db;
        }

        public async Task<List<system_price_rule>> GetAllAsync(CancellationToken ct)
        {
            return await BaseQuery()
                .AsNoTracking()
                .OrderByDescending(x => x.created_at)
                .ToListAsync(ct);
        }

        public async Task<system_price_rule?> GetByIdAsync(Guid ruleId, CancellationToken ct)
        {
            return await BaseQuery()
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.rule_id == ruleId, ct);
        }

        public async Task<system_price_rule?> GetWithDetailsAsync(Guid ruleId, CancellationToken ct)
        {
            return await BaseQuery()
                .AsNoTracking()
                .Include(x => x.pricing_factors)
                .Include(x => x.pricing_combos)
                .FirstOrDefaultAsync(x => x.rule_id == ruleId, ct);
        }

        public async Task<List<system_price_rule>> GetByModeAsync(PricingMode mode, CancellationToken ct)
        {
            return await BaseQuery()
                .AsNoTracking()
                .Where(x => x.pricing_mode == mode)
                .OrderByDescending(x => x.priority)
                .ToListAsync(ct);
        }

        public async Task<int> CountActiveByModeAsync(PricingMode mode, CancellationToken ct)
        {
            return await BaseQuery()
                .Where(x => x.pricing_mode == mode && x.is_active == true)
                .CountAsync(ct);
        }

        public async Task AddAsync(system_price_rule entity, CancellationToken ct)
        {
            await _db.system_price_rules.AddAsync(entity, ct);
        }

        public void Update(system_price_rule entity)
        {
            _db.system_price_rules.Update(entity);
        }

        public void Delete(system_price_rule entity)
        {
            _db.system_price_rules.Remove(entity);
        }

        public async Task<List<pricing_factor>> GetFactorsByRuleIdAsync(Guid ruleId, CancellationToken ct)
        {
            return await _db.pricing_factors
                .AsNoTracking()
                .Where(x => x.rule_id == ruleId)
                .OrderByDescending(x => x.priority)
                .ToListAsync(ct);
        }

        public async Task<pricing_factor?> GetFactorByIdAsync(Guid factorId, CancellationToken ct)
        {
            return await _db.pricing_factors
                .FirstOrDefaultAsync(x => x.factor_id == factorId, ct);
        }

        public async Task<Dictionary<Guid, int>> GetFactorCountsByRuleIdsAsync(List<Guid> ruleIds, CancellationToken ct)
        {
            return await _db.pricing_factors
                .Where(x => ruleIds.Contains(x.rule_id))
                .GroupBy(x => x.rule_id)
                .Select(g => new { RuleId = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.RuleId, x => x.Count, ct);
        }

        public async Task AddFactorAsync(pricing_factor entity, CancellationToken ct)
        {
            await _db.pricing_factors.AddAsync(entity, ct);
        }

        public void UpdateFactor(pricing_factor entity)
        {
            _db.pricing_factors.Update(entity);
        }

        public void DeleteFactor(pricing_factor entity)
        {
            _db.pricing_factors.Remove(entity);
        }

        public async Task<List<pricing_combo>> GetCombosByRuleIdAsync(Guid ruleId, CancellationToken ct)
        {
            return await _db.pricing_combos
                .AsNoTracking()
                .Where(x => x.rule_id == ruleId)
                .OrderBy(x => x.hours)
                .ToListAsync(ct);
        }

        public async Task<Dictionary<Guid, int>> GetComboCountsByRuleIdsAsync(List<Guid> ruleIds, CancellationToken ct)
        {
            return await _db.pricing_combos
                .Where(x => ruleIds.Contains(x.rule_id))
                .GroupBy(x => x.rule_id)
                .Select(g => new { RuleId = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.RuleId, x => x.Count, ct);
        }

        public async Task<pricing_combo?> GetComboByIdAsync(Guid comboId, CancellationToken ct)
        {
            return await _db.pricing_combos
                .FirstOrDefaultAsync(x => x.combo_id == comboId, ct);
        }

        public async Task AddComboAsync(pricing_combo entity, CancellationToken ct)
        {
            await _db.pricing_combos.AddAsync(entity, ct);
        }

        public void UpdateCombo(pricing_combo entity)
        {
            _db.pricing_combos.Update(entity);
        }

        public void DeleteCombo(pricing_combo entity)
        {
            _db.pricing_combos.Remove(entity);
        }

        private IQueryable<system_price_rule> BaseQuery() => _db.system_price_rules.AsQueryable();

        public async Task<int> CountFactorsAsync(Guid ruleId, CancellationToken ct)
        {
            return await _db.pricing_factors
                .Where(x => x.rule_id == ruleId)
                .CountAsync(ct);
        }

        public async Task<int> CountCombosAsync(Guid ruleId, CancellationToken ct)
        {
            return await _db.pricing_combos
                .Where(x => x.rule_id == ruleId)
                .CountAsync(ct);
        }

        public async Task<bool> ExistsAsync(Guid ruleId, CancellationToken ct)
        {
            return await _db.system_price_rules
                .AnyAsync(x => x.rule_id == ruleId, ct);
        }

        public IQueryable<system_price_rule> GetQueryable()
        {
            return _db.system_price_rules.AsNoTracking();
        }
    }
}
