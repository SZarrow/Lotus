using System;

namespace Lotus.Scheduler.Web.Models
{
    public class ErrorViewModel
    {
        public String RequestId { get; set; }

        public Boolean ShowRequestId
        {
            get
            {
                return !String.IsNullOrEmpty(RequestId);
            }
        }
    }
}