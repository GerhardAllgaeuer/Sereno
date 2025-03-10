using Microsoft.AspNetCore.Mvc;
using Sereno.Documentation.Api.Models;

namespace Sereno.Documentation.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DocumentationController : ControllerBase
    {
        private static readonly List<Models.Documentation> _dummyDocumentations = new()
        {
            new Models.Documentation
            {
                Id = 1,
                Title = "Angular Komponenten",
                Content = "Angular Komponenten sind die grundlegenden Bausteine einer Angular-Anwendung. Sie bestehen aus einem Template, einer Klasse und Metadaten. Das Template definiert die Benutzeroberfläche, die Klasse enthält die Logik und die Metadaten stellen zusätzliche Informationen bereit.",
                Topic = "Angular",
                CreatedAt = new DateTime(2023, 1, 15),
                UpdatedAt = new DateTime(2023, 2, 20)
            },
            new Models.Documentation
            {
                Id = 2,
                Title = "TypeScript Grundlagen",
                Content = "TypeScript ist eine von Microsoft entwickelte Programmiersprache, die auf JavaScript aufbaut und es um statische Typisierung erweitert. TypeScript-Code wird zu JavaScript kompiliert und kann in jedem Browser oder JavaScript-Laufzeitumgebung ausgeführt werden.",
                Topic = "TypeScript",
                CreatedAt = new DateTime(2023, 3, 10),
                UpdatedAt = new DateTime(2023, 3, 15)
            },
            new Models.Documentation
            {
                Id = 3,
                Title = "CSS Flexbox Layout",
                Content = "Flexbox ist ein CSS-Layout-Modell, das es ermöglicht, Elemente in einer flexiblen und responsiven Weise anzuordnen. Es bietet eine einfache Möglichkeit, Elemente horizontal oder vertikal auszurichten und den verfügbaren Platz zwischen ihnen zu verteilen.",
                Topic = "CSS",
                CreatedAt = new DateTime(2023, 4, 5),
                UpdatedAt = new DateTime(2023, 4, 10)
            },
            new Models.Documentation
            {
                Id = 4,
                Title = "Angular Routing",
                Content = "Angular Routing ermöglicht die Navigation zwischen verschiedenen Ansichten in einer Single-Page-Application. Es unterstützt die Verwendung von URL-Segmenten, Query-Parametern und Route-Guards für die Zugriffskontrolle.",
                Topic = "Angular",
                CreatedAt = new DateTime(2023, 5, 20),
                UpdatedAt = new DateTime(2023, 5, 25)
            },
            new Models.Documentation
            {
                Id = 5,
                Title = "JavaScript Promises",
                Content = "Promises in JavaScript sind Objekte, die den eventuellen Abschluss oder Fehler einer asynchronen Operation repräsentieren. Sie ermöglichen es, asynchronen Code auf eine sauberere Weise zu schreiben als mit Callbacks.",
                Topic = "JavaScript",
                CreatedAt = new DateTime(2023, 6, 15),
                UpdatedAt = new DateTime(2023, 6, 20)
            }
        };

        [HttpGet]
        public ActionResult<IEnumerable<Models.Documentation>> GetAll()
        {
            return Ok(_dummyDocumentations);
        }

        [HttpGet("{id}")]
        public ActionResult<Models.Documentation> GetById(int id)
        {
            var documentation = _dummyDocumentations.FirstOrDefault(d => d.Id == id);
            if (documentation == null)
            {
                return NotFound();
            }
            return Ok(documentation);
        }

        [HttpGet("topic/{topic}")]
        public ActionResult<IEnumerable<Models.Documentation>> GetByTopic(string topic)
        {
            var documentations = _dummyDocumentations
                .Where(d => d.Topic.ToLower() == topic.ToLower())
                .ToList();
            
            return Ok(documentations);
        }

        [HttpGet("search")]
        public ActionResult<IEnumerable<Models.Documentation>> Search([FromQuery] string query)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return Ok(new List<Models.Documentation>());
            }

            var lowercaseQuery = query.ToLower();
            var documentations = _dummyDocumentations
                .Where(d => d.Title.ToLower().Contains(lowercaseQuery) || 
                           d.Content.ToLower().Contains(lowercaseQuery))
                .ToList();
            
            return Ok(documentations);
        }
    }
} 