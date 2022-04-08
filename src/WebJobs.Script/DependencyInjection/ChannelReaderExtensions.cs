// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Channels;

namespace Microsoft.Azure.WebJobs.Script.DependencyInjection
{
    internal static class ChannelReaderExtensions
    {
        public static async IAsyncEnumerable<T> ReadAllAsync<T>(this ChannelReader<T> reader, [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            while (await reader.WaitToReadAsync(cancellationToken).ConfigureAwait(false))
            {
                while (reader.TryRead(out var item))
                {
                    yield return item;
                }
            }
        }
    }
}