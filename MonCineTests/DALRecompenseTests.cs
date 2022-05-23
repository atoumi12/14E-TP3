using MonCine.Data;
using MongoDB.Driver;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using MonCine.Data.Entitites;
using Xunit;

namespace MonCineTests
{
    public class DALRecompenseTests
    {
        private Mock<IMongoClient> mongoClient;
        private Mock<IMongoDatabase> mongodb;

        private Mock<IMongoCollection<Recompense>> recompenseCollection;
        private List<Recompense> recompenseList;
        private Mock<IAsyncCursor<Recompense>> recompenseCursor;

        public DALRecompenseTests()
        {
            mongoClient = new Mock<IMongoClient>();
            mongodb = new Mock<IMongoDatabase>();

            recompenseCollection = new Mock<IMongoCollection<Recompense>>();
            recompenseCursor = new Mock<IAsyncCursor<Recompense>>();

            recompenseList = new List<Recompense>()
            {
                new Recompense(TypeRecompense.AvantPremiere, new Film("Fast and furious"),
                    new Abonne("atoumi")),
                new Recompense(TypeRecompense.Reprojection, new Film("No way home"), new Abonne("gwen"))
            };

        }

        private void InitializeMongoDb()
        {
            mongoClient.Setup(x => x.GetDatabase(It.IsAny<string>(), default)).Returns(mongodb.Object);
            mongodb.Setup(x => x.GetCollection<Recompense>("Recompense", default)).Returns(recompenseCollection.Object);
        }

        private void InitializeMongoAbonneCollection()
        {
            recompenseCursor.Setup(x => x.Current).Returns(recompenseList);

            recompenseCursor.SetupSequence(x => x.MoveNext(It.IsAny<CancellationToken>())).Returns(true).Returns(false);

            recompenseCollection
                .Setup(x => x.FindSync(Builders<Recompense>.Filter.Empty, It.IsAny<FindOptions<Recompense>>(), default))
                .Returns(recompenseCursor.Object);


            InitializeMongoDb();
        }


        [Fact]
        public void ReadItems_moqFindSyncCall()
        {
            // Arrange
            InitializeMongoAbonneCollection();

            var dal = new DALRecompense(mongoClient.Object);

            // Act : appel de la méthode qui contient le faux objet
            var documents = dal.ReadItems();

            // Assert
            Assert.Equal(recompenseList, documents);
        }


        [Fact]
        public void UpdateItem_moqReplaceOne_ThrowsArgumentNullExceptionIfAbonneIsNull()
        {
            // Arrange
            InitializeMongoAbonneCollection();

            var dal = new DALAbonne(mongoClient.Object);

            Abonne abonne = null;

            // Act and Assert
            ExceptionUtil.AssertThrows<ArgumentNullException>(delegate { dal.UpdateItem(abonne); });
        }





    }
}