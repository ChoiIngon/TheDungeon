using System.Collections;
using UnityEngine;
using UnityEngine.Advertisements;
using GoogleMobileAds.Api;
public class Advertisement : MonoBehaviour {
//#if !UNITY_ADS // If the Ads service is not enabled...
    public string unity_game_id = "1305819"; // Set this value from the inspector.
//#endif
	private BannerView bannerView;

    void Start()
    {
//#if !UNITY_ADS // If the Ads service is not enabled...
        if (true == UnityEngine.Advertisements.Advertisement.isSupported)
		{ // If runtime platform is supported...
			Debug.Log("initialize advertisement");
			UnityEngine.Advertisements.Advertisement.Initialize(unity_game_id); // ...initialize.
        }
		//#endif
		MobileAds.Initialize(initStatus => { });
		//string adUnitID = "ca-app-pub-5331343349322603/2002078706";
		string adUnitID = "ca-app-pub-3940256099942544/6300978111";
		bannerView = new BannerView(adUnitID, AdSize.Banner, AdPosition.Bottom);
		// Create an empty ad request.
		AdRequest request = new AdRequest.Builder().Build();

		// Load the banner with the request.
		bannerView.LoadAd(request);
		bannerView.Show();
	}

	public void ShowBanner()
	{
		bannerView.Show();
	}

	public void HideBanner()
	{
		bannerView.Hide();
	}

	public IEnumerator ShowAds()
	{
		while (false == UnityEngine.Advertisements.Advertisement.isInitialized || false == UnityEngine.Advertisements.Advertisement.IsReady())
		{
			yield return new WaitForSeconds(0.5f);
		}

		// Show the default ad placement.
		UnityEngine.Advertisements.Advertisement.Show("video");
	}
	
    public void ShowRewardedAd()
    {
        if(UnityEngine.Advertisements.Advertisement.IsReady("rewardedVideo"))
        {
            var options = new ShowOptions { resultCallback = HandleShowResult };
			UnityEngine.Advertisements.Advertisement.Show("rewardedVideo", options);
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
