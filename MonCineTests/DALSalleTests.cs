using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using MonCine.Data;
using MongoDB.Driver;
using Moq;
using Xunit;

namespace MonCineTests
{
    public class DALSalleTests
    {
        private Mock<IMongoClient> mongoClient;
        private Mock<IMongoDatabase> mongodb;

        private Mock<IMongoCollection<Salle>> salleCollection;
        private List<Salle> salleList;
        private Mock<IAsyncCursor<Salle>> salleCursor;

        public DALSalleTests()
        {
            mongoClient = new Mock<IMongoClient>();
            mongodb = new Mock<IMongoDatabase>();

            salleCollection = new Mock<IMongoCollection<Salle>>();
            salleCursor= new Mock<IAsyncCursor<Salle>>();


            salleList= new List<Salle>
            {
                new Salle(20),
                new Salle(25),
            };
        }

        private void InitializeMongoDb()
        {
            mongoClient.Setup(x => x.GetDatabase(It.IsAny<string>(), default)).Returns(mongodb.Object);
            mongodb.Setup(x => x.GetCollection<Salle>("Salle", default)).Returns(salleCollection.Object);
        }

        private void InitializeMongoSalleCollection()
        {
            salleCursor.Setup(x => x.Current).Returns(salleList);

            salleCursor.SetupSequence(x => x.MoveNext(It.IsAny<CancellationToken>())).Returns(true).Returns(false);

            salleCollection.Setup(x => x.FindSync(Builders<Salle>.Filter.Empty, It.IsAny<FindOptions<Salle>>(), default)).Returns(salleCursor.Object);


            InitializeMongoDb();
        }



        [Fact]
        public void ReadItems_moqFindSyncCall()
        {
            // Arrange
            InitializeMongoSalleCollection();

            var dal = new DALSalle(mongoClient.Object);

            // Act : appel de la méthode qui contient le faux objet
            var documents = dal.ReadItems();

            // Assert
            Assert.Equal(salleList, documents);

        }
    }
}
