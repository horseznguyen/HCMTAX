using Services.Common.ObjUtils;
using System;

namespace Services.Common.ActionUtils.Models
{
    public class ActionModel : DisposableModel
    {
        public string Id { get; set; } = Guid.NewGuid().ToString("N");

        public Action Action { get; set; }

        public ActionModel(Action action)
        {
            Action = action;
        }
    }
}