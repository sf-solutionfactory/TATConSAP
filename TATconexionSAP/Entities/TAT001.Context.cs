﻿//------------------------------------------------------------------------------
// <auto-generated>
//     Este código se generó a partir de una plantilla.
//
//     Los cambios manuales en este archivo pueden causar un comportamiento inesperado de la aplicación.
//     Los cambios manuales en este archivo se sobrescribirán si se regenera el código.
// </auto-generated>
//------------------------------------------------------------------------------

namespace TATconexionSAP.Entities
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class TAT001Entities : DbContext
    {
        public TAT001Entities()
            : base("name=TAT001Entities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<APPSETTING> APPSETTINGs { get; set; }
        public virtual DbSet<CONPOSAPH> CONPOSAPHs { get; set; }
        public virtual DbSet<CONPOSAPP> CONPOSAPPs { get; set; }
        public virtual DbSet<DET_AGENTEC> DET_AGENTEC { get; set; }
        public virtual DbSet<DET_TAXEOC> DET_TAXEOC { get; set; }
        public virtual DbSet<DOCUMENTO> DOCUMENTOes { get; set; }
        public virtual DbSet<DOCUMENTOSAP> DOCUMENTOSAPs { get; set; }
        public virtual DbSet<FLUJO> FLUJOes { get; set; }
        public virtual DbSet<USUARIO> USUARIOs { get; set; }
        public virtual DbSet<WORKFH> WORKFHs { get; set; }
        public virtual DbSet<WORKFP> WORKFPs { get; set; }
        public virtual DbSet<WORKFT> WORKFTs { get; set; }
        public virtual DbSet<WORKFV> WORKFVs { get; set; }
        public virtual DbSet<DELEGAR> DELEGARs { get; set; }
        public virtual DbSet<ACCION> ACCIONs { get; set; }
        public virtual DbSet<DOCUMENTOREC> DOCUMENTORECs { get; set; }
        public virtual DbSet<DET_TAXEO> DET_TAXEO { get; set; }
        public virtual DbSet<TAX_LAND> TAX_LAND { get; set; }
    }
}
