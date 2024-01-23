using System.ComponentModel.DataAnnotations;

namespace MagicVilla.Modelos.Dto
{
    public class VillaCreateDto
    {

        [Required(ErrorMessage = "Nombre es Requerido")]
        [MaxLength(30)]
        public string Nombre { get; set; }

        public string Detalle { get; set; }

        [Required(ErrorMessage = "Tarifa es Requerida")]
        public double Tarifa { get; set; }

        public int Ocupantes { get; set; }

        public int MetrosCuadrados { get; set; }

        public string ImageUrl { get; set; }

        public string Amenidad { get; set; }

    }
}
