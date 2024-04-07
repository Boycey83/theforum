using Azure.Storage.Blobs;
using Microsoft.Extensions.Caching.Memory;
using System.Collections.Immutable;

namespace theforum.Whitelabel;

public class FaviconService
{
    private const string ContainerName = "theme";
    private readonly string[] _supportedSizes = { "16", "32", "48", "64", "128", "152", "192", "256" };
        
    private readonly IMemoryCache _cache;
    private readonly BlobServiceClient _blobServiceClient;
    private readonly ILogger<FaviconService> _logger;
    private CustomFavicons _favicons;

    public FaviconService(IMemoryCache cache, BlobServiceClient blobServiceClient, ILogger<FaviconService> logger)
    {
        _cache = cache;
        _blobServiceClient = blobServiceClient;
        _logger = logger;
        _favicons = new CustomFavicons(ImmutableHashSet<string>.Empty);
    }

    public async Task InitializeAsync(CancellationToken cancellationToken)
    {
        var blobContainerClient = _blobServiceClient.GetBlobContainerClient(ContainerName);
        var availableSizes = new List<string>();

        foreach (var size in _supportedSizes)
        {
            try
            {
                var fileName = $"favico-{size}.png";
                var blobClient = blobContainerClient.GetBlobClient(fileName);
                if (!await blobClient.ExistsAsync(cancellationToken))
                {
                    continue;
                }
                var content = await blobClient.DownloadContentAsync(cancellationToken);
                _cache.Set(fileName, content.Value.Content.ToArray()); // No expiration - for indefinite caching
                availableSizes.Add(size);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to load favicon of size {Size}", size);
            }
        }
        _favicons = new CustomFavicons(ImmutableHashSet.CreateRange(availableSizes));
    }

    public byte[]? GetFavicon(string size)
    {
        _cache.TryGetValue($"favico-{size}.png", out byte[]? favicon);
        return favicon;
    }

    public IEnumerable<FaviconDto> GetFaviconPaths() => 
        _favicons.Has32Favicon 
            ? GetCustomFaviconPaths() 
            : GetBuiltinFaviconPaths();

    private IEnumerable<FaviconDto> GetCustomFaviconPaths() => 
        _favicons.AvailableSizes.Select(size => new FaviconDto($"/favicon/{size}.png", $"{size}x{size}"));

    private IEnumerable<FaviconDto> GetBuiltinFaviconPaths() => 
        _supportedSizes.Select(size => new FaviconDto($"/favico-{size}.png", $"{size}x{size}"));
}