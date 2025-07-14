using UnityEngine;

public interface IFurnitureInteraction
{
    void OnPlayerArrive();

    void OnPlayerDepart();

    float InteractionX { get; }
}
