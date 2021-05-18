using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crypting
{
    interface ICryptable
    {
        byte[] Encrypt(byte[] bytes);

        byte[] Decrypt(byte[] bytes);
    }
}
