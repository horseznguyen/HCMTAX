using Services.Common.ObjUtils;
using System;

namespace Services.Common.ActionUtils.Models
{
    public class FuncModel<TInput, TOutput> : DisposableModel
    {
        public string Id { get; set; } = Guid.NewGuid().ToString("N");

        public Func<TInput, TOutput> Func { get; set; }

        public FuncModel(Func<TInput, TOutput> func)
        {
            Func = func;
        }
    }
}