# MongoDb Client API
This api provides an easy way to operate mongodb, suppports CRUD operations.

## Project Dependency

The dependent nuget packages:
* MongoDB.Bson -v2.7.0
* MongoDB.Driver -v2.7.0
* MongoDB.Driver.Core -2.7.0

## Preparing For Domain Objects

Your need to create a class named "Product" like this:

```csharp
using Lotus.Data.MongoDb;
public class Product : IdentityEntity
{
    public String ProductName { get; set; }
    public Decimal Price { get; set; }
    public DateTime CreateTime { get; set; }
}
```

Notice: The class must inherit class "IdentityEntity".

## The CRUD Operations Calling Example

You need to configure the connection parameters before CRUD operation.

```csharp
private MongoDbOption _option = new MongoDbOption()
{
    ConnectionString = "mongodb://127.0.0.1:27017",
    DbName = "test"
};
```

### Create Operation

```csharp
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
```

```csharp
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
```

