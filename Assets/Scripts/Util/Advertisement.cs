using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Advertisements;
using GoogleMobileAds.Api;

public class Advertisement : MonoBehaviour {
//#if !UNITY_ADS // If the Ads service is not enabled...
    public string unity_game_id = "1305819"; // Set this value from the inspector.
//#endif
	private BannerView bannerView;

	public enum PlacementType
	{
		Invalid,
		Video,
		Rewarded,
	}
	
	public abstract class RewardAdvertisement
	{
		public abstract void Init();
		public abstract bool IsReady();
		public abstract void Show(PlacementType placementType);
		public abstract string GetPlacementID(PlacementType placementType);
	}

	public class AdMobRewardAdvertisement : RewardAdvertisement
	{
		private BannerView banner_view;
		private Dictionary<PlacementType, string> placementIDs = new Dictionary<PlacementType, string>() {
			{ PlacementType.Invalid, "invalid" },
			{ PlacementType.Rewarded, "rewarded" }
		};

		public override void Init()
		{
			MobileAds.Initialize(initStatus => { });
			//string adUnitID = "ca-app-pub-5331343349322603/2002078706";
			string adUnitID = "ca-app-pub-3940256099942544/6300978111";
			banner_view = new BannerView(adUnitID, AdSize.Banner, AdPosition.Bottom);

			AdRequest request = new AdRequest.Builder().Build();
			banner_view.LoadAd(request);
			banner_view.Show();
		}

		public override bool IsReady()
		{
			return true;
		}
		public override void Show(PlacementType placementType)
		{
		}

		public override string GetPlacementID(PlacementType placementType)
		{
			return "";
		}
	}

	public class UnityRewardAdvertisement : RewardAdvertisement
	{
		private string unity_game_id = "1305819"; // Set this value from the inspector.
		private Dictionary<PlacementType, string> placementIDs = new Dictionary<PlacementType, string>() {
			{ PlacementType.Invalid, "invalid" },
			{ PlacementType.Video, "video" },
			{ PlacementType.Rewarded, "rewardedVideo" }
		};

		public override void Init()
		{
			if (true == UnityEngine.Advertisements.Advertisement.isSupported)
			{ // If runtime platform is supported...
				Debug.Log("initialize unity advertisement");
				UnityEngine.Advertisements.Advertisement.Initialize(unity_game_id); // ...initialize.
			}
		}

		public override bool IsReady()
		{
			return UnityEngine.Advertisements.Advertisement.isInitialized && UnityEngine.Advertisements.Advertisement.IsReady();
		}

		public override void Show(PlacementType placementType)
		{
			if (true == UnityEngine.Advertisements.Advertisement.IsReady(GetPlacementID(placementType)))
			{
				var options = new ShowOptions { resultCallback = HandleShowResult };
				UnityEngine.Advertisements.Advertisement.Show(GetPlacementID(placementType), options);
			}
		}

		public override string GetPlacementID(PlacementType placementType)
		{
			return placementIDs[placementType];
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

	private List<RewardAdvertisement> advertisements = new List<RewardAdvertisement>()
	{
		new UnityRewardAdvertisement(),
		new AdMobRewardAdvertisement()
	};

	void Start()
    {
		foreach (var advertisement in advertisements)
		{
			advertisement.Init();
		}

		Show(PlacementType.Video);
		/*
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
		*/
	}

	public void Show(PlacementType placementType)
	{
		foreach (var advertisement in advertisements)
		{
			if (true == advertisement.IsReady())
			{
				advertisement.Show(placementType);
				break;
			}
		}
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
