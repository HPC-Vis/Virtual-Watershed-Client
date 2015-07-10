/*
 * Copyright (c) 2014, Roger Lew (rogerlew.gmail.com)
 * Date: 2/5/2015
 * License: BSD (3-clause license)
 * 
 * The project described was supported by NSF award number IIA-1301792
 * from the NSF Idaho EPSCoR Program and by the National Science Foundation.
 * 
 */
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

namespace VTL.SimTimeControls
{
    [System.Serializable]

    public class PlaySpeedSliderControl : MonoBehaviour
    {
        public TimeSlider timeSlider;
        public GameObject scaleSlider;
        public enum timeScales { HOURS, DAYS, WEEKS, MONTHS }
        public timeScales currentScale;
        private Slider speedControlSlider;
        private Text speedText;
        int HOUR_STEPS = 24;
        int DAY_STEPS = 7, DAY_MULTIPLIER = 24;
        int WEEK_STEPS = 4, WEEK_MULTIPLIER = 168;
        int MONTH_STEPS = 12, MONTH_MULTIPLIER = 730;

        // Use this for initialization
        void Start()
        {
            speedControlSlider = gameObject.GetComponent<Slider>();

            foreach (Transform child in transform)
            {
                if (child.name == "Speed Text")
                    speedText = child.gameObject.GetComponent<Text>();
            }
            setSpeedText();


            //Invoke("OnValueChange", 1f / 60f);

            //float spd = timeSlider.PlaySpeed;
            //if (spd >= 1)
            //    speedText.text = string.Format("1s:{0:0.000}h", spd);
            //else
            //    speedText.text = string.Format("1s:{0:0.00}m", spd * 60f);
        }

        // Update is called once per frame
        void Update()
        {
        }

        
        public void OnValueChange()
        {
            int newSpeed = (int)speedControlSlider.value;

            if (currentScale == timeScales.HOURS)
            {
                timeSlider.PlaySpeed = (newSpeed + 1);
            }
            else if (currentScale == timeScales.DAYS)
            {
                timeSlider.PlaySpeed = DAY_MULTIPLIER * (newSpeed + 1);
            }
            else if (currentScale == timeScales.WEEKS)
            {
                timeSlider.PlaySpeed = WEEK_MULTIPLIER * (newSpeed + 1);
            }
            else if (currentScale == timeScales.MONTHS)
            {
                //not an accurate month speed
                timeSlider.PlaySpeed = MONTH_MULTIPLIER * (newSpeed + 1);
            }

            //if (speedSelectionInterval == SpeedSelectionInterval.Exponential)
            //{
            //    speedControlSlider.wholeNumbers = true;
            //    timeSlider.PlaySpeed = ExponentialSpeedSecToHourBaseline * Mathf.Pow(2f, value) / 4f;
            //}
            //else
            //{
            //    speedControlSlider.wholeNumbers = false;
            //    timeSlider.PlaySpeed = Mathf.Lerp(LinearSpeedMin, LinearSpeedMax, value / 8f);
            //}
            setSpeedText();
        }

        public void updateScale()
        {
            int val = (int)scaleSlider.GetComponent<Slider>().value;

            speedControlSlider.value = 0;
            switch (val)
            {
                case 0: 
                    currentScale = timeScales.HOURS;
                    setNewSliderMax(HOUR_STEPS-1);
                    break;
                case 1:
                    currentScale = timeScales.DAYS;
                    setNewSliderMax(DAY_STEPS-1);
                    break;
                case 2:
                    currentScale = timeScales.WEEKS;
                    setNewSliderMax(WEEK_STEPS-1);
                    break;
                case 3:
                    currentScale = timeScales.MONTHS;
                    setNewSliderMax(MONTH_STEPS-1);
                    break;
                default:
                    break;
            }
            OnValueChange();
            setSpeedText();
        }

        public void setNewSliderMax(int maxVal)
        {
            speedControlSlider.maxValue = maxVal;
        }

        public void setSpeedText()
        {
            float spd = timeSlider.PlaySpeed;
            if (spd >= MONTH_MULTIPLIER)
            {
                speedText.text = string.Format("1sec:{0:0}month(s)", spd / MONTH_MULTIPLIER);
            }
            else if (spd >= WEEK_MULTIPLIER)
            {
                speedText.text = string.Format("1sec:{0:0}week(s)", spd / WEEK_MULTIPLIER);
            }
            else if (spd >= DAY_MULTIPLIER)
            {
                speedText.text = string.Format("1sec:{0:0}day(s)", spd / DAY_MULTIPLIER);
            }
            else
            {
                speedText.text = string.Format("1sec:{0:0}hour(s)", spd);
            }
        }
        
    }
}