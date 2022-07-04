using System.Collections.Generic;
using Market;
using Market.Generics;
using UI.Generics;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class OrderButtonItem : MPButton
    {
        public OrdersSelectablePanel myPanel;
        struct ResourceItem
        {
            public Text counterText;
            public Image resourceImage;
            public GameObject itemObject;
        }
        private ResourceItem[] myItems = new ResourceItem[5];
        private Order curr_Order;
        //public 
        public void SetupOrder(Order _selOrder, OrdersSelectablePanel _myPanel)
        {
            myPanel = _myPanel;
            curr_Order = _selOrder;
            myItems[0].itemObject.SetActive(true);
            myItems[0].resourceImage.sprite = CurrencyManagerSingleton.Instance.GoldSprite;
            myItems[0].counterText.text = _selOrder.Reward.ToString();
            int i = 1;
            foreach (KeyValuePair<ResourceType, int> _kvp in _selOrder.ResourceList)
            {
                myItems[i].itemObject.SetActive(true);
                myItems[i].resourceImage.sprite = CurrencyManagerSingleton.Instance.ResourceSprites[_kvp.Key];
                myItems[i].counterText.text = _kvp.Value.ToString();
                i++;
            }
            for (int j = i; j < myItems.Length; j++)
            {
                myItems[j].itemObject.SetActive(false);
            }
        }

        public override void Awake()
        {
            base.Awake();
            int childCount = transform.childCount;
            myItems = new ResourceItem[5];
            for (int i = 0; i < childCount; i++)
            {
                Transform curr_Child = transform.GetChild(i);
                ResourceItem newItem = new ResourceItem();
                newItem.counterText = curr_Child.GetComponentInChildren<Text>();
                newItem.resourceImage = curr_Child.GetComponentInChildren<Image>();
                newItem.itemObject = curr_Child.gameObject;
                newItem.itemObject.SetActive(false);
                myItems[i] = newItem;
            }
        }

        public override void CLICK_BUTTONX()
        {
            base.CLICK_BUTTONX();
            myPanel.ProcessOrder(curr_Order);
        }

    }
}
