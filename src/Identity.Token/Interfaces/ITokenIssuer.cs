namespace AspNetCore.Authentication.Identity.Token.Interfaces
{
    public interface ITokenIssuer
    {
        string IssueAccessToken();
    }
}
