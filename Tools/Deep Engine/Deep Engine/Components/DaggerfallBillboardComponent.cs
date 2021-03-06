﻿// Project:         Deep Engine
// Description:     3D game engine for Ruins of Hill Deep and Daggerfall Workshop projects.
// Copyright:       Copyright (C) 2012 Gavin Clayton
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Web Site:        http://www.dfworkshop.net
// Contact:         Gavin Clayton (interkarma@dfworkshop.net)
// Project Page:    http://code.google.com/p/daggerfallconnect/

#region Using Statements
using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using DaggerfallConnect;
using DaggerfallConnect.Arena2;
using DeepEngine.Core;
using DeepEngine.Daggerfall;
using DeepEngine.Rendering;
using DeepEngine.Utility;
using DeepEngine.World;
#endregion

namespace DeepEngine.Components
{

    /// <summary>
    /// Component for drawing individual billboards from Daggerfall.
    /// </summary>
    public class DaggerfallBillboardComponent : DrawableComponent
    {
        #region Fields

        BaseMaterialEffect material = null;
        Vector3 offset = Vector3.Zero;
        Vector2 size = Vector2.Zero;

        #endregion

        #region Properties

        /// <summary>
        /// Gets size of billboard.
        /// </summary>
        public Vector2 Size
        {
            get { return size; }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="core">Engine core.</param>
        public DaggerfallBillboardComponent(DeepCore core)
            : base(core)
        {
        }

        /// <summary>
        /// Constructor to load billboard texture.
        /// </summary>
        /// <param name="core">Engine core.</param>
        /// <param name="archive">Texture archive index.</param>
        /// <param name="record">Texture record index.</param>
        /// <param name="flat">Flat data.</param>
        public DaggerfallBillboardComponent(DeepCore core, DaggerfallBlockComponent.BlockFlat flat)
            : base(core)
        {
            LoadBillboard(flat);
        }

        #endregion

        #region DrawableComponent Overrides

        /// <summary>
        /// Called when component should draw itself.
        /// </summary>
        /// <param name="caller">Entity calling the draw operation.</param>
        public override void Draw(BaseEntity caller)
        {
            // Do nothing if disabled or no texture set
            if (!enabled || material == null)
                return;

            // Calculate world matrix
            Matrix worldMatrix = matrix * caller.Matrix;

            // Submit to renderer
            core.Renderer.SubmitBillboard(material, worldMatrix.Translation + offset, size);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Loads a Daggerfall texture as a billboard.
        /// </summary>
        /// <param name="archive">Texture archive.</param>
        /// <param name="record">Texture record.</param>
        /// <param name="flat">Flat data.</param>
        public void LoadBillboard(DaggerfallBlockComponent.BlockFlat flat)
        {
            // Get path to texture file
            string path = Path.Combine(
                core.MaterialManager.Arena2Path,
                TextureFile.IndexToFileName(flat.Archive));

            // Get size and scale of this texture
            System.Drawing.Size size = TextureFile.QuickSize(path, flat.Record);
            System.Drawing.Size scale = TextureFile.QuickScale(path, flat.Record);

            // Set start size
            Vector2 startSize;
            startSize.X = size.Width;
            startSize.Y = size.Height;

            // Apply scale
            Vector2 finalSize;
            int xChange = (int)(size.Width * (scale.Width / BlocksFile.ScaleDivisor));
            int yChange = (int)(size.Height * (scale.Height / BlocksFile.ScaleDivisor));
            finalSize.X = (size.Width + xChange);
            finalSize.Y = (size.Height + yChange);
            finalSize *= ModelManager.GlobalScale;

            // Load material
            this.material = core.MaterialManager.CreateDaggerfallMaterialEffect(
                flat.Archive,
                flat.Record,
                null,
                MaterialManager.DefaultBillboardFlags);

            // Save settings
            this.size = finalSize;

            // Calcuate offset for correct positioning in scene
            if (flat.Dungeon)
                this.offset = Vector3.Zero;
            else
                this.offset = new Vector3(0, (finalSize.Y / 2) - (4 * ModelManager.GlobalScale), 0);

            // Set bounding sphere
            BoundingSphere sphere;
            sphere.Center = Vector3.Zero;
            if (finalSize.X > finalSize.Y)
                sphere.Radius = finalSize.X / 2;
            else
                sphere.Radius = finalSize.Y / 2;
            this.BoundingSphere = sphere;
        }

        #endregion
    }

}
