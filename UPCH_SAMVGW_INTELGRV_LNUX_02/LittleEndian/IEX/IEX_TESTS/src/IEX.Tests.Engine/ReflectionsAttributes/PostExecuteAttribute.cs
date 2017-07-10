using System;

namespace IEX.Tests.Reflections
{
    [AttributeUsage(AttributeTargets.Method)]
    public class PostExecuteAttribute : Attribute
    {
        public string Description { get; private set; }

        public PostExecuteAttribute(string description = "Test Post Execute")
        {
            Description = description;
        }
    }
}
