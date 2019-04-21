using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExperienceHelper
{

    public static int STARTING_EXP_CHARACTER = 300;
    public static float FACTOR_EXP_CHARACTER = 3.5f;


    public static int GiveMeTheNextExperienceNeededToReachTheNextLevel(int nextLevel)
    {
        int res = 0;
        if (nextLevel <= 20)
            res = (STARTING_EXP_CHARACTER * nextLevel) + (int)System.Math.Pow(nextLevel, FACTOR_EXP_CHARACTER);
        else
            res = (int)System.Math.Pow(nextLevel, FACTOR_EXP_CHARACTER);

        //print("[" + nextLevel + "] " + res);
        bool dontStop = true;
        int var = 10;
        double tempRes = 0;
        while (dontStop)
        {
            //if var has the same amount of 0 than res
            //print("var : " + var);
            if (res / var < 10)
            {
                //print("Has enought 0");
                var /= 10;
                //print("var2 : " + var);
                tempRes = (double)res / (double)var;
                //print("tempRes : " + tempRes);
                tempRes = System.Math.Floor(tempRes);
                //print("tempResFloor : " + tempRes);
                tempRes *= var;
                res = (int)tempRes;
                //print("res : " + res);
                dontStop = false;
            }
            var *= 10;
        }
        return res;
    }
}
