using Caching.StackExchangeRedis;
using Newtonsoft.Json;
using Services.Common.SignalRCore.Interfaces;
using Services.Common.SignalRCore.Models;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Services.Common.SignalRCore
{
    /// <summary>
    /// External Storage
    /// </summary>
    public class RedisCacheConnectionManager : ConnectionManagerBase, IConnectionManager
    {
        private readonly IDatabase _database;

        private readonly ConnectionMultiplexer _redis;

        private HashSet<Connection> _connections = new();

        public RedisCacheConnectionManager(IConnectionKeyNormalizer keyNormalizer) : base(keyNormalizer)
        {
            _redis = RedisConnectionFactory.GetConnection();

            _database = _redis.GetDatabase();
        }

        public virtual int Count()
        {
            var server = GetServer();

            var data = server.Keys();

            var key = data?.Select(k => k.ToString());

            var inValidKeys = key.Where(x => x.StartsWith(Options.KeyPrefix));

            return inValidKeys.Count();
        }

        public virtual List<Connection> Online()
        {
            List<Connection> lstOfConnection = new();

            var server = GetServer();

            var data = server.Keys();

            var key = data?.Select(k => k.ToString());

            var validKeys = key.Where(x => x.StartsWith(Options.KeyPrefix));

            if (validKeys.Any())
            {
                foreach (var inValidKey in validKeys)
                {
                    var connectionsByKey = GetConnections(inValidKey.Substring(Options.KeyPrefix.Length + 1));

                    if (connectionsByKey != null && connectionsByKey.Any()) lstOfConnection.AddRange(connectionsByKey.Where(x => x.ExitTime == null));
                }
            }

            return lstOfConnection;
        }

        public virtual void AddConnection(string key, Connection connection)
        {
            var data = _database.StringGet(NormalizeKey(key));

            if (string.IsNullOrEmpty(data))
            {
                _connections.Add(connection);

                _database.StringSet(NormalizeKey(key), JsonConvert.SerializeObject(_connections));

                return;
            }

            _connections = JsonConvert.DeserializeObject<HashSet<Connection>>(data);

            if (_connections.All(x => x.ConnectionID != connection.ConnectionID) || !_connections.Any())
            {
                _connections.Add(connection);

                _database.StringSet(NormalizeKey(key), JsonConvert.SerializeObject(_connections));
            }
        }

        public virtual List<Connection> GetConnections(string key, bool includingOfflineConnections = false)
        {
            var data = _database.StringGet(NormalizeKey(key));

            if (string.IsNullOrEmpty(data)) return default;

            var lstConnection = JsonConvert.DeserializeObject<List<Connection>>(data);

            return includingOfflineConnections == false ? lstConnection.Where(x => x.ExitTime == null).ToList() : lstConnection;
        }

        public virtual List<Connection> OnlineExceptThis(string exceptKey)
        {
            List<Connection> lstOfConnection = new();

            var server = GetServer();

            var data = server.Keys();

            var key = data?.Select(k => k.ToString());

            var validKeys = key.Where(x => x.StartsWith(Options.KeyPrefix) && x != NormalizeKey(exceptKey));

            if (validKeys.Any())
            {
                foreach (var inValidKey in validKeys)
                {
                    var connectionsByKey = GetConnections(inValidKey.Substring(Options.KeyPrefix.Length + 1));

                    if (connectionsByKey != null && connectionsByKey.Any()) lstOfConnection.AddRange(connectionsByKey.Where(x => x.ExitTime == null));
                }
            }

            return lstOfConnection;
        }

        public virtual void RemoveConnection(string key, string connectionId)
        {
            var listOfExistingConnection = GetConnections(key);

            if (listOfExistingConnection != null && listOfExistingConnection.Any())
            {
                var existingConnection = listOfExistingConnection.SingleOrDefault(x => x.ConnectionID == connectionId);

                if (existingConnection != null)
                {
                    existingConnection.ExitTime = DateTime.UtcNow;

                    _database.StringSet(NormalizeKey(key), JsonConvert.SerializeObject(listOfExistingConnection));
                }
            }
        }

        public virtual bool IsOnline(string key, string connectionId)
        {
            var listOfExistingConnection = GetConnections(key);

            if (listOfExistingConnection != null && listOfExistingConnection.Any())
            {
                Connection existingConnection = listOfExistingConnection.SingleOrDefault(x => x.ConnectionID == connectionId);

                return existingConnection?.ExitTime != null;
            }

            return false;
        }

        public virtual void UpdateConnection(string key, Connection connection)
        {
            var listOfExistingConnection = GetConnections(key);

            if (listOfExistingConnection != null && listOfExistingConnection.Any())
            {
                var existingConnection = listOfExistingConnection.SingleOrDefault(x => x.ConnectionID == connection.ConnectionID);

                if (existingConnection != null)
                {
                    existingConnection.ExitTime = connection.ExitTime;

                    existingConnection.LatestPingTime = connection.LatestPingTime;

                    existingConnection.UserAgent = connection.UserAgent;

                    _database.StringSet(NormalizeKey(key), JsonConvert.SerializeObject(listOfExistingConnection));
                }
            }
        }

        public virtual Connection GetConnection(string key, string connectionId)
        {
            var listOfConnection = GetConnections(key);

            return listOfConnection?.SingleOrDefault(x => x.ConnectionID == connectionId);
        }

        private IServer GetServer()
        {
            var endpoint = _redis.GetEndPoints();

            return _redis.GetServer(endpoint.First());
        }

        public virtual void CleanUpOrphanedConnection()
        {
            List<Connection> lstOfConnection = new();

            var server = GetServer();

            var data = server.Keys();

            var key = data?.Select(k => k.ToString());

            var validKeys = key.Where(x => x.StartsWith(Options.KeyPrefix));

            if (validKeys.Any())
            {
                foreach (var validKey in validKeys)
                {
                    List<Connection> connectionsByKey = GetConnections(validKey.Substring(Options.KeyPrefix.Length + 1));

                    if (connectionsByKey == null || !connectionsByKey.Any()) continue;

                    DateTime invalidLatestPingTime = DateTime.UtcNow.Subtract(new TimeSpan(0, Options.InValidLatestPingTime, 0));

                    DateTime invalidExitTime = DateTime.UtcNow.AddDays(Options.ValidTime);

                    connectionsByKey.RemoveAll(x => (x.LatestPingTime == null && x.ExitTime == null) ||
                                                    (x.ExitTime != null && x.ExitTime > invalidExitTime) ||
                                                    (x.LatestPingTime != null && x.LatestPingTime < invalidLatestPingTime && x.ExitTime == null));

                    if (connectionsByKey == null || !connectionsByKey.Any())
                    {
                        _database.KeyDelete(validKey);

                        continue;
                    }

                    _database.StringSet(validKey, JsonConvert.SerializeObject(connectionsByKey.ToHashSet()));
                }
            }
        }
    }
}