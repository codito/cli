// Copyright(c) .NET Foundation and contributors.All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using Microsoft.DotNet.Cli.CommandLine;
using Microsoft.DotNet.Cli.Utils;
using Microsoft.DotNet.Tools.MSBuild;

namespace Microsoft.DotNet.Tools.Test3
{
    public class Test3Command
    {
        public static int Run(string[] args)
        {
            DebugHelper.HandleDebugSwitch(ref args);

            var cmd = new CommandLineApplication(throwOnUnexpectedArg: false)
                          {
                              Name = "dotnet test3",
                              FullName = ".NET Test Driver",
                              Description = "Test Driver for the .NET Platform"
                          };

            cmd.HelpOption("-h|--help");

            var argRoot = cmd.Argument(
                "<PROJECT>",
                "The project to test, defaults to the current directory.",
                multipleValues: false);

            var settingOption = cmd.Option(
                "--settings <SettingsFile>",
                "Settings to use when running tests." + Environment.NewLine,
                CommandOptionType.SingleValue);

            var testsOption = cmd.Option(
                "--tests <TestNames>",
                @"Run tests with names that match the provided values. To provide multiple 
                                       values, separate them by commas.
                                       Examples: --tests:TestMethod1
                                       --tests:TestMethod1,testMethod2" + Environment.NewLine,
                CommandOptionType.SingleValue);

            var testAdapterPathOption = cmd.Option(
                "--testAdapterPath",
                @"This makes vstest.console.exe process use custom test adapters
                                       from a given path (if any) in the test run.
                                       Example  --testAdapterPath:<pathToCustomAdapters>" + Environment.NewLine,
                CommandOptionType.SingleValue);

            var platformOption = cmd.Option(
                "--platform <PlatformType>",
                @"Target platform architecture to be used for test execution.
                                       Valid values are x86, x64 and ARM." + Environment.NewLine,
                CommandOptionType.SingleValue);

            var frameworkOption = cmd.Option(
                "--framework <FrameworkVersion>",
                @"Target .Net Framework version to be used for test execution.
                                       Valid values are "".NETFramework, Version = v4.6"", "".NETCoreApp, Version = v1.0"" etc.
                                       Other supported values are Framework35, Framework40 and Framework45" + Environment.NewLine,
                CommandOptionType.SingleValue);

            var testCaseFilterOption = cmd.Option(
                "--testCaseFilter <Expression>",
                @"Run tests that match the given expression.
                                       <Expression> is of the format <property>Operator<value>[|&<Expression>]
                                       where Operator is one of =, != or ~  (Operator ~ has 'contains'
                                       semantics and is applicable for string properties like DisplayName).
                                       Parenthesis () can be used to group sub-expressions.
                                       Examples: --testCaseFilter:""Priority = 1""
                                       --testCaseFilter:""(FullyQualifiedName~Nightly | Name = MyTestMethod)""" + Environment.NewLine,
                CommandOptionType.SingleValue);

            // TODO tfs publisher text
            var loggerOption = cmd.Option(
                "--logger <LoggerUri/FriendlyName>",
                @"Specify a logger for test results.  For example, to log results into a
                                       Visual Studio Test Results File(TRX) use --logger:trx" + Environment.NewLine,
                CommandOptionType.MultipleValue);

            var listTestsOption = cmd.Option(
                "-lt|--listTests",
                @"Lists discovered tests" + Environment.NewLine,
                CommandOptionType.NoValue);

            var parentProcessIdOption = cmd.Option(
                "--parentProcessId <ParentProcessId>",
                @"Process Id of the Parent Process responsible for launching current process." + Environment.NewLine,
                CommandOptionType.SingleValue);

            var portOption = cmd.Option(
                "--port <Port>",
                @"The Port for socket connection and receiving the event messages." + Environment.NewLine,
                CommandOptionType.SingleValue);

            cmd.OnExecute(() =>
            {
                var msbuildArgs = new List<string>()
                {
                    "/t:VSTest"
                };

                if (settingOption.HasValue())
                {
                    msbuildArgs.Add($"/p:VSTestSetting={settingOption.Value()}");
                }

                // TODO: Only one test can be passed now. Need to fix this. msbuild is creating problem with ','
                if (testsOption.HasValue())
                {
                    msbuildArgs.Add($"/p:VSTestTests={testsOption.Value()}");
                }

                if (testAdapterPathOption.HasValue())
                {
                    msbuildArgs.Add($"/p:VSTestTestAdapterPath={testAdapterPathOption.Value()}");
                }

                if (platformOption.HasValue())
                {
                    msbuildArgs.Add($"/p:VSTestPlatform={platformOption.Value()}");
                }

                if (frameworkOption.HasValue())
                {
                    msbuildArgs.Add($"/p:VSTestFramework={frameworkOption.Value()}");
                }

                if (testCaseFilterOption.HasValue())
                {
                    msbuildArgs.Add($"/p:VSTestTestCaseFilter={testCaseFilterOption.Value()}");
                }

                // TODO: Currently it will work for single logger, need to fix this. msbuild is creating problem with ;
                if (loggerOption.HasValue())
                {
                    msbuildArgs.Add($"/p:VSTestLogger={string.Join(";", loggerOption.Values)}");
                }

                if (listTestsOption.HasValue())
                {
                    msbuildArgs.Add($"/p:VSTestListTests=true");
                }

                if (parentProcessIdOption.HasValue())
                {
                    msbuildArgs.Add($"/p:VSTestParentProcessId={parentProcessIdOption.Value()}");
                }

                if (portOption.HasValue())
                {
                    msbuildArgs.Add($"/p:VSTestPort={portOption.Value()}");
                }

                // Add in arguments
                msbuildArgs.AddRange(argRoot.Values);

                // Add remaining arguments that the parser did not understand
                msbuildArgs.AddRange(cmd.RemainingArguments);

                return new MSBuildForwardingApp(msbuildArgs).Execute();
            });

            return cmd.Execute(args);
        }
    }
}
