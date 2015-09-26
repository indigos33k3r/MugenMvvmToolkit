﻿#region Copyright

// ****************************************************************************
// <copyright file="EntityInitializedEventArgs.cs">
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
using JetBrains.Annotations;

namespace MugenMvvmToolkit.Models.EventArg
{
    public class EntityInitializedEventArgs : EventArgs
    {
        #region Fields

        private readonly object _entity;
        private readonly object _originalEntity;

        #endregion

        #region Constructors

        public EntityInitializedEventArgs([NotNull]object originalEntity, [NotNull] object entity)
        {
            Should.NotBeNull(originalEntity, "originalEntity");
            Should.NotBeNull(entity, "entity");
            _originalEntity = originalEntity;
            _entity = entity;
        }

        #endregion

        #region Properties

        [NotNull]
        public object OriginalEntity
        {
            get { return _originalEntity; }
        }

        [NotNull]
        public object Entity
        {
            get { return _entity; }
        }

        #endregion
    }

    public class EntityInitializedEventArgs<TEntity> : EntityInitializedEventArgs
    {
        #region Constructors

        public EntityInitializedEventArgs([NotNull]TEntity originalEntity, [NotNull] TEntity entity)
            : base(originalEntity, entity)
        {
        }

        #endregion

        #region Properties

        [NotNull]
        public new TEntity OriginalEntity
        {
            get { return (TEntity)base.OriginalEntity; }
        }

        [NotNull]
        public new TEntity Entity
        {
            get { return (TEntity)base.Entity; }
        }

        #endregion
    }
}
