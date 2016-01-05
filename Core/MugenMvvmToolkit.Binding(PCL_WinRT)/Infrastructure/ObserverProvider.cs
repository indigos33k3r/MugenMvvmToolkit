﻿#region Copyright

// ****************************************************************************
// <copyright file="ObserverProvider.cs">
// Copyright (c) 2012-2016 Vyacheslav Volkov
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

using MugenMvvmToolkit.Binding.Interfaces;
using MugenMvvmToolkit.Binding.Interfaces.Models;

namespace MugenMvvmToolkit.Binding.Infrastructure
{
    public class ObserverProvider : IObserverProvider
    {
        #region Implementation of IObserverProvider

        public virtual IObserver Observe(object target, IBindingPath path, bool ignoreAttachedMembers)
        {
            Should.NotBeNull(target, nameof(target));
            Should.NotBeNull(path, nameof(path));
            if (path.IsSingle)
                return new SinglePathObserver(target, path, ignoreAttachedMembers);
            if (path.IsEmpty)
                return new EmptyPathObserver(target, path);
            return new MultiPathObserver(target, path, ignoreAttachedMembers);
        }

        #endregion
    }
}
