﻿// -----------------------------------------------------------------------
// <copyright file="ListenerManager.cs" company="MicroLite">
// Copyright 2012 Trevor Pilley
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//    http://www.apache.org/licenses/LICENSE-2.0
//
// </copyright>
// -----------------------------------------------------------------------
namespace MicroLite.Listeners
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// The class which manages the IListeners used by the MicroLite ORM framework.
    /// </summary>
    internal static class ListenerManager
    {
        private static readonly IList<Func<IListener>> listenerFactories = new List<Func<IListener>>();
        private static readonly IList<Type> listenerTypes = new List<Type>();

        /// <summary>
        /// Adds the listener.
        /// </summary>
        /// <typeparam name="T">The type of <see cref="IListener"/> to add.</typeparam>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Justification = "The syntax is accepted practice for registering types.")]
        internal static void Add<T>()
            where T : IListener, new()
        {
            var listenerType = typeof(T);

            if (!listenerTypes.Contains(listenerType))
            {
                Add(() =>
                {
                    return new T();
                });

                listenerTypes.Add(listenerType);
            }
        }

        /// <summary>
        /// Adds the specified listener factory.
        /// </summary>
        /// <param name="listenerFactory">The listener factory.</param>
        /// <remarks>This method exists so that we can unit test with Mock IListeners, it should remain internal.</remarks>
        internal static void Add(Func<IListener> listenerFactory)
        {
            listenerFactories.Add(listenerFactory);
        }

        internal static void Clear()
        {
            listenerFactories.Clear();
            listenerTypes.Clear();
        }

        internal static IEnumerable<IListener> Create()
        {
            return listenerFactories.Select(x => x()).ToArray();
        }
    }
}