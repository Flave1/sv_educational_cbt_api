using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBT.Contracts.Common
{
    public class SingleDelete
    {
        public string Item { get; set; }
    }

    public class MultipleDelete
    {
        public string[] Items { get; set; }
    }
}
