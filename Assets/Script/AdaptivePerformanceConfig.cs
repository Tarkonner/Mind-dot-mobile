using UnityEngine;
using UnityEngine.AdaptivePerformance;
public class AdaptivePerformanceConfig
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
    static void Setup()
    {
        IAdaptivePerformanceSettings settings = AdaptivePerformanceGeneralSettings.Instance.Manager.activeLoader.GetSettings();
        settings.logging = false; // Disable logging
    }
}
