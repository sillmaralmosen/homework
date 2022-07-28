using System.ComponentModel.DataAnnotations;

namespace Notino.Model.Request
{
    public class ConvertXmlToJsonRequest
    {
        [Required(ErrorMessage = "Vyplňte zdroj")]
        public string sourceFileName { get; set; }

        [Required(ErrorMessage = "Vyplňte cíl")]
        public string targetFileName { get; set; }
    }
}
