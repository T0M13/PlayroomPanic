using UnityEngine;

[System.Serializable]
public class Personality
{
    // Playfulness: Increases the likelihood of engaging in playful activities, such as running around, playing with toys, or interacting with other children.
    [Tooltip("Increases the likelihood of engaging in playful activities, such as running around, playing with toys, or interacting with other children.")]
    [Range(0, 1)] public float playfulness = 0.5f;

    // Stubbornness: Makes children resist instructions or refuse to participate in activities.
    [Tooltip("Makes children resist instructions or refuse to participate in activities.")]
    [Range(0, 1)] public float stubbornness = 0.5f;

    // Curiosity: Increases the tendency to explore new areas or interact with new objects.
    [Tooltip("Increases the tendency to explore new areas or interact with new objects.")]
    [Range(0, 1)] public float curiosity = 0.5f;

    // Attention-seeking: Increases the likelihood of the child trying to get the attention of the player or other children.
    [Tooltip("Increases the likelihood of trying to get the attention of the player or other children.")]
    [Range(0, 1)] public float attentionSeeking = 0.5f;

    // Timidness: Causes the child to shy away from loud noises, new people, or unfamiliar situations.
    [Tooltip("Causes the child to shy away from loud noises, new people, or unfamiliar situations.")]
    [Range(0, 1)] public float timidness = 0.5f;

    // Energy Levels: Determines how quickly a child gets tired or how much they need to rest.
    [Tooltip("Determines how quickly a child gets tired or how much they need to rest.")]
    [Range(0, 1)] public float energyLevels = 0.5f;

    // Sociality: Influences how much the child enjoys being around others.
    [Tooltip("Influences how much the child enjoys being around others.")]
    [Range(0, 1)] public float sociality = 0.5f;

    // Messiness: Tendency to make a mess with toys, food, or art supplies.
    [Tooltip("Tendency to make a mess with toys, food, or art supplies.")]
    [Range(0, 1)] public float messiness = 0.5f;

    // Imagination: Leads to engaging in creative play, often making up stories or organizing imaginary games with other children.
    [Tooltip("Leads to engaging in creative play, often making up stories or organizing imaginary games.")]
    [Range(0, 1)] public float imagination = 0.5f;

    // Impulsiveness: Causes spontaneous and unpredictable actions, like suddenly running off, throwing toys, or grabbing items from others.
    [Tooltip("Causes spontaneous and unpredictable actions, like suddenly running off, throwing toys, or grabbing items from others.")]
    [Range(0, 1)] public float impulsiveness = 0.5f;

    // Function to randomize all traits within a given range
    public void RandomizeTraits(float minValue = 0.1f, float maxValue = 1f)
    {
        playfulness = Random.Range(minValue, maxValue);
        stubbornness = Random.Range(minValue, maxValue);
        curiosity = Random.Range(minValue, maxValue);
        attentionSeeking = Random.Range(minValue, maxValue);
        timidness = Random.Range(minValue, maxValue);
        energyLevels = Random.Range(minValue, maxValue);
        sociality = Random.Range(minValue, maxValue);
        messiness = Random.Range(minValue, maxValue);
        imagination = Random.Range(minValue, maxValue);
        impulsiveness = Random.Range(minValue, maxValue);
    }

    // Function to reset all traits to their default values
    public void ResetTraitsToDefault()
    {
        playfulness = 0.5f;
        stubbornness = 0.5f;
        curiosity = 0.5f;
        attentionSeeking = 0.5f;
        timidness = 0.5f;
        energyLevels = 0.5f;
        sociality = 0.5f;
        messiness = 0.5f;
        imagination = 0.5f;
        impulsiveness = 0.5f;
    }
}
