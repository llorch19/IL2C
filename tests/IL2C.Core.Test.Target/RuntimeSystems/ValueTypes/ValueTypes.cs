﻿using System;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace IL2C.RuntimeSystems
{
    public struct ValueType1
    {
        public readonly int Value;

        public ValueType1(int value)
        {
            this.Value = value;
        }

        public string GetStringValue() =>
            this.Value.ToString();
    }

    public interface IValueTypeAccessor
    {
        string GetStringValue();
    }

    public struct ValueType2 : IValueTypeAccessor
    {
        public readonly int Value;

        public ValueType2(int value)
        {
            this.Value = value;
        }

        public string GetStringValue() =>
            this.Value.ToString();
    }

    public struct ValueTypeWithOverridedVirtual1
    {
        public readonly int Value;

        public ValueTypeWithOverridedVirtual1(int value)
        {
            this.Value = value;
        }

        public override string ToString() =>
            this.Value.ToString();
    }

    public interface IValueTypeOverrideAccessor
    {
        string ToString();
    }

    public struct ValueTypeWithOverridedVirtual2 : IValueTypeOverrideAccessor
    {
        public readonly int Value;

        public ValueTypeWithOverridedVirtual2(int value)
        {
            this.Value = value;
        }

        public override string ToString() =>
            this.Value.ToString();
    }

    public struct ValueTypeInheritedMethod
    {
    }

    [Description("Value types are specialized types at the .NET type system. Because the type inherited from the System.ValueType (objref type), all method has the managed pointer at the arg0 and these instances will box and apply the pseudo vptrs. These tests are verified the IL2C can handle value types.")]
    [TestCase("123", "CallInstanceMethod", 123, IncludeTypes = new[] { typeof(ValueType1) })]
    [TestCase("123", "CallInstanceMethodDirectly", 123, IncludeTypes = new[] { typeof(ValueType2), typeof(IValueTypeAccessor) })]
    [TestCase("123", "CallInstanceMethodBoxedInterface", 123, IncludeTypes = new[] { typeof(ValueType2), typeof(IValueTypeAccessor) })]
    [TestCase("123", "CallOverrideMethod", 123, IncludeTypes = new[] { typeof(ValueTypeWithOverridedVirtual1) })]
    [TestCase("123", "CallOverrideMethodDirectly", 123, IncludeTypes = new[] { typeof(ValueTypeWithOverridedVirtual2), typeof(IValueTypeOverrideAccessor) })]
    [TestCase("123", "CallOverrideMethodBoxed", 123, IncludeTypes = new[] { typeof(ValueTypeWithOverridedVirtual2), typeof(IValueTypeOverrideAccessor) })]
    [TestCase("123", "CallOverrideMethodBoxedInterface", 123, IncludeTypes = new[] { typeof(ValueTypeWithOverridedVirtual2), typeof(IValueTypeOverrideAccessor) })]
    [TestCase("IL2C.RuntimeSystems.ValueTypeInheritedMethod", "CallInheritedMethod", IncludeTypes = new[] { typeof(ValueTypeInheritedMethod) })]
    public sealed class ValueTypes
    {
        public static string CallInstanceMethod(int value)
        {
            var v = new ValueType1(value);
            return v.GetStringValue();
        }

        public static string CallInstanceMethodDirectly(int value)
        {
            var v = new ValueType2(value);
            return v.GetStringValue();
        }

        public static string CallInstanceMethodBoxedInterface(int value)
        {
            // implicitly boxed
            IValueTypeAccessor v = new ValueType2(value);
            return v.GetStringValue();
        }

        public static string CallOverrideMethod(int value)
        {
            var v = new ValueTypeWithOverridedVirtual1(value);
            return v.ToString();
        }

        public static string CallOverrideMethodDirectly(int value)
        {
            var v = new ValueTypeWithOverridedVirtual2(value);
            return v.ToString();
        }

        public static string CallOverrideMethodBoxed(int value)
        {
            // implicitly boxed
            object v = new ValueTypeWithOverridedVirtual2(value);
            return v.ToString();
        }

        public static string CallOverrideMethodBoxedInterface(int value)
        {
            // implicitly boxed
            IValueTypeOverrideAccessor v = new ValueTypeWithOverridedVirtual2(value);
            return v.ToString();
        }

        public static string CallInheritedMethod()
        {
            var v = new ValueTypeInheritedMethod();
            return v.ToString();
        }
    }
}
