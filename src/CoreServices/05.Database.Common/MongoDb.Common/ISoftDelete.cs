using System;

namespace MongoDb.Common
{
    public interface ISoftDelete
    {
        bool IsDeleted { get; set; }
        DateTime DeletedOn { get; set; }
    }
}