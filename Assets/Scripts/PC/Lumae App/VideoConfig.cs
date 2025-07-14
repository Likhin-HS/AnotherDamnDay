using UnityEngine;

[System.Serializable]
public struct VideoConfig
{
    public float videoDuration, growthFactor, viralityChance, randomRange, subRate;
    public float earningMultiplier; 
}

public static class VideoConfigs
{
    public static readonly VideoConfig[] configs = {
        new VideoConfig { videoDuration = 10f,   growthFactor = 0.60f, viralityChance = 0.40f, randomRange = 10f, subRate = 0.0919f, earningMultiplier = 0.5f },
        new VideoConfig { videoDuration = 30f,   growthFactor = 0.50f, viralityChance = 0.37f, randomRange = 15f, subRate = 0.0848f, earningMultiplier = 0.7f },
        new VideoConfig { videoDuration = 60f,   growthFactor = 0.40f, viralityChance = 0.34f, randomRange = 20f, subRate = 0.0764f, earningMultiplier = 1.0f },
        new VideoConfig { videoDuration = 120f,  growthFactor = 0.36f, viralityChance = 0.31f, randomRange = 25f, subRate = 0.0612f, earningMultiplier = 1.2f },
        new VideoConfig { videoDuration = 300f,  growthFactor = 0.30f, viralityChance = 0.28f, randomRange = 30f, subRate = 0.0554f, earningMultiplier = 1.4f },
        new VideoConfig { videoDuration = 600f,  growthFactor = 0.24f, viralityChance = 0.25f, randomRange = 35f, subRate = 0.0496f, earningMultiplier = 1.6f },
        new VideoConfig { videoDuration = 1800f, growthFactor = 0.20f, viralityChance = 0.22f, randomRange = 40f, subRate = 0.0468f, earningMultiplier = 1.8f },
        new VideoConfig { videoDuration = 3600f, growthFactor = 0.16f, viralityChance = 0.15f, randomRange = 45f, subRate = 0.0410f, earningMultiplier = 2.0f }
    };
}
