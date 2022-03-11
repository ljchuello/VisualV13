
using System;
using System.IO;
using Newtonsoft.Json;

namespace VisualV13.Libreria
{
    public class Configuracion
    {
        public string Url { set; get; } = "http://localhost/";
        public string Usuario { set; get; } = "admin@admin.com";
        public string Contrasenia { set; get; } = "123456";
        public string MsSqlServer { set; get; } = "MSSQL$SQLEXPRESS";
        public bool ReenviarCorreo { set; get; } = false;

        public bool Guardar(Configuracion configuracion)
        {
            try
            {
                // Set's
                string directorio = $"{Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)}";
                string fileName = $"{directorio}\\SermatickV13.block";

                // Validamos el directorio
                ValidarDirectorio(directorio, fileName);

                // Guardamos
                File.WriteAllText(fileName, JsonConvert.SerializeObject(configuracion, Formatting.Indented));

                // Libre de pecados
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return false;
            }
        }

        public Configuracion Leer()
        {
            try
            {
                // Set's
                string directorio = $"{Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)}";
                string fileName = $"{directorio}\\SermatickV13.block";

                // Validamos el directorio
                ValidarDirectorio(directorio, fileName);

                // Leemos
                string contenido = File.ReadAllText(fileName);

                // Deserializamos
                Configuracion configuracion = JsonConvert.DeserializeObject<Configuracion>(contenido) ?? new Configuracion();

                // Libre de pecados
                return configuracion;
            }
            catch (Exception)
            {
                return new Configuracion();
            }
        }

        private static void ValidarDirectorio(string directorio, string fileName)
        {
            if (!Directory.Exists(directorio))
            {
                Directory.CreateDirectory(directorio);
            }

            if (!File.Exists(fileName))
            {
                File.WriteAllText(fileName, string.Empty);
            }
        }
    }
}
