using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualBasic.CompilerServices;

namespace MonCine.Data
{
    public interface ICRUD<T>
    {
        string CollectionName { get; set; }


        /// <summary>
        /// Récupère la liste des éléments contenus dans la BD
        /// </summary>
        /// <returns>Liste d'items</returns>
        List<T> ReadItems();

        /// <summary>
        /// Permet d'ajouter un élément dans la BD
        /// </summary>
        /// <param name="pObj"></param>
        /// <returns>Vrai si l'ajout est effectué, sinon False</returns>
        bool AddItem(T pObj);

        /// <summary>
        /// Peremt de mettre à jour un élément dans la BD
        /// </summary>
        /// <param name="pObj"></param>
        /// <returns>Vrai si la modification est effectué, sinon False</returns>
        bool UpdateItem(T pObj);


        /// <summary>
        /// Permet de supprimer un élément dans la BD
        /// </summary>
        /// <param name="pObj"></param>
        /// <returns>Vrai si la suppression est effectué, sinon False</returns>
        bool DeleteItem(T pObj);
    }
}