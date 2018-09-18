using System;
using System.Collections.Generic;
using System.Text;
using DotNetWheels.Core;
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

        public static IServiceCollection AddMongoDb<T>(this IServiceCollection services, Action<MongoDbOption> configAction = null) where T : MongoDbContext
        {
            services.AddScoped(provider =>
            {
                var option = new MongoDbOption();

                if (configAction != null)
                {
                    configAction(option);
                }

                var ctorInfo = typeof(T).GetConstructor(new Type[] { typeof(MongoDbOption) });
                if (ctorInfo == null)
                {
                    throw new NullReferenceException($"unable to reflect the constructor info of  type '{typeof(T).FullName}'");
                }

                return ctorInfo.XConstruct(new Object[] { option }) as T;
            });

            return services;
        }
    }
}
