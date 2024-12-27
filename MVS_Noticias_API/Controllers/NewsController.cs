﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MVS_Noticias_API.Data;
using MVS_Noticias_API.Models.News;
using Newtonsoft.Json;
using Ganss.Xss;

namespace MVS_Noticias_API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class NewsController : ControllerBase
    {
        private readonly DataContext _dataContext;
        public readonly IConfiguration _configuration;
        private readonly ILogger _logger;

        public NewsController(IConfiguration configuration, ILogger<NewsController> logger, DataContext dataContext)
        {
            _configuration = configuration;
            _dataContext = dataContext;
            _logger = logger;
        }

        [HttpGet("detailNews")]
        public async Task<ActionResult<List<CompleteNews>>> GetDetailNews(int idNews)
        {
            _logger.LogInformation("Starting getting news formatted details.");

            try
            {
                using var httpClient = new HttpClient();

                var apiEditor80 = _configuration.GetSection("AppSettings:Editor80Api").Value;
                var responseNewsMVS = await httpClient.GetStringAsync($"{apiEditor80}noticias.asp?id_noticia={idNews}&contenido=si");
                var newsData = JsonConvert.DeserializeObject<dynamic>(responseNewsMVS);

                var mostReadNews = new List<CompleteNews>();

                foreach (var news in newsData.Noticias)
                {
                    string content = news.contenido;

                    var mostRead = new CompleteNews
                    {
                        IdNews = news.id_noticia,
                        Title = news.titulo,
                        Description = news.descripcion,
                        Date = news.fecha,
                        Section = news.seccion,
                        SubSection = news.subseccion,
                        IdSection = news.id_seccion,
                        IdSubSection = news.id_subseccion,
                        Url = news.url,
                        Slug = news.slug,
                        Photo = news.foto,
                        PhotoMobile = news.foto_movil,
                        PhotoCredits = news.foto_creditos,
                        PhotoDescription = news.foto_descripcion,
                        Author = news.autor,
                        IdAuthor = news.id_autor,
                        Creator = news.creador,
                        IdCreator = news.id_creador,
                        Content = AddCustomStyles(SanitizeHtmlContent(content)),
                        IsVideo = news.isVideo,
                        VideoUrl = news.videoUrl,
                        IsSound = news.isSound,
                        SoundUrl = news.SoundUrl,
                        Type = news.Tipo,
                        Tags = "",
                        HiddenTags = "",
                        NewsQuantity = 0,
                        Number = news.numero,
                    };
                    mostReadNews.Add(mostRead);
                }

                return Ok(mostReadNews);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error getting saved news: " + ex.Message);
                return BadRequest("Error getting saved news: " + ex.Message);
            }
        }

        private string SanitizeHtmlContent(string htmlContent)
        {
            var sanitizer = new HtmlSanitizer
            {
                AllowedTags = { "p", "strong", "em", "b", "i", "u", "br", "a", "img", "iframe", "ul", "div", "aside" },
                AllowedAttributes = { "href", "src", "alt", "title", "width", "height", "allowfullscreen", "class" }
            };

            return sanitizer.Sanitize(htmlContent);
        }

        private string AddCustomStyles(string sanitizedHtml)
        {
            if (string.IsNullOrWhiteSpace(sanitizedHtml))
                return sanitizedHtml;

            try
            {
                var document = new HtmlAgilityPack.HtmlDocument();
                document.LoadHtml(sanitizedHtml);

                var relatedNewsNodes = document.DocumentNode.SelectNodes("//aside[contains(@class, 'relacionadas')]");
                if (relatedNewsNodes != null)
                {
                    foreach (var aside in relatedNewsNodes)
                    {
                        aside.SetAttributeValue("style", "background-color: #f6f6f6; padding: 10px; margin: 10px 0; border-radius: 5px;");

                        
                        var titleNode = aside.SelectSingleNode(".//strong[contains(@class, 'relacionadas-titulo-gral')]");
                        titleNode?.SetAttributeValue("style", "font-weight: bold; display: block; margin-bottom: 10px;");

                        
                        var ulNode = aside.SelectSingleNode(".//ul");
                        if (ulNode != null)
                        {
                            ulNode.ParentNode.ReplaceChild(HtmlAgilityPack.HtmlNode.CreateNode(ulNode.InnerHtml), ulNode);
                        }

                        var listItems = aside.SelectNodes(".//li");
                        if (listItems != null)
                        {
                            foreach (var li in listItems)
                            {
                                var linkNode = li.SelectSingleNode(".//a");
                                if (linkNode != null)
                                {
                                    linkNode.SetAttributeValue("style", "color: black; font-weight: bold; text-decoration: none; display: flex; align-items: center; margin-bottom: 10px;");

                                    var imgNode = linkNode.SelectSingleNode(".//img");
                                    imgNode?.SetAttributeValue("style", "float: left; margin-right: 10px; width: 171px; height: 96px;");

                                    var titleInLink = linkNode.SelectSingleNode(".//div[contains(@class, 'item-tit')]");
                                    titleInLink?.SetAttributeValue("style", "font-weight: bold; margin-top: 10px;");
                                }

                                li.ParentNode.ReplaceChild(linkNode, li);
                            }
                        }
                    }
                }

                return document.DocumentNode.OuterHtml;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error styling HTML content.");
                return sanitizedHtml;
            }
        }

    }
}