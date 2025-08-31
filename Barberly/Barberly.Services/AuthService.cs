using Barberly.Database;
using Barberly.Database.Models;
using Barberly.Interfaces;
using Barberly.Model;
using Barberly.Model.Requests;
using Barberly.Model.Responses;
using MapsterMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Barberly.Services
{
    public class AuthService : IAuthService
    {
        private readonly BarberlyDbContext _context;
        private readonly UserManager<User> _userManager;
        private readonly ITokenService _tokenService;
        private readonly IMapper _mapper;
        private readonly ILogger<AuthService> _logger;
        private readonly SignInManager<User> _signInManager;
        public AuthService(BarberlyDbContext context,UserManager<User> userManager, ITokenService tokenService, IMapper mapper, ILogger<AuthService> logger, SignInManager<User> signInManager)
        {
            _context = context;
            _userManager = userManager;
            _tokenService = tokenService;
            _mapper = mapper;
            _logger = logger;
            _signInManager = signInManager;
        }


        public async Task<IdentityResult> RegisterAsync(UserRegisterRequest request)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync();
            var user = _mapper.Map<User>(request); 
            user.RegistrationDate = DateOnly.FromDateTime(DateTime.UtcNow);
            
            var createUser = await _userManager.CreateAsync(user, request.Password);

            if (!createUser.Succeeded)
            {
                _logger.LogError("User creation failed for {Email}: {Errors}", request.Email,
                    string.Join(", ", createUser.Errors.Select(e => e.Description)));
                return createUser;
            }

            _logger.LogInformation("User {Email} created successfully. Assigning role...", request.Email);
            var userRole = await _userManager.AddToRoleAsync(user, "User");

            if (!userRole.Succeeded)
            {
                _logger.LogError("Failed to assign role 'User' to {Email}: {Errors}", request.Email,
                    string.Join(", ", userRole.Errors.Select(e => e.Description)));
                await transaction.RollbackAsync();
                return IdentityResult.Failed(userRole.Errors.ToArray());
            }

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
            _logger.LogInformation("User {Email} successfully registered and role assigned.", request.Email);


            return IdentityResult.Success;
        }
        public async Task<UserLoginResponse> LoginAsync(UserLoginRequest request)
        {
            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
            if (user == null)
                return null;
            
            var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, false);
            if (!result.Succeeded)
                return null;
            var roles = await _userManager.GetRolesAsync(user);
            var userTokenDto = new UserTokenDto(user.Id, user.Email, roles);
            var token = _tokenService.GenerateToken(userTokenDto);

            // dohvat uloga korisnika

            return new UserLoginResponse
            {
                Id = user.Id,
                Username = user.Email,
                Token = token,
                Roles = roles.ToArray()
            };


        }

       
    }
}
