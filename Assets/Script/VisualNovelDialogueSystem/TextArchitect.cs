using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Reflection;
using System.Linq;
using System;


public class TextArchitect
{
    private TextMeshProUGUI tmpro_ui;
    private TextMeshPro tmpro_world;

    
    /// The assigned text component for this architect.
    public TMP_Text tmpro => tmpro_ui != null ? tmpro_ui : tmpro_world;
    /// The text built by this architect.
    public string currentText => tmpro.text;
    /// The current text that this architect is trying to build. This is excluding any pretext that might be assigned for appending text.
    public string targetText { get; private set; } = "";
    /// The full text that this architect is trying to display, including the pre text that may have existed before appending the new target text..
    public string fullTargetText => preText + targetText;
    /// The text that should exist prior to an appending build.
    public string preText { get; private set; } = "";

    /// The color that is rendering on this text architect's tmpro component.
    public Color textColor { get { return tmpro.color; } set { tmpro.color = value; } }

    /// How fast text building is determined by the speed
    public float speed { get { return baseSpeed * speedMultiplier; } set { speedMultiplier = value; } }
    private const float baseSpeed = 1;
    private float speedMultiplier = 1;

    /// How many characters will be built per frame. When used with the fade technique, this instead just multiplies the speed.
    public int charactersPerCycle { get { return speed <= 2f ? characterMultiplier : speed <= 2.5f ? characterMultiplier * 2 : characterMultiplier * 3; } }
    private int characterMultiplier = 1;

    /// if the architect is set to rush, it will display text much faster than normal.
    public bool hurryUp = false;

    /// A list of all TABuilders available to be used in the architect. TABuilders are what build the strings of text in their own unique ways depening on the selected build method.
    private Dictionary<string, Type> builders = new Dictionary<string, Type>();
    /// Stores the created Text Architect Builder for generating text here. Changes according to the build method.
    private TABuilder builder = null;
    private TABuilder.BuilderTypes _builderType;
    /// What type of builder is the architect using to reveal its text?
    public TABuilder.BuilderTypes builderType => _builderType;

    private Coroutine buildProcess = null;
    /// Is this architect building its text at the moment?
    public bool isBuilding => buildProcess != null;

    /// Create a text architect using this ui text object
    public TextArchitect(TextMeshProUGUI uiTextObject, TABuilder.BuilderTypes builderType = TABuilder.BuilderTypes.Instant)
    {
        tmpro_ui = uiTextObject;
        AddBuilderTypes();
        SetBuilderType(builderType);
    }

    /// Create a text architect using this text object
    public TextArchitect(TextMeshPro worldTextObject, TABuilder.BuilderTypes builderType = TABuilder.BuilderTypes.Instant)
    {
        tmpro_world = worldTextObject;
        AddBuilderTypes();
        SetBuilderType(builderType);
    }

    /// Get all TABuilder classes and create a dictionary of possible TABuilders for this Text Architect to use.
    private void AddBuilderTypes()
    {
        builders = Assembly.GetExecutingAssembly()
            .GetTypes()
            .Where(t => t.IsSubclassOf(typeof(TABuilder)))
            .ToDictionary(t => t.Name, t => t);
    }

    /// Update the TABuilder to the correct one given the build type.
    public void SetBuilderType(TABuilder.BuilderTypes builderType)
    {
        string name = TABuilder.CLASS_NAME_PREFIX + builderType.ToString();
        Type classType = builders[name];

        builder = Activator.CreateInstance(classType) as TABuilder;
        builder.architect = this;
        builder.onComplete += OnComplete;

        _builderType = builderType;
    }

    /// Build and display a string using this text.
    /// <param name="text"></param>
    public Coroutine Build(string text)
    {
        preText = "";
        targetText = text;

        Stop();

        buildProcess = builder.Build();
        return buildProcess;
    }

    /// Append and build a string to what is already being displayed on this text
    /// <param name="text"></param>
    public Coroutine Append(string text)
    {
        preText = currentText;
        targetText = text;

        Stop();

        buildProcess = builder.Build();
        return buildProcess;
    }

    /// Immediately apply text to the object
    /// <param name="text"></param>
    public void SetText(string text)
    {
        preText = "";
        targetText = text;

        Stop();

        tmpro.text = targetText;
        builder.ForceComplete();
    }

    /// Stop building the text. This will not finish the text, but stop it where it is immediately.
    public void Stop()
    {
        if (isBuilding)
            tmpro.StopCoroutine(buildProcess);

        buildProcess = null;
    }

    /// Stop any active build process immediately and complete the text.
    public void ForceComplete()
    {
        if (isBuilding)
            builder.ForceComplete();

        Stop();

        OnComplete();
    }

    /// This is what happens when the text has finished completing.
    private void OnComplete()
    {
        hurryUp = false;
        buildProcess = null;
    }
}
