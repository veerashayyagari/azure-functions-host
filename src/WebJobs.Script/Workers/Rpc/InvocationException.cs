// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;

namespace Microsoft.Azure.WebJobs.Script.Workers.Rpc
{
    public class InvocationException : Exception
    {
        private string _stack;

        public InvocationException(string message, string stack)
            : base(message)
        {
            _stack = stack;

            var className = typeof(Exception).GetField("_className");
            if (className != null)
            {
                className.SetValue(this, "MyTestException");
            }
        }

        public override string StackTrace => _stack;
    }
}