using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using APIProyecto.Models;
namespace APIProyecto.Controllers
{
    [Route("api/compressions")]
    [ApiController]
    public class CompressIController : ControllerBase
    {
        public static List<CompressionData> uploadedFiles = new List<CompressionData>();

        [HttpGet]
        public ActionResult<List<CompressionData>> compressions()
        {
            return uploadedFiles;
        }
    }
}
