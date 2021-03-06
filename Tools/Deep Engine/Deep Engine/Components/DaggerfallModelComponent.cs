﻿// Project:         Deep Engine
// Description:     3D game engine for Ruins of Hill Deep and Daggerfall Workshop projects.
// Copyright:       Copyright (C) 2012 Gavin Clayton
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Web Site:        http://www.dfworkshop.net
// Contact:         Gavin Clayton (interkarma@dfworkshop.net)
// Project Page:    http://code.google.com/p/daggerfallconnect/

#region Using Statements
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using DeepEngine.Core;
using DeepEngine.Daggerfall;
using DeepEngine.World;
using DeepEngine.Utility;
#endregion

namespace DeepEngine.Components
{

    /// <summary>
    /// A lightweight component for drawing a single Daggerfall model.
    /// </summary>
    public class DaggerfallModelComponent : DrawableComponent
    {

        #region Fields

        uint id;
        ModelManager.ModelData modelData;
        BoundingBox boundingBox;

        #endregion

        #region Properties

        /// <summary>
        /// Gets ID of currently loaded model.
        /// </summary>
        public uint ModelID
        {
            get { return id; }
        }

        /// <summary>
        /// Gets reference to loaded model.
        /// </summary>
        public ModelManager.ModelData ModelData
        {
            get { return modelData; }
        }

        /// <summary>
        /// Gets model bounding box transformed by matrix.
        /// </summary>
        public BoundingBox BoundingBox
        {
            get
            {
                BoundingBox box;
                box.Min = Vector3.Transform(boundingBox.Min, matrix);
                box.Max = Vector3.Transform(boundingBox.Max, matrix);
                return box;
            }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="core">Engine core.</param>
        /// <param name="id">Model ID to load.</param>
        public DaggerfallModelComponent(DeepCore core, uint id)
            : base(core)
        {
            // Load model
            LoadModel(id);
            this.id = id;
        }

        #endregion

        #region DrawableComponents Overrides

        /// <summary>
        /// Draws component.
        /// </summary>
        /// <param name="caller">Entity calling the draw operation.</param>
        public override void Draw(BaseEntity caller)
        {
            // Do nothing if disabled or no model data
            if (!enabled || modelData == null)
                return;

            // Exit if model data is empty
            if (modelData.SubMeshes == null)
                return;

            // Calculate world matrix
            Matrix worldMatrix = matrix * caller.Matrix;

            // Set transforms
            core.ModelManager.ModelEffect_World = worldMatrix;
            core.ModelManager.ModelEffect_View = core.ActiveScene.Camera.ViewMatrix;
            core.ModelManager.ModelEffect_Projection = core.ActiveScene.Camera.ProjectionMatrix;

            // Set buffers
            core.GraphicsDevice.SetVertexBuffer(modelData.VertexBuffer);
            core.GraphicsDevice.Indices = modelData.IndexBuffer;

            // Draw batches
            foreach (var sm in modelData.SubMeshes)
            {
                // Apply material
                BaseMaterialEffect materialEffect = core.MaterialManager.GetMaterialEffect(sm.MaterialKey);
                materialEffect.Apply();

                // Render geometry
                foreach (EffectPass pass in materialEffect.Effect.CurrentTechnique.Passes)
                {
                    // Apply effect pass
                    pass.Apply();

                    // Draw indexed primitives
                    core.GraphicsDevice.DrawIndexedPrimitives(
                        PrimitiveType.TriangleList,
                        0,
                        0,
                        modelData.VertexBuffer.VertexCount,
                        sm.StartIndex,
                        sm.PrimitiveCount);
                }
            }
        }

        /// <summary>
        /// Frees resources used by this object when they are no longer needed.
        /// </summary>
        public override void Dispose()
        {
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Loads a new model to display with this component.
        ///  Replaces any previously loaded model.
        /// </summary>
        /// <param name="id">Model ID to load.</param>
        /// <returns>True if successful.</returns>
        public bool LoadModel(uint id)
        {
            try
            {
                // Load model data and set bounds
                modelData = core.ModelManager.GetModelData(id);
                this.BoundingSphere = modelData.BoundingSphere;
                this.boundingBox = modelData.BoundingBox;
                this.id = id;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                modelData = null;
                return false;
            }

            return true;
        }

        #endregion

    }

}
