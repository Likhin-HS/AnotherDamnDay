using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Defines the core viewership pattern for a video's duration.
/// </summary>
public enum ViewershipPattern { Standard, LatePeak, Volatile, ViralSpike, SteadyGrowth, Comeback }

public class LiveViewSimulator
{
    // Simulation parameters
    private readonly float videoDuration;
    private readonly float baseAudience;
    private readonly float viralMultiplier;
    private readonly float randomRange;
    private readonly float noiseOffset;
    private readonly ViewershipPattern pattern;

    // Event-based parameters
    private readonly List<(float time, float strength)> events = new List<(float, float)>();

    public LiveViewSimulator(int totalSubs, float videoDuration, float growthFactor, float viralityChance, float randomRange)
    {
        this.videoDuration = Mathf.Max(0.1f, videoDuration);
        this.randomRange = Mathf.Max(0f, randomRange);

        // Core audience calculation with diminishing returns
        this.baseAudience = Mathf.Max(5f, CalculateDiminishingSubs(totalSubs) * growthFactor);

        // The viral multiplier is now influenced by virality chance for a more direct impact
        this.viralMultiplier = 1f + (UnityEngine.Random.value < viralityChance ? UnityEngine.Random.Range(1.5f, 3.5f) : 0f);
        
        this.noiseOffset = UnityEngine.Random.Range(0f, 100f);

        // --- Intelligent Pattern Selection ---
        this.pattern = SelectPattern(videoDuration);
        
        // --- Dynamic Event Generation ---
        GenerateEvents(viralityChance);
    }

    /// <summary>
    /// Calculates the potential audience based on subscriber count, with diminishing returns.
    /// </summary>
    private float CalculateDiminishingSubs(int subs)
    {
        if (subs <= 0) return 0;
        // A square root model provides a balanced curve for initial and late-game growth.
        return Mathf.Sqrt(subs) * 50f;
    }

    /// <summary>
    /// Intelligently selects a viewership pattern based on video duration.
    /// </summary>
    private ViewershipPattern SelectPattern(float duration)
    {
        var selectedPattern = (ViewershipPattern)UnityEngine.Random.Range(0, Enum.GetValues(typeof(ViewershipPattern)).Length);

        // For long videos, avoid patterns that are just a single slow ramp to prevent boredom.
        if (duration > 300f && selectedPattern == ViewershipPattern.LatePeak)
        {
            var suitablePatterns = new[] { ViewershipPattern.Standard, ViewershipPattern.Volatile, ViewershipPattern.ViralSpike, ViewershipPattern.Comeback };
            return suitablePatterns[UnityEngine.Random.Range(0, suitablePatterns.Length)];
        }
        return selectedPattern;
    }

    /// <summary>
    /// Generates dynamic events that can occur during the video, influenced by virality.
    /// </summary>
    private void GenerateEvents(float viralityChance)
    {
        // Higher virality means a higher chance of more, stronger events.
        int numEvents = UnityEngine.Random.value < viralityChance * 0.5f ? UnityEngine.Random.Range(1, 4) : UnityEngine.Random.Range(0, 2);

        for (int i = 0; i < numEvents; i++)
        {
            float eventTime = UnityEngine.Random.Range(0.2f, 0.9f);
            // Events can be positive (spikes) or negative (dips).
            float eventStrength = (UnityEngine.Random.value < 0.8f) ? UnityEngine.Random.Range(1.2f, 2.5f) : UnityEngine.Random.Range(-0.5f, -0.2f);
            events.Add((eventTime, eventStrength));
        }
    }

    /// <summary>
    /// Runs the main viewership simulation.
    /// </summary>
    public IEnumerator Simulate(Action<int> onViewerUpdate)
    {
        float elapsed = 0f;
        int lastViewCount = 0;
        // The "floor" ensures viewership doesn't drop to zero in the middle of a video.
        float floorFrac = Mathf.Lerp(0.2f, 0.05f, Mathf.Clamp01(videoDuration / 600f));

        while (elapsed < videoDuration)
        {
            float t = elapsed / videoDuration;

            // 1. Calculate the base curve from the selected pattern
            float baseCurveValue = GetCurveValueForPattern(t, pattern);
            float curve = floorFrac + (1f - floorFrac) * baseCurveValue;

            // 2. Apply organic noise for realism
            float noise = (Mathf.PerlinNoise(t * 2.5f, noiseOffset) - 0.5f) * 2f;
            float noiseAmp = Mathf.Max(baseAudience * 0.1f, randomRange * 0.1f);

            // 3. Apply dynamic events (spikes and dips)
            float eventImpact = 0f;
            foreach (var (time, strength) in events)
            {
                eventImpact += CalculateEventSpike(t, time, strength);
            }

            // 4. Combine all factors to get the final viewer count
            float target = (baseAudience * curve + noise * noiseAmp) * viralMultiplier + baseAudience * eventImpact;
            int viewers = Mathf.Max(0, Mathf.FloorToInt(target));

            // Prevent sharp, unnatural drops at the very beginning
            if (t < 0.1f && viewers < lastViewCount) viewers = lastViewCount;
            
            lastViewCount = viewers;
            onViewerUpdate?.Invoke(viewers);

            yield return new WaitForSeconds(1f);
            elapsed += 1f;
        }
        onViewerUpdate?.Invoke(lastViewCount);
    }

    /// <summary>
    /// Calculates the value of the base viewership curve for a given time and pattern.
    /// </summary>
    private float GetCurveValueForPattern(float t, ViewershipPattern p)
    {
        switch (p)
        {
            case ViewershipPattern.LatePeak:
                return Mathf.Sin(t * Mathf.PI / 2f);
            case ViewershipPattern.Volatile:
                // Blend two layers of Perlin noise for a more organic, less uniform feel.
                float highFreq = Mathf.PerlinNoise(t * 15f, noiseOffset + 10f) * 0.7f;
                float lowFreq = Mathf.PerlinNoise(t * 5f, noiseOffset - 10f) * 0.3f;
                return (highFreq + lowFreq);
            case ViewershipPattern.SteadyGrowth:
                return t; // Simple linear growth.
            case ViewershipPattern.Comeback:
                // A double-humped curve using two sine waves.
                float firstPeak = Mathf.Sin(t * Mathf.PI * 2f) * 0.6f;
                float secondPeak = Mathf.Sin(t * Mathf.PI) * 0.4f;
                return firstPeak + secondPeak;
            case ViewershipPattern.Standard:
            case ViewershipPattern.ViralSpike: // ViralSpike uses the standard curve but adds discrete events.
            default:
                // Asymmetrical bell curve: slower rise, faster drop-off.
                float curveValue = Mathf.Sin(t * Mathf.PI);
                return (t > 0.5f) ? Mathf.Pow(curveValue, 1.5f) : curveValue;
        }
    }

    /// <summary>
    /// Calculates the impact of a single viewership event (spike or dip).
    /// </summary>
    private float CalculateEventSpike(float t, float spikeTime, float spikeStrength)
    {
        float spikeWidth = UnityEngine.Random.Range(0.04f, 0.12f);
        return Mathf.Exp(-Mathf.Pow((t - spikeTime) / spikeWidth, 2)) * spikeStrength;
    }
}
