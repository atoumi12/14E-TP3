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
                new Abonne("Abonne 1","Leonardo Di caprio","Denis Villeneuve", 12, uneDate,"Gwenael","Galliot"),
                new Abonne("Abonne 2","Johnny depp","Denis Villeneuve", 3, uneDate, "Loan", "Rage"),
                new Abonne("Abonne 3","robert downey jr","Denis Villeneuve", 22, uneDate,"Amhed","Toumi")
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
