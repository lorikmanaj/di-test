using AMS.Domain.Models;

namespace AMS.Infrastructure.Security
{
    public interface IJwtTokenService
    {
        string GenerateJwtToken(User user);
    }
}
