namespace LonelyApi.DTOs;

public class LoginResponse
{
    public bool Success { get; set; }
    public string Message { get; set; }
    public string Token { get; set; }
    public UserInfo User { get; set; }
}

public class UserInfo
{
    public string AnonUID { get; set; }
    public string Nickname { get; set; }
    public string Avatar { get; set; }
    public string? Signature { get; set; }
    public string? LoginType { get; set; }
}
