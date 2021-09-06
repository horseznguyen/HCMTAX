namespace Dapper.Common
{
    public class DapperOptions
    {
        public string ConnectionStrings { get; set; }
        public int CommandTimeOut { get; set; } = 300;
    }
}