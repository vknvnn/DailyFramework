using System;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Http;

namespace Df.TenantBase
{
    public struct JwtCustomizeClaimNames
    {
        public const string Tid = "tid";
        public const string Ofs = "ofs";
    }

    public interface ITenantFactory
    {        
        Guid GetTenantId();
        int GetClientOffset();
        string GetUserName();
        DateTimeOffset GetCurrentClientTime();
    }
    public class TenantFactory : ITenantFactory
    {
        private readonly Guid _tenantId;
        private readonly int _clientTime;
        private readonly string _userName;
        public TenantFactory(IHttpContextAccessor httpContextAccessor, Guid? tenantId = null, int? clientTime = null, string userName = null)
        {
            if (httpContextAccessor == null)
            {
                _tenantId = tenantId.GetValueOrDefault();
                _clientTime = clientTime.GetValueOrDefault();
                _userName = userName;
                return;
            }
            var httpContext = httpContextAccessor.HttpContext;
            if (httpContext != null)
            {
                var claim = httpContext.User.FindFirst(JwtCustomizeClaimNames.Tid);
                if (claim != null)
                {
                    _tenantId = Guid.Parse(claim.Value);
                }
                claim = httpContext.User.FindFirst(JwtCustomizeClaimNames.Ofs);
                if (claim != null)
                {
                    int.TryParse(claim.Value, out _clientTime);
                }
                claim = httpContext.User.FindFirst(JwtRegisteredClaimNames.Sub);
                if (claim != null)
                {
                    _userName = claim.Value;
                }
            }
        }
        
        public int GetClientOffset()
        {
            return _clientTime;
        }
        public DateTimeOffset GetCurrentClientTime()
        {
            //return DateTimeOffset.Now;
            return new DateTimeOffset(DateTimeOffset.Now.DateTime, TimeSpan.FromMinutes(_clientTime));
        }
        public Guid GetTenantId()
        {
            return _tenantId;
        }

        public string GetUserName()
        {
            return _userName;
        }
    }
}
