using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Numerics;
using System.Reflection;

namespace Cifrado
{
    public class Sdes : ISdes
    {
        private string[] Permutaciones;
        private string llave, P10, k1, k2, IP, EP, P4;
        private string[,] sb0 = { { "01", "00", "11", "10" }, { "11", "10", "01", "00" }, { "00", "10", "01", "11" }, { "11", "01", "11", "10" } };
        private string[,] sb1 = { { "00", "01", "10", "11" }, { "10", "00", "01", "11" }, { "11", "00", "01", "00" }, { "10", "01", "00", "11" } };

        public Sdes()
        {
            //var Direccion = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var path = "../Permutations.txt";
            Permutaciones = File.ReadAllLines(path);
        }

        private string Permutar(string clave, int numPermutacion)
        {
            char[] array = clave.ToCharArray();
            string resu = "";
            string[] aux = Permutaciones[numPermutacion].Split(',');
            for (int i = 0; i < aux.Length; i++)
            {
                int pos = Convert.ToInt32(aux[i]);
                resu += array[pos - 1];
            }
            return resu;
        }

        private string CorrerIzq(string clave)
        {
            string resu = "";
            char aux = ' ';
            for (int i = 0; i < clave.Length; i++)
            {
                if (i == 0)
                {
                    aux = clave[0];
                }
                else
                {
                    resu += clave[i];
                }
            }
            return resu + aux;
        }

        private string xor(string a, string b)
        {
            string resu = "";
            for (int i = 0; i < a.Length; i++)
            {
                if (a[i] == b[i])
                {
                    resu += "0";
                }
                else
                {
                    resu += "1";
                }
            }
            return resu;
        }

        private void GenerarClaves(string llave)
        {
            P10 = Permutar(llave, 0);
            string LS1 = P10.Substring(0, 5);
            LS1 = CorrerIzq(LS1);
            string LS2 = P10.Substring(5, 5);
            LS2 = CorrerIzq(LS2);
            k1 = Permutar(LS1 + LS2, 1);
            string LS3 = CorrerIzq(CorrerIzq(LS1));
            string LS4 = CorrerIzq(CorrerIzq(LS2));
            k2 = Permutar(LS3 + LS4, 1);
        }

        private string Swapbox0(string clave)
        {
            string f, c, s0;
            char[] aux = clave.ToCharArray();
            f = aux[0].ToString() + aux[3].ToString();
            c = aux[1].ToString() + aux[2].ToString();
            return s0 = sb0[BinarioDecimal(Convert.ToInt32(f)), BinarioDecimal(Convert.ToInt32(c))];
        }

        private string Swapbox1(string clave)
        {
            string f, c, s1;
            char[] aux = clave.ToCharArray();
            f = aux[4].ToString() + aux[7].ToString();
            c = aux[5].ToString() + aux[6].ToString();
            return s1 = sb1[BinarioDecimal(Convert.ToInt32(f)), BinarioDecimal(Convert.ToInt32(c))];
        }

        public byte[] Cifrar(byte[] texto, int dllave)
        {
            byte[] res = new byte[texto.Length];
            string bits, auxIP1, auxIP2, comb, s0, s1, swap, sw1, sw2, pinv;
            llave = DecimalBinario(dllave, 10);
            GenerarClaves(llave); //k1, k2

            for (int i = 0; i < texto.Length; i++)
            {
                bits = DecimalBinario(texto[i], 8);
                IP = Permutar(bits, 4);
                auxIP1 = IP.Substring(0, 4); // se utiliza en XOR
                auxIP2 = IP.Substring(4, 4); // se utiliza nuevamente al hacer swap
                EP = Permutar(auxIP2, 3);
                //XOR EP y K1
                comb = xor(EP, k1);
                s0 = Swapbox0(comb);
                s1 = Swapbox1(comb);
                P4 = Permutar(s0 + s1, 2);
                //XOR P4 y auxIP1
                comb = xor(P4, auxIP1);
                //swap
                swap = auxIP2 + comb;
                sw1 = swap.Substring(0, 4); // se realiza el xor
                sw2 = swap.Substring(4, 4);
                EP = Permutar(sw2, 3);
                comb = xor(EP, k2);
                s0 = Swapbox0(comb);
                s1 = Swapbox1(comb);
                P4 = Permutar(s0 + s1, 2);
                comb = xor(P4, sw1);
                pinv = Permutar(comb + sw2, 5);

                //res = BitConverter.GetBytes(BinarioDecimal(Convert.ToInt32(pinv)));
                res[i] = Convert.ToByte((BinarioDecimal(Convert.ToInt32(pinv))));
            }
            return res;
        }

        public byte[] Descifrar(byte[] texto, int dllave)
        {
            byte[] res = new byte[texto.Length];
            string bits, auxIP1, auxIP2, comb, s0, s1, swap, sw1, sw2, pinv;
            llave = DecimalBinario(dllave, 10);
            GenerarClaves(llave); //k1, k2

            for (int i = 0; i < texto.Length; i++)
            {
                bits = DecimalBinario(texto[i], 8);
                IP = Permutar(bits, 4);
                auxIP1 = IP.Substring(0, 4); // se utiliza en XOR
                auxIP2 = IP.Substring(4, 4); // se utiliza nuevamente al hacer swap
                EP = Permutar(auxIP2, 3);
                //XOR EP y K1
                comb = xor(EP, k2);
                s0 = Swapbox0(comb);
                s1 = Swapbox1(comb);
                P4 = Permutar(s0 + s1, 2);
                //XOR P4 y auxIP1
                comb = xor(P4, auxIP1);
                //swap
                swap = auxIP2 + comb;
                sw1 = swap.Substring(0, 4); // se realiza el xor
                sw2 = swap.Substring(4, 4);
                EP = Permutar(sw2, 3);
                comb = xor(EP, k1);
                s0 = Swapbox0(comb);
                s1 = Swapbox1(comb);
                P4 = Permutar(s0 + s1, 2);
                comb = xor(P4, sw1);
                pinv = Permutar(comb + sw2, 5);

                //res = BitConverter.GetBytes(BinarioDecimal(Convert.ToInt32(pinv)));
                res[i] = Convert.ToByte((BinarioDecimal(Convert.ToInt32(pinv))));
            }
            return res;
        }

        //Binario → Decimal
        int BinarioDecimal(long binario)
        {
            int numero = 0;
            int digito = 0;
            const int DIVISOR = 10;

            for (long i = binario, j = 0; i > 0; i /= DIVISOR, j++)
            {
                digito = (int)i % DIVISOR;
                if (digito != 1 && digito != 0)
                {
                    return -1;
                }
                numero += digito * (int)Math.Pow(2, j);
            }

            return numero;
        }

        //Decimal → Binario
        string DecimalBinario(int numero, int longitud)
        {
            long binario = 0;

            const int DIVISOR = 2;
            long digito = 0;

            for (int i = numero % DIVISOR, j = 0; numero > 0; numero /= DIVISOR, i = numero % DIVISOR, j++)
            {
                digito = i % DIVISOR;
                binario += digito * (long)Math.Pow(10, j);
            }
            string Binario = binario.ToString();

            if (Binario.Length < longitud) //autorelleno de los 10 bits
            {
                int ceros = longitud - Binario.Length;
                for (int i = 0; i < ceros; i++)
                {
                    Binario = "0" + Binario;
                }
            }

            binario = Convert.ToInt64(Binario);
            return Binario;
        }

        public List<string> generadorLlaves()
        {
            {
                throw new NotImplementedException();
            }
        }

        public byte[] Cifrar(byte[] data, int e, int n)
        {
            {
                throw new NotImplementedException();
            }
        }

        public byte[] Descifrar(byte[] data, int d, int n)
        {
            {
                throw new NotImplementedException();
            }
        }

    }
}
