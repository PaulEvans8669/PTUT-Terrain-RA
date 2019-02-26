using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.lib
{
    class Terrain
    {
        private int CHUNK_SIZE = 32;

        private GameObject topArrow, leftArrow, bottomArrow, rightArrow; //flèches directionelles au bord du terrain

        private GameObject gameObject;
        public GameObject GameObject { get; set; }

        private int id;
        public int Id { get; set; }

        private string nom;
        public string Nom { get; set; }

        private int size;
        public int Size { get; set; }

        private List<Chunk> chunkList;
        public List<Chunk> ChunkList { get; set; }

        public Terrain(GameObject associatedGameObject, string nom, int size)
        {
            GameObject = associatedGameObject;
            Nom = nom;
            Size = size;
            ChunkList = new List<Chunk>();
            generateNewTerrain();
        }

        public Terrain(GameObject associatedGameObject, int id)
        {
            GameObject = associatedGameObject;
            Id = id;
            ChunkList = chunkList;
        }

        private void generateNewTerrain()
        {
            for(int i = 0; i<Size*Size; i++)
            {
                Chunk c = new Chunk(this, i, CHUNK_SIZE);
                ChunkList.Add(c);
            }

        }

        private void placeArrows()
        {
            leftArrow = GameObject.transform.GetChild(1).gameObject;
            leftArrow.transform.position = new Vector3(-10f, 0f, -48.1f);

            rightArrow = GameObject.transform.GetChild(2).gameObject;
            rightArrow.transform.position = new Vector3(136f, 0f, -16.1f);

            topArrow = GameObject.transform.GetChild(3).gameObject;
            topArrow.transform.position = new Vector3(47.9f, 0f, 40f);

            bottomArrow = GameObject.transform.GetChild(4).gameObject;
            bottomArrow.transform.position = new Vector3(80.1f, 0f, -105.5f);
        }




    }
}
