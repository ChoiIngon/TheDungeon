using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Advertisements;
using GoogleMobileAds.Api;

public class Advertisement : MonoBehaviour
{
	//#if !UNITY_ADS // If the Ads service is not enabled...
	public string unity_game_id = "1305819"; // Set this value from the inspector.
											 //#endif
	public enum PlacementType
	{
		Invalid,
		Video,
		Rewarded,
	}

	public abstract class AdvertisementImpl
	{
		public abstract void Init();
		public abstract bool IsReady();
		public abstract IEnumerator Show(System.Action onSuccess);
	}

	public class AdMobRewardAdvertisement : AdvertisementImpl
	{
		private RewardedAd rewarded_ad;
		private BannerView banner_view;
		//private string ad_unit_id = "ca-app-pub-5331343349322603/5159059190";
		private string ad_unit_id = "ca-app-pub-3940256099942544/5224354917";
		private bool complete;
		private bool reward;
		private System.Action on_success;

		public override void Init()
		{
			//string adUnitID = "ca-app-pub-5331343349322603/2002078706";
			string adUnitID = "ca-app-pub-3940256099942544/6300978111";
			banner_view = new BannerView(adUnitID, AdSize.Banner, AdPosition.Bottom);

			AdRequest request = new AdRequest.Builder().Build();
			banner_view.LoadAd(request);
			banner_view.Show();

			LoadAd();
		}

		public override bool IsReady()
		{
#if UNITY_EDITOR
			return true;
#else
			return rewarded_ad.IsLoaded();
#endif
		}

		public override IEnumerator Show(System.Action onSuccess)
		{
			complete = false;
			reward = false;
			on_success = null;
			on_success += onSuccess;
#if UNITY_EDITOR
			on_success?.Invoke();
			yield break;
#else
			rewarded_ad.Show();
			while (false == complete)
			{
				yield return new WaitForSeconds(0.1f);
			}

			if(true == reward)
			{
				on_success?.Invoke();
			}
#endif
		}

		private void LoadAd()
		{
			rewarded_ad = new RewardedAd(ad_unit_id);
			rewarded_ad.OnAdLoaded += HandleRewardedAdLoaded;
			rewarded_ad.OnAdFailedToLoad += HandleRewardedAdFailedToLoad;
			rewarded_ad.OnAdOpening += HandleRewardedAdOpening;
			rewarded_ad.OnAdFailedToShow += HandleRewardedAdFailedToShow;
			rewarded_ad.OnUserEarnedReward += HandleUserEarnedReward;
			rewarded_ad.OnAdClosed += HandleRewardedAdClosed;
			AdRequest request = new AdRequest.Builder().Build();
			rewarded_ad.LoadAd(request);
		}

		public void HandleRewardedAdLoaded(object sender, EventArgs args)
		{
			MonoBehaviour.print("HandleRewardedAdLoaded event received");
		}

		public void HandleRewardedAdFailedToLoad(object sender, AdErrorEventArgs args)
		{
			MonoBehaviour.print("HandleRewardedAdFailedToLoad event received with message: " + args.Message);
		}

		public void HandleRewardedAdOpening(object sender, EventArgs args)
		{
			MonoBehaviour.print("HandleRewardedAdOpening event received");
		}

		public void HandleRewardedAdFailedToShow(object sender, AdErrorEventArgs args)
		{
			MonoBehaviour.print("HandleRewardedAdFailedToShow event received with message: " + args.Message);
		}

		public void HandleRewardedAdClosed(object sender, EventArgs args)
		{
			MonoBehaviour.print("HandleRewardedAdClosed event received");
			LoadAd();
			complete = true;
		}

		public void HandleUserEarnedReward(object sender, Reward args)
		{
			reward = true;
			MonoBehaviour.print("HandleRewardedAdRewarded event received");
		}
	}
	public class AdMobAdvertisement : AdvertisementImpl
	{
		private InterstitialAd interstitial;
		//private string ad_unit_id = "ca-app-pub-5331343349322603/2888119077";
		private string ad_unit_id = "ca-app-pub-3940256099942544/1033173712";
		private bool complete;

		public override void Init()
		{
			LoadAd();
		}

		public override bool IsReady()
		{
#if UNITY_EDITOR
			return true;
#else
			return interstitial.IsLoaded();
#endif
		}

		public override IEnumerator Show(System.Action onSuccess)
		{
#if UNITY_EDITOR
			yield break;
#else
			complete = false;
			interstitial.Show();
			while (false == complete)
			{
				yield return new WaitForSeconds(0.1f);
			}
#endif
		}

		private void LoadAd()
		{
			if (null != interstitial)
			{
				interstitial.Destroy();
			}

			interstitial = new InterstitialAd(ad_unit_id);
			interstitial.OnAdLoaded += HandleOnAdLoaded;
			interstitial.OnAdFailedToLoad += HandleOnAdFailedToLoad;
			interstitial.OnAdOpening += HandleOnAdOpened;
			interstitial.OnAdClosed += HandleOnAdClosed;
			interstitial.OnAdLeavingApplication += HandleOnAdLeavingApplication;

			AdRequest request = new AdRequest.Builder().Build();
			interstitial.LoadAd(request);
		}

		public void HandleOnAdLoaded(object sender, EventArgs args)
		{
			MonoBehaviour.print("HandleAdLoaded event received");
		}

		public void HandleOnAdFailedToLoad(object sender, AdFailedToLoadEventArgs args)
		{
			MonoBehaviour.print("HandleFailedToReceiveAd event received with message: " + args.Message);
		}

		public void HandleOnAdOpened(object sender, EventArgs args)
		{
			MonoBehaviour.print("HandleAdOpened event received");
		}

		public void HandleOnAdClosed(object sender, EventArgs args)
		{
			MonoBehaviour.print("HandleAdClosed event received");
			LoadAd();
			complete = true;
		}

		public void HandleOnAdLeavingApplication(object sender, EventArgs args)
		{
			MonoBehaviour.print("HandleAdLeavingApplication event received");
		}
	}
	public class UnityRewardAdvertisement : AdvertisementImpl
	{
		private bool complete;
		private System.Action on_success;
		private string unity_game_id = "1305819"; // Set this value from the inspector.
		private const string placement_id = "rewardedVideo";
		
		public override void Init()
		{
		}

		public override bool IsReady()
		{
			return UnityEngine.Advertisements.Advertisement.isInitialized && UnityEngine.Advertisements.Advertisement.IsReady();
		}

		public override IEnumerator Show(System.Action onSuccess)
		{
			complete = false;
			on_success = null;
			if (true == UnityEngine.Advertisements.Advertisement.IsReady(placement_id))
			{
				on_success += onSuccess;
				var options = new ShowOptions { resultCallback = HandleShowResult };
				UnityEngine.Advertisements.Advertisement.Show(placement_id, options);
			}
			while (false == complete)
			{
				yield return new WaitForSeconds(0.1f);
			}
		}

		private void HandleShowResult(ShowResult result)
		{
			complete = true;
			switch (result)
			{
				case ShowResult.Finished:
					Debug.Log("The ad was successfully shown.");
					on_success?.Invoke();
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
	public class UnityAdvertisement : AdvertisementImpl
	{
		private bool complete;
		private const string placement_id = "video";

		public override void Init()
		{
		}

		public override bool IsReady()
		{
			return UnityEngine.Advertisements.Advertisement.isInitialized && UnityEngine.Advertisements.Advertisement.IsReady();
		}

		public override IEnumerator Show(System.Action onSuccess)
		{
			complete = false;
			if (true == UnityEngine.Advertisements.Advertisement.IsReady(placement_id))
			{
				var options = new ShowOptions { resultCallback = HandleShowResult };
				UnityEngine.Advertisements.Advertisement.Show(placement_id, options);
			}
			while (false == complete)
			{
				yield return new WaitForSeconds(0.1f);
			}
		}

		private void HandleShowResult(ShowResult result)
		{
			complete = true;
			switch (result)
			{
				case ShowResult.Finished:
					Debug.Log("The ad was successfully shown.");
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

	private Dictionary<PlacementType, List<AdvertisementImpl>> advertisement_impls = new Dictionary<PlacementType, List<AdvertisementImpl>>()
	{
		{
			PlacementType.Video, new List<AdvertisementImpl>() {
				new AdMobAdvertisement(),
				new UnityAdvertisement()
			}
		},
		{
			PlacementType.Rewarded, new List<AdvertisementImpl>() {
				new AdMobRewardAdvertisement(),
				new UnityRewardAdvertisement()
			}
		}
	};

	void Start()
    {
		MobileAds.Initialize(initStatus => { });
		if (true == UnityEngine.Advertisements.Advertisement.isSupported)
		{
			Debug.Log("initialize unity advertisement");
			UnityEngine.Advertisements.Advertisement.Initialize(unity_game_id); // ...initialize.
		}

		foreach (var advertisement in advertisement_impls)
		{
			foreach (var impl in advertisement.Value)
			{
				impl.Init();
			}
		}
	}

	public IEnumerator Show(PlacementType placementType, System.Action onSuccess = null)
	{
		GameManager.Instance.EnableUI(false);
		List<AdvertisementImpl> impls = advertisement_impls[placementType];
		foreach (var impl in impls)
		{
			if (true == impl.IsReady())
			{
				yield return impl.Show(onSuccess);
				break;
			}
		}
		GameManager.Instance.EnableUI(true);
	}
}
