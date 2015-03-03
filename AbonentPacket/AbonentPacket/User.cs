using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AbonentPacket
{
    class User
    {
        public int ID;
        public string Name;
        public int DepartID;
        public override string ToString()
        {
            return this.Name;
        }
    }
}
