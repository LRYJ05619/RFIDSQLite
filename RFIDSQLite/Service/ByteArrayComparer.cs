using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RFIDSQLite.Service
{
    //自定义比较器
    public class ByteArrayComparer : IEqualityComparer<byte[]>
    {
        public bool Equals(byte[] x, byte[] y)
        {
            if (x == null || y == null)
                return x == y;

            if (x.Length != y.Length)
                return false;

            for (int i = 0; i < x.Length; i++)
                if (x[i] != y[i])
                    return false;

            return true;
        }

        public int GetHashCode(byte[] obj)
        {
            if (obj == null)
                return 0;

            int hash = 17;

            foreach (var b in obj)
                hash = hash * 31 + b;

            return hash;
        }
    }
}
