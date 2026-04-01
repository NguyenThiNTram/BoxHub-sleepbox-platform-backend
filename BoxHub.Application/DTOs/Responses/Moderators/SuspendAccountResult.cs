using BoxHub.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoxHub.Application.DTOs.Responses.Moderators
{
    public class SuspendAccountResult
    {
        public Guid UserId { get; set; }

        public string Username { get; set; } = string.Empty;

        public UserStatus NewStatus { get; set; }

        public Guid SuspendedById { get; set; }

        public DateTime SuspendedAt { get; set; } //luu audit log khi suspend account
    }
}
