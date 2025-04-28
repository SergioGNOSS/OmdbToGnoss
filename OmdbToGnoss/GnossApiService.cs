using Gnoss.ApiWrapper;
using Gnoss.ApiWrapper.ApiModel;
using Gnoss.ApiWrapper.Model;
using PeliculaOntology;
using PersonaOntology;
using System;
using System.Collections.Generic;
using System.IO;

namespace OMDbToGnoss
{
    class GnossApiService
    {

        private ResourceApi mResourceApi;
        string espacioNombresGrafo;
        public GnossApiService(string espacioNombresGrafo)
        {
            this.espacioNombresGrafo = espacioNombresGrafo;
            mResourceApi = new ResourceApi(Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Config\OAuth.config")));
        }
        public void CargarPersonas(string nombreOntologiaPersona, List<Person> personas, Dictionary<string,string> guidDictionary)
        {
            mResourceApi.ChangeOntoly(nombreOntologiaPersona);
            List<string> guidsExistentes = new List<string>();
            try
            {
                SparqlObject resultados = mResourceApi.VirtuosoQuery("Select distinct ?o ?id  ", "Where { ?o <http://schema.org/name> ?id }", nombreOntologiaPersona);
                foreach (var resultado in resultados.results.bindings)
                {
                    guidsExistentes.Add(resultado["o"].value);
                }
            }
            catch (Exception e)
            {
                mResourceApi.Log.Error($"Error al hacer la consulta a Virtuoso: {e.Message} -> {e.StackTrace}");
            }


            
            foreach (Person persona in personas)
            {
                if (!guidsExistentes.Contains(espacioNombresGrafo + guidDictionary[persona.Schema_name]))
                {
                    try
                    {
                        ComplexOntologyResource complexResource = persona.ToGnossApiResource(mResourceApi, null, new Guid(guidDictionary[persona.Schema_name].Split('_')[1]), new Guid(guidDictionary[persona.Schema_name].Split('_')[2]));
                        mResourceApi.LoadComplexSemanticResource(complexResource);
                    }
                    catch (Exception e)
                    {
                        mResourceApi.Log.Error(persona.Schema_name);
                    }
                }
            }

        }

        public void CargarPeliculas(string nombreOntologiaPeliculas, List<Movie> peliculas, Dictionary<string, string> guidDictionary)
        {
            mResourceApi.ChangeOntoly(nombreOntologiaPeliculas);
            List<string> guidsExistentes = new List<string>();
            try
            {
                SparqlObject resultados = mResourceApi.VirtuosoQuery("Select distinct ?o ?id  ", "Where { ?o <http://schema.org/name> ?id  }", nombreOntologiaPeliculas);
                foreach (var resultado in resultados.results.bindings)
                {
                    guidsExistentes.Add(resultado["o"].value);                   
                }
            }
            catch (Exception e)
            {
                mResourceApi.Log.Error($"Error al hacer la consulta a Virtuoso: {e.Message} -> {e.StackTrace}");
            }

            foreach (Movie pelicula in peliculas)
            {
                if (!guidsExistentes.Contains(espacioNombresGrafo + guidDictionary[pelicula.Schema_name])) { 
                    try
                    {
                    //ComplexOntologyResource complexResource = pelicula.ToGnossApiResource(mResourceApi, null, Guid.NewGuid(), new Guid());

                    ComplexOntologyResource complexResource = pelicula.ToGnossApiResource(mResourceApi, null, new Guid(guidDictionary[pelicula.Schema_name].Split('_')[1]), new Guid(guidDictionary[pelicula.Schema_name].Split('_')[2]));
                    mResourceApi.LoadComplexSemanticResource(complexResource);
                    }
                    catch (Exception e)
                    {
                        mResourceApi.Log.Error(pelicula.Schema_name);
                    }
                }
            }

        }
    }
}
