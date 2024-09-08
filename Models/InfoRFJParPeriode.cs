namespace WebApplication2
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("InfoRFJParPeriode")]
    public partial class InfoRFJParPeriode
    {
        public string Id { get; set; }

        [Required]
        [StringLength(128)]
        public string IdUtilisateur { get; set; }

        [Required]
        [StringLength(9)]
        public string Periode { get; set; }
        [Range(0,20)]
        public int NbAcquis { get; set; }
        [Range(-5, 20)]
        public int NbRestant { get; set; }
    }
}
