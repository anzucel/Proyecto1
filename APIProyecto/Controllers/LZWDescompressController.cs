using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cifrado;
using APIProyecto.Models;
using System.IO;

namespace APIProyecto.Controllers
{
    [Route("api/lzwdecompress")]
    [ApiController]
    public class LZWDescompressController : CompressIController
    {
        [HttpPost]
        public ActionResult<string> descompress([FromForm] List<IFormFile> files)
        {
            string path = Path.Combine("../..", "Archivos");

            byte[] fileBytes = null;
            try
            {
                IFormFile file = files[0];

                using (var ms = new MemoryStream())
                {
                    file.CopyTo(ms);
                    fileBytes = ms.ToArray();
                    fileBytes = LZWCompresscs.Decompress(fileBytes);
                }

                double SrazonDeCompresion = Convert.ToDouble((Convert.ToDouble(fileBytes.Length) / Convert.ToDouble(file.Length)) * 100);
                double SfactorDeCompresion = Convert.ToDouble(100 / SrazonDeCompresion);
                double SporcentajeDeReduccion = Convert.ToDouble(100 - SrazonDeCompresion);
                CompressionData compress = new CompressionData(file.FileName, Path.Combine(path, file.FileName), SrazonDeCompresion, SfactorDeCompresion, SporcentajeDeReduccion);
                uploadedFiles.Add(compress);
                string fileWihoutExtension = file.FileName.Substring(0, file.FileName.IndexOf('.'));
                string newFileName = fileWihoutExtension + ".txt";
                var cd = new System.Net.Mime.ContentDisposition
                {
                    FileName = newFileName,
                    Inline = true,
                };

                Response.Headers.Add("Content-Disposition", cd.ToString());

                return File(fileBytes, "text/plain");
            }
            catch (Exception e)
            {
                StatusCodeResult x = new StatusCodeResult(StatusCodes.Status500InternalServerError);
                return x;
            }
        }

    }
}
