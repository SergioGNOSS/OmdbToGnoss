using System;
using System.Collections.Generic;
using System.IO;
namespace OMDbToGnoss
{
    class Program
    {
        static void Main(string[] args)
        {
            int MAXNUMPELICULASPERMITIDO = 1000;
            //string NOMBRE_ONTOLOGIA_PELICULA = "mkgapelicula";
            string NOMBRE_ONTOLOGIA_PELICULA = "";
            //string NOMBRE_ONTOLOGIA_PERSONA = "mkgapersona";
            string NOMBRE_ONTOLOGIA_PERSONA = "";

            Dictionary<string, KeyValuePair<string, string>> ENTORNO_PREF_G_ONT = new Dictionary<string, KeyValuePair<string, string>>()
            {
                { "1", new KeyValuePair<string, string>("TESTING", "http://testing.gnoss.com/items/") },
                { "2", new KeyValuePair<string, string>("TRY", "http://gnoss.com/items/")},
                { "3", new KeyValuePair<string, string>("DEVAKADEMIACODE", "http://gnoss.com/items/")},
            };
            GestionDeCarga.LanzarOmdToGnoss(
                MAXNUMPELICULASPERMITIDO, 
                ENTORNO_PREF_G_ONT, 
                NOMBRE_ONTOLOGIA_PELICULA, 
                NOMBRE_ONTOLOGIA_PERSONA
                );
        }
    }

    class GestionDeCarga{ 
    
        public GestionDeCarga() { }
    
        public static void LanzarOmdToGnoss(
            int MAXNUMPELICULASPERMITIDO, 
            Dictionary<string, KeyValuePair<string, string>> ENTORNO_PREF_G_ONT,
            string NOMBRE_ONTOLOGIA_PELICULA,
            string NOMBRE_ONTOLOGIA_PERSONA
            )
        {
            int numPeliculasCargar = ObtenerNumPeliculasCargar(MAXNUMPELICULASPERMITIDO);
            KeyValuePair<string, string> entornoSeleccionado = ObtenerEntorno(ENTORNO_PREF_G_ONT);
            LimpiarCsvCacheInterna();
            LectorPeliculas lector = new LectorPeliculas(numPeliculasCargar);
            GnossApiService gnossApiService = new GnossApiService(entornoSeleccionado.Value);
            gnossApiService.CargarPeliculas(NOMBRE_ONTOLOGIA_PELICULA, lector.Movies, lector.PeliculasGuid);
            gnossApiService.CargarPersonas(NOMBRE_ONTOLOGIA_PERSONA, lector.Persons, lector.PersonasGuid);
            Console.ReadLine();
        }

        private static void LimpiarCsvCacheInterna()
        {
            Console.WriteLine($"-------------------------------------------");
            Console.WriteLine("¿Quieres realizar una carga desde cero? (Y/n)");
            string cargaNueva = Console.ReadLine();
            if (cargaNueva != "N" && cargaNueva != "No" && cargaNueva != "no" && cargaNueva != "n") {
                // Obtiene todos los archivos .csv en la carpeta
                string[] archivos = Directory.GetFiles(Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data")), "*.csv");
                // Elimina cada archivo
                foreach (string archivo in archivos)
                {
                    File.Delete(archivo);
                    Console.WriteLine($"Archivo eliminado: {Path.GetFileName(archivo)}");
                }
                Console.WriteLine($"-------------------------------------------");
                Console.WriteLine($"Se eliminaron {archivos.Length} archivos CSV de la ruta {Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data"))}");
            }
        }

        private static KeyValuePair<string, string> ObtenerEntorno(Dictionary<string, KeyValuePair<string, string>> ENTORNO_PREF_G_ONT)
        {
            KeyValuePair<string, string> entornoSeleccionado = new KeyValuePair<string, string>();
            Console.WriteLine("Selecciona un entorno para hacer la carga (Introduce el número)");
            bool condicion = !(entornoSeleccionado.Key is null);
            while (!condicion)
            {
                foreach (KeyValuePair<string, KeyValuePair<string, string>> par in ENTORNO_PREF_G_ONT)
                {
                    Console.WriteLine($"{par.Key} -> Valor: {par.Value.Key} - {par.Value.Value}");
                }
                string entorno = Console.ReadLine();
                try
                {
                    // Comprobación de excepción
                    int numEntorno = int.Parse(entorno);
                    condicion = numEntorno > 0 && numEntorno <= ENTORNO_PREF_G_ONT.Keys.Count;
                    if (condicion)
                    {
                        entornoSeleccionado = ENTORNO_PREF_G_ONT[numEntorno.ToString()];
                        Console.WriteLine($"Has selecccionado el entornno {entornoSeleccionado.Key} ({entornoSeleccionado.Key})");
                        Console.WriteLine($"-------------------------------------------");
                    }
                    else
                    {
                        Console.WriteLine($"Por favor, elige un número de entorno válido");
                    }
                }
                catch (Exception)
                {
                    Console.WriteLine($"Por favor, selecciona el entorno utilizando el número");
                }
            }
            return entornoSeleccionado;
        }

        public static int ObtenerNumPeliculasCargar(int MAXNUMPELICULASPERMITIDO)
        {
            int numPeliculasCargar = 0;
            bool condiciónNumPeliculas = false;
            Console.WriteLine($"Selecciona el numero maximo de películas a cargar (1 - {MAXNUMPELICULASPERMITIDO})");
            while (!condiciónNumPeliculas)
            {
                try
                {
                    numPeliculasCargar = int.Parse(Console.ReadLine());
                    condiciónNumPeliculas = numPeliculasCargar > 0 && numPeliculasCargar <= MAXNUMPELICULASPERMITIDO;
                    if (condiciónNumPeliculas)
                    {
                        Console.WriteLine($"Vas a cargar {numPeliculasCargar} películas");
                        Console.WriteLine($"-------------------------------------------");
                    }
                    else
                    {
                        Console.WriteLine($"Por favor, elige un número de películas dentro del rango permitido (1- {MAXNUMPELICULASPERMITIDO})");
                    }
                }
                catch (Exception)
                {
                    Console.WriteLine($"No has seleccionado un número de películas dentro del rango permitido. Por favor, selecciona un número de películas dentro del rango permitido  (1- {MAXNUMPELICULASPERMITIDO})");
                }
            }
            return numPeliculasCargar;
        }

    }
}
