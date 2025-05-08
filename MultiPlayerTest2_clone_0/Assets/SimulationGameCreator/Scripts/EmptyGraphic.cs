using UnityEngine.UI;

namespace SimulationGameCreator
{
	public class EmptyGraphic : Graphic
	{
		protected override void OnPopulateMesh(VertexHelper vh)
		{
			vh.Clear();
		}
	}
}
