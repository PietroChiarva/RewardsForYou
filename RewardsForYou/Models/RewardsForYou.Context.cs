﻿//------------------------------------------------------------------------------
// <auto-generated>
//     Codice generato da un modello.
//
//     Le modifiche manuali a questo file potrebbero causare un comportamento imprevisto dell'applicazione.
//     Se il codice viene rigenerato, le modifiche manuali al file verranno sovrascritte.
// </auto-generated>
//------------------------------------------------------------------------------

namespace RewardsForYou.Models
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class RewardsForYouEntities : DbContext
    {
        public RewardsForYouEntities()
            : base("name=RewardsForYouEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<Missions> Missions { get; set; }
        public virtual DbSet<Rewards> Rewards { get; set; }
        public virtual DbSet<Roles> Roles { get; set; }
        public virtual DbSet<Tasks> Tasks { get; set; }
        public virtual DbSet<Users> Users { get; set; }
        public virtual DbSet<UsersRewards> UsersRewards { get; set; }
        public virtual DbSet<NoticeMissionEnded> NoticeMissionEnded { get; set; }
    }
}
