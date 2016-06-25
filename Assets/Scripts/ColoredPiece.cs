using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class ColoredPiece : MonoBehaviour {

    public enum ColorType
    {
        YELLOW,
        PURPLE,
        RED,
        BLUE,
        GREEN,
        PINK,
        ANY,
        COUNT
    }

    private ColorType color;

    [System.Serializable]
    public struct ColorSprite
    {
        public ColorType color;
        public Sprite sprite;
    };

    public ColorSprite[] colorSprites;

    private Dictionary<ColorType, Sprite> colorDictionary;
    private SpriteRenderer spriteRenderer;

    public ColorType Color
    {
        get
        {
            return color;
        }

        set
        {
           SetColor(value);
        }
    }

    void Awake()
    {
        spriteRenderer = (SpriteRenderer) transform.Find("piece").GetComponent<SpriteRenderer>();

        colorDictionary = new Dictionary<ColorType, Sprite>();
        foreach (ColorSprite colorSprite in colorSprites)
        {
            if (!colorDictionary.ContainsKey(colorSprite.color))
            {
                colorDictionary.Add(colorSprite.color, colorSprite.sprite);
            }
        }
    }

    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void SetColor(ColorType newColor)
    {
        this.color = newColor;

        if (colorDictionary.ContainsKey(newColor))
        {
            spriteRenderer.sprite = colorDictionary [newColor];
        }
    }

    public int NumberOfColors()
    {
        return colorSprites.Length;
    }
}
