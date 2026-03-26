using BoxHub.Shared.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoxHub.Application.Interfaces.Services
{
    public interface IEmailService
    {
        Task<bool> SendEmailAsync(MailData mailData);
    }
}
