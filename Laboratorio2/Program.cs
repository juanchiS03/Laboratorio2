using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using RestSharp;
using Newtonsoft.Json.Linq;
using System.Text.RegularExpressions;

class Animal
{
    public string Nombre { get; set; }
    public string Descripcion { get; set; }
    public string Imagen { get; set; }
    public string Velocidad { get; set; }
    public string LinkWikidata { get; set; }
    public string NombreComun { get; set; }
    public string Interes { get; set; }
    public string Familia { get; set; } // Nueva propiedad para familia

    private static readonly Regex LinkPattern = new Regex(@"^(http|https)://", RegexOptions.Compiled);

    // Limpia los campos no relacionados con enlaces
    public void LimpiarDatos()
    {
        Descripcion = LimpiarCampoTexto(Descripcion);
        Velocidad = LimpiarCampoTexto(Velocidad);
        NombreComun = LimpiarCampoTexto(NombreComun);
        Interes = LimpiarCampoTexto(Interes);
    }

    private string LimpiarCampoTexto(string campo)
    {
        // Limpiar texto eliminando enlaces si existen
        if (string.IsNullOrEmpty(campo))
            return campo;

        // Si el campo contiene un enlace, limpiarlo
        return LinkPattern.IsMatch(campo) ? null : campo.Trim();
    }

    public void MostrarInformacion()
    {
        Console.WriteLine($"Animal: {Nombre}");
        Console.WriteLine($"Descripción: {Descripcion}");
        Console.WriteLine($"Imagen: {Imagen}");
        Console.WriteLine($"Velocidad: {Velocidad}");
        Console.WriteLine($"Link Wikidata: {LinkWikidata}");
        Console.WriteLine($"Nombre Común: {NombreComun}");
        Console.WriteLine($"Interés: {Interes}");
        Console.WriteLine($"Familia: {Familia}"); // Mostrar familia
        Console.WriteLine(new string('-', 30));
    }
}

class Program
{
    static async Task Main(string[] args)
    {
        // Diccionario para almacenar los datos por animal
        Dictionary<string, Animal> animales = new Dictionary<string, Animal>();

        // Leer el CSV
        string[] lineas = File.ReadAllLines(@"C:\Users\juansanchez\OneDrive - RIAM I+L LAB S.L\Lab_data_alumno_Lab.csv");

        foreach (string linea in lineas)
        {
            // Separar las columnas de la línea
            string[] columnas = linea.Split(';');

            // Asegurarse de que la línea tenga al menos 3 columnas
            if (columnas.Length < 3)
            {
                Console.WriteLine($"Línea ignorada por tener menos de 3 columnas: {linea}");
                continue; // Ignorar líneas con menos de 3 columnas
            }

            string animal = columnas[0];
            string campo = columnas[1];
            string informacion = columnas[2];

            if (!animales.ContainsKey(animal))
            {
                animales[animal] = new Animal { Nombre = animal };
            }

            switch (campo.ToLower())
            {
                case "descripción":
                    animales[animal].Descripcion = informacion;
                    break;
                case "imagen":
                    animales[animal].Imagen = informacion;
                    break;
                case "velocidad":
                    animales[animal].Velocidad = informacion;
                    break;
                case "link wikidata":
                    animales[animal].LinkWikidata = informacion;
                    break;
                case "nombre común":
                    animales[animal].NombreComun = informacion;
                    break;
                case "interés":
                    animales[animal].Interes = informacion;
                    break;
            }
        }

        // Obtener la familia de los animales desde Wikidata
        var options = new RestClientOptions("https://query.wikidata.org/sparql?query=SELECT%20%3Fanimal%20%3FanimalLabel%20%3Ffamilia%20%3FfamiliaLabel%20WHERE%20%7B%0A%20%20%3Fanimal%20wdt%3AP31%20wd%3AQ729.%0A%20%20%3Fanimal%20wdt%3AP171%20%3Ffamilia.%20%20%20%20%20%20%20%20%20%20%0A%20%20SERVICE%20wikibase%3Alabel%20%7B%20bd%3AserviceParam%20wikibase%3Alanguage%20%22es%2Cen%22.%20%7D%0A%7D%0A")
        {
            MaxTimeout = -1,
            UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/128.0.0.0 Safari/537.36",
        };
        var client = new RestClient(options);
        var request = new RestRequest();
        request.AddHeader("sec-ch-ua", "\"Chromium\";v=\"128\", \"Not;A=Brand\";v=\"24\", \"Google Chrome\";v=\"128\"");
        request.AddHeader("Accept", "application/sparql-results+json");
        request.AddHeader("X-Requested-With", "XMLHttpRequest");
        request.AddHeader("sec-ch-ua-mobile", "?0");
        request.AddHeader("sec-ch-ua-platform", "\"Windows\"");
        request.AddHeader("Sec-Fetch-Site", "same-origin");
        request.AddHeader("Sec-Fetch-Mode", "cors");
        request.AddHeader("Sec-Fetch-Dest", "empty");
        request.AddHeader("host", "query.wikidata.org");
        RestResponse response = await client.ExecuteAsync(request);

        var json = JObject.Parse(response.Content);
        var resultados = json["results"]["bindings"];

        // Mapeo de familias por identificador de animal
        var familias = resultados.ToDictionary(
            item => (string)item["animal"]["value"],
            item => (string)item["familiaLabel"]["value"]
        );

        // Asignar la familia a los animales
        foreach (var animal in animales.Values)
        {
            var linkWikidata = animal.LinkWikidata;
            if (linkWikidata != null && familias.TryGetValue(linkWikidata, out var familia))
            {
                animal.Familia = familia;
            }

            // Limpiar los datos del animal
            animal.LimpiarDatos();
        }

        // Guardar los datos actualizados en un nuevo archivo CSV
        using (var writer = new StreamWriter(@"C:\Users\juansanchez\Downloads\Lab_data_alumno_Lab_Actualizado.csv"))
        {
            writer.WriteLine("Nombre;Descripción;Imagen;Velocidad;Link Wikidata;Nombre Común;Interés;Familia");
            foreach (var animal in animales.Values)
            {
                writer.WriteLine($"{animal.Nombre};{animal.Descripcion};{animal.Imagen};{animal.Velocidad};{animal.LinkWikidata};{animal.NombreComun};{animal.Interes};{animal.Familia}");
            }
        }
    }
}
