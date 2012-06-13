﻿// -----------------------------------------------------------------------
// <copyright file="GuidListener.cs" company="MicroLite">
// Copyright 2012 Trevor Pilley
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// </copyright>
// -----------------------------------------------------------------------
namespace MicroLite.Listeners
{
    using System;
    using MicroLite.Logging;

    /// <summary>
    /// The implementation of <see cref="IListener"/> for setting the instance identifier value if
    /// <see cref="IdentifierStrategy"/>.Guid is used.
    /// </summary>
    internal sealed class GuidListener : Listener
    {
        private static readonly ILog log = LogManager.GetLog("MicroLite.GuidListener");

        public override void BeforeInsert(object instance)
        {
            var objectInfo = ObjectInfo.For(instance.GetType());

            if (objectInfo.TableInfo.IdentifierStrategy == IdentifierStrategy.Guid)
            {
                var propertyInfo = objectInfo.GetPropertyInfoForColumn(objectInfo.TableInfo.IdentifierColumn);

                var identifierValue = Guid.NewGuid();

                log.TryLogDebug(LogMessages.IListener_SettingIdentifierValue, objectInfo.ForType.FullName, identifierValue.ToString());
                propertyInfo.SetValue(instance, identifierValue, null);
            }
        }

        public override void BeforeUpdate(object instance)
        {
            var objectInfo = ObjectInfo.For(instance.GetType());

            if (objectInfo.TableInfo.IdentifierStrategy == IdentifierStrategy.Guid)
            {
                if (objectInfo.HasDefaultIdentifierValue(instance))
                {
                    throw new MicroLiteException(Messages.IListener_IdentifierNotSetForUpdate);
                }
            }
        }
    }
}