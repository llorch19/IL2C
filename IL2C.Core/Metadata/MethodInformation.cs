﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using Mono.Cecil;

namespace IL2C.Metadata
{
    [Flags]
    public enum FriendlyNameTypes
    {
        Standard = 0x00,
        ArgumentTypes = 0x01,
        ArgumentNames = 0x02,
        Index = 0x04,
        Full = 0x07,
        Mangled = 0x08,
    }

    public interface IMethodInformation : IMemberInformation
    {
        bool IsConstructor { get; }
        bool IsStatic { get; }
        bool IsVirtual { get; }
        bool IsAbstract { get; }
        bool IsSealed { get; }
        bool IsNewSlot { get; }
        bool HasBody { get; }

        bool IsStandardMethod { get; }

        ITypeInformation ReturnType { get; }
        VariableInformation[] Parameters { get; }
        VariableInformation[] LocalVariables { get; }
        ICodeStream CodeStream { get; }
        int OverloadIndex { get; }

        string GetFriendlyName(FriendlyNameTypes type = FriendlyNameTypes.Full);
        VariableInformation[] GetInstanceParameters(ITypeInformation thisType);

        string RawFunctionPrototype { get; }
        string FunctionPrototype { get; }
        string FunctionTypePrototype { get; }

    }

    internal sealed class MethodInformation
        : MemberInformation<MethodReference, MethodDefinition>
        , IMethodInformation
    {
        private static readonly DebugInformation[] empty = new DebugInformation[0];

        private readonly Lazy<ITypeInformation> returnType;
        private readonly Lazy<VariableInformation[]> parameters;
        private readonly Lazy<VariableInformation[]> variables;
        private readonly Lazy<CodeStream> codeStreams;
        private readonly Lazy<int> overloadIndex;

        public MethodInformation(MethodReference method, ModuleInformation module)
            : base(method, module)
        {
            returnType = this.Context.LazyGetOrAddMember(
                () => this.Member.ReturnType,
                type => (type != null) ? new TypeInformation(type, module) : this.Context.VoidType);
            parameters = Lazy.Create(() => (method.HasThis)
                ? new[] { this.CreateThisParameterInformation(this.DeclaringType) }
                    .Concat(this.Member.Parameters.Select(this.ToParameterInformation))
                    .ToArray()
                : this.Member.Parameters
                    .Select(ToParameterInformation)
                    .ToArray());
            variables = Lazy.Create(() => this.Definition.Body.Variables
                .Select(variable => new VariableInformation(
                    variable.Index,
                    this.Definition.Body.Method.DebugInformation.TryGetName(variable, out var name)
                        ? name
                        : string.Format("local{0}__", variable.Index),
                    this.Context.GetOrAddMember(
                        variable.VariableType,
                        variableType => new TypeInformation(
                            variableType,
                            this.Context.GetOrAddModule(
                                variableType.Module.Assembly,
                                variableType.Module,
                                (assembly, module_) => new ModuleInformation(
                                    module_,
                                    this.Context.GetOrAddAssembly(
                                        assembly,
                                        assembly_ => new AssemblyInformation(assembly_, this.Context))))))))
                .ToArray());
            codeStreams = Lazy.Create(() =>
                {
                    // It gathers sequence point informations.
                    // It will use writing the line preprocessor directive.
                    var paths = new Dictionary<string, string>();
                    var spd =
                        (from sp in this.Definition.DebugInformation.SequencePoints
                         where !sp.IsHidden
                         group sp by sp.Offset into g
                         let sps = g
                            .OrderBy(sp => sp.Offset)
                            .Select(sp => new DebugInformation(
                                paths.TryGetValue(sp.Document.Url, out var url) ? url : sp.Document.Url,
                                sp.StartLine,
                                sp.StartColumn))
                            .ToArray()
                         where sps.Length >= 1
                         select new { g.Key, sps })
                        .ToDictionary(g => g.Key, g => g.sps);
                    return new CodeStream(this.Definition.Body.Instructions
                        .OrderBy(instruction => instruction.Offset)
                        .Select(instruction => new CodeInformation(
                            instruction.Offset,
                            instruction.OpCode,
                            instruction.Operand,
                            instruction.GetSize(),
                            spd.TryGetValue(instruction.Offset, out var sps) ? sps : empty)));
                });
            overloadIndex = Lazy.Create(() =>
                {
                    if (this.Definition.DeclaringType.Methods.Count < 2)
                    {
                        return 0;
                    }

                    var found = this.Definition.DeclaringType.Methods
                        .Where(m => m.Name == this.Member.Name)
                        .OrderByParameters()     // Stable by overload types.
                        .Select((m, i) => new { m, i })
                        .First(e => MemberReferenceComparer.Instance.Equals(e.m, method));
                    return found.i;
                });
        }

        private VariableInformation CreateThisParameterInformation(ITypeInformation thisType)
        {
            return new VariableInformation(
                0,
                "this__",
                thisType);
        }

        private VariableInformation ToParameterInformation(ParameterReference parameter)
        {
            return new VariableInformation(
                parameter.Index,
                parameter.Name,
                this.Context.GetOrAddMember(
                    parameter.ParameterType,
                    type_ => new TypeInformation(
                        type_,
                        this.Context.GetOrAddModule(
                            type_.Module.Assembly,
                            type_.Module,
                            (assembly_, module_) => new ModuleInformation(
                                module_,
                                this.Context.GetOrAddAssembly(
                                    assembly_,
                                    assembly__ => new AssemblyInformation(assembly__, this.Context)))))));
        }

        public override string FriendlyName => this.GetFriendlyName(FriendlyNameTypes.Full);

        public bool IsConstructor => this.Definition.IsConstructor;
        public bool IsStatic => this.Definition.IsStatic;
        public bool IsVirtual => this.Definition.IsVirtual;
        public bool IsAbstract => this.Definition.IsAbstract;
        public bool IsSealed => this.Definition.IsFinal;
        public bool IsNewSlot => this.Definition.IsNewSlot;
        public bool HasBody => this.Definition.HasBody;

        public bool IsStandardMethod
        {
            get
            {
                // TODO: Except typed delegate's async methods
                //   Because currently IL2C not supported async methods.
                if (this.Context.DelegateType.IsAssignableFrom(this.DeclaringType))
                {
                    // Only "Invoke", exclude "BeginInvoke" and "EndInvoke"
                    return this.Name.Equals("Invoke");
                }

                // Except type initializer
                if (this.IsConstructor)
                {
                    return !this.IsStatic;
                }

                return true;
            }
        }

        public ITypeInformation ReturnType => returnType.Value;
        public VariableInformation[] Parameters => parameters.Value;
        public VariableInformation[] LocalVariables => variables.Value;
        public ICodeStream CodeStream => codeStreams.Value;
        public int OverloadIndex => overloadIndex.Value;

        private static bool IncludeNames(FriendlyNameTypes type)
        {
            return (type & FriendlyNameTypes.ArgumentNames) == FriendlyNameTypes.ArgumentNames;
        }

        private static bool IncludeTypes(FriendlyNameTypes type)
        {
            return (type & FriendlyNameTypes.ArgumentTypes) == FriendlyNameTypes.ArgumentTypes;
        }

        private static bool IncludeIndex(FriendlyNameTypes type)
        {
            return (type & FriendlyNameTypes.Index) == FriendlyNameTypes.Index;
        }

        private static bool Mangled(FriendlyNameTypes type)
        {
            return (type & FriendlyNameTypes.Mangled) == FriendlyNameTypes.Mangled;
        }

        public string GetFriendlyName(FriendlyNameTypes type = FriendlyNameTypes.Full)
        {
            var index = IncludeIndex(type)
                ? string.Format("@{0}", this.OverloadIndex)
                : string.Empty;

            var arguments = (IncludeNames(type) || IncludeTypes(type))
                ? string.Format(
                    "({0})",
                    string.Join(
                        ", ",
                        this.Parameters.Select(parameter =>
                            (IncludeNames(type) && IncludeTypes(type))
                                ? string.Format(
                                    "{0} {1}",
                                    parameter.TargetType.FriendlyName,
                                    parameter.SymbolName)
                                : IncludeTypes(type)
                                    ? parameter.TargetType.FriendlyName
                                    : parameter.SymbolName)))
                : string.Empty;

            var name = string.Format(
                "{0}{1}{2}",
                base.FriendlyName,
                index,
                arguments);

            return Mangled(type) ? ToMangledName(name) : name;
        }

        public VariableInformation[] GetInstanceParameters(ITypeInformation thisType)
        {
            Debug.Assert(this.Member.HasThis);

            return new[] { this.CreateThisParameterInformation(thisType) }
                .Concat(this.Member.Parameters.Select(this.ToParameterInformation))
                .ToArray();
        }

        private string GetFunctionPrototype(bool decorate)
        {
            var parametersString = string.Join(
                ", ",
                this.Parameters.Select(parameter => string.Format(
                    "{0} {1}",
                    parameter.TargetType.CLanguageName,
                    parameter.SymbolName)));

            var returnTypeName =
                this.ReturnType.CLanguageName;

            return string.Format(
                decorate ? "{0} __{1}__({2})" : "{0} {1}({2})",
                returnTypeName,
                this.MangledName,
                parametersString);
        }

        public string RawFunctionPrototype => this.GetFunctionPrototype(false);
        public string FunctionPrototype => this.GetFunctionPrototype(this.IsVirtual);

        public string FunctionTypePrototype
        {
            get
            {
                var parametersString = string.Join(
                    ", ",
                    this.Parameters.Select(parameter => string.Format(
                        "{0} {1}",
                        parameter.TargetType.CLanguageName,
                        parameter.SymbolName)));

                var returnTypeName =
                    this.ReturnType.CLanguageName;

                return string.Format(
                    "{0} (*{1})({2})",
                    returnTypeName,
                    this.Name,
                    parametersString);
            }
        }

        protected override MethodDefinition OnResolve(MethodReference member)
        {
            return member.Resolve();
        }
    }
}
