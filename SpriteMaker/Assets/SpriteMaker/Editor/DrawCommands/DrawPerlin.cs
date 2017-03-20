﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace SpriteMaker
{

    public class DrawPerlin : BaseDrawCommand
    {
        public float HorizontalScale = 6.4f;
        public float VerticalScale = 6.4f;
        public float xOffset;
        public float yOffset;
        public Color FirstColor = Color.clear;
        public Color SecondColor = Color.white;

        private float xPosition;
        private float yPosition;

        public override Color[] DrawToColorArray(Color[] _input, int _width, int _height)
        {
            for (int y = 0; y < _height; y++)
            {
                yPosition = (float)y / (float)_height;
                for (int x = 0; x < _width; x++)
                {
                    xPosition = (float)x / (float)_width;

                    _input[y * _width + x] = BlendPixelToCanvas(
                        Color.Lerp(FirstColor, SecondColor,
                            Mathf.PerlinNoise(
                                (xPosition * HorizontalScale) + xOffset, 
                                (yPosition * VerticalScale) + yOffset
                            ))
                        , _input[y * _width + x]);
                }
            }


            return base.DrawToColorArray(_input, _width, _height);
        }

        public override void DrawControls()
        {
            Name = "Perlin Noise";
            HorizontalScale = float.Parse(EditorGUILayout.TextField("Horizontal Scale", HorizontalScale.ToString()));
            VerticalScale = float.Parse(EditorGUILayout.TextField("Vertical Scale", VerticalScale.ToString()));
            FirstColor = EditorGUILayout.ColorField("First Color", FirstColor);
            SecondColor = EditorGUILayout.ColorField("Second Color", SecondColor);

            if (GUILayout.Button("Shuffle"))
            {
                xOffset = Random.Range(-9999, 9999);
                yOffset = Random.Range(-9999, 9999);
            }
            base.DrawControls();
        }
	
    }
}