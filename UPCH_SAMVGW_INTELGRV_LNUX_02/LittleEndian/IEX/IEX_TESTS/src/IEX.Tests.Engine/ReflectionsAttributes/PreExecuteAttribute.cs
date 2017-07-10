using System;

namespace IEX.Tests.Reflections
{
    [AttributeUsage(AttributeTargets.Method)]
    public class PreExecuteAttribute : Attribute
    {
        public string Description { get; private set; }

        public PreExecuteAttribute(string description = "Test Pre Execute")
        {
            Description = description;
        }
    }
}