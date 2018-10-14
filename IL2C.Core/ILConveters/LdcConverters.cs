﻿using System;

using Mono.Cecil.Cil;

using IL2C.Translators;

namespace IL2C.ILConverters
{
    internal sealed class Ldc_i4_0Converter : InlineNoneConverter
    {
        public override OpCode OpCode => OpCodes.Ldc_I4_0;

        public override Func<IExtractContext, string[]> Apply(DecodeContext decodeContext)
        {
            var symbolName = decodeContext.PushStack(decodeContext.PrepareContext.MetadataContext.Int32Type);
            return _ => new[] { string.Format("{0} = 0", symbolName) };
        }
    }

    internal sealed class Ldc_i4_1Converter : InlineNoneConverter
    {
        public override OpCode OpCode => OpCodes.Ldc_I4_1;

        public override Func<IExtractContext, string[]> Apply(DecodeContext decodeContext)
        {
            var symbolName = decodeContext.PushStack(decodeContext.PrepareContext.MetadataContext.Int32Type);
            return _ => new[] { string.Format("{0} = 1", symbolName) };
        }
    }

    internal sealed class Ldc_i4_2Converter : InlineNoneConverter
    {
        public override OpCode OpCode => OpCodes.Ldc_I4_2;

        public override Func<IExtractContext, string[]> Apply(DecodeContext decodeContext)
        {
            var symbolName = decodeContext.PushStack(decodeContext.PrepareContext.MetadataContext.Int32Type);
            return _ => new[] { string.Format("{0} = 2", symbolName) };
        }
    }

    internal sealed class Ldc_i4_3Converter : InlineNoneConverter
    {
        public override OpCode OpCode => OpCodes.Ldc_I4_3;

        public override Func<IExtractContext, string[]> Apply(DecodeContext decodeContext)
        {
            var symbolName = decodeContext.PushStack(decodeContext.PrepareContext.MetadataContext.Int32Type);
            return _ => new[] { string.Format("{0} = 3", symbolName) };
        }
    }

    internal sealed class Ldc_i4_4Converter : InlineNoneConverter
    {
        public override OpCode OpCode => OpCodes.Ldc_I4_4;

        public override Func<IExtractContext, string[]> Apply(DecodeContext decodeContext)
        {
            var symbolName = decodeContext.PushStack(decodeContext.PrepareContext.MetadataContext.Int32Type);
            return _ => new[] { string.Format("{0} = 4", symbolName) };
        }
    }

    internal sealed class Ldc_i4_5Converter : InlineNoneConverter
    {
        public override OpCode OpCode => OpCodes.Ldc_I4_5;

        public override Func<IExtractContext, string[]> Apply(DecodeContext decodeContext)
        {
            var symbolName = decodeContext.PushStack(decodeContext.PrepareContext.MetadataContext.Int32Type);
            return _ => new[] { string.Format("{0} = 5", symbolName) };
        }
    }

    internal sealed class Ldc_i4_6Converter : InlineNoneConverter
    {
        public override OpCode OpCode => OpCodes.Ldc_I4_6;

        public override Func<IExtractContext, string[]> Apply(DecodeContext decodeContext)
        {
            var symbolName = decodeContext.PushStack(decodeContext.PrepareContext.MetadataContext.Int32Type);
            return _ => new[] { string.Format("{0} = 6", symbolName) };
        }
    }

    internal sealed class Ldc_i4_7Converter : InlineNoneConverter
    {
        public override OpCode OpCode => OpCodes.Ldc_I4_7;

        public override Func<IExtractContext, string[]> Apply(DecodeContext decodeContext)
        {
            var symbolName = decodeContext.PushStack(decodeContext.PrepareContext.MetadataContext.Int32Type);
            return _ => new[] { string.Format("{0} = 7", symbolName) };
        }
    }

    internal sealed class Ldc_i4_8Converter : InlineNoneConverter
    {
        public override OpCode OpCode => OpCodes.Ldc_I4_8;

        public override Func<IExtractContext, string[]> Apply(DecodeContext decodeContext)
        {
            var symbolName = decodeContext.PushStack(decodeContext.PrepareContext.MetadataContext.Int32Type);
            return _ => new[] { string.Format("{0} = 8", symbolName) };
        }
    }

    internal sealed class Ldc_i4_m1Converter : InlineNoneConverter
    {
        public override OpCode OpCode => OpCodes.Ldc_I4_M1;

        public override Func<IExtractContext, string[]> Apply(DecodeContext decodeContext)
        {
            var symbolName = decodeContext.PushStack(decodeContext.PrepareContext.MetadataContext.Int32Type);
            return _ => new[] { string.Format("{0} = -1", symbolName) };
        }
    }

    internal sealed class Ldc_i4_sConverter : ShortInlineI1Converter
    {
        public override OpCode OpCode => OpCodes.Ldc_I4_S;

        public override Func<IExtractContext, string[]> Apply(sbyte operand, DecodeContext decodeContext)
        {
            var symbolName = decodeContext.PushStack(decodeContext.PrepareContext.MetadataContext.Int32Type);
            return _ => new[] { string.Format("{0} = {1}", symbolName, operand) };
        }
    }

    internal sealed class Ldc_i4Converter : InlineI4Converter
    {
        public override OpCode OpCode => OpCodes.Ldc_I4;

        public override Func<IExtractContext, string[]> Apply(int operand, DecodeContext decodeContext)
        {
            var symbolName = decodeContext.PushStack(decodeContext.PrepareContext.MetadataContext.Int32Type);

            // HACK: if operand is int.MinValue, compiler will cause constant literal warning:
            //   "warning: this decimal constant is unsigned only in ISO C90 [enabled by default]" (it's gcc, but vc causes same)
            //   IL2C makes the expression it special case.
            return _ => (operand != int.MinValue)
                ? new[] { string.Format("{0} = {1}", symbolName, operand) }
                : new[] { string.Format("{0} = ({1} - 1)", symbolName, int.MinValue + 1) };
        }
    }

    internal sealed class Ldc_i8Converter : InlineI8Converter
    {
        public override OpCode OpCode => OpCodes.Ldc_I8;

        public override Func<IExtractContext, string[]> Apply(long operand, DecodeContext decodeContext)
        {
            var symbolName = decodeContext.PushStack(decodeContext.PrepareContext.MetadataContext.Int64Type);

            // HACK: if operand is int.MinValue, compiler will cause constant literal warning:
            //   "warning: this decimal constant is unsigned only in ISO C90 [enabled by default]" (it's gcc, but vc causes same)
            //   IL2C makes the expression it special case.
            return _ => (operand != long.MinValue)
                ? new[] { string.Format("{0} = {1}LL", symbolName, operand) }
                : new[] { string.Format("{0} = ({1}LL - 1LL)", symbolName, long.MinValue + 1) };
        }
    }

    internal sealed class Ldc_R4Converter : InlineR4Converter
    {
        public override OpCode OpCode => OpCodes.Ldc_R4;

        public override Func<IExtractContext, string[]> Apply(float operand, DecodeContext decodeContext)
        {
            // The evaluation stack type at floating point, it has to 'F' type.
            // 'F' is only one type (64 bit floating point, it's System.Double.)
            // It expression causes implicitly up conversion between float and double.
            var symbolName = decodeContext.PushStack(decodeContext.PrepareContext.MetadataContext.DoubleType);

            // Single type format issue: https://docs.microsoft.com/en-us/dotnet/api/system.single.tostring
            //   By default, the return value only contains 7 digits of precision although a maximum of
            //   9 digits is maintained internally.
            //   If the value of this instance has greater than 7 digits, ToString returns
            //   PositiveInfinitySymbol or NegativeInfinitySymbol instead of the expected number.
            //   If you require more precision, specify format with the "G9" format specification,
            //   which always returns 9 digits of precision, or "R", which returns 7 digits if the number
            //   can be represented with that precision or 9 digits if the number can only be represented
            //   with maximum precision.
            return _ => new[] { string.Format("{0} = {1:g9}f", symbolName, operand) };
        }
    }

    internal sealed class Ldc_R8Converter : InlineR8Converter
    {
        public override OpCode OpCode => OpCodes.Ldc_R8;

        public override Func<IExtractContext, string[]> Apply(double operand, DecodeContext decodeContext)
        {
            var symbolName = decodeContext.PushStack(decodeContext.PrepareContext.MetadataContext.DoubleType);

            // Double type format issue: https://docs.microsoft.com/en-us/dotnet/api/system.double.tostring
            //   By default, the return value only contains 15 digits of precision although a maximum of
            //   17 digits is maintained internally.
            //   If the value of this instance has greater than 15 digits, ToString returns
            //   PositiveInfinitySymbol or NegativeInfinitySymbol instead of the expected number.
            //   If you require more precision, specify format with the "G17" format specification,
            //   which always returns 17 digits of precision, or "R", which returns 15 digits if the number
            //   can be represented with that precision or 17 digits if the number can only be represented
            //   with maximum precision.
            return _ => new[] { string.Format("{0} = {1:g17}", symbolName, operand) };
        }
    }
}
