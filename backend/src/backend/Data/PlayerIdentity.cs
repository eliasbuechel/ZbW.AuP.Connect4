using Microsoft.AspNetCore.Identity;

namespace backend.data
{
    public class PlayerIdentity : IdentityUser
    {
        public override bool Equals(object? obj)
        {
            if (obj == null)
                return false;

            if (obj is PlayerIdentity other)
                return other.Id == Id;

            return false;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id);
        }
    }
}