using AutoMapper;
using BoxHub.Application.DTOs.Requests.Moderators;
using BoxHub.Application.DTOs.Responses;
using BoxHub.Application.DTOs.Responses.Moderators;
using BoxHub.Application.Interfaces;
using BoxHub.Application.Interfaces.Repositories;
using BoxHub.Application.Interfaces.Services;
using BoxHub.Domain.Entities;
using BoxHub.Domain.Enums;
using BoxHub.Shared.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoxHub.Application.Services
{
    public class ModeratorService : IModeratorService
    {
        private readonly IUserRepository _userRepo;
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;

        public ModeratorService(IUserRepository userRepo, IUnitOfWork uow, IMapper mapper)
        {
            _userRepo = userRepo;
            _uow = uow;
            _mapper = mapper;
        }

        public async Task<PagedResponse<AccountListItem>> GetUsersAsync(AccountFilter filter, CancellationToken ct)
        {
            var (items, total) = await _userRepo
                .GetUsersForModeratorAsync(filter, ct);

            return new PagedResponse<AccountListItem>(
                items,
                total,
                filter.PageNumber,
                filter.PageSize
            );
        }

        public async Task<AccountDetail?> GetUserDetailAsync(Guid userId, CancellationToken ct)
        {
            return await _userRepo.GetUserDetailForModeratorAsync(userId, ct);
        }

        public async Task<SuspendAccountResult> SuspendUserAsync(Guid userId, SuspendAccountRequest request, Guid actorId, UserRole actorRole, CancellationToken ct)
        {
            // get user
            var user = await _userRepo.GetByIdAsync(userId, ct);
            if (user == null)
                throw new Exception("User not found");

            // validations
            if (user.user_id == actorId)
                throw new Exception("You cannot suspend yourself");

            if (user.role == UserRole.Admin)
                throw new Exception("Cannot suspend admin");

            if (user.user_status == UserStatus.Suspended)
                throw new Exception("User already suspended");

            var oldStatus = user.user_status;

            // update status
            user.user_status = UserStatus.Suspended;

            await _userRepo.UpdateAsync(user, ct);

            // audit log
            var audit = new audit_log
            {
                audit_id = Guid.NewGuid(),
                actor_id = actorId,
                actor_role = actorRole.ToString(),
                action = "SUSPEND_USER",

                target_type = "USER",
                target_id = user.user_id,

                old_value = AuditHelper.ToJson(new
                {
                    field = "user_status",
                    old = oldStatus.ToString()
                }),

                new_value = AuditHelper.ToJson(new
                {
                    field = "user_status",
                    @new = user.user_status.ToString()
                }),

                note = request.Reason,
                created_at = DateTime.UtcNow
            };

            await _userRepo.AddAuditLogAsync(audit, ct);

            // save changes
            await _uow.SaveChangesAsync(ct);

            var result = _mapper.Map<SuspendAccountResult>(user);

            result.NewStatus = user.user_status;
            result.SuspendedById = actorId;
            result.SuspendedAt = audit.created_at;

            return result;
        }

    }
}
