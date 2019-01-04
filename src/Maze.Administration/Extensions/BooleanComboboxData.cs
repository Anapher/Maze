using System;
using System.Collections.Generic;

namespace Maze.Administration.Extensions
{
    public static class BooleanComboboxData
    {
        public static List<BooleanEntity> Entities =>
            new List<BooleanEntity>
            {
                new BooleanEntity {Label = bool.TrueString, Value = true}, new BooleanEntity {Label = bool.FalseString, Value = false}
            };

        public class BooleanEntity
        {
            public string Label { get; set; }
            public bool Value { get; set; }
        }
    }
}