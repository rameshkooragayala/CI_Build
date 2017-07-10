using System;

namespace IEX.Tests.Reflections
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class StepAttribute : Attribute
    {
        public int Number { get; set; }
        public string Description { get; set; }

        public StepAttribute(int number, string description)
        {
            Number = number;
            Description = description;
        }
    }
}
