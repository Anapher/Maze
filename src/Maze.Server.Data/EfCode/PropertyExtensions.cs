using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Maze.Server.Data.EfCode
{
    public static class PropertyExtensions
    {
        public static PropertyBuilder<TProperty> IsCurrentTime<TProperty>(
            this PropertyBuilder<TProperty> propertyBuilder)
        {
            return propertyBuilder.HasDefaultValueSql("CURRENT_TIMESTAMP");
        }

        public static PropertyBuilder<TProperty> IsSha256Hash<TProperty>(
            this PropertyBuilder<TProperty> propertyBuilder)
        {
            return propertyBuilder.IsFixedLengthString(64);
        }

        public static PropertyBuilder<TProperty> IsFixedLengthString<TProperty>(
            this PropertyBuilder<TProperty> propertyBuilder, int length)
        {
            return propertyBuilder.HasColumnType($"char({length})");
        }

        public static PropertyBuilder<TProperty> IsRsaSignature<TProperty>(
            this PropertyBuilder<TProperty> propertyBuilder)
        {
            return propertyBuilder.HasColumnType("binary(512)");
        }
    }
}