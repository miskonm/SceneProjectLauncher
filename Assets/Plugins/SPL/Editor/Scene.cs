using System;
using UnityEditor;
using UnityEngine;

namespace SPL.Editor
{
    [Serializable]
    public struct Scene : IEquatable<Scene>, ISerializationCallbackReceiver
    {
        public string Name;
        public int Index;
        public string Path;
        public GUID Guid;

        public string GuidHex;

        public bool Equals(Scene other) =>
            Name == other.Name && Index == other.Index && Path == other.Path && Guid.Equals(other.Guid);

        public override bool Equals(object obj) =>
            obj is Scene other && Equals(other);

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = (Name != null ? Name.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ Index;
                hashCode = (hashCode * 397) ^ (Path != null ? Path.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ Guid.GetHashCode();
                return hashCode;
            }
        }

        public static bool operator ==(Scene a, Scene b) =>
            a.Equals(b);

        public static bool operator !=(Scene a, Scene b) =>
            !(a == b);

        public void OnBeforeSerialize() =>
            GuidHex = Guid.ToString();

        public void OnAfterDeserialize()
        {
            if (GUID.TryParse(GuidHex, out GUID guid))
                Guid = guid;
        }
    }
}