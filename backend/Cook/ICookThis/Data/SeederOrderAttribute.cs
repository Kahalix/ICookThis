using System;

namespace ICookThis.Data
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class SeederOrderAttribute : Attribute
    {
        public int Order { get; }
        public SeederOrderAttribute(int order) => Order = order;
    }
}