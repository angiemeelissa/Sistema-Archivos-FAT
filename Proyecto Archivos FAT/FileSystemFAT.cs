using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace Proyecto_Archivos_FAT
{
    public class ArchivoFAT
    {
        public string Nombre { get; set; }
        public string RutaInicial { get; set; }
        public bool EnPapelera { get; set; }
        public int Tamano { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime FechaModificacion { get; set; }
        public DateTime? FechaEliminacion { get; set; }
    }

    public class FileSystemFAT
    {
        private const string PathFAT = "fat_table.json";

        public void CrearArchivo(string nombre, string datos)
        {
            var archivoFAT = new ArchivoFAT
            {
                Nombre = nombre,
                RutaInicial = $"{nombre}_1.txt",
                EnPapelera = false,
                Tamano = datos.Length,
                FechaCreacion = DateTime.Now,
                FechaModificacion = DateTime.Now
            };

            // Partir los datos en trozos de 20 caracteres y almacenarlos en archivos
            GuardarDatosEnArchivos(nombre, datos);
            GuardarFAT(archivoFAT);
        }

        private void GuardarDatosEnArchivos(string nombre, string datos)
        {
            int index = 1;
            for (int i = 0; i < datos.Length; i += 20)
            {
                string chunk = datos.Substring(i, Math.Min(20, datos.Length - i));
                File.WriteAllText($"{nombre}_{index}.txt", chunk);
                index++;
            }
        }

        private void GuardarFAT(ArchivoFAT archivoFAT)
        {
            List<ArchivoFAT> archivosFAT = ObtenerTablaFAT();
            archivosFAT.Add(archivoFAT);
            File.WriteAllText(PathFAT, JsonConvert.SerializeObject(archivosFAT, Formatting.Indented));
        }

        public List<ArchivoFAT> ObtenerArchivosActivos()
        {
            List<ArchivoFAT> archivosFAT = ObtenerTablaFAT();
            return archivosFAT.FindAll(a => !a.EnPapelera);
        }

        public string AbrirArchivo(string nombre)
        {
            List<ArchivoFAT> archivosFAT = ObtenerTablaFAT();
            var archivo = archivosFAT.Find(a => a.Nombre == nombre && !a.EnPapelera);

            if (archivo != null)
            {
                return LeerContenidoArchivo(nombre);
            }

            return null;
        }

        private string LeerContenidoArchivo(string nombre)
        {
            string contenido = "";
            int index = 1;
            while (File.Exists($"{nombre}_{index}.txt"))
            {
                contenido += File.ReadAllText($"{nombre}_{index}.txt");
                index++;
            }
            return contenido;
        }

        private List<ArchivoFAT> ObtenerTablaFAT()
        {
            if (!File.Exists(PathFAT))
            {
                return new List<ArchivoFAT>();
            }

            string json = File.ReadAllText(PathFAT);
            return JsonConvert.DeserializeObject<List<ArchivoFAT>>(json);
        }

        public void ModificarArchivo(string nombre)
        {
            // Implementación de la modificación del archivo
        }

        public void EliminarArchivo(string nombre)
        {
            // Implementación de la eliminación del archivo
        }

        public void RecuperarArchivo(string nombre)
        {
            // Implementación de la recuperación del archivo
        }
    }
}
