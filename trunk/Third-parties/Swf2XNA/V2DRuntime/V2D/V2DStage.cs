using DDW.Display;
using DDW.V2D.Serialization;

namespace DDW.V2D
{
    public class V2DStage : Stage
    {
        public V2DWorld v2dWorld;
    

        public V2DStage()
        {
        }

		public override void AddChild(DisplayObject o)
		{
			base.AddChild(o);
		}

		public override void SetBounds(float x, float y, float w, float h)
		{
            // curScreen may be null if there are only persistant screens added (huds etc)
            if (curScreen != null)
            {
                curScreen.SetBounds(x, y, w, h);
            }
		}
    }
}
