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
    
    public partial class Lodge
    {
        public string Lodging { get; set; }
        public decimal Price { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string PostalCode { get; set; }
        public string PhoneNumber { get; set; }
        public string URL { get; set; }
        public string TripID { get; set; }
    
        public virtual Trip Trip { get; set; }
    }
}