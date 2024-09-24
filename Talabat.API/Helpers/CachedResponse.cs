using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.BLL.Interfaces;

namespace Talabat.API.Helpers
{
    public class CachedResponse : Attribute, IAsyncActionFilter
    {
        private readonly int _timeToLiveSeconds;

        public CachedResponse(int timeToLiveSeconds)
        {
            _timeToLiveSeconds = timeToLiveSeconds;
        }


        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var cacheService = context.HttpContext.RequestServices.GetRequiredService<IResponseCacheService>();

            var cacheKey = GenerateCacheKeyFromRequest(context.HttpContext.Request);

            var cachedResponse = await cacheService.GetCacheResponse(cacheKey);

            if (!string.IsNullOrEmpty(cachedResponse))
            {
                var contentResult = new ContentResult()
                {
                    Content = cachedResponse,
                    ContentType = "application/json",
                    StatusCode = 200
                };
                context.Result = contentResult;

                return;
            }

            var executedEndPointContext = await next();

            if (executedEndPointContext.Result is OkObjectResult okObjectResult)
                await cacheService.CacheResponseAsync(cacheKey, okObjectResult.Value, TimeSpan.FromSeconds(_timeToLiveSeconds));
        }

        private string GenerateCacheKeyFromRequest(HttpRequest request)
        {
            // baseUrl/api/products?pageIndex=1&pageSize&sort=name

            var keyBuilder = new StringBuilder();

            keyBuilder.Append($"{request.Path}");

            foreach(var (key , value) in request.Query.OrderBy(X => X.Key))
            {
                //pageIndex=1    -->   /api/products|pageIndex-1
                //pageSize =10   -->   /api/products|pageIndex-1|pageSize-10
                //sort=name      -->   /api/products|pageIndex-1|pageSize-10|sort-name

                keyBuilder.Append($"|{key}-{value}");
            }
            return keyBuilder.ToString();
        }
    }
}
