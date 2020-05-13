using UnityEngine;

namespace Klondike.Core
{
	public interface IValidArea
	{
		string SpotName { get; }
		RectTransform SpotPosition { get; }

		/// <summary>
		/// Check if the given card can be appended to the Safe Spot.
		/// </summary>
		/// <param name="cardToAppendGO">the GameObject representing the card to append</param>
		/// <returns> TRUE if the card can be appended, FALSE otherwise</returns>
		bool CanAppendCard(GameObject cardToAppendGO);

		void AppendCard(GameObject cardGO);

		void DetachCard(GameObject cardGO);
	}
}