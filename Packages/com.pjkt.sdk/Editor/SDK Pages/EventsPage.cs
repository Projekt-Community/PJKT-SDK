using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

namespace PJKT.SDK2
{
    public class EventsPage : SDKPage
    {
        private EventButton selectedEventButton;
        public override void OnTabEnable()
        {
            if (PjktEventManager.Projekts.Count <= 0 )
            {
                //no events somehow
                NoBoothsMessage message = new NoBoothsMessage();
                message.SetMessage("No events found. Check the discord to see if anything is coming up soon.");
                topArea.Add(message);
                return;
            }
            
            RefreshEventButtons();
        }
        
        private void RefreshEventButtons()
        {
            foreach (Project evt in PjktEventManager.Projekts)
            {
                if (!evt.accepting_booth) continue;
                DateTime deadline = DateTime.Parse(evt.booth_deadline_date);
                
                if (deadline < System.DateTime.Now) continue;
                
                EventButton eventButton = new EventButton(evt);
                eventButton.RegisterCallback<ClickEvent>(OnEventButtonClick);

                if (PjktEventManager.SelectedProjekt == evt)
                {
                    selectedEventButton = eventButton;
                    eventButton.ShowRequirements(true);
                    eventButton.SelectEvent();
                    topArea.Add(eventButton);
                }
                else scrollView.Add(eventButton);
            }
        }

        private void OnEventButtonClick(ClickEvent evt)
        {
            VisualElement element = evt.target as VisualElement;
            EventButton eventButton = element.GetFirstAncestorOfType<EventButton>();
            
            if (eventButton == null) return;
            if (selectedEventButton == eventButton) return;
            
            if (selectedEventButton != null)
            {
                selectedEventButton.ShowRequirements(false);
                topArea.Remove(selectedEventButton);
                scrollView.Add(selectedEventButton);
            }

            selectedEventButton = eventButton;
            selectedEventButton.ShowRequirements(true);
            selectedEventButton.SelectEvent();
            topArea.Add(selectedEventButton);
        }
    }
}