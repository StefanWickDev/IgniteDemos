using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrgTracker
{
    public class RoleDistribution
    {
        public RoleDistribution(string title, double count)
        {
            Title = title;
            Count = count;
        }
        public string Title { get; set; }
        public double Count { get; set; }
    }
}
