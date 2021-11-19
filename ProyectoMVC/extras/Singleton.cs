﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Proyecto1;
using Proyecto1.Extra;
using Proyecto1.Helper;
namespace Proyecto1.Extra
{
    public sealed class Singleton
    {
        private readonly static Singleton instance = new Singleton();
        
        //public List<> name_Original = new List<string>();
        public List<string> name_cipher = new List<string>();

        private Singleton()
        {
           
        }
      
        public static Singleton Instance
        {
            get
            {
                return instance;
            }
        }
    }
}
