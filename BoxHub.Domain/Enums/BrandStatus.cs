using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoxHub.Domain.Enums
{
    public enum BrandStatus
    {
        Pending,  // Chờ duyệt
        Active,   // Đang hoạt động
        Inactive, // Tạm ngưng (Host tự ngưng)
        Banned,   // Bị hệ thống khóa
        Rejected  // Bị từ chối phê duyệt
    }
}
