﻿using System;

using IL2C.Metadata;

namespace IL2C.Translators
{
    public struct SymbolInformation
    {
        public readonly string SymbolName;
        internal readonly ITypeInformation TargetType;

        internal SymbolInformation(string symbolName, ITypeInformation targetType)
        {
            this.SymbolName = symbolName;
            this.TargetType = targetType;
        }

        public bool Equals(SymbolInformation rhs)
        {
            return this.SymbolName.Equals(rhs.SymbolName)
                && this.TargetType.Equals(rhs.TargetType);
        }

        public override bool Equals(object rhs)
        {
            if (rhs is SymbolInformation)
            {
                return this.Equals((SymbolInformation)rhs);
            }
            else
            {
                return false;
            }
        }

        public override int GetHashCode()
        {
            return this.SymbolName.GetHashCode()
                ^ this.TargetType.GetHashCode();
        }

        public override string ToString()
        {
            return string.Format("{0}: {1}", this.SymbolName, this.TargetType.UniqueName);
        }
    }
}
