using System.Collections.Generic;

namespace RealEstate.Base.Api
{
    public class VersionCheck
    {
        public VersionCheck(int major, int minor, int bugfixes)
        {
            Major = major;
            Minor = minor;
            Bugfixes = bugfixes;
        }

        public VersionCheck()
        {
        }

        public void Deconstruct(out int major, out int minor)
        {
            major = Major;
            minor = Minor;
        }

        public int Major { get; set; }
        public int Minor { get; set; }
        public int Bugfixes { get; set; }

        private sealed class MajorMinorEqualityComparer : IEqualityComparer<VersionCheck>
        {
            public bool Equals(VersionCheck x, VersionCheck y)
            {
                if (ReferenceEquals(x, y))
                    return true;
                if (ReferenceEquals(x, null))
                    return false;
                if (ReferenceEquals(y, null))
                    return false;
                if (x.GetType() != y.GetType())
                    return false;
                return x.Major == y.Major
                       && x.Minor == y.Minor;
            }

            public int GetHashCode(VersionCheck obj)
            {
                unchecked
                {
                    return (obj.Major * 397) ^ obj.Minor;
                }
            }
        }

        public static IEqualityComparer<VersionCheck> MajorMinorComparer { get; } = new MajorMinorEqualityComparer();

        public override string ToString()
        {
            return $"{Major}.{Minor}.{Bugfixes}";
        }
    }
}