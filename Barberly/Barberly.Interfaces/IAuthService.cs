using Barberly.Model.Requests;
using Barberly.Model.Responses;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Barberly.Interfaces
{
    public interface IAuthService
    {
        Task<IdentityResult> RegisterAsync(UserRegisterRequest request);
        Task<UserLoginResponse> LoginAsync(UserLoginRequest request);
    }
}
