using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CRM_API___Credex.Common.CRM
{
    internal class Custom_Attributes
    {
        internal class MappedFieldAttribute : Attribute
        {
            public string Name { get; private set; }
            public bool TrackedForChange { get; private set; }
            public MappedFieldAttribute(string name)
            {
                this.Name = name;
                this.TrackedForChange = true;
            }
            public MappedFieldAttribute(string name, bool trackedforchange)
            {
                this.Name = name;
                this.TrackedForChange = trackedforchange;
            }
        }
        internal class PrimaryExternalKeyAttribute : Attribute { }
        internal class PrimaryCrmKeyAttribute : Attribute { }
        internal class OptionSetFromIntAttribute : Attribute { }
        internal class OptionSetFromStringWithDefaultAttribute : Attribute
        {
            public Int32 DefaultForNull { get; private set; }
            public Type EnumType { get; private set; }
            public OptionSetFromStringWithDefaultAttribute(Int32 _defaultForNull, Type _enumType)
            {
                this.EnumType = _enumType;
                this.DefaultForNull = _defaultForNull;
            }
        }
        internal class BoolFromStringWithDefaultAttribute : Attribute
        {
            public Int32 DefaultForNull { get; private set; }
            public Type EnumType { get; private set; }
            public BoolFromStringWithDefaultAttribute(Int32 _defaultForNull, Type _enumType)
            {
                this.EnumType = _enumType;
                this.DefaultForNull = _defaultForNull;
            }
        }
        internal class UTCDateTimeAttribute : Attribute { }
        internal class ConvertToTimeZone : Attribute
        {
            public TimeZoneInfo timeZone { get; private set; }
            public ConvertToTimeZone(string timeZoneId)
            {
                this.timeZone = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
            }
        }
        internal class EntityReferenceAttribute : Attribute
        {
            public string entityName { get; private set; }
            public EntityReferenceAttribute(string entityname)
            {
                this.entityName = entityname;
            }
        }
        internal class EntityNameAttribute : Attribute
        {
            public string entityName { get; private set; }
            public EntityNameAttribute(string _entityName)
            {
                this.entityName = _entityName;
            }
        }
        internal class IgnoreOnCreateAttribute : Attribute
        {



        }
        internal class DeterminedByOtherProperty : Attribute
        {
            public string propertyName { get; private set; }
            public string searchedEntity { get; private set; }
            public string searchedField { get; private set; }
            public string valueField { get; private set; }
            public DeterminedByOtherProperty(string _propertyName, string _matchedToEntity, string _matchedToField, string _valueField)
            {
                this.propertyName = _propertyName;
                this.searchedEntity = _matchedToEntity;
                this.searchedField = _matchedToField;
                this.valueField = _valueField;
            }
        }
        internal class TypeSwitch
        {
            Dictionary<Type, Action<object>> matches = new Dictionary<Type, Action<object>>();
            public TypeSwitch Case<T>(Action<T> action) { matches.Add(typeof(T), (x) => action((T)x)); return this; }
            public void Switch(object x) { matches[x.GetType()](x); }
        }

        internal class OptionSetValueAttribute : Attribute { }

        internal class EntityReferenceValue : Attribute { }

        internal class MoneyValue : Attribute { }

        internal class IntAttribute : Attribute { }
    }
}