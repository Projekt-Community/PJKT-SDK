using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace PJKT.SDK2
{
    public class AnimationsPage : SDKPage
    {
        private RequirementsInfo requirementsInfo;

        public override void OnTabEnable()
        {
            style.display = DisplayStyle.Flex;
            
            if (BoothValidator.BoothsInScene.Length == 0)
            {
                topArea.Add(new NoBoothsMessage());
                return;
            }
            
            //reports
            BoothStats animators = BoothValidator.Report.GetStats(StatsType.Animators);
            BoothStats animClips = BoothValidator.Report.GetStats(StatsType.AnimationClips);
            
            //top info area
            requirementsInfo = new RequirementsInfo(new BoothStats[] {animators, animClips});
            topArea.Add(requirementsInfo);
            
            if (BoothValidator.SelectedBooth == null) return;
            
            //fill out info box
            foreach (AnimatorInfo info in animators.ComponentList)
            {
                string animatorInfo = $"Controller: " + (info.Controller != null ? info.Controller.name : "No controller assigned") + '\n'
                                      + $"Layers: {info.Layers} \n"
                                      + $"States: {info.States} \n"
                                      + $"Any State Transitions: {info.AnyStateTransitions} \n"
                                      + $"Animationclips: {info.Clips.Count}\n"
                                      + "\n<b>Clips:</b>";
                
                foreach (AnimationClip clip in info.Clips) animatorInfo += $"\n Animation clip: {clip.name}";
                
                animatorInfo += "\n\n<b>Parameters:</b>";
                foreach (Tuple<string, AnimatorControllerParameterType> parameter in info.Parameters) animatorInfo += $"\n Parameter: {parameter.Item1} - {parameter.Item2}";
                
                InfoPanel panel = new InfoPanel(info.Animator.gameObject, AssetPreview.GetMiniTypeThumbnail(typeof(Animator)), "Animator", animatorInfo, PjktGraphics.GraphicColors["Text"]);
                scrollView.Add(panel);
            }
            
            foreach (AnimationClip clip in animClips.ComponentList)
            {
                string clipInfo = $"Name: {clip.name} \n"
                                  + $"Length: {clip.length} \n"
                                  + $"Frame Rate: {clip.frameRate}";
                InfoPanel panel = new InfoPanel(clip, AssetPreview.GetMiniTypeThumbnail(typeof(AnimationClip)), "Animation Clip", clipInfo, PjktGraphics.GraphicColors["Animation"]);
                scrollView.Add(panel);
            }
        }
    }
}