using CRM_API___Credex.Common.CRM;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Web;
using static CRM_API___Credex.Common.CRM.Custom_Attributes;

namespace CRM_API___Credex.Models
{
    public class Ticket_Get : CRMEntity
    {
        [MappedField("sd_recordid")]
        [IntAttribute]
        public int? TicketNo { get; set; }

        [MappedField("sd_contract")]
        [EntityReferenceAttribute("new_contract")]
        public string ContractNo { get; set; }

        [MappedField("sd_request_type")]
        [OptionSetValue]
        public int? TicketType { get; set; }

        [MappedField("sd_statusticket")]
        [OptionSetValue]
        public int? Status { get; set; }

        [MappedField("sd_name")]
        public string Name { get; set; }

        [MappedField("sd_comments")]
        public string Comments { get; set; }

        [MappedField("sd_rap_option")]
        [OptionSetValue]
        public int? RapOption { get; set; }

        [MappedField("sd_new_due_day")]
        [IntAttribute]
        public int? NewDueDay { get; set; }

        private static Dictionary<Type, PropertyInfo[]> dictionarListaProprietati = new Dictionary<Type, PropertyInfo[]>();

        internal Ticket_Get()
        {
            this.orgName = ConfigurationManager.AppSettings["orgName"];
            this.entityName = "sd_ticket";
        }
        internal Ticket_Get(Entity entity)
        {
            PropertyInfo[] listaProprietati;
            if (dictionarListaProprietati.TryGetValue(this.GetType(), out listaProprietati) == false)
            {
                dictionarListaProprietati.Add(this.GetType(), this.GetType().GetProperties());
            }
            listaProprietati = dictionarListaProprietati[this.GetType()];
            foreach (PropertyInfo property in listaProprietati)
            {
                if (property.GetCustomAttribute<MappedFieldAttribute>() == null)
                    continue;
                string fieldName = property.GetCustomAttribute<MappedFieldAttribute>().Name;
                if (!entity.Contains(fieldName))
                    continue;
                if (property.GetCustomAttribute<OptionSetValueAttribute>() != null)
                    property.SetValue(this, entity.GetAttributeValue<OptionSetValue>(fieldName).Value);
                else if (property.GetCustomAttribute<EntityReferenceAttribute>() != null)
                    property.SetValue(this, entity.GetAttributeValue<EntityReference>(fieldName).Name);
                else if (property.GetCustomAttribute<IntAttribute>() != null)
                    property.SetValue(this, entity.GetAttributeValue<int>(fieldName));
                else
                    property.SetValue(this, entity.GetAttributeValue<string>(fieldName));
            }
        }
    }
}