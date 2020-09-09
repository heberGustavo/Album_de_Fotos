using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AlbumDeFotos.Models
{
    public class Imagem
    {
        public int ImagemId { get; set; }

        [Required(ErrorMessage = "Campo Obrigatório!")]
        public string Link { get; set; }

        public int AlbumId { get; set; }
        public Album Album { get; set; }
    }
}
