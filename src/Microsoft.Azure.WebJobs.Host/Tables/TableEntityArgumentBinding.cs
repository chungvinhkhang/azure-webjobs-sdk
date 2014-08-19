﻿// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs.Host.Bindings;
using Microsoft.WindowsAzure.Storage.Table;

namespace Microsoft.Azure.WebJobs.Host.Tables
{
    internal class TableEntityArgumentBinding<TElement> : IArgumentBinding<TableEntityContext>
        where TElement : ITableEntity, new()
    {
        public Type ValueType
        {
            get { return typeof(TElement); }
        }

        public async Task<IValueProvider> BindAsync(TableEntityContext value, ValueBindingContext context)
        {
            TableOperation retrieve = TableOperation.Retrieve<TElement>(value.PartitionKey, value.RowKey);
            TableResult result = await value.Table.ExecuteAsync(retrieve, context.CancellationToken);
            TElement entity = (TElement)result.Result;

            if (entity == null)
            {
                return new NullEntityValueProvider(value, typeof(TElement));
            }

            return new TableEntityValueBinder(value, entity, typeof(TElement));
        }
    }
}