using System;
using System.Collections.Generic;

namespace BoxHub.Domain.Entities;

public partial class UserProfile
{
    public Guid ProfileId { get; set; }

    public Guid? UserId { get; set; }

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public string? Gender { get; set; }

    public DateOnly? DateOfBirth { get; set; }

    public string? AvatarUrl { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual User? User { get; set; }
}
