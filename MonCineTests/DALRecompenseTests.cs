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

        private void InitializeMongoRecompenseCollection()
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
            InitializeMongoRecompenseCollection();

            var dal = new DALRecompense(mongoClient.Object);

            // Act : appel de la méthode qui contient le faux objet
            var documents = dal.ReadItems();

            // Assert
            Assert.Equal(recompenseList, documents);
        }

        [Fact]
        public void AddItem_moqInsertOne_ReturnTrueWhenRecompenseInserted()
        {
            // Arrange
            InitializeMongoRecompenseCollection();

            var dal = new DALRecompense(mongoClient.Object);

            Recompense recompense = new Recompense(TypeRecompense.AvantPremiere, new Film("Fast"), new Abonne("pfleury"));

            // Act
            bool result = dal.AddItem(recompense);

            // Assert
            Assert.True(result);
        }


        [Fact]
        public void AddItem_moqInsertOne_ThrowsArgumentNullExceptionIfRecompenseIsNull()
        {
            // Arrange
            InitializeMongoRecompenseCollection();

            var dal = new DALRecompense(mongoClient.Object);

            Recompense recompense= null;

            // Act and Assert
            ExceptionUtil.AssertThrows<ArgumentNullException>(delegate
            {
                dal.AddItem(recompense);
            });
        }


        [Fact]
        public void UpdateItem_moqReplaceOne_ThrowsArgumentNullExceptionIfRecompenseIsNull()
        {
            // Arrange
            InitializeMongoRecompenseCollection();

            var dal = new DALRecompense(mongoClient.Object);

            Recompense recompense = null;

            // Act and Assert
            ExceptionUtil.AssertThrows<ArgumentNullException>(delegate { dal.UpdateItem(recompense); });
        }


        [Fact]
        public void UpdateItem_moqReplaceOne_ReturnTrueIfFilmUpdated()
        {
            // Arrange
            InitializeMongoRecompenseCollection();

            var dal = new DALRecompense(mongoClient.Object);

            // Act
            Recompense recompense = recompenseList[0];
            recompense.Type = TypeRecompense.Reprojection;
            dal.UpdateItem(recompense);

            // Assert
            Assert.Equal(recompense, recompenseList.Find(x => x.Id== recompense.Id));

        }


        [Fact]
        public void AbonneAdmissibleRecompense_ReturnTrueIfAbonneDoesntHaveItAlready()
        {
            InitializeMongoRecompenseCollection();

            var dal = new DALRecompense(mongoClient.Object);

            Abonne abonne = recompenseList[0].Abonne;
            Film film = new Film("new film");

            var res = dal.AbonneAdmissibleRecompense(TypeRecompense.Reprojection, abonne, film);

            Assert.True(res);

        }




    }
}