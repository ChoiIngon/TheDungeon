using System.Collections;
using UnityEngine;
using UnityEngine.Advertisements;

public class UnityAds : MonoBehaviour {
//#if !UNITY_ADS // If the Ads service is not enabled...
    public string gameId = "1305819"; // Set this value from the inspector.
    public bool enableTestMode = false;
//#endif

    IEnumerator Start()
    {
//#if !UNITY_ADS // If the Ads service is not enabled...
        if (true == Advertisement.isSupported)
		{ // If runtime platform is supported...
            Advertisement.Initialize(gameId, enableTestMode); // ...initialize.
        }
//#endif
		while (false == Advertisement.isInitialized || false == Advertisement.IsReady())
        {
            yield return new WaitForSeconds(0.5f);
        }

		// Show the default ad placement.
		//Advertisement.Show();
		StartCoroutine(ShowBanner());
    }

	IEnumerator ShowBanner()
	{
		Debug.Log("check ready for bannder");
		string placementID = "bottonBanner";
		/*
		while (false == Advertisement.IsReady(placementID))
		{
			yield return new WaitForSeconds(0.5f);
		}
		*/
		Debug.Log("show bannder");
		//Advertisement.Banner.SetPosition(BannerPosition.TOP_CENTER);
		//Advertisement.Banner.Show(placementID);
		yield break;
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
