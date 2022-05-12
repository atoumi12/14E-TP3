using MonCine.Data;
using MongoDB.Driver;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Xunit;

namespace MonCineTests
{
    //TODO:Add updateItem tests
    public class DALAbonneTests
    {
        private Mock<IMongoClient> mongoClient;
        private Mock<IMongoDatabase> mongodb;

        private Mock<IMongoCollection<Abonne>> abonneCollection;
        private List<Abonne> abonneList;
        private Mock<IAsyncCursor<Abonne>> abonneCursor;

        public DALAbonneTests()
        {
            mongoClient = new Mock<IMongoClient>();
            mongodb = new Mock<IMongoDatabase>();

            abonneCollection = new Mock<IMongoCollection<Abonne>>();
            abonneCursor = new Mock<IAsyncCursor<Abonne>>();

            DateTime uneDate = new DateTime();
            uneDate = DateTime.Today;

            abonneList = new List<Abonne>
            {
                new Abonne("Gwenael", "Galliot", "Abonne 1", 12, uneDate),
                new Abonne("Loan", "Rage", "Abonne 2", 3, uneDate),
                new Abonne("Ahmed", "Toumi", "Abonne 3", 22, uneDate)
            };
        }

        private void InitializeMongoDb()
        {
            mongoClient.Setup(x => x.GetDatabase(It.IsAny<string>(), default)).Returns(mongodb.Object);
            mongodb.Setup(x => x.GetCollection<Abonne>("Abonne", default)).Returns(abonneCollection.Object);
        }

        private void InitializeMongoAbonneCollection()
        {
            abonneCursor.Setup(x => x.Current).Returns(abonneList);

            abonneCursor.SetupSequence(x => x.MoveNext(It.IsAny<CancellationToken>())).Returns(true).Returns(false);

            abonneCollection.Setup(x => x.FindSync(Builders<Abonne>.Filter.Empty, It.IsAny<FindOptions<Abonne>>(), default)).Returns(abonneCursor.Object);


            InitializeMongoDb();
        }



        [Fact]
        public void ReadItems_moqFindSyncCall()
        {
            // Arrange
            InitializeMongoAbonneCollection();

            var dal = new DALAbonne(mongoClient.Object);

            // Act : appel de la méthode qui contient le faux objet
            var documents = dal.ReadItems();

            // Assert
            Assert.Equal(abonneList, documents);
        }

    }
}
