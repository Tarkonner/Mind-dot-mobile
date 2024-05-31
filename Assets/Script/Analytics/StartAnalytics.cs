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
        StartCoroutine(Flsuh());

        InvokeRepeating("TestInitialization", 0, .5f);
    }

    void TestInitialization()
    {
        if (UnityServices.State == ServicesInitializationState.Initialized)
        {
            Debug.Log("Unity Services Initialized");
        }
        else
            Debug.Log("Not Ini");
    }

    IEnumerator ConsentGiven()
    {
        yield return new WaitForSeconds(1); 
        AnalyticsService.Instance.StartDataCollection();
        Analytics.enabled = true;
        Debug.Log(Analytics.enabled);
    }

    IEnumerator Flsuh()
    {
        yield return new WaitForSeconds(4);
        AnalyticsResult ar = Analytics.CustomEvent("gameRunning");
        Debug.Log(ar);
    }
}
