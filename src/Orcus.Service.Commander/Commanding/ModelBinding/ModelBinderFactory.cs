// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.Options;
using Orcus.Modules.Api.ModelBinding;
using Orcus.Service.Commander.Commanding.Formatters.Abstractions;
using Orcus.Service.Commander.Extensions;

namespace Orcus.Service.Commander.Commanding.ModelBinding
{
    /// <summary>
    ///     A factory for <see cref="IModelBinder" /> instances.
    /// </summary>
    public class ModelBinderFactory : IModelBinderFactory
    {
        private readonly ConcurrentDictionary<Key, IModelBinder> _cache;
        private readonly IModelBinderProvider[] _providers;
        private readonly IServiceProvider _serviceProvider;

        /// <summary>
        ///     Creates a new <see cref="ModelBinderFactory" />.
        /// </summary>
        /// <param name="options">The <see cref="IOptions{TOptions}" /> for <see cref="Orcus.Service.Commander.OrcusServerOptions" />.</param>
        /// <param name="serviceProvider">The <see cref="IServiceProvider" />.</param>
        public ModelBinderFactory(IOptions<OrcusServerOptions> options, IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;

            _providers = options.Value.ModelBinderProviders.ToArray();
            _cache = new ConcurrentDictionary<Key, IModelBinder>();
        }

        /// <inheritdoc />
        public IModelBinder CreateBinder(ModelBinderFactoryContext context)
        {
            if (TryGetCachedBinder(context.Metadata, context.CacheToken, out var binder))
                return binder;

            var providerContext = new DefaultModelBinderProviderContext(this, context);

            foreach (var provider in _providers)
            {
                binder = provider.GetBinder(providerContext);
                if (binder != null)
                    break;
            }

            if (binder == null)
                throw new InvalidOperationException(
                    $"Could not find binder for model type {context.Metadata.ModelType}");

            AddToCache(context.Metadata, context.CacheToken, binder);
            return binder;
        }

        private void AddToCache(ModelMetadata metadata, object cacheToken, IModelBinder binder)
        {
            if (cacheToken == null)
                return;

            _cache.TryAdd(new Key(metadata, cacheToken), binder);
        }

        private bool TryGetCachedBinder(ModelMetadata metadata, object cacheToken, out IModelBinder binder)
        {
            if (cacheToken == null)
            {
                binder = null;
                return false;
            }

            return _cache.TryGetValue(new Key(metadata, cacheToken), out binder);
        }

        private class DefaultModelBinderProviderContext : ModelBinderProviderContext
        {
            public DefaultModelBinderProviderContext(ModelBinderFactory factory, ModelBinderFactoryContext context)
            {
                Services = factory._serviceProvider;
                Metadata = context.Metadata;
            }

            public override ModelMetadata Metadata { get; }
            public override IServiceProvider Services { get; }
        }

        // This key allows you to specify a ModelMetadata which represents the type/property being bound
        // and a 'token' which acts as an arbitrary discriminator.
        //
        // This is necessary because the same metadata might be bound as a top-level parameter (with BindingInfo on
        // the ParameterDescriptor) or in a call to TryUpdateModel (no BindingInfo) or as a collection element.
        //
        // We need to be able to tell the difference between these things to avoid over-caching.
        private struct Key : IEquatable<Key>
        {
            private readonly ModelMetadata _metadata;
            private readonly object _token; // Explicitly using ReferenceEquality for tokens.

            public Key(ModelMetadata metadata, object token)
            {
                _metadata = metadata;
                _token = token;
            }

            public bool Equals(Key other)
            {
                return _metadata.Equals(other._metadata) && ReferenceEquals(_token, other._token);
            }

            public override bool Equals(object obj)
            {
                return obj is Key other && Equals(other);
            }

            public override int GetHashCode()
            {
                var hash = new HashCodeCombiner();
                hash.AddObject(_metadata);
                hash.AddObject(RuntimeHelpers.GetHashCode(_token));
                return hash.CombinedHash;
            }
        }
    }
}