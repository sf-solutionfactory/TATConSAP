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
    
    public partial class CLIENTEI
    {
        public string VKORG { get; set; }
        public string VTWEG { get; set; }
        public string SPART { get; set; }
        public string KUNNR { get; set; }
        public string MWSKZ { get; set; }
        public bool ACTIVO { get; set; }
    
        public virtual CLIENTE CLIENTE { get; set; }
        public virtual IMPUESTO IMPUESTO { get; set; }
    }
}
