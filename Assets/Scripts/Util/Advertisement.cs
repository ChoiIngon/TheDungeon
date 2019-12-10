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
	private int ad_ready_index = 0;
	public enum PlacementType
	{
		Invalid,
		Banner,
		Video,
		Rewarded,
	}

	public abstract class AdvertisementImpl
	{
		public abstract void Init();
		public abstract bool IsReady();
		public abstract IEnumerator Show(System.Action onSuccess);
	}

	public class AdMobBannerAdvertisement : AdvertisementImpl
	{
		private BannerView banner_view;

		public override void Init()
		{
			//string ad_unit_id = "ca-app-pub-5331343349322603/2002078706";
			string ad_unit_id = "ca-app-pub-3940256099942544/6300978111";
			banner_view = new BannerView(ad_unit_id, AdSize.Banner, AdPosition.Bottom);
			AdRequest request = new AdRequest.Builder().Build();
			banner_view.LoadAd(request);
			banner_view.Show();
		}

		public override bool IsReady()
		{
			return true;
		}

		public override IEnumerator Show(Action onSuccess)
		{
			yield break;
		}
	}

	public class AdMobRewardAdvertisement : AdvertisementImpl
	{
		private RewardedAd rewarded_ad;
		//private string ad_unit_id = "ca-app-pub-5331343349322603/5159059190";
		private string ad_unit_id = "ca-app-pub-3940256099942544/5224354917";
		private bool complete;
		private bool reward;
		
		public override void Init()
		{
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
			
#if UNITY_EDITOR
			onSuccess?.Invoke();
			yield break;
#else
			complete = false;
			reward = false;
			rewarded_ad.Show();
			while (false == complete)
			{
				yield return new WaitForSeconds(0.1f);
			}

			if(true == reward)
			{
				onSuccess?.Invoke();
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
			complete = true;
		}

		public void HandleRewardedAdClosed(object sender, EventArgs args)
		{
			MonoBehaviour.print("HandleRewardedAdClosed event received");
			LoadAd();
			complete = true;
		}

		public void HandleUserEarnedReward(object sender, Reward args)
		{
			MonoBehaviour.print("HandleRewardedAdRewarded event received");
			reward = true;
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
			complete = true;
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
			complete = true;
		}
	}
	public class UnityRewardAdvertisement : AdvertisementImpl
	{
		private bool complete;
		private bool reward;
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
			reward = false;
			if (true == UnityEngine.Advertisements.Advertisement.IsReady(placement_id))
			{
				var options = new ShowOptions { resultCallback = HandleShowResult };
				UnityEngine.Advertisements.Advertisement.Show(placement_id, options);
			}
			while (false == complete)
			{
				yield return new WaitForSeconds(0.1f);
			}

			if (true == reward)
			{
				onSuccess?.Invoke();
			}
		}

		private void HandleShowResult(ShowResult result)
		{
			complete = true;
			switch (result)
			{
				case ShowResult.Finished:
					Debug.Log("The ad was successfully shown.");
					reward = true;
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
			PlacementType.Banner, new List<AdvertisementImpl>() {
				new AdMobBannerAdvertisement()
			}
		},
		{
			PlacementType.Video, new List<AdvertisementImpl>() {
				new UnityAdvertisement(),
				new AdMobAdvertisement(),
			}
		},
		{
			PlacementType.Rewarded, new List<AdvertisementImpl>() {
				new UnityRewardAdvertisement(),
				new AdMobRewardAdvertisement(),
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
		for(int i=0; i<impls.Count; i++)
		{
			ad_ready_index = ad_ready_index % impls.Count;
			AdvertisementImpl impl = impls[ad_ready_index++];
			if (true == impl.IsReady())
			{
				yield return impl.Show(onSuccess);
				break;
			}
		}
		GameManager.Instance.EnableUI(true);
	}
}
