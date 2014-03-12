//#define DEBUG
//#define DEBUG_UPDATES

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;
using KSP;

namespace IoncrossKerbal
{
    public class IonModuleDisplay : PartModule
    {
        public string name;

        [KSPField(guiActive = true, guiName = "ResourceName Rate", guiUnits = "/h", guiFormat = "F3", isPersistant = false)]
        public float displayRate;

        public bool isRate;

        //Private members for storing the rate of usage
        private Queue<double> queue_rates;  //queue of rates from past update cycles, used to average out varriations
        public int ratesSize;               //number of rates to store in queue_rates
        public double curRate;              //current rate for this update cycle
        private double sumRates;            //current sum of all rates in queue_rates


        /************************************************************************\
         * IonModuleDisplay class                                               *
         * Constructor                                                          *
         *                                                                      *
        \************************************************************************/
        public IonModuleDisplay()
        {
            isRate = true;
            queue_rates = new Queue<double>();
            ratesSize = 20;
            curRate = 0;
            sumRates = 0;
        }

        /************************************************************************\
         * IonModuleDisplay class                                               *
         * OnLoad function                                                      *
         *                                                                      *
        \************************************************************************/
        public override void OnLoad(ConfigNode node)
        {
            base.OnLoad(node);
#if DEBUG
            Debug.Log("IonModuleDisplay.OnLoad() " + this.part.name);
            Debug.Log("IonModuleDisplay.OnLoad(): node\n" + node.ToString());
#endif
            if (node.HasValue("Name"))
                name = node.GetValue("Name");

            if (node.HasValue("IsRate"))
                isRate = "True" == node.GetValue("IsRate") || "true" == node.GetValue("IsRate") || "TRUE" == node.GetValue("IsRate");
            else
                isRate = true;

            if (node.HasValue("RatesSize"))
                ratesSize = Convert.ToInt32(node.GetValue("RatesSize"));
            else
                ratesSize = 20;
        }

        /************************************************************************\
         * IonModuleDisplay class                                               *
         * OnSave function                                                      *
         *                                                                      *
        \************************************************************************/
        public override void OnSave(ConfigNode node)
        {
            base.OnSave(node);
#if DEBUG
            Debug.Log("IonModuleDisplay.OnSave() " + this.part.name);
#endif
            node.AddValue("Name", name);
            node.AddValue("IsRate", isRate);
            node.AddValue("RatesSize", ratesSize);
#if DEBUG
            Debug.Log("IonModuleDisplay.OnSave(): node\n" + node.ToString());
#endif
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
            sumRates = 0;

            if (isRate)
                Fields["displayRate"].guiName = name + " Rate";
            else
                Fields["displayRate"].guiName = name;
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
                    queue_rates.Enqueue(curRate / TimeWarp.deltaTime);
                    sumRates += curRate / TimeWarp.deltaTime;
                }

                //while the queue has enough elements in it
                //dequeue the last item and subtract it from the sum
                while (queue_rates.Count > ratesSize)
                {
                    sumRates -= queue_rates.Dequeue();
                }

                //set the display value by averaging all rates in queue_rates
                displayRate = (float)(sumRates / queue_rates.Count);

                //adjust the units so the value is above 0.5
                string units = "";
                setRateUnits(ref displayRate, ref units, 0.5f);
                Fields["displayRate"].guiUnits = "/" + units;

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
        public static void setRateUnits(ref float rate, ref string units, float minValue)
        {
            units = "sec.";
            float rateABS = Math.Abs(rate);

            //adjust rate and units so the value stays above minValue
            if (rateABS < minValue)
            {
                rate *= 60.0f;
                units = "min.";

                if (rateABS < minValue)
                {
                    rate *= 60.0f;
                    units = "hour";

                    if (rateABS < minValue)
                    {
                        rate *= 24.0f;
                        units = "day";

                        if (rateABS < minValue)
                        {
                            rate *= 7.0f;
                            units = "week";

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
        public static IonModuleDisplay findDisplayModule(Part part, IonResourceData resource)
        {
#if DEBUG
            Debug.Log("IonModuleDisplay.findDisplayModule( " + part.name + ", " + resource.Name + " )");
#endif
            IonModuleDisplay displayModule = null;

            //if there is a list of module
            //traverse through them untill finding one that is a IonModuleDisplay type and it matches the resouce
            if (null != part.Modules)
            {
                foreach (PartModule module in part.Modules)
                {
                    if (module is IonModuleDisplay && ((IonModuleDisplay)module).name == resource.Name)
                    {
#if DEBUG
                        Debug.Log("IonModuleDisplay.findDisplayModule(): " + resource.Name + " display module found");
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
        public static IonModuleDisplay findAndCreateDisplayModule(Part part, IonResourceData resource)
        {
#if DEBUG
            Debug.Log("IonModuleDisplay.findAndCreateDisplayModule( " + part.name + ", " + resource.Name + " )");
#endif
            //look for the display module
            IonModuleDisplay displayModule = findDisplayModule(part, resource);

            //if no display module was found
            //create a new one
            if (null == displayModule)
            {
#if DEBUG
                Debug.Log("IonModuleDisplay.findAndCreateDisplayModule(): " + resource.Name + " module not found, creating new");
#endif
                try { displayModule = (IonModuleDisplay)part.AddModule("IonModuleDisplay"); }
                catch (NullReferenceException)
                {
#if DEBUG
                    Debug.Log("IonModuleDisplay.findAndCreateDisplayModule(): NULL REFERENCE EXCEPTION CAUGHT! part.Modules was probablly null");
#endif
                    return null;
                }

                displayModule.name = resource.Name;
#if DEBUG
                Debug.Log("IonModuleDisplay.findAndCreateDisplayModule(): " + displayModule.name + " module added");
#endif
            }

            return displayModule;
        }

    }//end of class
    //==========================================================================================================
    // END of IonModuleDisplay Class
    //==========================================================================================================

}//end of namespace
