using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace ModelBuilder
{
    /// <summary>
    /// Creating class at runtime.
    /// </summary>
    public static class CustomTypeBuilder
    {
        /// <summary>
        /// Creates a class dynamically.
        /// </summary>
        /// <param name="typeSignature">The type signature.</param>
        /// <param name="properties">The properties.</param>
        /// <returns>A result type.</returns>
        public static TypeInfo CompileResultType(string typeSignature, IDictionary<string, Type> properties)
        {
            var tb = GetTypeBuilder(typeSignature);
            var constructor = tb.DefineDefaultConstructor(MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName);

            foreach (var prop in properties)
                CreateProperty(tb, prop.Key, prop.Value);

            var objectType = tb.CreateTypeInfo();
            return objectType;
        }

        /// <summary>
        /// Gets the type builder.
        /// </summary>
        /// <param name="typeSignature">The type signature.</param>
        /// <returns>A type builder.</returns>
        private static TypeBuilder GetTypeBuilder(string typeSignature)
        {
            var an = new AssemblyName(typeSignature);
            var assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(an, AssemblyBuilderAccess.Run);
            var moduleBuilder = assemblyBuilder.DefineDynamicModule("Module");

            var tb = moduleBuilder.DefineType(
                typeSignature,
                TypeAttributes.Public
                    | TypeAttributes.Class
                    | TypeAttributes.AutoClass
                    | TypeAttributes.AnsiClass
                    | TypeAttributes.BeforeFieldInit
                    | TypeAttributes.AutoLayout,
                null);

            return tb;
        }

        /// <summary>
        /// Creates the property.
        /// </summary>
        /// <param name="typeBuilder">The type builder.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="propertyType">Type of the property.</param>
        private static void CreateProperty(TypeBuilder typeBuilder, string propertyName, Type propertyType)
        {
            var fieldBuilder = typeBuilder.DefineField("_" + propertyName, propertyType, FieldAttributes.Private);
            var propertyBuilder = typeBuilder.DefineProperty(propertyName, PropertyAttributes.HasDefault, propertyType, null);

            var getPropMthdBldr = typeBuilder.DefineMethod(
                "get_" + propertyName,
                MethodAttributes.Public
                    | MethodAttributes.SpecialName
                    | MethodAttributes.HideBySig,
                propertyType,
                Type.EmptyTypes);

            var getIl = getPropMthdBldr.GetILGenerator();

            getIl.Emit(OpCodes.Ldarg_0);
            getIl.Emit(OpCodes.Ldfld, fieldBuilder);
            getIl.Emit(OpCodes.Ret);

            var setPropMthdBldr = typeBuilder.DefineMethod(
                "set_" + propertyName,
                MethodAttributes.Public
                    | MethodAttributes.SpecialName
                    | MethodAttributes.HideBySig,
                null,
                new[] { propertyType });

            var setIl = setPropMthdBldr.GetILGenerator();
            var modifyProperty = setIl.DefineLabel();
            var exitSet = setIl.DefineLabel();

            setIl.MarkLabel(modifyProperty);
            setIl.Emit(OpCodes.Ldarg_0);
            setIl.Emit(OpCodes.Ldarg_1);
            setIl.Emit(OpCodes.Stfld, fieldBuilder);

            setIl.Emit(OpCodes.Nop);
            setIl.MarkLabel(exitSet);
            setIl.Emit(OpCodes.Ret);

            propertyBuilder.SetGetMethod(getPropMthdBldr);
            propertyBuilder.SetSetMethod(setPropMthdBldr);
        }
    }
}
