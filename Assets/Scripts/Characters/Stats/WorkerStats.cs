using System.Collections.Generic;
using UnityEngine;

public class WorkerStats {

    private List<int> possibleStats = new() { 5, 4, 4, 3, 3, 3, 2, 2, 2, 2, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0 };
    public WorkerStats(bool ador)
    {
        if(ador)
        {
            this.alchemyStat = 3;
            this.summonStat = 3;
            this.enchantStat = 3;
            this.adivinationStat = 3;

        }
        else
        {
            this.alchemyStat = possibleStats[Random.Range(0, possibleStats.Count)];
            this.summonStat = possibleStats[Random.Range(0, possibleStats.Count)];
            this.enchantStat = possibleStats[Random.Range(0, possibleStats.Count)];
            this.adivinationStat = possibleStats[Random.Range(0, possibleStats.Count)];
        }
    }

    public int alchemyStat;
    public int summonStat;
    public int enchantStat;
    public int adivinationStat;
}
