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
    
    public partial class AssignPollingAgent
    {
        public long AssignPollingAgentId { get; set; }
        public long AgentId { get; set; }
        public long PollId { get; set; }
    
        public virtual TblUser TblUser { get; set; }
        public virtual Poll Poll { get; set; }
    }
}
