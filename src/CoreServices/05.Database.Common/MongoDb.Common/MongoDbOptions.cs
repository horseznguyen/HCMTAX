namespace MongoDb.Common.Model
{
    public class MongoDbOptions
    {
        public string HostName { get; set; }
        public int Port { get; set; }
        public string DatabaseConnection { get; set; }
        public string DatabaseName { get; set; }
        public string Prefix { get; set; } = "dev";
    }
}