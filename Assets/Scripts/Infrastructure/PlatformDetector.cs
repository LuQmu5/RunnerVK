using UnityEngine;

public static class PlatformDetector
{
    private const int MobileScreenWidthThreshold = 800;

    public static bool IsMobileByResolution()
    {
        return Screen.width <= MobileScreenWidthThreshold;
    }
}

