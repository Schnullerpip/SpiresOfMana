using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Check_DamageDistribution : ICheck
{

    public string[] plotFile;

    /// <summary>
    /// saves damage per dealer per player - best for "how much damage was dealt to player X through fireballs this game?"
    /// </summary>
    public List<Dictionary<System.Type, List<damage_time>>> player_source_damage;

    /// <summary>
    /// saves damage per dealer - best for "how much damage did fireball in general this game?"
    /// </summary>
    public Dictionary<Type, List<damage_time>> source_damage;

    /// <summary>
    /// caches the last dealer for a player - to be able to check whether e.g. fireballs inflicted falldamage 
    /// </summary>
    public damageInstance[] player_lastDealer;

    [SerializeField]
    [Tooltip("In Seconds -> If a damagesource is unclear we will attribute it to the last 'dealer' if the last dealer occured n seconds ago")]
    private float mAttributionThreshold = 5.0f;

    /// <summary>
    /// simple POD to store damage amount at what time
    /// </summary>
    public struct damage_time
    {
        public int damage;
        public float time;
    }

    /// <summary>
    /// simple POD to store who dealt damage last and when specifically
    /// </summary>
    public struct damageInstance
    {
        public System.Type dealer;
        public damage_time dt;
    }

    //overall damage filePaths
    public static uint ACCUMULATED_DAMAGE = 0;
    public static uint DISCRETE_DAMAGE = 1;

    //per player damage filePaths
    public static uint PER_PLAYER_DELT_DAMAGE = 2;
    public static uint DISCRETE_PER_PLAYER_DELT_DAMAGE = 3;



    public void Init()
    {
        source_damage = new Dictionary<Type, List<damage_time>>();
        player_source_damage = new List<Dictionary<Type, List<damage_time>>>();
        player_lastDealer = new damageInstance[GameManager.instance.mPlayers.Count];

        //get the selected spells
	    for (int i = 0; i < GameManager.instance.mPlayers.Count; ++i)
	    {
	        var p = GameManager.instance.mPlayers[i];

            //initialize player's source_damage
            player_source_damage.Add(new Dictionary<Type, List<damage_time>>());

            //link to the player's OnStatsRelevantDamage event
	        var i1 = i;
	        p.healthScript.OnStatsRelevantDamage += (dmg, dealer) =>
	        {
                //check whether the damage should be attributed to the previous damage/effect source - e.g. falldamage should be attributed to whatever pushed the player off something
	            if (player_lastDealer[i1].dealer != null && (
                dealer == PlayerHealthScript.fallDamageInstance.GetType() ||
                dealer == typeof(LavaFloor) ||
                dealer == null ))
	            {
	                //but only do this if the last dealer occured shortly before
	                var timeDifferenceSincceLastDealer = Time.time - player_lastDealer[i1].dt.time;
	                if (timeDifferenceSincceLastDealer < mAttributionThreshold)
	                {
	                    //Attribute the damage, that was dealt to the last dealer, we now assume, that the last dealer's effect/damage resulted in this damage
	                    dealer = player_lastDealer[i1].dealer;
	                }
	            }

                var newDamageTime = new damage_time{damage = dmg, time = Time.time};

                //update player_source_damage
	            if (!player_source_damage[i1].ContainsKey(dealer))
	            {
	                player_source_damage[i1].Add(dealer, new List<damage_time>());
	            }
                player_source_damage[i1][dealer].Add(newDamageTime);

                //cache last dealer
	            player_lastDealer[i1].dealer = dealer;
	            player_lastDealer[i1].dt.damage = dmg;
	            player_lastDealer[i1].dt.time = Time.time;

                //update the source_damage
	            if (!source_damage.ContainsKey(dealer))
	            {
	                source_damage.Add(dealer, new List<damage_time>());
	            }
                source_damage[dealer].Add(newDamageTime);
	        };
	    }
    }

	// Use this for initialization
	public new void Start()
	{
	    base.Start();

        //revert all the log files, so they can e written freshly
        foreach (var p in fileName)
        {
            System.IO.File.WriteAllText(p,string.Empty);
        }
	    GameManager.OnGameStarted += Init;

        //copy the fileName array
        plotFile = new string[fileName.Length];
        fileName.CopyTo(plotFile, 0);
        //now change their endings to .m
        for(int i = 0; i < plotFile.Length; ++i)
	    {
	        plotFile[i] = plotFile[i].Replace(preferedFileExtension, ".m");
	    }
	}


    //----------------------- functions and methods relevant for LOG functionality  ----------------------------------------------//


    private static int AccumulateDamage(List<damage_time> dmg_time)
    {
        int accumulatedDamage = 0;
        foreach (var dt in dmg_time)
        {
            accumulatedDamage += dt.damage;
        }
        return accumulatedDamage;
    }

    private static void AppendDamageAndTime(List<damage_time> dmg_time, out int[] damage, out string time)
    {
        damage = new int[dmg_time.Count];
        time = "";
        for(int i = 0; i < dmg_time.Count; ++i)
        {
            var dt = dmg_time[i];
            damage[i] = dt.damage;
            time += dt.time;
            if (i < dmg_time.Count - 1)
            {
                time += ", ";
            }
        }
    }

    [ContextMenu("revert all damage-logfiles")]
    public void RevertAllFiles()
    {
        foreach (var f in fileName)
        {
            File.WriteAllText(f, string.Empty);
        }
    }

    [ContextMenu("log accumulated damage sources")]
    public void LogAccumulatedDamageSources()
    {

        int[] damageBars = new int[source_damage.Count];
        string barCode = "bar(sort([";
        string labelCode = "set(gca, 'xticklabel', {";

        var log_path = fileName[ACCUMULATED_DAMAGE];
        File.WriteAllText(log_path, string.Empty);
        var plot_path = plotFile[ACCUMULATED_DAMAGE];
        File.WriteAllText(plot_path, string.Empty);


        WriteString("----- dealers accumulated damage -----", log_path);
        var o = 0;
        foreach (var i in source_damage)
        {
            //log to file
            WriteString("" + i.Key + ":\n" + (damageBars[o] = AccumulateDamage(i.Value)), log_path);
            //log to plotFile
            barCode += damageBars[o] + "";
            labelCode += "'" + i.Key + "'";
            if (o < (source_damage.Count - 1))
            {
                barCode += ", ";
                labelCode += ", ";
            }
        }
        barCode += "]), 0.5)\n";
        labelCode += "})";
        WriteString(barCode, plot_path);
        WriteString(labelCode, plot_path);
    }

    //log function for non accumulated damage per player
    [ContextMenu("log discrete damage sources")]
    public void LogDiscreteDamageSources()
    {
        var logpath = fileName[DISCRETE_DAMAGE];
        File.WriteAllText(logpath, string.Empty);
        var plotpath = plotFile[DISCRETE_DAMAGE];
        File.WriteAllText(plotpath, string.Empty);

        string[] xAxis = new string[source_damage.Count];
        string[] data = new string[source_damage.Count];
        string[] legend = new string[source_damage.Count];


        WriteString("----- discrete dealers damage -----", logpath);
        var o = -1;
        foreach (var sd in source_damage)
        {
            ++o;
            string axisCode = "[";
            string dataCode = "[";
            int[] damageSteps;
            string timeSteps;
            AppendDamageAndTime(sd.Value, out damageSteps, out timeSteps);

            xAxis[o] = (axisCode += timeSteps + "]");
            //data[o] = (dataCode += damageSteps + "]");
            legend[o] = sd.Key+"";
            string damageLine = "";
            int accumulator = 0;
            for(int i = 0; i < damageSteps.Length; ++i)
            {
                damageLine += damageSteps[i];
                accumulator += damageSteps[i];
                dataCode += accumulator+"";
                if (i < damageSteps.Length - 1)
                {
                    damageLine += ", ";
                    dataCode += ", ";
                }
            }
            data[o] = (dataCode += "]");

            WriteString(sd.Key + ":", logpath);
            WriteString(damageLine, logpath);
            WriteString(timeSteps + "\n", logpath);
        }

        //accumulate damage for the plot

        string plotcode = "plot(";
        string legendCode = "legend(";
        for (int i = 0; i < source_damage.Count; ++i)
        {
            plotcode += xAxis[i] + ", " + data[i];
            legendCode += "'" + legend[i] + "' ";
            if (i < (source_damage.Count - 1))
            {
                plotcode += ", ";
                legendCode += ", ";
            }
        }
        plotcode += ")\n";
        legendCode += ")";
        WriteString(plotcode + legendCode, plotpath);
    }

    [ContextMenu("log all")]
    public void LogAll()
    {
        LogAccumulatedDamageSources();
        LogDiscreteDamageSources();
        LogDedicated();
        LogDiscreteDedicated();
    }



    [ContextMenu("log accumulated damage dealt to by dealer per player ")]
    public void LogDedicated()
    {
        var path = fileName[PER_PLAYER_DELT_DAMAGE];
        File.WriteAllText(path, string.Empty);
        PerPlayer(sd =>
            WriteString("" + sd.Key + " -> " + AccumulateDamage(sd.Value), path)
        );
    }

    //log function for non accumulated damage dealt per dealer per player
    [ContextMenu("log discrete damage dealt to by dealer per player ")]
    public void LogDiscreteDedicated()
    {
        var path = fileName[DISCRETE_PER_PLAYER_DELT_DAMAGE];
        File.WriteAllText(path, string.Empty);
        PerPlayer(sd => {
            string timeSteps;
            int[] damageSteps;

            AppendDamageAndTime(sd.Value, out damageSteps, out timeSteps);

            WriteString(sd.Key + ":", path);
            //WriteString(damageSteps, path);TODO
            WriteString(timeSteps + "\n", path);
        });
    }

    /// <summary>
    /// helper function for specific LogMechanisms
    /// </summary>
    /// <param name="mode"></param>
    private void PerPlayer(Action<KeyValuePair<Type, List<damage_time>>> mode)
    {
        var o = 0;
        foreach (var psd in player_source_damage)
        {
            var player = GameManager.instance.mPlayers[o++];
            WriteString("-----" + player.name + " -> (netId: " + player.netId + ") -----", fileName[PER_PLAYER_DELT_DAMAGE]);
            foreach (var i in psd)
            {
                mode(i);
            }
        }
    }
}