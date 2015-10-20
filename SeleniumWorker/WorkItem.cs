using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeleniumWorker
{
    public class WorkItem
    {
        public int WorkTypeCode { get; set; }
        public double WorkHours { get; set; }
        public long CostUnitCode { get; set; }
        public long CostCenterCode { get; set; }
    }
}
