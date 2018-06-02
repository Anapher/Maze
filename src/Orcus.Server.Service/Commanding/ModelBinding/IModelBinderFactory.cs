// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Orcus.Server.Service.Commanding.Binders;

namespace Orcus.Server.Service.Commanding.ModelBinding
{
    /// <summary>
    ///     A factory abstraction for creating <see cref="IModelBinder" /> instances.
    /// </summary>
    public interface IModelBinderFactory
    {
        /// <summary>
        ///     Creates a new <see cref="IModelBinder" />.
        /// </summary>
        /// <param name="context">The <see cref="ModelBinderFactoryContext" />.</param>
        /// <returns>An <see cref="IModelBinder" /> instance.</returns>
        IModelBinder CreateBinder(ModelBinderFactoryContext context);
    }

    /// <summary>
    ///     A context object for <see cref="IModelBinderFactory.CreateBinder" />.
    /// </summary>
    public class ModelBinderFactoryContext
    {
        /// <summary>
        /// Gets or sets the <see cref="ModelBinding.BindingInfo"/>.
        /// </summary>
        public BindingInfo BindingInfo { get; set; }

        /// <summary>
        ///     Gets or sets the <see cref="ModelMetadata" />.
        /// </summary>
        public ModelMetadata Metadata { get; set; }

        /// <summary>
        /// Gets or sets the cache token. If <c>non-null</c> the resulting <see cref="IModelBinder"/>
        /// will be cached.
        /// </summary>
        public object CacheToken { get; set; }
    }
}