using Azure.Storage.Blobs;
using Microsoft.Extensions.Caching.Memory;
using System.Collections.Immutable;

namespace theforum.Whitelabel;

public class FaviconService
{
    private readonly IMemoryCache _cache;
    private readonly BlobServiceClient _blobServiceClient;
    private const string ContainerName = "theme";
    private readonly ILogger<FaviconService> _logger;
    public CustomFavicons Favicons { get; private set; }
    private readonly string[] _supportedSizes = { "16", "32", "48", "64", "128", "152", "192", "256" };

    public FaviconService(IMemoryCache cache, BlobServiceClient blobServiceClient, ILogger<FaviconService> logger)
    {
        _cache = cache;
        _blobServiceClient = blobServiceClient;
        _logger = logger;
        Favicons = new CustomFavicons(ImmutableHashSet<string>.Empty);
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
                // TODO: Figure out if the memory cache has other eviction policies that may affect us
                availableSizes.Add(size);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to load favicon of size {Size}", size);
            }
        }
        Favicons = new CustomFavicons(ImmutableHashSet.CreateRange(availableSizes));
    }

    public byte[]? GetFavicon(string size)
    {
        _cache.TryGetValue($"favico-{size}.png", out byte[]? favicon);
        return favicon;
    }
}
