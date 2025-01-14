using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CognitiveServices.Speech;

namespace MVS_Noticias_API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TextToSpeechController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public TextToSpeechController(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        [HttpPost("generate-audio")]
        public async Task<IActionResult> GenerateAudio([FromBody] string text)
        {
            try
            {
                // Obtener las configuraciones desde appsettings.json
                var subscriptionKey = _configuration["AzureSpeech:SubscriptionKey"];
                var region = _configuration["AzureSpeech:Region"];

                // Configuración del servicio de Speech
                var config = SpeechConfig.FromSubscription(subscriptionKey, region);

                // Configurar idioma y voz (ejemplo: español de México)
                config.SpeechSynthesisVoiceName = "es-MX-DaliaNeural";

                // Archivo temporal donde se guardará el audio
                var tempFilePath = Path.Combine(Path.GetTempPath(), "outputAudio.mp3");

                // Generar el audio usando Speech Synthesizer
                using (var synthesizer = new SpeechSynthesizer(config, null))
                {
                    var result = await synthesizer.SpeakTextAsync(text);

                    if (result.Reason == ResultReason.SynthesizingAudioCompleted)
                    {
                        // Guardar el audio en el archivo temporal
                        await System.IO.File.WriteAllBytesAsync(tempFilePath, result.AudioData);
                    }
                    else
                    {
                        return BadRequest($"Error during speech synthesis: {result.Reason}");
                    }
                }

                // Leer el archivo temporal y devolverlo como respuesta
                var audioBytes = await System.IO.File.ReadAllBytesAsync(tempFilePath);

                // Eliminar el archivo temporal
                System.IO.File.Delete(tempFilePath);

                // Devolver el archivo como respuesta
                return File(audioBytes, "audio/wav", "outputAudio.wav");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
