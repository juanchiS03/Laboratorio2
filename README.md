# Laboratorio2

Este proyecto en C# procesa y limpia datos sobre animales desde un archivo CSV y consulta datos adicionales desde Wikidata.

## Requisitos

- [.NET Core 3.1](https://dotnet.microsoft.com/download/dotnet/3.1) o superior.
- [RestSharp](https://www.nuget.org/packages/RestSharp) y [Newtonsoft.Json](https://www.nuget.org/packages/Newtonsoft.Json) deben ser instalados a través de NuGet.

## Instalación

### 1. Clonar el Repositorio

Abre una terminal o línea de comandos y clona el repositorio:
```bash
git clone https://github.com/juanchiS03/Laboratorio2.git
cd Laboratorio2

### 2. Notas
Asegúrate de que el archivo Lab_data_alumno_Lab.csv esté en la misma ruta especificada que en el código (en mi caso C:\Users\juansanchez\OneDrive - RIAM I+L LAB S.L\Lab_data_alumno_Lab.csv). Puedes ajustar la ruta en el código si es necesario para que coincida con la tuya.
El programa generará un nuevo archivo CSV llamado Lab_data_alumno_Lab_Actualizado.csv en la misma ruta que el archivo CSV de entrada. Este archivo contendrá los datos procesados y limpiados.

Para ejecutar el programa, clona el repositorio en tu ordenador y ejecútalo mediante Visual Studio.
