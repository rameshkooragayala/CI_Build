using System;

namespace IEX.Tests.Reflections
{
    [AttributeUsage(AttributeTargets.Method)]
    public class CreateStructureAttribute : Attribute
    {
        public CreateStructureAttribute()
        {
        }
    }
}
