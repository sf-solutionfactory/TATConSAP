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
    
    public partial class CAMPOZKE24
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public CAMPOZKE24()
        {
            this.CAMPOZKE24T = new HashSet<CAMPOZKE24T>();
            this.CONFDIST_CAT = new HashSet<CONFDIST_CAT>();
        }
    
        public string CAMPO { get; set; }
        public string DESCRIPCION { get; set; }
        public Nullable<bool> ACTIVO { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CAMPOZKE24T> CAMPOZKE24T { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CONFDIST_CAT> CONFDIST_CAT { get; set; }
    }
}
