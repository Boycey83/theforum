using System.Collections.Immutable;

namespace theforum.Whitelabel;

public class CustomFavicons
{
    public CustomFavicons(IImmutableSet<string> availableSizes)
    {
        AvailableSizes = availableSizes;
    }
    public IImmutableSet<string> AvailableSizes { get; }

    public bool Has32Favicon => AvailableSizes.Contains("32");
};
