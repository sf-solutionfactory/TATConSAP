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
    
    public partial class WARNING
    {
        public int PAGINA_ID { get; set; }
        public string CAMPO_ID { get; set; }
        public string SPRAS_ID { get; set; }
        public string WARNING1 { get; set; }
        public string POSICION { get; set; }
        public Nullable<bool> ACTIVO { get; set; }
    
        public virtual CAMPOS CAMPOS { get; set; }
        public virtual POSICION POSICION1 { get; set; }
        public virtual SPRA SPRA { get; set; }
    }
}
