using NodeCanvas.Framework;
using ParadoxNotion;
using ParadoxNotion.Design;
using ParadoxNotion.Services;
using System.Collections.Generic;

using UnityEngine;
using SlotMaker;

namespace NodeCanvas.Tasks.Actions{

    [Category("★ SlotMaker/Utility")]
    [Description("Screen Capture.")]
    public class SaveScreenShot : ActionTask<GraphOwner> 
    {
        public BBParameter<string> fileName;

        public BBParameter<int> quality;
        public BBParameter<float> factor;
        public BBParameter<SlotMaker.ScreenCapture.CaptureParams.ScaleType> scaleType;

        public BBParameter<string> texturePath;

        protected override string info
        {
            get {return string.Format("Screen Capture \"{0}\" Factor {1}", fileName, factor);}
        }

        protected override void OnExecute()
        {
            if(fileName != null && !string.IsNullOrEmpty(fileName.value))
            {
                texturePath.value = Application.temporaryCachePath + "/" + fileName.value;

                SlotMaker.ScreenCapture.Instance.Capture(
                    new SlotMaker.ScreenCapture.CaptureParams
                    {
                        path = texturePath.value,
                        quality = quality.value,
                        scaleType = scaleType.value,
                        scaleFactor = factor.value,
                        destroyTexture = true
                    },
                    delegate
                    {
                        EndAction();
                    }
                );
            }

            // EndAction();
        }       
    }
}
