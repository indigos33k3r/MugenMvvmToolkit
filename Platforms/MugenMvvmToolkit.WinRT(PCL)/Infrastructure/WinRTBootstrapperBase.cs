﻿#region Copyright

// ****************************************************************************
// <copyright file="WinRTBootstrapperBase.cs">
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
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using JetBrains.Annotations;
using MugenMvvmToolkit.Infrastructure;
using MugenMvvmToolkit.Infrastructure.Presenters;
using MugenMvvmToolkit.Interfaces;
using MugenMvvmToolkit.Interfaces.Models;
using MugenMvvmToolkit.Interfaces.Presenters;
using MugenMvvmToolkit.Interfaces.ViewModels;
using MugenMvvmToolkit.Models;
using MugenMvvmToolkit.ViewModels;
using MugenMvvmToolkit.WinRT.Infrastructure.Navigation;
using MugenMvvmToolkit.WinRT.Interfaces.Navigation;

namespace MugenMvvmToolkit.WinRT.Infrastructure
{
    public abstract class WinRTBootstrapperBase : BootstrapperBase
    {
        #region Fields

        protected const string BindingAssemblyName = "MugenMvvmToolkit.WinRT.Binding";
        private readonly Frame _rootFrame;
        private readonly bool _overrideAssemblies;
        private readonly PlatformInfo _platform;
        private List<Assembly> _assemblies;

        #endregion

        #region Constructors

        static WinRTBootstrapperBase()
        {
            DynamicMultiViewModelPresenter.CanShowViewModelDefault = CanShowViewModelTabPresenter;
            DynamicViewModelNavigationPresenter.CanShowViewModelDefault = CanShowViewModelNavigationPresenter;
        }

        protected WinRTBootstrapperBase([NotNull] Frame rootFrame, bool overrideAssemblies, PlatformInfo platform = null)
        {
            Should.NotBeNull(rootFrame, "rootFrame");
            _rootFrame = rootFrame;
            _overrideAssemblies = overrideAssemblies;
            _platform = platform ?? PlatformExtensions.GetPlatformInfo();
        }

        #endregion

        #region Properties

        protected Frame RootFrame
        {
            get { return _rootFrame; }
        }

        #endregion

        #region Overrides of BootstrapperBase

        protected override void InitializeInternal()
        {
            var application = CreateApplication();
            var iocContainer = CreateIocContainer();
            application.Initialize(_platform, iocContainer, GetAssemblies().ToArrayEx(), InitializationContext ?? DataContext.Empty);
            var service = CreateNavigationService(_rootFrame);
            if (service != null)
                iocContainer.BindToConstant(service);
        }

        #endregion

        #region Methods

        public virtual void Start()
        {
            Initialize();
            var app = MvvmApplication.Current;
            var ctx = new DataContext(app.Context);
            var viewModelType = app.GetStartViewModelType();
            var viewModel = app.IocContainer
               .Get<IViewModelProvider>()
               .GetViewModel(viewModelType, ctx);
            viewModel.ShowAsync((model, result) => model.Dispose(), context: ctx);
        }

        public async Task InitializeAsync()
        {
            if (!_overrideAssemblies)
                _assemblies = await GetAssembliesAsync();
            Initialize();
        }

        protected virtual ICollection<Assembly> GetAssemblies()
        {
            if (_assemblies != null)
                return _assemblies;
            var assemblies = new HashSet<Assembly> { GetType().GetAssembly(), typeof(WinRTBootstrapperBase).GetAssembly() };
            var application = Application.Current;
            if (application != null)
                assemblies.Add(application.GetType().GetAssembly());
            TryLoadAssembly(BindingAssemblyName, assemblies);
            return assemblies;
        }

        [CanBeNull]
        protected virtual INavigationService CreateNavigationService(Frame frame)
        {
            return new FrameNavigationService(frame);
        }

        private static bool CanShowViewModelTabPresenter(IViewModel viewModel, IDataContext dataContext, IViewModelPresenter arg3)
        {
            var viewName = viewModel.GetViewName(dataContext);
            var container = viewModel.GetIocContainer(true);
            var mappingProvider = container.Get<IViewMappingProvider>();
            var mappingItem = mappingProvider.FindMappingForViewModel(viewModel.GetType(), viewName, false);
            return mappingItem == null || !typeof(Page).IsAssignableFrom(mappingItem.ViewType);
        }

        private static bool CanShowViewModelNavigationPresenter(IViewModel viewModel, IDataContext dataContext, IViewModelPresenter arg3)
        {
            var viewName = viewModel.GetViewName(dataContext);
            var container = viewModel.GetIocContainer(true);
            var mappingProvider = container.Get<IViewMappingProvider>();
            var mappingItem = mappingProvider.FindMappingForViewModel(viewModel.GetType(), viewName, false);
            return mappingItem != null && typeof(Page).IsAssignableFrom(mappingItem.ViewType);
        }

        private static async Task<List<Assembly>> GetAssembliesAsync()
        {
            var assemblies = new List<Assembly>();
            var files = await Windows.ApplicationModel.Package.Current.InstalledLocation.GetFilesAsync();
            foreach (var file in files)
            {
                try
                {
                    if ((file.FileType == ".dll") || (file.FileType == ".exe"))
                    {
                        var name = new AssemblyName { Name = Path.GetFileNameWithoutExtension(file.Name) };
                        Assembly asm = Assembly.Load(name);
                        if (asm.IsToolkitAssembly())
                            assemblies.Add(asm);
                    }

                }
                catch
                {
                    ;
                }
            }
            return assemblies;
        }

        #endregion
    }
}
