using System;
using System.IO;
using System.Collections.Generic;
using Xunit;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Axle.Engine.Database;
using Axle.Engine.Database.Models.Index;

namespace Axle.FunctionalTests.Database
{
    public class DatabaseTests
    {
        /// <summary>
        /// Creates instance of MongoCRUD for all CRUD operations
        /// </summary>
        /// <returns>MongoCRUD object</returns>
        private MongoCRUD CreateLocalTestDatabase()
        {
            MongoCRUD TestCRUD = new MongoCRUD("TestDB");
            // TestCRUD.InsertRecord("TestTable", new {firstname = "Mohammed", lastname = "Ali"});
            return TestCRUD;
        }

        [Fact]
        public void TestDocumentModel()
        {
            MongoCRUD TestCRUD = CreateLocalTestDatabase();

            var id = new Guid(Guid.NewGuid().ToString());
            decimal tf = 0.33M;
            string sourcePath = "C:/src/files";
            bool isIndexed = true;
            DateTime dateIndexed = DateTime.UtcNow;

            TestCRUD.InsertRecord("TestTable", new DocumentModel {Id = id, Tf = tf, SourcePath = sourcePath, IsIndexed = isIndexed, DateIndexed = dateIndexed});

            Assert.IsType<DocumentModel>(TestCRUD.ReadCollection<DocumentModel>("TestTable")[0]);


            // Assert.Equal(id, entry.Id);
            // Assert.Equal(tf, entry.Tf);
            // Assert.Equal(sourcePath, entry.SourcePath);
            // Assert.Equal(isIndexed, entry.IsIndexed);
            // Assert.Equal(dateIndexed, entry.DateIndexed);
            
        }

        [Fact]
        public void TestTokentModel()
        {
            MongoCRUD TestCRUD = CreateLocalTestDatabase();

            var id = new Guid(Guid.NewGuid().ToString());
            string token = "balooney";
            decimal idf = 0.56M;
            List<DocumentModel> ContainingDocuments = new List<DocumentModel>();

            TestCRUD.InsertRecord("TestTable2", new TokenModel {Id = id, Token = token, Idf = idf});

            Assert.IsType<TokenModel>(TestCRUD.ReadCollection<TokenModel>("TestTable2")[0]);
        }

        [Fact]
        public void ShouldInsertRecord()
        {
            MongoCRUD TestCRUD = CreateLocalTestDatabase();

            //expected values
            Guid id = Guid.NewGuid();
            decimal tf = 0.33M;
            string sourcePath = "C:/src/files";
            bool isIndexed = true;
            DateTime dateIndexed = DateTime.UtcNow;

            TestCRUD.InsertRecord("TestTable", new DocumentModel {Id = id, Tf = tf, SourcePath = sourcePath, IsIndexed = isIndexed, DateIndexed = dateIndexed});

            var entry = TestCRUD.ReadRecordById<DocumentModel>("TestTable", id);

            Assert.Equal(id, entry.Id);
            Assert.Equal(tf, entry.Tf);
            Assert.Equal(sourcePath, entry.SourcePath);
            Assert.Equal(isIndexed, entry.IsIndexed);
        }

        [Fact]
        public void ShouldUpsertRecord()
        {
            MongoCRUD TestCRUD = CreateLocalTestDatabase();
            Guid id = Guid.NewGuid();
            decimal tf = 0.83M;
            string sourcePath = "C:/src/files";
            bool isIndexed = true;
            DateTime dateIndexed = DateTime.UtcNow;

            TestCRUD.InsertRecord("TestTable", new DocumentModel {Id = id, Tf = tf, SourcePath = sourcePath, IsIndexed = isIndexed, DateIndexed = dateIndexed});

            decimal tf_new = 0.18M;
            string sourcePath_new = "C:/src/new";
            bool isIndexed_new = false;
            DateTime dateIndexed_new = DateTime.UtcNow;

            TestCRUD.UpsertRecord("TestTable", id, new DocumentModel {Id = id, Tf = tf_new, SourcePath = sourcePath_new, IsIndexed = isIndexed_new, DateIndexed = dateIndexed_new});

            var entry = TestCRUD.ReadRecordById<DocumentModel>("TestTable", id);

            Assert.Equal(tf_new, entry.Tf);
            Assert.Equal(sourcePath_new, entry.SourcePath);
            Assert.Equal(isIndexed_new, entry.IsIndexed);
        }

        [Fact]
        public void ShouldReadRecord()
        {
            MongoCRUD TestCRUD = CreateLocalTestDatabase();

            //expected values
            Guid id = Guid.NewGuid();
            decimal tf = 0.45M;
            string sourcePath = "C:/src/files";
            bool isIndexed = true;
            DateTime dateIndexed = DateTime.UtcNow;

            TestCRUD.InsertRecord("TestTable", new DocumentModel {Id = id, Tf = tf, SourcePath = sourcePath, IsIndexed = isIndexed, DateIndexed = dateIndexed});

            var entry = TestCRUD.ReadRecordById<DocumentModel>("TestTable", id);

            Assert.NotNull(entry);
        }

        [Fact]
        public void ShouldDeleteRecord()
        {
            MongoCRUD TestCRUD = CreateLocalTestDatabase();

            //expected values
            Guid id = Guid.NewGuid();
            decimal tf = 0.68M;
            string sourcePath = "C:/src/files/sdfghj";
            bool isIndexed = true;
            DateTime dateIndexed = DateTime.UtcNow;

            TestCRUD.InsertRecord("TestTable", new DocumentModel {Id = id, Tf = tf, SourcePath = sourcePath, IsIndexed = isIndexed, DateIndexed = dateIndexed});

            TestCRUD.DeleteRecord<DocumentModel>("TestTable", id);

            var ex = Assert.Throws<InvalidOperationException>(() => TestCRUD.ReadRecordById<DocumentModel>("TestTable", id));

        }

        // [Fact]
        // public void ShouldBatchInsertRecords()
        // {
        //     MongoCRUD TestCRUD = CreateLocalTestDatabase();

        //     //expected values
        //     Guid id1, id2, id3;
        //     id1 = Guid.NewGuid(); id2 = Guid.NewGuid(); id3 = Guid.NewGuid();
        //     decimal tf1 = 0.33M, tf2 = 0.25M, tf3 = 0.12M;
        //     string sourcePath1 = "C:/src/files/file1", sourcePath2 = "C:/src/files/file2", sourcePath3 = "C:/src/files/file3";
        //     bool isIndexed1 = true, isIndexed2 = true, isIndexed3 = true;
        //     DateTime dateIndexed1 = DateTime.UtcNow; DateTime dateIndexed2 = DateTime.UtcNow; DateTime dateIndexed3 = DateTime.UtcNow;

        //     List<DocumentModel> list = new List<DocumentModel>
        //     {
        //         new DocumentModel {Id = id1, Tf = tf1, SourcePath = sourcePath1, IsIndexed = isIndexed1, DateIndexed = dateIndexed1},
        //         new DocumentModel {Id = id2, Tf = tf2, SourcePath = sourcePath2, IsIndexed = isIndexed2, DateIndexed = dateIndexed2},
        //         new DocumentModel {Id = id3, Tf = tf3, SourcePath = sourcePath3, IsIndexed = isIndexed3, DateIndexed = dateIndexed3}
        //     };

        //     IEnumerable<DocumentModel> batch = list;
            

        //     // Insert the batch
        //     TestCRUD.BatchInsertRecords("TestTable", batch);

        //     // read each entry
        //     var entry1 = TestCRUD.ReadRecordById<DocumentModel>("TestTable", id1);
        //     var entry2 = TestCRUD.ReadRecordById<DocumentModel>("TestTable", id2);
        //     var entry3 = TestCRUD.ReadRecordById<DocumentModel>("TestTable", id3);

        //     Assert.Equal(id1, entry1.Id); Assert.Equal(id2, entry2.Id); Assert.Equal(id3, entry3.Id);
        //     Assert.Equal(tf1, entry1.Tf); Assert.Equal(tf2, entry2.Tf); Assert.Equal(tf3, entry3.Tf);
        //     Assert.Equal(sourcePath1, entry1.SourcePath); Assert.Equal(sourcePath2, entry2.SourcePath); Assert.Equal(sourcePath3, entry3.SourcePath);
        //     Assert.Equal(isIndexed1, entry1.IsIndexed); Assert.Equal(isIndexed2, entry2.IsIndexed); Assert.Equal(isIndexed3, entry3.IsIndexed);
        //     Assert.Equal(dateIndexed1, entry1.DateIndexed); Assert.Equal(dateIndexed2, entry2.DateIndexed); Assert.Equal(dateIndexed3, entry3.DateIndexed);
        // }

        [Fact]
        public void ShouldReadCollection()
        {
            MongoCRUD TestCRUD = CreateLocalTestDatabase();
            TestCRUD.DeleteCollection("NewCollection");
            TestCRUD.CreateCollection("NewCollection");
            TestCRUD.InsertRecord("NewCollection", new {firstname = "Mohammed", lastname = "Ali"});

            Assert.NotEmpty(TestCRUD.ReadCollection<object>("NewCollection"));
        }        

        [Fact]
        public void ShouldDeleteCollection()
        {
            MongoCRUD TestCRUD = CreateLocalTestDatabase();
            TestCRUD.DeleteCollection("NewCollection2");
            TestCRUD.CreateCollection("NewCollection2");
            TestCRUD.InsertRecord("NewCollection2", new {firstname = "Mohammed", lastname = "Ali"});
            TestCRUD.DeleteCollection("NewCollection2");

            Assert.Empty(TestCRUD.ReadCollection<object>("NewCollection2"));
        }

    }
    
}