﻿// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs.Logging;
using Microsoft.Azure.WebJobs.Script.Configuration;
using Microsoft.Azure.WebJobs.Script.Description;
using Microsoft.Azure.WebJobs.Script.Diagnostics;
using Microsoft.Azure.WebJobs.Script.Diagnostics.Extensions;
using Microsoft.Azure.WebJobs.Script.Workers;
using Microsoft.Azure.WebJobs.Script.Workers.Rpc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;

namespace Microsoft.Azure.WebJobs.Script
{
    public class WorkerFunctionMetadataProvider : IFunctionMetadataProvider
    {
        private readonly Dictionary<string, ICollection<string>> _functionErrors = new Dictionary<string, ICollection<string>>();
        private readonly ILogger _logger;
        private readonly IFunctionInvocationDispatcher _dispatcher;
        private ImmutableArray<FunctionMetadata> _functions;

        public WorkerFunctionMetadataProvider(ILogger<WorkerFunctionMetadataProvider> logger, IFunctionInvocationDispatcher invocationDispatcher)
        {
            _logger = logger;
            _dispatcher = invocationDispatcher ?? throw new ArgumentNullException(nameof(invocationDispatcher));
        }

        public ImmutableDictionary<string, ImmutableArray<string>> FunctionErrors
           => _functionErrors.ToImmutableDictionary(kvp => kvp.Key, kvp => kvp.Value.ToImmutableArray());

        public ImmutableArray<FunctionMetadata> GetFunctionMetadata(IEnumerable<RpcWorkerConfig> workerConfigs, bool forceRefresh)
        {
            return GetFunctionMetadataAsync(forceRefresh).GetAwaiter().GetResult();
        }

        internal async Task<ImmutableArray<FunctionMetadata>> GetFunctionMetadataAsync(bool forceRefresh)
        {
            IEnumerable<RawFunctionMetadata> rawFunctions = new List<RawFunctionMetadata>();
            IEnumerable<FunctionMetadata> functions = new List<FunctionMetadata>();
            List<FunctionMetadata> validatedFunctions;
            _logger.FunctionMetadataProviderParsingFunctions();
            if (_functions.IsDefaultOrEmpty || forceRefresh)
            {
                if (_dispatcher != null)
                {
                    await _dispatcher.InitializeAsync(new List<FunctionMetadata>());
                    rawFunctions = await _dispatcher.GetWorkerMetadata();
                    functions = ValidateMetadata(rawFunctions);
                    if (functions.Count() == 0)
                    {
                        FunctionMetadata one = new FunctionMetadata()
                        {
                            Name = "hello there",
                            FunctionDirectory = "somethingRandom"
                        };
                        validatedFunctions = functions.ToList();
                        validatedFunctions.Add(one);
                        functions = validatedFunctions;
                    }
                    await _dispatcher.FinishInitialization(functions);
                }
            }
            _logger.FunctionMetadataProviderFunctionFound(functions.Count());
            _functions = functions.ToImmutableArray();
            return _functions;
        }

        internal IEnumerable<FunctionMetadata> ValidateMetadata(IEnumerable<RawFunctionMetadata> functions)
        {
            if (functions == null)
            {
                _logger.LogError("There is no metadata to be validated.");
                return null;
            }
            _functionErrors.Clear();
            List<FunctionMetadata> validatedMetadata = new List<FunctionMetadata>();
            foreach (RawFunctionMetadata rawFunction in functions)
            {
                var function = rawFunction.Metadata;
                // function name validation
                try
                {
                    ValidateName(function.Name);

                    function.Language = SystemEnvironment.Instance.GetEnvironmentVariable(RpcWorkerConstants.FunctionWorkerRuntimeSettingName);

                    // skip function directory validation because this involves reading function.json

                    // skip function ScriptFile validation for now because this involves enumerating file directory

                    // retry option validation
                    //function.Retry = rawFunction.Property(ConfigurationSectionNames.Retry)?.Value?.ToObject<RetryOptions>();
                    Utility.ValidateRetryOptions(function.Retry);

                    // add bindings
                    foreach (string binding in rawFunction.Bindings)
                    {
                        var functionBinding = BindingMetadata.Create(JObject.Parse(binding));
                        function.Bindings.Add(functionBinding);
                    }

                    // binding validation
                    if (function.Bindings == null || function.Bindings.Count == 0)
                    {
                        throw new FormatException("At least one binding must be declared.");
                    }

                    // add validated metadata to validated list if it gets this far
                    validatedMetadata.Add(function);
                }
                catch (Exception ex)
                {
                    Utility.AddFunctionError(_functionErrors, function.Name, Utility.FlattenException(ex, includeSource: false), isFunctionShortName: true);
                }
            }
            return validatedMetadata;
        }

        internal static void ValidateName(string name, bool isProxy = false)
        {
            if (!Utility.IsValidFunctionName(name))
            {
                throw new InvalidOperationException(string.Format("'{0}' is not a valid {1} name.", name, isProxy ? "proxy" : "function"));
            }
        }
    }
}
