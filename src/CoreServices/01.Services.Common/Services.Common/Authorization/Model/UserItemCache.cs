using System.Collections.Generic;
using Services.Common.Caching;

namespace Services.Common.Authorization.Model
{
    [CacheName("UserItemCache")]
    public class UserItemCache
    {
        public int UserId { get; set; }
        public bool IsPermissionChanged { get; set; }
        public string ListOfPermission { get; set; }
        public List<string> ListOfSessionCodeInValid { get; set; }
    }
}