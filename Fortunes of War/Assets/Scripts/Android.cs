using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using DataService;
using Mono.Data.Sqlite;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class Android : MonoBehaviour {
    private string connection;
    private IDbConnection dbCon;
    private IDbCommand dbCmd;
    private IDataReader dbReader;
    private string databaseName = "Mercenaries.s3db";
    private string tableName = "Mercenaries";
    string dataMercs;
    // Start is called before the first frame update
    void Start () {
        string filepath = $"{Application.persistentDataPath}/{databaseName}";
        if (!File.Exists (filepath)) {
            // If not found, create new tables and database
            Debug.LogWarning ($"File \"{filepath}\" does not exist. Attempt to create from \"{Application.dataPath}!/Assets/DB/Mercenaries");

            // UNITY_ANDROID
            UnityWebRequest loadDB = new UnityWebRequest ($"jar:file://{Application.dataPath}!/Assets/DB/Mercenaries.s3db");
            byte[] dbBytes = BitConverter.GetBytes (loadDB.downloadedBytes);
            while (!loadDB.isDone) {
                //save to Application.persistentDataPath
                File.WriteAllBytes (filepath, dbBytes);
            }
        }

        connection = $"URI=File: {filepath}";
        Debug.Log ($"Establishing connection to: {connection}");
        dbCon = new SqliteConnection (connection);
        dbCon.Open ();

        string createTable = @"CREATE TABLE IF NOT EXIST \" + tableName + @" ( 
                ID FLOAT(24) PRIMARY KEY NOT NULL,
                Name VARCHAR(255) NOT NULL,
                AttackName VARCHAR(255) NOT NULL,
                AttackInfo VARCHAR(1000) NOT NULL,
                AttackDamage FLOAT(24) NOT NULL,
                AttackRange FLOAT(24) NOT NULL,
                CrestName VARCHAR(255) NOT NULL,
                CrestInfo VARCHAR(1000) NOT NULL,
                CrestDamage FLOAT(24) NOT NULL,
                ClassType VARCHAR(255) NOT NULL,
                PassiveName VARCHAR(255),
                Health FLOAT(24) NOT NULL,
                Movement FLOAT(24) NOT NULL,
                Rate FLOAT(24) NOT NULL,
                Tier VARCHAR(255) NOT NULL)";
        try {
            // // create table
            dbCmd = dbCon.CreateCommand ();
            dbCmd.CommandText = createTable;
            dbReader = dbCmd.ExecuteReader ();
        } catch (Exception e) {
            Debug.Log ($"Create table exception: {e}");
        }
        dbCon.Close ();
        readData ();
    }

    // Insert Function
    public void insertData (string name, string atkName, string atkInfo, float atkDmg, float atkRange,
        string crestName, string crestInfo, float crestDmg, string classType, string passiveName, float health, float movement, float rate, DataService.DataService.Tier tier) {
        using (dbCon = new SqliteConnection (connection)) {
            dbCon.Open ();
            dbCmd = dbCon.CreateCommand ();
            string insertItem = @"INSERT INTO \" + tableName + @" 
            (Name, AttackName, AttackInfo, AttackDamage, AttackRange, 
            CrestName, CrestInfo, CrestDamage, ClassType, PassiveName, Health, Movement) 
            VALUES ({name}, {atkName}, {atkInfo}, {atkDmg}, {atkRange}, 
            {crestName}, {crestInfo}, {crestDmg}, {classType}, {passiveName}, {health}, {movement}, {rate}, {tier})";
            dbCmd.CommandText = insertItem;
            dbCmd.ExecuteScalar ();
            dbCon.Close ();
        }
        Debug.Log ("Insert Done");
        readData ();
    }

    // Read Function
    public void readData () {
        string name, atkName, atkInfo, crestName, crestInfo, classType, passiveName;
        float id, atkDmg, atkRange, crestDmg, health, movement;
        using (dbCon = new SqliteConnection (connection)) {
            dbCon.Open ();
            dbCmd = dbCon.CreateCommand ();
            string readItem = $"";
            dbCmd.CommandText = readItem;
            dbReader = dbCmd.ExecuteReader ();
            while (dbReader.Read ()) {
                id = dbReader.GetFloat (0);
                name = dbReader.GetString (2);
                atkName = dbReader.GetString (3);
                atkInfo = dbReader.GetString (4);
                atkDmg = dbReader.GetFloat (5);
                atkRange = dbReader.GetFloat (6);
                crestName = dbReader.GetString (7);
                crestInfo = dbReader.GetString (8);
                crestDmg = dbReader.GetFloat (9);
                classType = dbReader.GetString (10);
                passiveName = dbReader.GetString (11);
                health = dbReader.GetFloat (12);
                movement = dbReader.GetFloat (13);

                dataMercs += $"{id}, {name}, \n{atkName}, {atkInfo}, {atkDmg}, {atkRange}, \n{crestName}, {crestInfo}, {crestDmg}, \n{classType}, \n{passiveName}, \n{health}, \n{movement}";
                Debug.Log ($"Read Done: {dataMercs}");
            }
            dbReader.Close ();
            dbReader = null;
            dbCmd.Dispose ();
            dbCmd = null;
            dbCon.Close ();
        }
    }

    public void searchData (float ID) {
        using (dbCon = new SqliteConnection (connection)) {
            string name, atkName, atkInfo, crestName, crestInfo, classType, passiveName;
            float id, atkDmg, atkRange, crestDmg, health, movement;
            dbCon.Open ();
            dbCmd = dbCon.CreateCommand ();
            string searchItem = @"SELECT Name, AttackName, AttackInfo, AttackDamage, AttackRange, 
            CrestName, CrestInfo, CrestDamage, ClassType, PassiveName, Health, Movement FROM \" + tableName + @" where ID = \" + ID;
            dbCmd.CommandText = searchItem;
            dbReader = dbCmd.ExecuteReader ();
            while (dbReader.Read ()) {
                id = dbReader.GetFloat (0);
                name = dbReader.GetString (2);
                atkName = dbReader.GetString (3);
                atkInfo = dbReader.GetString (4);
                atkDmg = dbReader.GetFloat (5);
                atkRange = dbReader.GetFloat (6);
                crestName = dbReader.GetString (7);
                crestInfo = dbReader.GetString (8);
                crestDmg = dbReader.GetFloat (9);
                classType = dbReader.GetString (10);
                passiveName = dbReader.GetString (11);
                health = dbReader.GetFloat (12);
                movement = dbReader.GetFloat (13);

                dataMercs += $" { id }, { name }, \n { atkName }, { atkInfo }, { atkDmg }, { atkRange }, \n { crestName }, { crestInfo }, { crestDmg }, \n { classType }, \n { passiveName }, \n { health }, \n { movement }";
                Debug.Log ($"Search Done: {dataMercs}");
            }
            dbReader.Close ();
            dbReader = null;
            dbCmd.Dispose ();
            dbCmd = null;
            dbCon.Close ();
        }
    }

    public void updateData (float id, string name, string atkName, string atkInfo, float atkDmg, float atkRange,
        string crestName, string crestInfo, float crestDmg, string classType, string passiveName, float health, float movement) {
        using (dbCon = new SqliteConnection (connection)) {
            dbCon.Open ();
            dbCmd = dbCon.CreateCommand ();
            string updateItem = $"UPDATE { tableName } SET Name = { name }, AttackName = { atkName }, AttackInfo = { atkInfo }, AttackDamage = { atkDmg }, AttackRange = { atkRange }, CrestName = { crestName }, CrestInfo = { crestInfo }, CrestDamage = { crestDmg }, ClassType = { classType }, PassiveName = { passiveName }, Health = { health }, Movement = { movement } WHERE ID = { id }";
            dbCmd.CommandText = updateItem;
            dbCmd.ExecuteScalar ();
            dbCon.Close ();
            searchData (id);
        }
    }

    public void deleteData (float id) {
        using (dbCon = new SqliteConnection (connection)) {
            dbCon.Open ();
            dbCmd = dbCon.CreateCommand ();
            string deleteItem = $"DELETE FROM { tableName } WHERE ID = { id }";
            dbCmd.CommandText = deleteItem;
            dbReader = dbCmd.ExecuteReader ();

            dbCmd.Dispose ();
            dbCmd = null;
            dbCon.Close ();
            dataMercs = $" Delete Done: { id }";
            Debug.Log ($"{dataMercs}");
        }
    }
    // Update is called once per frame
    void Update () {

    }
}