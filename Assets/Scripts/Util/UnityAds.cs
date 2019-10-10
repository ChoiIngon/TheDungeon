using System.Collections;
using UnityEngine;
using UnityEngine.Advertisements;
using GoogleMobileAds.Api;
public class UnityAds : MonoBehaviour {
//#if !UNITY_ADS // If the Ads service is not enabled...
    public string gameId = "1305819"; // Set this value from the inspector.
//#endif

    void Start()
    {
//#if !UNITY_ADS // If the Ads service is not enabled...
        if (true == Advertisement.isSupported)
		{ // If runtime platform is supported...
			Debug.Log("initialize advertisement");
            Advertisement.Initialize(gameId); // ...initialize.
        }
		//#endif
		MobileAds.Initialize(initStatus => { });
		ShowBanner();
	}

	private BannerView bannerView;
	private void ShowBanner()
	{
		//string adUnitID = "ca-app-pub-5331343349322603/2002078706";
		string adUnitID = "ca-app-pub-3940256099942544/6300978111";
		bannerView = new BannerView(adUnitID, AdSize.Banner, AdPosition.Bottom);
	}
	public IEnumerator ShowAds()
	{
		while (false == Advertisement.isInitialized || false == Advertisement.IsReady())
		{
			yield return new WaitForSeconds(0.5f);
		}

		// Show the default ad placement.
		Advertisement.Show("video");
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
