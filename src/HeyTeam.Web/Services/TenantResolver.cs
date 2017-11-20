using HeyTeam.Core;
using HeyTeam.Core.Queries;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using SaasKit.Multitenancy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HeyTeam.Web.Services {
	public class TenantResolver : ITenantResolver<Club> {
        private readonly string Tenant_List_Cache_Key = "Tenant_List_Cache_Key";
        private readonly IEnumerable<Club> clubs;

        public TenantResolver(IClubQuery query, IMemoryCache cache) {
            if (!cache.TryGetValue(Tenant_List_Cache_Key, out clubs)) {
                clubs = query.GetClubs();
                cache.Set(Tenant_List_Cache_Key, clubs, new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromHours(2)));
            }                
        }

        public Task<TenantContext<Club>> ResolveAsync(HttpContext context) {
            TenantContext<Club> tenantContext = null;
            var host = $"{context.Request.Scheme}://{context.Request.Host.Value.ToLower()}";
            var club = clubs.FirstOrDefault(t => t.Url.ToLowerInvariant().Equals(host));

            if (club != null)
                tenantContext = new TenantContext<Club>(club);

            return Task.FromResult(tenantContext);
        }
    }
}