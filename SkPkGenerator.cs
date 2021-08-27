using System;
using System.Text;
using System.Security.Cryptography;
using System.Collections.Generic;
using System.Linq;

namespace LamportSigner
{
    class SkPkGenerator
    {
        static byte[] Hash(byte[] input)
        {
            SHA256 enc = SHA256.Create();
            return enc.ComputeHash(input);
        }
        public static Tuple<List<byte[][]>, List<byte[][]>> GenerateKeys()
        {
            Random rd = new Random();
            int blocks = 32;
            //Lists of size 2 (0,1) arrays of 256 arrays of bytes.
            List<byte[][]> pk = new List<byte[][]>();
            pk.Add(new byte[32][]);
            pk.Add(new byte[32][]);
            List<byte[][]> sk = new List<byte[][]>();
            sk.Add(new byte[32][]);
            sk.Add(new byte[32][]);
            for (int i = 0; i < blocks; i++)
            {
                //Pos 0
                byte[] bufferA = new byte[32];
                rd.NextBytes(bufferA);
                sk[0][i] = bufferA;
                pk[0][i] = Hash(bufferA);
                //Pos 1
                byte[] bufferB = new byte[32];
                rd.NextBytes(bufferB);
                sk[1][i] = bufferB;
                pk[1][i] = Hash(bufferB);
            }
            return new Tuple<List<byte[][]>, List<byte[][]>>(pk, sk);
        }
        public static byte[][] Sign(String message, List<byte[][]> pk, List<byte[][]> sk)
        {
            byte[] hashedMessage = Hash(Encoding.UTF8.GetBytes(message));
            byte[][] signature = new byte[32][];
            for (int i = 0; i < hashedMessage.Length; i++)
            {
                byte bt = hashedMessage[i];
                int firstBit = (1 << 0) & bt;
                signature[i] = sk[firstBit][i];
            }
            return signature;
            //Add signature to message, DEPRECATED
            string stringSignature = "";
            for (int i = 0; i < signature.Length; i++)
            {
                stringSignature += Encoding.UTF8.GetString(signature[i]);
            }
        }
        public static void Verify(string message, byte[][] signature, List<byte[][]> pk)
        {
            byte[] hashedMessage = Hash(Encoding.UTF8.GetBytes(message));
            for (int i = 0; i < hashedMessage.Length; i++)
            {
                byte bt = hashedMessage[i];
                int firstBit = (1 << 0) & bt;
                byte[] value = Hash(signature[i]);
                if (pk[firstBit][i].SequenceEqual(value))
                {
                }
                else
                {
                    Console.WriteLine("False");
                    return;
                }
            }
            Console.WriteLine("ALLLESSS GUUUUUT");
        }
        public static void PrintHashMessageBinary(byte[] hashedMessage)
        {
            for (int i = 0; i < hashedMessage.Length; i++)
            {
                byte bt = hashedMessage[i];
                Console.WriteLine(Convert.ToString(bt, 2).PadLeft(8, '0'));
            }
        }
        public static void CheckSkPk(List<byte[][]> sk, List<byte[][]> pk)
        {
            Console.WriteLine("First iteration");
            int count = 0;
            for (int i = 0; i < sk[0].Length; i++)
            {
                byte[] skBlock = Hash(sk[0][i]);
                byte[] pkBlockA = pk[0][i];
                byte[] pkBlockB = pk[1][i];
                if (!skBlock.SequenceEqual(pkBlockA))
                {
                    count++;
                    Console.WriteLine("Missed");
                    if (!pkBlockB.SequenceEqual(skBlock))
                    {
                        Console.WriteLine("Double missed");
                    }
                }
            }
            Console.WriteLine("First Second");
            for (int i = 0; i < sk[1].Length; i++)
            {
                byte[] skBlock = Hash(sk[1][i]);
                byte[] pkBlockA = pk[0][i];
                byte[] pkBlockB = pk[1][i];
                if (!skBlock.SequenceEqual(pkBlockB))
                {
                    count++;
                    Console.WriteLine("Missed");
                    if (!pkBlockA.SequenceEqual(skBlock))
                    {
                        Console.WriteLine("Double missed");
                    }
                }
            }
            Console.WriteLine("Fails: "+count);
        }
    }
}
