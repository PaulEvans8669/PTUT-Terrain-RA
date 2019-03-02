using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Data;
using Mono.Data.Sqlite;
using System;

public class TerrainDatabase : MonoBehaviour {

    IDbConnection dbConnection;

    private int CHUNK_SIZE;
    private int TERRAIN_SIZE;
    private string terrainName;

    // Use this for initialization
    void Start()
    {
        CHUNK_SIZE = this.gameObject.GetComponent<GenerateTerrain>().getChunkSize();
        TERRAIN_SIZE = this.gameObject.GetComponent<GenerateTerrain>().getTerrainSize();
        openConnection();
        terrainName = this.gameObject.name;
    }

    void OnApplicationQuit()
    {
        closeConnection();
    }


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            saveTerrain(this.gameObject);
        }
        if (Input.GetKeyDown(KeyCode.C))
        {
            closeConnection();
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            openConnection();
        }
    }

    public void openConnection()
    {
        string dbFilePath = "Assets/Resources/mapsData.db";
        dbConnection = (IDbConnection)new SqliteConnection("URI=file:" + dbFilePath);
        dbConnection.Open();
    }

    public void closeConnection()
    {
        dbConnection.Close();
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
        string sqlInsertCommand = "INSERT INTO 'Terrain' VALUES (null,'" + terrainName + "'," + TERRAIN_SIZE + ");";
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
        int terrainId = getIDofTerrain(terrainName);
        string sqlInsertCommand = "INSERT INTO 'Chunk' VALUES ("+chunkId+",'" + terrainId + "',@bytes);";
        Texture2D chunkTexture = chunk.GetComponent<Renderer>().material.mainTexture as Texture2D;
        byte[] bytes = chunkTexture.EncodeToPNG();
        SqliteParameter param = new SqliteParameter("@bytes", System.Data.DbType.Binary);
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

        int terrainId = getIDofTerrain(terrainName);
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

    private int getIDofTerrain(string tName)
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
        int terrainId = getIDofTerrain(terrainName);
        IDbCommand command = dbConnection.CreateCommand();

        string sqlDeleteCommand = "DELETE FROM 'Chunk' WHERE terrainID = " + terrainId + ";";
        command.CommandText = sqlDeleteCommand;
        command.ExecuteNonQuery();

        sqlDeleteCommand = "DELETE FROM 'HeightMap' WHERE terrainID = " + terrainId + ";";
        command.CommandText = sqlDeleteCommand;
        command.ExecuteNonQuery();
    }

}
