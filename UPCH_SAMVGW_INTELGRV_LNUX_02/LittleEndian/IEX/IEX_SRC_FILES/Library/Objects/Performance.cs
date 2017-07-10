using System;


   public class Performance
    {
        private double minimumTime;

       public Double MinimumTime
       {
           get {return minimumTime;}
           set { minimumTime = value;}
       }

       private double maximumTime;
       
       public Double MaximumTime
       {
           get{return maximumTime;}
           set{maximumTime = value;}
       }

       private double averageTime;

       public Double AverageTime
       {
           get {return averageTime;}
           set {averageTime = value;}
       }

       private double timeforfirstlaunchTime;

       public Double TimeForFirstLaunchTime
       {
           get {return timeforfirstlaunchTime;}
           set{timeforfirstlaunchTime = value;}
       }
       
       private double failedthresholdvalueTime;

       public Double FailedThresholdValueTime
       {
           get {return failedthresholdvalueTime;}
           set {failedthresholdvalueTime = value;}
       }

       private int runningiterationnumber;

       public Int32 RunningNumberOfIteration
       {
           get {return runningiterationnumber;}
           set {runningiterationnumber = value;}
       }
	   
	          private double thresholdvalue;

       public double Thresholdvalue
       {
           get { return thresholdvalue; }
           set { thresholdvalue = value; }
       }

    }

