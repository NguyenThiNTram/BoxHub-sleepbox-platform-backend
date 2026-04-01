using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace BoxHub.Shared.Helpers
{
    public static class AuditHelper
    {
        public static string? ToJson(object? obj)
        {
            return obj == null ? null : JsonSerializer.Serialize(obj);
        }
    }
}
