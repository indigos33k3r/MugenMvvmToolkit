﻿#region Copyright

// ****************************************************************************
// <copyright file="IEntityStateEntry.cs">
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

using JetBrains.Annotations;
using MugenMvvmToolkit.Models;

namespace MugenMvvmToolkit.Interfaces.Models
{
    public interface IEntityStateEntry
    {
        EntityState State { get; }

        [NotNull]
        object Entity { get; }
    }
}
