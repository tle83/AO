using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Service : MonoBehaviour {
    public int crestDmg;
    public enum Tier {
        BLUE,
        PURPLE,
        ORANGE,
        RED,
        WHITE
    }

    public enum ClassType {
        ARCHER,
        LANCER,
        FIGHTER,
        SWORDSMAN,
        MAGE,
        TRICKSTER
    }

    public int getDamage (int baseDmg, int multiplier, int level) {
        return (baseDmg * level) * multiplier;
    }

    public int getHealth (int baseHP, int multiplier, int level) {
        return (baseHP * level) * multiplier;
    }

    public int getPower (int basePwr, int multiplier, int level) {
        return (basePwr * level) * multiplier;
    }

    public int getCrestDamage (int baseCrest, int multiplier, int level) {
        if (level <= 20) {
            this.crestDmg = baseCrest * multiplier * 1;
        } else if (level > 20 && level <= 40) {
            this.crestDmg = baseCrest * multiplier * 2;
        } else if (level > 40 && level <= 60) {
            this.crestDmg = baseCrest * multiplier * 3;
        } else if (level > 60 && level <= 80) {
            this.crestDmg = baseCrest * multiplier * 4;
        } else if (level > 80 && level < 100) {
            this.crestDmg = baseCrest * multiplier * 5;
        } else if (level == 100) {
            this.crestDmg = baseCrest * multiplier * 6;
        }

        return this.crestDmg;

    }

}