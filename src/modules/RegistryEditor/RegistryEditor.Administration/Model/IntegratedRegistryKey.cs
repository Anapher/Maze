using RegistryEditor.Shared.Dtos;

namespace RegistryEditor.Administration.Model
{
    public class IntegratedRegistryKey : RegistryKeyDto
    {
        public string Path { get; set; }

        protected bool Equals(IntegratedRegistryKey other) => string.Equals(Path, other.Path) && HasSubKeys == other.HasSubKeys;

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((IntegratedRegistryKey) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((Path != null ? Path.GetHashCode() : 0) * 397) ^ HasSubKeys.GetHashCode();
            }
        }
    }
}