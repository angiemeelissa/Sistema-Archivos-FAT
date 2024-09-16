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

        // FUNCION PARA CREAR UN ARCHIVO NUEVO
        public void CrearArchivo(string nombre, string datos)
        {
            List<ArchivoFAT> archivosFAT = ObtenerTablaFAT();

            // VERIFICAR SI YA EXISTE UN ARCHIVO CON EL MISMO NOMBRE
            if (archivosFAT.Exists(a => a.Nombre == nombre && !a.EnPapelera))
            {
                throw new InvalidOperationException("YA EXISTE UN ARCHIVO CON ESE NOMBRE");
            }

            // ASEGURARSE DE QUE LOS DATOS NO EXCEDAN 20 CARACTERES
            if (datos.Length > 20)
            {
                throw new InvalidOperationException("NO SE PUEDE INGRESAR MAS DE 20 CARACTERES EN LOS DATOS DEL ARCHIVO");
            }

            // CREAR UN OBJETO ARCHIVOFAT
            var archivoFAT = new ArchivoFAT
            {
                Nombre = nombre,
                RutaInicial = $"{nombre}_1.txt",
                EnPapelera = false,
                Tamano = datos.Length,
                FechaCreacion = DateTime.Now,
                FechaModificacion = DateTime.Now
            };

            // GUARDAR LOS DATOS EN ARCHIVOS SEPARADOS
            GuardarDatosEnArchivos(nombre, datos);
            GuardarFAT(archivoFAT);
        }

        // FUNCION PARA GUARDAR LOS DATOS EN ARCHIVOS SEPARADOS
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

        // FUNCION PARA GUARDAR LA TABLA FAT
        private void GuardarFAT(ArchivoFAT archivoFAT)
        {
            List<ArchivoFAT> archivosFAT = ObtenerTablaFAT();
            archivosFAT.Add(archivoFAT);
            File.WriteAllText(PathFAT, JsonConvert.SerializeObject(archivosFAT, Formatting.Indented));
        }

        // FUNCION PARA OBTENER LA LISTA DE ARCHIVOS ACTIVOS
        public List<ArchivoFAT> ObtenerArchivosActivos()
        {
            List<ArchivoFAT> archivosFAT = ObtenerTablaFAT();
            return archivosFAT.FindAll(a => !a.EnPapelera);
        }

        // FUNCION PARA ABRIR UN ARCHIVO EXISTENTE
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

        // FUNCION PARA LEER EL CONTENIDO DE UN ARCHIVO
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

        // FUNCION PARA OBTENER LA TABLA FAT
        private List<ArchivoFAT> ObtenerTablaFAT()
        {
            if (!File.Exists(PathFAT))
            {
                return new List<ArchivoFAT>();
            }

            string json = File.ReadAllText(PathFAT);
            return JsonConvert.DeserializeObject<List<ArchivoFAT>>(json);
        }

        // FUNCION PARA MODIFICAR UN ARCHIVO EXISTENTE
        public void ModificarArchivo(string nombre, string nuevosDatos)
        {
            // OBTENER LA LISTA DE ARCHIVOS
            List<ArchivoFAT> archivosFAT = ObtenerTablaFAT();
            var archivo = archivosFAT.Find(a => a.Nombre == nombre && !a.EnPapelera);

            if (archivo != null)
            {
                // ASEGURARSE DE QUE LOS NUEVOS DATOS NO EXCEDAN 20 CARACTERES
                if (nuevosDatos.Length > 20)
                {
                    throw new InvalidOperationException("NO SE PUEDE INGRESAR MAS DE 20 CARACTERES EN LOS DATOS DEL ARCHIVO");
                }

                // MODIFICAR LOS DATOS DEL ARCHIVO
                archivo.Tamano = nuevosDatos.Length;
                archivo.FechaModificacion = DateTime.Now;

                // GUARDAR LOS NUEVOS DATOS EN EL SISTEMA DE ARCHIVOS
                GuardarDatosEnArchivos(nombre, nuevosDatos);
                GuardarFAT(archivo);
            }
        }

        // FUNCION PARA ELIMINAR UN ARCHIVO (MOVER A LA PAPELERA)
        public void EliminarArchivo(string nombre)
        {
            // OBTENER LA LISTA DE ARCHIVOS DE LA TABLA FAT
            List<ArchivoFAT> archivosFAT = ObtenerTablaFAT();

            // BUSCAR EL ARCHIVO QUE SE VA A ELIMINAR
            var archivo = archivosFAT.Find(a => a.Nombre == nombre && !a.EnPapelera);

            if (archivo != null)
            {
                // MARCAR EL ARCHIVO COMO "EN PAPELERA"
                archivo.EnPapelera = true;
                archivo.FechaEliminacion = DateTime.Now;

                // GUARDAR LOS CAMBIOS EN LA TABLA FAT
                File.WriteAllText(PathFAT, JsonConvert.SerializeObject(archivosFAT, Formatting.Indented));
            }
        }

        // FUNCION PARA RECUPERAR UN ARCHIVO DESDE LA PAPELERA
        public void RecuperarArchivo(string nombre)
        {
            // OBTENER LA LISTA DE ARCHIVOS
            List<ArchivoFAT> archivosFAT = ObtenerTablaFAT();

            // BUSCAR EL ARCHIVO EN LA PAPELERA
            var archivo = archivosFAT.Find(a => a.Nombre == nombre && a.EnPapelera);

            if (archivo != null)
            {
                // MARCAR EL ARCHIVO COMO NO ESTANDO EN LA PAPELERA
                archivo.EnPapelera = false;
                archivo.FechaEliminacion = null;

                // GUARDAR LOS CAMBIOS EN LA TABLA FAT
                File.WriteAllText(PathFAT, JsonConvert.SerializeObject(archivosFAT, Formatting.Indented));
            }
        }

        // FUNCION PARA OBTENER LOS ARCHIVOS QUE ESTAN EN LA PAPELERA
        public List<ArchivoFAT> ObtenerArchivosEnPapelera()
        {
            List<ArchivoFAT> archivosFAT = ObtenerTablaFAT();
            return archivosFAT.FindAll(a => a.EnPapelera);
        }
    }
}
