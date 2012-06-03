﻿// -----------------------------------------------------------------------
// <copyright file="SqlQuery.cs" company="MicroLite">
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
namespace MicroLite
{
    using System.Collections.Generic;

    /// <summary>
    /// A class which represents a parameterised SQL query.
    /// </summary>
    public sealed class SqlQuery
    {
        private readonly List<object> arguments = new List<object>();
        private string commandText;

        /// <summary>
        /// Initialises a new instance of the <see cref="SqlQuery"/> class with the supplied command text and parameter values.
        /// </summary>
        /// <param name="commandText">The SQL command text.</param>
        /// <param name="arguments">The parameter values for the query.</param>
        public SqlQuery(string commandText, params object[] arguments)
        {
            this.commandText = commandText;
            this.arguments.AddRange(arguments ?? new object[0]);
        }

        /// <summary>
        /// Gets the parameter values of the SQL statement or stored procedure.
        /// </summary>
        public IList<object> Arguments
        {
            get
            {
                return this.arguments;
            }
        }

        /// <summary>
        /// Gets or sets the SQL statement or stored procedure to execute at the data source.
        /// </summary>
        public string CommandText
        {
            get
            {
                return this.commandText;
            }

            set
            {
                this.commandText = value;
            }
        }
    }
}