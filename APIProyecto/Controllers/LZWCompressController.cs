using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cifrado;
using APIProyecto.Models;
using System.IO;
using System.Net.Http;
using Microsoft.AspNetCore.Hosting;
using MongoDB.Driver;

namespace APIProyecto.Controllers
{
    [Route("api/lzwcompress")]
    [ApiController]
    public class LZWCompressController : CompressIController
    {
        private IWebHostEnvironment Environment;
        public LZWCompressController(IWebHostEnvironment _environment)
        {
            Environment = _environment;
        }

        [Route ("sendfile")]
        [HttpPost]
        public IActionResult compress([FromBody] Messages message /*, string name*/)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest("Invalid model");
                }
                else
                {

                    byte[] fileBytes = LZWCompresscs.Compress(message.Texto); // comprime el archivo 

                    //byte[] fileBytesD = LZWCompresscs.Decompress(fileBytes); // comprime el archivo 

                    // almacena el archivo compreso en el servidor
                    if (!Directory.Exists(Environment.WebRootPath + "\\FilesLZW\\"))
                    {
                        Directory.CreateDirectory(Environment.WebRootPath + "\\FilesLZW\\");
                    }

                    System.IO.File.WriteAllBytes(Environment.WebRootPath + "\\FilesLZW\\" + message.FilePath + ".lzw", fileBytes);
                    //System.IO.File.WriteAllBytes(Environment.WebRootPath + "\\FilesLZW\\" + message.FilePath + ".txt", fileBytesD);

                    var client = new MongoClient("mongodb://127.0.0.1:27017");
                    var database = client.GetDatabase("ChatDB");
                    var dbmessages = database.GetCollection<Messages>("Messages");


                    Messages newMessage = new Messages();
                    newMessage.UsuarioEmisor = message.UsuarioEmisor;
                    newMessage.UsuarioReceptor = message.UsuarioReceptor;
                    newMessage.Fecha_envio = DateTime.Now.ToString("yy-MM-dd H:m:ss");
                    newMessage.FilePath = message.FilePath + ".lzw";
                    newMessage.Texto = fileBytes;

                    dbmessages.InsertOne(newMessage);

                    return Ok();
                }
            }
            catch (Exception e)
            {
                StatusCodeResult x = new StatusCodeResult(StatusCodes.Status500InternalServerError);
                return x;
            }
        }

        [HttpGet]
        [Route("downloadFile/{emisor}/{receptor}/{Filename}")]
        public Messages DownloadFile([FromRoute] string emisor, [FromRoute] string receptor, [FromRoute] string Filename)
        {
            Messages message = new Messages();
            byte[] fileBytesD = null;

            var client = new MongoClient("mongodb://127.0.0.1:27017");
            var database = client.GetDatabase("ChatDB");

            //bd mensajes
            var dbmessages = database.GetCollection<Messages>("Messages");
            var buscarMensaje = dbmessages.AsQueryable<Messages>(); //comentado
            var result = from a in buscarMensaje
                         where (((a.UsuarioEmisor == emisor && a.UsuarioReceptor == receptor) ||
                                (a.UsuarioEmisor == receptor && a.UsuarioReceptor == emisor)) &&
                                (a.FilePath == Filename + ".lzw"))
                         select a;

            foreach (Messages mess in result)
            {
                message.UsuarioEmisor = mess.UsuarioEmisor;
                message.UsuarioReceptor = mess.UsuarioReceptor;
                fileBytesD = LZWCompresscs.Decompress(mess.Texto); // guarda los bytes del archivo decompreso
            }

            // Guardar archivo 
            if (!Directory.Exists(Environment.WebRootPath + "\\Files\\"))
            {
                Directory.CreateDirectory(Environment.WebRootPath + "\\Files\\");
            }

            string[] splt = message.FilePath.Split('.');
            string type = "." + splt[1];

            message.FilePath = splt[0] + type;

            System.IO.File.WriteAllBytes(Environment.WebRootPath + "\\Files\\" + splt[0] + type, fileBytesD);

            return message;
        }
    }
}
