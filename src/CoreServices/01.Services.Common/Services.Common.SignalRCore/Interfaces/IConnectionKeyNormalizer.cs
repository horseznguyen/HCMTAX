namespace Services.Common.SignalRCore.Interfaces
{
    public interface IConnectionKeyNormalizer
    {
        string NormalizeKey(string key);
    }
}