//#define DEBUG
//#define DEBUG_UPDATES

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;
using KSP;

namespace IoncrossKerbal_Generator
{
    public class IonModuleDisplay : PartModule
    {
        public const string RESOURCE_NAME = "name";
        public const string IS_RATE = "isRate";
        public const string RATE_SIZE = "rateSize";

        public string resourceName;

        [KSPField(guiActive = true, guiName = "ResourceName Rate", guiUnits = "/h", guiFormat = "F3", isPersistant = false)]
        public float displayRate;

        public bool isRate;

        //Private members for storing the rate of usage
        private Queue<double> _queue_rates; //queue of rates from past update cycles, used to average out varriations
        public int ratesSize;               //number of rates to store in queue_rates
        public double curRate;              //current rate for this update cycle
        private double _sumRates;           //current sum of all rates in queue_rates


        /************************************************************************\
         * IonModuleDisplay class                                               *
         * Constructor                                                          *
         *                                                                      *
        \************************************************************************/
        public IonModuleDisplay()
        {
            isRate = true;
            _queue_rates = new Queue<double>();
            ratesSize = 20;
            curRate = 0;
            _sumRates = 0;
        }

        /************************************************************************\
         * IonModuleDisplay class                                               *
         * OnStart function                                                     *
         *                                                                      *
        \************************************************************************/
        public override void OnStart(PartModule.StartState state)
        {
            base.OnStart(state);
#if DEBUG
            Debug.Log("IonModuleDisplay.OnStart() " + this.part.name);
            Debug.Log("IonModuleDisplay.OnStart(): state " + state.ToString());
#endif
            curRate = 0;
            _sumRates = 0;

            if (isRate)
                Fields["displayRate"].guiName = resourceName + " Rate";
            else
                Fields["displayRate"].guiName = resourceName;
        }

        /************************************************************************\
         * IonModuleDisplay class                                               *
         * OnUpdate function                                                    *
         *                                                                      *
        \************************************************************************/
        public override void OnUpdate()
        {
            base.OnUpdate();
#if DEBUG_UPDATES
            Debug.Log("IonModuleDisplay.OnUpdate() " + this.part.name);
#endif
            if (isRate)
            {
                //if time has passed
                //enqueue the current rate and add it to the sum
                if (TimeWarp.deltaTime != 0)
                {
                    _queue_rates.Enqueue(curRate / TimeWarp.deltaTime);
                    _sumRates += curRate / TimeWarp.deltaTime;
                }

                //while the queue has enough elements in it
                //dequeue the last item and subtract it from the sum
                while (_queue_rates.Count > ratesSize)
                {
                    _sumRates -= _queue_rates.Dequeue();
                }

                //set the display value by averaging all rates in queue_rates
                displayRate = (float)(_sumRates / _queue_rates.Count);

                //adjust the units so the value is above 0.5
                setRateUnits(0.5f);

                //Reset curRate for next update
                curRate = 0.0f;
            }
        }

        /************************************************************************\
         * IonModuleDisplay class                                               *
         * setRateUnits functions                                               *
         *                                                                      *
         * Adjusts the units of rate so the value will be greater than minValue.*
        \************************************************************************/
        private void setRateUnits(float minValue)
        {
            float rateABS = Math.Abs(displayRate);
            Fields["displayRate"].guiUnits = "/sec.";

            //adjust rate and units so the value stays above minValue
            if (rateABS * 86400 < 0.000001f)
            {
                Fields["displayRate"].guiUnits = "";
            }
            else if (rateABS < minValue)
            {
                displayRate *= 60.0f;
                Fields["displayRate"].guiUnits = "/min.";

                if (rateABS * 60.0f < minValue)
                {
                    displayRate *= 60.0f;
                    Fields["displayRate"].guiUnits = "/hour";

                    if (rateABS * 3600.0f < minValue)
                    {
                        displayRate *= 24.0f;
                        Fields["displayRate"].guiUnits = "/day";

                        if (rateABS * 86400 < minValue)
                        {
                            displayRate *= 7.0f;
                            Fields["displayRate"].guiUnits = "/week";

                        }//end of week
                    }//end of day
                }//end of hour
            }//end of min.

        }


        /************************************************************************\
         * IonModuleDisplay class                                               *
         * findDisplayModule functions                                          *
         *                                                                      *
         * Finds an IonModuleDisplay for resourceName on part and returns it.   *
         * If one does not exisit it return null.                               *
        \************************************************************************/
        public static IonModuleDisplay FindDisplayModule(Part part, string resourceName)
        {
#if DEBUG
            Debug.Log("IonModuleDisplay.findDisplayModule( " + part.name + ", " + resourceName + " )");
#endif
            IonModuleDisplay displayModule = null;

            //if there is a list of module
            //traverse through them untill finding one that is a IonModuleDisplay type and it matches the resouce
            if (null != part.Modules)
            {
                foreach (PartModule module in part.Modules)
                {
                    if (module is IonModuleDisplay && ((IonModuleDisplay)module).resourceName == resourceName)
                    {
#if DEBUG
                        Debug.Log("IonModuleDisplay.findDisplayModule(): " + resourceName + " display module found");
#endif
                        displayModule = (IonModuleDisplay)module;
                        break;
                    }
                }
            }

            return displayModule;
        }

        /************************************************************************\
         * IonModuleDisplay class                                               *
         * findAndCreateGeneratorModule functions                               *
         *                                                                      *
         * Finds an IonModuleDisplay for resourceName on part and returns it.   *
         * If one does not exisit it creates one and returns it.                *
        \************************************************************************/
        public static IonModuleDisplay FindCreateDisplayModule(Part part, string resourceName)
        {
#if DEBUG
            Debug.Log("IonModuleDisplay.findAndCreateDisplayModule( " + part.name + ", " + resourceName + " )");
#endif
            //look for the display module
            IonModuleDisplay displayModule = FindDisplayModule(part, resourceName);

            //if no display module was found
            //create a new one
            if (null == displayModule)
            {
#if DEBUG
                Debug.Log("IonModuleDisplay.findAndCreateDisplayModule(): " + resourceName + " module not found, creating new");
#endif
                try { displayModule = (IonModuleDisplay)part.AddModule("IonModuleDisplay"); }
                catch (NullReferenceException)
                {
#if DEBUG
                    Debug.Log("IonModuleDisplay.findAndCreateDisplayModule(): NULL REFERENCE EXCEPTION CAUGHT! part.Modules was probablly null");
#endif
                    return null;
                }

                displayModule.resourceName = resourceName;
#if DEBUG
                Debug.Log("IonModuleDisplay.findAndCreateDisplayModule(): " + displayModule.resourceName + " module added");
#endif
            }

            return displayModule;
        }

    }//end of class
    //==========================================================================================================
    // END of IonModuleDisplay Class
    //==========================================================================================================

}//end of namespace
