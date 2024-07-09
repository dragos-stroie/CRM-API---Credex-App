using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using static CRM_API___Credex.Common.CRM.Custom_Attributes;

namespace CRM_API___Credex.Common.CRM
{
    public enum TipActiune
    {
        Create = 1,
        Update = 2
    }
    public class CRMEntity
    {
        private static Dictionary<Type, PropertyInfo[]> dictionarListaProprietati = new Dictionary<Type, PropertyInfo[]>();
        //public IDictionary<PropertyInfo, object> dictionarProprietatiSchimbate = new Dictionary<PropertyInfo, object>();
        internal string orgName = string.Empty;
        internal string entityName = string.Empty;
        //private IOrganizationService _service { get { return CRMConnecter.GetServiceForOrganization(orgName); } set {; } }
        private IOrganizationService _online_service { get { return CRMConnecter.GetServiceForOrganization_Online(orgName); } set {; } }
        internal KeyValuePair<string, object> externalKey
        {
            get
            {
                return new KeyValuePair<string, object>(this.GetType().GetProperties().Where(p => p.GetCustomAttribute<PrimaryExternalKeyAttribute>() != null).FirstOrDefault().Name,
                                                        this.GetType().GetProperties().Where(p => p.GetCustomAttribute<PrimaryExternalKeyAttribute>() != null).FirstOrDefault().GetValue(this));
            }
            set
            {
                ;
            }
        }
        internal List<Entity> RetrieveMultiple(string EntityName, string FilterField, object FilterValue)
        {
            ColumnSet columnSet = new ColumnSet();
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
                columnSet.AddColumn(property.GetCustomAttribute<MappedFieldAttribute>().Name);
            }

            QueryByAttribute qEntities = new QueryByAttribute(EntityName);
            qEntities.AddAttributeValue(FilterField, FilterValue);
            qEntities.ColumnSet = columnSet;

            EntityCollection rEntities = _online_service.RetrieveMultiple(qEntities);
            return rEntities.Entities.ToList();
        }
        protected Entity ToCRMEntity(TipActiune TipActiune)
        {
            Entity rEntity = new Entity(this.entityName);
            PropertyInfo[] listaProprietati;
            if (dictionarListaProprietati.TryGetValue(this.GetType(), out listaProprietati) == false)
            {
                dictionarListaProprietati.Add(this.GetType(), this.GetType().GetProperties());
            }
            switch (TipActiune)
            {
                case TipActiune.Create:
                    listaProprietati = dictionarListaProprietati[this.GetType()];
                    foreach (PropertyInfo property in listaProprietati)
                    {
                        if (property.GetCustomAttribute<PrimaryCrmKeyAttribute>() != null ||
                            property.GetCustomAttribute<IgnoreOnCreateAttribute>() != null ||
                            property.GetCustomAttribute<MappedFieldAttribute>() == null)
                            continue;
                        if (property.GetCustomAttribute<UTCDateTimeAttribute>() != null)
                        {
                            if (property.GetValue(this) != null)
                            {
                                DateTime utcTime = DateTime.SpecifyKind((DateTime)property.GetValue(this), DateTimeKind.Utc);
                                rEntity.Attributes.Add(property.GetCustomAttribute<MappedFieldAttribute>().Name, utcTime);
                            }
                            else
                                rEntity.Attributes.Add(property.GetCustomAttribute<MappedFieldAttribute>().Name, null);
                        }
                        else if (property.GetCustomAttribute<OptionSetFromIntAttribute>() != null)
                        {
                            if (property.GetValue(this) != null)
                                rEntity.Attributes.Add(property.GetCustomAttribute<MappedFieldAttribute>().Name, new OptionSetValue((Int32)property.GetValue(this)));
                            else
                                rEntity.Attributes.Add(property.GetCustomAttribute<MappedFieldAttribute>().Name, null);
                        }
                        else if (property.GetCustomAttribute<OptionSetFromStringWithDefaultAttribute>() != null)
                        {
                            if (property.GetValue(this) != null)
                            {
                                Type tipEnumerare = property.GetCustomAttribute<OptionSetFromStringWithDefaultAttribute>().EnumType;
                                if (Enum.IsDefined(tipEnumerare, property.GetValue(this)) == true)
                                {
                                    Int32 valoareSursa = (Int32)Enum.Parse(tipEnumerare, property.GetValue(this).ToString(), true);
                                    rEntity.Attributes.Add(property.GetCustomAttribute<MappedFieldAttribute>().Name, new OptionSetValue(valoareSursa));
                                }
                                else
                                {
                                    rEntity.Attributes.Add(property.GetCustomAttribute<MappedFieldAttribute>().Name, null);
                                }
                            }
                            else
                            {
                                rEntity.Attributes.Add(property.GetCustomAttribute<MappedFieldAttribute>().Name, new OptionSetValue(property.GetCustomAttribute<OptionSetFromStringWithDefaultAttribute>().DefaultForNull));
                            }
                        }
                        else if (property.GetCustomAttribute<BoolFromStringWithDefaultAttribute>() != null)
                        {
                            if (property.GetValue(this) != null)
                            {
                                Type tipEnumerare = property.GetCustomAttribute<BoolFromStringWithDefaultAttribute>().EnumType;
                                if (Enum.IsDefined(tipEnumerare, property.GetValue(this)) == true)
                                {
                                    Int32 valoareSursa = (Int32)Enum.Parse(tipEnumerare, property.GetValue(this).ToString(), true);
                                    rEntity.Attributes.Add(property.GetCustomAttribute<MappedFieldAttribute>().Name, Convert.ToBoolean(valoareSursa));
                                }
                                else
                                {
                                    rEntity.Attributes.Add(property.GetCustomAttribute<MappedFieldAttribute>().Name, null);
                                }
                            }
                            else
                            {
                                rEntity.Attributes.Add(property.GetCustomAttribute<MappedFieldAttribute>().Name, Convert.ToBoolean(property.GetCustomAttribute<BoolFromStringWithDefaultAttribute>().DefaultForNull));
                            }
                        }
                        else if (property.GetCustomAttribute<EntityReferenceAttribute>() != null)
                        {
                            if (property.GetValue(this) == null) continue;
                            else
                            {
                                rEntity.Attributes.Add(property.GetCustomAttribute<MappedFieldAttribute>().Name, new Microsoft.Xrm.Sdk.EntityReference(property.GetCustomAttribute<EntityReferenceAttribute>().entityName, (Guid)property.GetValue(this)));
                            }
                        }
                        else
                        {
                            rEntity.Attributes.Add(property.GetCustomAttribute<MappedFieldAttribute>().Name, property.GetValue(this));
                        }

                    };
                    rEntity.Attributes.Add("sd_source", 4);
                    break;
                //case TipActiune.Update:
                //    rEntity.Id = (Guid)this.guidCRM;
                //    foreach (KeyValuePair<PropertyInfo, object> modifiedProperty in this.GetModifiedProperties())
                //    {
                //        PropertyInfo property = modifiedProperty.Key;
                //        object vProperty = modifiedProperty.Value;
                //        if (property.GetCustomAttribute<UTCDateTimeAttribute>() != null)
                //        {
                //            if (vProperty != null)
                //            {
                //                DateTime utcTime = DateTime.SpecifyKind((DateTime)vProperty, DateTimeKind.Utc);
                //                rEntity.Attributes.Add(property.Name, utcTime);
                //            }
                //            else
                //                rEntity.Attributes.Add(property.Name, null);
                //        }
                //        else if (property.GetCustomAttribute<OptionSetFromIntAttribute>() != null)
                //        {
                //            if (property.GetValue(this) != null)
                //                rEntity.Attributes.Add(property.GetCustomAttribute<BdImportAttribute>().Name, new Microsoft.Xrm.Sdk.OptionSetValue((Int32)property.GetValue(this)));
                //            else
                //                rEntity.Attributes.Add(property.GetCustomAttribute<BdImportAttribute>().Name, null);
                //        }
                //        else if (property.GetCustomAttribute<OptionSetFromStringWithDefaultAttribute>() != null)
                //        {
                //            if (property.GetValue(this) != null)
                //            {
                //                Type tipEnumerare = property.GetCustomAttribute<OptionSetFromStringWithDefaultAttribute>().EnumType;
                //                if (Enum.IsDefined(tipEnumerare, property.GetValue(this)) == true)
                //                {
                //                    Int32 valoareSursa = (Int32)Enum.Parse(tipEnumerare, property.GetValue(this).ToString(), true);
                //                    rEntity.Attributes.Add(property.GetCustomAttribute<BdImportAttribute>().Name, new Microsoft.Xrm.Sdk.OptionSetValue(valoareSursa));
                //                }
                //                else
                //                {
                //                    rEntity.Attributes.Add(property.GetCustomAttribute<BdImportAttribute>().Name, null);
                //                }
                //            }
                //            else
                //            {
                //                rEntity.Attributes.Add(property.GetCustomAttribute<BdImportAttribute>().Name, new Microsoft.Xrm.Sdk.OptionSetValue(property.GetCustomAttribute<OptionSetFromStringWithDefaultAttribute>().DefaultForNull));
                //            }
                //        }
                //        else if (property.GetCustomAttribute<BoolFromStringWithDefaultAttribute>() != null)
                //        {
                //            if (property.GetValue(this) != null)
                //            {
                //                Type tipEnumerare = property.GetCustomAttribute<BoolFromStringWithDefaultAttribute>().EnumType;
                //                if (Enum.IsDefined(tipEnumerare, property.GetValue(this)) == true)
                //                {
                //                    Int32 valoareSursa = (Int32)Enum.Parse(tipEnumerare, property.GetValue(this).ToString(), true);
                //                    rEntity.Attributes.Add(property.GetCustomAttribute<BdImportAttribute>().Name, Convert.ToBoolean(valoareSursa));
                //                }
                //                else
                //                {
                //                    rEntity.Attributes.Add(property.GetCustomAttribute<BdImportAttribute>().Name, null);
                //                }
                //            }
                //            else
                //            {
                //                rEntity.Attributes.Add(property.GetCustomAttribute<BdImportAttribute>().Name, Convert.ToBoolean(property.GetCustomAttribute<BoolFromStringWithDefaultAttribute>().DefaultForNull));
                //            }
                //        }
                //        else if (property.GetCustomAttribute<EntityReferenceAttribute>() != null)
                //            if (vProperty != null)
                //                rEntity.Attributes.Add(property.GetCustomAttribute<BdImportAttribute>().Name, new Microsoft.Xrm.Sdk.EntityReference(property.GetCustomAttribute<EntityReferenceAttribute>().entityName, (Guid)vProperty));
                //            else
                //                rEntity.Attributes.Add(property.GetCustomAttribute<BdImportAttribute>().Name, null);
                //        else
                //            rEntity.Attributes.Add(property.GetCustomAttribute<BdImportAttribute>().Name, vProperty);

                //    }
                //    break;
                default:
                    break;
            }
            return rEntity;
        }
        internal Guid? CommitToCRM(TipActiune TipActiune)
        {
            if (TipActiune == TipActiune.Create)
            {
                if (this.IsAlreadyImported())
                    return null;
                else
                {
                    return _online_service.Create(this.ToCRMEntity(TipActiune.Create));
                    //return _service.Create(this.ToCRMEntity(TipActiune.Create));
                }
            }
            //else if (TipActiune == TipActiune.Update)
            //{
            //    if (copieLead.guidCRM == Guid.Empty)
            //        return Guid.Empty;
            //    else
            //    {
            //        this.RetrieveDeterminedFieldsFromCRM(_service);
            //        this.ComparaLead(copieLead);
            //        if (this.GetModifiedProperties(_service) != null && this.GetModifiedProperties(_service).Count > 0)
            //            _service.Update(this.ToCRMEntity(TipActiune.Update));
            //        return this.RetrieveFromCRM(_service).guidCRM;
            //    }
            //}
            else return null;
        }
        protected bool IsAlreadyImported()
        {
            PropertyInfo[] listaProprietati = this.GetType().GetProperties().Where(p => p.GetCustomAttribute<MappedFieldAttribute>() != null).ToArray();

            if (this.GetType().GetProperties().Where(p => p.GetCustomAttribute<PrimaryExternalKeyAttribute>() != null).Count() == 0)
                return false;

            QueryByAttribute qT = new QueryByAttribute(this.entityName);
            string primaryAttributeName = this.GetType().GetProperties().Where(p => p.GetCustomAttribute<PrimaryExternalKeyAttribute>() != null).FirstOrDefault().GetCustomAttribute<MappedFieldAttribute>().Name;
            object primaryAttributeValue = this.GetType().GetProperties().Where(p => p.GetCustomAttribute<PrimaryExternalKeyAttribute>() != null).FirstOrDefault().GetValue(this);
            qT.AddAttributeValue(primaryAttributeName, primaryAttributeValue);
            qT.ColumnSet = new ColumnSet();

            EntityCollection rT = _online_service.RetrieveMultiple(qT);
            if (rT.Entities.Count > 0)
                return true;
            else
                return false;

            //foreach (PropertyInfo property in listaProprietati)
            //{
            //    if (property.GetCustomAttribute<IgnoreOnCreateAttribute>() != null ||
            //        property.GetCustomAttribute<BdImportAttribute>() == null) continue;
            //    qT.ColumnSet.AddColumn(property.GetCustomAttribute<BdImportAttribute>().Name);
            //}
            //EntityCollection rEntities = _service.RetrieveMultiple(qT);
            //if (rEntities.Entities.Count > 0)
            //{
            //    Entity rEntity = rEntities[0];
            //    foreach (PropertyInfo property in listaProprietati)
            //    {
            //        string attributeName = property.GetCustomAttribute<BdImportAttribute>().Name;
            //        if (rEntity.Contains(attributeName))
            //        {
            //            TypeSwitch TypeSwitch =
            //                new TypeSwitch()
            //                    .Case((int x) => property.SetValue(this, x))
            //                    .Case((double x) => property.SetValue(this, x))
            //                    .Case((bool x) => property.SetValue(this, x))
            //                    .Case((string x) => property.SetValue(this, x))
            //                    .Case((Guid x) => property.SetValue(this, x))
            //                    .Case((DateTime x) => property.SetValue(this, x))
            //                    .Case((OptionSetValue x) => property.SetValue(this, rEntity.FormattedValues[attributeName]))
            //                    .Case((EntityReference x) => property.SetValue(this, x.Name));
            //            TypeSwitch.Switch(rEntity[attributeName]);
            //        }
            //    }
            //    this.RetrieveDeterminedFieldsFromCRM(_service);
            //    return T;
            //}
        }

        private void RetrieveDeterminedFieldsFromCRM(IOrganizationService _service)
        {
            PropertyInfo[] listaProprietatiDeterminate = this.GetType().GetProperties().Where(p => p.GetCustomAttribute<DeterminedByOtherProperty>() != null).ToArray();
            foreach (PropertyInfo property in listaProprietatiDeterminate)
            {

                string numeProprietateDeterminanta = property.GetCustomAttribute<DeterminedByOtherProperty>().propertyName;
                PropertyInfo proprietateDeterminata = this.GetType().GetProperty(numeProprietateDeterminanta);
                object vProprietateDeterminata = proprietateDeterminata.GetValue(this);

                if (vProprietateDeterminata == null) continue;

                QueryByAttribute qField = new QueryByAttribute(property.GetCustomAttribute<DeterminedByOtherProperty>().searchedEntity);
                qField.AddAttributeValue(property.GetCustomAttribute<DeterminedByOtherProperty>().searchedField, vProprietateDeterminata);
                qField.ColumnSet = new ColumnSet(property.GetCustomAttribute<DeterminedByOtherProperty>().valueField);

                EntityCollection rEntities = _service.RetrieveMultiple(qField);
                if (rEntities.Entities.Count > 0)
                {
                    Entity rEntity = rEntities[0];
                    string attributeName = property.GetCustomAttribute<DeterminedByOtherProperty>().valueField;
                    if (rEntity.Contains(attributeName))
                    {
                        TypeSwitch TypeSwitch =
                            new TypeSwitch()
                                .Case((int x) => property.SetValue(this, x))
                                .Case((double x) => property.SetValue(this, x))
                                .Case((bool x) => property.SetValue(this, x))
                                .Case((string x) => property.SetValue(this, x))
                                .Case((Guid x) => property.SetValue(this, x))
                                .Case((DateTime x) => property.SetValue(this, x))
                                .Case((OptionSetValue x) => property.SetValue(this, rEntity.FormattedValues[attributeName]))
                                .Case((EntityReference x) => property.SetValue(this, x.Name));
                        TypeSwitch.Switch(rEntity[attributeName]);
                    }
                }
            }
        }

        //private void ComparaLead(Lead copieLead)
        //{
        //    PropertyInfo[] listaProprietati = this.GetType().GetProperties().Where(p => p.GetCustomAttribute<BdImportAttribute>() != null &&
        //                    p.GetCustomAttribute<BdImportAttribute>().TrackedForChange == true).ToArray();

        //    //Lead leadDeActualizat = new Lead();
        //    //leadDeActualizat.guidCRM = copieLead.guidCRM;
        //    foreach (PropertyInfo property in listaProprietati)
        //    {
        //        object updatedValue = property.GetValue(this);
        //        object currentValue = property.GetValue(copieLead);
        //        if (updatedValue == null && currentValue == null) continue;
        //        if (updatedValue == null || currentValue == null || !updatedValue.Equals(currentValue))
        //        {
        //            dictionarProprietatiSchimbate.Add(new KeyValuePair<PropertyInfo, object>(property, updatedValue));
        //        }
        //    }
        //}

        internal bool EntityInstanceExists(string logicalName, string fieldName, string fieldValue)
        {
            QueryByAttribute qEntityInstance = new QueryByAttribute(logicalName);
            qEntityInstance.AddAttributeValue(fieldName, fieldValue);
            qEntityInstance.ColumnSet = new ColumnSet(false);

            EntityCollection rEntityInstance = _online_service.RetrieveMultiple(qEntityInstance);

            if (rEntityInstance.Entities.Count > 0)
                return true;
            else
                return false; 
        }
        internal Guid GetEntityId(string logicalName, string fieldName, string fieldValue)
        {
            QueryByAttribute qEntityInstance = new QueryByAttribute(logicalName);
            qEntityInstance.AddAttributeValue(fieldName, fieldValue);
            qEntityInstance.ColumnSet = new ColumnSet(false);

            EntityCollection rEntityInstance = _online_service.RetrieveMultiple(qEntityInstance);

            if (rEntityInstance.Entities.Count > 0)
                return rEntityInstance.Entities[0].Id;
            else
                return Guid.Empty;
        }
    }
}