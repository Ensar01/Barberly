namespace Barberly.Model
{
    public record UserTokenDto(string Id, string Username, IEnumerable<string> Roles);
}
