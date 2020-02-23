<Query Kind="Program">
  <NuGetReference>Microsoft.AspNetCore</NuGetReference>
  <Namespace>Microsoft.AspNetCore</Namespace>
  <Namespace>Microsoft.Extensions.Configuration</Namespace>
  <Namespace>Microsoft.Extensions.DependencyInjection.Extensions</Namespace>
  <Namespace>Microsoft.Extensions.FileProviders</Namespace>
  <Namespace>Microsoft.Extensions.FileProviders.Internal</Namespace>
  <Namespace>Microsoft.Extensions.FileProviders.Physical</Namespace>
  <Namespace>Microsoft.Extensions.FileSystemGlobbing</Namespace>
  <Namespace>Microsoft.Extensions.Hosting</Namespace>
  <Namespace>Microsoft.Extensions.Logging</Namespace>
  <Namespace>Microsoft.Extensions.ObjectPool</Namespace>
  <Namespace>Microsoft.Extensions.Options</Namespace>
  <Namespace>Microsoft.Extensions.Primitives</Namespace>
  <Namespace>System.IO.Pipelines</Namespace>
  <Namespace>Microsoft.Extensions.DependencyInjection</Namespace>
  <Namespace>Microsoft.AspNetCore.Builder</Namespace>
  <Namespace>Microsoft.AspNetCore.Http</Namespace>
  <Namespace>Microsoft.AspNetCore.Hosting</Namespace>
</Query>

void Main()
{
	WebHost
		.CreateDefaultBuilder()
		.UseStartup<EntryPoint>()
		.Build()
		.Run();
}

public class EntryPoint
{
	IConfiguration Configuration { get; }
	
	public EntryPoint(IConfiguration configuration)
	{
		Configuration  = configuration;
	}

	// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
	public void Configure(IApplicationBuilder app, Microsoft.AspNetCore.Hosting.IHostingEnvironment env)
	{
		if (env.IsDevelopment())
		{
			app.UseDeveloperExceptionPage();
		}

		app.Use(async (context, next) =>
		{
			if(context.Request.QueryString.HasValue && context.Request.QueryString.Value.Contains("hello"))
			{
				await context.Response.WriteAsync("hello");
			}
			else
			{
				await next();
			}
		});

		app.Run(async context =>
		{
			await context.Response.WriteAsync("Default response");
		});
	}
}