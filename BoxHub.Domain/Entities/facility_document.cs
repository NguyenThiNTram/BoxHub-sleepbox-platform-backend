using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BoxHub.Domain.Enums;

namespace BoxHub.Domain.Entities;
public class facility_document
{
    public Guid document_id { get; set; }

    public Guid facility_id { get; set; }

    public FacilityDocumentType document_type { get; set; }

    public int version { get; set; }

    public string attachments { get; set; } = null!;

    public DateOnly? expiry_date { get; set; }

    public string? document_status { get; set; }

    public Guid? reviewed_by { get; set; }

    public DateTime? reviewed_at { get; set; }

    public string? reject_reason { get; set; }

    public DateTime created_at { get; set; }

    public DateTime? updated_at { get; set; }

    public virtual facility facility { get; set; } = null!;
    public virtual user? reviewed_by_navigation { get; set; }
}
