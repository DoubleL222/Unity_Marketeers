using System;
using System.Collections.Generic;
using System.Linq;
using Systems;
using Entities;
using Market.Generics;
using UnityEngine;
using Utils;

namespace Market
{
    public class CurrencyManagerSingleton : SingletonBehaviour<CurrencyManagerSingleton> {
        public Dictionary<ResourceType, int[]> ResourceValues;
        public Dictionary<ResourceType, Sprite> ResourceSprites;

        public int SellValueWood;
        public int SellValueIron;
        public int SellValueSpice;
        public int SellValueGem;

        public int BuyValueWood;
        public int BuyValueIron;
        public int BuyValueSpice;
        public int BuyValueGem;

        public float BuyValuePlusRatio = 0.3f;

        public Sprite IronSprite;
        public Sprite WoodSprite;
        public Sprite GemSprite;
        public Sprite SpiceSprite;

        public Sprite GoldSprite;
        //Floats going between 0.1-1.0, which symbolize how much of a resource the ship has.
        private float _ironFloat;
        private float _woodFloat;
        private float _gemFloat;
        private float _spiceFloat;

        //Used to maintain the relationship between the resources and how fast the player acquires them.
        public float WoodPickFloat;
        public float IronPickFloat;
        public float SpicePickFloat;
        public float GemPickFloat;

        //Handle order
        private int _numberOfOrdersSoFar;
        private int _orderIdGivenOut;
        //Order bonuses
        //private const float _sameResourceBonus = 1.05f;
        private const float DifferentResourceBonus2 = 1.2f;
        private const float DifferentResourceBonus3 = 1.1f;
        private const float DifferentResourceBonus4 = 1.1f;

        private List<Order> _orderList;
        private int _maxNumberOfOrders; //Max number of orders that can be in the orderlist
        private Dictionary<int, ResourceType> _resourceList;

        protected override void OnAwake()
        {
            _maxNumberOfOrders = 6;
            ResourceSprites = new Dictionary<ResourceType, Sprite>();
            ResourceSprites.Add(ResourceType.Iron, IronSprite);
            ResourceSprites.Add(ResourceType.Wood, WoodSprite);
            ResourceSprites.Add(ResourceType.Spice, SpiceSprite);
            ResourceSprites.Add(ResourceType.Gem, GemSprite);

            //Float
            _ironFloat = 0.5f;
            _woodFloat = 0.5f;
            _gemFloat = 0.5f;
            _spiceFloat = 0.5f;

            //Sell price
            SellValueIron = DetermineIronValue(0.0f);
            SellValueWood = DetermineWoodValue(0.0f);
            SellValueSpice = DetermineSpiceValue(0.0f);
            SellValueGem = DetermineGemValue(0.0f);

            //Buy price
            BuyValueIron = DetermineIronValue(BuyValuePlusRatio);
            BuyValueWood = DetermineWoodValue(BuyValuePlusRatio);
            BuyValueSpice = DetermineSpiceValue(BuyValuePlusRatio);
            BuyValueGem = DetermineGemValue(BuyValuePlusRatio);

            ResourceValues = new Dictionary<ResourceType, int[]>();
            ResourceValues.Add(ResourceType.Iron, new int[] { SellValueIron, BuyValueIron });
            ResourceValues.Add(ResourceType.Wood, new int[] { SellValueWood, BuyValueWood });
            ResourceValues.Add(ResourceType.Spice, new int[] { SellValueSpice, BuyValueSpice });
            ResourceValues.Add(ResourceType.Gem, new int[] { SellValueGem, BuyValueGem });

            _resourceList = new Dictionary<int, ResourceType>();
            _resourceList.Add(0, ResourceType.Iron);
            _resourceList.Add(1, ResourceType.Wood);
            _resourceList.Add(2, ResourceType.Spice);
            _resourceList.Add(3, ResourceType.Gem);
        }

        // Use this for initialization
        void Start() {
            //Orders
            _orderList = new List<Order>();
            _numberOfOrdersSoFar = 0;
            _orderIdGivenOut = 0;
            for (int i = 0; i < _maxNumberOfOrders; i++) {
                AddNewOrder();
            }

            MarketeersGameManager.Instance.Ship.OnLeavePort += IncreasePricesWhenLeavingPort;
        }

        public void ResetNumberOfMaxNumberOfOrders(){
            _maxNumberOfOrders = 6; 
        }

        private void IncreasePricesWhenLeavingPort(int portPlayerNum)
        {
            NewPrices(0.2f);
        }

        public void NewPrices(float changeValue){
            if (changeValue >= 0.0f)
            {
                ChangeResource(ResourceType.Wood, true, changeValue);
                ChangeResource(ResourceType.Iron, true, changeValue);
                ChangeResource(ResourceType.Spice, true, changeValue);
                ChangeResource(ResourceType.Gem, true, changeValue);
            }
            else
            {
                ChangeResource(ResourceType.Wood, false, -changeValue);
                ChangeResource(ResourceType.Iron, false, -changeValue);
                ChangeResource(ResourceType.Spice, false, -changeValue);
                ChangeResource(ResourceType.Gem, false, -changeValue);
            }
        }

        public Order ReevaluateOrder(Order order){
            order.Reward = DetermineValueOfOrder(order);
            return order;
        }

        //Determine what a given order should consit of
        private Order DetermineWhatNewOrderShouldContain() {
            //Set up variables
            Order newOrder = new Order();
            int resourceNumber = 0;
            var resource = _resourceList[resourceNumber];
            var _ironFloat = this._ironFloat;
            var _woodFloat = this._woodFloat;
            var _spiceFloat = this._spiceFloat;
            var _gemFloat = this._gemFloat;

            //LAZY IMPLIMENTED
            /*
            Debug.Log(_ironFloat);
            Debug.Log(_woodFloat);
            Debug.Log(_spiceFloat);
            Debug.Log(_gemFloat);
            */
            foreach (var ord in _orderList) {
                if (ord.ResourceList.ContainsKey(ResourceType.Iron)) { //lazy
                    _ironFloat -= 0.1f;
                }
                if (ord.ResourceList.ContainsKey(ResourceType.Wood)) { //lazy
                    _woodFloat -= 0.1f;
                }
                if (ord.ResourceList.ContainsKey(ResourceType.Spice)) { //lazy
                    _spiceFloat -= 0.1f;
                }
                if (ord.ResourceList.ContainsKey(ResourceType.Gem)) { //lazy
                    _gemFloat -= 0.1f;
                }
            }
            _ironFloat -= (UnityEngine.Random.Range(0, 5) * 0.01f);
            _woodFloat -= (UnityEngine.Random.Range(0, 5) * 0.01f);
            _spiceFloat -= (UnityEngine.Random.Range(0, 5) * 0.01f);
            _gemFloat -= (UnityEngine.Random.Range(0, 5) * 0.01f);

            var a = new float[] { _ironFloat, _woodFloat, _spiceFloat, _gemFloat };
            int numberOfResource = 0;
            /*
            int numberOfExtraResources = 4;
            if (UnityEngine.Random.Range(0, 2) == 1){
                numberOfExtraResources--;
            }
            if (UnityEngine.Random.Range(0, 2) == 1){
                numberOfExtraResources--;
            }
            */
            int numberOfDifferentReasources = UnityEngine.Random.Range(0, 10);
            if(numberOfDifferentReasources < 2) { numberOfDifferentReasources = 2; }
            else if (numberOfDifferentReasources < 8) { numberOfDifferentReasources = 3; }
            else { numberOfDifferentReasources = 4; }
            /*
            while(numberOfDifferentReasources != 0){

                if (_ironFloat == _woodFloat && _woodFloat == _spiceFloat && _spiceFloat == _gemFloat){
                    resourceNumber = UnityEngine.Random.Range(0, 4);
                }
                else{
                    //lazy
                    var maxValue = a.Max();
                    resourceNumber = a.ToList().IndexOf(maxValue);
                    a[resourceNumber] -= 0.15f; //More unlikely for order to contain the same resources.
                }

                var numberOfResourcesTo = numberOfResourcesToAdd;
                resource = resourceList[resourceNumber];

                if (newOrder.resourceList.ContainsKey(resource)){
                    //Don't do anything
                }
                else{
                    numberOfDifferentReasources--;
                    if (resource == ResourceType.Spice) { numberOfResourcesTo *= 2; }
                    else if (resource == ResourceType.Iron
                        || resource == ResourceType.Wood) { numberOfResourcesTo *= 4; }
                    newOrder.resourceList.Add(resource, numberOfResourcesTo);
                    a[resourceNumber] -= 0.1f;
                }

            }
            */
            //Fill up order with resources. Currently between 2 and 4
            int numberOfResourcesToAdd = 5;
            //Debug.Log("numberOfDifferentReasources "+ numberOfDifferentReasources);
            //Debug.Log("numberOfResourcesToAdd " + numberOfResourcesToAdd);
            
            while ((numberOfDifferentReasources != 0) && numberOfResource < 5) {
                numberOfResourcesToAdd = 5;
                if (UnityEngine.Random.Range(0, 2) == 1){
                    numberOfResourcesToAdd--;
                }
                if (UnityEngine.Random.Range(0, 2) == 1){
                    numberOfResourcesToAdd--;
                }
                if (UnityEngine.Random.Range(0, 2) == 1){
                    numberOfResourcesToAdd--;
                }
                if (UnityEngine.Random.Range(0, 2) == 1){
                    numberOfResourcesToAdd--;
                }

                if (Mathf.Approximately(_ironFloat, _woodFloat) && 
                    Mathf.Approximately(_woodFloat, _spiceFloat) && 
                    Mathf.Approximately(_spiceFloat, _gemFloat)) {
                    resourceNumber = UnityEngine.Random.Range(0, 4);
                }
                else {
                    //lazy
                    var maxValue = a.Max();
                    resourceNumber = a.ToList().IndexOf(maxValue);
                    a[resourceNumber] -= 0.15f; //More unlikely for order to contain the same resources.
                }

                bool t = true;
                
                while (t){
                    resource = _resourceList[resourceNumber];
                    if (newOrder.ResourceList.ContainsKey(resource)){
                        resourceNumber++;
                        if(resourceNumber >= a.Length){
                            resourceNumber = 0;
                        }
                    }
                    else { t = false; }
                }

                var numberOfResourcesTo = numberOfResourcesToAdd;
                numberOfResourcesTo *= 3;


                if (newOrder.ResourceList.ContainsKey(resource)) {
                    var numb = newOrder.ResourceList[resource];
                    newOrder.ResourceList.Remove(resource);
                    if(resource== ResourceType.Spice) { numberOfResourcesTo *= 3; }
                    else if (resource == ResourceType.Iron 
                        || resource == ResourceType.Wood) { numberOfResourcesTo *= 7;}
                    newOrder.ResourceList.Add(resource, numb + numberOfResourcesTo);
                }
                else {
                    numberOfDifferentReasources--;

                    if (resource == ResourceType.Spice) { numberOfResourcesTo *= 3; }
                    else if (resource == ResourceType.Iron
                        || resource == ResourceType.Wood) { numberOfResourcesTo *= 7; }
                    newOrder.ResourceList.Add(resource, numberOfResourcesTo);
                }
                a[resourceNumber] -= 0.1f;
                numberOfResource++;
                //numberOfExtraResources--;
                //numberOfDifferentReasources--;
            }
            /*
            Debug.Log("-----------------------");
            foreach (var ord in orderList)
            {
                var s = ord.id + "";
                foreach(var re in ord.resourceList) { s += re.Key + " " + re.Value + " "; }
                Debug.Log(s);
            }
            Debug.Log("-----------------------");
            */
            return newOrder;
        }

        //Dertermine the reward of the order
        private int DetermineValueOfOrder(Order order) {
            int reward = 0;
            int numberOfResources = 0;
            //int numberOfResourcesToAdd_bonus = 10;
            foreach (var key in order.ResourceList.Keys) {
                var amountOfItem = order.ResourceList[key];

                var pricePerItem = GetPriceOfResource(key, false);
                var price = pricePerItem * amountOfItem;

                for (int j = 0; j < 1; j++) {
                    if (reward == 0) { reward = price; }
                    //else if (j <= 1) { reward = Convert.ToInt32(Math.Ceiling((getPriceOfResource(key, false)* numberOfResourcesToAdd_bonus + reward) * sameResourceBonus)); }
                    else if (numberOfResources == 1) { reward = Convert.ToInt32(Math.Ceiling((price + reward) * DifferentResourceBonus2)); }
                    else if (numberOfResources == 2) { reward = Convert.ToInt32(Math.Ceiling((price + reward) * DifferentResourceBonus3)); }
                    else if (numberOfResources == 3) { reward = Convert.ToInt32(Math.Ceiling((price + reward) * DifferentResourceBonus4)); }
                    numberOfResources++;
                }
            }

            return reward;
        }

        private void AddNewOrder() {
            //determine what the order should be
            var newOrder = DetermineWhatNewOrderShouldContain();

            //Handle id and list order
            newOrder.Id = _numberOfOrdersSoFar;
            _numberOfOrdersSoFar++;

            //Determine reward of order
            newOrder.Reward = DetermineValueOfOrder(newOrder);

            //Add order
            _orderList.Add(newOrder);

            //Sort LIST
            _orderList.Sort();
        }

        public int GetPriceOfResource(ResourceType resource, bool buy) {
            if (resource == ResourceType.Iron) {
                if (buy) { return BuyValueIron; }
                else { return SellValueIron; }
            } else if (resource == ResourceType.Wood) {
                if (buy) { return BuyValueWood; }
                else { return SellValueWood; }
            }
            else if (resource == ResourceType.Gem) {
                if (buy) { return BuyValueGem; }
                else { return SellValueGem; }
            }
            else {
                if (buy) { return BuyValueSpice; }
                else { return SellValueSpice; }
            }
        }

        //Returns count of orders in List
        public int GetOrderCount()
        {
            return _orderList.Count();
        }

        //Returns whole list
        public List<Order> GetAllOrders()
        {
            return _orderList;
        }

        //Returns whole list
        public List<Order> GetOrders()
        {
            List<Order> ordList = new List<Order>();
            for(int i = 0; i < _maxNumberOfOrders; i++){
                ordList.Add(_orderList[i]);
            }
            return ordList;
        }
        //Returns Order at Index
        public Order GetOrdersAtIndex(int _i)
        {
            return _orderList[_i];
        }


        //Returns a order that hasen't been given jet. Also creates a new order.
        public Order ReceiveOrder() {
            var order = new Order();
            foreach(var obj in _orderList){
                if (obj.Id == _orderIdGivenOut) { order = obj; break; }
            }

            _orderIdGivenOut++;

            AddNewOrder();

            return order;
        }

        //Returns a order based on the id
        public Order GetOrder(int id) {
            var order = new Order();
            foreach (var obj in _orderList){
                if(obj.Id == id) { order = obj; break; }
            }
            return order;
        }

        public void ReEavaluateAllOrders() {
            foreach (var order in _orderList){
                ReevaluateOrder(order);
            }
        }

        //Deletes a given order.
        public void HandInOrder(Order order) {
            foreach(var key in order.ResourceList.Keys) {
                //var amountOfItem = order.ResourceList[key];
                //for (int j = 0; j<amountOfItem; j++){
                //    //changeResource(key,false,0.02f+(UnityEngine.Random.Range(0, 6)*0.01f));
                //    ChangeResource(key, false, 0.01f);
                //    break; //Only executes the effect of the sold resource once
                //}
                ChangeResource(key, false, 0.01f); //Also only executes the effect of the sold resource once.
            }
            //Debug.Log("order count: " + orderList.Count);
            _orderList.Remove(order); //Delete order

            _maxNumberOfOrders--;
            if (_maxNumberOfOrders <= 0) { _maxNumberOfOrders++; }

            ReEavaluateAllOrders();
        }

        //Function to be called that determines the iron's value
        //Uses an linear function
        private int DetermineIronValue(float plusRatio) {
            return Convert.ToInt32(
                Math.Ceiling(10 * IronPickFloat *
                (_ironFloat + plusRatio)
                )
                );
        }

        //Function to be called that determines the wood's value
        //Uses an function similar to easeInOutQuad
        //Acceleration until halfway, then deceleration
        private int DetermineWoodValue(float plusRatio) {
            var newWoodFloat = _woodFloat + plusRatio;
            if (newWoodFloat < 0.5)
            {
                var result = Convert.ToInt32(
                    Math.Ceiling(
                        (2 * newWoodFloat * newWoodFloat) * WoodPickFloat * 10
                        * Mathf.Lerp(0.8f, 1.0f, (newWoodFloat * 2) / 1)
                        // NOTE: Wood needs to be worth less than iron, since it yields 50%
                        // more on average, so we just always cut 30% of the result, since
                        // the base value of the two is the same.
                        * 0.7f
                    )
                );
                return result;
            }
            else
            {
                // NOTE: Due to the easeInOutQuad going downwards when the value exceeds 1.0,
                // we set newWoodFloat to be 1.0f, and then add in 10% extra to the result,
                // interpolated over the difference between 1.0 and 1.0 + BuyValuePlusRatio
                // (because the buy-price is always higher). This make the base-price end at
                // 11, where the value of iron ends at 13, because iron-resources yield less.

                // NOTE: Wood needs to be worth less than iron, since it yields 50%
                // more on average, so instead of adjusting the values for when newWoodFloat
                // is above 1.0, we just always cut 30% of the result.

                var tempNewWoodFloat = newWoodFloat;
                if (newWoodFloat > 1)
                    newWoodFloat = 1.0f;
                var result = Convert.ToInt32(
                    Math.Ceiling(
                        (-1 + (4 - 2 * newWoodFloat) * newWoodFloat) * WoodPickFloat * 10
                        * (tempNewWoodFloat > 1 ? Mathf.Lerp(1.0f, 1.3f, tempNewWoodFloat / (1.0f + BuyValuePlusRatio)): 1)
                        * 0.7f
                    )
                );

                return result;
            }
        }

        //Function to be called that determines the gem's value
        //Uses an function similar to easeInSin
        private int DetermineGemValue(float plusRatio)
        {
            var newGemFloat = _gemFloat + plusRatio;
            var result = Convert.ToInt32(
                Math.Ceiling(
                    newGemFloat * newGemFloat * GemPickFloat * 10
                )
            );

            //Debug.Log("newSpiceFloat = " + newSpiceFloat + ")\nBonus = " + bonus + "\nResult = " + result);
            return result;
        }

        //Function to be called that determines the spice's value
        //Uses a function similar to easeInQuad
        //Accelerating from zero velocity
        private int DetermineSpiceValue(float ratioPlus)
        {
            var newSpiceFloat = _spiceFloat + ratioPlus;
            var result = Convert.ToInt32(
                Math.Ceiling(
                    (-1 * Mathf.Cos(newSpiceFloat / 1 * (Mathf.PI / 2)) + 1) * 10 * SpicePickFloat
                    //    (1 + (SpicePickFloat * Mathf.PI - Mathf.PI / 2)) / 4 * newSpiceFloat * 10
                    //+ bonus
                )
            );
            //Debug.Log(_spiceFloat);

            if (result == SellValueSpice && ratioPlus >= 0.1f) { result += 2; }

            return result;
        }

        ////Function to be called that determines the gem's value
        ////Uses an function similar to easeInSin
        //private int DetermineGemValue(float plusRatio) {
        //    //int bonus = 0;
        //    var newGemFloat = _gemFloat + plusRatio;

        //    //if (newGemFloat <= 0.19f) { bonus = 3; }
        //    //else if (newGemFloat <= 0.2f && plusRatio >= 0.1f) { bonus = 2; }
        //    //else if (newGemFloat <= 0.2f) { bonus = 4; }
        //    //else if (newGemFloat <= 0.39f) { bonus = 0; }
        //    //else if (newGemFloat <= 0.49f) { bonus = -2; }
        //    //else if (0.49f <= newGemFloat) { bonus = -3; }
        //    var result = Convert.ToInt32(
        //        Math.Ceiling(
        //            (-1 * Mathf.Cos(newGemFloat / 1 * (Mathf.PI / 2)) + 1) * 10 * GemPickFloat
        //        //    (1 + (GemPickFloat * Mathf.PI - Mathf.PI / 2)) / 4 * newGemFloat * 10
        //        //+ bonus
        //        )
        //    );
        //    //Debug.Log(_gemFloat);

        //    if (result == SellValueGem && plusRatio >= 0.1f) { result += 2; }

        //    return result;
        //}

        ////Function to be called that determines the spice's value
        ////Uses a function similar to easeInQuad
        ////Accelerating from zero velocity
        //private int DetermineSpiceValue(float ratioPlus) {
        //    //Debug.Log("------------------------------------------------------------\nDetermineSpiceValue(" + ratioPlus + ")");
        //    //int bonus = 0;
        //    var newSpiceFloat = _spiceFloat + ratioPlus;
        //    //if (Mathf.Approximately(newSpiceFloat, 0.7f)) { bonus = 2; }
        //    //else if (Mathf.Approximately(newSpiceFloat, 0.6f)) { bonus = 3; }
        //    //else if (Mathf.Approximately(newSpiceFloat, 0.5f)) { bonus = 4; }
        //    //else if (newSpiceFloat > 0.2f && _spiceFloat < 0.5f && ratioPlus >= 0.1f) { bonus = 3; }
        //    //else if (newSpiceFloat > 0.2f && _spiceFloat < 0.5f) { bonus = 2; }
        //    //else if (newSpiceFloat <= 0.2f && ratioPlus >= 0.1f) { bonus = 2; }
        //    //else if (newSpiceFloat <= 0.2f) { bonus = 1; }
        //    var result = Convert.ToInt32(
        //        Math.Ceiling(
        //            newSpiceFloat * newSpiceFloat * SpicePickFloat * 10
        //            //+ bonus
        //        )
        //    );

        //    //Debug.Log("newSpiceFloat = " + newSpiceFloat + ")\nBonus = " + bonus + "\nResult = " + result);
        //    return result;
        //}

        //Method to be called to change resource whenever someone buys or sells a resource. 
        public void ChangeResource(ResourceType resource, bool buy, float changeValue)
        {
            switch (resource)
            {
                case ResourceType.Iron:
                    if (buy){
                        _ironFloat += changeValue;
                        if (_ironFloat > 1.0f) { _ironFloat = 1.0f; }
                    }
                    else{
                        _ironFloat -= changeValue;
                        if (_ironFloat <= 0.1f) { _ironFloat = 0.1f; }
                    }
                    SellValueIron = DetermineIronValue(0.0f);
                    BuyValueIron = DetermineIronValue(BuyValuePlusRatio);
                    ResourceValues[ResourceType.Iron] = new int[] {SellValueIron, BuyValueIron };
                    break;
                case ResourceType.Wood:
                    if (buy){
                        _woodFloat += changeValue;
                        if (_woodFloat > 1.0f) { _woodFloat = 1.0f; }
                    }
                    else{
                        _woodFloat -= changeValue;
                        if (_woodFloat <= 0.1f) { _woodFloat = 0.1f; }
                    }
                    SellValueWood = DetermineWoodValue(0.0f);
                    BuyValueWood = DetermineWoodValue(BuyValuePlusRatio);
                    ResourceValues[ResourceType.Wood] = new int[] { SellValueWood, BuyValueWood };
                    break;
                case ResourceType.Gem:
                    if (buy){
                        _gemFloat += changeValue;
                        if (_gemFloat > 1.0f) { _gemFloat = 1.0f; }
                    }
                    else {
                        _gemFloat -= changeValue;
                        if (_gemFloat <= 0.1f) { _gemFloat = 0.1f; }
                    }
                    SellValueGem = DetermineGemValue(0.0f);
                    BuyValueGem = DetermineGemValue(BuyValuePlusRatio);
                    ResourceValues[ResourceType.Gem] = new int[] { SellValueGem, BuyValueGem };
                    break;
                case ResourceType.Spice:
                    if (buy){
                        _spiceFloat += changeValue;
                        if (_spiceFloat > 1.0f) { _spiceFloat = 1.0f; }
                    }
                    else {
                        _spiceFloat -= changeValue;
                        if(_spiceFloat <= 0.1f) { _spiceFloat = 0.1f; }
                    }
                    SellValueSpice = DetermineSpiceValue(0.0f);
                    BuyValueSpice = DetermineSpiceValue(BuyValuePlusRatio);
                    ResourceValues[ResourceType.Spice] = new int[] { SellValueSpice, BuyValueSpice };
                    break;
            }
        }

        //Each player will call this to receive an instance of the iron resource
        public IronResource GetIronResource(){
            return new IronResource(ResourceType.Iron, IronSprite);
        }

        //Each player will call this to receive an instance of the wood resource
        public WoodResource GetWoodResource(){
            return new WoodResource(ResourceType.Wood, WoodSprite);
        }

        //Each player will call this to receive an instance of the gem resource
        public GemResource GetGemResource(){
            return new GemResource(ResourceType.Gem, GemSprite);
        }

        //Each player will call this to receive an instance of the spice resource
        public SpiceResource GetSpiceResource(){
            return new SpiceResource(ResourceType.Spice, SpiceSprite);
        }

        // Update is called once per frame
        void Update (){
		
        }
    }

    public class Order : IComparable<Order>
    {
        public int Id { get; set; }
        public SortedList<ResourceType, int> ResourceList = new SortedList<ResourceType, int>();
        public int Reward { get; set; }

        public int CompareTo(Order other){
            return Id - other.Id;
        }
    }
}