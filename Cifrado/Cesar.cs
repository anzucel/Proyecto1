using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading.Tasks;



namespace Cifrado
{
    public class Cesar : ISdes
    {
        public string CifrarCesar(string txtInicial, int Key)
        {
            int t;
            var Letras = txtInicial.Length;
            char[] ch = new char[Letras];
            var Resultante = string.Empty;
            for (int i = 0; i < Letras; i++)
            {
                t = (int)txtInicial[i];
                ch[i] = (char)(t + Key);
                Resultante = Resultante + ch[i];
            }
            return Resultante;
        }
        public string DesifrarCesar(string txtInicial, int Key)
        {
            int t;
            var Letras = txtInicial.Length;
            char[] ch = new char[Letras];
            var Resultante = string.Empty;
            for (int i = 0; i < Letras; i++)
            {
                t = (int)txtInicial[i];
                ch[i] = (char)(t - Key);
                Resultante = Resultante + ch[i];
            }
            return Resultante;
        }

        //Sobre cargoo.........................
        public byte[] Cifrar(byte[] texto, int llave)
        {
            throw new NotImplementedException();
        }

        public byte[] Cifrar(byte[] texto, int e, int n)
        {
            throw new NotImplementedException();
        }

        public byte[] Descifrar(byte[] texto, int llave)
        {
            throw new NotImplementedException();
        }

        public byte[] Descifrar(byte[] texto, int d, int n)
        {
            throw new NotImplementedException();
        }

        public List<string> generadorLlaves()
        {
            throw new NotImplementedException();
        }

        public List<string> generadorLlaves(int p, int q)
        {
            throw new NotImplementedException();
        }

        public byte[] CifrarRSA(byte[] texto, int e, int n)
        {
            throw new NotImplementedException();
        }

        public byte[] DescifrarRSA(byte[] texto, int d, int n)
        {
            throw new NotImplementedException();
        }
    }
    }
    



