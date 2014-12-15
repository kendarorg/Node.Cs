using Castle.MicroKernel.Registration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Node.Cs.CommandHandlers;
using Node.Cs.Consoles;
using Node.Cs.Test;
using System.IO;

namespace Node.Cs
{
    [TestClass]
    public class BasicNodeCommandsTest : TestBase<BasicNodeCommands, IBasicNodeCommands>
    {
        [TestInitialize]
        public override void TestInitialize()
        {
            base.TestInitialize();
            InitializeMock<INodeConsole>();
            Container.Register(Component.For<INodeExecutionContext>().Instance(new MockExecutionContext()));
        }

        [TestMethod]
        public void Echo_ShouldWriteNewLineToConsole()
        {
            //Setup
            SetupTarget();

            //Act
            Target.Echo(null, "message");

            //Verify
            MockOf<INodeConsole>().Verify(a => a.WriteLine("message"), Times.Once);
        }


        [TestMethod]
        public void Run_ShouldThrowANotFoundExceptionIfTheFileDoesNotExists()
        {
            //Setup
            SetupTarget();
            var context = Object<INodeExecutionContext>();

            //Act
            ExceptionAssert.Throws<FileNotFoundException>(()=>Target.Run(context, "BasicNodeCommandsTest\\notExisting.cs"));
        }

        [TestMethod]
        public void Run_ShouldLoadCsFileCompileAndRun()
        {
            //Setup
            SetupTarget();
            var context = Object<INodeExecutionContext>();

            //Act
            Target.Run(context, "BasicNodeCommandsTests\\test.cs");

            //Verify
						MockOf<INodeConsole>().Verify(a => a.WriteLine("Executing Test.cs"), Times.Once);
        }

        [TestMethod]
        public void Run_ShouldLoadCsFileCompileAndRunDespiteDirectorySeparatorChar()
        {
            //Setup
            SetupTarget();
            var context = Object<INodeExecutionContext>();

            //Act
            Target.Run(context, "BasicNodeCommandsTests/test.cs");

            //Verify
						MockOf<INodeConsole>().Verify(a => a.WriteLine("Executing Test.cs"), Times.Once);
        }
    }
}