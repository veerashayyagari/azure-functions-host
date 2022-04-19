// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;

namespace Microsoft.Azure.WebJobs.Script.DependencyInjection
{
    public struct ServiceResolutionInfo
    {
        public string Name { get; set; }

        public TimeSpan TimeTaken { get; set; }

        public string Source { get; set; }
    }
}
