using Mono.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.lib
{
    class Chunk
    {
        private int textureSize = 256;
        
        public Terrain MainTerrain { get; set; }
        public GameObject ChunkGameObject { get; set; }
        public int Id { get; set; }
        public int MeshSize { get; set; }
        public Texture2D Texture { get; set; }
        public List<GameObject> VegetationList { get; set; }
        private bool visible;
        public bool Visible
        {
            get
            {
                return visible;
            }
            set
            {
                ChunkGameObject.SetActive(value);
                visible = value;
            }
        }
        public bool NeedsTextureUpdate { get; set; }
        public bool NeedsVegetationUpdate { get; set; }


        public Chunk(Terrain terrain, int id, int size)
        {
            this.MainTerrain = terrain;
            Id = id;
            MeshSize = 32;// size;
            NeedsTextureUpdate = false;
            NeedsVegetationUpdate = false;
            GenerateChunk();
            GenerateMesh();
            VegetationList = new List<GameObject>();
            GenerateTexture(true);
        }

        public Chunk(IDbConnection dbConnection, Terrain mainTerrain, int terrainId, int terrainSize, int id)
        {
            Id = id;
            MeshSize = 32;
            MainTerrain = mainTerrain;
            NeedsTextureUpdate = false;
            NeedsVegetationUpdate = false;
            VegetationList = new List<GameObject>();
            load(dbConnection, MainTerrain.TerrainGameObject, terrainId, terrainSize);
        }




        /* Generators
         * 
         */
        #region Generators
        private void GenerateChunk(GameObject terrainGameObject, int terrainSize)
        {
            int x = Id % terrainSize;
            int z = Id / terrainSize;

            ChunkGameObject = new GameObject("Chunk " + Id);
            ChunkGameObject.name = "Chunk " + (z * terrainSize + x);
            ChunkGameObject.transform.parent = terrainGameObject.transform;
            ChunkGameObject.transform.localPosition = new Vector3(Id % terrainSize * MeshSize, 0, -(Id / terrainSize) * MeshSize);
            ChunkGameObject.AddComponent<MeshFilter>();
            ChunkGameObject.AddComponent<MeshRenderer>();
            if (x < 4 && z < 4)
            {
                Visible = true;
            }
            else
            {
                Visible = false;
            }
        }
        private void GenerateChunk()
        {
            GenerateChunk(MainTerrain.TerrainGameObject, MainTerrain.Size);
        }

        private void GenerateMesh(List<Vector3> heightsList = null)
        {

            /*
             *  Vertices
             *  UVs
             *  Tangents
             */
            Mesh mesh;
            ChunkGameObject.GetComponent<MeshFilter>().mesh = mesh = new Mesh();
            mesh.name = "Chunk";

            List<Vector3> vertices = new List<Vector3>();
            List<Vector2> uv = new List<Vector2>();
            List<Vector4> tangents = new List<Vector4>();

            Vector4 tangent = new Vector4(1f, 0f, 0f, -1f);

            for (int i = 0, z = 0; z <= MeshSize; z++)
            {
                for (int x = 0; x <= MeshSize; x++, i++)
                {
                    vertices.Add(new Vector3(x, 0, z));
                    uv.Add(new Vector2((float)x / MeshSize, (float)z / MeshSize));
                    tangents.Add(tangent);
                }
            }
            if(heightsList != null)
            {
                foreach(Vector3 height in heightsList)
                {
                    int index = (int)height.z * (MeshSize + 1) + (int)height.x;
                    vertices[index] = height;
                }
            }
            mesh.MarkDynamic();
            mesh.SetVertices(vertices);
            mesh.SetUVs(0, uv);
            mesh.SetTangents(tangents);

            /*
             * Triangles
             */

            int[] triangles = new int[MeshSize * MeshSize * 6];
            for (int ti = 0, vi = 0, z = 0; z < MeshSize; z++, vi++)
            {
                for (int x = 0; x < MeshSize; x++, ti += 6, vi++)
                {
                    triangles[ti] = vi;
                    triangles[ti + 3] = triangles[ti + 2] = vi + 1;
                    triangles[ti + 4] = triangles[ti + 1] = vi + MeshSize + 1;
                    triangles[ti + 5] = vi + MeshSize + 2;
                }
            }

            mesh.SetTriangles(triangles, 0);
            mesh.RecalculateNormals();

            /*
             * Collider
             */

            MeshCollider meshCollider = ChunkGameObject.AddComponent<MeshCollider>();
            meshCollider.sharedMesh = mesh;


        }

        public void GenerateTexture(bool firstInit)
        {

            Texture = new Texture2D(textureSize, textureSize);
            Texture.filterMode = FilterMode.Bilinear;
            ChunkGameObject.GetComponent<Renderer>().material.mainTexture = Texture;
            //AssociatedGameObject.GetComponent<Renderer>().material.SetTextureOffset("_MainTex", new Vector2((float)0.0001, (float)0.0001));
            if (firstInit)
            {
                setBaseColorsOnTexture();
            }
            ChunkGameObject.GetComponent<Renderer>().material.enableInstancing = true;
        }

        private void setBaseColorsOnTexture()
        {
            Texture = ChunkGameObject.GetComponent<Renderer>().material.mainTexture as Texture2D;
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
        }

        public void addVegetation(UnityEngine.Object model, int localX, int localZ)
        {
            int index = localZ * (MeshSize+1) + localX;
            float height = ChunkGameObject.GetComponent<MeshFilter>().mesh.vertices[index].y;
            GameObject clone = GameObject.Instantiate(model, ChunkGameObject.transform.position + new Vector3(localX, height, localZ), Quaternion.identity) as GameObject;
            clone.transform.parent = ChunkGameObject.transform;
            clone.name = "vegetation";
            clone.isStatic = true;
            VegetationList.Add(clone);
            NeedsVegetationUpdate = true;
        }

        public void deleteVegetation(GameObject vegetation)
        {
            VegetationList.Remove(vegetation);
            GameObject.Destroy(vegetation);
        }

        #endregion

        /* Util:
         *  -top left bottom right chunk
         */
        #region Util

        public void moveNorth()
        {
            ChunkGameObject.transform.Translate(new Vector3(0, 0, MeshSize));
            if (ChunkGameObject.transform.position.z > 0)
            {
                ChunkGameObject.SetActive(false);
            }
            else
            {
                ChunkGameObject.SetActive(true);
            }
        }
        public void moveSouth()
        {
            ChunkGameObject.transform.Translate(new Vector3(0, 0, -MeshSize));
            if (ChunkGameObject.transform.position.z < -3*MeshSize)
            {
                ChunkGameObject.SetActive(false);
            }
            else
            {
                ChunkGameObject.SetActive(true);
            }
        }
        public void moveEast()
        {
            ChunkGameObject.transform.Translate(new Vector3(MeshSize, 0, 0));

            if (ChunkGameObject.transform.position.x > 3*MeshSize)
            {
                ChunkGameObject.SetActive(false);
            }
            else
            {
                ChunkGameObject.SetActive(true);
            }
        }
        public void moveWest()
        {
            ChunkGameObject.transform.Translate(new Vector3(-MeshSize, 0, 0));
            if (ChunkGameObject.transform.position.x < 0)
            {
                ChunkGameObject.SetActive(false);
            }
            else
            {
                ChunkGameObject.SetActive(true);
            }
        }

        public Chunk getLeftChunk()
        {
            int idChunkOnTheLeft = Id - 1;
            if (idChunkOnTheLeft >= 0 && (idChunkOnTheLeft / MainTerrain.Size) == (Id / MainTerrain.Size))
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
        #endregion

        /* Database:
         * -save
         * -load
         */
        #region Database
        public void save(IDbConnection dbConnection)
        {
            insertChunkData(dbConnection);
        }

        private void insertChunkData(IDbConnection dbConnection)
        {
            IDbCommand command = dbConnection.CreateCommand();
            string sqlInsertCommand = "INSERT INTO 'Chunk' VALUES (" + Id + ",'" + MainTerrain.Id + "',@bytes);";
            Texture2D chunkTexture = ChunkGameObject.GetComponent<Renderer>().material.mainTexture as Texture2D;
            byte[] bytes = chunkTexture.EncodeToPNG();
            SqliteParameter param = new SqliteParameter("@bytes", System.Data.DbType.Binary);
            command.CommandText = sqlInsertCommand;
            param.Value = bytes;
            command.Parameters.Add(param);
            command.ExecuteNonQuery();
            command.Dispose();
            Debug.Log("Insert Chunk (" + Id + ")");
            insertHeightsData(dbConnection);
            insertNatureData(dbConnection);
        }

        private void insertHeightsData(IDbConnection dbConnection)
        {
            List<Vector3> heights = new List<Vector3>();
            ChunkGameObject.GetComponent<MeshFilter>().mesh.GetVertices(heights);
            IDbCommand command = dbConnection.CreateCommand();
            string sqlInsertCommand = "INSERT INTO 'HeightMap' VALUES";
            for (int z = 0; z <= MeshSize; z++)
            {
                for (int x = 0; x <= MeshSize; x++)
                {
                    int index = z * (MeshSize + 1) + x;
                    float height = heights[index].y;
                    sqlInsertCommand += " (" + MainTerrain.Id + "," + Id + "," + z + "," + x + "," + height + ")";
                    if (z == MeshSize && x == MeshSize)
                    {
                        sqlInsertCommand += ";";
                    }
                    else
                    {
                        sqlInsertCommand += ",";
                    }
                }
            }
            command.CommandText = sqlInsertCommand;
            command.ExecuteNonQuery();
            command.Dispose();
            Debug.Log("Insert Heights");
        }

        private void insertNatureData(IDbConnection dbConnection)
        {
            IDbCommand command = dbConnection.CreateCommand();
            bool needsInsert = false;
            string sqlInsertCommand = "INSERT INTO 'Nature' VALUES ";

            foreach (GameObject vegetation in VegetationList)
            {
                float z = vegetation.transform.localPosition.z;
                float x = vegetation.transform.localPosition.x;
                float y = vegetation.transform.localPosition.y;
                string prefabName = vegetation.GetComponent<MeshFilter>().mesh.name.Replace(" Instance", "");
                sqlInsertCommand += "(" + MainTerrain.Id + "," + Id + "," + z + "," + x + "," + y + ",'" + prefabName + "'),";
                needsInsert = true;
            }
            if (needsInsert) // false si la liste de végétation est vide
            {
                sqlInsertCommand = sqlInsertCommand.Substring(0, sqlInsertCommand.Length - 1) + ";";
                Debug.Log(sqlInsertCommand);
                command.CommandText = sqlInsertCommand;
                command.ExecuteNonQuery();
            }
            command.Dispose();
        }

        private void load(IDbConnection dbConnection, GameObject terrainGameObject, int terrainId, int terrainSize)
        {
            string sqlSelectCommand = "SELECT * FROM 'Chunk' WHERE terrainId = " + terrainId + " AND chunkId =" + Id + ";";
            IDbCommand command = dbConnection.CreateCommand();
            command.CommandText = sqlSelectCommand;
            IDataReader reader = command.ExecuteReader();
            reader.Read();

            GenerateChunk(terrainGameObject, terrainSize);
            GenerateTexture(false);


            byte[] blobContent = new byte[0];
            blobContent = (byte[])reader["texture"];
            
            Texture.LoadImage(blobContent);//.LoadRawTextureData(blobContent);

            command.Dispose();
            loadHeightsData(dbConnection, terrainId);
            loadNature(dbConnection, terrainId);
        }

        private void loadHeightsData(IDbConnection dbConnection, int terrainId)
        {
            string sqlSelectCommand = "SELECT * FROM 'HeightMap' WHERE terrainId = " + terrainId + " AND chunkId =" + Id + ";";
            IDbCommand command = dbConnection.CreateCommand();
            command.CommandText = sqlSelectCommand;
            IDataReader reader = command.ExecuteReader();
            
            List<Vector3> vertices = new List<Vector3>();

            while (reader.Read())
            {
                int loadedChunkId = reader.GetInt32(1);
                int z = reader.GetInt32(2);
                int x = reader.GetInt32(3);
                float height = reader.GetFloat(4);

                vertices.Add(new Vector3(z, height, x));
            }

            command.Dispose();
            GenerateMesh(vertices);

        }

        private void loadNature(IDbConnection dbConnection, int terrainId)
        {
            string sqlSelectCommand = "SELECT * FROM 'Nature' WHERE terrainId = " + terrainId + " AND chunkId =" + Id + " ORDER BY type;";
            IDbCommand command = dbConnection.CreateCommand();
            command.CommandText = sqlSelectCommand;
            IDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {

                int chId = reader.GetInt32(1);
                float z = reader.GetFloat(2);
                float x = reader.GetFloat(3);
                float y = reader.GetFloat(4);
                string type = reader.GetString(5);

                GameObject model = null;
                if (model == null || !model.GetComponent<MeshFilter>().mesh.name.Replace(" Instance", "").Equals(type))
                {
                    model = Resources.Load("LowPolyNaturePackLite/w_Pallete/Prefabs/" + type, typeof(GameObject)) as GameObject;
                }
                GameObject c = GameObject.Instantiate(model, new Vector3(0, 0, 0), Quaternion.identity, ChunkGameObject.transform) as GameObject;
                c.transform.localPosition = new Vector3(x, y, z);
                c.name = "vegetation";
            }
            command.Dispose();
        }
        #endregion
    }
}
