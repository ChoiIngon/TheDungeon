using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Advertisements;

public class UnityAdsExample : MonoBehaviour {
//#if !UNITY_ADS // If the Ads service is not enabled...
    public string gameId; // Set this value from the inspector.
    public bool enableTestMode = true;
//#endif

    IEnumerator Start()
    {
//#if !UNITY_ADS // If the Ads service is not enabled...
        if (Advertisement.isSupported) { // If runtime platform is supported...
            Advertisement.Initialize(gameId, enableTestMode); // ...initialize.
        }
//#endif
		while (!Advertisement.isInitialized || !Advertisement.IsReady())
        {
            Debug.Log("initailize:" + Advertisement.isInitialized + ", ready:" + Advertisement.IsReady().ToString());
            yield return new WaitForSeconds(0.5f);
        }

        // Show the default ad placement.
        Advertisement.Show();
    }

    public void ShowRewardedAd()
    {
        if(Advertisement.IsReady("rewardedVideo"))
        {
            var options = new ShowOptions { resultCallback = HandleShowResult };
            Advertisement.Show("rewardedVideo", options);
        }
    }
    private void HandleShowResult(ShowResult result)
    {
        switch (result)
        {
            case ShowResult.Finished:
                Debug.Log("The ad was successfully shown.");
                //
                // YOUR CODE TO REWARD THE GAMER
                // Give coins etc.
                break;
            case ShowResult.Skipped:
                Debug.Log("The ad was skipped before reaching the end.");
                break;
            case ShowResult.Failed:
                Debug.LogError("The ad failed to be shown.");
                break;
        }
    }
}
