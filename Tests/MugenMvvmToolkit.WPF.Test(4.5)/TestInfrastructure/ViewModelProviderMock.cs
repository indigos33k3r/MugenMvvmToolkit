﻿using System;
using MugenMvvmToolkit.Interfaces;
using MugenMvvmToolkit.Interfaces.Models;
using MugenMvvmToolkit.Interfaces.ViewModels;
using MugenMvvmToolkit.Models;

namespace MugenMvvmToolkit.Test.TestInfrastructure
{
    public class ViewModelProviderMock : IViewModelProvider
    {
        #region Proeprties

        public Func<GetViewModelDelegate<IViewModel>, IDataContext, IViewModel> GetViewModel { get; set; }

        public Func<Type, IDataContext, IViewModel> GetViewModelType { get; set; }

        public Action<IViewModel, IDataContext> InitializeViewModel { get; set; }

        #endregion

        #region Implementation of IViewModelProvider

        IViewModel IViewModelProvider.GetViewModel(GetViewModelDelegate<IViewModel> getViewModel,
            IDataContext dataContext)
        {
            return GetViewModel(getViewModel, dataContext);
        }

        IViewModel IViewModelProvider.GetViewModel(Type viewModelType, IDataContext dataContext)
        {
            return GetViewModelType(viewModelType, dataContext);
        }

        void IViewModelProvider.InitializeViewModel(IViewModel viewModel, IDataContext dataContext)
        {
            InitializeViewModel(viewModel, dataContext);
        }

        public IDataContext PreserveViewModel(IViewModel viewModel, IDataContext dataContext)
        {
            throw new NotImplementedException();
        }

        public IViewModel RestoreViewModel(IDataContext viewModelState, IDataContext dataContext, bool throwOnError)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
