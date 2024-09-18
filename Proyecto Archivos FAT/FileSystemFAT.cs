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

        //FUNCION PARA CREAR UN ARCHIVO NUEVO
        public void CrearArchivo(string nombre, string datos)
        {
            List<ArchivoFAT> archivosFAT = ObtenerTablaFAT();

            //VERIFICAR SI YA EXISTE UN ARCHIVO CON EL MISMO NOMBRE
            if (archivosFAT.Exists(a => a.Nombre == nombre && !a.EnPapelera))
            {
                throw new InvalidOperationException("Ya Existe un Archivo con ese Nombre\nIntente de Nuevo");
            }

            //20 CARACTERES
            if (datos.Length > 20)
            {
                throw new InvalidOperationException("No se Pueden Ingresar mas de 20 Caracteres\nIntente de Nuevo");
            }

            //CREAR UN OBJETO ARCHIVOFAT
            var archivoFAT = new ArchivoFAT
            {
                Nombre = nombre,
                RutaInicial = $"{nombre}_1.txt",
                EnPapelera = false,
                Tamano = datos.Length,
                FechaCreacion = DateTime.Now,
                FechaModificacion = DateTime.Now
            };

            //GUARDAR LOS DATOS EN ARCHIVOS SEPARADOS
            GuardarDatosEnArchivos(nombre, datos);
            GuardarFAT(archivoFAT);
        }

        //FUNCION PARA GUARDAR LOS DATOS EN ARCHIVOS SEPARADOS
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

        //GUARDA LA TABLA FAT
        private void GuardarFAT(ArchivoFAT archivoFAT)
        {
            List<ArchivoFAT> archivosFAT = ObtenerTablaFAT();
            archivosFAT.Add(archivoFAT);
            File.WriteAllText(PathFAT, JsonConvert.SerializeObject(archivosFAT, Formatting.Indented));
        }

        //OBTENER LA LISTA DE ARCHIVOS *SIN ELIMINAR*
        public List<ArchivoFAT> ObtenerArchivosActivos()
        {
            List<ArchivoFAT> archivosFAT = ObtenerTablaFAT();
            return archivosFAT.FindAll(a => !a.EnPapelera);
        }

        //ABRIR UN ARCHIVO EXISTENTE
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

        //LEER EL CONTENIDO DE UN ARCHIVO
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

        //OBTENER LA TABLA FAT
        private List<ArchivoFAT> ObtenerTablaFAT()
        {
            if (!File.Exists(PathFAT))
            {
                return new List<ArchivoFAT>();
            }

            string json = File.ReadAllText(PathFAT);
            return JsonConvert.DeserializeObject<List<ArchivoFAT>>(json);
        }

        // MODIFICAR UN ARCHIVO EXISTENTE
        public void ModificarArchivo(string nombre, string nuevosDatos)
        {
            List<ArchivoFAT> archivosFAT = ObtenerTablaFAT();
            var archivo = archivosFAT.Find(a => a.Nombre == nombre && !a.EnPapelera);

            if (archivo != null)
            {
                if (nuevosDatos.Length > 20)
                {
                    throw new InvalidOperationException("No se pueden ingresar más de 20 caracteres.");
                }

                int index = 1;
                while (File.Exists($"{nombre}_{index}.txt"))
                {
                    File.Delete($"{nombre}_{index}.txt");
                    index++;
                }

                GuardarDatosEnArchivos(nombre, nuevosDatos);

                archivo.Tamano = nuevosDatos.Length;
                archivo.FechaModificacion = DateTime.Now;

                File.WriteAllText(PathFAT, JsonConvert.SerializeObject(archivosFAT, Formatting.Indented));
            }
            else
            {
                throw new FileNotFoundException("Archivo no encontrado.");
            }
        }


        //ELIMINAR UN ARCHIVO *MOVER A LA PAPELERA*
        public void EliminarArchivo(string nombre)
        {
            List<ArchivoFAT> archivosFAT = ObtenerTablaFAT();
            var archivo = archivosFAT.Find(a => a.Nombre == nombre && !a.EnPapelera);

            if (archivo != null)
            {
                archivo.EnPapelera = true;
                archivo.FechaEliminacion = DateTime.Now;
                File.WriteAllText(PathFAT, JsonConvert.SerializeObject(archivosFAT, Formatting.Indented));
            }
        }

        //RECUPERAR UN ARCHIVO DESDE LA PAPELERA
        public void RecuperarArchivo(string nombre)
        {
            List<ArchivoFAT> archivosFAT = ObtenerTablaFAT();
            var archivo = archivosFAT.Find(a => a.Nombre == nombre && a.EnPapelera);

            if (archivo != null)
            {
                archivo.EnPapelera = false;
                archivo.FechaEliminacion = null;
                File.WriteAllText(PathFAT, JsonConvert.SerializeObject(archivosFAT, Formatting.Indented));
            }
        }

        //OBTENER LOS ARCHIVOS QUE ESTAN EN LA PAPELERA
        public List<ArchivoFAT> ObtenerArchivosEnPapelera()
        {
            List<ArchivoFAT> archivosFAT = ObtenerTablaFAT();
            return archivosFAT.FindAll(a => a.EnPapelera);
        }
    }
}
