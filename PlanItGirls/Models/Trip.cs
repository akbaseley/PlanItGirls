//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace PlanItGirls.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Trip
    {
        public string TripID { get; set; }
        public string UserID { get; set; }
        public int Price { get; set; }
        public string StartCity { get; set; }
        public string StartState { get; set; }
        public string EndCity { get; set; }
        public string EndState { get; set; }
        public System.DateTime StartDate { get; set; }
        public System.DateTime EndDate { get; set; }
        public string Food { get; set; }
        public string Entertainment { get; set; }
        public string Lodging { get; set; }
    
        public virtual AspNetUser AspNetUser { get; set; }
        public virtual Entertainment Entertainment1 { get; set; }
        public virtual Food Food1 { get; set; }
        public virtual Lodge Lodge { get; set; }
    }
}
