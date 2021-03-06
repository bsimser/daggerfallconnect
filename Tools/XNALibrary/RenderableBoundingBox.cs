﻿// Project:         XNALibrary
// Description:     Explore and export 3D models from Daggerfall.
// Copyright:       Copyright (C) 2011 Gavin Clayton
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Web Site:        http://www.dfworkshop.net
// Contact:         Gavin Clayton (interkarma@dfworkshop.net)
// Project Page:    http://code.google.com/p/daggerfallconnect/

#region Using Statements
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
#endregion

namespace XNALibrary
{

    /// <summary>
    /// Renders a BoundingBox as a series of line primitives
    /// </summary>
    public class RenderableBoundingBox
    {

        #region Class Variables

        // Buffers
        VertexPositionColor[] vertexBuffer = new VertexPositionColor[8];
        int[] indexBuffer = new int[]
            {
                0, 1, 1, 2, 2, 3, 3, 0, 0, 4, 1, 5,
                2, 6, 3, 7, 4, 5, 5, 6, 6, 7, 7, 4,
            };

        // Appearance
        private Color boundingBoxColor = Color.White;

        // XNA
        private GraphicsDevice graphicsDevice;
        private VertexDeclaration lineVertexDeclaration;
        private BasicEffect lineEffect;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets BoundingBox colour.
        /// </summary>
        public Color Color
        {
            get { return boundingBoxColor; }
            set { boundingBoxColor = value; }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        public RenderableBoundingBox(GraphicsDevice graphicsDevice)
        {
            // Setup line BasicEffect
            lineEffect = new BasicEffect(graphicsDevice);
            lineEffect.LightingEnabled = false;
            lineEffect.TextureEnabled = false;
            lineEffect.VertexColorEnabled = true;

            // Create vertex declaration
            lineVertexDeclaration = new VertexDeclaration(
                VertexPositionColor.VertexDeclaration.GetVertexElements());

            // Store GraphicsDevice
            this.graphicsDevice = graphicsDevice;
        }

        /// <summary>
        /// BoundingBox constructor.
        /// </summary>
        /// <param name="boundingBox">BoundingBox.</param>
        public RenderableBoundingBox(GraphicsDevice graphicsDevice, BoundingBox boundingBox)
            : this(graphicsDevice)
        {
            SetBoundingBox(boundingBox);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Sets the BoundingBox to draw.
        /// </summary>
        /// <param name="boundingBox">BoundingBox</param>
        public void SetBoundingBox(BoundingBox boundingBox)
        {
            // Deploy corner verts to buffer
            Vector3[] corners = boundingBox.GetCorners();
            for (int i = 0; i < 8; i++)
            {
                vertexBuffer[i].Position = corners[i];
                vertexBuffer[i].Color = boundingBoxColor;
            }
        }

        /// <summary>
        /// Draw the current BoundingBox.
        /// </summary>
        /// <param name="boundingBox">New bounding box to draw.</param>
        /// <param name="viewMatrix">View matrix.</param>
        /// <param name="projectionMatrix">Projection matrix.</param>
        /// <param name="worldMatrix">World Matrix.</param>
        public void Draw(BoundingBox boundingBox, Matrix viewMatrix, Matrix projectionMatrix, Matrix worldMatrix)
        {
            // Set new bounding box and draw as normal
            SetBoundingBox(boundingBox);
            Draw(viewMatrix, projectionMatrix, worldMatrix);
        }

        /// <summary>
        /// Draw the current BoundingBox.
        /// </summary>
        /// <param name="viewMatrix">View matrix.</param>
        /// <param name="projectionMatrix">Projections matrix.</param>
        /// <param name="worldMatrix">World Matrix.</param>
        public void Draw(Matrix viewMatrix, Matrix projectionMatrix, Matrix worldMatrix)
        {
            // Set render states
            graphicsDevice.BlendState = BlendState.AlphaBlend;
            graphicsDevice.DepthStencilState = DepthStencilState.DepthRead;
            graphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;

            // Set view and projection matrices
            lineEffect.View = viewMatrix;
            lineEffect.Projection = projectionMatrix;
            lineEffect.World = worldMatrix;

            lineEffect.CurrentTechnique.Passes[0].Apply();

            graphicsDevice.DrawUserIndexedPrimitives(
                PrimitiveType.LineList,
                vertexBuffer,
                0,
                8,
                indexBuffer,
                0,
                indexBuffer.Length / 2);
        }

        #endregion

    }

}
