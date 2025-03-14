using Microsoft.AspNetCore.Mvc;
using Sereno.Documentation.DataAccess;

namespace Sereno.Documentation.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DocumentationsController : ControllerBase
    {
        private static readonly List<Models.Documentation> _dummyDocumentations = new()
        {
            new Models.Documentation
            {
                Id = "1",
                Title = "Angular Komponenten",
                Content = "Angular Komponenten sind die grundlegenden Bausteine einer Angular-Anwendung. Sie bestehen aus einem Template, einer Klasse und Metadaten. Das Template definiert die Benutzeroberfläche, die Klasse enthält die Logik und die Metadaten stellen zusätzliche Informationen bereit.",
                Topic = "Angular/Components",
                DocumentKey = "angular-components",
                Author = "Max Mustermann",
                CreatedAt = new DateTime(2023, 1, 15),
                CreatedBy = "admin",
                UpdatedAt = new DateTime(2023, 2, 20),
                UpdatedBy = "admin"
            },
            new Models.Documentation
            {
                Id = "2",
                Title = "TypeScript Grundlagen",
                Content = "TypeScript ist eine von Microsoft entwickelte Programmiersprache, die auf JavaScript aufbaut und es um statische Typisierung erweitert. TypeScript-Code wird zu JavaScript kompiliert und kann in jedem Browser oder JavaScript-Laufzeitumgebung ausgeführt werden.",
                Topic = "TypeScript/Basics",
                DocumentKey = "typescript-basics",
                Author = "Erika Musterfrau",
                CreatedAt = new DateTime(2023, 3, 10),
                CreatedBy = "admin",
                UpdatedAt = new DateTime(2023, 3, 15),
                UpdatedBy = "admin"
            },
            new Models.Documentation
            {
                Id = "3",
                Title = "CSS Flexbox Layout",
                Content = "Flexbox ist ein CSS-Layout-Modell, das es ermöglicht, Elemente in einer flexiblen und responsiven Weise anzuordnen. Es bietet eine einfache Möglichkeit, Elemente horizontal oder vertikal auszurichten und den verfügbaren Platz zwischen ihnen zu verteilen.",
                Topic = "CSS/Flexbox",
                DocumentKey = "css-flexbox",
                Author = "John Doe",
                CreatedAt = new DateTime(2023, 4, 5),
                CreatedBy = "editor",
                UpdatedAt = new DateTime(2023, 4, 10),
                UpdatedBy = "editor"
            },
            new Models.Documentation
            {
                Id = "4",
                Title = "Angular Routing",
                Content = "Angular Routing ermöglicht die Navigation zwischen verschiedenen Ansichten in einer Single-Page-Application. Es unterstützt die Verwendung von URL-Segmenten, Query-Parametern und Route-Guards für die Zugriffskontrolle.",
                Topic = "Angular/Routing",
                DocumentKey = "angular-routing",
                Author = "Jane Smith",
                CreatedAt = new DateTime(2023, 5, 20),
                CreatedBy = "editor",
                UpdatedAt = new DateTime(2023, 5, 25),
                UpdatedBy = "admin"
            },
            new Models.Documentation
            {
                Id = "5",
                Title = "JavaScript Promises",
                Content = "Promises in JavaScript sind Objekte, die den eventuellen Abschluss oder Fehler einer asynchronen Operation repräsentieren. Sie ermöglichen es, asynchronen Code auf eine sauberere Weise zu schreiben als mit Callbacks.",
                Topic = "JavaScript/Async",
                DocumentKey = "javascript-promises",
                Author = "Alex Johnson",
                CreatedAt = new DateTime(2023, 6, 15),
                CreatedBy = "contributor",
                UpdatedAt = new DateTime(2023, 6, 20),
                UpdatedBy = "contributor"
            }
        };

        private readonly AppDbContext _dbContext;

        public DocumentationsController(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet]
        public ActionResult<IEnumerable<Models.Documentation>> GetAll()
        {
            var documents = _dbContext.Documents.ToList();

            var documentations = documents.Select(d => new Models.Documentation
            {
                Id = d.Id,
                Title = d.Title,
                Content = d.Content,
                HtmlContent = d.HtmlContent,
                Topic = GetTopicFromLibraryPath(d.LibraryPath),
                DocumentKey = d.DocumentKey,
                Author = d.Author,
                NextCheck = d.NextCheck,
                CreatedAt = d.Create,
                CreatedBy = d.CreateUser,
                UpdatedAt = d.Modify,
                UpdatedBy = d.ModifyUser
            }).ToList();
            
            return Ok(documentations);
        }

        private string GetTopicFromLibraryPath(string libraryPath)
        {
            int index = libraryPath.IndexOf('\\');
            string result = index >= 0 ? libraryPath.Substring(0, index) : libraryPath;

            return result;
        }

        [HttpGet("{id}")]
        public ActionResult<Models.Documentation> GetById(string id)
        {
            var document = _dbContext.Documents.FirstOrDefault(d => d.Id == id);
            if (document == null)
            {
                return NotFound();
            }
            
            var documentation = new Models.Documentation
            {
                Id = document.Id,
                Title = document.Title,
                Content = document.Content,
                HtmlContent = document.HtmlContent,
                Topic = document.LibraryPath,
                DocumentKey = document.DocumentKey,
                Author = document.Author,
                NextCheck = document.NextCheck,
                CreatedAt = document.Create,
                CreatedBy = document.CreateUser,
                UpdatedAt = document.Modify,
                UpdatedBy = document.ModifyUser
            };
            
            return Ok(documentation);
        }

        [HttpGet("topic/{topic}")]
        public ActionResult<IEnumerable<Models.Documentation>> GetByTopic(string topic)
        {
            if (string.IsNullOrWhiteSpace(topic))
            {
                return Ok(new List<Models.Documentation>());
            }

            var documents = _dbContext.Documents
                .Where(d => d.Title != null && d.Title.Contains(topic, StringComparison.CurrentCultureIgnoreCase))
                .ToList();
            
            var documentations = documents.Select(d => new Models.Documentation
            {
                Id = d.Id,
                Title = d.Title,
                Content = d.Content,
                HtmlContent = d.HtmlContent,
                Topic = d.LibraryPath,
                DocumentKey = d.DocumentKey,
                Author = d.Author,
                NextCheck = d.NextCheck,
                CreatedAt = d.Create,
                CreatedBy = d.CreateUser,
                UpdatedAt = d.Modify,
                UpdatedBy = d.ModifyUser
            }).ToList();
            
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
            var documents = _dbContext.Documents
                .Where(d => d.Title != null && d.Title.Contains(lowercaseQuery, StringComparison.CurrentCultureIgnoreCase) ||
                           d.Content != null && d.Content.Contains(lowercaseQuery, StringComparison.CurrentCultureIgnoreCase))
                .ToList();
            
            var documentations = documents.Select(d => new Models.Documentation
            {
                Id = d.Id,
                Title = d.Title,
                Content = d.Content,
                HtmlContent = d.HtmlContent,
                Topic = d.LibraryPath,
                DocumentKey = d.DocumentKey,
                Author = d.Author,
                NextCheck = d.NextCheck,
                CreatedAt = d.Create,
                CreatedBy = d.CreateUser,
                UpdatedAt = d.Modify,
                UpdatedBy = d.ModifyUser
            }).ToList();
            
            return Ok(documentations);
        }
    }
} 