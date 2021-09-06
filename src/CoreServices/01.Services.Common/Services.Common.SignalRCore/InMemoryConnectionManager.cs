using Services.Common.SignalRCore.Interfaces;
using Services.Common.SignalRCore.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Services.Common.SignalRCore
{
    /// <summary>
    /// If the application shuts down, all of the information is lost,
    /// but it will be re-populated as the users re-establish their connections.
    /// In-memory storage does not work if your environment includes more than one web server
    /// because each server would have a separate collection of connections.
    /// </summary>
    public class InMemoryConnectionManager : ConnectionManagerBase, IConnectionManager
    {
        /// <summary>
        /// The dictionary uses a HashSet to store the connection Id.
        /// At any time a user could have move than one connection to the SignalR Core Application.
        /// For example, a user who is connected through multiple devices or more then
        /// one browser tab would have move than one connection id.
        /// </summary>
        private static readonly ConcurrentDictionary<string, HashSet<Connection>> _connections = new();

        public InMemoryConnectionManager(IConnectionKeyNormalizer keyNormalizer) : base(keyNormalizer)
        {
        }

        public virtual int Count() => _connections.Count;

        public virtual void AddConnection(string key, Connection connection)
        {
            string normalizeKey = NormalizeKey(key);

            if (!_connections.ContainsKey(normalizeKey))
            {
                _connections[normalizeKey] = new HashSet<Connection>();
            }

            _connections[normalizeKey].Add(connection);
        }

        public virtual List<Connection> GetConnections(string key, bool includingOfflineConnections = false)
        {
            HashSet<Connection> conn;

            try
            {
                conn = _connections[NormalizeKey(key)];
            }
            catch
            {
                conn = null;
            }

            return includingOfflineConnections == false ? conn.Where(x => x.ExitTime == null).ToList() : conn.ToList();
        }

        public virtual List<Connection> Online()
        {
            List<Connection> lstOfConnection = new();

            if (_connections.Any())
            {
                foreach (var key in _connections.Keys)
                {
                    var connectionsByKey = GetConnections(key.Substring(Options.KeyPrefix.Length + 1));

                    if (connectionsByKey != null && connectionsByKey.Any()) lstOfConnection.AddRange(connectionsByKey.Where(x => x.ExitTime == null));
                }
            }

            return lstOfConnection;
        }

        public virtual List<Connection> OnlineExceptThis(string exceptKey)
        {
            List<Connection> lstOfConnection = new();

            if (_connections.Any())
            {
                foreach (var key in _connections.Keys)
                {
                    if (key == exceptKey) continue;

                    var connectionsByKey = GetConnections(key.Substring(Options.KeyPrefix.Length + 1));

                    if (connectionsByKey != null && connectionsByKey.Any()) lstOfConnection.AddRange(connectionsByKey.Where(x => x.ExitTime == null));
                }
            }

            return lstOfConnection;
        }

        public virtual void RemoveConnection(string key, string connectionId)
        {
            string normalizeKey = NormalizeKey(key);

            HashSet<Connection> connections;

            if (!_connections.TryGetValue(normalizeKey, out connections)) return;

            lock (connections)
            {
                var exsitingConnection = connections.SingleOrDefault(x => x.ConnectionID == connectionId);

                if (exsitingConnection != null)
                {
                    exsitingConnection.ExitTime = DateTime.UtcNow;
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
                }
            }
        }

        public virtual Connection GetConnection(string key, string connectionId)
        {
            var listOfConnection = GetConnections(key);

            return listOfConnection?.SingleOrDefault(x => x.ConnectionID == connectionId);
        }

        public void CleanUpOrphanedConnection()
        {
        }
    }
}