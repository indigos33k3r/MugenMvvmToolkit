﻿#region Copyright
// ****************************************************************************
// <copyright file="Binder.cs">
// Copyright © Vyacheslav Volkov 2012-2014
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
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing.Design;
using System.Reflection;
using System.Windows.Forms;
using System.Xml.Linq;
using JetBrains.Annotations;
using MugenMvvmToolkit.Binding.Interfaces;

namespace MugenMvvmToolkit.Binding.UiDesigner
{
    /// <summary>
    ///     Represents the component that provides a data binding for controls.
    /// </summary>
    [Description("Provides a data binding for controls."), ToolboxItem(true)]
    public class Binder : Component, ISupportInitialize
    {
        #region Fields

        private readonly Dictionary<object, Dictionary<string, string>> _controlBindings;
        private string _bindings;
        private readonly List<IDataBinding> _dataBindings;

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="Binder" /> class.
        /// </summary>
        public Binder()
        {
            _controlBindings = new Dictionary<object, Dictionary<string, string>>();
            _dataBindings = new List<IDataBinding>();
            RootTagName = "Bindings";
            IgnoreControlException = true;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Binder" /> class.
        /// </summary>
        public Binder([NotNull] IContainer container)
            : this()
        {
            Should.NotBeNull(container, "container");
            container.Add(this);
        }

        #endregion

        #region Properties

        public string RootTagName { get; set; }

        public bool IgnoreControlException { get; set; }

        public ContainerControl ContainerControl { get; set; }

        [Editor(typeof(BindingEditorUITypeEditor), typeof(UITypeEditor))]
        public string Bindings
        {
            get
            {
                BindingEditorView.CurrentControl = ContainerControl;
                if (DesignMode && string.IsNullOrEmpty(_bindings))
                    return string.Format(@"<{0}>
</{0}>", RootTagName);
                return _bindings;
            }
            set
            {
                _bindings = value;
                if (DesignMode)
                    SetBindings(value);
            }
        }

        #endregion

        #region Methods

        private void BindControls()
        {
            ClearBindings();
            SetBindings(Bindings);
            IBindingProvider bindingProvider = BindingServiceProvider.BindingProvider;
            foreach (var controlBinding in _controlBindings)
            {
                string value;
                if (controlBinding.Value.TryGetValue(AttachedMemberConstants.DataContext, out value))
                {
                    controlBinding.Value.Remove(AttachedMemberConstants.DataContext);
                    _dataBindings.Add(bindingProvider.CreateBindingFromString(controlBinding.Key, AttachedMemberConstants.DataContext, value));
                }
                foreach (var binding in controlBinding.Value)
                    _dataBindings.Add(bindingProvider.CreateBindingFromString(controlBinding.Key, binding.Key, binding.Value));
            }
        }

        private void SetBindings(string bindingsString)
        {
            _controlBindings.Clear();
            if (string.IsNullOrEmpty(bindingsString))
                return;
            try
            {
                XElement xElement = XElement.Parse(bindingsString);
                XElement element = xElement.Name == RootTagName ? xElement : xElement.Element(RootTagName);
                if (element == null)
                    throw new ArgumentException(string.Format("The root tag: {0} is not found.", RootTagName),
                        "bindingsString");
                foreach (XElement descendant in element.Descendants())
                    UpdateControlBinding(descendant);
            }
            catch (Exception e)
            {
                if (DesignMode)
                    MessageBox.Show(e.Flatten(false));
                else
                    Tracer.Error(e.Flatten(false));
            }
        }

        private void UpdateControlBinding(XElement element)
        {
            var name = element.Name.LocalName;
            object component = FindComponent(name);
            bool throwOnError = !DesignMode || !IgnoreControlException;
            if (component == null)
            {
                var msg = string.Format("The control with name '{0}' is not found", name);
                if (throwOnError)
                    throw new ArgumentException(msg);
                Tracer.Error(msg);
                return;
            }

            Dictionary<string, string> bindings;
            if (!_controlBindings.TryGetValue(component, out bindings))
            {
                bindings = new Dictionary<string, string>();
                _controlBindings[component] = bindings;
            }
            foreach (XAttribute attribute in element.Attributes())
                bindings[attribute.Name.LocalName] = attribute.Value;
        }

        private object FindComponent(string name)
        {
            if (DesignMode)
            {
                var container = Site.Container;
                if (container == null)
                    return null;
                for (int i = 0; i < container.Components.Count; i++)
                {
                    var cmp = container.Components[i];
                    if (PlatformExtensions.TryGetValue(cmp, "Name") == name)
                        return cmp;
                }
                return null;
            }
            if (ContainerControl == null || ContainerControl.Name == name)
                return ContainerControl;
            var findByName = BindingServiceProvider.VisualTreeManager.FindByName(ContainerControl, name);
            if (findByName != null)
                return findByName;

            var type = ContainerControl.GetType();
            var field = type.GetField(name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            if (field == null)
                return null;
            return field.GetValue(ContainerControl);
        }

        private void ClearBindings()
        {
            for (int i = 0; i < _dataBindings.Count; i++)
                _dataBindings[i].Dispose();
            _dataBindings.Clear();
        }

        #endregion

        #region Overrides of Component

        public override ISite Site
        {
            get { return base.Site; }
            set
            {
                base.Site = value;
                if (value == null)
                    return;
                var host = value.GetService<IDesignerHost>();
                if (host == null)
                    return;
                IComponent componentHost = host.RootComponent;
                ContainerControl = componentHost as ContainerControl;
            }
        }
        
        protected override void Dispose(bool disposing)
        {
            if (disposing)
                ClearBindings();
            base.Dispose(disposing);
        }

        #endregion

        #region Implementation of interfaces

        public virtual void BeginInit()
        {
        }

        public virtual void EndInit()
        {
            if (DesignMode)
                SetBindings(Bindings);
            else
                BindControls();
        }

        #endregion
    }
}