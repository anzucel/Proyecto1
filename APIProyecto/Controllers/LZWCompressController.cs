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
    [Route("api/lzwcompress")]
    [ApiController]
    public class LZWCompressController : CompressIController
    {
        [Route ("sendfile")]
        [HttpPost]
        public IActionResult compress([FromBody] IFormFile files/*, string name*/)
        {
            string path = Path.Combine("../", "Archivos");

            //IFormFile file = files[0];
            byte[] fileBytes = null;
            try
            {


                using (var ms = new MemoryStream())
                {
                    files.CopyTo(ms);
                    fileBytes = ms.ToArray();
                    fileBytes = LZWCompresscs.Compress(fileBytes);
                }

                double SrazonDeCompresion = Convert.ToDouble((Convert.ToDouble(fileBytes.Length) / Convert.ToDouble(files.Length)) * 100);
                double SfactorDeCompresion = Convert.ToDouble(100 / SrazonDeCompresion);
                double SporcentajeDeReduccion = Convert.ToDouble(100 - SrazonDeCompresion);
                CompressionData compress = new CompressionData(files.FileName, Path.Combine(path, files.FileName), SrazonDeCompresion, SfactorDeCompresion, SporcentajeDeReduccion);
                uploadedFiles.Add(compress);

            }
            catch (Exception e)
            {
                StatusCodeResult x = new StatusCodeResult(StatusCodes.Status500InternalServerError);
                return x;
            }
          //  string newFileName = name + ".lzw";
            var cd = new System.Net.Mime.ContentDisposition
            {
                //FileName = newFileName,
                Inline = true,
            };

            Response.Headers.Add("Content-Disposition", cd.ToString());

            // return File(fileBytes, "text/plain");
            return Ok();
        }
    }
}
