using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
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

        connection = $"URI=file:{filepath}";
        Debug.Log ($"Establishing connection to: {connection}");
        dbCon = new SqliteConnection (connection);
        dbCon.Open ();

        string createTable = @"CREATE TABLE IF NOT EXISTS " + tableName + @" ( 
                Id INT(24) PRIMARY KEY NOT NULL,
                Name VARCHAR(255) NOT NULL,
                AttackName VARCHAR(255) NOT NULL,
                AttackInfo VARCHAR(1000) NOT NULL,
                AttackDamage INT(24) NOT NULL,
                AttackRange INT(24) NOT NULL,
                CrestName VARCHAR(255) NOT NULL,
                CrestInfo VARCHAR(1000) NOT NULL,
                CrestDamage INT(24) NOT NULL,
                ClassType VARCHAR(255) NOT NULL,
                PassiveName VARCHAR(255),
                Health INT(24) NOT NULL,
                Movement INT(24) NOT NULL,
                Rate DECIMAL (24, 6) NOT NULL,
                Tier VARCHAR(255) NOT NULL,
                Power INT(24) NOT NULL,
                Multiplier DECIMAL(24, 6) NOT NULL)";
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
    public void insertData (string name, string atkName, string atkInfo, int atkDmg, int atkRange,
        string crestName, string crestInfo, int crestDmg, string classType, string passiveName,
        int health, int movement, double rate, Service.Tier tier, double multiplier) {
        using (dbCon = new SqliteConnection (connection)) {
            dbCon.Open ();
            dbCmd = dbCon.CreateCommand ();
            string insertItem = @"INSERT INTO \" + tableName + @" 
            (Name, AttackName, AttackInfo, AttackDamage, AttackRange, 
            CrestName, CrestInfo, CrestDamage, ClassType, PassiveName, Health, Movement, Rate, Tier, Multiplier) 
            VALUES ({name}, {atkName}, {atkInfo}, {atkDmg}, {atkRange}, 
            {crestName}, {crestInfo}, {crestDmg}, {classType}, {passiveName}, {health}, {movement}, {rate}, {tier}, {multiplier})";
            dbCmd.CommandText = insertItem;
            dbCmd.ExecuteScalar ();
            dbCon.Close ();
        }
        Debug.Log ("Insert Done");
        readData ();
    }

    // Read Function
    public void readData () {
        string name, atkName, atkInfo, crestName, crestInfo, classType, passiveName, tier;
        int id, atkDmg, atkRange, crestDmg, health, movement;
        double rate, multiplier;
        using (dbCon = new SqliteConnection (connection)) {
            dbCon.Open ();
            dbCmd = dbCon.CreateCommand ();
            string readItem = $"SELECT * FROM {tableName}";
            dbCmd.CommandText = readItem;
            dbReader = dbCmd.ExecuteReader ();
            while (dbReader.Read ()) {
                id = dbReader.GetInt32 (0);
                name = dbReader.GetString (1);
                atkName = dbReader.GetString (2);
                atkInfo = dbReader.GetString (3);
                atkDmg = dbReader.GetInt32 (4);
                atkRange = dbReader.GetInt32 (5);
                crestName = dbReader.GetString (6);
                crestInfo = dbReader.GetString (7);
                crestDmg = dbReader.GetInt32 (8);
                classType = dbReader.GetString (9);
                passiveName = dbReader.IsDBNull (10) ? "" : dbReader.GetString (10);
                health = dbReader.GetInt32 (11);
                movement = dbReader.GetInt32 (12);
                rate = dbReader.GetDouble (13);
                tier = dbReader.GetString (14);
                multiplier = dbReader.GetDouble (15);

                dataMercs += $"{id}, {name}, \n{atkName}, \n{atkInfo}, \n{atkDmg}, \n{atkRange}, \n{crestName}, \n{crestInfo}, \n{crestDmg}, \n{classType}, \n{passiveName}, \n{health}, \n{movement}, \n{rate}, \n{tier}, \n{multiplier}";
                Debug.Log ($"Read Done: {dataMercs}");
            }
            dbReader.Close ();
            dbReader = null;
            dbCmd.Dispose ();
            dbCmd = null;
            dbCon.Close ();
        }
    }

    public void searchData (int ID) {
        using (dbCon = new SqliteConnection (connection)) {
            string name, atkName, atkInfo, crestName, crestInfo, classType, passiveName, tier;
            int id, atkDmg, atkRange, crestDmg, health, movement, rate, multiplier;
            dbCon.Open ();
            dbCmd = dbCon.CreateCommand ();
            string searchItem = @"SELECT Name, AttackName, AttackInfo, AttackDamage, AttackRange, 
            CrestName, CrestInfo, CrestDamage, ClassType, PassiveName, Health, Movement, Rate, Tier, Multiplier FROM \" + tableName + @" where ID = \" + ID;
            dbCmd.CommandText = searchItem;
            dbReader = dbCmd.ExecuteReader ();
            while (dbReader.Read ()) {
                id = dbReader.GetInt32 (0);
                name = dbReader.GetString (2);
                atkName = dbReader.GetString (3);
                atkInfo = dbReader.GetString (4);
                atkDmg = dbReader.GetInt32 (5);
                atkRange = dbReader.GetInt32 (6);
                crestName = dbReader.GetString (7);
                crestInfo = dbReader.GetString (8);
                crestDmg = dbReader.GetInt32 (9);
                classType = dbReader.GetString (10);
                passiveName = dbReader.GetString (11);
                health = dbReader.GetInt32 (12);
                movement = dbReader.GetInt32 (13);
                rate = dbReader.GetInt32 (14);
                tier = dbReader.GetString (15);
                multiplier = dbReader.GetInt32 (16);

                dataMercs += $"{id}, {name}, \n{atkName}, \n{atkInfo}, \n{atkDmg}, \n{atkRange}, \n{crestName}, \n{crestInfo}, \n{crestDmg}, \n{classType}, \n{passiveName}, \n{health}, \n{movement}, \n{rate}, \n{tier}, \n{multiplier}";
                Debug.Log ($"Search Done: {dataMercs}");
            }
            dbReader.Close ();
            dbReader = null;
            dbCmd.Dispose ();
            dbCmd = null;
            dbCon.Close ();
        }
    }

    public void updateData (int id, string name, string atkName, string atkInfo, int atkDmg, int atkRange,
        string crestName, string crestInfo, int crestDmg, string classType, string passiveName, int health, int movement, double rate, Service.Tier tier, double multiplier) {
        using (dbCon = new SqliteConnection (connection)) {
            dbCon.Open ();
            dbCmd = dbCon.CreateCommand ();
            string updateItem = $"UPDATE { tableName } SET Name = { name }, AttackName = { atkName }, AttackInfo = { atkInfo }, AttackDamage = { atkDmg }, AttackRange = { atkRange }, CrestName = { crestName }, CrestInfo = { crestInfo }, CrestDamage = { crestDmg }, ClassType = { classType }, PassiveName = { passiveName }, Health = { health }, Movement = { movement }, Rate = {rate}, Tier = {tier}, Multiplier = {multiplier} WHERE ID = { id }";
            dbCmd.CommandText = updateItem;
            dbCmd.ExecuteScalar ();
            dbCon.Close ();
            searchData (id);
        }
    }

    public void deleteData (int id) {
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