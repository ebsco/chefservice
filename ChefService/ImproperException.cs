using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ChefService
{
    class ImproperException:Exception
    {
        public ImproperException(string data) : base(data) { }
    }
}
