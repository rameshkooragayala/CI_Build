using System;

namespace IEX.Tests.Reflections
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class TestAttribute : Attribute
    {
        public string Name { get; set; }

        public TestAttribute(string name)
        {
            Name = name;
        }
    }
}
