using System.ComponentModel.DataAnnotations;

namespace Ab108Uniqlo.ViewModels.Sliders;

public class SliderCreateVM
{
    [MaxLength(32, ErrorMessage = "Uzunluq maksimum 32 ola biler"), Required]
    public string Title { get; set; }
    [Required]
    public string Subtitle { get; set; }
    public string? Link { get; set; }

    [Required(ErrorMessage = "Fayl duzgun formatta secilmeyib")]
    public IFormFile File { get; set; }
}
