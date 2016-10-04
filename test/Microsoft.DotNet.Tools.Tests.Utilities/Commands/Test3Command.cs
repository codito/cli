// Copyright (c) .NET Foundation and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.DotNet.Cli.Utils;

namespace Microsoft.DotNet.Tools.Test.Utilities
{
    public sealed class Test3Command : TestCommand
    {
        public Test3Command()
            : base("dotnet")
        {
        }

        public new Test3Command WithWorkingDirectory(string workingDirectory)
        {
            base.WithWorkingDirectory(workingDirectory);
            return this;
        }

        public override CommandResult Execute(string args = "")
        {
            args = $"test3 {args}";
            return base.Execute(args);
        }

        public override CommandResult ExecuteWithCapturedOutput(string args = "")
        {
            args = $"test3 {args}";
            return base.ExecuteWithCapturedOutput(args);
        }
    }
}
