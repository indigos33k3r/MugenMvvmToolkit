﻿#region Copyright

// ****************************************************************************
// <copyright file="DataTemplateSelectorBase.cs">
// Copyright (c) 2012-2015 Vyacheslav Volkov
// </copyright>
// ****************************************************************************
// <author>Vyacheslav Volkov</author>
// <email>vvs0205@outlook.com</email>
// <project>MugenMvvmToolkit</project>
// <web>https://github.com/MugenMvvmToolkit/MugenMvvmToolkit</web>
// <license>
// See license.txt in this solution or http://opensource.org/licenses/MS-PL
// </license>
// ****************************************************************************

#endregion

using System;
using System.Threading;
using JetBrains.Annotations;
using MugenMvvmToolkit.Binding.Builders;
using MugenMvvmToolkit.Binding.Interfaces;

namespace MugenMvvmToolkit.Binding.Infrastructure
{
    public abstract class DataTemplateSelectorBase<TSource, TTemplate> : IDataTemplateSelector where TTemplate : class
    {
        #region Fields

        // ReSharper disable once StaticFieldInGenericType
        private static readonly bool IsTemplateObjectType;
        private BindingSet<TTemplate, TSource> _bindingSet;

        #endregion

        #region Constructors

        static DataTemplateSelectorBase()
        {
            IsTemplateObjectType = typeof(TTemplate) == typeof(object);
        }

        #endregion

        #region Implementation of IDataTemplateSelector

        public object SelectTemplate(object item, object container)
        {
            TTemplate template = SelectTemplate((TSource)item, container);
            if (template != null && CanInitialize(template, container))
            {
                if (_bindingSet == null)
                    Interlocked.CompareExchange(ref _bindingSet, new BindingSet<TTemplate, TSource>(template), null);
                bool lockTaken = false;
                try
                {
                    Monitor.Enter(_bindingSet, ref lockTaken);
                    _bindingSet.Target = template;
                    Initialize(template, _bindingSet);
                    _bindingSet.Apply();
                }
                finally
                {
                    _bindingSet.Target = null;
                    if (lockTaken)
                        Monitor.Exit(_bindingSet);
                }
            }
            return template;
        }

        #endregion

        #region Methods

        protected abstract TTemplate SelectTemplate(TSource item, object container);

        protected abstract void Initialize(TTemplate template, BindingSet<TTemplate, TSource> bindingSet);

        protected virtual bool CanInitialize([NotNull] TTemplate template, [NotNull] object container)
        {
            return !IsTemplateObjectType || !(template is ValueType);
        }

        #endregion
    }
}
