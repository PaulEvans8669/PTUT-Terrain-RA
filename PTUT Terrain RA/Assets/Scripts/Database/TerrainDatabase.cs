using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Data;
using Mono.Data.Sqlite;
using System;
using System.IO;

public class TerrainDatabase : MonoBehaviour
{

    /* 
     * Le chargement se fait dans GenerateTerrain.cs 
     * car c'est l'endroit où est généré le terrain
     * donc fonction publique dans TerrainDatabase.cs
     * 
     */

    IDbConnection dbConnection;

    private int CHUNK_SIZE;
    private int TERRAIN_SIZE;
    private List<GameObject> chunkList;

    private GenerateTerrain genTerComponent;


    // Use this for initialization
    void Start()
    {
        CHUNK_SIZE = gameObject.GetComponent<GenerateTerrain>().getChunkSize();
        TERRAIN_SIZE = gameObject.GetComponent<GenerateTerrain>().getTerrainSize();
        chunkList = gameObject.GetComponent<GenerateTerrain>().getChunkList();

        string dbFilePath = "Assets/Resources/mapsData.db";
        dbConnection = (IDbConnection)new SqliteConnection("URI=file:" + dbFilePath);

        openConnection();

        genTerComponent = GetComponent<GenerateTerrain>();
    }
    void OnApplicationQuit()
    {
        closeConnection();
    }


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.S)) //Save
        {
            saveTerrain(this.gameObject);
        }
        if (Input.GetKeyDown(KeyCode.C)) //Close
        {
            closeConnection();
        }
        if (Input.GetKeyDown(KeyCode.O)) //Open
        {
            openConnection();
        }
        if (Input.GetKeyDown(KeyCode.L)) //Load
        {
            loadTerrain(4);
        }
    }

    public void openConnection()
    {
        if (dbConnection.State.Equals(ConnectionState.Closed))
        {
            dbConnection.Open();
            Debug.Log("Connection opened.");
        }
        else
        {
            Debug.Log("Connection not closed.");
        }
    }

    public void loadTerrain(int terrainId)
    {
        clearTerrain();
        openConnection();
        string sqlSelectCommand = "SELECT * FROM 'Terrain' WHERE terrainId = " + terrainId + ";";
        IDbCommand command = dbConnection.CreateCommand();
        command.CommandText = sqlSelectCommand;
        IDataReader reader = command.ExecuteReader();
        reader.Read();

        string name = reader.GetString(1);
        int loadedTerrainSize = reader.GetInt32(2);

        GetComponent<GenerateTerrain>().setTerrainSize(loadedTerrainSize);
        GetComponent<GenerateTerrain>().setId(terrainId);
        GetComponent<GenerateTerrain>().setName(name);

        for (int chunkId = 0; chunkId < loadedTerrainSize * loadedTerrainSize; chunkId++)
        {
            loadChunk(terrainId, chunkId);
        }


    }

    public void loadChunk(int terrainId, int chunkId)
    {
        string sqlSelectCommand = "SELECT * FROM 'Chunk' WHERE terrainId = " + terrainId + " AND chunkId =" + chunkId + ";";
        IDbCommand command = dbConnection.CreateCommand();
        command.CommandText = sqlSelectCommand;
        IDataReader reader = command.ExecuteReader();
        reader.Read();

        GameObject modelChunk = GameObject.Find("ModelChunk");
        int terrainSize = genTerComponent.getTerrainSize();

        int z = chunkId / terrainSize;
        int x = chunkId % terrainSize;
        
        GameObject newChunk = Instantiate(modelChunk, new Vector3(x * CHUNK_SIZE, 0, -z * CHUNK_SIZE), Quaternion.identity, this.gameObject.transform);
        newChunk.name = "Chunk " + (z * TERRAIN_SIZE + x);
        newChunk.AddComponent<GenerateChunk>();

        List<GameObject> chunkList = genTerComponent.getChunkList();
        chunkList.Add(newChunk);
        genTerComponent.setChunkList(chunkList);


        byte[] blobContent = new byte[0];
        blobContent = (byte[])reader["texture"];

        GenerateChunk genChuComponent = newChunk.GetComponent<GenerateChunk>();
        genChuComponent.getTexture().LoadImage(blobContent);//.LoadRawTextureData(blobContent);


        loadHeightsData(terrainId, chunkId);
        loadNature(terrainId, chunkId, newChunk);

    }

    private void loadNature(int terrainId, int chunkId, GameObject chunk)
    {
        string sqlSelectCommand = "SELECT * FROM 'Nature' WHERE terrainId = " + terrainId + " AND chunkId =" + chunkId + " ORDER BY type;";
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
            if (model == null || !model.GetComponent<MeshFilter>().mesh.name.Replace(" Instance","").Equals(type))
            {
                model = Resources.Load("LowPolyNaturePackLite/w_Pallete/Prefabs/"+type, typeof(GameObject)) as GameObject;
            }
            GameObject c = Instantiate(model, new Vector3(0,0,0), Quaternion.identity, chunk.transform) as GameObject;
            c.transform.localPosition = new Vector3(x, y, z);
            c.name = "vegetation";
        }
    }

    private void clearTerrain()
    {
        foreach (Transform child in transform)
        {
            if (child.name.Contains("Chunk"))
            {
                Destroy(child.gameObject);
            }
        }
    }

    public byte[] GetBytes(IDataReader reader, int column)
    {
        using (MemoryStream ms = new MemoryStream())
        {
            byte[] buff = new byte[8192];
            long offset = 0L;
            long n = 0L;
            do
            {
                n = reader.GetBytes(column, offset, buff, 0, buff.Length);
                ms.Write(buff, 0, (int)n);
                offset += n;
            } while (n >= buff.Length);
            return ms.ToArray();
        }
    }

    private void loadHeightsData(int terrainId, int chunkId)
    {
        string sqlSelectCommand = "SELECT * FROM 'HeightMap' WHERE terrainId = " + terrainId + " AND chunkId =" + chunkId + ";";
        IDbCommand command = dbConnection.CreateCommand();
        command.CommandText = sqlSelectCommand;
        IDataReader reader = command.ExecuteReader();

        GameObject chunkToEdit = GameObject.Find("Chunk " + chunkId);
        List<Vector3> vertices = new List<Vector3>();
        chunkToEdit.GetComponent<MeshFilter>().mesh.GetVertices(vertices);

        while (reader.Read())
        {
            int loadedChunkId = reader.GetInt32(1);
            int z = reader.GetInt32(2);
            int x = reader.GetInt32(3);
            float height = reader.GetFloat(4);

            int index = z * (CHUNK_SIZE + 1) + x;

            vertices[index] = new Vector3(z, height, x);
        }

        chunkToEdit.GetComponent<MeshFilter>().mesh.SetVertices(vertices);
        chunkToEdit.GetComponent<MeshCollider>().sharedMesh = chunkToEdit.GetComponent<MeshFilter>().mesh;

    }


    public void closeConnection()
    {
        if (!dbConnection.State.Equals(ConnectionState.Closed))
        {
            dbConnection.Close();
            Debug.Log("Connection closed.");
        }
        else
        {
            Debug.Log("Connection already closed.");
        }
    }

    public void saveTerrain(GameObject terrain)
    {
        if (!terrainExistsInDatabase(GetComponent<GenerateTerrain>().getName()))
        {
            insertTerrainData();
        }
        else
        {
            Debug.Log("Terrain déjà existant, mise à jour des chunks");
            updateTerrainData();
        }
        Debug.Log("Sauvegarde terminée avec succès!");
    }

    private Boolean terrainExistsInDatabase(string tName)
    {
        string sqlSelectCommand = "SELECT COUNT(*) FROM 'Terrain' WHERE name = '" + tName + "';";
        IDbCommand command = dbConnection.CreateCommand();
        command.CommandText = sqlSelectCommand;
        IDataReader reader = command.ExecuteReader();
        reader.Read();
        return (reader.GetInt32(0) == 1);
    }
    private void insertTerrainData()
    {
        IDbCommand command = dbConnection.CreateCommand();
        string sqlInsertCommand = "INSERT INTO 'Terrain' VALUES (null,'" + GetComponent<GenerateTerrain>().getName() + "'," + TERRAIN_SIZE + ");";
        command.CommandText = sqlInsertCommand;
        command.ExecuteNonQuery();
        Debug.Log("Insert Terrain");
        foreach (Transform child in transform)
        {
            if (child.name.Contains("Chunk"))
            {
                insertChunkData(child.gameObject);
            }
        }

    }

    private void insertChunkData(GameObject chunk)
    {
        int chunkId = int.Parse(chunk.name.Split(' ')[1]);
        IDbCommand command = dbConnection.CreateCommand();
        int terrainId = getIdOfTerrain(GetComponent<GenerateTerrain>().getName());
        string sqlInsertCommand = "INSERT INTO 'Chunk' VALUES (" + chunkId + ",'" + terrainId + "',@bytes);";
        Texture2D chunkTexture = chunk.GetComponent<Renderer>().material.mainTexture as Texture2D;
        byte[] bytes = chunkTexture.EncodeToPNG();
        SqliteParameter param = new SqliteParameter("@bytes", DbType.Binary);
        command.CommandText = sqlInsertCommand;
        param.Value = bytes;
        command.Parameters.Add(param);
        command.ExecuteNonQuery();
        Debug.Log("Insert Chunk");
        insertHeightsData(chunk);
        insertNatureData(chunk);
    }

    private void insertHeightsData(GameObject chunk)
    {

        List<Vector3> heights = new List<Vector3>();
        chunk.GetComponent<MeshFilter>().mesh.GetVertices(heights);

        int terrainId = getIdOfTerrain(GetComponent<GenerateTerrain>().getName());
        int chunkId = int.Parse(chunk.name.Split(' ')[1]);
        IDbCommand command = dbConnection.CreateCommand();
        string sqlInsertCommand = "INSERT INTO 'HeightMap' VALUES";
        for (int z = 0; z <= CHUNK_SIZE; z++)
        {
            for (int x = 0; x <= CHUNK_SIZE; x++)
            {
                int index = z * (CHUNK_SIZE + 1) + x;
                float height = heights[index].y;
                sqlInsertCommand += " (" + terrainId + "," + chunkId + "," + z + "," + x + "," + height + ")";
                if (z == CHUNK_SIZE && x == CHUNK_SIZE)
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
        Debug.Log("Insert Heights");
    }



    private int getIdOfTerrain(string tName)
    {
        string sqlSelectCommand = "SELECT terrainId FROM 'Terrain' WHERE name = '" + tName + "';";
        IDbCommand command = dbConnection.CreateCommand();
        command.CommandText = sqlSelectCommand;
        IDataReader reader = command.ExecuteReader();
        reader.Read();
        return reader.GetInt32(0);
    }

    private void updateTerrainData()
    {
        deleteChunkData();

        foreach (Transform child in transform)
        {
            if (child.name.Contains("Chunk"))
            {
                insertChunkData(child.gameObject);
            }
        }

    }

    private void deleteChunkData()
    {
        int terrainId = getIdOfTerrain(GetComponent<GenerateTerrain>().getName());
        IDbCommand command = dbConnection.CreateCommand();

        string sqlDeleteCommand = "DELETE FROM 'Chunk' WHERE terrainID = " + terrainId + ";";
        command.CommandText = sqlDeleteCommand;
        command.ExecuteNonQuery();

        sqlDeleteCommand = "DELETE FROM 'HeightMap' WHERE terrainID = " + terrainId + ";";
        command.CommandText = sqlDeleteCommand;
        command.ExecuteNonQuery();

        sqlDeleteCommand = "DELETE FROM 'Nature' WHERE terrainID = " + terrainId + ";";
        command.CommandText = sqlDeleteCommand;
        command.ExecuteNonQuery();
    }

    private void insertNatureData(GameObject chunk)
    {
        int terrainId = getIdOfTerrain(GetComponent<GenerateTerrain>().getName());
        int chunkId = int.Parse(chunk.name.Split(' ')[1]);
        IDbCommand command = dbConnection.CreateCommand();
        bool needsInsert = false;
        string sqlInsertCommand = "INSERT INTO 'Nature' VALUES ";

        foreach (Transform child in chunk.transform)
        {
            if (child.gameObject.name.Equals("vegetation"))
            {
                float z = child.transform.localPosition.z;
                float x = child.transform.localPosition.x;
                float y = child.transform.localPosition.y;
                string prefabName = child.gameObject.GetComponent<MeshFilter>().mesh.name.Replace(" Instance","");
                sqlInsertCommand += "(" + terrainId + "," + chunkId + "," + z + "," + x +","+y+ ",'" + prefabName + "'),";
                needsInsert = true;

            }
        }
        if (needsInsert)
        {
            sqlInsertCommand = sqlInsertCommand.Substring(0, sqlInsertCommand.Length - 1) + ";";
            Debug.Log(sqlInsertCommand);
            command.CommandText = sqlInsertCommand;
            command.ExecuteNonQuery();
        }
    }

}
