//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace JcoDAL
{
    using System;
    using System.Collections.Generic;
    
    public partial class ManagerdistributeAmount
    {
        public long ID { get; set; }
        public Nullable<long> AgentId { get; set; }
        public Nullable<long> ManagerId { get; set; }
        public Nullable<decimal> Amount { get; set; }
        public System.DateTime Date { get; set; }
        public bool IsActive { get; set; }
    
        public virtual TblUser TblUser { get; set; }
        public virtual TblUser TblUser1 { get; set; }
    }
}