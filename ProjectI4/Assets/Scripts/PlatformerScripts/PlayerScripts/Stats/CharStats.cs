using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

[Serializable]
public class CharStats
{
    public float baseVal;

    public float Value
    {
        get
        {
            if (isDirty || baseVal != lastBaseVal)
            {
                lastBaseVal = baseVal;
                _val = CalcFinalVal();
                isDirty = false;
            }
            return _val;
        }
    }

    private bool isDirty = true;
    private float _val;
    private float lastBaseVal = float.MinValue;

    private readonly List<StatModifier> statMod;
    public readonly ReadOnlyCollection<StatModifier> StatModifiers; // Ref to statMod. Is constantly updated, cannot be changed.

    public CharStats()
    {
        statMod = new List<StatModifier>();
        StatModifiers = statMod.AsReadOnly();
    }

    public CharStats(float val) : this()
    {
        baseVal = val;
    }

    public void AddMod(StatModifier mod)
    {
        isDirty = true;
        statMod.Add(mod);
        statMod.Sort(CompareModOrder);
    }

    public bool RemoveMod(StatModifier mod)
    {
        if (statMod.Remove(mod))
        {
            isDirty = true;
            return true;
        }
        return false;
    }

    private int CompareModOrder(StatModifier x, StatModifier y)
    {
        if (x.order < y.order)
        {
            return -1;
        }
        else if (y.order < x.order)
        {
            return 1;
        }
        return 0;
        
    }

    public bool RemoveAllMods(object _source)
    {
        bool removed = false;

        for (int i = statMod.Count; i >= 0; --i)
        {
            if (statMod[i].source == _source)
            {
                isDirty = true;
                removed = true;
                statMod.RemoveAt(i);
            }
        }

        return removed;
    }

    private float CalcFinalVal()
    {
        float finalVal = baseVal;
        float sumPercentAdd = 0;

        for (int i = 0; i < statMod.Count; ++i)
        {
            StatModifier mod = statMod[i];

            if (mod.type == StatModType.Flat)
            {
                finalVal += mod.val;
            }
            else if (mod.type == StatModType.PercentAdd)
            {
                sumPercentAdd += mod.val;

                if (i + 1 >= statMod.Count|| statMod[i + 1].type != StatModType.PercentAdd)
                {
                    finalVal *= 1 + sumPercentAdd;
                    sumPercentAdd = 0;
                }
            }
            else if (mod.type == StatModType.PercentMult)   // Else incase we have other types.
            {
                finalVal *= 1 + mod.val;
            }
        }

        return (float)Math.Round(finalVal, 4);
    }

}
