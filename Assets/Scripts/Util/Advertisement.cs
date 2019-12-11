using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Advertisements;
using GoogleMobileAds.Api;
using AudienceNetwork;

public class Advertisement : MonoBehaviour
{
	public string unity_game_id = "1305819"; // Set this value from the inspector.
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

	public class FacebookRewardAdvertisement : AdvertisementImpl
	{
		private AudienceNetwork.RewardedVideoAd rewarded_video;
		private GameObject game_object;
		private bool is_loaded;
		private bool complete;
		private bool did_close;
		private bool reward;
		public FacebookRewardAdvertisement(GameObject gameObject)
		{
			game_object = gameObject;
			is_loaded = false;
		}

		public override void Init()
		{
#if UNITY_EDITOR
#else
			LoadAd();
#endif
		}

		public override bool IsReady()
		{
#if UNITY_EDITOR
			return false;
#else
			if (null == rewarded_video && false == is_loaded)
			{
				LoadAd();
			}
			return is_loaded;
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
			did_close = false;

			if (true == is_loaded)
			{
				rewarded_video.Show();
				is_loaded = false;
			}

			while (false == complete)
			{
				yield return new WaitForSeconds(0.1f);
			}

			if (true == reward)
			{
				onSuccess?.Invoke();
			}
#endif
		}

		private void LoadAd()
		{
			rewarded_video = new AudienceNetwork.RewardedVideoAd("445801979415468_445805622748437");
			rewarded_video.Register(game_object);
			rewarded_video.RewardedVideoAdDidLoad = () =>
			{
				is_loaded = true;
			};

			rewarded_video.RewardedVideoAdDidFailWithError = (string error) =>
			{
				Debug.LogError(error);
			};

			rewarded_video.RewardedVideoAdWillLogImpression = (delegate () {
				Debug.Log("RewardedVideo ad logged impression.");
				complete = true;
			});

			rewarded_video.RewardedVideoAdDidClick = (delegate () {
				Debug.Log("RewardedVideo ad clicked.");
				complete = true;
			});

			rewarded_video.RewardedVideoAdDidClose = (delegate () {
				Debug.Log("Rewarded video ad did close.");
				complete = true;
				reward = true;
				did_close = true;
				if (null != rewarded_video)
				{
					rewarded_video.Dispose();
					rewarded_video = null;
				}
			});
			
#if UNITY_ANDROID
			/*
			 * Android에만 적용됩니다. 
			 * 보상형 동영상 활동이 적절히 닫히지 않은 상태에서 
			 * 폐기되었을 경우에만 이 콜백이 트리거됩니다.  launchMode:singleTask가 있는 앱(예: Unity 게임)을 
			 * 백그라운드로 돌렸다가 아이콘을 탭해서 다시 시작하면 이렇게 될 수 있습니다. 
			 */
			rewarded_video.rewardedVideoAdActivityDestroyed = (delegate() 
			{
				if (false == did_close)
				{
					Debug.Log("Rewarded video activity destroyed without being closed first.");
					Debug.Log("Game should resume. User should not get a reward.");
				}
			}); 
#endif

			// Initiate the request to load the ad.
			rewarded_video.LoadAd();
		}
	}

	public class AdMobBannerAdvertisement : AdvertisementImpl
	{
		private BannerView banner_view;

		public override void Init()
		{
			//string ad_unit_id = "ca-app-pub-5331343349322603/2002078706";
			string ad_unit_id = "ca-app-pub-3940256099942544/6300978111";
			banner_view = new BannerView(ad_unit_id, GoogleMobileAds.Api.AdSize.Banner, GoogleMobileAds.Api.AdPosition.Bottom);
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
			return false;
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
		private GoogleMobileAds.Api.InterstitialAd interstitial;
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
			return false;
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

			interstitial = new GoogleMobileAds.Api.InterstitialAd(ad_unit_id);
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

	private Dictionary<PlacementType, List<AdvertisementImpl>> advertisement_impls;

	void Start()
    {
		advertisement_impls = new Dictionary<PlacementType, List<AdvertisementImpl>>()
		{
			{
				PlacementType.Banner, new List<AdvertisementImpl>() {
					new AdMobBannerAdvertisement()
				}
			},
			{
				PlacementType.Video, new List<AdvertisementImpl>() {
					new AdMobAdvertisement(),
					new UnityAdvertisement(),
				}
			},
			{
				PlacementType.Rewarded, new List<AdvertisementImpl>() {
					new FacebookRewardAdvertisement(gameObject),
					new AdMobRewardAdvertisement(),
					new UnityRewardAdvertisement(),
				}
			}
		};

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
		foreach(AdvertisementImpl impl in impls)
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
