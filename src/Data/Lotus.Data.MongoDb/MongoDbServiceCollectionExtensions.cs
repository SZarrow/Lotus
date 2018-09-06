using System;
using System.Collections.Generic;
using System.Text;
using Lotus.Data.MongoDb;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class MongoDbServiceCollectionExtensions
    {
        public static IServiceCollection AddMongoDb(this IServiceCollection services, Action<MongoDbOption> configAction = null)
        {
            services.AddScoped(provider =>
            {
                var option = new MongoDbOption();

                if (configAction != null)
                {
                    configAction(option);
                }

                return new MongoDbContext(option);
            });

            return services;
        }
    }
}
