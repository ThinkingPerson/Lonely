namespace LonelyApi.DTOs;

public class MessageRequest
{
    public int ToUserId { get; set; }
    public string Content { get; set; }
    public string Type { get; set; }
}
