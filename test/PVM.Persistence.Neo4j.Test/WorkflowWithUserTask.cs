﻿#region License
// -------------------------------------------------------------------------------
//  <copyright file="WorkflowWithUserTask.cs" company="PVM.NET Project Contributors">
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

using NUnit.Framework;
using PVM.Core.Builder;
using PVM.Persistence.Neo4j;
using PVM.Persistence.Neo4j.Test;

namespace PVM.Persistence.Sql.Test
{
    [Ignore]
    [TestFixture]
    public class WorkflowWithUserTask : Neo4jTestBase
    {
        [Test]
        public void SingleTask()
        {
            var builder = new WorkflowDefinitionBuilder();

            var workflowDefinition = builder
                .AddNode()
                    .WithName("myTask")
                    .IsStartNode()                    
                    .AddTransition()
                        .WithName("transition")
                        .To("end")
                    .BuildTransition()
                .BuildUserTask()
                .AddNode()
                    .WithName("end")
                    .IsEndNode()
                .BuildNode()
                .BuildWorkflow();

            var workflowEngine = new WorkflowEngineBuilder().ConfigureServiceLocator().ImportModule(new Neo4jPersistenceModule()).Build();
            var instance = workflowEngine.StartNewInstance(workflowDefinition);

            Assert.False(instance.IsFinished);

            var userTask = workflowEngine.FindTask("myTask", instance.WorkflowInstanceIdentifier);
            workflowEngine.Complete(userTask);

            instance = workflowEngine.Load(instance.Identifier);
            Assert.That(instance.IsFinished);
        }

        [Test]
        public void TwoParallelTasks()
        {
            var builder = new WorkflowDefinitionBuilder();

            var workflowDefinition = builder
                .AddNode()
                    .WithName("start")
                    .IsStartNode()
                    .AddTransition()
                        .To("task1")
                   .BuildTransition()
                   .AddTransition()
                        .To("task2")
                    .BuildTransition()
                .BuildParallelGateway()
                .AddNode()
                    .WithName("task1")
                    .AddTransition()
                        .To("join")
                    .BuildTransition()
                .BuildUserTask()
                .AddNode()
                    .WithName("task2")
                    .AddTransition()
                        .To("join")
                    .BuildTransition()
                .BuildUserTask()
                .AddNode()
                    .WithName("join")
                    .AddTransition()
                        .To("end")
                    .BuildTransition()
                .BuildParallelGateway()
                .AddNode()
                    .WithName("end")
                    .IsEndNode()
                .BuildNode()
                .BuildWorkflow();

            var workflowEngine = new WorkflowEngineBuilder().ConfigureServiceLocator().ImportModule(new Neo4jPersistenceTestModule(GraphClient)).Build();
            var instance = workflowEngine.StartNewInstance(workflowDefinition);

            Assert.False(instance.IsFinished);

            var userTask = workflowEngine.FindTask("task1", instance.WorkflowInstanceIdentifier);
            workflowEngine.Complete(userTask);

            Assert.False(instance.IsFinished);

            var userTask2 = workflowEngine.FindTask("task2", instance.WorkflowInstanceIdentifier);
            workflowEngine.Complete(userTask2);

            instance = workflowEngine.Load(instance.Identifier);
            Assert.That(instance.IsFinished);
        }

        [Test]
        public void JoinWaitsOnNestedTask()
        {
            var builder = new WorkflowDefinitionBuilder();

            var workflowDefinition = builder
                .AddNode()
                    .WithName("start")
                    .IsStartNode()
                    .AddTransition()
                        .To("task1")
                   .BuildTransition()
                   .AddTransition()
                        .To("path2")
                    .BuildTransition()
                .BuildParallelGateway()
                .AddNode()
                    .WithName("task1")
                    .AddTransition()
                        .To("join")
                    .BuildTransition()
                .BuildUserTask()
                .AddNode()
                    .WithName("path2")
                    .AddTransition()
                        .To("nestedSplit")
                    .BuildTransition()
                .BuildNode()
                .AddNode()
                    .WithName("nestedSplit")
                    .AddTransition()
                        .To("nestedTask")
                    .BuildTransition()
                    .AddTransition()
                        .To("nestedPath2")
                    .BuildTransition()
                .BuildParallelGateway()
                .AddNode()
                    .WithName("nestedTask")
                    .AddTransition()
                        .To("join")
                    .BuildTransition()
                .BuildUserTask()
                .AddNode()
                    .WithName("nestedPath2")
                    .AddTransition()
                        .To("join")
                    .BuildTransition()
                .BuildNode()
                .AddNode()
                    .WithName("join")
                    .AddTransition()
                        .To("end")
                    .BuildTransition()
                .BuildParallelGateway()
                .AddNode()
                    .WithName("end")
                    .IsEndNode()
                .BuildNode()
                .BuildWorkflow();

            var workflowEngine = new WorkflowEngineBuilder().ConfigureServiceLocator().ImportModule(new Neo4jPersistenceTestModule(GraphClient)).Build();
            var instance = workflowEngine.StartNewInstance(workflowDefinition);

            Assert.False(instance.IsFinished);

            var userTask = workflowEngine.FindTask("task1", instance.WorkflowInstanceIdentifier);
            workflowEngine.Complete(userTask);

            instance = workflowEngine.Load(instance.Identifier);
            Assert.False(instance.IsFinished);

            var nestedTask = workflowEngine.FindTask("nestedTask", instance.WorkflowInstanceIdentifier);
            workflowEngine.Complete(nestedTask);

            instance = workflowEngine.Load(instance.Identifier);
            Assert.That(instance.IsFinished);
        }
    }
}