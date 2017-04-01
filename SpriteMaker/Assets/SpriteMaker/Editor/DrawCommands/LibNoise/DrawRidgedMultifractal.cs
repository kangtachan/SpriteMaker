﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LibNoise;
using UnityEditor;

namespace SpriteMaker
{
    public class DrawRidgedMultifractal : BaseDrawCommand
    {

        public float frequency = 0.05f;
        public float lacunarity = 0.0f;
        public float XOffset = 0.0f;
        public float YOffset = 0.0f;
        public int seed = 1;
        public int octaves = 1;

        public Gradient gradient;

        private ScriptableGradientAsset gradientObject;



        private float xPosition;
        private float yPosition;

        public override Color[] DrawToColorArray(Color[] _input, int _width, int _height)
        {

            LibNoise.Generator.RidgedMultifractal mv = new LibNoise.Generator.RidgedMultifractal(frequency, lacunarity, octaves, seed, QualityMode.High);

            int scaleX = 128;
            int scaleY = 128;

            Noise2D m = new Noise2D(_width, _height, (ModuleBase)mv);
            m.GeneratePlanar(XOffset, XOffset + scaleX, YOffset, YOffset + scaleY);
            float[,] result = m.GetNormalizedData(false, 0, 0);

            for (int y = 0; y < _height; y++)
            {
                for (int x = 0; x < _width; x++)
                {
                    _input[y * _width + x] = BlendPixelToCanvas(
                        gradient.Evaluate(
                            result[(int)(x), (int)(y)])
                        , _input[y * _width + x]);
                }
            }

            return base.DrawToColorArray(_input, _width, _height);
        }

        public override void DrawControls()
        {
            Name = "Ridged Multifractal Noise";
            frequency = EditorGUILayout.FloatField("Frequency", frequency);
            lacunarity = EditorGUILayout.FloatField("Lacunarity", lacunarity);
            XOffset = EditorGUILayout.FloatField("X Offset", XOffset);
            YOffset = EditorGUILayout.FloatField("Y Offset", YOffset);
            seed = EditorGUILayout.IntField("Seed", seed);
            octaves = EditorGUILayout.IntField("Octaves", octaves);


            if (gradientObject == null)
            {
                gradientObject = ScriptableObject.CreateInstance<ScriptableGradientAsset>();
                gradientObject.gradient = gradient;
            }

            EditorGUI.BeginChangeCheck();
            {
                SerializedObject obj = new SerializedObject(gradientObject);
                SerializedProperty sgrad = obj.FindProperty("gradient");
                EditorGUILayout.PropertyField(sgrad, true, null);
                if (EditorGUI.EndChangeCheck())
                {
                    obj.ApplyModifiedProperties();
                    gradient = gradientObject.gradient;
                }

            }


            base.DrawControls();
        }

        #region SERIALIZATION

        public override void OnBeforeSerialize()
        {
            data = new SerializedData();

            data.serializedFloats = new float[6];
            data.serializedFloats[0] = frequency;
            data.serializedFloats[1] = lacunarity;
            data.serializedFloats[2] = XOffset;
            data.serializedFloats[3] = YOffset;
            data.serializedFloats[4] = (float)seed;
            data.serializedFloats[5] = (float)octaves;



            data.serializedGradients = new Gradient[1];
            data.serializedGradients[0] = gradient;


            base.OnBeforeSerialize();
        }

        public override void PopulateFromBase(BaseDrawCommand bd)
        {
            frequency = bd.data.serializedFloats[0];
            lacunarity = bd.data.serializedFloats[1];
            XOffset = bd.data.serializedFloats[2];
            YOffset = bd.data.serializedFloats[3];
            seed = (int)bd.data.serializedFloats[4];
            octaves = (int)bd.data.serializedFloats[5];


            gradient = bd.data.serializedGradients[0];





            base.PopulateFromBase(bd);
        }

        #endregion


    }
}