using System;

namespace Tasks.Infrastructure.Server.Library
{
    /// <summary>
    ///     An attribute that should be applied to a <see cref="IFilterService{TFilterInfo}"/> to specify the cost of the operation.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class FilterPerformanceCostAttribute : Attribute
    {
        /// <summary>
        /// Initialize a new instance of <see cref="FilterPerformanceCostAttribute"/>
        /// </summary>
        /// <param name="value">
        /// The cost as integer, normally a number between 0 and 10. References:
        /// - Database access (e. g. querying client) = 3
        /// - HTTP request                            = 8
        /// </param>
        public FilterPerformanceCostAttribute(int value)
        {
            Value = value;
        }

        /// <summary>
        /// The cost as integer, normally a number between 0 and 10. References:
        /// - Database access (e. g. querying client) = 3
        /// - HTTP request  
        /// </summary>
        public int Value { get; }
    }
}
