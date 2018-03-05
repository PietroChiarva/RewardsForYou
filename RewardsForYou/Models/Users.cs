//------------------------------------------------------------------------------
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
    using System.Collections.Generic;
    
    public partial class Users
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Users()
        {
            this.UsersRewards = new HashSet<UsersRewards>();
            this.Missions = new HashSet<Missions>();
            this.Users11 = new HashSet<Users>();
        }
    
        public int UserID { get; set; }
        public string Serial { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string EMail { get; set; }
        public int RoleID { get; set; }
        public Nullable<int> ManagerUserID { get; set; }
        public Nullable<int> UserPoints { get; set; }
        public Nullable<System.DateTime> FiredDate { get; set; }
    
        public virtual Roles Roles { get; set; }
        public virtual Users Users1 { get; set; }
        public virtual Users Users2 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<UsersRewards> UsersRewards { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Missions> Missions { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Users> Users11 { get; set; }
        public virtual Users Users21 { get; set; }
    }
}
