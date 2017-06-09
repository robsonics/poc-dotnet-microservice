//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;

//namespace Root.Versioning
//{
//    public class Version : IComparable<Version>
//    {
//        public Version(int major, int? minor = null, int? release = null, int? build = null)
//        {
//            Major = major;
//            Minor = minor;
//            Release = release;
//            Build = build;
//        }
//        public int Major { get; private set; }

//        public int? Minor { get; private set; }

//        public int? Release { get; private set; }

//        public int? Build { get; private set; }

//        public static Version FromString(string version)
//        {
//            if (string.IsNullOrEmpty(version))
//                throw new ArgumentException("version");
//            var ele = version.Split('.');
//            var major = Int32.Parse(ele[0]);
//            var minor = ele.Length > 1 ? Int32.Parse(ele[1]) : (int?)null;
//            var release = ele.Length > 2 ? Int32.Parse(ele[2]) : (int?)null;
//            var build = ele.Length > 3 ? Int32.Parse(ele[3]) : (int?)null;
//            return new Version(major, minor, release, build);
//        }

//        public int CompareTo(Version other)
//        {
//            if (null == other)
//                return 1;
//            if (Major < other.Major)
//                return -1;
//            else if (Major > other.Major)
//                return 1;
//            else if (Major == other.Major)
//            {
//                if (Minor.HasValue && other.Minor.HasValue)
//                {
//                    if (Minor.Value < other.Minor.Value)
//                        return -1;
//                    else if (Minor.Value > other.Minor.Value)
//                        return 1;
//                    else if (Minor.Value == other.Minor.Value)
//                    {
//                        if (Release.HasValue && other.Release.HasValue)
//                        {
//                            if (Release.Value < other.Release.Value)
//                                return -1;
//                            else if (Release.Value > other.Release.Value)
//                                return 1;
//                            else if (Release.Value == other.Release.Value)
//                            {
//                                if (Build.HasValue && other.Build.HasValue)
//                                {
//                                    if (Build.Value < other.Build.Value)
//                                        return -1;
//                                    else if (Build.Value > other.Build.Value)
//                                        return 1;
//                                    else if (Build.Value == other.Build.Value)
//                                        return 0;
//                                }
//                                else if (Build.HasValue && !other.Build.HasValue)
//                                    return 1;
//                                else if (!Build.HasValue && other.Build.HasValue)
//                                    return -1;
//                                else if (!Build.HasValue && !other.Build.HasValue)
//                                    return 0;
//                            }
//                        }
//                        else if (Release.HasValue && !other.Release.HasValue)
//                            return 1;
//                        else if (!Release.HasValue && other.Release.HasValue)
//                            return -1;
//                        else if (!Release.HasValue && !other.Release.HasValue)
//                            return 0;
//                    }
//                }
//                else if (Minor.HasValue && !other.Minor.HasValue)
//                    return 1;
//                else if (!Minor.HasValue && other.Minor.HasValue)
//                    return -1;
//                else if (!Minor.HasValue && !other.Minor.HasValue)
//                    return 0;
//            }
//            return 0;
//        }

//        public override string ToString()
//        {
//            var minor = Minor.HasValue ? "." + Minor.Value : string.Empty;
//            var release = Release.HasValue ? "." + Release.Value : string.Empty;
//            var build = Build.HasValue ? "." + Build : string.Empty;
//            return Major + minor + release + build;
//        }
//    }
//}
