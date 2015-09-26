﻿#region Copyright

// ****************************************************************************
// <copyright file="RelativeSourcePathMergerVisitor.cs">
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

using System.Collections.Generic;
using MugenMvvmToolkit.Binding.Interfaces.Parse;
using MugenMvvmToolkit.Binding.Interfaces.Parse.Nodes;
using MugenMvvmToolkit.Binding.Parse.Nodes;

namespace MugenMvvmToolkit.Binding.Parse
{
    public class RelativeSourcePathMergerVisitor : IExpressionVisitor
    {
        #region Fields

        public static readonly RelativeSourcePathMergerVisitor Instance;

        private static readonly ICollection<string> DefaultElementSourceAliases;

        private static readonly ICollection<string> DefaultRelativeSourceAliases;

        #endregion

        #region Constructors

        static RelativeSourcePathMergerVisitor()
        {
            Instance = new RelativeSourcePathMergerVisitor();
            DefaultElementSourceAliases = new[]
            {
                RelativeSourceExpressionNode.ElementSourceType,
                "Element",
                "El"
            };
            DefaultRelativeSourceAliases = new[]
            {
                RelativeSourceExpressionNode.RelativeSourceType,
                "Relative",
                "Rel"
            };
        }

        private RelativeSourcePathMergerVisitor()
        {
        }

        #endregion

        #region Properties

        private static ICollection<string> RelativeSourceAliases
        {
            get
            {
                var bindingParser = BindingServiceProvider.BindingProvider.Parser as BindingParser;
                if (bindingParser == null)
                    return DefaultRelativeSourceAliases;
                return bindingParser.RelativeSourceAliases;
            }
        }

        private static ICollection<string> ElementSourceAliases
        {
            get
            {
                var bindingParser = BindingServiceProvider.BindingProvider.Parser as BindingParser;
                if (bindingParser == null)
                    return DefaultElementSourceAliases;
                return bindingParser.ElementSourceAliases;
            }
        }


        #endregion

        #region Implementation of IExpressionVisitor

        public IExpressionNode Visit(IExpressionNode node)
        {
            var member = node as IMemberExpressionNode;
            if (member != null && member.Target is ResourceExpressionNode)
            {
                if (member.Member == "self" || member.Member == "this")
                    return new MemberExpressionNode(member.Target, BindingServiceProvider.ResourceResolver.SelfResourceName);
                if (member.Member == "context")
                    return new MemberExpressionNode(member.Target, BindingServiceProvider.ResourceResolver.DataContextResourceName);
                if (member.Member == "args" || member.Member == "arg")
                    return new MethodCallExpressionNode(member.Target, DefaultBindingParserHandler.GetEventArgsMethod, Empty.Array<IExpressionNode>(), Empty.Array<string>());
            }

            var nodes = new List<IExpressionNode>();
            var members = new List<string>();
            string memberName = node.TryGetMemberName(true, true, nodes, members);
            if (memberName == null)
            {
                var relativeExp = nodes[0] as IRelativeSourceExpressionNode;
                if (relativeExp != null)
                {
                    relativeExp.MergePath(BindingExtensions.MergePath(members));
                    return relativeExp;
                }

                var methodCall = nodes[0] as IMethodCallExpressionNode;
                if (methodCall != null && methodCall.Target is ResourceExpressionNode)
                {
                    if (RelativeSourceAliases.Contains(methodCall.Method))
                    {
                        if ((methodCall.Arguments.Count == 1 || methodCall.Arguments.Count == 2) &&
                            methodCall.Arguments[0] is IMemberExpressionNode)
                        {
                            int level = 1;
                            var relativeType = (IMemberExpressionNode)methodCall.Arguments[0];
                            if (methodCall.Arguments.Count == 2)
                                level = (int)((IConstantExpressionNode)methodCall.Arguments[1]).Value;
                            return RelativeSourceExpressionNode.CreateRelativeSource(relativeType.Member, (uint)level,
                                BindingExtensions.MergePath(members));
                        }
                    }

                    if (ElementSourceAliases.Contains(methodCall.Method))
                    {
                        if (methodCall.Arguments.Count == 1 && methodCall.Arguments[0] is IMemberExpressionNode)
                        {
                            var elementSource = (IMemberExpressionNode)methodCall.Arguments[0];
                            return RelativeSourceExpressionNode.CreateElementSource(elementSource.Member,
                                BindingExtensions.MergePath(members));
                        }
                    }
                }
            }
            return node;
        }

        #endregion
    }
}
