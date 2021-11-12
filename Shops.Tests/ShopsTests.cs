using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Shops.Actions;
using Shops.Entities;
using Shops.Interfaces;
using Shops.Tools;

namespace Shops.Tests
{
    public class ShopsTests
    {
        private IProductsNames _productsNames;
        private List<IShop> _shopsList;
        private IShop _testShop;
        private const string TestShopName = "Perekrestok";
        private const float TestProductCost = 60;
        private Guid _testProductId;
        private Guid _notExistedProductId;

        [SetUp]
        public void Setup()
        {
            _productsNames = new ProductsNames();
            IShopFactory shopFactory = new ShopFactory(_productsNames);

            _notExistedProductId = Guid.NewGuid();
            _testProductId = _productsNames.Register("beer");
            _testShop = shopFactory
                .CreateShop(TestShopName, "Street 4, 1")
                .AddProductsInAssortment(new List<(Guid, float)>
                {
                    (_testProductId, TestProductCost)
                });
            _shopsList = new List<IShop>
            {

                _testShop,
                shopFactory.CreateShop("Krasnoe&Beloe", "Street 43, 53"),
                shopFactory.CreateShop("Pyaterochka", "Street 23, 110"),
            };
        }

        [Test]
        public void AddProduct_ProductAdded()
        {
            Guid breadId = _productsNames.Register("bread");
            const float breadCost = 20;
            try
            {
                _testShop = _testShop.AddProductsInAssortment(new List<(Guid, float)> {(breadId, breadCost)});
                Assert.True(_testShop.GetCost(breadId) == breadCost);
            }
            catch (ShopsException)
            {
                Assert.Fail("Product wasn't added");
            }
        }

        [Test]
        public void TakeDelivery_DeliveryTaken()
        {
            Guid breadId = _productsNames.Register("Bread");
            Guid meatId = _productsNames.Register("Meat");
            const uint breadCount = 10;
            const uint meatCount = 20;
            _testShop = _testShop.AddProductsInAssortment(new List<(Guid, float)> {(breadId, 25), (meatId, 40)});
            _testShop = _testShop.TakeDelivery(new List<(Guid, uint)> {(breadId, breadCount), (meatId, meatCount)});
            Assert.True(breadCount == _testShop.GetCount(breadId) && meatCount == _testShop.GetCount(meatId));
        }

        [Test]
        public void ChangeProductCost_ProductCostChanged()
        {
            const uint costAfter = 68;
            Assert.True(TestProductCost != _testShop.ChangeCost(_testProductId, 68).GetCost(_testProductId));
        }

        [Test]
        public void SellProducts_QuantityOfGoodsAndMoneyIsDecreasing()
        {
            const int nProductsBefore = 100;
            const float moneyBefore = 2430;
            const int nPurchasedProducts = 40;
            const int nProductsAfter = nProductsBefore - nPurchasedProducts;
            const float moneyAfter = moneyBefore - nPurchasedProducts * TestProductCost;
            _testShop = _testShop.TakeDelivery(new List<(Guid, uint)> {(_testProductId, nProductsBefore)});
            var customer = new Customer("Vova", moneyBefore);
            (_testShop, customer) = Deal.Resolve(_testShop, customer,
                new List<(Guid, uint)> {(_testProductId, nPurchasedProducts)});
            Assert.True(nProductsAfter == _testShop.GetCount(_testProductId) && moneyAfter == customer.Money);
        }

        [Test]
        public void SellProducts_InsufficientFunds_ThrowException()
        {
            _testShop = _testShop.TakeDelivery(new List<(Guid, uint)> {(_testProductId, 100)});
            var customer = new Customer("Vova", 100);
            Assert.Catch<ShopsException>(() =>
                Deal.Resolve(_testShop, customer, new List<(Guid, uint)> {(_testProductId, 40)})
            );
        }

        [Test]
        public void SellProducts_InsufficientAmount_ThrowException()
        {
            var customer = new Customer("Vova", float.MaxValue);
            Assert.Catch<ShopsException>(() =>
                Deal.Resolve(_testShop, customer, new List<(Guid, uint)> {(_testProductId, 1)})
            );
        }

        [Test]
        public void SellProducts_NotFound_ThrowException()
        {
            var customer = new Customer("Vova", 100000);
            Assert.Catch<ShopsException>(() =>
                Deal.Resolve(_testShop, customer, new List<(Guid, uint)> {(_notExistedProductId, 1)})
            );
        }
    }
}