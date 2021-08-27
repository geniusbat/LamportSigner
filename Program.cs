using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace LamportSigner
{
    class Program
    {
        static void Main(string[] args)
        {
            byte[] bytes = Encoding.ASCII.GetBytes("ABC");
            List<byte[][]> pk;
            List<byte[][]> sk;
            Tuple<List<byte[][]>, List<byte[][]>> tp = SkPkGenerator.GenerateKeys();
            pk = tp.Item1; sk = tp.Item2;
            string msg = "Hola mundo";
            byte[][] signature = SkPkGenerator.Sign(msg, pk, sk);
            SkPkGenerator.Verify(msg, signature, pk);
        }
    }
}
