namespace LonelyApi.DTOs;

public class PostRequest
{
    public string Content { get; set; }
    public List<string> Images { get; set; }
    public string Audio { get; set; }
}