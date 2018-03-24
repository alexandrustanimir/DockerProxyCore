using System;
using System.Security.Cryptography.X509Certificates;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DockerProxyCore
{
	public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
			var col = new X509Certificate2Collection();
			col.Import(Configuration["CertificateName"] , Configuration["CertificatePassword"], X509KeyStorageFlags.DefaultKeySet);

			using (var userIntermediateStore = new X509Store(StoreName.My, StoreLocation.CurrentUser, OpenFlags.ReadWrite))
			{
				userIntermediateStore.AddRange(col);
			}
			services.AddMvc();
			services.Add(new ServiceDescriptor(typeof(AuthorizedRestClient), typeof(AuthorizedRestClient), ServiceLifetime.Singleton));
		}

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();
        }
    }
}
