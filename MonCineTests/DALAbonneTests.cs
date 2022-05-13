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

            abonneCollection
                .Setup(x => x.FindSync(Builders<Abonne>.Filter.Empty, It.IsAny<FindOptions<Abonne>>(), default))
                .Returns(abonneCursor.Object);


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

        [Fact]
        public void UpdateItem_moqReplaceOne_ReturnTrueIfAbonneUpdated()
        {
            // Arrange
            InitializeMongoAbonneCollection();

            var dal = new DALAbonne(mongoClient.Object);

            // Act
            Abonne abonne = abonneList[0];
            abonne.Username = "username 02";
            bool res = dal.UpdateItem(abonne);

            // Assert
            Assert.True(res);
            Assert.Equal(abonne, abonneList.Find(x => x.Username == abonne.Username));
        }


        #region cat

        // ajout
        [Fact]
        public void AjouterCategorieFavorie_ThrowsArgumentExceptionIfCategorieIsNullOrEmpty()
        {
            // Arrange
            InitializeMongoAbonneCollection();
            var dal = new DALAbonne(mongoClient.Object);


            // Act
            Abonne abonne = abonneList[0];
            string cat = null;


            // Act and Assert
            ExceptionUtil.AssertThrows<ArgumentException>(delegate
            {
                dal.AjouterCategorieFavorie(abonne, cat);
            });
        }

        [Fact]
        public void AjouterCategorieFavorie_ThrowsArgumentNullExceptionIfAbonneIsNull()
        {
            // Arrange
            InitializeMongoAbonneCollection();
            var dal = new DALAbonne(mongoClient.Object);


            // Act
            Abonne abonne = null;
            string cat = Categorie.Action.ToString();


            // Act and Assert
            ExceptionUtil.AssertThrows<ArgumentNullException>(delegate { dal.AjouterCategorieFavorie(abonne, cat); });
        }

        [Fact]
        public void AjouterCategorieFavorie_ReturnsTrueIfCategoryAddedAndAbonneIsUpdated()
        {
            // Arrange
            InitializeMongoAbonneCollection();
            var dal = new DALAbonne(mongoClient.Object);

            // Act
            Abonne abonne = abonneList[0];
            string cat = Categorie.Action.ToString();
            bool res = dal.AjouterCategorieFavorie(abonne, cat);

            // Assert
            Assert.True(res);
        }

        [Fact]
        public void AjouterCategorieFavorie_CategoriePref_ConstainsNewCategory()
        {
            // Arrange
            InitializeMongoAbonneCollection();
            var dal = new DALAbonne(mongoClient.Object);

            // Act
            Abonne abonne = abonneList[0];
            string cat = Categorie.Action.ToString();
            dal.AjouterCategorieFavorie(abonne, cat);

            // Assert
            Assert.True(abonne.CategoriesPref != null && abonne.CategoriesPref.Contains(cat));
        }

        // supp
        [Fact]
        public void SupprimerCategorieFavorie_ThrowsArgumentExceptionIfCategorieIsNullOrEmpty()
        {
            // Arrange
            InitializeMongoAbonneCollection();
            var dal = new DALAbonne(mongoClient.Object);


            // Act
            Abonne abonne = abonneList[0];
            string cat = null;


            // Act and Assert
            ExceptionUtil.AssertThrows<ArgumentException>(delegate { dal.SupprimerCategorieFavorie(abonne, cat); });
        }

        [Fact]
        public void SupprimerCategorieFavorie_ThrowsArgumentNullExceptionIfAbonneIsNull()
        {
            // Arrange
            InitializeMongoAbonneCollection();
            var dal = new DALAbonne(mongoClient.Object);


            // Act
            Abonne abonne = null;
            string cat = Categorie.Action.ToString();


            // Act and Assert
            ExceptionUtil.AssertThrows<ArgumentNullException>(delegate { dal.SupprimerCategorieFavorie(abonne, cat); });
        }

        [Fact]
        public void SupprimerCategorieFavorie_ReturnsTrueIfCategoryDeletedAndAbonneIsUpdated()
        {
            // Arrange
            InitializeMongoAbonneCollection();
            var dal = new DALAbonne(mongoClient.Object);

            // Act
            Abonne abonne = abonneList[0];
            string cat = Categorie.Action.ToString();
            dal.AjouterCategorieFavorie(abonne, cat);
            bool res = dal.SupprimerCategorieFavorie(abonne, cat);

            // Assert
            Assert.True(res);
        }

        [Fact]
        public void SupprimerCategorieFavorie_CategoriePref_DoesNotConstainCategoryAnymore()
        {
            // Arrange
            InitializeMongoAbonneCollection();
            var dal = new DALAbonne(mongoClient.Object);

            // Act
            Abonne abonne = abonneList[0];
            string cat = Categorie.Action.ToString();
            dal.AjouterCategorieFavorie(abonne, cat);
            dal.SupprimerCategorieFavorie(abonne, cat);

            // Assert
            Assert.True(abonne.CategoriesPref != null && !abonne.CategoriesPref.Contains(cat));
        }

        #endregion


        #region acteur

        // ajout
        [Fact]
        public void AjouterActeurFavori_ThrowsArgumentNullExceptionIfActeurIsNull()
        {
            // Arrange
            InitializeMongoAbonneCollection();
            var dal = new DALAbonne(mongoClient.Object);


            // Act
            Abonne abonne = abonneList[0];
            Acteur acteur = null;


            // Act and Assert
            ExceptionUtil.AssertThrows<ArgumentNullException>(delegate { dal.AjouterActeurFavori(abonne, acteur); });
        }

        [Fact]
        public void AjouterActeurFavori_ThrowsArgumentNullExceptionIfAbonneIsNull()
        {
            // Arrange
            InitializeMongoAbonneCollection();
            var dal = new DALAbonne(mongoClient.Object);


            // Act
            Abonne abonne = null;
            Acteur acteur = new Acteur("Tom", "Bernard");


            // Act and Assert
            ExceptionUtil.AssertThrows<ArgumentNullException>(delegate { dal.AjouterActeurFavori(abonne, acteur); });
        }

        [Fact]
        public void AjouterActeurFavori_ReturnsTrueIfActeurAddedAndAbonneIsUpdated()
        {
            // Arrange
            InitializeMongoAbonneCollection();
            var dal = new DALAbonne(mongoClient.Object);

            // Act
            Abonne abonne = abonneList[0];
            Acteur acteur = new Acteur("Tom", "Bernard");

            bool res = dal.AjouterActeurFavori(abonne, acteur);

            // Assert
            Assert.True(res);
        }
        [Fact]
        public void AjouterActeurFavori_ReturnsFalseIfActeurAlreadyExits()
        {
            // Arrange
            InitializeMongoAbonneCollection();
            var dal = new DALAbonne(mongoClient.Object);

            // Act
            Abonne abonne = abonneList[0];
           Acteur acteur =  new Acteur("Tom", "Bernard");

            dal.AjouterActeurFavori(abonne, acteur);
            bool res = dal.AjouterActeurFavori(abonne, acteur);

            // Assert
            Assert.False(res);
        }

        [Fact]
        public void AjouterActeurFavori_ActeursPref_ConstainsNewActeur()
        {
            // Arrange
            InitializeMongoAbonneCollection();
            var dal = new DALAbonne(mongoClient.Object);

            // Act
            Abonne abonne = abonneList[0];
            Acteur acteur = new Acteur("Tom", "Bernard");

            dal.AjouterActeurFavori(abonne, acteur);

            // Assert
            Assert.True(abonne.ActeursPref != null && abonne.ActeursPref.Contains(acteur));
        }

        // supp
        [Fact]
        public void SupprimerActeurFavori_ThrowsArgumentNullExceptionIfActeurIsNull()
        {
            // Arrange
            InitializeMongoAbonneCollection();
            var dal = new DALAbonne(mongoClient.Object);

            // Act
            Abonne abonne = abonneList[0];
            Acteur acteur = null;

            // Act and Assert
            ExceptionUtil.AssertThrows<ArgumentException>(delegate { dal.SupprimerActeurFavori(abonne, acteur); });
        }

        [Fact]
        public void SupprimerActeurFavori_ThrowsArgumentNullExceptionIfAbonneIsNull()
        {
            // Arrange
            InitializeMongoAbonneCollection();
            var dal = new DALAbonne(mongoClient.Object);

            // Act
            Abonne abonne = null;
            Acteur acteur = new Acteur("Tom", "Bernard");

            // Act and Assert
            ExceptionUtil.AssertThrows<ArgumentNullException>(delegate { dal.SupprimerActeurFavori(abonne, acteur); });
        }

        [Fact]
        public void SupprimerActeurFavori_ReturnsTrueIfActeurDeletedAndAbonneIsUpdated()
        {
            // Arrange
            InitializeMongoAbonneCollection();
            var dal = new DALAbonne(mongoClient.Object);

            // Act
            Abonne abonne = abonneList[0];
            Acteur acteur = new Acteur("Tom", "Bernard");

            dal.AjouterActeurFavori(abonne, acteur);
            bool res = dal.SupprimerActeurFavori(abonne, acteur);

            // Assert
            Assert.True(res);
        }

        [Fact]
        public void SupprimerActeurFavori_ReturnsFalseIfActeurDoesNotExist()
        {
            // Arrange
            InitializeMongoAbonneCollection();
            var dal = new DALAbonne(mongoClient.Object);

            // Act
            Abonne abonne = abonneList[0];
            Acteur acteur = new Acteur("Tom", "Bernard");

            bool res = dal.SupprimerActeurFavori(abonne, acteur);

            // Assert
            Assert.False(res);
        }

        [Fact]
        public void SupprimerActeurFavori_ActeursPref_DoesNotConstainActeurAnymore()
        {
            // Arrange
            InitializeMongoAbonneCollection();
            var dal = new DALAbonne(mongoClient.Object);

            // Act
            Abonne abonne = abonneList[0];
            Acteur acteur = new Acteur("Tom", "Bernard");

            dal.AjouterActeurFavori(abonne, acteur);
            dal.SupprimerActeurFavori(abonne, acteur);

            // Assert
            Assert.True(abonne.ActeursPref != null && !abonne.ActeursPref.Contains(acteur));
        }

        #endregion




        #region realisateur

        // ajout
        [Fact]
        public void AjouterRealisateurFavori_ThrowsArgumentNullExceptionIfRealisateurIsNull()
        {
            // Arrange
            InitializeMongoAbonneCollection();
            var dal = new DALAbonne(mongoClient.Object);


            // Act
            Abonne abonne = abonneList[0];
            Realisateur realisateur = null;


            // Act and Assert
            ExceptionUtil.AssertThrows<ArgumentNullException>(delegate
            {
                dal.AjouterRealisateurFavori(abonne, realisateur);
            });

        }

        [Fact]
        public void AjouterRealisateurFavori_ThrowsArgumentNullExceptionIfAbonneIsNull()
        {
            // Arrange
            InitializeMongoAbonneCollection();
            var dal = new DALAbonne(mongoClient.Object);


            // Act
            Abonne abonne = null;
            Realisateur realisateur= new Realisateur("Tom", "Bernard");


            // Act and Assert
            ExceptionUtil.AssertThrows<ArgumentNullException>(delegate
            {
                dal.AjouterRealisateurFavori(abonne, realisateur);
            });
        }

        [Fact]
        public void AjouterRealisateurFavori_ReturnsTrueIfRealisateurAddedAndAbonneIsUpdated()
        {
            // Arrange
            InitializeMongoAbonneCollection();
            var dal = new DALAbonne(mongoClient.Object);

            // Act
            Abonne abonne = abonneList[0];
            Realisateur realisateur = new Realisateur("Tom", "Bernard");

            bool res = dal.AjouterRealisateurFavori(abonne, realisateur);

            // Assert
            Assert.True(res);
        }

        [Fact]
        public void AjouterRealisateurFavori_ReturnsFalseIfRealisateurAlreadyExits()
        {
            // Arrange
            InitializeMongoAbonneCollection();
            var dal = new DALAbonne(mongoClient.Object);

            // Act
            Abonne abonne = abonneList[0];
            Realisateur realisateur = new Realisateur("Tom", "Bernard");

            dal.AjouterRealisateurFavori(abonne, realisateur);
            bool res = dal.AjouterRealisateurFavori(abonne, realisateur);

            // Assert
            Assert.False(res);
        }

        [Fact]
        public void AjouterRealisateurFavori_RealisateursPref_ConstainsNewRealisateur()
        {
            // Arrange
            InitializeMongoAbonneCollection();
            var dal = new DALAbonne(mongoClient.Object);

            // Act
            Abonne abonne = abonneList[0];
            Realisateur realisateur = new Realisateur("Tom", "Bernard");

            dal.AjouterRealisateurFavori(abonne, realisateur);

            // Assert
            Assert.True(abonne.RealisationsPref!= null && abonne.RealisationsPref.Contains(realisateur));
        }

        // supp
        [Fact]
        public void SupprimerRealisateurFavori_ThrowsArgumentNullExceptionIfRealisateurIsNull()
        {
            // Arrange
            InitializeMongoAbonneCollection();
            var dal = new DALAbonne(mongoClient.Object);

            // Act
            Abonne abonne = abonneList[0];
            Realisateur realisateur = null;

            // Act and Assert
            ExceptionUtil.AssertThrows<ArgumentException>(delegate
            {
                dal.SupprimerRealisateurFavori(abonne, realisateur);
            });

        }

        [Fact]
        public void SupprimerRealisateurFavori_ThrowsArgumentNullExceptionIfAbonneIsNull()
        {
            // Arrange
            InitializeMongoAbonneCollection();
            var dal = new DALAbonne(mongoClient.Object);

            // Act
            Abonne abonne = null;
            Realisateur realisateur =  new Realisateur("Tom", "Bernard");

            // Act and Assert
            ExceptionUtil.AssertThrows<ArgumentNullException>(delegate
            {
                dal.SupprimerRealisateurFavori(abonne, realisateur);
            });
        }

        [Fact]
        public void SupprimerRealisateurFavori_ReturnsTrueIfRealisateurDeletedAndAbonneIsUpdated()
        {
            // Arrange
            InitializeMongoAbonneCollection();
            var dal = new DALAbonne(mongoClient.Object);

            // Act
            Abonne abonne = abonneList[0];
            Realisateur realisateur = new Realisateur("Tom", "Bernard");

            dal.AjouterRealisateurFavori(abonne, realisateur);
            bool res = dal.SupprimerRealisateurFavori(abonne, realisateur);

            // Assert
            Assert.True(res);
        }

        [Fact]
        public void SupprimerRealisateurFavori_ReturnsFalseIfRealisateurDoesNotExist()
        {
            // Arrange
            InitializeMongoAbonneCollection();
            var dal = new DALAbonne(mongoClient.Object);

            // Act
            Abonne abonne = abonneList[0];
            Realisateur realisateur = new Realisateur("Tom", "Bernard");

            bool res = dal.SupprimerRealisateurFavori(abonne, realisateur);

            // Assert
            Assert.False(res);
        }

        [Fact]
        public void SupprimerRealisateurFavori_RealisateursPref_DoesNotConstainRealisateurAnymore()
        {
            // Arrange
            InitializeMongoAbonneCollection();
            var dal = new DALAbonne(mongoClient.Object);

            // Act
            Abonne abonne = abonneList[0];
            Realisateur realisateur =  new Realisateur("Tom", "Bernard");

            dal.AjouterRealisateurFavori(abonne, realisateur);
            dal.SupprimerRealisateurFavori(abonne, realisateur);

            // Assert
            Assert.True(abonne.RealisationsPref != null && !abonne.RealisationsPref.Contains(realisateur));
        }

        #endregion

    }
}