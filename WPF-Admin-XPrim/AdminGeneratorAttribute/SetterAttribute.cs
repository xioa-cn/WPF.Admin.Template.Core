using System;

namespace AdminGeneratorAttribute
{
    [AttributeUsage(AttributeTargets.Field)]
    public class SetterAttribute : Attribute
    {
        public SetterAttribute(Level level = Level.PUBLIC)
        {
            Level = level;
        }

        public Level Level { get; set; }
    }

    public enum Level
    {
        PUBLIC,
        PRIVATE,
        INIT,
    }
}