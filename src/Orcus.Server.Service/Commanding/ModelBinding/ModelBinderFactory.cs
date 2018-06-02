using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Orcus.Server.Service.Commanding.Binders;
using Orcus.Server.Service.Commanding.Formatters;
using Orcus.Server.Service.Commanding.ModelBinding.Binders;
using Orcus.Server.Service.Extensions;
using Orcus.Server.Service.Infrastructure;

namespace Orcus.Server.Service.Commanding.ModelBinding
{
    public class ModelBinderFactory : IModelBinderFactory
    {
        private readonly ConcurrentDictionary<Key, IModelBinder> _cache;
        private readonly IModelBinderProvider[] _providers;
        private readonly IServiceProvider _serviceProvider;

        public ModelBinderFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;

            var formatters = new List<IInputFormatter>();

            _providers = new IModelBinderProvider[]
            {
                new ServicesModelBinderProvider(),
                //TODO new BodyModelBinderProvider(formatters, new MemoryPoolHttpRequestStreamReaderFactory()),
                new EnumTypeModelBinderProvider(),
                new SimpleTypeModelBinderProvider(),
                new ByteArrayModelBinderProvider()

            };
            _cache = new ConcurrentDictionary<Key, IModelBinder>();
        }

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