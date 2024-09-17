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

// BOTON PARA CREAR UN NUEVO ARCHIVO
        private void Boton_Crear_Archivo_Click(object sender, EventArgs e)
        {
            string fileName = PromptUserForFileName();
            string fileData = PromptUserForData();

            try
            {
                //CREAR ARCHIVO
                fileSystem.CrearArchivo(fileName, fileData);
                MessageBox.Show("¡Archivo Creado con Exito","CREAR ARCHIVO");
            }
            catch (Exception ex)
            {
                //MOSTRAR ERROR SI OCURRE ALGUN PROBLEMA
                MessageBox.Show($"¡ERROR!: {ex.Message}");
            }
        }

// BOTON PARA LISTAR LOS ARCHIVOS ACTIVOS *NO EN LA PAPELERA*
        private void Boton_Listar_Archivos_Click(object sender, EventArgs e)
        {
            //OBTENER LA LISTA DE ARCHIVOS 
            List<ArchivoFAT> archivos = fileSystem.ObtenerArchivosActivos();
            string mensaje = "Archivos Disponibles:\n";

            for (int i = 0; i < archivos.Count; i++)
            {
                var archivo = archivos[i];
                mensaje += $"{i + 1}. {archivo.Nombre} - {archivo.Tamano} caracteres - Creado: {archivo.FechaCreacion} - Modificado: {archivo.FechaModificacion}\n";
            }
            MessageBox.Show(mensaje, "LISTAR ARCHIVOS");
        }

 // BOTON PARA ABRIR UN ARCHIVO 
        private void Boton_Abrir_Archivo_Click(object sender, EventArgs e)
        {
            string fileName = PromptUserForFileName();
            string contenido = fileSystem.AbrirArchivo(fileName);

            if (!string.IsNullOrEmpty(contenido))
            {
                MessageBox.Show($"Contenido de {fileName}:\n{contenido}", "ABRIR ARCHIVO");
            }
            else
            {
                MessageBox.Show("Archivo NO Encontrado\nIntente de Nuevo","¡ERROR!");
            }
        }

//BOTON PARA MODIFICAR UN ARCHIVO
        private void Boton_Modificar_Archivo_Click(object sender, EventArgs e)
        {
            string fileName = PromptUserForFileName();
            string nuevosDatos = PromptUserForData();

            try
            {
                //MODIFICAR EL ARCHIVO CON LOS NUEVOS DATOS
                fileSystem.ModificarArchivo(fileName, nuevosDatos);
                MessageBox.Show("¡Archivo Modificado Correctamente!","MODIFICAR ARCHIVO");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"¡ERROR!: {ex.Message}");
            }
        }

//BOTON PARA ELIMINAR UN ARCHIVO (MOVER A LA PAPELERA)
        private void Boton_Eliminar_Archivo_Click(object sender, EventArgs e)
        {
            string fileName = PromptUserForFileName();

            try
            {
                //ELIMINAR EL ARCHIVO (MOVER A LA PAPELERA)
                fileSystem.EliminarArchivo(fileName);
                MessageBox.Show("¡Archivo Eliminado Correctamente!","ELIMINAR ARCHIVO");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }

// BOTON PARA RECUPERAR UN ARCHIVO
        private void Boton_Recuperar_Archivo_Click(object sender, EventArgs e)
        {
            string fileName = PromptUserForFileName();

            try
            {
                // RECUPERAR EL ARCHIVO DESDE LA PAPELERA
                fileSystem.RecuperarArchivo(fileName);
                MessageBox.Show("¡Archivo Recuperado Correctamente!","RECUPERAR ARCHIVO");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }

//BOTON PARA LISTAR LOS ARCHIVOS EN LA PAPELERA
        private void Boton_Papelera_Click(object sender, EventArgs e)
        {
            //OBTENER LA LISTA DE ARCHIVOS EN LA PAPELERA
            List<ArchivoFAT> archivosEnPapelera = fileSystem.ObtenerArchivosEnPapelera();
            string mensaje = "Archivos en la Papelera de Reciclaje:\n";

            if (archivosEnPapelera.Count == 0)
            {
                mensaje += "No hay Archivos en la Papelera de Reciclaje";
            }
            else
            {
                for (int i = 0; i < archivosEnPapelera.Count; i++)
                {
                    var archivo = archivosEnPapelera[i];
                    mensaje += $"{i + 1}. {archivo.Nombre} - {archivo.Tamano} caracteres - Eliminado: {archivo.FechaEliminacion}\n";
                }
            }

            //MOSTRAR LOS ARCHIVOS EN LA PAPELERA
            MessageBox.Show(mensaje, "PAPELERA DE RECICLAJE");
        }

        //BOTON PARA SALIR DE LA APLICACION
        private void Boton_Salir_Click(object sender, EventArgs e)
        {
            // MENSAJE DE DESPEDIDA ANTES DE SALIR
            MessageBox.Show("Gracias por Utilizar mi Programa\nPrograma Hecho Por: Angie Melissa Santiago Rodriguez - 1555123", "SALIR");
            Application.Exit();
        }

        //SOLICITAR AL USUARIO EL NOMBRE DEL ARCHIVO
        private string PromptUserForFileName()
        {
            return ShowInputDialog("Ingrese el Nombre del Archivo:", "NOMBRE DEL ARCHIVO");
        }

        //SOLICITA AL USUARIO LOS DATOS DEL ARCHIVI
        private string PromptUserForData()
        {
            return ShowInputDialog("Ingrese los Datos del Archivo", "DATOS DEL ARCHIVO");
        }

        //VENTANA EMERGENTE PARA SOLICITAR INFORMACION
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

        //CON EL PRIMER CLICK NO DABA, CON ESTE SI DA
        private void Boton_Papelera_Click_1(object sender, EventArgs e)
        {
            Boton_Papelera_Click(sender, e);
        }
    }
}
