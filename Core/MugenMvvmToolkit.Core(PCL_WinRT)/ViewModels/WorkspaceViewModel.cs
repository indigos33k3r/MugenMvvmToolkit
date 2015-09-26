﻿#region Copyright

// ****************************************************************************
// <copyright file="WorkspaceViewModel.cs">
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

using System.Threading.Tasks;
using MugenMvvmToolkit.Annotations;
using MugenMvvmToolkit.Interfaces.Navigation;
using MugenMvvmToolkit.Interfaces.ViewModels;
using MugenMvvmToolkit.Interfaces.Views;
using MugenMvvmToolkit.Models.Messages;

namespace MugenMvvmToolkit.ViewModels
{
    [BaseViewModel(Priority = 7)]
    public abstract class WorkspaceViewModel : WorkspaceViewModel<object>
    {
    }

    [BaseViewModel(Priority = 7)]
    public abstract class WorkspaceViewModel<TView> : CloseableViewModel, IWorkspaceViewModel, INavigableViewModel,
        IViewAwareViewModel<TView>
        where TView : class
    {
        #region Fields

        private string _displayName;
        private bool _isSelected;
        private TView _view;

        #endregion

        #region Constructors

        protected WorkspaceViewModel()
        {
            _isSelected = false;
        }

        #endregion

        #region Implementation of interfaces

        void INavigableViewModel.OnNavigatedTo(INavigationContext context)
        {
            OnNavigatedTo(context);
            //To invalidate command state.
            Publish(StateChangedMessage.Empty);
        }

        void INavigableViewModel.OnNavigatedFrom(INavigationContext context)
        {
            OnNavigatedFrom(context);
        }

        Task<bool> INavigableViewModel.OnNavigatingFrom(INavigationContext context)
        {
            return OnNavigatingFrom(context);
        }

        public TView View
        {
            get { return _view; }
            set
            {
                if (ReferenceEquals(value, _view))
                    return;
                TView oldValue = _view;
                _view = value;
                OnViewChanged(oldValue, value);
                OnPropertyChanged("View");
            }
        }

        public virtual string DisplayName
        {
            get { return _displayName; }
            set
            {
                if (value == _displayName) return;
                _displayName = value;
                OnPropertyChanged("DisplayName");
            }
        }

        public virtual bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                if (value.Equals(_isSelected)) return;
                _isSelected = value;
                OnPropertyChanged("IsSelected");
            }
        }

        #endregion

        #region Methods

        protected virtual Task<bool> OnNavigatingFrom(INavigationContext context)
        {
            return Empty.TrueTask;
        }

        protected virtual void OnNavigatedFrom(INavigationContext context)
        {
        }

        protected virtual void OnNavigatedTo(INavigationContext context)
        {
        }

        protected virtual void OnViewChanged(TView oldView, TView newView)
        {
        }

        #endregion
    }
}
