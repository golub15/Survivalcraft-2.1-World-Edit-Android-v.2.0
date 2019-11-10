using Engine;
using Game;
using System.Xml.Linq;
using System;
using System.Collections.Generic;
using System.Collections;

namespace API_WE_Mod
{
    public class FastRun : Dialog
    {

        private ComponentPlayer player;
        // BlockIconWidget s;

        private LabelWidget m_title;
        private SliderWidget m_speed;



        private ButtonWidget plusButton;
        private ButtonWidget minusButton;



        private ButtonWidget m_okButton;
        private ButtonWidget m_cancelButton;

  
        private int Speed = 1;




        public FastRun(ComponentPlayer player)
        {
        

           
            this.player = player;
            WidgetsManager.LoadWidgetContents(this, this, ContentManager.Get<XElement>("WE/DialogsWE/FastRunDialog"));
            m_title = this.Children.Find<LabelWidget>("Dialog.Title", true);



           m_speed = this.Children.Find<SliderWidget>("Dialog.Slider1", true);

            plusButton = this.Children.Find<ButtonWidget>("Dialog.Button4", true);
            minusButton = this.Children.Find<ButtonWidget>("Dialog.Button3", true);

            m_okButton = this.Children.Find<ButtonWidget>("Dialog.OK", true);
            m_cancelButton = this.Children.Find<ButtonWidget>("Dialog.Cancel", true);


            m_speed.MinValue = 0f;
            m_speed.MaxValue = 100;
            m_speed.Value = ComponentWE5.speed;

            m_title.Text = "FastRun";


        }

        public override void Update()
        {


            if (plusButton.IsClicked)
            {
                m_speed.Value = MathUtils.Min(m_speed.Value + 1f, (int)m_speed.MaxValue);
            }

            if (minusButton.IsClicked)
            {
                m_speed.Value = MathUtils.Max(m_speed.Value - 1f, (int)m_speed.MinValue);
            }

            Speed = (int)m_speed.Value;
      

            if (this.m_okButton.IsClicked)
            {


                ComponentWE5.speed = Speed;
              

                DialogsManager.HideDialog(this);
            }

            if ( this.m_cancelButton.IsClicked)
            {
                DialogsManager.HideDialog((Dialog)this);
            }

            UpdateControls();

        }

        private void UpdateControls()
        {
    
            m_speed.Text = string.Format("{0}", Speed);



        }





      



    }
}