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

        private void Form1_Load(object sender, EventArgs e)
        {
        }

        private void Boton_Crear_Archivo_Click(object sender, EventArgs e)
        {
            string fileName = PromptUserForFileName();
            string fileData = PromptUserForData();

            fileSystem.CrearArchivo(fileName, fileData);
            MessageBox.Show("Archivo creado con éxito.");
        }

        private void Boton_Listar_Archivos_Click(object sender, EventArgs e)
        {
            List<ArchivoFAT> archivos = fileSystem.ObtenerArchivosActivos();
            string mensaje = "Archivos disponibles:\n";

            for (int i = 0; i < archivos.Count; i++)
            {
                var archivo = archivos[i];
                mensaje += $"{i + 1}. {archivo.Nombre} - {archivo.Tamano} caracteres - Creado: {archivo.FechaCreacion} - Modificado: {archivo.FechaModificacion}\n";
            }

            MessageBox.Show(mensaje, "Listado de Archivos");
        }

        private void Boton_Abrir_Archivo_Click(object sender, EventArgs e)
        {
            string fileName = PromptUserForFileName();
            string contenido = fileSystem.AbrirArchivo(fileName);

            if (!string.IsNullOrEmpty(contenido))
            {
                MessageBox.Show($"Contenido de {fileName}:\n{contenido}", "Archivo Abierto");
            }
            else
            {
                MessageBox.Show("Archivo no encontrado o en la Papelera de Reciclaje.");
            }
        }

        private void Boton_Modificar_Archivo_Click(object sender, EventArgs e)
        {
            string fileName = PromptUserForFileName();
            fileSystem.ModificarArchivo(fileName);
        }

        private void Boton_Eliminar_Archivo_Click(object sender, EventArgs e)
        {
            string fileName = PromptUserForFileName();
            fileSystem.EliminarArchivo(fileName);
        }

        private void Boton_Recuperar_Archivo_Click(object sender, EventArgs e)
        {
            string fileName = PromptUserForFileName();
            fileSystem.RecuperarArchivo(fileName);
        }

        private void Boton_Salir_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private string PromptUserForFileName()
        {
            return ShowInputDialog("Ingrese el nombre del archivo:", "Nombre de Archivo");
        }

        private string PromptUserForData()
        {
            return ShowInputDialog("Ingrese los datos del archivo:", "Datos del Archivo");
        }

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
