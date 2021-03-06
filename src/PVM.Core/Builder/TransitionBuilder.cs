﻿#region License

// -------------------------------------------------------------------------------
//  <copyright file="TransitionBuilder.cs" company="PVM.NET Project Contributors">
//    Copyright (c) 2015 PVM.NET Project Contributors
//    Authors: Dominik Schlosser (dominik.schlosser@gmail.com)
//            
//    Licensed under the Apache License, Version 2.0 (the "License");
//    you may not use this file except in compliance with the License.
//    You may obtain a copy of the License at
// 
//      http://www.apache.org/licenses/LICENSE-2.0
// 
//    Unless required by applicable law or agreed to in writing, software
//    distributed under the License is distributed on an "AS IS" BASIS,
//    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//    See the License for the specific language governing permissions and
//    limitations under the License.
//  </copyright>
// -------------------------------------------------------------------------------

#endregion

using System;

namespace PVM.Core.Builder
{
    public class TransitionBuilder
    {
        private readonly NodeBuilder parentNodeBuilder;
        private readonly string source;
        private bool isDefault;
        private string name = Guid.NewGuid().ToString();
        private string target;

        public TransitionBuilder(NodeBuilder parentNodeBuilder, string source)
        {
            this.parentNodeBuilder = parentNodeBuilder;
            this.source = source;
        }

        protected string Target
        {
            get { return target; }
        }

        protected string Name
        {
            get { return name; }
        }

        protected bool IsDefaultValue
        {
            get { return isDefault; }
        }

        protected string Source
        {
            get { return source; }
        }

        protected NodeBuilder ParentNodeBuilder
        {
            get { return parentNodeBuilder; }
        }

        public TransitionBuilder WithName(string name)
        {
            this.name = name;

            return this;
        }

        public TransitionBuilder To(string targetNode)
        {
            target = targetNode;

            return this;
        }

        public TransitionBuilder IsDefault()
        {
            isDefault = true;

            return this;
        }

        public virtual NodeBuilder BuildTransition()
        {
            parentNodeBuilder.AddTransition(new TransitionData(name, isDefault, target, source));
            return parentNodeBuilder;
        }
    }
}