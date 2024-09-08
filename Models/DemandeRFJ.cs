namespace WebApplication2
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("DemandeRFJ")]
    public partial class DemandeRFJ
    {
        public string Id { get; set; }

        [Required]
        [StringLength(128)]
        public string IdUtilisateur { get; set; }
        [Required]
        public DateTime DateDebut { get; set; }
        [Required]
        public DateTime DateFin { get; set; }
        [Required]
        public DateTime DateCreation { get; set; }
        
        [StringLength(200)]
        public string NoteFacultative { get; set; }

        [Required]
        [StringLength(50)]
        public string Etat { get; set; }
    }
}
