using System.Collections.Generic;
using UnityEngine;

public class WorkerStats {

    private List<int> possibleStats = new() { 5, 4, 4, 3, 3, 3, 2, 2, 2, 2, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0 };
    public WorkerStats(int type)
    {
        if(type == 0)
        {
            this.alchemyStat = 3;
            this.summonStat = 3;
            this.enchantStat = 3;
            this.adivinationStat = 3;

        }
        else if(type == 1)
        {
            this.alchemyStat = possibleStats[Random.Range(0, possibleStats.Count)];
            this.summonStat = possibleStats[Random.Range(0, possibleStats.Count)];
            this.enchantStat = possibleStats[Random.Range(0, possibleStats.Count)];
            this.adivinationStat = possibleStats[Random.Range(0, possibleStats.Count)];
        }
        else if(type == 2)
        {
            this.alchemyStat = -1;
            this.summonStat = -1;
            this.enchantStat = -1;
            this.adivinationStat = -1;
        }
        else if (type == 3)
        {
            this.alchemyStat = 7;
            this.summonStat = 7;
            this.enchantStat = 7;
            this.adivinationStat = 7;
        }

    }

    public int alchemyStat;
    public int summonStat;
    public int enchantStat;
    public int adivinationStat;
}
