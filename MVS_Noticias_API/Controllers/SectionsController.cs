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
            var sections = new List<dynamic>
            {
                new
                {
                    Id = 751,
                    Section = "Nacional",
                    Subsection = new List<dynamic>
                    {
                        new { Id = 762, Section = "Cdmx" },
                        new { Id = 763, Section = "Estados" },
                        new { Id = 764, Section = "Policiaca" }
                    }
                },
                new
                {
                    Id = 763,
                    Section = "Estados",
                    Subsection = new List<dynamic>
                    {
                        new { Id = 1779, Section = "Nuevo León" }
                    }
                },
                new { Id = 752, Section = "Mundo", Subsection = new List<dynamic>() },
                new { Id = 0,   Section = "Podcast", Subsection = new List<dynamic>() },
                new { Id = 753, Section = "Economía", Subsection = new List<dynamic>() },
                new { Id = 754, Section = "Entretenimiento", Subsection = new List<dynamic>() },
                new
                {
                    Id = 755,
                    Section = "Tendencias",
                    Subsection = new List<dynamic>
                    {
                        new { Id = 766, Section = "Viral" },
                        new { Id = 767, Section = "Salud y Bienestar" },
                        new { Id = 768, Section = "Ciencia y Tecnología" },
                        new { Id = 769, Section = "Mascotas" }
                    }
                },
                new { Id = 756, Section = "Opinión", Subsection = new List<dynamic>() },
                new { Id = 758, Section = "Entrevistas", Subsection = new List<dynamic>() },
                new { Id = 759, Section = "Video", Subsection = new List<dynamic>() },
                new { Id = 760, Section = "Deportes", Subsection = new List<dynamic>() },
                new { Id = 0,   Section = "Programación", Subsection = new List<dynamic>() }
            };

            if (includeSubsections)
            {
                return Ok(new { Sections = sections });
            }
            else
            {
                var flatList = new List<dynamic>();

                foreach (var section in sections)
                {
                    flatList.Add(new { Id = section.Id, Section = section.Section });
                    foreach (var sub in section.Subsection)
                    {
                        flatList.Add(new { Id = sub.Id, Section = sub.Section });
                    }
                }

                return Ok(flatList);
            }
        }

    }
}
