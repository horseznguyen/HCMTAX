using Services.Common.ObjUtils;
using System;

namespace Services.Common.ActionUtils.Models
{
    public class ActionModel<T> : DisposableModel
    {
        public string Id { get; set; } = Guid.NewGuid().ToString("N");

        public Action<T> Action { get; set; }

        public ActionModel(Action<T> action)
        {
            Action = action;
        }
    }
}