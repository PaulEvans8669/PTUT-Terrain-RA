using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Data;
using Mono.Data.Sqlite;
using System;
using System.IO;

public class TerrainDatabase : MonoBehaviour {

    /* 
     * Le chargement se fait dans GenerateTerrain.cs 
     * car c'est l'endroit où est généré le terrain
     * donc fonction publique dans TerrainDatabase.cs
     * 
     */

    IDbConnection dbConnection;

    private int CHUNK_SIZE;
    private int TERRAIN_SIZE;

    private GenerateTerrain genTerComponent;


    // Use this for initialization
    void Start()
    {
        CHUNK_SIZE = gameObject.GetComponent<GenerateTerrain>().getChunkSize();
        TERRAIN_SIZE = gameObject.GetComponent<GenerateTerrain>().getTerrainSize();

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

        byte[] textureBytes = GetBytes(reader, 2);

        GenerateChunk genChuComponent = newChunk.GetComponent<GenerateChunk>();
        genChuComponent.getTexture().LoadRawTextureData(textureBytes);


        loadHeightsData(terrainId, chunkId);

    }

    private void clearTerrain()
    {
        foreach(Transform child in transform)
        {
            if (child.name.Contains("Chunk"))
            {
                Destroy(child);
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
        if (!terrainExistsInDatabase(terrain.name))
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
        string sqlSelectCommand = "SELECT COUNT(*) FROM 'Terrain' WHERE name = '"+ tName + "';";
        IDbCommand command = dbConnection.CreateCommand();
        command.CommandText = sqlSelectCommand;
        IDataReader reader = command.ExecuteReader();
        reader.Read();
        return (reader.GetInt32(0) == 1);
    }
    private void insertTerrainData()
    {
        IDbCommand command = dbConnection.CreateCommand();
        string sqlInsertCommand = "INSERT INTO 'Terrain' VALUES (null,'" + gameObject.name + "'," + TERRAIN_SIZE + ");";
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
        int terrainId = getIdOfTerrain(gameObject.name);
        string sqlInsertCommand = "INSERT INTO 'Chunk' VALUES ("+chunkId+",'" + terrainId + "',@bytes);";
        Texture2D chunkTexture = chunk.GetComponent<Renderer>().material.mainTexture as Texture2D;
        byte[] bytes = chunkTexture.EncodeToPNG();
        SqliteParameter param = new SqliteParameter("@bytes", DbType.Binary);
        command.CommandText = sqlInsertCommand;
        param.Value = bytes;
        command.Parameters.Add(param);
        command.ExecuteNonQuery();
        Debug.Log("Insert Chunk");
        insertHeightsData(chunk);
    }

    private void insertHeightsData(GameObject chunk)
    {

        List<Vector3> heights = new List<Vector3>();
        chunk.GetComponent<MeshFilter>().mesh.GetVertices(heights);

        int terrainId = getIdOfTerrain(gameObject.name);
        int chunkId = int.Parse(chunk.name.Split(' ')[1]);
        IDbCommand command = dbConnection.CreateCommand();
        string sqlInsertCommand = "INSERT INTO 'HeightMap' VALUES";
        for (int z = 0; z<=CHUNK_SIZE; z++)
        {
            for(int x = 0; x<=CHUNK_SIZE; x++)
            {
                int index = z * (CHUNK_SIZE + 1) + x;
                float height = heights[index].y;
                sqlInsertCommand += " (" + terrainId + "," + chunkId + "," + z + "," + x + ","+height+")";
                if(z== CHUNK_SIZE && x == CHUNK_SIZE)
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
        int terrainId = getIdOfTerrain(gameObject.name);
        IDbCommand command = dbConnection.CreateCommand();

        string sqlDeleteCommand = "DELETE FROM 'Chunk' WHERE terrainID = " + terrainId + ";";
        command.CommandText = sqlDeleteCommand;
        command.ExecuteNonQuery();

        sqlDeleteCommand = "DELETE FROM 'HeightMap' WHERE terrainID = " + terrainId + ";";
        command.CommandText = sqlDeleteCommand;
        command.ExecuteNonQuery();
    }

    
}
