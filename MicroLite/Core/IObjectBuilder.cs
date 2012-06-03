﻿// -----------------------------------------------------------------------
// <copyright file="IObjectBuilder.cs" company="MicroLite">
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
namespace MicroLite.Core
{
    using System.Data;

    /// <summary>
    /// The interface for a class which builds an object instance from the values in a <see cref="IDataReader"/>.
    /// </summary>
    internal interface IObjectBuilder
    {
        /// <summary>
        /// Builds the new instance.
        /// </summary>
        /// <typeparam name="T">The type of object to be built.</typeparam>
        /// <param name="reader">The <see cref="IDataReader"/> containing the values to populate the object with.</param>
        /// <returns>
        /// The new instance populated with the values from the <see cref="IDataReader"/>.
        /// </returns>
        T BuildNewInstance<T>(IDataReader reader) where T : class, new();
    }
}