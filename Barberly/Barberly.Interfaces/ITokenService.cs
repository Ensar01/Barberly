using Barberly.Model;

namespace Barberly.Interfaces
{
    public interface ITokenService
    {
        string GenerateToken(UserTokenDto user);
    }
}
