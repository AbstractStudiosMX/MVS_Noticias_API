using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MVS_Noticias_API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SectionsController : ControllerBase
    {
        [HttpGet("sections")]
        public ActionResult<object> GetSections(bool includeSubsections)
        {
            var sections = new List<Section>
            {
                new Section { Id = 111111, Name = "Última hora" },
                new Section
                {
                    Id = 751,
                    Name = "Nacional",
                    Subsections = new List<Subsection>
                    {
                        new Subsection { Id = 762, Name = "Cdmx" },
                        new Subsection { Id = 763, Name = "Estados" },
                        new Subsection { Id = 764, Name = "Policiaca" }
                    }
                },
                new Section
                {
                    Id = 763,
                    Name = "Estados",
                    Subsections = new List<Subsection>
                    {
                        new Subsection { Id = 1779, Name = "Nuevo León" }
                    }
                },
                new Section { Id = 752, Name = "Mundo" },
                new Section
                {
                    Id = 758451,
                    Name = "Multimedia",
                    Subsections = new List<Subsection>
                    {
                         new Subsection { Id = 655144, Name = "Podcast" },
                         new Subsection { Id = 759, Name = "Video" },
                    }
                },
                new Section { Id = 753, Name = "Economía" },
                new Section { Id = 754, Name = "Entretenimiento" },
                new Section
                {
                    Id = 755,
                    Name = "Tendencias",
                    Subsections = new List<Subsection>
                    {
                        new Subsection { Id = 766, Name = "Viral" },
                        new Subsection { Id = 767, Name = "Salud y Bienestar" },
                        new Subsection { Id = 768, Name = "Ciencia y Tecnología" },
                        new Subsection { Id = 769, Name = "Mascotas" }
                    }
                },
                new Section { Id = 756, Name = "Opinión" },
                new Section { Id = 758, Name = "Entrevistas" },
                new Section { Id = 760, Name = "MVS Deportes" },
                new Section { Id = 788999, Name = "Programación" },
                new Section { Id = 788100, Name = "Guardados" },
                new Section { Id = 788101, Name = "Más leídas" }
            };

            if (includeSubsections)
            {
                return Ok(new { Sections = sections });
            }
            else
            {
                var predefinedFlatList = new List<object>
            {
                new { id = 111111, name = "Última hora" },
                new { id = 758, name = "Entrevistas" },
                new { id = 759, name = "Video" },
                new { id = 751, name = "Nacional" },
                new { id = 762, name = "Cdmx" },
                new { id = 763, name = "Estados" },
                new { id = 1779, name = "Nuevo León" },
                new { id = 764, name = "Policiaca" },
                new { id = 752, name = "Mundo" },
                new { id = 655144, name = "Podcast" },
                new { id = 753, name = "Economía" },
                new { id = 788101, name = "Más leídas" },
                new { id = 754, name = "Entretenimiento" },
                new { id = 755, name = "Tendencias" },
                new { id = 766, name = "Viral" },
                new { id = 767, name = "Salud y Bienestar" },
                new { id = 768, name = "Ciencia y Tecnología" },
                new { id = 769, name = "Mascotas" },
                new { id = 756, name = "Opinión" },
                new { id = 760, name = "MVS Deportes" },
                new { id = 788999, name = "Programación" },
                new { id = 788100, name = "Guardados" },
                new { id = 758451, name = "Multimedia" }
            };

                return Ok(predefinedFlatList);
            }
        }

    }
}

public class Section
{
    public int Id { get; set; }
    public string Name { get; set; }
    public List<Subsection> Subsections { get; set; } = new List<Subsection>();
}

public class Subsection
{
    public int Id { get; set; }
    public string Name { get; set; }
}