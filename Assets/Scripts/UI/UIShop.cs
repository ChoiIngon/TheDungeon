﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIShop : MonoBehaviour
{
	public class Product
	{
		public int index;
		public Item.Meta meta;
		public Image item_image;
		public Image soldout_image;
		public ImageOutline outline;
		public bool soldout;
		public Transform transform;
	};

	private const int PRODUCT_COUNT = 3;
	private UICoin coin;
	private Text item_name;
	private Text price;
	private Text description;
	private Button buy;
	private Text buy_button_text;
	private Button close;
	private List<Product> products;
	private int current_index;
	private bool complete;

	private void Awake()
	{
		item_name = UIUtil.FindChild<Text>(transform, "ItemInfo/ItemName");
		description = UIUtil.FindChild<Text>(transform, "ItemInfo/Description");
		price = UIUtil.FindChild<Text>(transform, "ItemInfo/Price/Value");
		buy = UIUtil.FindChild<Button>(transform, "ItemInfo/Buy");
		buy_button_text = UIUtil.FindChild<Text>(transform, "ItemInfo/Buy/Text");
		close = UIUtil.FindChild<Button>(transform, "BottomBar/Close");
		coin = UIUtil.FindChild<UICoin>(transform, "Npc/UICoin");
	}

	private void Start()
	{
		buy.onClick.AddListener(Buy);
		close.onClick.AddListener(() => {
			complete = true;
		});
	}

	private void OnEnable()
	{
		Util.EventSystem.Publish(EventID.Shop_Open);
	}

	private void OnDisable()
	{
		Util.EventSystem.Publish(EventID.Shop_Close);
	}

	public IEnumerator Open()
	{
		gameObject.SetActive(true);
		complete = false;
		if (null != products)
		{
			foreach (Product product in products)
			{
				product.transform.SetParent(null);
				Object.Destroy(product.transform.gameObject);
			}
		}

		products = new List<Product>();
		Transform slots = UIUtil.FindChild<Transform>(transform, "ItemSlots");
		Transform productPrefab = UIUtil.FindChild<Transform>(slots, "ItemSlot");
		productPrefab.gameObject.SetActive(false);
		for (int i = 0; i < PRODUCT_COUNT; i++)
		{
			int index = i;
			Product product = new Product();
			product.index = i;

			product.transform = GameObject.Instantiate<Transform>(productPrefab);
			product.transform.SetParent(slots, false);
			product.transform.gameObject.SetActive(true);
			UIUtil.AddPointerUpListener(product.transform.gameObject, () =>
			{
				SelectProduct(index);
			});

			product.outline = product.transform.GetComponent<ImageOutline>();
			product.item_image = UIUtil.FindChild<Image>(product.transform, "Item");
			product.soldout_image = UIUtil.FindChild<Image>(product.transform, "Soldout");
			product.soldout_image.gameObject.SetActive(false);
			product.soldout = false;

			InitProduct(product, ItemManager.Instance.GetRandomExpendableItemMeta());
			products.Add(product);
		}

		InitProduct(products[Random.Range(0, products.Count)], ItemManager.Instance.FindMeta<HealPotionItem.Meta>("ITEM_POTION_HEALING"));
		SelectProduct(0);

		Util.EventSystem.Publish(EventID.CoinAmountChanged);
		while (false == complete)
		{
			yield return null;
		}
		gameObject.SetActive(false);
	}

	private void InitProduct(Product product, Item.Meta meta)
	{
		product.item_image.sprite = ResourceManager.Instance.Load<Sprite>(meta.sprite_path);
		product.meta = meta;
	}

	private void SelectProduct(int index)
	{
		foreach (Product p in products)
		{
			p.outline.active = false;
		}

		current_index = index;

		Product product = products[index];
		item_name.text = product.meta.name;
		description.text = product.meta.description;
		price.text = (product.meta.price * GameManager.Instance.player.level).ToString();
		buy_button_text.text = "Buy";
		product.soldout_image.gameObject.SetActive(false);
		if (true == product.soldout)
		{
			buy_button_text.text = "SOLD OUT";
			product.soldout_image.gameObject.SetActive(true);
		}
		product.outline.active = true;
	}

	private void Buy()
	{
		Product product = products[current_index];
		if (GameManager.Instance.player.coin < product.meta.price * GameManager.Instance.player.level)
		{
			GameManager.Instance.ui_textbox.AsyncWrite("코인이 부족합니다");
			return;
		}

		if (Inventory.MAX_SLOT_COUNT <= GameManager.Instance.player.inventory.count)
		{
			GameManager.Instance.ui_textbox.AsyncWrite("가방이 가득 찼습니다");
			return;
		}

		if (true == product.soldout)
		{
			GameManager.Instance.ui_textbox.AsyncWrite("더 이상 구매 할 수 없습니다");
			return;
		}

		GameManager.Instance.player.ChangeCoin(-product.meta.price * GameManager.Instance.player.level, true);

		product.soldout = true;
		product.soldout_image.gameObject.SetActive(product.soldout);
		Item item = product.meta.CreateInstance();
		GameManager.Instance.player.inventory.Add(item);
		GameManager.Instance.ui_textbox.AsyncWrite(product.meta.name + " 을 구매했습니다");
		buy_button_text.text = "SOLD OUT";
	}
}
