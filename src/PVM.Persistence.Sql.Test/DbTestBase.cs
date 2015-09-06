﻿#region License

// -------------------------------------------------------------------------------
//  <copyright file="DbTestBase.cs" company="PVM.NET Project Contributors">
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

using System.Data.Entity.Migrations;
using NUnit.Framework;
using PVM.Persistence.Sql.Migrations;

namespace PVM.Persistence.Sql.Test
{
    public abstract class DbTestBase
    {
        protected PvmContext TestDbContext { get; private set; }

        [TestFixtureSetUp]
        public void Setup()
        {
            new DbMigrator(new Configuration()).Update();
        }

        [SetUp]
        public void TestSetUp()
        {
            ResetDb();
            TestDbContext = new PvmContext();
        }

        [TearDown]
        public void TestTearDown()
        {
            TestDbContext.Dispose();
            TestDbContext = null;
        }

        private void ResetDb()
        {
            string[] allTableNames =
            {
                "TransitionModels",
                "NodeModels",
                "ExecutionVariableModels",
                "ExecutionModels"
            };

            using (var context = new PvmContext())
            {
                foreach (var tableName in allTableNames)
                {
                    context.Database.ExecuteSqlCommand(string.Format("DELETE FROM [{0}]", tableName));
                }
            }
        }
    }
}