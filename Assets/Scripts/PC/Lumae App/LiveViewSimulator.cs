using System;
using System.Collections;
using UnityEngine;

public class LiveViewSimulator
{
    readonly int totalSubs;
    readonly float videoDuration, growthFactor, viralityChance, randomRange;
    readonly float baseAudience, viralMultiplier, noiseOffset;

    public LiveViewSimulator(int totalSubs, float videoDuration, float growthFactor, float viralityChance, float randomRange)
    {
        this.totalSubs = totalSubs;
        this.videoDuration = Mathf.Max(0.1f, videoDuration);
        this.growthFactor = Mathf.Clamp01(growthFactor);
        this.viralityChance = Mathf.Clamp01(viralityChance);
        this.randomRange = Mathf.Max(0f, randomRange);

        baseAudience = totalSubs > 0 ? totalSubs * growthFactor : 0f;
        viralMultiplier = UnityEngine.Random.value < viralityChance ? UnityEngine.Random.Range(2f, 4f) : 1f;
        noiseOffset = UnityEngine.Random.Range(0f, 100f);
    }

    public IEnumerator Simulate(Action<int> onViewerUpdate)
    {
        float elapsed = 0f;
        int lastViewCount = 0;
        float floorFrac = Mathf.Lerp(0.15f, 0.05f, Mathf.Clamp01(videoDuration / 600f));
        float viralSpikeTime = UnityEngine.Random.Range(0.3f, 0.7f);
        float viralSpikeStrength = UnityEngine.Random.Range(1.2f, 2.0f);
        float viralSpikeWidth = UnityEngine.Random.Range(0.05f, 0.15f);

        while (elapsed < videoDuration)
        {
            float t = elapsed / videoDuration;
            float curve = floorFrac + (1f - floorFrac) * Mathf.Sin(t * Mathf.PI);
            float noise = (Mathf.PerlinNoise(t * 2.5f, noiseOffset) - 0.5f) * 2f;
            float noiseAmp = Mathf.Max(baseAudience * 0.15f, randomRange * 0.15f);
            float spike = Mathf.Exp(-Mathf.Pow((t - viralSpikeTime) / viralSpikeWidth, 2)) * viralSpikeStrength;
            float target = (baseAudience * curve + noise * noiseAmp) * viralMultiplier + baseAudience * spike;
            int viewers = Mathf.Max(0, Mathf.FloorToInt(target));
            if (t < 0.1f && viewers < lastViewCount) viewers = lastViewCount;
            lastViewCount = viewers;
            onViewerUpdate?.Invoke(viewers);
            yield return new WaitForSeconds(1f);
            elapsed += 1f;
        }
        onViewerUpdate?.Invoke(lastViewCount);
    }
}
