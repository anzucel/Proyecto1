﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Cifrado
{
    public interface ISdes
    {
        public byte[] Cifrar(byte[] texto, int llave);
        public byte[] Descifrar(byte[] texto, int llave);

        List<string> generadorLlaves(int p, int q);

        public   byte[] CifrarRSA(byte[] texto, int e, int n);
        public byte[] DescifrarRSA(byte[] texto, int d, int n);


        public string CifrarCesar(string txtInicial, int Key);
        public string DesifrarCesar(string txtInicial, int Key);

        
    }
}
