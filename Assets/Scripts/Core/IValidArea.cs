using UnityEngine;

namespace Klondike.Core
{
	public interface IValidArea
	{

		string SpotName { get; }

		Vector2 SpotPosition { get; }

		void DetachCard(GameObject cardGO);

		void AppendCard(GameObject cardGO);

		/// <summary>
		/// Check if the given card can be appended to the Safe Spot.
		/// </summary>
		/// <param name="cardToAppend">the card to append</param>
		/// <returns> TRUE if the card can be appended, FALSE otherwise</returns>
		bool CanAppendCard(PlayableCard cardToAppend);
	}
}