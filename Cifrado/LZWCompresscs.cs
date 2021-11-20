using System;
using System.Collections.Generic;
using System.Security;
using System.Text;

namespace Cifrado
{
    public static class LZWCompresscs
    {

        public static int comrpimidotamaño { get; set; }
        public static int tamañodescomp { get; set; }
        public static double Ratio => (double)comrpimidotamaño / tamañodescomp * 100.0;

        public static byte[] Compress(byte[] dataToCompress)
        {
            tamañodescomp = dataToCompress.Length;
            var dictionary = new Dictionary<List<byte>, int>(new ArrayComparer());
            for (int i = 0; i < 256; i++)
            {
                var e = new List<byte> { (byte)i };
                dictionary.Add(e, i);
            }
            var ventanac = new List<byte>();
            var intalist = new List<int>();
            foreach (byte b in dataToCompress)
            {
                var cadenav = new List<byte>(ventanac) { b };
                if (dictionary.ContainsKey(cadenav))
                {
                    ventanac.Clear();
                    ventanac.AddRange(cadenav);
                }
                else
                {
                    if (dictionary.ContainsKey(ventanac))
                        intalist.Add(dictionary[ventanac]);
                    else
                        throw new Exception("Error Encoding.");
                    comrpimidotamaño = intalist.Count;
                    dictionary.Add(cadenav, dictionary.Count);
                    ventanac.Clear();
                    ventanac.Add(b);
                }
            }
            if (ventanac.Count != 0)
            {
                intalist.Add(dictionary[ventanac]);
                comrpimidotamaño = intalist.Count;
            }
            return GetBytes(intalist.ToArray());
        }

        public static byte[] Decompress(this byte[] dataToDescompress)
        {
            var dataaint = Dimen(dataToDescompress);
            var intalist = new List<int>(dataaint);
            comrpimidotamaño = intalist.Count;
            var diccionario = new Dictionary<int, List<byte>>();

            for (int i = 0; i < 256; i++)
            {
                var e = new List<byte> { (byte)i };
                diccionario.Add(i, e);
            }

            var ventanac = diccionario[intalist[0]];
            intalist.RemoveAt(0);
            var listadescomp = new List<byte>(ventanac);

            foreach (int k in intalist)
            {
                var entradadebytes = new List<byte>();
                if (diccionario.ContainsKey(k))
                    entradadebytes.AddRange(diccionario[k]);
                else if (k == diccionario.Count)
                    entradadebytes.AddRange(Add(ventanac.ToArray(), new[] { ventanac.ToArray()[0] }));
                if (entradadebytes.Count > 0)
                {
                    listadescomp.AddRange(entradadebytes);
                    tamañodescomp = listadescomp.Count;
                    diccionario.Add(diccionario.Count, new List<byte>(Add(ventanac.ToArray(), new[] { entradadebytes.ToArray()[0] })));
                    ventanac = entradadebytes;
                }
            }

            return listadescomp.ToArray();
        }

        private static byte[] GetBytes(int[] value)
        {
            if (value == null)
                throw new Exception("GetBytes (int[]) object cannot be null.");
            var anumeros = new byte[value.Length * 4];
            Buffer.BlockCopy(value, 0, anumeros, 0, anumeros.Length);
            return anumeros;
        }

        private static byte[] Add(byte[] left, byte[] right)
        {
            byte[] agregarbytes = new byte[left.Length + right.Length];
            Buffer.BlockCopy(left, 0, agregarbytes, 0, left.Length);
            Buffer.BlockCopy(right, 0, agregarbytes, left.Length, right.Length);
            return agregarbytes;
        }

        private static int[] Dimen(byte[] blockc)
        {
            var largomientras = blockc.Length;
            var int32Count = largomientras / 4 + (largomientras % 4 == 0 ? 0 : 1);
            var arr = new int[int32Count];
            Buffer.BlockCopy(blockc, 0, arr, 0, largomientras);
            return arr;
        }

        
    }

}