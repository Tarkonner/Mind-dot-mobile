using System;
using System.Collections;
using Unity.Services.Analytics;
using Unity.Services.Core;
using Unity.Services.Core.Environments;
using UnityEngine;
using UnityEngine.Analytics;

public class StartAnalytics : MonoBehaviour
{
    public string environment = "production";

    async void Start()
    {
        try
        {
            var options = new InitializationOptions().SetEnvironmentName(environment);
            await UnityServices.InitializeAsync(options);
            
        }
        catch (Exception exception)
        {
            Debug.LogError($"Failed to initialize Unity Services: {exception.Message}");
        }
        


        StartCoroutine(ConsentGiven());
    }

    IEnumerator ConsentGiven()
    {
        yield return new WaitForSeconds(1); 
        AnalyticsService.Instance.StartDataCollection();
    }

    IEnumerator Flsuh()
    {
        yield return new WaitForSeconds(4);
        //AnalyticsResult ar = AnalyticsService.Instance.RecordEvent();
    }
}
