using System;
using System.Collections.Generic;
using System.Linq;
using Lotus.Data.MongoDb.Tests.Entities;
using MongoDB.Bson.Serialization;
using Xunit;

namespace Lotus.Data.MongoDb.Tests
{
    public class MongoDbContextTest
    {
        private MongoDbOption _option = new MongoDbOption()
        {
            ConnectionString = "mongodb://127.0.0.1:27017",
            DbName = "test"
        };

        [Fact]
        public void TestAddSingle()
        {
            using (var db = new MongoDbContext(_option))
            {
                db.Add(new Product()
                {
                    ProductName = "Surface",
                    Price = 12.5m,
                    CreateTime = DateTime.Now
                });

                var result = db.SaveChange();
                Assert.True(result.Success && result.Value == 1);
            }
        }

        [Fact]
        public void TestAddRange()
        {
            List<Product> products = new List<Product>();

            for (var i = 0; i < 10000; i++)
            {
                products.Add(new Product()
                {
                    CreateTime = DateTime.Now,
                    MongoId = i,
                    Price = i * 12.5m,
                    ProductName = "Surface" + i.ToString()
                });
            }

            using (var db = new MongoDbContext(_option))
            {
                db.RemoveRange(products);
                var result = db.SaveChange();

                db.AddRange(products);
                result = db.SaveChange();
                Assert.True(result.Success && result.Value == products.Count);
            }
        }

        [Fact]
        public void TestFind()
        {
            using (var db = new MongoDbContext(_option))
            {
                var products = db.Find<Product>(x => x.Price > 0).ToList();
                Assert.True(products != null && products.Count > 0);
            }
        }

        [Fact]
        public void TestDeleteSingle()
        {
            using (var db = new MongoDbContext(_option))
            {
                db.Add(new Product()
                {
                    MongoId = 5324941893933151371,
                    ProductName = "Surface",
                    Price = 12.5m,
                    CreateTime = DateTime.Now
                });

                var result = db.SaveChange();
                Assert.True(result.Success && result.Value == 1);

                var findProduct = db.Find<Product>(x => x.MongoId == 5324941893933151371).FirstOrDefault();
                Assert.NotNull(findProduct);

                db.Remove(findProduct);

                result = db.SaveChange();
                Assert.True(result.Success && result.Value == 1);
            }
        }

        [Fact]
        public void TestDeleteMany()
        {
            using (var db = new MongoDbContext(_option))
            {
                var findProducts = db.Find<Product>(x => x.MongoId >= 0 && x.MongoId < 20);
                Assert.True(findProducts != null && findProducts.Count() > 0);

                db.RemoveRange(findProducts);

                var result = db.SaveChange();
                Assert.True(result.Success && result.Value == findProducts.Count());
            }
        }

        [Fact]
        public void TestUpdateSingle()
        {
            using (var db = new MongoDbContext(_option))
            {
                db.Add(new Product()
                {
                    MongoId = 9001,
                    Price = 12.5m,
                    CreateTime = DateTime.Now,
                    ProductName = "Surface1713"
                });

                var result = db.SaveChange();

                var findProduct = db.Find<Product>(x => x.MongoId == 9001).FirstOrDefault();
                Assert.NotNull(findProduct);

                findProduct.ProductName = $"Updated-Surface-{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}";
                db.Update(findProduct);

                result = db.SaveChange();
                Assert.True(result.Success && result.Value == 1);
            }
        }

        [Fact]
        public void TestUpdateMany()
        {
            using (var db = new MongoDbContext(_option))
            {
                for (var i = 1; i <= 3; i++)
                {
                    db.Add(new Product()
                    {
                        MongoId = 1000 + i,
                        Price = 12.5m,
                        ProductName = "Surface100" + i.ToString(),
                        CreateTime = DateTime.Now
                    });
                }

                var result = db.SaveChange();

                var findProducts = db.Find<Product>(x => x.MongoId > 1000 && x.MongoId < 1004);
                Assert.True(findProducts != null && findProducts.Count() > 0);

                foreach (var product in findProducts)
                {
                    product.Price -= 10;
                    product.ProductName = "Updated-" + product.ProductName;
                }

                db.UpdateRange(findProducts);

                result = db.SaveChange();
                Assert.True(result.Success && result.Value == findProducts.Count());
            }
        }

        [Fact]
        public async void TestSaveChangeAsync()
        {
            List<Product> products = new List<Product>();

            for (var i = 0; i < 1000000; i++)
            {
                products.Add(new Product()
                {
                    CreateTime = DateTime.Now,
                    MongoId = i,
                    Price = i * 12.5m,
                    ProductName = "Surface" + i.ToString()
                });
            }

            using (var db = new MongoDbContext(_option))
            {
                db.RemoveRange(products);
                var result = db.SaveChange();

                db.AddRange(products);
                result = await db.SaveChangeAsync();
                Assert.True(result.Success && result.Value == products.Count);
            }
        }
    }
}
