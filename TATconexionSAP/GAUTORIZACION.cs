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
    
    public partial class GAUTORIZACION
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public GAUTORIZACION()
        {
            this.DET_AGENTE = new HashSet<DET_AGENTE>();
            this.DET_AGENTEH = new HashSet<DET_AGENTEH>();
            this.USUARIOs = new HashSet<USUARIO>();
        }
    
        public long ID { get; set; }
        public string CLAVE { get; set; }
        public string NOMBRE { get; set; }
        public string BUKRS { get; set; }
        public string LAND { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DET_AGENTE> DET_AGENTE { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DET_AGENTEH> DET_AGENTEH { get; set; }
        public virtual SOCIEDAD SOCIEDAD { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<USUARIO> USUARIOs { get; set; }
    }
}
