using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Library_SDES;
using System.IO;
using System.Net;

namespace Lab5ED2S.Controllers
{
    [Route("")]
    [ApiController]
    public class CifradoController : ControllerBase
    {
        public readonly IHostingEnvironment fistenviroment;
        public CifradoController(IHostingEnvironment enviroment)
        {
            this.fistenviroment = enviroment;
        }
        SDES CifradoSDES = new SDES();
        LRSA CifradoRSA = new LRSA();
        public static string nombreoriginal = "";

        [Route("api/sdes/cipher/{nombre}")]
        [HttpPost]
        public IActionResult SDES([FromForm] IFormFile file, [FromForm] int key, string nombre)
        {
            try
            {
                string filepath = Archivo(file, 1, nombre);
                string filepathNuevo = Archivo(file, 2, nombre);
                string uploadsFolder = Path.Combine(fistenviroment.ContentRootPath, "Helpers");
                uploadsFolder = Path.Combine(uploadsFolder, "Permutations.txt");
                CifradoSDES.Read_File(filepath, filepathNuevo, uploadsFolder, key, 0);
                return Ok("Se Cifro Correctamente!!!!!. El archivo cifrado se guardo en la carpeta UploadCifrado del proyecto");
            }
            catch (Exception e)
            {
                return StatusCode(500);
                throw;
            }
        }
        [Route("api/sdes/decipher")]
        [HttpPost]
        public IActionResult SDESDecipher([FromForm] IFormFile file, [FromForm] int key)
        {
            try
            {
                string filepath = Archivo(file, 4, nombreoriginal);
                string filepathNuevo = Archivo(file, 3, nombreoriginal);
                string uploadsFolder = Path.Combine(fistenviroment.ContentRootPath, "Helpers");
                uploadsFolder = Path.Combine(uploadsFolder, "Permutations.txt");
                CifradoSDES.Read_File(filepath,filepathNuevo, uploadsFolder, key, 1);
                return Ok("Se Decifro Correctamente!!!!!. El archivo decifrado se guardo en la carpeta Uploaddecifrado del proyecto");
            }
            catch (Exception e)
            {
                return StatusCode(500);
                throw;
            }
        }
            public string Archivo(IFormFile file, int num, string nombre)
        {
            string uploadsFolder = null;
            object aCifrar = default;
            string ccc = default;
            string filepath = "";
            if (file != null)
            {
                if (num == 1)//se crea el primer directorio
                {
                    uploadsFolder = Path.Combine(fistenviroment.ContentRootPath, "Helpers");
                }
                else if (num == 2)// el directorio donde se va a escribir
                {
                    uploadsFolder = Path.Combine(fistenviroment.ContentRootPath, "UploadCifrado");
                }
                else if (num == 3)// donde se va a decifrar
                {
                    uploadsFolder = Path.Combine(fistenviroment.ContentRootPath, "UploadDecifrado");
                }
                else if (num == 4)
                {
                    uploadsFolder = Path.Combine(fistenviroment.ContentRootPath, "UploadCifrado");

                }
                filepath = Path.Combine(uploadsFolder, file.FileName);
                if (num == 1)
                {
                    string nom = Convert.ToString(file.FileName).Replace(".txt", string.Empty);
                    nombreoriginal = nom;
                    if (!System.IO.File.Exists(filepath))
                    {
                        using (var INeadLearn = new FileStream(filepath, FileMode.CreateNew))
                        {
                            file.CopyTo(INeadLearn);
                        }
                    }
                }
                else if (num == 2)
                {
                    string direccionNuevo = Path.Combine(uploadsFolder, nombre + ".txt");
                    System.IO.File.WriteAllLines(direccionNuevo, new string[0]);
                    filepath = direccionNuevo;


                }
                else if (num == 3)
                {
                    string nom = Convert.ToString(file.FileName).Replace(".txt", string.Empty);
                    string direccionNuevo = Path.Combine(uploadsFolder, nom+ ".txt");
                    System.IO.File.WriteAllLines(direccionNuevo, new string[0]);
                    filepath = direccionNuevo;
                }
                if (num == 4)
                {
                    string nom = Convert.ToString(file.FileName).Replace(".txt", string.Empty);
                    if (!System.IO.File.Exists(filepath))
                    {
                        using (var INeadLearn = new FileStream(filepath, FileMode.CreateNew))
                        {
                            file.CopyTo(INeadLearn);
                        }
                    }
                }
            }
            return filepath;
        }

        [Route("api/rsa/keys/{p}/{q}")]
        [HttpGet]
        public IActionResult Llaves(int p, int q)
        {
            bool VerificarLlaves=CifradoRSA.ComprobacionLlavesPrimos(p, q);
            if (VerificarLlaves)
            {
                string uploadsFolder = Path.Combine(fistenviroment.ContentRootPath, "UploadCifrado");
                string path = Path.Combine(uploadsFolder, "Llaves");
                if (!Directory.Exists(path))
                {
                    DirectoryInfo di = Directory.CreateDirectory(path);
                }

                CifradoRSA.GenerarLlaves(p, q, path);

                var files = System.IO.File.OpenRead(path+".zip");
                string mimeType = "application/zip";
                return new FileStreamResult(files, mimeType)
                {
                    FileDownloadName = "LlavesNuevas.zip"
                };
            }
            else
            {
                return Ok("El ingreso de los numeros p y q no son primos");

            }
        }
        [Route("api/rsa/{nombre}")]
        [HttpPost]
        public IActionResult CifrarDecifrarRSA([FromForm] IFormFile File, [FromForm] IFormFile Key, string nombre)
        {
            string uploadsFolder = Path.Combine(fistenviroment.ContentRootPath, "UploadDecifrado");
            string direccionNuevo = Path.Combine(uploadsFolder, nombre + ".txt");
            System.IO.File.WriteAllLines(direccionNuevo, new string[0]);
            String CifrarDesc = Path.Combine(uploadsFolder, File.FileName);
            String LlaveDesc = Path.Combine(uploadsFolder, Key.FileName);


            if (System.IO.File.Exists(CifrarDesc))
            {
                System.IO.File.Delete(CifrarDesc);
            }
            if (System.IO.File.Exists(LlaveDesc))
            {
                System.IO.File.Delete(LlaveDesc);
            }

            if (!System.IO.File.Exists(CifrarDesc))
            {
                using (var INeadLearn = new FileStream(CifrarDesc, FileMode.CreateNew))
                {
                    File.CopyTo(INeadLearn);
                }
            }
            if (!System.IO.File.Exists(LlaveDesc))
            {
                using (var INeadLearn = new FileStream(LlaveDesc, FileMode.CreateNew))
                {
                    Key.CopyTo(INeadLearn);
                }
            }
            CifradoRSA.CifrarRSA(CifrarDesc, direccionNuevo, LlaveDesc);

            var files = System.IO.File.OpenRead(direccionNuevo);
            string mimeType = "application/txt";
            return new FileStreamResult(files, mimeType)
            {
                FileDownloadName = nombre+".txt"
            };
        }
    }
}
