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
    
    public partial class PRESUPSAPH
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public PRESUPSAPH()
        {
            this.PRESUPSAPPs = new HashSet<PRESUPSAPP>();
        }
    
        public int ID { get; set; }
        public int ANIO { get; set; }
        public string USUARIO_ID { get; set; }
        public Nullable<System.DateTime> FECHAC { get; set; }
    
        public virtual USUARIO USUARIO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PRESUPSAPP> PRESUPSAPPs { get; set; }
    }
}
