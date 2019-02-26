using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.lib
{
    class Chunk
    {
        private static GameObject modelChunk;
        private int textureSize = 256;

        private Terrain mainTerrain;
        public Terrain MainTerrain { get; set; }

        private GameObject gameObject;
        public GameObject GameObject { get; set; }

        private int id;
        public int Id { get; set; }

        private int meshSize;
        public int MeshSize { get; set; }
        
        private Texture2D texture;
        public Texture2D Texture { get; set; }

        public List<GameObject> vegetationList;
        public List<GameObject> VegetationList { get; set; }



        public Chunk(Terrain terrain, int id, int size)
        {
            if(modelChunk == null)
            {
                modelChunk = GameObject.Find("ModelChunk");
            }
            this.MainTerrain = terrain;
            Id = id;
            meshSize = size;
            GenerateChunk();
        }




        private void GenerateChunk()
        {
            int x = Id % MainTerrain.Size;
            int z = Id / MainTerrain.Size;
            //Possibiliter de modifier le instantiate et de remplacer par des addComponent
            //AssociatedGameObject = GameObject.Instantiate(modelChunk, new Vector3(Id % terrain.Size * Size, 0, -(Id / terrain.Size) * Size), Quaternion.identity, terrain.AssociatedGameObject.transform);
            GameObject = new GameObject("Chunk " + Id);
            GameObject.name = "Chunk " + (z * MainTerrain.Size + x);
            GameObject.transform.parent = MainTerrain.GameObject.transform;
            GameObject.transform.localPosition = new Vector3(Id % MainTerrain.Size * meshSize, 0, -(Id / MainTerrain.Size) * meshSize);
            GameObject.AddComponent<MeshFilter>();
            GameObject.AddComponent<MeshRenderer>();
            GenerateMesh();
            GenerateTexture();
        }

        private void GenerateMesh()
        {

            /*
             *  Vertices
             *  UVs
             *  Tangents
             */
            Mesh mesh;
            GameObject.GetComponent<MeshFilter>().mesh = mesh = new Mesh();
            mesh.name = "Chunk";

            List<Vector3> vertices = new List<Vector3>();
            List<Vector2> uv = new List<Vector2>();
            List<Vector4> tangents = new List<Vector4>();

            Vector4 tangent = new Vector4(1f, 0f, 0f, -1f);

            for (int i = 0, z = 0; z <= meshSize; z++)
            {
                for (int x = 0; x <= meshSize; x++, i++)
                {
                    vertices.Add(new Vector3(x, 0, z));
                    uv.Add(new Vector2((float)x / meshSize, (float)z / meshSize));
                    tangents.Add(tangent);
                }
            }
            mesh.MarkDynamic();
            mesh.SetVertices(vertices);
            mesh.SetUVs(0, uv);
            mesh.SetTangents(tangents);

            /*
             * Triangles
             */

            int[] triangles = new int[meshSize * meshSize * 6];
            for (int ti = 0, vi = 0, z = 0; z < meshSize; z++, vi++)
            {
                for (int x = 0; x < meshSize; x++, ti += 6, vi++)
                {
                    triangles[ti] = vi;
                    triangles[ti + 3] = triangles[ti + 2] = vi + 1;
                    triangles[ti + 4] = triangles[ti + 1] = vi + meshSize + 1;
                    triangles[ti + 5] = vi + meshSize + 2;
                }
            }

            mesh.SetTriangles(triangles, 0);
            mesh.RecalculateNormals();

            /*
             * Collider
             */

            MeshCollider meshCollider = GameObject.AddComponent<MeshCollider>();
            meshCollider.sharedMesh = mesh;


        }

        public void GenerateTexture()
        {

            Texture = new Texture2D(textureSize, textureSize);
            Texture.filterMode = FilterMode.Bilinear;
            GameObject.GetComponent<Renderer>().material.mainTexture = Texture;
            //AssociatedGameObject.GetComponent<Renderer>().material.SetTextureOffset("_MainTex", new Vector2((float)0.0001, (float)0.0001));
            Texture = GameObject.GetComponent<Renderer>().material.mainTexture as Texture2D;
            for (int textureY = 0; textureY < textureSize; textureY++)
            {
                for (int textureX = 0; textureX < textureSize; textureX++)
                {
                    Color color = new Color((float)113 / 255, (float)125 / 255, (float)45 / 255);
                    if (textureX == 0 || textureX == textureSize - 1 || textureY == 0 || textureY == textureSize - 1)
                    {
                        color = new Color(0, 0, 1);
                    }
                    Texture.SetPixel(textureX, textureY, color);
                }
            }

            Texture.Apply();
            GameObject.GetComponent<Renderer>().material.enableInstancing = true;
        }

        public void recalculateAdjacentHeights()
        {
            List<Vector3> centerChunkHeights = new List<Vector3>();
            this.GameObject.GetComponent<MeshFilter>().mesh.GetVertices(centerChunkHeights);

            //Bord inférieur
            Chunk bottomChunk = getBottomChunk();
            if (bottomChunk != null)
            {
                List<Vector3> adjacentChunkHeights = new List<Vector3>();
                bottomChunk.GameObject.GetComponent<MeshFilter>().mesh.GetVertices(adjacentChunkHeights);
                for (int i = 0; i <= MeshSize; i++)
                {
                    Vector3 vertice = adjacentChunkHeights[(MeshSize + 1) * MeshSize + i];
                    adjacentChunkHeights[(MeshSize + 1) * MeshSize + i] = new Vector3(vertice.x, centerChunkHeights[i].y, vertice.z);
                }
                bottomChunk.GameObject.GetComponent<MeshFilter>().mesh.SetVertices(adjacentChunkHeights);
            }

            //Bord supérieur
            Chunk topchunk = getTopChunk();
            if (topchunk != null)
            {
                List<Vector3> adjacentChunkHeights = new List<Vector3>();
                topchunk.GameObject.GetComponent<MeshFilter>().mesh.GetVertices(adjacentChunkHeights);
                for (int i = 0; i <= MeshSize; i++)
                {
                    Vector3 vertice = adjacentChunkHeights[i];
                    adjacentChunkHeights[i] = new Vector3(vertice.x, centerChunkHeights[(MeshSize + 1) * MeshSize + i].y, vertice.z);
                }
                topchunk.GameObject.GetComponent<MeshFilter>().mesh.SetVertices(adjacentChunkHeights);
            }

            //Bord gauche
            Chunk leftchunk = getLeftChunk();
            if (leftchunk != null)
            {
                List<Vector3> adjacentChunkHeights = new List<Vector3>();
                leftchunk.GameObject.GetComponent<MeshFilter>().mesh.GetVertices(adjacentChunkHeights);
                for (int i = 0; i <= (MeshSize + 1) * MeshSize; i += MeshSize + 1)
                {
                    Vector3 vertice = adjacentChunkHeights[i + MeshSize];
                    adjacentChunkHeights[i + MeshSize] = new Vector3(vertice.x, centerChunkHeights[i].y, vertice.z);
                }
                leftchunk.GameObject.GetComponent<MeshFilter>().mesh.SetVertices(adjacentChunkHeights);
            }


            //Bord Droit
            Chunk rightchunk = getRightChunk();
            if (rightchunk != null)
            {
                List<Vector3> adjacentChunkHeights = new List<Vector3>();
                rightchunk.GameObject.GetComponent<MeshFilter>().mesh.GetVertices(adjacentChunkHeights);
                for (int i = 0; i <= (MeshSize + 1) * MeshSize; i += MeshSize + 1)
                {
                    Vector3 vertice = adjacentChunkHeights[i];
                    adjacentChunkHeights[i] = new Vector3(vertice.x, centerChunkHeights[i + MeshSize].y, vertice.z);
                }
                rightchunk.GameObject.GetComponent<MeshFilter>().mesh.SetVertices(adjacentChunkHeights);
            }

            //Coin inférieur gauche
            Chunk bottomLeftChunk = getBottomLeftChunk();
            if (bottomLeftChunk != null)
            {
                List<Vector3> adjacentChunkHeights = new List<Vector3>();
                bottomLeftChunk.GameObject.GetComponent<MeshFilter>().mesh.GetVertices(adjacentChunkHeights);
                Vector3 vertice = adjacentChunkHeights[(MeshSize + 1) * (MeshSize + 1) - 1];
                adjacentChunkHeights[(MeshSize + 1) * (MeshSize + 1) - 1] = new Vector3(vertice.x, centerChunkHeights[0].y, vertice.z);
                bottomLeftChunk.GameObject.GetComponent<MeshFilter>().mesh.SetVertices(adjacentChunkHeights);
            }

            //Coin inférieur droit
            Chunk bottomRightChunk = getBottomRightChunk();
            if (bottomRightChunk != null)
            {
                List<Vector3> adjacentChunkHeights = new List<Vector3>();
                bottomRightChunk.GameObject.GetComponent<MeshFilter>().mesh.GetVertices(adjacentChunkHeights);
                Vector3 vertice = adjacentChunkHeights[(MeshSize + 1) * MeshSize];
                adjacentChunkHeights[(MeshSize + 1) * MeshSize] = new Vector3(vertice.x, centerChunkHeights[MeshSize].y, vertice.z);
                bottomRightChunk.GameObject.GetComponent<MeshFilter>().mesh.SetVertices(adjacentChunkHeights);
            }

            //Coin supérieur gauche
            Chunk topLeftChunk = getTopRightChunk();
            if (topLeftChunk != null)
            {
                List<Vector3> adjacentChunkHeights = new List<Vector3>();
                topLeftChunk.GameObject.GetComponent<MeshFilter>().mesh.GetVertices(adjacentChunkHeights);
                Vector3 vertice = adjacentChunkHeights[0];
                adjacentChunkHeights[0] = new Vector3(vertice.x, centerChunkHeights[(MeshSize + 1) * (MeshSize + 1) - 1].y, vertice.z);
                topLeftChunk.GameObject.GetComponent<MeshFilter>().mesh.SetVertices(adjacentChunkHeights);
            }

            //Coin supérieur droit
            Chunk topRightChunk = getTopLeftChunk();
            if (topRightChunk != null)
            {
                List<Vector3> adjacentChunkHeights = new List<Vector3>();
                topRightChunk.GameObject.GetComponent<MeshFilter>().mesh.GetVertices(adjacentChunkHeights);
                Vector3 vertice = adjacentChunkHeights[MeshSize];
                adjacentChunkHeights[MeshSize] = new Vector3(vertice.x, centerChunkHeights[(MeshSize + 1) * MeshSize].y, vertice.z);
                topRightChunk.GameObject.GetComponent<MeshFilter>().mesh.SetVertices(adjacentChunkHeights);
            }
        }


        /*
         * Utilitaires:
         *  -top left bottom right chunk
         */

        public Chunk getLeftChunk()
        {
            int idChunkOnTheLeft = Id - 1;
            if (idChunkOnTheLeft >= 0 && (idChunkOnTheLeft / MainTerrain.Size) == (id / MainTerrain.Size))
            {
                return MainTerrain.ChunkList[idChunkOnTheLeft];
            }
            return null;
        }

        public Chunk getRightChunk()
        {
            int idChunkOnTheRight = Id + 1;
            if (idChunkOnTheRight < (1 + Id / MainTerrain.Size) * MainTerrain.Size)
            {
                return MainTerrain.ChunkList[idChunkOnTheRight];
            }
            return null;
        }

        public Chunk getTopChunk()
        {
            int idChunkAbove = Id - MainTerrain.Size;
            if (idChunkAbove >= 0)
            {
                return MainTerrain.ChunkList[idChunkAbove];
            }
            return null;
        }

        public Chunk getBottomChunk()
        {
            int idChunkUnder = Id + MainTerrain.Size;
            if (idChunkUnder < MainTerrain.Size * MainTerrain.Size)
            {
                return MainTerrain.ChunkList[idChunkUnder];
            }
            return null;
        }

        public Chunk getTopLeftChunk()
        {
            Chunk topChunk = getTopChunk();
            if (topChunk != null)
            {
                return topChunk.getLeftChunk();
            }
            return null;
        }

        public Chunk getTopRightChunk()
        {
            Chunk topChunk = getTopChunk();
            if (topChunk != null)
            {
                return topChunk.getRightChunk();
            }
            return null;
        }

        public Chunk getBottomLeftChunk()
        {
            Chunk bottomChunk = getBottomChunk();
            if (bottomChunk != null)
            {
                return bottomChunk.getLeftChunk();
            }
            return null;
        }

        public Chunk getBottomRightChunk()
        {
            Chunk bottomChunk = getBottomChunk();
            if (bottomChunk != null)
            {
                return bottomChunk.getRightChunk();
            }
            return null;
        }


    }
}
