using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Lotus.Net.Http;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using DotNetWheels.AutoDI;

namespace TestWeb
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMongoDb(option =>
            {
                option.ConnectionString = "mongodb://127.0.0.1:27017";
                option.DbName = "test";
            });

            //services.AddHttpClient("xxx").AddHttpClientHandler(handler =>
            //{
            //    handler.ClientCertificates.Add(new X509Certificate());
            //});

            //var x = new HttpX(null);
            //x.PostJsonAsync<Object>("xxx", "xxx", content =>
            //{
            //    content.Headers.Add("xxx", "xxx");
            //}).Wait();


            //services.AddHttpClient("快钱Client").AddHttpClientHandler(handler =>
            //{
            //    handler.ClientCertificates.Add("块钱的证书");
            //});

            //services.AddScoped<HttpClient>();
            //services.AddScoped<HttpX>();

            services.AddMvc();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc(router =>
            {
                router.MapRoute("default", "{controller}/{action}", new
                {
                    controller = "home",
                    action = "index"
                });
            });
        }
    }
}
