using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MonCine.Data;
using MongoDB.Bson;
using MongoDB.Driver;
using Moq;
using Xunit;

namespace MonCineTests
{
    public class DALFilmTests
    {
        private Mock<IMongoClient> mongoClient;
        private Mock<IMongoDatabase> mongodb;

        private Mock<IMongoCollection<Film>> filmCollection;
        private List<Film> filmList;
        private Mock<IAsyncCursor<Film>> filmCursor;

        public DALFilmTests()
        {
            mongoClient = new Mock<IMongoClient>();
            mongodb = new Mock<IMongoDatabase>();

            filmCollection = new Mock<IMongoCollection<Film>>();
            filmCursor = new Mock<IAsyncCursor<Film>>();


            filmList = new List<Film>
            {
               new Film("Film 1"),
               new Film("Film 2")
            };
        }

        private void InitializeMongoDb()
        {
            mongoClient.Setup(x => x.GetDatabase(It.IsAny<string>(), default)).Returns(mongodb.Object);
            mongodb.Setup(x => x.GetCollection<Film>("Film", default)).Returns(filmCollection.Object);
        }

        private void InitializeMongoFilmCollection()
        {
            filmCursor.Setup(x => x.Current).Returns(filmList);

            filmCursor.SetupSequence(x => x.MoveNext(It.IsAny<CancellationToken>())).Returns(true).Returns(false);

            filmCollection.Setup(x => x.FindSync(Builders<Film>.Filter.Empty, It.IsAny<FindOptions<Film>>(), default)).Returns(filmCursor.Object);


            InitializeMongoDb();
        }

       


        [Fact]
        public void ReadItems_moqFindSyncCall()
        {
            // Arrange
            InitializeMongoFilmCollection();

            var dal = new DALFilm(mongoClient.Object);

            // Act : appel de la méthode qui contient le faux objet
            var documents = dal.ReadItems();

            // Assert
            Assert.Equal(filmList, documents);

        }

        [Fact]
        public void AddItem_moqInsertOne_ReturnTrueWhenFilmInserted()
        {
            // Arrange
            InitializeMongoFilmCollection();

            var dal = new DALFilm(mongoClient.Object);

            Film film = new Film("Film moq");

            // Act
            bool result = dal.AddItem(film);

            // Assert
            Assert.True(result);
        }


        [Fact]
        public void AddItem_moqInsertOne_ThrowsArgumentNullExceptionIfFilmIsNull()
        {
            // Arrange
            InitializeMongoFilmCollection();

            var dal = new DALFilm(mongoClient.Object);

            Film film = null;

            // Act and Assert
            ExceptionUtil.AssertThrows<ArgumentNullException>(delegate
            {
                dal.AddItem(film);
            });
        }

        [Fact]
        public void DeleteItem_moqDeleteOne_ReturnTrueIfFilmDeleted()
        {
            // Arrange
            InitializeMongoFilmCollection();

            var dal = new DALFilm(mongoClient.Object);

            Film film = filmList[0];
            
            // Act 
            bool result =  dal.DeleteItem(film);

            
            // Assert
            Assert.True(result);
           
        }


        [Fact]
        public void DeleteItem_moqDeleteOne_ThrowsArgumentNullExceptionIfFilmIsNull()
        {
            // Arrange
            InitializeMongoFilmCollection();

            var dal = new DALFilm(mongoClient.Object);

            Film film = null;

            // Act and Assert
            ExceptionUtil.AssertThrows<ArgumentNullException>(delegate
            {
                dal.DeleteItem(film);
            });
        }

        [Fact]
        public void DeleteItem_moqDeleteOne_UpdateListCountCorrectly()
        {
            // Arrange
            InitializeMongoFilmCollection();

            var dal = new DALFilm(mongoClient.Object);

            Film film = filmList[0];

            // Act
            dal.DeleteItem(film);

            //Assert
            Assert.Equal(filmList.Count, dal.ReadItems().Count);

        }




        [Fact]
        public void UpdateItem_moqReplaceOne_ThrowsArgumentNullExceptionIfFilmIsNull()
        {
            // Arrange
            InitializeMongoFilmCollection();

            var dal = new DALFilm(mongoClient.Object);

            Film film = null;

            // Act and Assert
            ExceptionUtil.AssertThrows<ArgumentNullException>(delegate
            {
                dal.UpdateItem(film);
            });
        }


        [Fact]
        public void UpdateItem_moqReplaceOne_ReturnTrueIfFilmUpdated()
        {
            // Arrange
            InitializeMongoFilmCollection();

            var dal = new DALFilm(mongoClient.Object);

            // Act
            Film film = filmList[0];
            film.Name = "Film updated moq";
            dal.UpdateItem(film);

            // Assert
            Assert.Equal(film, filmList.Find(x=>x.Name == film.Name) );

        }


        [Fact]
        public void AddProjectionDate_AjouterDateProjection_ReturnTrueIfFilmUpdated()
        {
            // Arrange
            InitializeMongoFilmCollection();

            var dal = new DALFilm(mongoClient.Object);
            Film film = filmList[0];
            Projection projection = new Projection(new Salle(20), film, DateTime.Now);

            // Act
            var result = dal.AddProjectionDate(projection);



            // Assert
            Assert.True(result);

        }

        [Fact]
        public void AddProjectionDate_AjouterDateProjection_ThrowArgumentNullExceptionifProjectionIsNull()
        {
            // Arrange
            InitializeMongoFilmCollection();

            var dal = new DALFilm(mongoClient.Object);
            Film film = filmList[0];
            Projection projection = null;



            // Act and Assert
            ExceptionUtil.AssertThrows<ArgumentNullException>(delegate
            {
                dal.AddProjectionDate(projection);
            });

        }
        
    }


}