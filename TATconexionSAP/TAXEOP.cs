//------------------------------------------------------------------------------
// <auto-generated>
//     Este código se generó a partir de una plantilla.
//
//     Los cambios manuales en este archivo pueden causar un comportamiento inesperado de la aplicación.
//     Los cambios manuales en este archivo se sobrescribirán si se regenera el código.
// </auto-generated>
//------------------------------------------------------------------------------

namespace TATconexionSAP
{
    using System;
    using System.Collections.Generic;
    
    public partial class TAXEOP
    {
        public string SOCIEDAD_ID { get; set; }
        public string PAIS_ID { get; set; }
        public string VKORG { get; set; }
        public string VTWEG { get; set; }
        public string SPART { get; set; }
        public string KUNNR { get; set; }
        public int CONCEPTO_ID { get; set; }
        public int POS { get; set; }
        public Nullable<int> RETENCION_ID { get; set; }
        public Nullable<decimal> PORC { get; set; }
        public bool ACTIVO { get; set; }
        public string TRETENCION_ID { get; set; }
    
        public virtual RETENCION RETENCION { get; set; }
        public virtual TAXEOH TAXEOH { get; set; }
        public virtual TRETENCION TRETENCION { get; set; }
    }
}
