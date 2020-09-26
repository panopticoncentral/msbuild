// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.IO;
using System.Reflection;
using Microsoft.Build.Evaluation;
using Microsoft.Build.Shared;
using Xunit;

namespace Microsoft.Build.UnitTests.XamlDataDrivenToolTask_Tests
{
    /// <summary>
    /// Test fixture for testing the DataDrivenToolTask class with a fake task generated by XamlTestHelpers.cs
    /// Tests to see if certain switches are appended.
    /// </summary>
    public class GeneratedTask
    {
        private readonly Assembly _fakeTaskDll;

        public GeneratedTask()
        {
            _fakeTaskDll = XamlTestHelpers.SetupGeneratedCode();
        }

        /// <summary>
        /// Test to see whether all of the correct boolean switches are appended.
        /// </summary>
        [Fact]
        [Trait("Category", "mono-osx-failing")]
        public void TestDefaultFlags()
        {
            object fakeTaskInstance = CreateFakeTask();
            CheckCommandLine("/always /Cr:CT", XamlTestHelpers.GenerateCommandLine(fakeTaskInstance));
        }

        /// <summary>
        /// A test to see if all of the reversible flags are generated correctly
        /// This test case leaves the default flags the way they are
        /// </summary>
        [Fact]
        [Trait("Category", "mono-osx-failing")]
        public void TestReversibleFlagsWithDefaults()
        {
            object fakeTaskInstance = CreateFakeTask();
            XamlTestHelpers.SetProperty(fakeTaskInstance, "BasicReversible", true);
            string expectedResult = "/always /Br /Cr:CT";
            CheckCommandLine(expectedResult, XamlTestHelpers.GenerateCommandLine(fakeTaskInstance));
        }

        /// <summary>
        /// A test to see if all of the reversible flags are generated correctly
        /// This test case explicitly sets the ComplexReversible to be false
        /// </summary>
        [Fact]
        [Trait("Category", "mono-osx-failing")]
        public void TestReversibleFlagsWithoutDefaults()
        {
            object fakeTaskInstance = CreateFakeTask();
            XamlTestHelpers.SetProperty(fakeTaskInstance, "BasicReversible", true);
            XamlTestHelpers.SetProperty(fakeTaskInstance, "ComplexReversible", false);
            string expectedResult = "/always /Br /Cr:CF";
            CheckCommandLine(expectedResult, XamlTestHelpers.GenerateCommandLine(fakeTaskInstance));
        }

        /// <summary>
        /// Tests to make sure enums are working well.
        /// </summary>
        [Fact]
        [Trait("Category", "mono-osx-failing")]
        public void TestBasicString()
        {
            object fakeTaskInstance = CreateFakeTask();
            XamlTestHelpers.SetProperty(fakeTaskInstance, "BasicString", "Enum1");
            string expectedResult = "/always /Bs1 /Cr:CT";
            CheckCommandLine(expectedResult, XamlTestHelpers.GenerateCommandLine(fakeTaskInstance));
        }

        [Fact]
        [Trait("Category", "mono-osx-failing")]
        public void TestDynamicEnum()
        {
            object fakeTaskInstance = CreateFakeTask();
            XamlTestHelpers.SetProperty(fakeTaskInstance, "BasicDynamicEnum", "MyBeforeTarget");
            string expectedResult = "/always MyBeforeTarget /Cr:CT";
            CheckCommandLine(expectedResult, XamlTestHelpers.GenerateCommandLine(fakeTaskInstance));
        }

        /// <summary>
        /// Tests the basic string array type 
        /// </summary>
        [Fact]
        [Trait("Category", "mono-osx-failing")]
        public void TestBasicStringArray()
        {
            object fakeTaskInstance = CreateFakeTask();
            string[] fakeArray = new string[1];
            fakeArray[0] = "FakeStringArray";
            XamlTestHelpers.SetProperty(fakeTaskInstance, "BasicStringArray", new object[] { fakeArray });
            string expectedResult = "/always /BsaFakeStringArray /Cr:CT";
            CheckCommandLine(expectedResult, XamlTestHelpers.GenerateCommandLine(fakeTaskInstance));
        }

        /// <summary>
        /// Tests the basic string array type, with an array that contains multiple values. 
        /// </summary>
        [Fact]
        [Trait("Category", "mono-osx-failing")]
        public void TestBasicStringArray_MultipleValues()
        {
            object fakeTaskInstance = CreateFakeTask();
            string[] fakeArray = new string[3];
            fakeArray[0] = "Fake";
            fakeArray[1] = "String";
            fakeArray[2] = "Array";
            XamlTestHelpers.SetProperty(fakeTaskInstance, "BasicStringArray", new object[] { fakeArray });
            string expectedResult = "/always /BsaFake /BsaString /BsaArray /Cr:CT";
            CheckCommandLine(expectedResult, XamlTestHelpers.GenerateCommandLine(fakeTaskInstance));
        }

        /// <summary>
        /// Tests to see whether the integer appears correctly on the command line
        /// </summary>
        [Fact]
        [Trait("Category", "mono-osx-failing")]
        public void TestInteger()
        {
            object fakeTaskInstance = CreateFakeTask();
            XamlTestHelpers.SetProperty(fakeTaskInstance, "BasicInteger", 2);
            string expectedResult = "/always /Bi2 /Cr:CT";
            CheckCommandLine(expectedResult, XamlTestHelpers.GenerateCommandLine(fakeTaskInstance));
        }

        // complex tests
        /// <summary>
        /// Tests the (full) functionality of a reversible property
        /// </summary>
        [Fact]
        [Trait("Category", "mono-osx-failing")]
        public void TestComplexReversible()
        {
            // When flag is set to false
            object fakeTaskInstance = CreateFakeTask();
            XamlTestHelpers.SetProperty(fakeTaskInstance, "ComplexReversible", false);
            string expectedResult = "/always /Cr:CF";
            CheckCommandLine(expectedResult, XamlTestHelpers.GenerateCommandLine(fakeTaskInstance));

            // When flag is set to true
            fakeTaskInstance = CreateFakeTask();
            XamlTestHelpers.SetProperty(fakeTaskInstance, "ComplexReversible", true);
            expectedResult = "/always /Cr:CT";
            CheckCommandLine(expectedResult, XamlTestHelpers.GenerateCommandLine(fakeTaskInstance));
        }

        [Fact]
        [Trait("Category", "mono-osx-failing")]
        public void TestComplexString()
        {
            // check to see that the resulting value is good
            object fakeTaskInstance = CreateFakeTask();
            XamlTestHelpers.SetProperty(fakeTaskInstance, "ComplexString", "LegalValue1");
            string expectedResult = "/always /Cr:CT /Lv1";
            CheckCommandLine(expectedResult, XamlTestHelpers.GenerateCommandLine(fakeTaskInstance));
        }

        /// <summary>
        /// Tests the functionality of a string type property
        /// </summary>
        [Fact]
        [Trait("Category", "mono-osx-failing")]
        public void TestComplexStringArray()
        {
            object fakeTaskInstance = CreateFakeTask();
            string[] fakeArray = new string[] { "FakeFile1", "FakeFile2", "FakeFile3" };
            XamlTestHelpers.SetProperty(fakeTaskInstance, "ComplexStringArray", new object[] { fakeArray });
            string expectedResult = "/always /Cr:CT /CsaFakeFile1;FakeFile2;FakeFile3";
            CheckCommandLine(expectedResult, XamlTestHelpers.GenerateCommandLine(fakeTaskInstance));
        }

        [Fact]
        [Trait("Category", "mono-osx-failing")]
        public void TestComplexIntegerLessThanMin()
        {
            Assert.Throws<InvalidOperationException>(() =>
            {
                object fakeTaskInstance = CreateFakeTask();
                XamlTestHelpers.SetProperty(fakeTaskInstance, "ComplexInteger", 2);
            }
           );
        }

        [Fact]
        [Trait("Category", "mono-osx-failing")]
        public void TestComplexIntegerGreaterThanMax()
        {
            Assert.Throws<InvalidOperationException>(() =>
            {
                object fakeTaskInstance = CreateFakeTask();
                XamlTestHelpers.SetProperty(fakeTaskInstance, "ComplexInteger", 256);
                string expectedResult = "/always /Ci256 /Cr:CT";
                CheckCommandLine(expectedResult, XamlTestHelpers.GenerateCommandLine(fakeTaskInstance));
            }
           );
        }
        [Fact]
        [Trait("Category", "mono-osx-failing")]
        public void TestComplexIntegerWithinRange()
        {
            object fakeTaskInstance = CreateFakeTask();
            XamlTestHelpers.SetProperty(fakeTaskInstance, "ComplexInteger", 128);
            string expectedResult = "/always /Cr:CT /Ci128";
            CheckCommandLine(expectedResult, XamlTestHelpers.GenerateCommandLine(fakeTaskInstance));
        }

        /// <summary>
        /// This method checks the generated command line against the expected command line
        /// </summary>
        /// <param name="expected"></param>
        /// <param name="actual"></param>
        /// <returns>true if the two are the same, false if they are different</returns>
        private void CheckCommandLine(string expected, string actual)
        {
            Assert.Equal(expected, actual);
        }

        /// <summary>
        /// XamlTaskFactory does not, in and of itself, support the idea of "always" switches or default values.  At least 
        /// for Dev10, the workaround is to create a property as usual, and then specify the required values in the .props 
        /// file.  Since these unit tests are just testing the task itself, this method serves as our ".props file".  
        /// </summary>
        public object CreateFakeTask()
        {
            object fakeTaskInstance = _fakeTaskDll.CreateInstance("XamlTaskNamespace.FakeTask");

            XamlTestHelpers.SetProperty(fakeTaskInstance, "Always", true);
            XamlTestHelpers.SetProperty(fakeTaskInstance, "ComplexReversible", true);

            return fakeTaskInstance;
        }
    }

    /// <summary>
    /// Tests for XamlDataDrivenToolTask / XamlTaskFactory in the context of a project file.  
    /// </summary>
    public class ProjectFileTests
    {
        /// <summary>
        /// Tests that when a call to a XamlDataDrivenTask fails, the commandline is reported in the error message. 
        /// </summary>
        [Fact]
        [Trait("Category", "mono-osx-failing")]
        public void CommandLineErrorsReportFullCommandlineAmpersandTemp()
        {
            string projectFile = @"
                      <Project ToolsVersion=`msbuilddefaulttoolsversion` DefaultTargets=`XamlTaskFactory` xmlns=`http://schemas.microsoft.com/developer/msbuild/2003`>
                        <UsingTask TaskName=`TestTask` TaskFactory=`XamlTaskFactory` AssemblyName=`Microsoft.Build.Tasks.v4.0, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a`>
                          <Task>
                            <![CDATA[
                              <ProjectSchemaDefinitions xmlns=`clr-namespace:Microsoft.Build.Framework.XamlTypes;assembly=Microsoft.Build.Framework` xmlns:x=`http://schemas.microsoft.com/winfx/2006/xaml` xmlns:sys=`clr-namespace:System;assembly=mscorlib` xmlns:impl=`clr-namespace:Microsoft.VisualStudio.Project.Contracts.Implementation;assembly=Microsoft.VisualStudio.Project.Contracts.Implementation`>
                                <Rule Name=`TestTask` ToolName=`findstr.exe`>
                                  <StringProperty Name=`test` />
                                </Rule>
                              </ProjectSchemaDefinitions>
                            ]]>
                          </Task>
                        </UsingTask>
                        <Target Name=`XamlTaskFactory`>
                          <TestTask CommandLineTemplate='findstr'/>
                        </Target>
                      </Project>";


            string directoryWithAmpersand = "xaml&datadriven";
            string newTmp = Path.Combine(Path.GetTempPath(), directoryWithAmpersand);
            string oldTmp = Environment.GetEnvironmentVariable("TMP");

            try
            {
                Directory.CreateDirectory(newTmp);
                Environment.SetEnvironmentVariable("TMP", newTmp);
                Project p = ObjectModelHelpers.CreateInMemoryProject(projectFile);
                MockLogger logger = new MockLogger();

                bool success = p.Build(logger);
                Assert.False(success);
                logger.AssertLogContains("FINDSTR");

                // Should not be logging ToolTask.ToolCommandFailed, should be logging Xaml.CommandFailed
                logger.AssertLogDoesntContain("MSB6006");
                logger.AssertLogContains("MSB3721");
            }
            finally
            {
                Environment.SetEnvironmentVariable("TMP", oldTmp);
                ObjectModelHelpers.DeleteDirectory(newTmp);
                if (Directory.Exists(newTmp)) FileUtilities.DeleteWithoutTrailingBackslash(newTmp);
            }
        }


        /// <summary>
        /// Tests that when a call to a XamlDataDrivenTask fails, the commandline is reported in the error message. 
        /// </summary>
        [Fact]
        [Trait("Category", "mono-osx-failing")]
        public void CommandLineErrorsReportFullCommandline()
        {
            string projectFile = @"
                      <Project ToolsVersion=`msbuilddefaulttoolsversion` DefaultTargets=`XamlTaskFactory` xmlns=`http://schemas.microsoft.com/developer/msbuild/2003`>
                        <UsingTask TaskName=`TestTask` TaskFactory=`XamlTaskFactory` AssemblyName=`Microsoft.Build.Tasks.v4.0, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a`>
                          <Task>
                            <![CDATA[
                              <ProjectSchemaDefinitions xmlns=`clr-namespace:Microsoft.Build.Framework.XamlTypes;assembly=Microsoft.Build.Framework` xmlns:x=`http://schemas.microsoft.com/winfx/2006/xaml` xmlns:sys=`clr-namespace:System;assembly=mscorlib` xmlns:impl=`clr-namespace:Microsoft.VisualStudio.Project.Contracts.Implementation;assembly=Microsoft.VisualStudio.Project.Contracts.Implementation`>
                                <Rule Name=`TestTask` ToolName=`echoparameters.exe`>
                                  <StringProperty Name=`test` />
                                </Rule>
                              </ProjectSchemaDefinitions>
                            ]]>
                          </Task>
                        </UsingTask>
                        <Target Name=`XamlTaskFactory`>
                          <TestTask CommandLineTemplate=`where /x` />
                        </Target>
                      </Project>";

            Project p = ObjectModelHelpers.CreateInMemoryProject(projectFile);
            MockLogger logger = new MockLogger();

            bool success = p.Build(logger);

            Assert.False(success); // "Build should have failed"

            // Should not be logging ToolTask.ToolCommandFailed, should be logging Xaml.CommandFailed
            logger.AssertLogDoesntContain("MSB6006");
            logger.AssertLogContains("MSB3721");
        }

        /// <summary>
        /// Tests that when a call to a XamlDataDrivenTask fails, the commandline is reported in the error message. 
        /// </summary>
        [Fact]
        [Trait("Category", "mono-osx-failing")]
        public void SquareBracketEscaping()
        {
            string projectFile = @"
                      <Project ToolsVersion=`msbuilddefaulttoolsversion` DefaultTargets=`XamlTaskFactory` xmlns=`http://schemas.microsoft.com/developer/msbuild/2003`>
                        <UsingTask TaskName=`TestTask` TaskFactory=`XamlTaskFactory` AssemblyName=`Microsoft.Build.Tasks.v4.0, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a`>
                          <Task>
                            <![CDATA[
                              <ProjectSchemaDefinitions xmlns=`clr-namespace:Microsoft.Build.Framework.XamlTypes;assembly=Microsoft.Build.Framework` xmlns:x=`http://schemas.microsoft.com/winfx/2006/xaml` xmlns:sys=`clr-namespace:System;assembly=mscorlib` xmlns:impl=`clr-namespace:Microsoft.VisualStudio.Project.Contracts.Implementation;assembly=Microsoft.VisualStudio.Project.Contracts.Implementation`>
                                <Rule Name=`TestTask` ToolName=`echoparameters.exe`>
                                  <StringProperty Name=`test` />
                                </Rule>
                              </ProjectSchemaDefinitions>
                            ]]>
                          </Task>
                        </UsingTask>
                        <Target Name=`XamlTaskFactory`>
                          <TestTask CommandLineTemplate=`echo  1) [test]            end` test=`value` />
                          <TestTask CommandLineTemplate=`echo  2) [[[test]           end` test=`value` />
                          <TestTask CommandLineTemplate=`echo  3) [ [test]          end` test=`value` />
                          <TestTask CommandLineTemplate=`echo  4) [ [test] [test]    end` test=`value` />
                          <TestTask CommandLineTemplate=`echo  5) [test]]           end` test=`value` />
                          <TestTask CommandLineTemplate=`echo  6) [[test]]          end` test=`value` />
                          <TestTask CommandLineTemplate=`echo  7) [[[test]]          end` test=`value` />
                          <TestTask CommandLineTemplate=`echo  8) [notaproperty]   end` test=`value` />
                          <TestTask CommandLineTemplate=`echo  9) [[[notaproperty]  end` test=`value` />
                          <TestTask CommandLineTemplate=`echo 10) [ [notaproperty] end` test=`value` />
                          <TestTask CommandLineTemplate=`echo 11) [ [nap] [nap]    end` test=`value` />
                          <TestTask CommandLineTemplate=`echo 12) [notaproperty]]  end` test=`value` />
                          <TestTask CommandLineTemplate=`echo 13) [[notaproperty]]  end` test=`value` />
                          <TestTask CommandLineTemplate=`echo 14) [[[notaproperty]] end` test=`value` />
                        </Target>
                      </Project>";

            Project p = ObjectModelHelpers.CreateInMemoryProject(projectFile);
            MockLogger logger = new MockLogger();

            bool success = p.Build(logger);

            Assert.True(success); // "Build should have succeeded"

            logger.AssertLogContains("echo  1) value            end");
            logger.AssertLogContains("echo  2) [value           end");
            logger.AssertLogContains("echo  3) [ value          end");
            logger.AssertLogContains("echo  4) [ value value    end");
            logger.AssertLogContains("echo  5) value]           end");
            logger.AssertLogContains("echo  6) [test]]          end");
            logger.AssertLogContains("echo  7) [value]          end");
            logger.AssertLogContains("echo  8) [notaproperty]   end");
            logger.AssertLogContains("echo  9) [[notaproperty]  end");
            logger.AssertLogContains("echo 10) [ [notaproperty] end");
            logger.AssertLogContains("echo 11) [ [nap] [nap]    end");
            logger.AssertLogContains("echo 12) [notaproperty]]  end");
            logger.AssertLogContains("echo 13) [notaproperty]]  end");
            logger.AssertLogContains("echo 14) [[notaproperty]] end");
        }
    }
}
