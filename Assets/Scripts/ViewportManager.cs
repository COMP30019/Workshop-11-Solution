// COMP30019 - Graphics and Interaction
// (c) University of Melbourne, 2023

using UnityEngine;

public class ViewportManager : MonoBehaviour
{
    [SerializeField] private Camera viewportCamera;
    [SerializeField] private ParticleSystem atmosphereParticles;

    private static ViewportManager _instance;

    public static Camera Camera => _instance.viewportCamera;

    private void Awake()
    {
        // Max one viewport manager per scene (persistent across scene changes).
        if (_instance != null)
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);

        // Once created we set it as the static instance so it can be directly
        // referenced from any script. This is sometimes known as the
        // "singleton pattern", and should be used sparingly since it can lead
        // to spaghetti code. 
        //
        // Here we are illustrating one valid use case for it since the same
        // viewport persists across the entire game, and there is only ever
        // going to be one. Other valid use cases for singletons include
        // managers that are responsible for managing a single resource (e.g. a
        // "SoundManager" that manages all audio in the game). 
        //
        // Theoretically the GameManager could also be a singleton, but it
        // contains mutable state and is (arguably) better off decoupled.
        // Ultimately there are no hard and fast rules, and it's up to you to
        // decide what works best for individual projects.
        _instance = this;
    }
}
