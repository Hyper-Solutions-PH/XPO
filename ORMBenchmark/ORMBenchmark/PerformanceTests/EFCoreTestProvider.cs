﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ORMBenchmark.Models.EFCore;

namespace ORMBenchmark.PerformanceTests {

    public class EFCoreTestProvider : TestProviderBase {
        private EFCoreContext dataContext;

        public override string ToString() {
            return "EF Core";
        }

        public override void CreateTestDataSet(int recordsCount) {
            CleanupTestDataSet();
            EFCoreEntity[] data = new EFCoreEntity[recordsCount];
            for(int i = 0; i < recordsCount; i++) {
                data[i] = new EFCoreEntity() { Id = i, Value = i };
            }
            using(var dataContext = new EFCoreContext()) {
                dataContext.Entities.AddRange(data);
                dataContext.SaveChanges();
            }
            RecordsCount = recordsCount;
        }

        public override void CleanupTestDataSet() {
            using(var dataContext = new EFCoreContext()) {
                dataContext.Entities.RemoveRange(dataContext.Entities.ToList());
                dataContext.SaveChanges();
            }
        }

        public override void InitSession() {
            dataContext = new EFCoreContext();
        }

        public override void TearDownSession() {
            if(dataContext != null) {
                dataContext.Dispose();
                dataContext = null;
            }
        }

        public override void InsertOne(int recordsCount) {
            using(var transaction = dataContext.Database.BeginTransaction()) {
                for(int i = 0; i < recordsCount; i++) {
                    var item = new EFCoreEntity() { Id = i, Value = i };
                    dataContext.Entities.Add(item);
                    dataContext.SaveChanges();
                }
                transaction.Commit();
            }
        }

        public override void InsertMany(int recordsCount) {
            using(var transaction = dataContext.Database.BeginTransaction()) {
                for(int i = 0; i < recordsCount; i++) {
                    var item = new EFCoreEntity() { Id = i, Value = i };
                    dataContext.Entities.Add(item);
                }
                dataContext.SaveChanges();
                transaction.Commit();
            }
        }

        public override void UpdateOne() {
            using(var transaction = dataContext.Database.BeginTransaction()) {
                foreach(var s in dataContext.Entities) {
                    s.Value++;
                    dataContext.SaveChanges();
                }
                transaction.Commit();
            }
        }

        public override void UpdateMany() {
            using(var transaction = dataContext.Database.BeginTransaction()) {
                foreach(var s in dataContext.Entities) {
                    s.Value++;
                }
                dataContext.SaveChanges();
                transaction.Commit();
            }
        }

        public override void DeleteOne() {
            using(var transaction = dataContext.Database.BeginTransaction()) {
                foreach(var item in dataContext.Entities) {
                    dataContext.Entry(item).State = Microsoft.EntityFrameworkCore.EntityState.Deleted;
                    dataContext.SaveChanges();
                }
                transaction.Commit();
            }
        }

        public override void DeleteMany() {
            using(var transaction = dataContext.Database.BeginTransaction()) {
                foreach(var item in dataContext.Entities) {
                    dataContext.Entry(item).State = Microsoft.EntityFrameworkCore.EntityState.Deleted;
                }
                dataContext.SaveChanges();
                transaction.Commit();
            }
        }

        public override void Fetch() {
            for(int i = 0; i < RecordsCount; i++) {
                var item = dataContext.Entities.First(o => o.Id == i);
            }
        }

        public override void LinqQuery() {
            for(int i = 0; i < RecordsCount; i++) {
                var result = dataContext.Entities.Where(o => o.Id == i);
                foreach(var o in result) { }
            }
        }

        public override void ObjectInstantiationNative() {
            foreach(var o in dataContext.Entities) { }
        }

        public override void ObjectInstantiationLinq() {
            foreach(var o in dataContext.Entities.Where(s => s.Id != -1)) { }
        }

        protected override void LinqTakeRecords(int takeRecords) {
            for(int i = 0; i < RecordsCount; i += takeRecords) {
                var query = dataContext.Entities.Where(o => o.Id >= i).Take(takeRecords);
                foreach(var o in query) { }
            }
        }
    }
}