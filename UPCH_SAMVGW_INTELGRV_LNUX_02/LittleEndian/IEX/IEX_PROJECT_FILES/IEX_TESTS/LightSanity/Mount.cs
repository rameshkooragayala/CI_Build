using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using IEX.Tests.Engine;
using IEX.ElementaryActions.Functionality;
using IEX.Tests.Utils;

//Basic Mount Test
public class Mount : _Test
{
    [ThreadStatic]
    static _Platform CL;



    #region Create Structure
    public override void CreateStructure()
    {

        //Brief Description: Mount Gateway and Client        
        this.AddStep(new Step1(), "Step 1: Mount Gateway and Client");

        //Get Client Platform
        CL = GetClient();
    }
    #endregion

    #region Steps
    #region Step1
    private class Step1 : _Step
    {
        public override void Execute()
        {
            PassStep();

        }
    }
    #endregion
    #endregion

    #region PostExecute
    public override void PostExecute()
    {

    }
    #endregion
}