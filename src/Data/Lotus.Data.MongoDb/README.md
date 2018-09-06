# MongoDb Client API
This api provides an easy way to use mongodb.


### Project Dependency

The dependent nuget packages:
* MongoDB.Bson -v2.7.0
* MongoDB.Driver -v2.7.0
* MongoDB.Driver.Core -2.7.0

### Preparing For Domain Objects

Your need to create an domain object named "Product" like this:

```csharp
using Lotus.Data.MongoDb;
public class Product : IdentityEntity
{
    public String ProductName { get; set; }
    public Decimal Price { get; set; }
    public DateTime CreateTime { get; set; }
}
```

### The CRUD Operations Calling Example

### Create Operation



