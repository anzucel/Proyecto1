using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace APIProyecto.Models
{
    public class CompressionData
    {
        public string fileName { get; set; }
        public string compressedFile { get; set; }
        public double compresionRatio { get; set; }
        public double compresionFactor { get; set; }
        public double reduction { get; set; }

        public CompressionData(string fileName, string compressedFile, double compresionRatio,
            double compresionFactor, double reduction)
        {
            this.fileName = fileName;
            this.compressedFile = compressedFile;
            this.compresionRatio = compresionRatio;
            this.compresionFactor = compresionFactor;
            this.reduction = reduction;
        }
    }
}
