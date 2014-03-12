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
    class IonGUIGenerator
    {
        private const int COLWIDTH_STATUS1 = 120;
        private const int COLWIDTH_STATUS2 = 80;
        private const int COLWIDTH_TOGGLEBUTTON = 70;

        private const int COLWIDTH_TEXTFIELD = 35;

        private const int COLWIDTH_RESOURCENAME = 120;
        private const int COLWIDTH_RATE = 60;
        private const int COLWIDTH_AVALIABLE = 90;

        public bool isActive;
        public bool detailsActive;

        public IonModuleGenerator generator;

        public Rect windowPos;
        public Rect windowPos_ResourceDetails;

        public GUIStyle windowStyle;

        public bool show_catalysts;
        public bool show_inputs;
        public bool show_outputs;

        public bool showdetails_catalysts;
        public bool showdetails_inputs;
        public bool showdetails_outputs;

        private string _strOutputLevel;
        public float strOutputLevel { set { _strOutputLevel = value.ToString(); } }
        private string _strOutputLevel_step;

        public bool limitFactorReset;


        /************************************************************************\
         * IonGUIGenerator class                                                *
         * Constructor                                                          *
        \************************************************************************/
        public IonGUIGenerator(IonModuleGenerator gen)
        {
            generator = gen;
			isActive = false;
            windowPos = new Rect(Screen.width / 2, Screen.height / 2, 300, 100);
            windowPos_ResourceDetails = new Rect(Screen.width / 2, Screen.height / 2, 500, 100);

            windowStyle = new GUIStyle(HighLogic.Skin.window);

            show_catalysts = false;
            show_inputs = false;
            show_outputs = false;
            detailsActive = false;
            showdetails_catalysts = false;
            showdetails_inputs = false;
            showdetails_outputs = false;

            limitFactorReset = true;

            _strOutputLevel = (generator.outputLevel * 100).ToString();
            _strOutputLevel_step = (generator.outputLevel_step * 100).ToString();
        }


        /************************************************************************\
         * IonGUIGenerator class                                                *
         * DrawGUI function                                                     *
         *                                                                      *
         * Draws the GUI window.                                                *
        \************************************************************************/
        public void DrawGUI(int WindowID)
        {
            GUILayout.BeginVertical();
            {
                DrawStatus();
                DrawResources();
            }
            GUILayout.EndVertical();

            if((generator.outputLevel * 100).ToString() != _strOutputLevel)
            {
                UpdateOutputLevel();
            }

            if((generator.outputLevel_step * 100).ToString() != _strOutputLevel_step)
            {
                UpdateOutputLevelStep();
            }

            GUI.DragWindow(new Rect(0, 0, 10000, 20));
        }


        /************************************************************************\
         * IonGUIGenerator class                                                *
         * DrawStatus function                                                  *
         *                                                                      *
         * Helper function for DrawGUI.  Draws information related to the       *
         * status of the generator to the GUI window.                           *
        \************************************************************************/
        private void DrawStatus()
        {

            GUILayout.Box(generator.generatorGUIName + " Status");

            GUILayout.BeginHorizontal();
            {
                GUILayout.Label("Generator Status: ", GUILayout.Width(COLWIDTH_STATUS1));
                GUILayout.Label(generator.isActive ? "Active" : "Inactive", GUILayout.Width(COLWIDTH_STATUS2));
                if (!generator.alwaysOn)
                {
                    if (GUILayout.Button(generator.isActive ? "Shutdown" : "Activate", GUILayout.Width(COLWIDTH_TOGGLEBUTTON)))
                    {
                        generator.SetGeneratorState(!generator.isActive);
                    }
                }
            }
            GUILayout.EndHorizontal();

            if (generator.outputLevel_min < generator.outputLevel_max)
            {
                GUILayout.BeginHorizontal();
                {
                    GUILayout.Label("Output Setting: ", GUILayout.Width(COLWIDTH_STATUS1));
                    _strOutputLevel = GUILayout.TextField(_strOutputLevel, GUILayout.Width(COLWIDTH_TEXTFIELD));
                    GUILayout.Label("%");
                }
                GUILayout.EndHorizontal();

                if (!generator.hide_OutputControls)
                {
                    GUILayout.BeginHorizontal();
                    {
                        GUILayout.Label("Output Increment: ", GUILayout.Width(COLWIDTH_STATUS1));
                        _strOutputLevel_step = GUILayout.TextField(_strOutputLevel_step, GUILayout.Width(COLWIDTH_TEXTFIELD));
                        GUILayout.Label("%");
                    }
                    GUILayout.EndHorizontal();
                }
            }

            if (generator.limitFactor < 1)
            {
                GUILayout.BeginHorizontal();
                {
                    GUILayout.Label("Limiting Resource: ", GUILayout.Width(COLWIDTH_STATUS1));
                    GUILayout.Label((null != generator.limitngResource ? generator.limitngResource.resourceName : "N/A"));
                }
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                {
                    GUILayout.Label(" Limited to: ", GUILayout.Width(COLWIDTH_STATUS1));
                    GUILayout.Label(generator.display_OutputActual.ToString("F2") + "% total output");
                }
                GUILayout.EndHorizontal();

                limitFactorReset = false;
            }
            else if (!limitFactorReset)
            {
                windowPos.height = 100;
                limitFactorReset = true;
            }


            GUILayout.BeginHorizontal();
            {
                GUILayout.Label("Efficiency: ", GUILayout.Width(COLWIDTH_STATUS1));
                GUILayout.Label(generator.display_Efficiency.ToString() + "%", GUILayout.Width(COLWIDTH_STATUS2));
            }
            GUILayout.EndHorizontal();
        }


        /************************************************************************\
         * IonGUIGenerator class                                                *
         * DrawResources function                                               *
         *                                                                      *
         * Helper function for DrawGUI.  Draws information related to the       *
         * genertor's resouces to the GUI window.                               *
        \************************************************************************/
        private void DrawResources()
        {
            int buttonWidth = COLWIDTH_RESOURCENAME + COLWIDTH_RATE + COLWIDTH_AVALIABLE;

            GUILayout.Box(generator.generatorGUIName + " Resources");

            if (generator.list_catalysts.Count > 0)
            {
                if (GUILayout.Button(show_catalysts ? "CATALYSTS [hide]" : "CATALYSTS [show]", GUILayout.Width(buttonWidth)))
                {
                    show_catalysts = !show_catalysts;
                    windowPos.height = 100;
                }
                if (show_catalysts)
                    DrawResourceList(generator.list_catalysts, true);
            }

            if (generator.list_inputs.Count > 0)
            {
                if (GUILayout.Button(show_inputs ? "INPUTS [hide]" : "INPUTS [show]", GUILayout.Width(buttonWidth)))
                {
                    show_inputs = !show_inputs;
                    windowPos.height = 100;
                }
                if (show_inputs)
                    DrawResourceList(generator.list_inputs);
            }

            if (generator.list_outputs.Count > 0)
            {
                if (GUILayout.Button(show_outputs ? "OUTPUTS [hide]" : "OUTPUTS [show]", GUILayout.Width(buttonWidth)))
                {
                    show_outputs = !show_outputs;
                    windowPos.height = 100;
                }
                if (show_outputs)
                    DrawResourceList(generator.list_outputs);
            }

            if(GUILayout.Button("Resource Details"))
            {
                detailsActive = !detailsActive;
            }
        }

        /************************************************************************\
         * IonGUIGenerator class                                                *
         * DrawResourceList function                                            *
         *                                                                      *
         * Helper function for DrawResources.  Draws information for a list of  *
         * resources to the GUI window.                                         *
         *                                                                      *
         * list_resources:  list of resources to draw info for.                 *
         * isCatalist:      if true the Rate column is replaced with Needed.    *
        \************************************************************************/
        private void DrawResourceList(List<IonResourceData_Generator> list_resources, bool isCatalist = false)
        {
            string avaliable, capacity;
            string rateRequest, rateReturned;

            if (list_resources.Count > 0)
            {
                GUILayout.BeginHorizontal();
                {
                    GUILayout.Label("Resource:", GUILayout.Width(COLWIDTH_RESOURCENAME));

                    if (!isCatalist)
                        GUILayout.Label("Rate:", GUILayout.Width(COLWIDTH_RATE));
                    else
                        GUILayout.Label("Needed:", GUILayout.Width(COLWIDTH_RATE));

                    GUILayout.Label("Avaliable:", GUILayout.Width(COLWIDTH_AVALIABLE));
                }
                GUILayout.EndHorizontal();

                foreach (IonResourceData_Generator resource in list_resources)
                {
                    avaliable = ParseHelper.FormatValue((float)resource.curAvailable, 2);
                    capacity = ParseHelper.FormatValue((float)(resource.curAvailable + resource.curFreeAmount), 0);
                    rateRequest = ParseHelper.FormatValue((float)resource.curRateRequested, 2);
                    rateReturned = ParseHelper.FormatValue((float)resource.curRateReturned, 2);

                    GUILayout.BeginHorizontal();
                    {
                        GUILayout.Label(resource.resourceName, GUILayout.Width(COLWIDTH_RESOURCENAME));
                        GUILayout.Label(rateReturned, GUILayout.Width(COLWIDTH_RATE));
                        GUILayout.Label(avaliable + "/" + capacity, GUILayout.Width(COLWIDTH_AVALIABLE));
                    }
                    GUILayout.EndHorizontal();
                }
            }
        }

        public void DrawResourceDetailsGUI(int WindowID)
        {
            GUILayout.BeginVertical();
            {
                DrawResourcesDetails();
            }
            GUILayout.EndVertical();

            GUI.DragWindow(new Rect(0, 0, 10000, 20));
        }

        private void DrawResourcesDetails()
        {
            int buttonWidth = COLWIDTH_RESOURCENAME + COLWIDTH_RATE + COLWIDTH_AVALIABLE;
            GUILayout.Box(generator.generatorGUIName + " Resources");

            if (generator.list_catalysts.Count > 0)
            {
                if (GUILayout.Button(showdetails_catalysts ? "CATALYSTS [hide]" : "CATALYSTS [show]", GUILayout.Width(buttonWidth)))
                {
                    showdetails_catalysts = !showdetails_catalysts;
                    windowPos_ResourceDetails.height = 100;
                }
                if (showdetails_catalysts)
                    DrawResourceDetailsList(generator.list_catalysts, true);
            }

            if (generator.list_inputs.Count > 0)
            {
                if (GUILayout.Button(showdetails_inputs ? "INPUTS [hide]" : "INPUTS [show]", GUILayout.Width(buttonWidth)))
                {
                    showdetails_inputs = !showdetails_inputs;
                    windowPos_ResourceDetails.height = 100;
                }
                if (showdetails_inputs)
                    DrawResourceDetailsList(generator.list_inputs);
            }

            if (generator.list_outputs.Count > 0)
            {
                if (GUILayout.Button(showdetails_outputs ? "OUTPUTS [hide]" : "OUTPUTS [show]", GUILayout.Width(buttonWidth)))
                {
                    showdetails_outputs = !showdetails_outputs;
                    windowPos_ResourceDetails.height = 100;
                }
                if (showdetails_outputs)
                    DrawResourceDetailsList(generator.list_outputs);
            }
        }
        private void DrawResourceDetailsList(List<IonResourceData_Generator> list_resources, bool isCatalist = false)
        {
            string avaliable, capacity;
            string rateRequest, rateReturned;

            if (list_resources.Count > 0)
            {
                GUILayout.BeginHorizontal();
                {
                    GUILayout.Label("Resource:", GUILayout.Width(COLWIDTH_RESOURCENAME));

                    if (!isCatalist)
                        GUILayout.Label("Rate:", GUILayout.Width(COLWIDTH_RATE));
                    else
                        GUILayout.Label("Needed:", GUILayout.Width(COLWIDTH_RATE));

                    GUILayout.Label("Avaliable:", GUILayout.Width(COLWIDTH_AVALIABLE));
                }
                GUILayout.EndHorizontal();

                foreach (IonResourceData_Generator resource in list_resources)
                {
                    avaliable = ParseHelper.FormatValue((float)resource.curAvailable, 2);
                    capacity = ParseHelper.FormatValue((float)(resource.curAvailable + resource.curFreeAmount), 0);
                    rateRequest = ParseHelper.FormatValue((float)resource.curRateRequested, 2);
                    rateReturned = ParseHelper.FormatValue((float)resource.curRateReturned, 2);

                    GUILayout.BeginHorizontal();
                    {
                        GUILayout.Label(resource.resourceName, GUILayout.Width(COLWIDTH_RESOURCENAME));
                        GUILayout.Label(rateReturned, GUILayout.Width(COLWIDTH_RATE));
                        GUILayout.Label(avaliable + "/" + capacity, GUILayout.Width(COLWIDTH_AVALIABLE));
                    }
                    GUILayout.EndHorizontal();
                }
            }
        }


        /************************************************************************\
         * IonGUIGenerator class                                                *
         * UpdateOutputLevel function                                           *
         *                                                                      *
         * Updates outputLevel from changes input on _strOutputLevel.           *
        \************************************************************************/
        private void UpdateOutputLevel()
        {
            float outputLevel;
            if (float.TryParse(_strOutputLevel, out outputLevel))
            {
                outputLevel /= 100.0f;

                if (outputLevel > generator.outputLevel_max)
                    outputLevel = generator.outputLevel_max;
                else if (outputLevel < generator.outputLevel_min)
                    outputLevel = generator.outputLevel_min;

                generator.outputLevel = outputLevel;
                generator.display_OutputSetting = outputLevel * 100.0f;
            }
            else
            {
                _strOutputLevel = (generator.outputLevel * 100).ToString();
            }
        }

        /************************************************************************\
         * IonGUIGenerator class                                                *
         * UpdateOutputLevelStep function                                       *
         *                                                                      *
         * Updates outputLevel_step from changes input on _strOutputLevel_step. *
        \************************************************************************/
        private void UpdateOutputLevelStep()
        {
            if (float.TryParse(_strOutputLevel_step, out generator.outputLevel_step))
            {
                generator.outputLevel_step /= 100;
            }
            else
            {
                _strOutputLevel_step = (generator.outputLevel_step * 100).ToString();
            }
        }

    }
}
