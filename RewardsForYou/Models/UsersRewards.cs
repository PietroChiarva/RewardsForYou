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
    
    public partial class UsersRewards
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public UsersRewards()
        {
            this.NoticeRewardsTake = new HashSet<NoticeRewardsTake>();
        }
    
        public int UserRewardsID { get; set; }
        public int UserID { get; set; }
        public int RewardsID { get; set; }
        public System.DateTime RewardsDate { get; set; }
        public string Note { get; set; }
    
        public virtual Rewards Rewards { get; set; }
        public virtual Users Users { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<NoticeRewardsTake> NoticeRewardsTake { get; set; }
    }
}
