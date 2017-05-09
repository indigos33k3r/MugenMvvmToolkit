﻿#region Copyright

// ****************************************************************************
// <copyright file="WpfToolkitExtensions.cs">
// Copyright (c) 2012-2017 Vyacheslav Volkov
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
using MugenMvvmToolkit.Models;

namespace MugenMvvmToolkit.WPF
{
    public static class WpfToolkitExtensions
    {
        #region Methods

        internal static PlatformInfo GetPlatformInfo()
        {
            return new PlatformInfo(PlatformType.WPF, Environment.Version.ToString(), PlatformIdiom.Desktop);
        }

        #endregion
    }
}