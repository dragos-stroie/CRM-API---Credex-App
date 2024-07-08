using CRM_API___Credex.Common.CRM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static CRM_API___Credex.Common.CRM.Custom_Attributes;

namespace CRM_API___Credex.Models
{
    public class Ticket_Post : CRMEntity
    {
        [MappedField("sd_mobileapp_ticketid")]
        [PrimaryExternalKeyAttribute]
        public string MobileApp_TicketId { get; set; }

        [MappedField("sd_contractno")]
        public string ContractNo { get; set; }

       // [MappedField("sd_contractno")]
        //[DeterminedByOtherProperty("ContractNo", "new_contract", "new_name", "new_contractid")]
        //[EntityReference("new_contract")]
        //public Guid ContractId { get; set; }
        
        [MappedField("sd_request_type")]
        //[OptionSetFromInt]
        public int TicketType { get; set; }

        [MappedField("sd_comments")]
        public string Comments { get; set; }

        [MappedField("sd_rap_option")]
        //[OptionSetFromInt]
        public int? RapOption { get; set; }

        [MappedField("sd_new_due_day")]
        public int? NewDueDay { get; set; }

        //[MappedField("sd_recordid")]
        //public int TicketNo { get; set; }

        internal Ticket_Post()
        {
            this.orgName = "CollectionCredex";
            this.entityName = "sd_ticketforcreate"; 
        }
    }
}