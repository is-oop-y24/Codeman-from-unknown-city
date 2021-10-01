using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Shops.Actions;
using Shops.Entities;
using Shops.Exceptions;

namespace Shops.Tests
{
    public class ShopsTests
    {
        private List<Shop> _shopsList;
        private Shop _testShop;
        private const string NotExistedProductName = "chocolate";
        private const string TestProductName = "beer";
        private const uint TestProductCost = 60;

        [SetUp]
        public void Setup()
        {
            _testShop = new Shop("Perekrestok", "Street 4, 1");
            _testShop.AddProductsInAssortment(new List<(string, uint)> { (TestProductName, TestProductCost) });
            _shopsList = new List<Shop>
            {
                _testShop,
                new Shop("Krasnoe&Beloe", "Street 43, 53"),
                new Shop("Pyaterochka", "Street 23, 110")
            };
        }

        [Test]
        public void AddProduct_ProductAdded()
        {
            try
            {
                _testShop.GetProductCost(TestProductName);
            } catch (ShopException)
            {
                Assert.Fail("Product wasn't added");
            }
        }

        [Test]
        public void TakeDelivery_DeliveryTaken()
        {
            const uint breadCount = 10;
            const uint meatCount = 20;
            _testShop.AddProductsInAssortment(new List<(string, uint)>
            {
                ("bread", 25), 
                ("meat", 40)
            });
            _testShop.TakeProductsDelivery(new List<(string, uint)>
            {
                ("bread", breadCount),
                ("meat", meatCount)
            });
            Assert.True(breadCount == _testShop.GetProductCount("bread") 
                        || meatCount == _testShop.GetProductCount("meat"));
        }

        [Test]
        public void ChangeProductCost_ProductCostChanged()
        {
            const uint costAfter = 68;
            _testShop.ChangeCost(TestProductName, costAfter);
            Assert.True(TestProductCost != _testShop.GetProductCost(TestProductName));
        }

        [Test]
        public void SellProducts_QuantityOfGoodsAndMoneyIsDecreasing()
        {
            const int nProductsBefore = 100;
            const int moneyBefore = 2430;
            const int nPurchasedProducts = 40;
            const int nProductsAfter = nProductsBefore - nPurchasedProducts;
            const int moneyAfter = moneyBefore - nPurchasedProducts * (int)TestProductCost; 
            _testShop.TakeProductsDelivery(new List<(string, uint)> {(TestProductName, nProductsBefore)});
            var customer = new Customer("Vova", moneyBefore);
            Deal.Resolve(_testShop, ref customer, new List<(string, uint)> {(TestProductName, nPurchasedProducts)});
            Assert.True(nProductsAfter == _testShop.GetProductCount(TestProductName) || moneyAfter == customer.Money);
        }

        [Test]
        public void SellProducts_InsufficientFunds_ThrowException()
        {
            const int nProductsBefore = 100;
            const int moneyBefore = 100;
            const int nPurchasedProducts = 40;
            _testShop.TakeProductsDelivery(new List<(string, uint)> { (TestProductName, nProductsBefore) });
            var customer = new Customer("Vova", moneyBefore);
            Assert.Catch<PurchaseException>(() =>
                Deal.Resolve(_testShop, ref customer, new List<(string, uint)> {(TestProductName, nPurchasedProducts)})
            );
        }

        [Test]
        public void SellProducts_InsufficientAmount_ThrowException()
        {
            const int nProductsBefore = 100;
            const int moneyBefore = 100000;
            const int nPurchasedProducts = 120;
            _testShop.TakeProductsDelivery(new List<(string, uint)> { (TestProductName, nProductsBefore) });
            var customer = new Customer("Vova", moneyBefore);
            Assert.Catch<PurchaseException>(() =>
                Deal.Resolve(_testShop, ref customer, new List<(string, uint)> { (TestProductName, nPurchasedProducts) })
            );
        }

        [Test]
        public void SellProducts_NotFound_ThrowException()
        {
            const int moneyBefore = 100000;
            const int nPurchasedProducts = 120;
            var customer = new Customer("Vova", moneyBefore);
            Assert.Catch<ShopException>(() =>
                Deal.Resolve(_testShop, ref customer, new List<(string, uint)> { (NotExistedProductName, nPurchasedProducts) })
            );
        }

        [Test]
        public void FindShopWithSmallestProductCost_NotFound()
        {
            const string notExistedProductName = "chocolate";
            uint smallestCost = Int32.MaxValue;
            Shop shopWithSmallestCost = null;

            foreach (Shop shop in _shopsList)
            {
                try
                {
                    uint cost = shop.GetProductCost(notExistedProductName);
                    if (cost < smallestCost)
                    {
                        shopWithSmallestCost = shop;
                        smallestCost = cost;
                    }
                }
                catch (ShopException)
                {
                }
            }

            Assert.True(shopWithSmallestCost == null);
        }

        [Test]
        public void FindShopWithSmallestProductCost_FoundRightShow()
        {
            uint counter = 0;

            foreach (Shop shop in _shopsList.Where(shop => shop.Name != _testShop.Name))
            {
                shop.AddProductsInAssortment(new List<(string, uint)> {(TestProductName, TestProductCost - counter++)});
            }

            Shop rightShopWithSmallestCost = _shopsList[^1];
            uint smallestCost = Int32.MaxValue;
            Shop shopWithSmallestCost = null;

            foreach (Shop shop in _shopsList)
            {
                try
                {
                    uint cost = shop.GetProductCost(TestProductName);
                    if (cost >= smallestCost) continue;
                    shopWithSmallestCost = shop;
                    smallestCost = cost;
                }
                catch (ShopException)
                {
                }
            }

            Assert.True(shopWithSmallestCost == rightShopWithSmallestCost);
        }
    }
}