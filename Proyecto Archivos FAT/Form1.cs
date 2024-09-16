using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using Newtonsoft.Json;

namespace Proyecto_Archivos_FAT
{
    public partial class Form1 : Form
    {
        private FileSystemFAT fileSystem = new FileSystemFAT();

        public Form1()
        {
            InitializeComponent();
        }

        // EVENTO QUE SE EJECUTA AL CARGAR EL FORMULARIO
        private void Form1_Load(object sender, EventArgs e)
        {
        }

        // BOTON PARA CREAR UN NUEVO ARCHIVO
        private void Boton_Crear_Archivo_Click(object sender, EventArgs e)
        {
            string fileName = PromptUserForFileName();
            string fileData = PromptUserForData();

            try
            {
                // INTENTAR CREAR EL ARCHIVO
                fileSystem.CrearArchivo(fileName, fileData);
                MessageBox.Show("Archivo creado con éxito.");
            }
            catch (Exception ex)
            {
                // MOSTRAR ERROR SI OCURRE ALGUN PROBLEMA
                MessageBox.Show($"Error: {ex.Message}");
            }
        }

        // BOTON PARA LISTAR LOS ARCHIVOS ACTIVOS (NO EN LA PAPELERA)
        private void Boton_Listar_Archivos_Click(object sender, EventArgs e)
        {
            // OBTENER LA LISTA DE ARCHIVOS ACTIVOS
            List<ArchivoFAT> archivos = fileSystem.ObtenerArchivosActivos();
            string mensaje = "Archivos disponibles:\n";

            for (int i = 0; i < archivos.Count; i++)
            {
                var archivo = archivos[i];
                mensaje += $"{i + 1}. {archivo.Nombre} - {archivo.Tamano} caracteres - Creado: {archivo.FechaCreacion} - Modificado: {archivo.FechaModificacion}\n";
            }

            // MOSTRAR LOS ARCHIVOS ACTIVOS EN UN MESSAGEBOX
            MessageBox.Show(mensaje, "Listado de Archivos");
        }

        // BOTON PARA ABRIR UN ARCHIVO ESPECIFICO
        private void Boton_Abrir_Archivo_Click(object sender, EventArgs e)
        {
            string fileName = PromptUserForFileName();
            string contenido = fileSystem.AbrirArchivo(fileName);

            if (!string.IsNullOrEmpty(contenido))
            {
                // MOSTRAR EL CONTENIDO DEL ARCHIVO EN UN MESSAGEBOX
                MessageBox.Show($"Contenido de {fileName}:\n{contenido}", "Archivo Abierto");
            }
            else
            {
                // MOSTRAR UN MENSAJE SI EL ARCHIVO NO SE ENCUENTRA
                MessageBox.Show("Archivo no encontrado o en la Papelera de Reciclaje.");
            }
        }

        // BOTON PARA MODIFICAR UN ARCHIVO EXISTENTE
        private void Boton_Modificar_Archivo_Click(object sender, EventArgs e)
        {
            string fileName = PromptUserForFileName();
            string nuevosDatos = PromptUserForData();

            try
            {
                // MODIFICAR EL ARCHIVO CON LOS NUEVOS DATOS
                fileSystem.ModificarArchivo(fileName, nuevosDatos);
                MessageBox.Show("Archivo modificado con éxito.");
            }
            catch (Exception ex)
            {
                // MOSTRAR ERROR SI OCURRE ALGUN PROBLEMA
                MessageBox.Show($"Error: {ex.Message}");
            }
        }

        // BOTON PARA ELIMINAR UN ARCHIVO (MOVER A LA PAPELERA)
        private void Boton_Eliminar_Archivo_Click(object sender, EventArgs e)
        {
            string fileName = PromptUserForFileName();

            try
            {
                // ELIMINAR EL ARCHIVO (MOVER A LA PAPELERA)
                fileSystem.EliminarArchivo(fileName);
                MessageBox.Show("Archivo movido a la Papelera de Reciclaje.");
            }
            catch (Exception ex)
            {
                // MOSTRAR ERROR SI OCURRE ALGUN PROBLEMA
                MessageBox.Show($"Error: {ex.Message}");
            }
        }

        // BOTON PARA RECUPERAR UN ARCHIVO DESDE LA PAPELERA
        private void Boton_Recuperar_Archivo_Click(object sender, EventArgs e)
        {
            string fileName = PromptUserForFileName();

            try
            {
                // RECUPERAR EL ARCHIVO DESDE LA PAPELERA
                fileSystem.RecuperarArchivo(fileName);
                MessageBox.Show("Archivo recuperado con éxito.");
            }
            catch (Exception ex)
            {
                // MOSTRAR ERROR SI OCURRE ALGUN PROBLEMA
                MessageBox.Show($"Error: {ex.Message}");
            }
        }

        // BOTON PARA LISTAR LOS ARCHIVOS EN LA PAPELERA
        private void Boton_Papelera_Click(object sender, EventArgs e)
        {
            // OBTENER LA LISTA DE ARCHIVOS EN LA PAPELERA
            List<ArchivoFAT> archivosEnPapelera = fileSystem.ObtenerArchivosEnPapelera();
            string mensaje = "Archivos en la Papelera de Reciclaje:\n";

            for (int i = 0; i < archivosEnPapelera.Count; i++)
            {
                var archivo = archivosEnPapelera[i];
                mensaje += $"{i + 1}. {archivo.Nombre} - {archivo.Tamano} caracteres - Eliminado: {archivo.FechaEliminacion}\n";
            }

            // MOSTRAR LOS ARCHIVOS EN LA PAPELERA EN UN MESSAGEBOX
            MessageBox.Show(mensaje, "Papelera de Reciclaje");
        }

        // BOTON PARA SALIR DE LA APLICACION
        private void Boton_Salir_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        // FUNCION PARA SOLICITAR AL USUARIO EL NOMBRE DEL ARCHIVO
        private string PromptUserForFileName()
        {
            return ShowInputDialog("Ingrese el nombre del archivo:", "Nombre de Archivo");
        }

        // FUNCION PARA SOLICITAR AL USUARIO LOS DATOS DEL ARCHIVO
        private string PromptUserForData()
        {
            return ShowInputDialog("Ingrese los datos del archivo (max 20 caracteres):", "Datos del Archivo");
        }

        // FUNCION QUE MUESTRA UNA VENTANA EMERGENTE PARA SOLICITAR INFORMACION
        private string ShowInputDialog(string prompt, string title)
        {
            Form promptForm = new Form()
            {
                Width = 400,
                Height = 200,
                Text = title,
                StartPosition = FormStartPosition.CenterScreen
            };

            Label textLabel = new Label() { Left = 50, Top = 20, Text = prompt, AutoSize = true };
            TextBox inputBox = new TextBox() { Left = 50, Top = 50, Width = 300 };
            Button confirmationButton = new Button() { Text = "Aceptar", Left = 250, Width = 100, Top = 100, DialogResult = DialogResult.OK };

            promptForm.Controls.Add(textLabel);
            promptForm.Controls.Add(inputBox);
            promptForm.Controls.Add(confirmationButton);
            promptForm.AcceptButton = confirmationButton;

            return promptForm.ShowDialog() == DialogResult.OK ? inputBox.Text : "";
        }
    }
}
