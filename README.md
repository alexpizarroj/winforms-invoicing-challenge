# Code Challenge: App WinForms para Servicio de Facturación

Este repositorio presenta un reto de programación para evaluar tus habilidades con C# y
Windows Forms.

## Objetivos

- Diseñar e implementar una aplicación Windows Forms en C# que se conecte a una API de
facturación simulada, permitiendo la consulta y visualización de facturas.
- Escribir código de alta calidad, haciendo uso de buenas prácticas y buena organización.
- Escribir una aplicación robusta frente a errores comunes, como ser una falta de
  conexión a la API.

## Requerimientos

- La aplicación debe permitir emitir una factura. Después de la emisión, la aplicación
  debe hacer "polling" al estado de la misma hasta que esta pase a `Accepted`.
- La aplicación debe permitir ver las facturas emitidas y sus estados (`Pending`, `Accepted`).
- La aplicación debe permitir descargar un documento PDF para facturas en estado `Accepted`.

Puedes utilizar la versión de .NET que desees.

## Servicio de Facturación Electrónica

La carpeta `InvoiceServiceMock` contiene un servicio simulado de facturación electrónica.
El servicio permite registrar y consultar facturas, y expone los siguientes endpoints:

- `GET /api/invoices`. Lista todas las facturas, ordenadas de forma descendente por la
  fecha de creación.
- `POST /api/invoices`. Crea una nueva factura en estado "Pending". El endpoint acepta
  un JSON con los detalles de la factura y devuelve un estado `202 Accepted` junto con
  un CUF (código único de factura) para seguimiento.
- `GET /api/invoices/{cuf}`. Devuelve los detalles de una factura específica por CUF.
  Las facturas cambian a estado "Accepted" 5 segundos después de su creación.
- `GET /api/invoices/{cuf}/pdf`. Devuelve un archivo PDF con la factura especificada por
  el CUF, si y si sólo si la factura ya se encuentra en estado "Accepted".
 
El servicio está escrito en C# con .NET 8, ASP.NET Core, EF Core y SQLite. Para
ejecutarlo, primero instala [.NET 8](https://dotnet.microsoft.com/en-us/download) y
luego ejecuta en una terminal:

```shell
dotnet restore
dotnet build
dotnet run --project InvoiceServiceMock --urls=http://localhost:5050/
```

El servicio estará escuchando localmente en el puerto 5050.

Una vez iniciado el servicio, puedes acceder a la UI de Swagger para explorar y probar
los endpoints de manera interactiva: http://localhost:5050/swagger. Alternativamente,
también puedes utilizar herramientas como Postman o `curl`.

## Criterios de Evaluación

- **Funcionalidad.** Cumplimiento de los requisitos y consideraciones descritos.
- **Interacción con la API.** Integración efectiva con los endpoints del servicio de facturación.
- **Calidad del Código.** Organización, claridad y uso de buenas prácticas.
- **Robustez.** Manejo adecuado de errores y validación de entradas.
