using Audit.Core;
using Audit.Redis.Providers;
using Polly;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Audit.Common
{
    public class PollyRedisDataProvider : RedisDataProvider
    {
        private readonly Policy<object> _policy;

        private readonly AsyncPolicy<object> _asyncPolicy;

        public PollyRedisDataProvider(RedisProviderHandler handler, Policy<object> policy, AsyncPolicy<object> asyncPolicy) : base(handler)
        {
            _policy = policy;
            _asyncPolicy = asyncPolicy;
        }

        public override object InsertEvent(AuditEvent auditEvent)
        {
            var contextData = new Dictionary<string, object>() { { "event", auditEvent } };

            return _policy.Execute(ctx => base.InsertEvent(auditEvent), contextData);
        }

        public async override Task<object> InsertEventAsync(AuditEvent auditEvent)
        {
            var contextData = new Dictionary<string, object>() { { "event", auditEvent } };

            return await _asyncPolicy.ExecuteAsync(async ctx => await base.InsertEventAsync(auditEvent), contextData);
        }

        public override void ReplaceEvent(object eventId, AuditEvent auditEvent)
        {
            var contextData = new Dictionary<string, object>() { { "id", eventId }, { "event", auditEvent } };

            _policy.Execute(ctx => { base.ReplaceEvent(eventId, auditEvent); return null; }, contextData);
        }

        public async override Task ReplaceEventAsync(object eventId, AuditEvent auditEvent)
        {
            var contextData = new Dictionary<string, object>() { { "id", eventId }, { "event", auditEvent } };

            await _asyncPolicy.ExecuteAsync(async (ctx) => { return base.ReplaceEventAsync(eventId, auditEvent); return null; }, contextData);
        }
    }
}