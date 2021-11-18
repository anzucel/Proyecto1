using System;
using System.Collections.Generic;
using System.Text;

namespace Cifrado
{
    public interface ISdes
    {
        public byte[] Cifrar(byte[] texto, int llave);
        public byte[] Descifrar(byte[] texto, int llave);

        List<string> generadorLlaves();

        public   byte[] Cifrar(byte[] texto, int e, int n);

         public  byte[] Descifrar(byte[] texto, int d, int n);
    }
}
