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
    /*------------------------------------------------------*\
     * IonGeneratorData Class                               *
     * This classes stores the information related to       *
     * default generators placed in all parts with an       *
     * IonCrewSupportModule attached.                       *
    \*------------------------------------------------------*/
    [Serializable]
    public class IonGeneratorData : IConfigNode
    {
        public string moduleClass;
        public string generatorName;
        public string generatorGUIName;

        public bool startOn;
        public bool alwaysOn;

        public float outputLevelStep;
        public float outputLevelMin;
        public float outputLevelMax;

        public bool hideStatus;
        public bool hideStatusL2;
        public bool hideEfficency;
        public bool hideOutputControls;
        public bool hideActivateControls;

        public List<IonResourceData_Generator> listInputs;
        public List<IonResourceData_Generator> listOutputs;

        /************************************************************************\
         * IonGeneratorData class                                               *
         * Load function                                                        *
        \************************************************************************/
        public virtual void Load(ConfigNode node)
        {
#if DEBUG
            Debug.Log("IonGeneratorData.Load()");
            Debug.Log("IonGeneratorData.Load(): node\n" + node.ToString());
#endif
            if (node.HasValue("moduleClass"))
                moduleClass = node.GetValue("moduleClass");

            //Read variables from node
            if (node.HasValue("generatorName"))
                generatorName = node.GetValue("generatorName");
            else if (node.HasValue("name"))
                generatorName = node.GetValue("name");

            if (node.HasValue("generatorGUIName"))
                generatorGUIName = node.GetValue("generatorGUIName");

            if (node.HasValue("startOn"))
                startOn = "True" == node.GetValue("startOn") || "true" == node.GetValue("startOn") || "TRUE" == node.GetValue("startOn");

            if (node.HasValue("alwaysOn"))
                alwaysOn = "True" == node.GetValue("alwaysOn") || "true" == node.GetValue("alwaysOn") || "TRUE" == node.GetValue("alwaysOn");

            //Read output level variables
            if (node.HasValue("outputLevelStep"))
            {
                outputLevelStep = Convert.ToSingle(node.GetValue("outputLevelStep"));
                outputLevelStep = Math.Min(outputLevelStep, 1.0f);
                outputLevelStep = Math.Max(outputLevelStep, 0.0f);
            }

            if (node.HasValue("outputLevelMin"))
            {
                outputLevelMin = Convert.ToSingle(node.GetValue("outputLevelMin"));
                outputLevelMin = Math.Min(outputLevelMin, 1.0f);
                outputLevelMin = Math.Max(outputLevelMin, 0.0f);
            }

            if (node.HasValue("outputLevelMax"))
            {
                outputLevelMax = Convert.ToSingle(node.GetValue("outputLevelMax"));
                outputLevelMax = Math.Min(outputLevelMax, 1.0f);
                outputLevelMax = Math.Max(outputLevelMax, 0.0f);
            }

            //Read hide variables
            if (node.HasValue("hideStatus"))
                hideStatus = "True" == node.GetValue("hideStatus") || "true" == node.GetValue("hideStatus") || "TRUE" == node.GetValue("hideStatus");
            if (node.HasValue("hideStatusL2"))
                hideStatusL2 = "True" == node.GetValue("hideStatusL2") || "true" == node.GetValue("hideStatusL2") || "TRUE" == node.GetValue("hideStatusL2");
            if (node.HasValue("hideEfficency"))
                hideEfficency = "True" == node.GetValue("hideEfficency") || "true" == node.GetValue("hideEfficency") || "TRUE" == node.GetValue("hideEfficency");
            if (node.HasValue("hideOutputControls"))
                hideOutputControls = "True" == node.GetValue("hideOutputControls") || "true" == node.GetValue("hideOutputControls") || "TRUE" == node.GetValue("hideOutputControls");
            if (node.HasValue("hideActivateControls"))
                hideActivateControls = "True" == node.GetValue("hideActivateControls") || "true" == node.GetValue("hideActivateControls") || "TRUE" == node.GetValue("hideActivateControls");


            //Create lists
            listInputs = new List<IonResourceData_Generator>();
            listOutputs = new List<IonResourceData_Generator>();

            //Traverse through subNodes and add to lists
            foreach (ConfigNode subNode in node.nodes)
            {
#if DEBUG
                Debug.Log("IonGeneratorData.Load(): subnode " + subNode.ToString());
#endif
                if ("INPUT_RESOURCE" == subNode.name)
                {
                    IonResourceData_Generator inputResource = new IonResourceData_Generator(subNode);
                    listInputs.Add(inputResource);
                }
                else if ("OUTPUT_RESOURCE" == subNode.name)
                {
                    IonResourceData_Generator outputResource = new IonResourceData_Generator(subNode);
                    listOutputs.Add(outputResource);
                }
            }
        }

        /************************************************************************\
         * IonGeneratorData class                                               *
         * Save function                                                        *
        \************************************************************************/
        public virtual void Save(ConfigNode node)
        {
#if DEBUG
            Debug.Log("IonGeneratorData.Save()");
#endif
            node.AddValue("moduleClass", moduleClass);
            node.AddValue("generatorName", generatorName);
            node.AddValue("generatorGUIName", generatorGUIName);

            node.AddValue("startOn", startOn);
            node.AddValue("alwaysOn", alwaysOn);

            node.AddValue("outputLevelStep", outputLevelStep);
            node.AddValue("outputLevelMin", outputLevelMin);
            node.AddValue("outputLevelMax", outputLevelMax);

            node.AddValue("hideStatus", hideStatus);
            node.AddValue("hideStatusL2", hideStatusL2);
            node.AddValue("hideEfficency", hideEfficency);
            node.AddValue("hideOutputControls", hideOutputControls);
            node.AddValue("hideActivateControls", hideActivateControls);

            if (null != listInputs)
            {
                foreach (IonResourceData_Generator inputResource in listInputs)
                {
                    ConfigNode inputNode = new ConfigNode("INPUT_RESOURCE");
                    inputResource.Save(inputNode);
                }
            }

            if (null != listOutputs)
            {
                foreach (IonResourceData_Generator outputResource in listOutputs)
                {
                    ConfigNode outputNode = new ConfigNode("OUTPUT_RESOURCE");
                    outputResource.Save(outputNode);
                }
            }
#if DEBUG
            Debug.Log("IonGeneratorData.Save(): node\n" + node.ToString());
#endif
        }

    }//end of class
    //==========================================================================================================
    // END of IonGeneratorData Class
    //==========================================================================================================

    /*------------------------------------------------------*\
     * IonCollectorData Class                                *
     * This classes stores the information related to       *
     * default collectors placed in all parts with an       *
     * IonCrewSupportModule attached.                       *
    \*------------------------------------------------------*/
    [Serializable]
    public class IonCollectorData : IonGeneratorData
    {
        public float minAtmosphere;

        public bool isAutomaticOxygen;
        public bool isAutomaticNoOxygen;

        public bool hideAtmoContents;

        public List<IonResourceData_Generator> listOutputs_oxygen;
        public List<IonResourceData_Generator> listOutputs_noOxygen;

        /************************************************************************\
         * IonCollectorData class                                               *
         * Load function                                                        *
        \************************************************************************/
        public override void Load(ConfigNode node)
        {
            base.Load(node);
#if DEBUG
            Debug.Log("IonSupportCollector.Load()");
            Debug.Log("IonSupportCollector.Load(): node\n" + node.ToString());
#endif
            //Read variables from node
            if (node.HasValue("minAtmosphere"))
                minAtmosphere = Convert.ToSingle(node.GetValue("minAtmosphere"));

            if (node.HasValue("isAutomaticOxygen"))
                isAutomaticOxygen = "True" == node.GetValue("isAutomaticOxygen") || "true" == node.GetValue("isAutomaticOxygen") || "TRUE" == node.GetValue("isAutomaticOxygen");

            if (node.HasValue("isAutomaticNoOxygen"))
                isAutomaticNoOxygen = "True" == node.GetValue("isAutomaticNoOxygen") || "true" == node.GetValue("isAutomaticNoOxygen") || "TRUE" == node.GetValue("isAutomaticNoOxygen");

            if (node.HasValue("hideAtmoContents"))
                hideAtmoContents = "True" == node.GetValue("hideAtmoContents") || "true" == node.GetValue("hideAtmoContents") || "TRUE" == node.GetValue("hideAtmoContents");


            //Create lists
            listOutputs_oxygen = new List<IonResourceData_Generator>();
            listOutputs_noOxygen = new List<IonResourceData_Generator>();

            //Traverse through subNodes and add to lists
            foreach (ConfigNode subNode in node.nodes)
            {
#if DEBUG
                Debug.Log("IonSupportCollector.Load(): subnode " + subNode.ToString());
#endif
                if ("OUTPUT_RESOURCE_OXYGEN" == subNode.name)
                {
                    IonResourceData_Generator outputResource = new IonResourceData_Generator(subNode);
                    listOutputs_oxygen.Add(outputResource);
                }
                else if ("OUTPUT_RESOURCE_NO_OXYGEN" == subNode.name)
                {
                    IonResourceData_Generator outputResource = new IonResourceData_Generator(subNode);
                    listOutputs_noOxygen.Add(outputResource);
                }
            }
        }

        /************************************************************************\
         * IonCollectorData class                                               *
         * Save function                                                        *
        \************************************************************************/
        public override void Save(ConfigNode node)
        {
            base.Save(node);
#if DEBUG
            Debug.Log("IonSupportCollector.Save()");
#endif
            node.AddValue("minAtmosphere", minAtmosphere);
            node.AddValue("isAutomaticOxygen", isAutomaticOxygen);
            node.AddValue("isAutomaticNoOxygen", isAutomaticNoOxygen);
            node.AddValue("hideAtmoContents", hideAtmoContents);

            if (null != listOutputs_oxygen)
            {
                foreach (IonResourceData_Generator outputResource in listOutputs_oxygen)
                {
                    ConfigNode outputNode = new ConfigNode("OUTPUT_RESOURCE_OXYGEN");
                    outputResource.Save(outputNode);
                }
            }

            if (null != listOutputs_noOxygen)
            {
                foreach (IonResourceData_Generator outputResource in listOutputs_noOxygen)
                {
                    ConfigNode outputNode = new ConfigNode("OUTPUT_RESOURCE_NO_OXYGEN");
                    outputResource.Save(outputNode);
                }
            }
#if DEBUG
            Debug.Log("IonSupportCollector.Save(): node\n" + node.ToString());
#endif
        }

    }//end of class
    //==========================================================================================================
    // END of IonCollectorData Class
    //==========================================================================================================        
}
