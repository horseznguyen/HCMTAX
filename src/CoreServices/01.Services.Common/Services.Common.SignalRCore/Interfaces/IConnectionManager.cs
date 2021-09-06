using Services.Common.SignalRCore.Models;
using System.Collections.Generic;

namespace Services.Common.SignalRCore.Interfaces
{
    public interface IConnectionManager
    {
        int Count();

        void AddConnection(string key, Connection connection);

        void RemoveConnection(string key, string connectionId);

        void UpdateConnection(string key, Connection connection);

        Connection GetConnection(string key, string connectionId);

        List<Connection> GetConnections(string key, bool includingOfflineConnections = false);

        bool IsOnline(string key, string connectionId);

        List<Connection> Online();

        List<Connection> OnlineExceptThis(string key);

        void CleanUpOrphanedConnection();
    }
}