namespace LonelyApi.DTOs;

public class BottleRequest
{
    public string Content { get; set; }
    public string? VoiceUrl { get; set; }
    public string? ImageUrl { get; set; }
    public string? Emotion { get; set; }
    public string? Topics { get; set; }
}
