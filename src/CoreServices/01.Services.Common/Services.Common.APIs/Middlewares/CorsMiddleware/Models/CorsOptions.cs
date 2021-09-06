﻿using Microsoft.AspNetCore.Cors.Infrastructure;
using System;
using System.Collections.Generic;

namespace Services.Common.APIs.Middlewares.CorsMiddleware.Models
{
    public class CorsOptions
    {
        public string PolicyName { get; set; }

        /// <summary>
        /// Default is "*". If contains "*" then allow all.
        /// </summary>
        public List<string> AllowOrigins { get; set; } = new List<string> { "*" };

        /// <summary>
        /// Check on runtime for origin allow or not, this one will override the setting in <see cref="AllowOrigins"/>.
        /// </summary>
        public Func<string, bool> IsOriginAllowed { get; set; }

        /// <summary>
        /// Default is "*". If contains "*" then allow all.
        /// </summary>
        public List<string> AllowHeaders { get; set; } = new List<string> { "*" };

        /// <summary>
        /// Default is "GET, POST, PUT", "DELETE", "OPTIONS". If contains "*"  then allow all.
        /// </summary>
        public List<string> AllowMethods { get; set; } = new List<string>
        {
            //HttpMethod.POST.AsString(EnumFormat.DisplayName),
            //HttpMethod.PUT.AsString(EnumFormat.DisplayName),
            //HttpMethod.DELETE.AsString(EnumFormat.DisplayName),
            //HttpMethod.OPTIONS.AsString(EnumFormat.DisplayName)
        };

        public bool IsAllowCredentials { get; set; } = true;

        /// <summary>
        /// Additional Config Builder for Policy if you want to add your customize after Elect add Config Policy Builder.
        /// </summary>
        public Action<CorsPolicyBuilder> ExtendPolicyBuilder { get; set; }

        /// <summary>
        /// Additional Config Options for Policy if you want to add your customize after EF add Config Policy Options.
        /// </summary>
        public Action<Microsoft.AspNetCore.Cors.Infrastructure.CorsOptions> ExtendPolicyOptions { get; set; }
    }
}