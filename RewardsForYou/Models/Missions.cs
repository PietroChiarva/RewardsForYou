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
    
    public partial class Missions
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Missions()
        {
            this.NoticeMissionEnded = new HashSet<NoticeMissionEnded>();
        }
    
        public int MissionID { get; set; }
        public int UserID { get; set; }
        public int TaskID { get; set; }
        public System.DateTime StartDate { get; set; }
        public Nullable<System.DateTime> EndDate { get; set; }
        public string Note { get; set; }
        public int Status { get; set; }
        public System.DateTime DesiredEndDate { get; set; }
    
        public virtual Tasks Tasks { get; set; }
        public virtual Users Users { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<NoticeMissionEnded> NoticeMissionEnded { get; set; }
    }
}
