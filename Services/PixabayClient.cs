using System.Runtime.Serialization.Json;
using System.Text.Json;
using media_bot.DTO.Photo;
using media_bot.DTO.Video;

namespace media_bot.Services;
public class PixabayClient
{
    private readonly HttpClient _client;
    private readonly ILogger<PixabayClient> _logger;

    public PixabayClient(HttpClient client, ILogger<PixabayClient> logger)
    {
        _client = client;
        _logger = logger;
    }
    public async Task<(Video video, bool IsSuccess, Exception e)> GetVideoAsync(string theme)
    {
        theme = theme.Replace(" ", "+");
        var query = $"videos/?key=25465013-37fc5927a1716bf38d074a2b0&q={theme}&pretty=true";
        using var httpResponse = await _client.GetAsync(query);
        if(httpResponse.IsSuccessStatusCode)
        {
            var json = await httpResponse.Content.ReadAsStringAsync();
            var data = JsonSerializer.Deserialize<Video>(json);
            return (data, true, null);
        }
        return (null, false, new Exception(httpResponse.ReasonPhrase));
    }
    public async Task<(Photo photo, bool IsSuccess, Exception e)> GetPhotoAsync(string theme)
    {
        theme = theme.Replace(" ", "+");
        var query = $"?key=25465013-37fc5927a1716bf38d074a2b0&q={theme}&image_type=photo&pretty=true";
        using var httpResponse = await _client.GetAsync(query);
        if(httpResponse.IsSuccessStatusCode)
        {
            var json = await httpResponse.Content.ReadAsStringAsync();
            var data = JsonSerializer.Deserialize<Photo>(json);
            return (data, true, null);
        }
        return (null, false, new Exception(httpResponse.ReasonPhrase));
    }
}