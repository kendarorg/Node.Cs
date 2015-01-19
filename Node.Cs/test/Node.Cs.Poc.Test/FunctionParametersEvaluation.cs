using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Node.Cs.Poc.Test.samples;
using System.Runtime.InteropServices;

namespace Node.Cs.Poc.Test
{
    [TestClass]
    public class FunctionParametersEvaluation
    {
        [TestMethod]
        public void WithReflection_ShouldBePossibleToRecognize_OptionalParameters_WithStringType()
        {
            var type = typeof(FunctionParamsSource);
            var methodArray = type.GetMethod("NonOptionalFunctionString");
            var parametersMethodArray = methodArray.GetParameters();

            Assert.AreEqual(typeof(string), parametersMethodArray[0].ParameterType);
            Assert.AreEqual(typeof(string), parametersMethodArray[1].ParameterType);
            Assert.AreEqual(0, parametersMethodArray[1].CustomAttributes.ToList().Count());

            var method = type.GetMethod("OptionalFunctionString");
            var parameters = method.GetParameters();

            Assert.AreEqual(typeof(string), parameters[0].ParameterType);
            Assert.AreEqual(typeof(string), parameters[1].ParameterType);
            Assert.AreEqual(1, parameters[1].CustomAttributes.ToList().Count());

            var parameter = parameters[1];
            var customAttribute = parameter
                .GetCustomAttributes(typeof(OptionalAttribute), true)
                .FirstOrDefault() as OptionalAttribute;

            Assert.IsNotNull(customAttribute);
            Assert.IsTrue(parameter.HasDefaultValue);
            Assert.AreEqual("optionalValue", parameter.DefaultValue);
            Assert.AreEqual(typeof(string), parameter.DefaultValue.GetType());
        }

        [TestMethod]
        public void WithReflection_ShouldBePossibleToRecognize_OptionalParameters_WithObjectType()
        {
            var type = typeof(FunctionParamsSource);
            var methodArray = type.GetMethod("NonOptionalFunctionObject");
            var parametersMethodArray = methodArray.GetParameters();

            Assert.AreEqual(typeof(string), parametersMethodArray[0].ParameterType);
            Assert.AreEqual(typeof(FunctionParamsSource), parametersMethodArray[1].ParameterType);
            Assert.AreEqual(0, parametersMethodArray[1].CustomAttributes.ToList().Count());

            Assert.IsFalse(parametersMethodArray[0].HasDefaultValue);
            Assert.IsFalse(parametersMethodArray[1].HasDefaultValue);

            var method = type.GetMethod("OptionalFunctionObject");
            var parameters = method.GetParameters();

            Assert.AreEqual(typeof(string), parameters[0].ParameterType);
            Assert.AreEqual(typeof(FunctionParamsSource), parameters[1].ParameterType);
            Assert.AreEqual(1, parameters[1].CustomAttributes.ToList().Count());

            var parameter = parameters[1];
            var customAttribute = parameter
                .GetCustomAttributes(typeof(OptionalAttribute), true)
                .FirstOrDefault() as OptionalAttribute;

            Assert.IsNotNull(customAttribute);
            Assert.IsTrue(parameter.HasDefaultValue);
            Assert.AreEqual(null, parameter.DefaultValue);
        }

        [TestMethod]
        public void WithReflection_ShouldBePossibleToRecognize_OptionalParameters_WithBaseTypes()
        {
            var type = typeof(FunctionParamsSource);
            var methodArray = type.GetMethod("NonOptionalFunction");
            var parametersMethodArray = methodArray.GetParameters();

            Assert.AreEqual(typeof(string), parametersMethodArray[0].ParameterType);
            Assert.AreEqual(typeof(int), parametersMethodArray[1].ParameterType);
            Assert.AreEqual(0, parametersMethodArray[1].CustomAttributes.ToList().Count());

            Assert.IsFalse(parametersMethodArray[0].HasDefaultValue);
            Assert.IsFalse(parametersMethodArray[1].HasDefaultValue);

            var method = type.GetMethod("OptionalFunction");
            var parameters = method.GetParameters();

            Assert.AreEqual(typeof(string), parameters[0].ParameterType);
            Assert.AreEqual(typeof(int), parameters[1].ParameterType);
            Assert.AreEqual(1, parameters[1].CustomAttributes.ToList().Count());

            var parameter = parameters[1];
            var customAttribute = parameter
                .GetCustomAttributes(typeof(OptionalAttribute), true)
                .FirstOrDefault() as OptionalAttribute;

            Assert.IsNotNull(customAttribute);
            Assert.IsTrue(parameter.HasDefaultValue);
            Assert.AreEqual(1, parameter.DefaultValue);
        }


        [TestMethod]
        public void WithReflection_VarArgsParameters_MustBeRecognized()
        {
            var type = typeof(FunctionParamsSource);
            var methodArray = type.GetMethod("ArrayFunction");
            var parametersMethodArray = methodArray.GetParameters();

            Assert.AreEqual(typeof(string), parametersMethodArray[0].ParameterType);
            Assert.AreEqual(typeof(string[]), parametersMethodArray[1].ParameterType);
            Assert.AreEqual(0, parametersMethodArray[1].CustomAttributes.ToList().Count());

            Assert.IsFalse(parametersMethodArray[0].HasDefaultValue);
            Assert.IsFalse(parametersMethodArray[1].HasDefaultValue);

            var method = type.GetMethod("VaArgsFunction");
            var parameters = method.GetParameters();

            Assert.AreEqual(typeof(string), parameters[0].ParameterType);
            Assert.AreEqual(typeof(string[]), parameters[1].ParameterType);
            Assert.AreEqual(1, parameters[1].CustomAttributes.ToList().Count());

            var customAttribute = parameters[1]
                .GetCustomAttributes(typeof(ParamArrayAttribute), true)
                .FirstOrDefault() as ParamArrayAttribute;

            Assert.IsNotNull(customAttribute);
        }
    }
}
